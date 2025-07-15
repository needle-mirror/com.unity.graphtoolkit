using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.GraphToolkit.CSO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    static class WireCommandConfig
    {
        public const int nodeOffset = 60;
    }

    static class WireCommandHelper
    {
        public static List<WireModel> GetDropWireModelsToDelete(PortModel portModel, (WireSide, List<WireModel>) wiresToMove = default, List<WireModel> exceptWires = null)
        {
            if (portModel == null)
                return new List<WireModel>();

            var (sideToMove, wires) = wiresToMove;
            if (portModel.Capacity == PortCapacity.Multi && wires == null)
                return new List<WireModel>();

            var wireModelsToDelete = new List<WireModel>();

            using var pooledList = ListPool<WireModel>.Get(out var connectedWires);
            connectedWires.AddRange(portModel.GetConnectedWires());

            if (portModel.Capacity != PortCapacity.Multi)
            {
                // if a wire is created on a sub port, then none of its parent should have a wire.
                var parentPort = portModel.ParentPort;
                while (parentPort != null)
                {
                    connectedWires.AddRange(parentPort.GetConnectedWires());
                    parentPort = parentPort.ParentPort;
                }

                // if a wire is created on a parent port, then none of its descendant ports should have a wire.
                RecurseAddSubPortWires(portModel);
            }

            void RecurseAddSubPortWires(PortModel port)
            {
                foreach (var subPort in port.SubPorts)
                {
                    connectedWires.AddRange(subPort.GetConnectedWires());
                    RecurseAddSubPortWires(subPort);
                }
            }

            for (var i = 0; i < connectedWires.Count; i++)
            {
                var otherWire = connectedWires[i];
                if (otherWire is IGhostWireModel)
                    continue;

                if (exceptWires != null && exceptWires.Contains(otherWire))
                    continue;

                if (wires is null)
                {
                    if (portModel.Capacity != PortCapacity.Multi)
                        wireModelsToDelete.Add(otherWire);
                }
                else
                {
                    for (var j = 0; j < wires.Count; j++)
                    {
                        var wireToMove = wires[j];

                        if (otherWire.Guid == wireToMove.Guid)
                            break;

                        if (portModel.Capacity != PortCapacity.Multi)
                        {
                            wireModelsToDelete.Add(otherWire);
                            break;
                        }

                        if (!wireModelsToDelete.Contains(otherWire))
                        {
                            if (sideToMove == WireSide.To && otherWire.FromPort == wireToMove.FromPort && otherWire.ToPort == portModel ||
                                sideToMove == WireSide.From && otherWire.FromPort == portModel && otherWire.ToPort == wireToMove.ToPort)
                            {
                                wireModelsToDelete.Add(otherWire);
                                break;
                            }
                        }
                    }
                }
            }

            return wireModelsToDelete;
        }

        /// <summary>
        /// Gets the elements to frame during portals creation.
        /// </summary>
        /// <remarks>Framing is only necessary when it is not possible to see all created portals on screen.</remarks>
        public static List<GraphElement> GetElementsToFrameDuringPortalsCreation(IEnumerable<WireModel> wireModels, GraphView graphView)
        {
            var elementsToFrame = new List<GraphElement>();
            var inputNodes = new List<GraphElement>();
            var nodesBoundingRect = new Rect
            {
                xMin = float.MaxValue,
                xMax = float.MinValue,
                yMin = float.MaxValue,
                yMax = float.MinValue
            };

            var graphViewLayout = graphView.layout;
            var graphViewRect = new Rect(0, 0, graphViewLayout.width, graphViewLayout.height);

            var shouldFrame = false;
            foreach (var wireModel in wireModels)
            {
                var outputNode = wireModel.FromPort.NodeModel.GetView(graphView) as GraphElement;
                var inputNode = wireModel.ToPort.NodeModel.GetView(graphView) as GraphElement;

                if (outputNode != null && inputNode != null)
                {
                    elementsToFrame.Add(outputNode);
                    inputNodes.Add(inputNode);

                    var outputNodeLayout = outputNode.layout;
                    var inputNodeLayout = inputNode.layout;

                    if (!shouldFrame)
                    {
                        var gvOutputNodeRect = graphView.WorldToLocal(outputNode.parent.LocalToWorld(outputNodeLayout));
                        shouldFrame = !graphViewRect.Contains(gvOutputNodeRect.center);
                    }
                    if (!shouldFrame)
                    {
                        var gvInputNodeRect = graphView.WorldToLocal(outputNode.parent.LocalToWorld(inputNodeLayout));
                        shouldFrame = !graphViewRect.Contains(gvInputNodeRect.center);
                    }

                    nodesBoundingRect = RectUtils.Encompass(nodesBoundingRect, outputNodeLayout);
                    nodesBoundingRect = RectUtils.Encompass(nodesBoundingRect, inputNodeLayout);
                }
            }

            // Frame only when it is not possible to see both entry and exit portals on screen.
            if (!shouldFrame)
                return new List<GraphElement>();

            // If it is not possible to see all portals on screen at the minimal zoom scale out, prioritize entry portals.
            if (!RectFitInScreenAtMinScale(nodesBoundingRect, graphView))
            {
                var outputNodesBoundingRect = new Rect
                {
                    xMin = float.MaxValue,
                    xMax = float.MinValue,
                    yMin = float.MaxValue,
                    yMax = float.MinValue
                };

                foreach (var outputNode in elementsToFrame)
                    outputNodesBoundingRect = RectUtils.Encompass(outputNodesBoundingRect, outputNode.layout);

                // If it is not possible to see all entry portals on screen at the minimal zoom scale out, prioritize the first one.
                return RectFitInScreenAtMinScale(outputNodesBoundingRect, graphView) ? elementsToFrame : new List<GraphElement> { elementsToFrame[0] };
            }

            elementsToFrame.AddRange(inputNodes);

            return elementsToFrame;

            static bool RectFitInScreenAtMinScale(Rect rectToFit, VisualElement graphView)
            {
                var screenRect = new Rect
                {
                    xMin = GraphView.frameBorder,
                    xMax = graphView.layout.width - GraphView.frameBorder,
                    yMin = GraphView.frameBorder,
                    yMax = graphView.layout.height - GraphView.frameBorder
                };

                var identity = GUIUtility.ScreenToGUIRect(screenRect);
                var zoomLevel = Math.Min(identity.width / rectToFit.width, identity.height / rectToFit.height);

                return zoomLevel > ContentZoomer.DefaultMinScale;
            }
        }
    }

    /// <summary>
    /// Command to create a new wire.
    /// </summary>
    [UnityRestricted]
    internal class CreateWireCommand : UndoableCommand
    {
        const string k_UndoString = "Create Wire";

        /// <summary>
        /// Destination port.
        /// </summary>
        public PortModel ToPortModel;
        /// <summary>
        /// Origin port.
        /// </summary>
        public PortModel FromPortModel;
        /// <summary>
        /// Align the node that owns the <see cref="FromPortModel"/> to the <see cref="ToPortModel"/>.
        /// </summary>
        public bool AlignFromNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateWireCommand" /> class.
        /// </summary>
        public CreateWireCommand()
        {
            UndoString = k_UndoString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateWireCommand" /> class.
        /// </summary>
        /// <param name="toPortModel">Destination port.</param>
        /// <param name="fromPortModel">Origin port.</param>
        /// <param name="alignFromNode">Set to true if the node that owns the <paramref name="fromPortModel"/> should be aligned on the <paramref name="toPortModel"/>.</param>
        public CreateWireCommand(PortModel toPortModel, PortModel fromPortModel, bool alignFromNode = false)
            : this()
        {
            ToPortModel = toPortModel;
            FromPortModel = fromPortModel;
            AlignFromNode = alignFromNode;
        }

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="selectionState">The selection state.</param>
        /// <param name="autoPlacementState">The auto placement state component.</param>
        /// <param name="preferences">The tool preferences.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, SelectionStateComponent selectionState, AutoPlacementStateComponent autoPlacementState, Preferences preferences, CreateWireCommand command)
        {
            var selectionHelper = new GlobalSelectionCommandHelper(selectionState);
            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
                undoStateUpdater.SaveStates(selectionHelper.SelectionStates);

                if (command.AlignFromNode)
                {
                    undoStateUpdater.SaveState(autoPlacementState);
                }
            }

            var createdElements = new List<GraphElementModel>();
            var graphModel = graphModelState.GraphModel;
            using (var autoPlacementUpdater = autoPlacementState.UpdateScope)
            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModel.ChangeDescriptionScope)
            {
                var fromPortModel = command.FromPortModel;
                var toPortModel = command.ToPortModel;

                var wiresToDelete = WireCommandHelper.GetDropWireModelsToDelete(command.FromPortModel);
                wiresToDelete.AddRange(WireCommandHelper.GetDropWireModelsToDelete(command.ToPortModel));

                if (wiresToDelete.Count > 0)
                    graphModel.DeleteWires(wiresToDelete);

                WireModel wireModel;
                // Auto-itemization preferences will determine if a new node is created or not
                if ((fromPortModel.NodeModel is ConstantNodeModel && preferences.GetBool(BoolPref.AutoItemizeConstants)) ||
                    (fromPortModel.NodeModel is VariableNodeModel && preferences.GetBool(BoolPref.AutoItemizeVariables)))
                {
                    var itemizedNode = graphModel.CreateItemizedNode(WireCommandConfig.nodeOffset, ref fromPortModel);
                    if (itemizedNode != null)
                    {
                        createdElements.Add(itemizedNode);
                    }
                    wireModel = graphModel.CreateWire(toPortModel, fromPortModel);
                }
                else
                {
                    wireModel = graphModel.CreateWire(toPortModel, fromPortModel);
                    createdElements.Add(wireModel);
                }

                if (command.AlignFromNode)
                {
                    autoPlacementUpdater.MarkModelToAutoAlign(wireModel);
                }

                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }

            if (createdElements.Count > 0)
            {
                using (var selectionUpdaters = selectionHelper.UpdateScopes)
                {
                    foreach (var updater in selectionUpdaters)
                        updater.ClearSelection();
                    selectionUpdaters.MainUpdateScope.SelectElements(createdElements, true);
                }
            }
        }
    }

    /// <summary>
    /// Command to move one or more wires to a new port.
    /// </summary>
    [UnityRestricted]
    internal class MoveWireCommand : ModelCommand<WireModel>
    {
        const string k_UndoStringSingular = "Move Wire";
        const string k_UndoStringPlural = "Move Wires";

        /// <summary>
        /// The port where to move the wire(s).
        /// </summary>
        public PortModel NewPortModel;

        /// <summary>
        /// The Side of the wire to move.
        /// </summary>
        /// <remarks>In most case this should be inferred from <see cref="NewPortModel"/>,
        /// unless its <see cref="PortDirection"/> is not explicitly <see cref="PortDirection.Input"/> or <see cref="PortDirection.Output"/>.</remarks>
        public WireSide WireSideToMove;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveWireCommand" /> class.
        /// </summary>
        public MoveWireCommand()
            : base(k_UndoStringSingular)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveWireCommand" /> class.
        /// </summary>
        /// <param name="newPortModel">The port where to move the wire(s).</param>
        /// <param name="wireSide">The side of the wire(s) to move to the port.</param>
        /// <param name="wiresToMove">The list of wires to move.</param>
        public MoveWireCommand(PortModel newPortModel, WireSide wireSide, IReadOnlyList<WireModel> wiresToMove)
            : base(k_UndoStringSingular, k_UndoStringPlural, wiresToMove)
        {
            WireSideToMove = wireSide;
            NewPortModel = newPortModel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveWireCommand" /> class.
        /// </summary>
        /// <param name="newPortModel">The port where to move the wire(s).</param>
        /// <param name="wireSide">The side of the wire(s) to move to the port.</param>
        /// <param name="wiresToMove">The list of wires to move.</param>
        public MoveWireCommand(PortModel newPortModel, WireSide wireSide, params WireModel[] wiresToMove)
            : this(newPortModel, wireSide, (IReadOnlyList<WireModel>)(wiresToMove))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveWireCommand" /> class.
        /// </summary>
        /// <param name="newPortModel">The port where to move the wire(s).</param>
        /// <param name="wiresToMove">The list of wires to move.</param>
        public MoveWireCommand(PortModel newPortModel, IReadOnlyList<WireModel> wiresToMove)
            : this(newPortModel, WireSide.From, wiresToMove)
        {
            if (newPortModel != null)
            {
                Assert.IsNotNull(newPortModel);
                var newDir = newPortModel.Direction;
                WireSideToMove = newDir == PortDirection.Input ? WireSide.To : WireSide.From;
                Assert.IsFalse(newDir == PortDirection.None,
                    $"Can't move wires to a Port with direction {PortDirection.None}.");
                Assert.IsFalse(newDir.HasFlag(PortDirection.Input) == newDir.HasFlag(PortDirection.Output),
                    $"Can't infer port direction from port {newPortModel}, use the constructor for {nameof(MoveWireCommand)} with a specific direction.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveWireCommand" /> class.
        /// </summary>
        /// <param name="newPortModel">The port where to move the wire(s).</param>
        /// <param name="wiresToMove">The list of wires to move.</param>
        public MoveWireCommand(PortModel newPortModel, params WireModel[] wiresToMove)
            : this(newPortModel, (IReadOnlyList<WireModel>)(wiresToMove))
        {
        }

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState,
            MoveWireCommand command)
        {
            if (command.Models == null || command.Models.Count == 0)
                return;

            if (command.Models.Count > 1 && command.NewPortModel?.Capacity == PortCapacity.Single)
                return;

            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
            }

            var graphModel = graphModelState.GraphModel;
            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModel.ChangeDescriptionScope)
            {
                var newPortModel = command.NewPortModel;
                var wiresToMove = new List<WireModel>(command.Models);
                wiresToMove.Sort(WiresOrderComparer.Default);

                var wiresToDelete = WireCommandHelper.GetDropWireModelsToDelete(newPortModel, (command.WireSideToMove, wiresToMove));
                if (wiresToDelete.Count > 0)
                    graphModel.DeleteWires(wiresToDelete);

                foreach (var wire in wiresToMove)
                {
                    wire.SetPort(command.WireSideToMove, command.NewPortModel);
                }

                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }
        }

        class WiresOrderComparer : IComparer<WireModel>
        {
            public static WiresOrderComparer Default = new WiresOrderComparer();

            public int Compare(WireModel a, WireModel b)
            {
                if (a == null || a.FromPort == null || b == null || !ReferenceEquals(b.FromPort, a.FromPort))
                    return 0;
                return a.FromPort.GetWireOrder(a) - a.FromPort.GetWireOrder(b);
            }
        }
    }

    /// <summary>
    /// Command to delete one or more wires.
    /// </summary>
    [UnityRestricted]
    internal class DeleteWireCommand : ModelCommand<WireModel>
    {
        const string k_UndoStringSingular = "Delete Wire";
        const string k_UndoStringPlural = "Delete Wires";

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteWireCommand" /> class.
        /// </summary>
        public DeleteWireCommand()
            : base(k_UndoStringSingular) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteWireCommand" /> class.
        /// </summary>
        /// <param name="wiresToDelete">The list of wires to delete.</param>
        public DeleteWireCommand(IReadOnlyList<WireModel> wiresToDelete)
            : base(k_UndoStringSingular, k_UndoStringPlural, wiresToDelete)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteWireCommand" /> class.
        /// </summary>
        /// <param name="wiresToDelete">The list of wires to delete.</param>
        public DeleteWireCommand(params WireModel[] wiresToDelete)
            : this((IReadOnlyList<WireModel>)wiresToDelete)
        {
        }

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, DeleteWireCommand command)
        {
            if (command.Models == null || command.Models.Count == 0)
                return;

            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
            }

            var graphModel = graphModelState.GraphModel;
            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModel.ChangeDescriptionScope)
            {
                graphModel.DeleteWires(command.Models);
                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }
        }
    }

    /// <summary>
    /// Command to change the order of a wire.
    /// </summary>
    [UnityRestricted]
    internal class ReorderWireCommand : UndoableCommand
    {
        /// <summary>
        /// The wire to reorder.
        /// </summary>
        public readonly WireModel WireModel;
        /// <summary>
        /// The reorder operation to apply.
        /// </summary>
        public readonly ReorderType Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReorderWireCommand"/> class.
        /// </summary>
        public ReorderWireCommand()
        {
            UndoString = "Reorder Wire";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReorderWireCommand"/> class.
        /// </summary>
        /// <param name="wireModel">The wire to reorder.</param>
        /// <param name="type">The reorder operation to apply.</param>
        public ReorderWireCommand(WireModel wireModel, ReorderType type) : this()
        {
            WireModel = wireModel;
            Type = type;

            switch (Type)
            {
                case ReorderType.MoveFirst:
                    UndoString = "Move Wire First";
                    break;
                case ReorderType.MoveUp:
                    UndoString = "Move Wire Up";
                    break;
                case ReorderType.MoveDown:
                    UndoString = "Move Wire Down";
                    break;
                case ReorderType.MoveLast:
                    UndoString = "Move Wire Last";
                    break;
            }
        }

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, ReorderWireCommand command)
        {
            var fromPort = command.WireModel?.FromPort;
            if (fromPort != null && fromPort.HasReorderableWires)
            {
                var siblingWires = fromPort.GetConnectedWires();
                if (siblingWires.Count < 2)
                    return;

                using (var undoStateUpdater = undoState.UpdateScope)
                {
                    undoStateUpdater.SaveState(graphModelState);
                }

                using (var graphUpdater = graphModelState.UpdateScope)
                using (var changeScope = graphModelState.GraphModel.ChangeDescriptionScope)
                {
                    fromPort.ReorderWire(command.WireModel, command.Type);
                    graphUpdater.MarkUpdated(changeScope.ChangeDescription);
                }
            }
        }
    }

    /// <summary>
    /// Command to insert a node in the middle of a wire.
    /// </summary>
    [UnityRestricted]
    internal class SplitWireAndInsertExistingNodeCommand : UndoableCommand
    {
        public readonly WireModel WireModel;
        public readonly InputOutputPortsNodeModel NodeModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitWireAndInsertExistingNodeCommand"/> class.
        /// </summary>
        public SplitWireAndInsertExistingNodeCommand()
        {
            UndoString = "Insert Node On Wire";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitWireAndInsertExistingNodeCommand"/> class.
        /// </summary>
        /// <param name="wireModel">The wire on which to insert a node.</param>
        /// <param name="nodeModel">The node to insert.</param>
        public SplitWireAndInsertExistingNodeCommand(WireModel wireModel, InputOutputPortsNodeModel nodeModel) : this()
        {
            WireModel = wireModel;
            NodeModel = nodeModel;
        }

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, SplitWireAndInsertExistingNodeCommand command)
        {
            Assert.IsTrue(command.NodeModel.InputsById.Count > 0);
            Assert.IsTrue(command.NodeModel.OutputsById.Count > 0);

            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
            }

            var graphModel = graphModelState.GraphModel;
            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModel.ChangeDescriptionScope)
            {
                var wireInput = command.WireModel.ToPort;
                var wireOutput = command.WireModel.FromPort;
                graphModel.DeleteWire(command.WireModel);

                PortModel fromPort = null;
                for (var i = 0; i < command.NodeModel.OutputsByDisplayOrder.Count; i++)
                {
                    var output = command.NodeModel.OutputsByDisplayOrder[i];
                    if (output != null && output.PortType == wireInput?.PortType)
                    {
                        fromPort = output;
                        break;
                    }
                }

                PortModel toPort = null;
                for (var i = 0; i < command.NodeModel.InputsByDisplayOrder.Count; i++)
                {
                    var input = command.NodeModel.InputsByDisplayOrder[i];
                    if (input != null && input.PortType == wireOutput?.PortType)
                    {
                        toPort = input;
                        break;
                    }
                }

                graphModel.CreateWire(wireInput, fromPort);
                graphModel.CreateWire(toPort, wireOutput);

                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }
        }
    }

    /// <summary>
    /// Command to convert wires to portal nodes.
    /// </summary>
    [UnityRestricted]
    internal class ConvertWiresToPortalsCommand : UndoableCommand
    {
        const string k_UndoStringSingular = "Convert Wire to Portal";
        const string k_UndoStringPlural = "Convert Wires to Portals";

        static readonly Vector2 k_EntryPortalBaseOffset = Vector2.right * 75;
        static readonly Vector2 k_ExitPortalBaseOffset = Vector2.left * 250;
        public static readonly int portalHeight = 24;

        /// <summary>
        /// Data describing which wire to transform and the position of the portals.
        /// </summary>
        public IReadOnlyList<(WireModel wire, Vector2 startPortPos, Vector2 endPortPos)> WireData;

        /// <summary>
        /// The <see cref="GraphView"/> that will contain the portals.
        /// </summary>
        public GraphView GraphView;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertWiresToPortalsCommand"/> class.
        /// </summary>
        public ConvertWiresToPortalsCommand()
        {
            UndoString = k_UndoStringSingular;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertWiresToPortalsCommand"/> class.
        /// </summary>
        /// <param name="wireData">A list of tuple, each tuple containing the wire to convert, the position of the entry portal node and the position of the exit portal node.</param>
        /// <param name="graphView">The <see cref="GraphView"/> that will contain the portals.</param>
        public ConvertWiresToPortalsCommand(IReadOnlyList<(WireModel, Vector2, Vector2)> wireData, GraphView graphView) : this()
        {
            WireData = wireData;
            GraphView = graphView;
            UndoString = (WireData?.Count ?? 0) <= 1 ? k_UndoStringSingular : k_UndoStringPlural;
        }

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphViewState">The graph view state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="selectionState">The selection state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphViewStateComponent graphViewState, GraphModelStateComponent graphModelState, SelectionStateComponent selectionState, ConvertWiresToPortalsCommand command)
        {
            if (!graphModelState.GraphModel.AllowPortalCreation)
                return;

            if (command.WireData == null || command.WireData.Count == 0)
                return;

            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
            }

            var createdElements = new List<Hash128>();
            var graphModel = graphModelState.GraphModel;

            var elementsToFrame = new List<GraphElement>();
            if (command.GraphView != null)
            {
                var wireModels = new List<WireModel>();
                for (var i = 0; i < command.WireData.Count; i++)
                    wireModels.Add(command.WireData[i].wire);

                elementsToFrame = WireCommandHelper.GetElementsToFrameDuringPortalsCreation(wireModels, command.GraphView);
            }

            using (var graphModelUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModel.ChangeDescriptionScope)
            {
                var existingPortalEntries = new Dictionary<PortModel, WirePortalModel>();
                var existingPortalExits = new Dictionary<PortModel, List<WirePortalModel>>();

                foreach (var wireModel in command.WireData)
                    graphModel.CreatePortalsFromWire(
                        wireModel.wire,
                        wireModel.startPortPos + k_EntryPortalBaseOffset,
                        wireModel.endPortPos + k_ExitPortalBaseOffset,
                        portalHeight, existingPortalEntries, existingPortalExits);

                // Adjust placement in case of multiple incoming exit portals so they don't overlap
                foreach (var portalList in existingPortalExits.Values)
                {
                    if (portalList.Count < 2)
                        continue;

                    var cnt = portalList.Count;
                    bool isEven = cnt % 2 == 0;
                    int offset = isEven ? portalHeight / 2 : 0;
                    for (int i = (cnt - 1) / 2; i >= 0; i--)
                    {
                        portalList[i].Position = new Vector2(portalList[i].Position.x, portalList[i].Position.y - offset);
                        portalList[cnt - 1 - i].Position = new Vector2(portalList[cnt - 1 - i].Position.x, portalList[cnt - 1 - i].Position.y + offset);
                        offset += portalHeight;
                    }
                }

                graphModelUpdater.MarkUpdated(changeScope.ChangeDescription);
                var createdElementGuids = new List<Hash128>();
                foreach (var newModelGuid in changeScope.ChangeDescription.NewModels)
                {
                    var model = graphModel.GetModel(newModelGuid);
                    if (model is AbstractNodeModel)
                        createdElementGuids.Add(newModelGuid);
                }
                createdElements.AddRange(createdElementGuids);
            }

            if (command.GraphView != null && elementsToFrame.Count > 0)
            {
                using (var graphViewUpdater = graphViewState.UpdateScope)
                {
                    command.GraphView.CalculateFrameTransformToFitElements(elementsToFrame, out var frameTranslation, out var frameScaling, 0.75f);
                    graphViewUpdater.Position = frameTranslation;
                    graphViewUpdater.Scale = frameScaling;
                }
            }

            if (createdElements.Count > 0)
            {
                var selectionHelper = new GlobalSelectionCommandHelper(selectionState);
                using (var selectionUpdaters = selectionHelper.UpdateScopes)
                {
                    foreach (var updater in selectionUpdaters)
                        updater.ClearSelection();
                    selectionUpdaters.MainUpdateScope.SelectElements(createdElements, true);
                }
            }
        }
    }
}
