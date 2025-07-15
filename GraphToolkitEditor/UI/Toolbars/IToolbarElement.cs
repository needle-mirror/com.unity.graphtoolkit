using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface for toolbar elements.
    /// </summary>
    /// <remarks>
    /// 'IToolbarElement' is an interface for toolbar elements. The toolbar elements can be updated by an observer. It allows for changes
    /// to the toolbar based on changes in the graph.
    /// </remarks>
    [UnityRestricted]
    internal interface IToolbarElement
    {
        /// <summary>
        /// Updates the element to reflect changes made to the model.
        /// </summary>
        /// <remarks>For example, implementation can disable the toolbar element if there is no opened graph.</remarks>
        void Update();
    }
}
