namespace Unity.GraphToolkit.ItemLibrary.Editor
{
    /// <summary>
    /// View model for any item in the library tree view.
    /// </summary>
    interface ITreeItemView
    {
        /// <summary>
        /// Parent of this item in the hierarchy.
        /// </summary>
        public ICategoryView Parent { get; }

        /// <summary>
        /// Custom name used to generate USS styles when creating UI for this item.
        /// </summary>
        public string StyleName { get; }

        /// <summary>
        /// Depth of this item in the hierarchy.
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// Path in the hierarchy of items.
        /// </summary>
        /// <returns>The path.</returns>
        public string GetPath();

        /// <summary>
        /// Name of the Item.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Help content to display about this item.
        /// </summary>
        public string Help { get; }
    }
}
