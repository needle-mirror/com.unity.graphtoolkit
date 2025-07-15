using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Specifies the direction of a port in the graph.
    /// </summary>
    /// <remarks>
    /// Use <see cref="PortDirection"/> to indicate whether a port receives or sends information. This enum supports bitwise operations
    /// which allows for combinations of values, although most ports are either <see cref="Input"/> or <see cref="Output"/>.
    /// The direction affects how the port is used in the graph.
    /// <see cref="Input"/> ports receive connections and typically appear on the left side of a node.
    /// <see cref="Output"/> ports initiate connections and typically appear on the right side of a node.
    /// Use <see cref="None"/> when a port has no fixed direction.
    /// </remarks>
    [Flags]
    public enum PortDirection
    {
        /// <summary>
        /// The port does not have a fixed direction. It may receive or send information.
        /// </summary>
        None = 0,

        /// <summary>
        /// The port receives information. It accepts connections from output ports.
        /// </summary>
        Input = 1,

        /// <summary>
        /// The port sends information. It creates connections to input ports.
        /// </summary>
        Output = 2
    }
}
