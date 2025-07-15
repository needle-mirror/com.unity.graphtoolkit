using System;
using Unity.GraphToolkit.ItemLibrary.Editor;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Simple <see cref="ItemLibraryAdapter"/> without preview.
    /// </summary>
    [UnityRestricted]
    internal class SimpleLibraryAdapter : ItemLibraryAdapter
    {
        // TODO: Disable details panel for now
        /// <inheritdoc />
        public override bool HasDetailsPanel => false;

        public SimpleLibraryAdapter(string title, string toolName = null)
            : base(title, toolName)
        {
        }
    }
}
