using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.GraphToolkit.CSO;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Command to collapse and expand nodes.
    /// </summary>
    [UnityRestricted]
    internal class CollapseNodeCommand : ModelCommand<AbstractNodeModel, bool>
    {
        const string k_CollapseUndoStringSingular = "Collapse Node";
        const string k_CollapseUndoStringPlural = "Collapse Nodes";
        const string k_ExpandUndoStringSingular = "Expand Node";
        const string k_ExpandUndoStringPlural = "Expand Nodes";

        /// <summary>
        /// Initializes a new instance of the <see cref="CollapseNodeCommand"/> class.
        /// </summary>
        public CollapseNodeCommand()
            : base("Collapse Or Expand Node") {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CollapseNodeCommand"/> class.
        /// </summary>
        /// <param name="value">True if the nodes should be collapsed, false otherwise.</param>
        /// <param name="nodes">The nodes to expand or collapse.</param>
        public CollapseNodeCommand(bool value, IReadOnlyList<AbstractNodeModel> nodes)
            : base(value ? k_CollapseUndoStringSingular : k_ExpandUndoStringSingular,
                   value ? k_CollapseUndoStringPlural : k_ExpandUndoStringPlural, value, nodes)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CollapseNodeCommand"/> class.
        /// </summary>
        /// <param name="value">True if the nodes should be collapsed, false otherwise.</param>
        /// <param name="nodes">The nodes to expand or collapse.</param>
        public CollapseNodeCommand(bool value, params AbstractNodeModel[] nodes)
            : this(value, (IReadOnlyList<AbstractNodeModel>)nodes)
        {}

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, CollapseNodeCommand command)
        {
            if (!command.Models.Any())
                return;

            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
            }

            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModelState.GraphModel.ChangeDescriptionScope)
            {
                foreach (var model in command.Models.OfType<ICollapsible>())
                {
                    model.Collapsed = command.Value;
                }
                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }
        }
    }

    /// <summary>
    /// Command to hide and show node previews.
    /// </summary>
    [UnityRestricted]
    internal class ShowNodePreviewCommand : ModelCommand<AbstractNodeModel, bool>
    {
        const string k_HidePreviewUndoStringSingular = "Hide Node Preview";
        const string k_HidePreviewUndoStringPlural = "Hide Node Previews";
        const string k_ShowPreviewUndoStringSingular = "Show Node Preview";
        const string k_ShowPreviewUndoStringPlural = "Show Node Previews";

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowNodePreviewCommand"/> class.
        /// </summary>
        public ShowNodePreviewCommand()
            : base("Show or Hide Node Preview") {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowNodePreviewCommand"/> class.
        /// </summary>
        /// <param name="value">True if the node previews should be hidden, false otherwise.</param>
        /// <param name="nodes">The nodes which node previews are to be hidden or shown.</param>
        public ShowNodePreviewCommand(bool value, IReadOnlyList<AbstractNodeModel> nodes)
            : base(value ? k_ShowPreviewUndoStringSingular : k_HidePreviewUndoStringSingular,
                   value ? k_ShowPreviewUndoStringPlural : k_HidePreviewUndoStringPlural, value, nodes)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowNodePreviewCommand"/> class.
        /// </summary>
        /// <param name="value">True if the node previews should be hidden, false otherwise.</param>
        /// <param name="nodes">The nodes which node previews are to be hidden or shown.</param>
        public ShowNodePreviewCommand(bool value, params AbstractNodeModel[] nodes)
            : this(value, (IReadOnlyList<AbstractNodeModel>)nodes)
        {}

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, ShowNodePreviewCommand command)
        {
            if (!command.Models.Any())
                return;

            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
            }

            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModelState.GraphModel.ChangeDescriptionScope)
            {
                foreach (var model in command.Models)
                {
                    if (model.NodePreviewModel != null)
                        model.NodePreviewModel.ShowNodePreview = command.Value;
                }
                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }
        }
    }

    /// <summary>
    /// Command to change the name of graph elements.
    /// </summary>
    [UnityRestricted]
    internal class RenameElementsCommand : UndoableCommand
    {
        const string k_SingleElementRenamedName = "Rename Element";
        const string k_MultipleElementsRenamedName = "Rename Elements";

        /// <summary>
        /// The graph elements to rename.
        /// </summary>
        public IReadOnlyList<IRenamable> Models;

        /// <summary>
        /// The new name.
        /// </summary>
        public string ElementName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenameElementsCommand"/> class.
        /// </summary>
        public RenameElementsCommand()
        {
            UndoString = k_SingleElementRenamedName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenameElementsCommand"/> class.
        /// </summary>
        /// <param name="model">The graph element to rename.</param>
        /// <param name="name">The new name.</param>
        public RenameElementsCommand(IRenamable model, string name) : this(new[] {model}, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenameElementsCommand"/> class.
        /// </summary>
        /// <param name="models">The graph elements to rename.</param>
        /// <param name="name">The new name.</param>
        public RenameElementsCommand(IReadOnlyList<IRenamable> models, string name)
        {
            Models = models;
            if (models?.Count == 1)
                UndoString = k_SingleElementRenamedName;
            else
                UndoString = k_MultipleElementsRenamedName;

            ElementName = name;
        }

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, RenameElementsCommand command)
        {
            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
            }

            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModelState.GraphModel.ChangeDescriptionScope)
            {
                foreach (var model in command.Models)
                {
                    model.Rename(command.ElementName);
                    if (model is Model m && m is not GraphElementModel)
                        graphUpdater.MarkChanged(m.Guid, ChangeHint.Data);
                }
                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }
        }
    }

    /// <summary>
    /// Command to update the value of a constant.
    /// </summary>
    [UnityRestricted]
    internal class UpdateConstantValueCommand : UndoableCommand
    {
        /// <summary>
        /// The constant to update.
        /// </summary>
        public Constant Constant;
        /// <summary>
        /// The new value.
        /// </summary>
        public object Value;
        /// <summary>
        /// The node model that owns the constant, if any.
        /// </summary>
        public GraphElementModel OwnerModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateConstantValueCommand"/> class.
        /// </summary>
        public UpdateConstantValueCommand()
        {
            UndoString = "Update Value";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateConstantValueCommand"/> class.
        /// </summary>
        /// <param name="constant">The constant to update.</param>
        /// <param name="value">The new value.</param>
        /// <param name="owner">The model that owns the constant, if any.</param>
        public UpdateConstantValueCommand(Constant constant, object value, GraphElementModel owner) : this()
        {
            Constant = constant;
            Value = value;
            OwnerModel = owner;
        }

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, UpdateConstantValueCommand command)
        {
            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
            }

            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModelState.GraphModel.ChangeDescriptionScope)
            {
                command.Constant.ObjectValue = command.Value;
                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }
        }
    }

    /// <summary>
    /// Command to update the values of some constants.
    /// </summary>
    [UnityRestricted]
    internal class UpdateConstantsValueCommand : UndoableCommand
    {
        /// <summary>
        /// The constants to update.
        /// </summary>
        public IReadOnlyList<Constant> Constants;

        /// <summary>
        /// The new value.
        /// </summary>
        public object Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateConstantsValueCommand"/> class.
        /// </summary>
        public UpdateConstantsValueCommand()
        {
            UndoString = "Update Values";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateConstantsValueCommand"/> class.
        /// </summary>
        /// <param name="constants">The constants to update.</param>
        /// <param name="value">The new value.</param>
        public UpdateConstantsValueCommand(IEnumerable<Constant> constants, object value) : this()
        {
            Constants = constants?.ToList() ?? new List<Constant>();
            Value = value;
        }

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, UpdateConstantsValueCommand command)
        {
            if (command.Constants.Any())
            {
                using (var undoStateUpdater = undoState.UpdateScope)
                {
                    undoStateUpdater.SaveState(graphModelState);
                }

                using (var graphUpdater = graphModelState.UpdateScope)
                using (var changeScope = graphModelState.GraphModel.ChangeDescriptionScope)
                {
                    foreach (var constant in command.Constants)
                    {
                        constant.ObjectValue = command.Value;
                        constant.SetterMethod?.Invoke(constant.ObjectValue);
                    }
                    graphUpdater.MarkUpdated(changeScope.ChangeDescription);
                }
            }
        }
    }

    /// <summary>
    /// Command to remove all wires on nodes.
    /// </summary>
    [UnityRestricted]
    internal class DisconnectWiresCommand : ModelCommand<AbstractNodeModel>
    {
        const string k_UndoStringSingular = "Disconnect Wires";
        const string k_UndoStringPlural = "Disconnect Wires";

        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectWiresCommand"/> class.
        /// </summary>
        public DisconnectWiresCommand()
            : base(k_UndoStringSingular) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectWiresCommand"/> class.
        /// </summary>
        /// <param name="nodeModels">The nodes to disconnect.</param>
        public DisconnectWiresCommand(IReadOnlyList<AbstractNodeModel> nodeModels)
            : base(k_UndoStringSingular, k_UndoStringPlural, nodeModels) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectWiresCommand"/> class.
        /// </summary>
        /// <param name="nodeModels">The nodes to disconnect.</param>
        public DisconnectWiresCommand(params AbstractNodeModel[] nodeModels)
            : this((IReadOnlyList<AbstractNodeModel>)nodeModels) {}

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, DisconnectWiresCommand command)
        {
            if (!command.Models.Any())
                return;

            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
            }

            var graphModel = graphModelState.GraphModel;
            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModel.ChangeDescriptionScope)
            {
                foreach (var nodeModel in command.Models)
                {
                    var connectedWires = nodeModel.GetConnectedWires().ToList();
                    graphModel.DeleteWires(connectedWires);
                }
                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }
        }
    }

    /// <summary>
    /// Command to remove all wires connected to specific ports.
    /// </summary>
    [UnityRestricted]
    internal class DisconnectWiresOnPortCommand : ModelCommand<PortModel>
    {
        const string k_UndoStringSingular = "Disconnect Wire(s) on Port";
        const string k_UndoStringPlural = "Disconnect Wire(s) on Port";

        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectWiresOnPortCommand"/> class.
        /// </summary>
        public DisconnectWiresOnPortCommand()
            : base(k_UndoStringSingular) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectWiresOnPortCommand"/> class.
        /// </summary>
        /// <param name="portModels">The ports to disconnect.</param>
        public DisconnectWiresOnPortCommand(IReadOnlyList<PortModel> portModels)
            : base(k_UndoStringSingular, k_UndoStringPlural, portModels) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectWiresOnPortCommand"/> class.
        /// </summary>
        /// <param name="portModels">The ports to disconnect.</param>
        public DisconnectWiresOnPortCommand(params PortModel[] portModels)
            : this((IReadOnlyList<PortModel>)portModels) {}

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, DisconnectWiresOnPortCommand command)
        {
            if (!command.Models.Any())
                return;

            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
            }

            var graphModel = graphModelState.GraphModel;
            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModel.ChangeDescriptionScope)
            {
                foreach (var port in command.Models)
                {
                    var connectedWires = port.GetConnectedWires().ToList();
                    graphModel.DeleteWires(connectedWires);
                }
                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }
        }
    }

    /// <summary>
    /// Command to bypass nodes using wires. Optionally deletes the nodes.
    /// </summary>
    [UnityRestricted]
    internal class BypassNodesCommand : ModelCommand<AbstractNodeModel>
    {
        const string k_UndoStringSingular = "Delete Element";
        const string k_UndoStringPlural = "Delete Elements";

        /// <summary>
        /// The nodes to bypass.
        /// </summary>
        public readonly IReadOnlyList<InputOutputPortsNodeModel> NodesToBypass;

        /// <summary>
        /// Initializes a new instance of the <see cref="BypassNodesCommand"/> class.
        /// </summary>
        public BypassNodesCommand()
            : base(k_UndoStringSingular) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="BypassNodesCommand"/> class.
        /// </summary>
        /// <param name="nodesToBypass">The nodes to bypass.</param>
        /// <param name="elementsToRemove">The nodes to delete.</param>
        public BypassNodesCommand(IReadOnlyList<InputOutputPortsNodeModel> nodesToBypass, IReadOnlyList<AbstractNodeModel> elementsToRemove)
            : base(k_UndoStringSingular, k_UndoStringPlural, elementsToRemove)
        {
            NodesToBypass = nodesToBypass;
        }

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="selectionState">The selection state of the graph view.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, SelectionStateComponent selectionState, BypassNodesCommand command)
        {
            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
                undoStateUpdater.SaveState(selectionState);
            }

            var graphModel = graphModelState.GraphModel;

            using (var selectionUpdater = selectionState.UpdateScope)
            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModel.ChangeDescriptionScope)
            {
                foreach (var model in command.NodesToBypass)
                {
                    var inputWireModel = new List<WireModel>();
                    foreach (var portModel in model.InputsByDisplayOrder)
                    {
                        inputWireModel.AddRange(graphModel.GetWiresForPort(portModel));
                    }

                    if (!inputWireModel.Any())
                        continue;

                    var outputWireModels = new List<WireModel>();
                    foreach (var portModel in model.OutputsByDisplayOrder)
                    {
                        outputWireModels.AddRange(graphModel.GetWiresForPort(portModel));
                    }

                    if (!outputWireModels.Any())
                        continue;

                    graphModel.DeleteWires(inputWireModel);
                    graphModel.DeleteWires(outputWireModels);

                    if(graphModel.IsCompatiblePort(inputWireModel[0].FromPort, outputWireModels[0].ToPort))
                        graphModel.CreateWire(outputWireModels[0].ToPort, inputWireModel[0].FromPort);
                }

                graphModel.DeleteElements(command.Models);
                graphUpdater.MarkUpdated(changeScope.ChangeDescription);

                selectionUpdater.SelectElements(changeScope.ChangeDescription.DeletedModels, false);
            }
        }
    }

    /// <summary>
    /// Command to change the state of nodes.
    /// </summary>
    [UnityRestricted]
    internal class ChangeNodeStateCommand : ModelCommand<AbstractNodeModel, ModelState>
    {
        const string k_UndoStringSingular = "Change Node State";
        const string k_UndoStringPlural = "Change Nodes State";

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNodeStateCommand"/> class.
        /// </summary>
        public ChangeNodeStateCommand()
            : base(k_UndoStringSingular) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNodeStateCommand"/> class.
        /// </summary>
        /// <param name="state">The new node state.</param>
        /// <param name="nodeModels">The nodes to modify.</param>
        public ChangeNodeStateCommand(ModelState state, IReadOnlyList<AbstractNodeModel> nodeModels)
            : base(k_UndoStringSingular, k_UndoStringPlural, state, nodeModels) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNodeStateCommand"/> class.
        /// </summary>
        /// <param name="state">The new node state.</param>
        /// <param name="nodeModels">The nodes to modify.</param>
        public ChangeNodeStateCommand(ModelState state, params AbstractNodeModel[] nodeModels)
            : this(state, (IReadOnlyList<AbstractNodeModel>)nodeModels) {}

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, ChangeNodeStateCommand command)
        {
            if (!command.Models.Any(t => t.IsDisableable()))
                return;

            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
            }

            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModelState.GraphModel.ChangeDescriptionScope)
            {
                foreach (var nodeModel in command.Models)
                {
                    nodeModel.State = command.Value;
                }
                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }
        }
    }

    /// <summary>
    /// Command to change the mode of a node.
    /// </summary>
    [UnityRestricted]
    internal class ChangeNodeModeCommand : UndoableCommand
    {
        /// <summary>
        /// The node whose mode is changed.
        /// </summary>
        public NodeModel NodeModel;

        /// <summary>
        /// The new mode index.
        /// </summary>
        public int NewNodeModeIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNodeModeCommand"/> class.
        /// </summary>
        public ChangeNodeModeCommand()
        {
            UndoString = "Change Node Mode";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNodeModeCommand"/> class.
        /// </summary>
        /// <param name="nodeModel">The node model to change.</param>
        /// <param name="newNodeModeIndex">The node mode index to change to.</param>
        public ChangeNodeModeCommand(NodeModel nodeModel, int newNodeModeIndex) : this()
        {
            NodeModel = nodeModel;
            NewNodeModeIndex = newNodeModeIndex;
        }

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, ChangeNodeModeCommand command)
        {
            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
            }

            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModelState.GraphModel.ChangeDescriptionScope)
            {
                command.NodeModel.ChangeMode(command.NewNodeModeIndex);
                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }
        }
    }
}
