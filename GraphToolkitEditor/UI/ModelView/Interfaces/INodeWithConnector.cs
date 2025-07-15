using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface for nodes that have connectors to connect wires without visually using ports.
    /// </summary>
    [UnityRestricted]
    internal interface INodeWithConnector
    {
        /// <summary>
        /// Shows the connector for the given wire.
        /// </summary>
        /// <param name="wire">The wire to show the connector for.</param>
        void ShowConnector(AbstractWire wire);

        /// <summary>
        /// Hides the connector for the given wire.
        /// </summary>
        /// <param name="wire">The wire to hide the connector for.</param>
        void HideConnector(AbstractWire wire);
    }
}
