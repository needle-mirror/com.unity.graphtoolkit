using System;
using Unity.GraphToolkit.ItemLibrary.Editor;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// <see cref="IItemLibraryAdapter"/> used in GraphToolkit
    /// </summary>
    interface IGraphElementLibraryAdapter : IItemLibraryAdapter
    {
        /// <summary>
        /// Sets the graphview calling the searcher.
        /// </summary>
        /// <param name="graphView">The host graph view.</param>
        /// <param name="size">The library size.</param>
        /// <remarks>Used to instantiate an appropriate preview graphview.</remarks>
        void SetHostGraphView(GraphView graphView, ItemLibrarySize size);
    }
}
