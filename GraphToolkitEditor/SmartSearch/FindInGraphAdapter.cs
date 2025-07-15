using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.ItemLibrary.Editor;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// <see cref="ItemLibraryAdapter"/> allowing to highlight search results in a graph.
    /// </summary>
    [UnityRestricted]
    internal class FindInGraphAdapter : SimpleLibraryAdapter
    {
        readonly Action<FindItem> m_OnHighlightDelegate;

        [UnityRestricted]
        internal class FindItem : ItemLibraryItem
        {
            public FindItem(string name, AbstractNodeModel node)
                : base(name)
            {
                Node = node;
            }

            public AbstractNodeModel Node { get; }
        }

        /// <summary>
        /// Initializes a new instance of the FindInGraphAdapter class.
        /// </summary>
        /// <param name="onHighlightDelegate">Delegate called to highlight matching items.</param>
        public FindInGraphAdapter(Action<FindItem> onHighlightDelegate)
            : base("Find in graph")
        {
            m_OnHighlightDelegate = onHighlightDelegate;
        }

        /// <inheritdoc />
        public override void OnSelectionChanged(IEnumerable<ItemLibraryItem> items)
        {
            var selectedItems = items.ToList();

            if (selectedItems.Count > 0 && selectedItems[0] is FindItem fsi)
                m_OnHighlightDelegate(fsi);

            base.OnSelectionChanged(selectedItems);
        }
    }
}
