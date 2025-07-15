using System;
using System.Collections.Generic;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Class that contains all the <see cref="GraphProcessor"/> used to process a graph.
    /// </summary>
    [UnityRestricted]
    internal class GraphProcessorContainer
    {
        List<GraphProcessor> m_GraphProcessors;

        /// <summary>
        /// Adds a graph processor to the container.
        /// </summary>
        /// <param name="graphProcessor">The graph processor.</param>
        public void AddGraphProcessor(GraphProcessor graphProcessor)
        {
            m_GraphProcessors ??= new List<GraphProcessor>();
            m_GraphProcessors.Add(graphProcessor);
        }

        /// <summary>
        /// Performs tasks that need to be done when the <see cref="GraphModel"/> is enabled.
        /// </summary>
        internal void OnGraphModelEnabled()
        {
            if (m_GraphProcessors == null)
            {
                return;
            }

            foreach (var processor in m_GraphProcessors)
            {
                processor.OnGraphModelEnabled();
            }
        }

        /// <summary>
        /// Performs tasks that need to be done when the <see cref="GraphModel"/> is disabled.
        /// </summary>
        internal void OnGraphModelDisabled()
        {
            if (m_GraphProcessors == null)
            {
                return;
            }

            foreach (var processor in m_GraphProcessors)
            {
                processor.OnGraphModelDisabled();
            }
        }

        /// <summary>
        /// Processes a graph using the container's graph processors.
        /// </summary>
        /// <param name="changes">A description of what changed in the graph. If null, the method assumes everything changed.</param>
        /// <returns>A list of <see cref="BaseGraphProcessingResult"/>, one for each <see cref="GraphProcessor"/>.</returns>
        public IReadOnlyList<BaseGraphProcessingResult> ProcessGraph(GraphChangeDescription changes)
        {
            var results = new List<BaseGraphProcessingResult>();

            if (m_GraphProcessors != null)
            {
                foreach (var graphProcessor in m_GraphProcessors)
                {
                    results.Add(graphProcessor.ProcessGraph(changes));
                }
            }

            return results;
        }
    }
}
