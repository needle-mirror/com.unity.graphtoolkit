using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface for the model of a node that has a single output port.
    /// </summary>
    [UnityRestricted]
    internal interface ISingleOutputPortNodeModel
    {
        /// <summary>
        /// Gets the model of the output port for this node.
        /// </summary>
        PortModel OutputPort { get; }
    }
}
