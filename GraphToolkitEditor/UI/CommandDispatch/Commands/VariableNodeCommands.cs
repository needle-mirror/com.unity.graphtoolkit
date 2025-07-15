using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.GraphToolkit.CSO;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// A command to convert variables to constants and vice versa.
    /// </summary>
    [UnityRestricted]
    internal class ConvertConstantNodesAndVariableNodesCommand : UndoableCommand
    {
        /// <summary>
        /// The constant nodes to convert to variable nodes.
        /// </summary>
        public IReadOnlyList<ConstantNodeModel> ConstantNodeModels;
        /// <summary>
        /// The variable nodes to convert to constant nodes.
        /// </summary>
        public IReadOnlyList<VariableNodeModel> VariableNodeModels;

        const string k_UndoString = "Convert Constants And Variables";
        const string k_UndoStringCToVSingular = "Convert Constant To Variable";
        const string k_UndoStringCToVPlural = "Convert Constants To Variables";
        const string k_UndoStringVToCSingular = "Convert Variable To Constant";
        const string k_UndoStringVToCPlural = "Convert Variables To Constants";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertConstantNodesAndVariableNodesCommand" /> class.
        /// </summary>
        public ConvertConstantNodesAndVariableNodesCommand()
        {
            UndoString = k_UndoString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertConstantNodesAndVariableNodesCommand" /> class.
        /// </summary>
        /// <param name="constantNodeModels">The constants to convert to variables.</param>
        /// <param name="variableNodeModels">The variables to convert to constants.</param>
        public ConvertConstantNodesAndVariableNodesCommand(
            IReadOnlyList<ConstantNodeModel> constantNodeModels,
            IReadOnlyList<VariableNodeModel> variableNodeModels)
        {
            ConstantNodeModels = constantNodeModels;
            VariableNodeModels = variableNodeModels;

            var constantCount = ConstantNodeModels?.Count ?? 0;
            var variableCount = VariableNodeModels?.Count ?? 0;

            if (constantCount == 0)
            {
                if (variableCount == 1)
                {
                    UndoString = k_UndoStringVToCSingular;
                }
                else
                {
                    UndoString = k_UndoStringVToCPlural;
                }
            }
            else if (variableCount == 0)
            {
                if (constantCount == 1)
                {
                    UndoString = k_UndoStringCToVSingular;
                }
                else
                {
                    UndoString = k_UndoStringCToVPlural;
                }
            }
            else
            {
                UndoString = k_UndoString;
            }
        }

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="selectionState">The selection state component.</param>
        /// <param name="command">The command to apply to the state.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, SelectionStateComponent selectionState, ConvertConstantNodesAndVariableNodesCommand command)
        {
            if ((command.ConstantNodeModels?.Count ?? 0) == 0 && (command.VariableNodeModels?.Count ?? 0) == 0)
                return;

            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
                undoStateUpdater.SaveState(selectionState);
            }

            var graphModel = graphModelState.GraphModel;
            using (var graphUpdater = graphModelState.UpdateScope)
            using (var selectionUpdater = selectionState.UpdateScope)
            using (var changeScope = graphModel.ChangeDescriptionScope)
            {
                foreach (var constantModel in command.ConstantNodeModels ?? Enumerable.Empty<ConstantNodeModel>())
                {
                    var declarationModel = graphModel.CreateGraphVariableDeclaration(
                        constantModel.Type.GenerateTypeHandle(),
                        TypeHelpers.GetFriendlyName(constantModel.Type).CodifyString(), ModifierFlags.None,
                        VariableScope.Local, null, -1, constantModel.Value.Clone());

                    var variableModel = graphModel.CreateVariableNode(declarationModel, constantModel.Position);
                    if (variableModel != null)
                    {
                        selectionUpdater.SelectElement(variableModel, true);

                        variableModel.State = constantModel.State;

                        if (constantModel.ElementColor.HasUserColor)
                            variableModel.SetColor(constantModel.ElementColor.Color);

                        foreach (var wireModel in graphModel.GetWiresForPort(constantModel.OutputPort).ToList())
                        {
                            graphModel.CreateWire(wireModel.ToPort, variableModel.OutputPort);
                            graphModel.DeleteWire(wireModel);
                        }
                    }

                    graphModel.DeleteNode(constantModel, deleteConnections: false);
                }

                foreach (var variableModel in command.VariableNodeModels ?? Enumerable.Empty<VariableNodeModel>())
                {
                    if (graphModel.GetConstantType(variableModel.DataType) == null)
                        continue;
                    var constantModel = graphModel.CreateConstantNode(variableModel.DataType, variableModel.Title, variableModel.Position);
                    constantModel.Value.ObjectValue = variableModel.VariableDeclarationModel?.InitializationModel?.ObjectValue;
                    constantModel.State = variableModel.State;
                    if (variableModel.ElementColor.HasUserColor)
                        constantModel.SetColor(variableModel.ElementColor.Color);
                    selectionUpdater.SelectElement(constantModel, true);

                    var wireModels = graphModel.GetWiresForPort(variableModel.OutputPort).ToList();
                    foreach (var wireModel in wireModels)
                    {
                        graphModel.CreateWire(wireModel.ToPort, constantModel.OutputPort);
                        graphModel.DeleteWire(wireModel);
                    }

                    graphModel.DeleteNode(variableModel, deleteConnections: false);
                }

                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
                selectionUpdater.SelectElements(changeScope.ChangeDescription.DeletedModels, false);
            }
        }
    }

    /// <summary>
    /// Command to itemize a node.
    /// </summary>
    [UnityRestricted]
    internal class ItemizeNodeCommand : ModelCommand<ISingleOutputPortNodeModel>
    {
        const string k_UndoStringSingular = "Itemize Node";
        const string k_UndoStringPlural = "Itemize Nodes";

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemizeNodeCommand"/> class.
        /// </summary>
        public ItemizeNodeCommand()
            : base(k_UndoStringSingular) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemizeNodeCommand"/> class.
        /// </summary>
        /// <param name="models">The nodes to itemize.</param>
        public ItemizeNodeCommand(IReadOnlyList<ISingleOutputPortNodeModel> models)
            : base(k_UndoStringSingular, k_UndoStringPlural, models) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemizeNodeCommand"/> class.
        /// </summary>
        /// <param name="models">The nodes to itemize.</param>
        public ItemizeNodeCommand(params ISingleOutputPortNodeModel[] models)
            : this((IReadOnlyList<ISingleOutputPortNodeModel>)models) {}

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="selectionState">The selection state.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, SelectionStateComponent selectionState, ItemizeNodeCommand command)
        {
            bool undoPushed = false;

            var createdElements = new List<GraphElementModel>();
            var graphModel = graphModelState.GraphModel;
            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModel.ChangeDescriptionScope)
            {
                foreach (var model in command.Models.Where(m => m is VariableNodeModel || m is ConstantNodeModel))
                {
                    var wireModels = graphModel.GetWiresForPort(model.OutputPort).ToList();

                    for (var i = 1; i < wireModels.Count; i++)
                    {
                        if (!undoPushed)
                        {
                            undoPushed = true;
                            using (var undoStateUpdater = undoState.UpdateScope)
                            {
                                undoStateUpdater.SaveState(graphModelState);
                            }
                        }

                        var newModel = graphModel.DuplicateNode(model as AbstractNodeModel, i * 50 * Vector2.up);
                        createdElements.Add(newModel);
                        var wireModel = wireModels[i];
                        graphModel.CreateWire(wireModel.ToPort, ((ISingleOutputPortNodeModel)newModel).OutputPort);
                        graphModel.DeleteWire(wireModel);
                    }
                }
                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }

            if (createdElements.Any())
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

    /// <summary>
    /// Command to change the variable declaration of variable nodes.
    /// </summary>
    [UnityRestricted]
    internal class ChangeVariableDeclarationCommand : ModelCommand<VariableNodeModel>
    {
        const string k_UndoStringSingular = "Change Variable";

        /// <summary>
        /// The new variable declaration for the nodes.
        /// </summary>
        public readonly VariableDeclarationModelBase Variable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeVariableDeclarationCommand"/> class.
        /// </summary>
        public ChangeVariableDeclarationCommand()
            : base(k_UndoStringSingular) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeVariableDeclarationCommand"/> class.
        /// </summary>
        /// <param name="models">The variable node for which to change the variable declaration.</param>
        /// <param name="variable">The new variable declaration.</param>
        public ChangeVariableDeclarationCommand(IReadOnlyList<VariableNodeModel> models, VariableDeclarationModelBase variable)
            : base(k_UndoStringSingular, k_UndoStringSingular, models)
        {
            Variable = variable;
        }

        /// <summary>
        /// Default command handler.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="command">The command.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState, ChangeVariableDeclarationCommand command)
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
                    model.SetDeclarationModel(command.Variable);
                }
                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }
        }
    }
}
