using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface to be implemented by a <see cref="AbstractNodeModel"/> when the local inspector should display some
    /// other object instead of the node.
    /// </summary>
    [UnityRestricted]
    internal interface IHasInspectorSurrogate
    {
        object Surrogate { get; }
    }
}
