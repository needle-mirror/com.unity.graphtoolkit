using Unity.GraphToolkit.Editor;

namespace Unity.GraphToolkit.ItemLibrary.Editor
{
    /// <summary>
    /// Base class to filter databases.
    /// </summary>
    [UnityRestricted]
    internal abstract class ItemLibraryFilter
    {
        /// <summary>
        /// Checks if an item matches the filter.
        /// </summary>
        /// <param name="item">item the check</param>
        /// <returns>true if the item matches the filter, false otherwise.</returns>
        public abstract bool Match(ItemLibraryItem item);
    }
}
