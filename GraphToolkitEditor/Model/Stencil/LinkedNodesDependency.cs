using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Represents a dependency between two nodes linked together by a wire.
    /// </summary>
    [UnityRestricted]
    internal class LinkedNodesDependency : IDependency
    {
        /// <summary>
        /// The dependent port.
        /// </summary>
        public PortModel DependentPort { get; set; }

        /// <summary>
        /// The parent port.
        /// </summary>
        public PortModel ParentPort { get; set; }

        /// <inheritdoc />
        public AbstractNodeModel DependentNode => DependentPort.NodeModel;

        /// <summary>
        /// The number of such a dependency in a graph.
        /// </summary>
        public int Count { get; set; }
    }
}
