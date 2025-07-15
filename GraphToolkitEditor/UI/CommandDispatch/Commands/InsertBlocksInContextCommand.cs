using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.GraphToolkit.CSO;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Command to insert blocks into contexts.
    /// </summary>
    [UnityRestricted]
    internal class InsertBlocksInContextCommand : UndoableCommand
    {
        const string k_UndoString = "Insert Block";
        const string k_UndoStringPlural = "Insert Blocks";
        const string k_UndoStringDuplicate = "Duplicate Block";
        const string k_UndoStringDuplicatePlural = "Duplicate Blocks";

        /// <summary>
        /// The data needed per context to insert.
        /// </summary>
        [UnityRestricted]
        internal struct ContextData
        {
            /// <summary>
            /// The target context for the insertion.
            /// </summary>
            public ContextNodeModel Context { get; set; }

            /// <summary>
            /// The target index in the context for the insertion.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// The blocks to be inserted in the context.
            /// </summary>
            public IReadOnlyList<BlockNodeModel> Blocks { get; set; }
        }

        public ContextData[] Data;
        public bool Duplicate;

        public bool CorrectForPlaceholders;

        /// <summary>
        /// Initializes a new <see cref="InsertBlocksInContextCommand" />.
        /// </summary>
        /// <param name="context">The context in which to add the block.</param>
        /// <param name="index">The index in the context to which add the block.</param>
        /// <param name="blocks">The blocks to insert.</param>
        /// <param name="correctForPlaceholders">Whether the index must be corrected for eventual placeholder blocks.</param>
        /// <param name="duplicate">If true the blocks will be duplicated before being inserted.</param>
        /// <param name="undoText">The undo string.</param>
        public InsertBlocksInContextCommand(ContextNodeModel context, int index, IEnumerable<BlockNodeModel> blocks, bool correctForPlaceholders, bool duplicate = false, string undoText = null) :
            this(new[] { new ContextData { Context = context, Index = index, Blocks = blocks?.ToList() ?? Enumerable.Empty<BlockNodeModel>().ToList() }}, correctForPlaceholders, duplicate, undoText)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="InsertBlocksInContextCommand" />.
        /// </summary>
        /// <param name="data">The data containing blocks and their target context.</param>
        /// <param name="duplicate">If true the blocks will be duplicated before being inserted.</param>
        /// <param name="correctForPlaceholders">Whether the index must be corrected for eventual placeholder blocks.</param>
        /// <param name="undoText">The undo string.</param>
        public InsertBlocksInContextCommand(ContextData[] data, bool correctForPlaceholders, bool duplicate = true, string undoText = null)
        {
            if (data == null)
            {
                UndoString = k_UndoString;
                return;
            }

            if (undoText != null)
                UndoString = undoText;
            else if (data.Length > 1 || data.Length == 1 &&  data[0].Blocks.Count() > 1)
                UndoString = duplicate ? k_UndoStringDuplicatePlural : k_UndoStringPlural;
            else
                UndoString = duplicate ? k_UndoStringDuplicate : k_UndoString;

            Data = data;

            Duplicate = data.Length > 1 || duplicate;
            CorrectForPlaceholders = correctForPlaceholders;
        }

        /// <summary>
        /// Default command handler for InsertBlocksInContextCommand.
        /// </summary>
        /// <param name="undoState">The undo state component.</param>
        /// <param name="graphModelState">The graph model state component.</param>
        /// <param name="selectionState">The selection state component.</param>
        /// <param name="command">The command to handle.</param>
        [UsedImplicitly]
        public static void DefaultCommandHandler(UndoStateComponent undoState, GraphModelStateComponent graphModelState,
            SelectionStateComponent selectionState, InsertBlocksInContextCommand command)
        {
            if (command.Data == null)
                return;

            using (var undoStateUpdater = undoState.UpdateScope)
            {
                undoStateUpdater.SaveState(graphModelState);
                undoStateUpdater.SaveState(selectionState);
            }

            using (var graphUpdater = graphModelState.UpdateScope)
            using (var changeScope = graphModelState.GraphModel.ChangeDescriptionScope)
            {
                using (var selected = selectionState.UpdateScope)
                {
                    selected.ClearSelection();
                    foreach (var contextData in command.Data)
                    {
                        //Correct for any possible placeholder blocks
                        int placeHolderOffset = 0;

                        if (command.CorrectForPlaceholders)
                        {
                            for (int i = 0; i < contextData.Index + 1 && i < contextData.Context.BlockCount; ++i)
                            {
                                if (contextData.Context.GetBlock(i) == null)
                                    placeHolderOffset++;
                            }
                        }

                        IEnumerable<BlockNodeModel> newNodes;
                        if (!command.Duplicate)
                        {
                            foreach (var block in contextData.Blocks)
                            {
                                var context = block.ContextNodeModel;
                                if (context != null)
                                {
                                    context.RemoveContainerElements(new[] {block});
                                }
                            }

                            newNodes = contextData.Blocks;
                        }
                        else
                        {
                            var duplicatedNodes = new List<BlockNodeModel>();
                            foreach (var block in contextData.Blocks)
                            {
                                var blockNodeModel = block.Clone();

                                blockNodeModel.ContextNodeModel = null;

                                duplicatedNodes.Add(blockNodeModel);
                            }

                            newNodes = duplicatedNodes;
                        }

                        int currentIndex = contextData.Index;
                        foreach (var block in newNodes)
                        {
                            if (block.IsCompatibleWith(contextData.Context))
                            {
                                contextData.Context.InsertBlock(block, placeHolderOffset + currentIndex++);

                                // If the block was duplicated, mark it as new, otherwise remove it from the deleted model
                                // as the block was only moved, i.e. not deleted nor added.
                                if (command.Duplicate)
                                    graphUpdater.MarkNew(block);
                                else
                                    changeScope.ChangeDescription.RemoveDeletedModel(block);
                            }
                        }

                        if (command.Duplicate)
                        {
                            int cpt = 0;
                            foreach (var block in newNodes)
                            {
                                if (block.ContextNodeModel == contextData.Context)
                                    block.OnDuplicateNode(contextData.Blocks[cpt++]);
                                selected.SelectElement(block, true);
                            }
                        }
                        else
                        {
                            //we need to update edges for non duplicated blocks. Duplicated blocks have no edge.
                            foreach (var block in newNodes)
                            {
                                foreach (var port in block.GetPorts())
                                {
                                    foreach (var edge in port.GetConnectedWires())
                                    {
                                        graphUpdater.MarkChanged(edge, ChangeHint.GraphTopology);
                                    }
                                }
                                selected.SelectElement(block, true);
                            }
                        }
                    }
                }

                graphUpdater.MarkUpdated(changeScope.ChangeDescription);
            }
        }
    }
}
