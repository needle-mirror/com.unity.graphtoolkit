using System;
using Unity.GraphToolkit.ItemLibrary.Editor;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// interface for ModelUI that will show the <see cref="ItemLibraryWindow"/>.
    /// </summary>
    interface IShowItemLibraryUI
    {
        /// <summary>
        /// Shows the <see cref="ItemLibraryWindow"/>.
        /// </summary>
        /// <param name="mousePosition">The mouse position in window coordinates.</param>
        /// <returns>True if a <see cref="ItemLibraryWindow"/> could be displayed.</returns>
        bool ShowItemLibrary(Vector2 mousePosition);
    }
}
