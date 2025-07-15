using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface for a Context for the creation of a view.
    /// </summary>
    /// <remarks>Useful to create a different view based on the context.</remarks>
    [UnityRestricted]
    internal interface IViewContext : IEquatable<IViewContext>
    {
    }
}
