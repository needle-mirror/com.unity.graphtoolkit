using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;

namespace Unity.GraphToolkit.ItemLibrary.Editor
{
    /// <summary>
    /// Basic filter made of collections of functors
    /// </summary>
    [UnityRestricted]
    internal class ItemLibraryFuncFilter : ItemLibraryFilter
    {
        /// <summary>
        /// Empty filter, will not filter anything.
        /// </summary>
        public static ItemLibraryFuncFilter Empty => new ItemLibraryFuncFilter();

        protected List<Func<ItemLibraryItem, bool>> m_FilterFunctions = new List<Func<ItemLibraryItem, bool>>();

        /// <summary>
        /// Instantiates a filter with filtering functions.
        /// </summary>
        /// <param name="functions">Filtering functions that say whether to keep an item or not</param>
        public ItemLibraryFuncFilter(params Func<ItemLibraryItem, bool>[] functions)
        {
            m_FilterFunctions.AddRange(functions);
        }

        /// <summary>
        /// Add a filter functor to a filter in place.
        /// </summary>
        /// <param name="func">filter functor to add</param>
        /// <returns>The filter with the new functor added</returns>
        public ItemLibraryFuncFilter WithFilter(Func<ItemLibraryItem, bool> func)
        {
            m_FilterFunctions.Add(func);
            return this;
        }

        /// <inheritdoc />
        public override bool Match(ItemLibraryItem item)
        {
            return m_FilterFunctions.All(f => f(item));
        }
    }
}
