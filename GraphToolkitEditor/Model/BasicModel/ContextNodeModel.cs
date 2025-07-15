using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// The model for context nodes.
    /// </summary>
    [Serializable]
    [UnityRestricted]
    internal class ContextNodeModel : NodeModel, IGraphElementContainer
    {
        [SerializeReference]
        protected List<BlockNodeModel> m_Blocks = new();

        [SerializeField, HideInInspector]
        List<Hash128> m_BlockGuids = new();

        internal static string blocksFieldName = nameof(m_Blocks);

        [NonSerialized]
        List<BlockNodePlaceholder> m_BlockPlaceholders = new List<BlockNodePlaceholder>();

        public IReadOnlyList<BlockNodeModel> BlockPlaceholders => m_BlockPlaceholders;
        public IReadOnlyList<Hash128> BlockGuids => m_BlockGuids;

        [SerializeField, NodeOption(true)]
        string m_Subtitle;

        /// <inheritdoc />
        public override string Subtitle => m_Subtitle;

        /// <summary>
        /// Whether the blocks in the context have etches.
        /// </summary>
        /// <remarks>Etches serve as markers to signify a meaningful order in the blocks.</remarks>
        public virtual bool BlocksHaveEtches => true;

        /// <summary>
        /// The text of the button's label for adding a block.
        /// </summary>
        public virtual string AddBlockText => "Add a Block";

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextNodeModel"/> class.
        /// </summary>
        public ContextNodeModel()
        {
            SetCapability(Editor.Capabilities.Collapsible, false);
            SetCapability(Editor.Capabilities.Colorable, false);
        }

        /// <inheritdoc />
        public override IEnumerable<GraphElementModel> DependentModels => base.DependentModels.Concat(m_Blocks);

        /// <summary>
        /// Inserts a block in the context.
        /// </summary>
        /// <param name="blockModel">The block model to insert.</param>
        /// <param name="index">The index at which insert the block. -1 means at the end of the list.</param>
        /// <param name="spawnFlags">The flags specifying how the node is to be spawned.</param>
        public void InsertBlock(BlockNodeModel blockModel, int index = -1, SpawnFlags spawnFlags = SpawnFlags.Default)
        {
            if (blockModel is BlockNodePlaceholder placeholder)
                m_BlockPlaceholders.Add(placeholder);
            else
            {
                // Remove the block model from its former context node model first as it will unregister the block.
                var wasRemoved = false;
                if (blockModel.ContextNodeModel != null)
                {
                    blockModel.ContextNodeModel.RemoveContainerElements(new[] { blockModel });
                    wasRemoved = true;
                }

                if ((spawnFlags & SpawnFlags.Orphan) == 0)
                {
                    GraphModel.RegisterBlockNode(blockModel);

                    // RemoveElements potentially removed the block from the graph. Since we are inserting it inside this context node
                    // instead, it is not technically removed.
                    if (wasRemoved)
                        GraphModel.CurrentGraphChangeDescription.RemoveDeletedModel(blockModel);
                }

                if (index > m_Blocks.Count)
                    throw new ArgumentException(nameof(index));

                if (!blockModel.IsCompatibleWith(this) && GetType() != typeof(ContextNodeModel)) // Blocks have to be compatible with the base ContextNodeModel because of the item library's "Dummy Context".
                    throw new ArgumentException(nameof(blockModel));

                if (index < 0 || index == m_Blocks.Count)
                {
                    m_Blocks.Add(blockModel);
                    m_BlockGuids.Add(blockModel.Guid);
                }
                else
                {
                    m_Blocks.Insert(index, blockModel);
                    m_BlockGuids.Insert(index, blockModel.Guid);
                }
            }

            blockModel.GraphModel = GraphModel;
            blockModel.ContextNodeModel = this;
            GraphModel.CurrentGraphChangeDescription.AddChangedModel(this, ChangeHint.GraphTopology);
        }

        /// <inheritdoc />
        public override void OnDuplicateNode(AbstractNodeModel sourceNode)
        {
            foreach (var block in m_Blocks)
            {
                block.GraphModel = GraphModel;
            }
            base.OnDuplicateNode(sourceNode);
        }

        /// <summary>
        /// Creates a new block and inserts it in the context.
        /// </summary>
        /// <param name="blockType">The type of block to instantiate.</param>
        /// <param name="index">The index at which insert the block. -1 means at the end of the list.</param>
        /// <param name="guid">The GUID of the new block.</param>
        /// <param name="initializationCallback">A callback called once the block is ready.</param>
        /// <param name="spawnFlags">The flags specifying how the node is to be spawned.</param>
        /// <returns>The newly created block.</returns>
        public BlockNodeModel CreateAndInsertBlock(Type blockType, int index = -1, Hash128 guid = default, Action<AbstractNodeModel> initializationCallback = null, SpawnFlags spawnFlags = SpawnFlags.Default)
        {
            var block = (BlockNodeModel)GraphModel.CreateNode(blockType, blockType.Name, Vector2.zero, guid, initializationCallback, spawnFlags);

            InsertBlock(block, index, spawnFlags);

            if (!spawnFlags.IsOrphan())
                GraphModel.CurrentGraphChangeDescription.AddNewModel(block);

            return block;
        }

        /// <summary>
        /// Creates a new block and inserts it in the context.
        /// </summary>
        /// <typeparam name="T">The type of block to instantiate.</typeparam>
        /// <param name="index">The index at which to insert the block.</param>
        /// <param name="guid">The GUID of the new block</param>
        /// <param name="initializationCallback">A callback called once the block is ready.</param>
        /// <param name="spawnFlags">The flags specifying how the node is to be spawned.</param>
        /// <returns>The newly created block.</returns>
        /// <remarks>
        /// This method creates a block of the specified type and inserts it into the specified index within the context. If the provided
        /// index is -1, the block is added to the end of the list.
        /// </remarks>
        public T CreateAndInsertBlock<T>(int index = -1, Hash128 guid = default,
            Action<AbstractNodeModel> initializationCallback = null, SpawnFlags spawnFlags = SpawnFlags.Default) where T : BlockNodeModel, new()
        {
            return (T)CreateAndInsertBlock(typeof(T), index, guid, initializationCallback, spawnFlags);
        }

        /// <inheritdoc />
        public virtual IEnumerable<GraphElementModel> GetGraphElementModels() => m_Blocks;

        /// <summary>
        /// Gets a block by its index.
        /// </summary>
        /// <param name="index">The index of the block</param>
        /// <returns>the block a its index</returns>
        public BlockNodeModel GetBlock(int index)
        {
            return m_Blocks.Count > index ? m_Blocks[index] : null;
        }

        /// <summary>
        /// Gets the first block.
        /// </summary>
        public virtual BlockNodeModel FirstBlock => m_Blocks.Count > 0 ? m_Blocks[0] : null;

        /// <summary>
        /// The number of blocks.
        /// </summary>
        public int BlockCount => m_Blocks.Count;

        /// <inheritdoc />
        public void RemoveContainerElements(IReadOnlyCollection<GraphElementModel> elementModels)
        {
            foreach (var blockNodeModel in elementModels.OfType<BlockNodeModel>())
            {
                if (GraphModel?.TryGetModelFromGuid(blockNodeModel.Guid, out _) ?? false)
                {
                    GraphModel.CurrentGraphChangeDescription.AddDeletedModel(blockNodeModel);
                    GraphModel?.UnregisterBlockNode(blockNodeModel);
                }

                if (!RemoveBlock(blockNodeModel))
                {
                    throw new ArgumentException(nameof(blockNodeModel));
                }
                blockNodeModel.ContextNodeModel = null;
            }

            if (!m_BlockPlaceholders.Any())
                SetCapability(Editor.Capabilities.Copiable, true);
            GraphModel?.CurrentGraphChangeDescription.AddChangedModel(this, ChangeHint.GraphTopology);
        }

        /// <inheritdoc/>
        protected override void OnDefineNode(NodeDefinitionScope scope)
        {
            for (var i = 0; i < BlockCount; ++i)
            {
                var blockNodeModel = GetBlock(i);
                if (blockNodeModel != null)
                {
                    blockNodeModel.ContextNodeModel = this;
                    blockNodeModel.DefineNode();
                }
            }

            if (m_BlockPlaceholders.Any())
                SetCapability(Editor.Capabilities.Copiable, false);
        }

        /// <inheritdoc />
        public bool Repair()
        {
            var dirty = false;

            for (var i = m_Blocks.Count - 1; i >= 0; i--)
            {
                if (m_Blocks[i] == null)
                {
                    if (i < m_Blocks.Count)
                    {
                        m_Blocks.RemoveAt(i);
                        dirty = true;
                    }

                    if (i < m_BlockGuids.Count)
                    {
                        m_BlockGuids.RemoveAt(i);
                        dirty = true;
                    }
                }
            }
            m_BlockPlaceholders.Clear();

            return dirty;
        }

        bool RemoveBlock(BlockNodeModel blockNodeModel)
        {
            int indexToRemove;

            if (blockNodeModel is BlockNodePlaceholder blockNodePlaceholder)
            {
                // When removing a placeholder block, we also remove the corresponding null block.
                indexToRemove = m_BlockGuids.IndexOf(blockNodePlaceholder.Guid);
                if (indexToRemove != -1)
                {
                    m_Blocks.RemoveAt(indexToRemove);
                    m_BlockGuids.RemoveAt(indexToRemove);
                    SerializationUtility.ClearManagedReferenceWithMissingType(GraphModel.GraphObject, blockNodePlaceholder.ReferenceId);
                }

                return m_BlockPlaceholders.Remove(blockNodePlaceholder);
            }

            indexToRemove = m_Blocks.IndexOf(blockNodeModel);
            if (indexToRemove != -1)
            {
                m_Blocks.RemoveAt(indexToRemove);
                m_BlockGuids.RemoveAt(indexToRemove);
                InsertNullReferencesWhileHasMissingTypes(indexToRemove);
                return true;
            }

            return false;
        }

        void InsertNullReferencesWhileHasMissingTypes(int index)
        {
            if (!SerializationUtility.HasManagedReferencesWithMissingTypes(GraphModel.GraphObject))
                return;

            // While there are references with missing types, null should be added whenever an element is removed from a list to keep its topology.
            // This is needed because the missing type registry will keep information about instances and where they were referenced from on load.
            // If the property path from which they were referenced from is either no longer containing a null or no longer available and the instance is no longer referenced from anywhere else it will be pruned out.
            m_Blocks.Insert(index, null);
        }

        /// <inheritdoc />
        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            // For compatibility with old version or corruption
            if (m_BlockGuids == null || m_BlockGuids.Count < m_Blocks.Count)
            {
                if (m_BlockGuids == null)
                    m_BlockGuids = new List<Hash128>();

                m_BlockGuids.Clear();

                m_Blocks = m_Blocks.Where(t => t != null).ToList();

                for (int i = 0; i < m_Blocks.Count; ++i)
                {
                    m_BlockGuids.Add(m_Blocks[i].Guid);
                }
            }
        }
    }
}
