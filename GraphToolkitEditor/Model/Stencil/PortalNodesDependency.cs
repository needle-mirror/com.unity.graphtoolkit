using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Represents a dependency between two nodes linked together by portal pair.
    /// </summary>
    class PortalNodesDependency : IDependency
    {
        /// <inheritdoc />
        public AbstractNodeModel DependentNode { get; set; }
    }
}
