using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.Pool;

namespace Unity.GraphToolkit.Editor.Implementation
{
    [Serializable]
    class GraphModelImp : GraphModel
    {
        List<Type> m_SupportedTypes;
        IReadOnlyList<Type> m_SupportedNodes;

        [SerializeReference]
        Graph m_Graph;

        [NonSerialized]
        UndoStateComponent m_CurrentUndoStateComponent;

        [NonSerialized]
        GraphModelStateComponent.StateUpdater m_CurrentGraphModelStateUpdater;

        [NonSerialized]
        List<INode> m_Nodes;

        public Graph Graph => m_Graph;

        public override void OnEnable()
        {
            var graphObject = GraphObject as GraphObjectImp;
            if (graphObject != null && m_Graph == null)
            {
                var graphType = graphObject.GraphType;
                if (graphType != null)
                {
                    m_Graph = (Graph)Activator.CreateInstance(graphType);
                }
            }

            foreach (var variable in VariableDeclarations)
            {
                if( VariableDeclarationRequiresInitialization(variable) && variable.InitializationModel == null )
                {
                    variable.CreateInitializationValue();
                }
            }

            if (m_Graph != null)
            {
                m_Graph.SetImplementation(this);

                base.OnEnable();

                m_Graph.OnEnable();

                foreach (var nodeModel in NodeModels)
                {
                    if (nodeModel is IUserNodeModelImp userNodeModelImp)
                    {
                        userNodeModelImp.CallOnEnable();
                    }
                }
            }
        }

        public override void OnDisable()
        {
            foreach (var nodeModel in NodeModels)
            {
                if (nodeModel is IUserNodeModelImp userNodeModelImp)
                {
                    userNodeModelImp.CallOnDisable();
                }
            }
            m_Graph?.OnDisable();
            base.OnDisable();
        }

        public IReadOnlyList<IVariable> VariableModels => this.VariableDeclarations;

        protected override Type VariableNodeType => typeof(VariableNodeModelImp);
        protected override Type SubgraphNodeType => typeof(SubgraphNodeModelImp);
        protected override Type ConstantNodeType => typeof(ConstantNodeModelImp);

        public override bool CanAssignTo(PortModel destination, PortModel source)
        {
            if(destination.DataTypeHandle == TypeHandle.ExecutionFlow)
                return source.DataTypeHandle == TypeHandle.ExecutionFlow;
            return destination.PortDataType.IsAssignableFrom(source.PortDataType);
        }

        public NodeModel CreateNodeModel(Node node, Vector2 position)
        {

            if (node is ContextNode contextNode)
            {
                return CreateNode<UserContextNodeModelImp>(position : position, initializationCallback:n =>n.InitCustomNode(contextNode));
            }

            if (node is BlockNode blockNode)
            {
                return CreateNode<UserBlockNodeModelImp>(initializationCallback:n =>n.InitCustomNode(blockNode));
            }
            return CreateNode<UserNodeModelImp>(position : position, initializationCallback:n =>n.InitCustomNode(node));
        }

        //public override object ToolbarActionsObject => Graph;

        public IReadOnlyList<INode> Nodes
        {
            get
            {
                BuildNodesFromNodeModels();
                return m_Nodes;
            }
        }

        public override bool VariableDeclarationRequiresInitialization(VariableDeclarationModelBase _)
        {
            // We want all variables to have a default value field.
            return true;
        }

        public void RegisterUndo(string actionName )
        {
            if (m_CurrentUndoStateComponent != null)
                return;

            var window = GraphViewEditorWindowImp.GetOpenedWindow((GraphObjectImp)GraphObject);

            if (window?.GraphTool?.UndoState != null && window.GraphView?.GraphModel == this)
            {
                m_CurrentUndoStateComponent = window.GraphTool.UndoState;
                m_CurrentUndoStateComponent.BeginOperation(actionName);
                using (var undoStateUpdater = m_CurrentUndoStateComponent.UpdateScope)
                {
                    undoStateUpdater.SaveState(window.GraphView.GraphViewModel.GraphModelState);
                }

                PushNewGraphChangeDescription();
                m_CurrentGraphModelStateUpdater = window.GraphView.GraphViewModel.GraphModelState.UpdateScope;
            }
            else
            {
                Undo.RegisterCompleteObjectUndo(GraphObject, actionName);
            }
        }

        public override Constant CreateConstantValue(TypeHandle constantTypeHandle)
        {
            try
            {
                return base.CreateConstantValue(constantTypeHandle);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return null;
        }

        public void EndUndo()
        {
            try
            {
                if (m_CurrentUndoStateComponent != null)
                {
                    m_CurrentGraphModelStateUpdater.MarkUpdated(CurrentGraphChangeDescription);
                    m_CurrentGraphModelStateUpdater.Dispose();
                    PopGraphChangeDescription();
                    m_CurrentUndoStateComponent.EndOperation();
                }
            }
            finally
            {
                m_CurrentUndoStateComponent = null;
            }
        }

        protected override void CreateGraphProcessors()
        {
            base.CreateGraphProcessors();

            var overridden = Graph.GetType().GetMethod(nameof(GraphToolkit.Editor.Graph.OnGraphChanged)).DeclaringType != typeof(Graph);

            if( overridden )
                GetGraphProcessorContainer().AddGraphProcessor(new GraphProcessorImp(this));
        }

        public IVariable CreateVariable(string name, Type valueType, object defaultValue = null, VariableKind kind = VariableKind.Local)
        {
            var typeHandle = valueType.GenerateTypeHandle();

            var constant = CreateConstantValue(typeHandle);
            if( defaultValue != null )
                constant.ObjectValue = defaultValue;

            return CreateGraphVariableDeclaration(typeHandle, name, kind == VariableKind.Input ?ModifierFlags.Read : (kind == VariableKind.Output ? ModifierFlags.Write : ModifierFlags.None), (kind != VariableKind.Local)?VariableScope.Exposed:VariableScope.Local, initializationModel:constant);
        }

        public bool DeleteWiresBetween(IPort output, IPort input)
        {
            if (input.direction == output.direction)
            {
                return false;
            }
            if( output.direction == PortDirection.Input )
            {
                (output, input) = (input, output);
            }
            using var dispose = ListPool<GraphElementModel>.Get( out var elementsToDelete);
            foreach (var wire in WireModels)
            {
                bool sameInput = wire.ToPort == input;
                bool sameOutput = wire.ToPort == output;

                if (sameInput && sameOutput)
                {
                    elementsToDelete.Add(wire);
                }
                else if (sameInput || sameOutput) // Check if ports are connected through a portal.
                {
                    var otherWirePort = sameInput ? wire.FromPort : wire.ToPort;
                    var otherPortToDelete = sameInput ? output : input;
                    if (otherWirePort.NodeModel is WirePortalModel portalModel)
                    {
                        var otherPortals = sameInput ? GetExitPortals(portalModel.DeclarationModel) : GetEntryPortals(portalModel.DeclarationModel);

                        foreach( var otherPortal in otherPortals)
                        {
                            var otherPortalPort = otherPortal is WirePortalEntryModel entryPortal ? entryPortal.InputPort : ((WirePortalExitModel)otherPortal).OutputPort;

                            foreach (var wireOnTheOtherSideOfPortal in otherPortalPort.GetConnectedWires())
                            {
                                if (wireOnTheOtherSideOfPortal.FromPort == otherPortToDelete || wireOnTheOtherSideOfPortal.ToPort == otherPortToDelete)
                                {
                                    elementsToDelete.Add(wire);
                                    elementsToDelete.Add(wireOnTheOtherSideOfPortal);

                                    // If there is only one entry portal and one exit portal, and they are only connected to one wire (the one we are asked to delete), we delete them as well.
                                    if (otherPortalPort.GetConnectedWires().Count == 1 && otherPortals.Count == 1)
                                    {
                                        var samePortals = sameInput ? GetEntryPortals(portalModel.DeclarationModel) : GetExitPortals(portalModel.DeclarationModel);
                                        if (samePortals.Count == 1)
                                        {
                                            var samePortalPort = portalModel is WirePortalEntryModel entryPortal2 ? entryPortal2.InputPort : ((WirePortalExitModel)otherPortal).OutputPort;
                                            if (samePortalPort.GetConnectedWires().Count == 1)
                                            {
                                                elementsToDelete.Add(samePortals[0]);
                                                elementsToDelete.Add(otherPortals[0]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (elementsToDelete.Count > 0)
            {
                DeleteElements(elementsToDelete);
                return true;
            }

            return false;
        }

        public override bool CanPasteNode(AbstractNodeModel originalModel)
        {
            switch (originalModel)
            {
                case IUserNodeModelImp customNodeModel:
                    return IsNodeCompatible(customNodeModel.Node);

                case VariableNodeModel variableNodeModel:
                    return variableNodeModel.VariableDeclarationModel.GetType() == typeof(VariableDeclarationModel) && SupportedTypes.Contains(variableNodeModel.VariableDeclarationModel.DataType.Resolve());

                case ConstantNodeModel constantNodeModel:
                    return SupportedTypes.Contains(constantNodeModel.Type);

                case SubgraphNodeModel subgraphNodeModel:
                    var subgraph = (subgraphNodeModel.GetSubgraphModel() as GraphModelImp)?.Graph;
                    if (subgraph == null )
                        return false;

                    var subgraphTypes = PublicGraphFactory.GetSubGraphTypes(Graph.GetType());

                    foreach (var subgraphType in subgraphTypes)
                    {
                        if (subgraphType.IsInstanceOfType(subgraph))
                            return true;
                    }

                    return false;
            }

            return false;
        }

        bool IsNodeCompatible(Node node)
        {
            var graphType = m_Graph.GetType();
            if (node.GetType().GetCustomAttribute<UseWithGraphAttribute>()?.IsGraphTypeSupported(graphType) == true )
            {
                return true;
            }
            var attribute = graphType.GetCustomAttribute<GraphAttribute>();
            if (attribute?.options.HasFlag(GraphOptions.AutoIncludeNodesFromGraphAssembly) == true && node.GetType().Assembly == graphType.Assembly)
            {
                return true;
            }

            return false;
        }

        public override bool CanPasteVariable(VariableDeclarationModelBase originalModel)
        {
            return originalModel is VariableDeclarationModel && SupportedTypes.Contains(originalModel.DataType.Resolve());
        }

        public override bool CanBeDroppedInOtherGraph(GraphModel otherGraph)
        {
            if (otherGraph is GraphModelImp otherGraphModelImp)
            {
                var validSubgraphTypesForOtherGraph = PublicGraphFactory.GetSubGraphTypes(otherGraphModelImp.Graph.GetType());
                var droppedGraphType = Graph.GetType();
                return validSubgraphTypesForOtherGraph.Contains(droppedGraphType);
            }

            return false;
        }

        void BuildNodesFromNodeModels()
        {
            if (m_Nodes == null)
            {
                m_Nodes = new List<INode>( NodeModels.Count);

                foreach (var nodeModel in NodeModels)
                {
                    AddNodeFromNodeModel(nodeModel);
                }
            }
        }

        void AddNodeFromNodeModel(AbstractNodeModel nodeModel)
        {
            if( nodeModel is IUserNodeModelImp imp)
                m_Nodes.Add(imp.Node);
            else if( nodeModel is IVariableNode || nodeModel is IConstantNode || nodeModel is ISubgraphNode)
            {
                m_Nodes.Add((INode)nodeModel);
            }
        }

        protected override void AddNode(AbstractNodeModel nodeModel)
        {
            BuildNodesFromNodeModels();
            base.AddNode(nodeModel);
            AddNodeFromNodeModel(nodeModel);
        }

        public IConstantNode CreateConstantNode(string name, Vector2 position, Type valueType, object defaultValue = null)
        {
            return ((ConstantNodeModelImp)CreateConstantNode(valueType.GenerateTypeHandle(), name, position, initializationCallback: n => n.Value.ObjectValue = defaultValue));
        }

        protected override void RemoveNode(AbstractNodeModel nodeModel)
        {
            if (m_Nodes != null)
            {
                if (nodeModel is IUserNodeModelImp imp)
                {
                    m_Nodes.Remove(imp.Node);
                    imp.CallOnDisable();
                }
                else if( nodeModel is IVariableNode || nodeModel is IConstantNode || nodeModel is ISubgraphNode )
                    m_Nodes.Remove((INode)nodeModel);
            }

            base.RemoveNode(nodeModel);
        }

        public IReadOnlyList<Type> SupportedTypes
        {
            get
            {
                if (m_SupportedTypes == null)
                {
                    InitializeSupportedTypes();
                }

                return m_SupportedTypes;
            }
        }

        public IReadOnlyList<Type> SupportedNodes => m_SupportedNodes ??= PublicGraphFactory.GetNodeTypes(m_Graph.GetType());

        internal static GraphElementModel CreateContextNodeFromData(IGraphNodeCreationData nodeCreationData, Type customNodeType)
        {
            return nodeCreationData.CreateNode(UserNodeHelper.GetNodeImpType(customNodeType),
                string.Empty,
                n => ((UserContextNodeModelImp)n).InitCustomNode((ContextNode)Activator.CreateInstance(customNodeType)));
        }

        internal static GraphElementModel CreateNodeFromData(IGraphNodeCreationData nodeCreationData, Type customNodeType)
        {
            return nodeCreationData.CreateNode(UserNodeHelper.GetNodeImpType(customNodeType),
                string.Empty,
                n => ((UserNodeModelImp)n).InitCustomNode((Node)Activator.CreateInstance(customNodeType)));
        }

        internal static GraphElementModel CreateContextFromBlockData(IGraphNodeCreationData nodeCreationData, Type blockType, Type contextType)
        {
            Action<AbstractNodeModel> initializationCallback = n => ((UserBlockNodeModelImp)n).InitCustomNode((BlockNode)Activator.CreateInstance(blockType));

            if (nodeCreationData is GraphBlockCreationData blockData)
                return blockData.ContextNodeModel.CreateAndInsertBlock(
                    typeof(UserBlockNodeModelImp), blockData.OrderInContext, nodeCreationData.Guid, initializationCallback, nodeCreationData.SpawnFlags);

            //This code path is only meant to display the block in the Item Library
            if (nodeCreationData.SpawnFlags != SpawnFlags.Orphan)
                return null;

            var context = nodeCreationData.GraphModel.CreateNode(typeof(UserContextNodeModelImp) , "Dummy Context", nodeCreationData.Position, nodeCreationData.Guid,
                n => ((UserContextNodeModelImp)n).InitCustomNode((ContextNode)Activator.CreateInstance(contextType)), nodeCreationData.SpawnFlags);
            (context as ContextNodeModel)?.CreateAndInsertBlock(typeof(UserBlockNodeModelImp), -1, nodeCreationData.Guid, initializationCallback, nodeCreationData.SpawnFlags);

            return context;
        }

        public class DummyContext : ContextNode
        {}

        void InitializeSupportedTypes()
        {
            m_SupportedTypes = new List<Type>();
            var supportedTypes = new HashSet<Type>();
            var nodeCreationData = new GraphNodeCreationData(this, Vector2.zero, SpawnFlags.Orphan);

            foreach (var type in SupportedNodes)
            {
                if (type.IsAbstract)
                    return;

                IUserNodeModelImp createdElement;

                bool isBlock = typeof(BlockNode).IsAssignableFrom(type);

                if (isBlock)
                {
                    var context = (UserContextNodeModelImp)CreateContextFromBlockData(nodeCreationData, type, typeof(DummyContext));

                    createdElement = (IUserNodeModelImp)context.blocks[0].m_Implementation;
                }
                else if (typeof(ContextNode).IsAssignableFrom(type))
                    createdElement = (IUserNodeModelImp)CreateContextNodeFromData(nodeCreationData, type);
                else
                    createdElement = (IUserNodeModelImp)CreateNodeFromData(nodeCreationData, type);

                foreach (var input in createdElement.Node.GetInputPorts())
                {
                    if (input.dataType != null)
                        supportedTypes.Add(input.dataType);
                }

                foreach (var output in createdElement.Node.GetOutputPorts())
                {
                    if (output.dataType != null)
                        supportedTypes.Add(output.dataType);
                }

                createdElement.CallOnDisable();
            }

            m_SupportedTypes.AddRange(supportedTypes);
            m_SupportedTypes.Sort((a,b)=> Comparer<string>.Default.Compare(a.Name,b.Name));
        }

        public void RecreateGraph(Type graphType)
        {
            // This will recreate the graph. Can be called when pasting or creating a local subgraph, where the graph is originally
            // the graph type of the GraphObjectImp, but we want to use a different graph type.
            m_Graph = (Graph)Activator.CreateInstance(graphType);
            m_Graph.SetImplementation(this);
        }

        public override void CloneGraph(GraphModel sourceGraphModel, bool keepVariableDeclarationGuids = false)
        {
            base.CloneGraph(sourceGraphModel, keepVariableDeclarationGuids);

            if (sourceGraphModel is GraphModelImp sourceGraphModelImp)
            {
                var sourceGraphType = sourceGraphModelImp.Graph.GetType();
                if (!sourceGraphType.IsInstanceOfType(Graph))
                    RecreateGraph(sourceGraphType);
            }
        }
    }
}
