using System;

// ReSharper disable InconsistentNaming

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// The part to build the UI for vertical port containers.
    /// </summary>
    [UnityRestricted]
    internal class VerticalPortContainerPart : BasePortContainerPart
    {
        /// <summary>
        /// The USS class name added to the part.
        /// </summary>
        public static readonly string ussClassName = "ge-vertical-port-container-part";

        /// <summary>
        /// The USS class name added to the port container.
        /// </summary>
        public static readonly string portsUssName = "vertical-port-container";

        /// <inheritdoc />
        protected override bool SetCountClassOnParent => true;

        /// <summary>
        /// Creates a new <see cref="VerticalPortContainerPart"/>.
        /// </summary>
        /// <param name="name">The name of the part to create.</param>
        /// <param name="model">The model which the part represents.</param>
        /// <param name="ownerUI">The owner of the part to create.</param>
        /// <param name="parentClassName">The class name of the parent UI.</param>
        /// <param name="portFilter">A filter used to select the ports to display in the container.</param>
        /// <returns>A new instance of <see cref="VerticalPortContainerPart"/>.</returns>
        public static VerticalPortContainerPart Create(string name, Model model, ChildView ownerUI, string parentClassName, Func<PortModel, bool> portFilter = null)
        {
            return model is PortNodeModel ? new VerticalPortContainerPart(name, model, ownerUI, parentClassName, portFilter) : null;
        }

        /// <summary>
        /// Creates a new VerticalPortContainerPart.
        /// </summary>
        /// <param name="name">The name of the part to create.</param>
        /// <param name="model">The model which the part represents.</param>
        /// <param name="ownerUI">The owner of the part to create.</param>
        /// <param name="parentClassName">The class name of the parent UI.</param>
        /// <param name="portFilter">A filter used to select the ports to display in the container.</param>
        protected VerticalPortContainerPart(string name, Model model, ChildView ownerUI, string parentClassName, Func<PortModel, bool> portFilter)
            : base(name, model, ownerUI, parentClassName, portsUssName, ussClassName,
                   portFilter == null ? verticalPortFilter : p => verticalPortFilter(p) && portFilter(p)) {}
    }
}
