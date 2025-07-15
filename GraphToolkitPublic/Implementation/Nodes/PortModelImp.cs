using System;

namespace Unity.GraphToolkit.Editor.Implementation
{
    class PortModelImp : PortModel
    {
        public PortConnectorUI ConnectorUI { get; set; }

        public override string DefaultTooltip => ComputePortLabel(true) + ": " +(Direction == PortDirection.Output ? "Output" : "Input") +
            (((IPort)this).dataType == null ? string.Empty : $" of type {DataTypeHandle.FriendlyName}");

        public PortModelImp(PortNodeModel nodeModel, PortDirection direction, PortOrientation orientation, string portName, PortType portType, TypeHandle dataType, string portId, PortModelOptions options, Attribute[] attributes, PortModel parentPort)
            : base(nodeModel, direction, orientation, portName, portType, dataType, portId, options, attributes, parentPort) { }
    }
}
