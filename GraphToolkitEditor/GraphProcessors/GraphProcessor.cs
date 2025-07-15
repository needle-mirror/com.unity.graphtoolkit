using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Base class for graph processors.
    /// </summary>
    [UnityRestricted]
    internal abstract class GraphProcessor
    {
        /// <summary>
        /// Performs tasks that need to be done when the <see cref="GraphModel"/> is enabled.
        /// </summary>
        public virtual void OnGraphModelEnabled() {}

        /// <summary>
        /// Performs tasks that need to be done when the <see cref="GraphModel"/> is disabled.
        /// </summary>
        public virtual void OnGraphModelDisabled() {}

        /// <summary>
        /// Processes the graph.
        /// </summary>
        /// <param name="changes">A description of what changed in the graph. If null, the method assumes everything changed.</param>
        /// <returns>The results of the processing.</returns>
        public abstract BaseGraphProcessingResult ProcessGraph(GraphChangeDescription changes);
    }
}
