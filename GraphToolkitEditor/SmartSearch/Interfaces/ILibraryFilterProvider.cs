using System;
using System.Collections.Generic;
using Unity.GraphToolkit.ItemLibrary.Editor;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface to provide different <see cref="ItemLibraryFilter"/> depending on context.
    /// </summary>
    [UnityRestricted]
    internal interface ILibraryFilterProvider
    {
        /// <summary>
        /// Gets the <see cref="ItemLibraryFilter"/> to apply for general search in a graph.
        /// </summary>
        /// <returns>The filter.</returns>
        ItemLibraryFilter GetGraphFilter();

        /// <summary>
        /// Gets the <see cref="ItemLibraryFilter"/> to apply during a search for graph element connecting to outputs.
        /// </summary>
        /// <param name="portModels">Ports to connect the search result to.</param>
        /// <returns>The filter.</returns>
        ItemLibraryFilter GetOutputToGraphFilter(IEnumerable<PortModel> portModels);

        /// <summary>
        /// Gets the <see cref="ItemLibraryFilter"/> to apply during a search for graph element connecting to an output.
        /// </summary>
        /// <param name="portModel">Port to connect the search result to.</param>
        /// <returns>The filter.</returns>
        ItemLibraryFilter GetOutputToGraphFilter(PortModel portModel);

        /// <summary>
        /// Gets the <see cref="ItemLibraryFilter"/> to apply during a search for graph element connecting to inputs.
        /// </summary>
        /// <param name="portModels">Ports to connect the search result to.</param>
        /// <returns>The filter.</returns>
        ItemLibraryFilter GetInputToGraphFilter(IEnumerable<PortModel> portModels);

        /// <summary>
        /// Gets the <see cref="ItemLibraryFilter"/> to apply during a search for graph element connecting to an input.
        /// </summary>
        /// <param name="portModel">Port to connect the search result to.</param>
        /// <returns>The filter.</returns>
        ItemLibraryFilter GetInputToGraphFilter(PortModel portModel);

        /// <summary>
        /// Gets the <see cref="ItemLibraryFilter"/> to apply during a search for graph element connecting to an input.
        /// </summary>
        /// <param name="wireModel">Wire to connect the search result to.</param>
        /// <returns>The filter.</returns>
        ItemLibraryFilter GetWireFilter(WireModel wireModel);

        /// <summary>
        /// Gets the <see cref="ItemLibraryFilter"/> for a given context.
        /// </summary>
        /// <param name="contextNodeModel">The context the filter references.</param>
        /// <returns>The filter for the given context.</returns>
        ItemLibraryFilter GetContextFilter(ContextNodeModel contextNodeModel);
    }
}
