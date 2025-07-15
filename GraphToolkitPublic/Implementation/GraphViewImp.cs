using System;
using System.Linq;
using System.Reflection;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor.Implementation
{
    class GraphViewImp : GraphView
    {
        public GraphViewImp(EditorWindow window, GraphTool graphTool, string graphViewName, GraphRootViewModel graphViewModel, ViewSelection viewSelection, GraphViewDisplayMode displayMode = GraphViewDisplayMode.Interactive, TypeHandleInfos typeHandleInfos = null)
            : base(window, graphTool, graphViewName, graphViewModel, viewSelection, displayMode, typeHandleInfos) { }

        protected override IDragAndDropHandler GraphAssetDragAndDropHandler
        {
            get
            {
                if (GraphModel is GraphModelImp graphModelImp)
                {
                    var graphAttribute = graphModelImp.Graph.GetType().GetCustomAttribute<GraphAttribute>();
                    if (graphAttribute != null && graphAttribute.options.HasFlag(GraphOptions.SupportsSubgraphs))
                    {
                        // Only add subgraph drag and drop handler if the graph supports subgraphs
                        return m_SubgraphAssetDragAndDropHandler ??= new SubgraphDragAndDropHandler(this);
                    }
                }

                return null;
            }
        }

        protected override ItemLibraryHelper CreateItemLibraryHelper()
        {
            return (Window as GraphViewEditorWindow)?.CreateItemLibraryHelper(GraphModel);
        }

        public override GraphView CreateSimplePreview()
        {
            return new PreviewGraphViewImp(null, null, "",  null, null, GraphViewDisplayMode.NonInteractive);
        }

        internal void CallBuildContextualMenuForTests(ContextualMenuPopulateEvent evt)
        {
            BuildContextualMenu(evt);
            evt.menu.PrepareForDisplay(evt.triggerEvent);
        }

        protected override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.menu.MenuItems().Count > 0)
                evt.menu.AppendSeparator();

            if (GraphModel is GraphModelImp graphModel)
            {
                var subGraphTypes = PublicGraphFactory.GetSubGraphTypes(graphModel.Graph.GetType());

                foreach (var subGraphType in subGraphTypes)
                {
                    var template = new SubgraphTemplateImp(subGraphType,subGraphTypes.Count == 1 ? "Subgraph" : $"{subGraphType.Name} Subgraph");
                    AddConvertToSubgraphMenuItem(typeof(GraphObjectImp), typeof(GraphModelImp), evt, GetSelection(), template);
                }
            }

            base.BuildContextualMenu(evt);
        }
    }
}
