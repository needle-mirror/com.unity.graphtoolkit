using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface that represents a dependency.
    /// </summary>
    [UnityRestricted]
    internal interface IDependency
    {
        /// <summary>
        /// The dependant node in the dependency.
        /// </summary>
        AbstractNodeModel DependentNode { get; }
    }
}
