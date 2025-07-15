using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface for the model of a node that has a single input port.
    /// </summary>
    [UnityRestricted]
    internal interface ISingleInputPortNodeModel
    {
        /// <summary>
        /// Gets the model of the input port for this node.
        /// </summary>
        PortModel InputPort { get; }
    }
}
