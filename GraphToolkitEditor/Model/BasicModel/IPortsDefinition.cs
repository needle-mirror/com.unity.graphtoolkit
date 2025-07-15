using System;

namespace Unity.GraphToolkit.Editor
{
    interface IPortsDefinition
    {
        /// <summary>
        /// Adds a new input port on the node.
        /// </summary>
        /// <param name="portName">The name of port to create.</param>
        /// <param name="dataType">The type of data the port to create handles.</param>
        /// <param name="portId">The ID of the port to create.</param>
        /// <param name="orientation">The orientation of the port to create.</param>
        /// <param name="attributes">The attributes used to convey information about the port, if any.</param>
        /// <param name="defaultValue">The default value to assign to the constant associated to the port.</param>
        /// <param name="connectorUI">The visual used for the port connector.</param>
        IPort AddInputPort(string portName, Type dataType = null,
                string portId = null, PortOrientation orientation = PortOrientation.Horizontal,
                Attribute[] attributes = null, object defaultValue = default);

        /// <summary>
        /// Adds a new output port on the node.
        /// </summary>
        /// <param name="portName">The name of port to create.</param>
        /// <param name="dataType">The type of data the port to create handles.</param>
        /// <param name="portId">The ID of the port to create.</param>
        /// <param name="orientation">The orientation of the port to create.</param>
        /// <param name="attributes">The attributes used to convey information about the port, if any.</param>
        /// <param name="connectorUI">The visual used for the port connector.</param>
        IPort AddOutputPort(string portName, Type dataType = null,
            string portId = null, PortOrientation orientation = PortOrientation.Horizontal, Attribute[] attributes = null);
    }
}
