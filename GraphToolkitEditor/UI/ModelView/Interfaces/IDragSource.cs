using System;
using System.Collections.Generic;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface for views that can act as the source of a drag and drop operation.
    /// </summary>
    [UnityRestricted]
    internal interface IDragSource
    {
        /// <summary>
        /// Get the currently selected graph element models.
        /// </summary>
        /// <returns>The currently selected graph element models.</returns>
        IReadOnlyList<GraphElementModel> GetSelection();
    }
}
