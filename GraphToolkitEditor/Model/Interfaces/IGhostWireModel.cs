using System;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface for temporary wires.
    /// </summary>
    [UnityRestricted]
    internal interface IGhostWireModel
    {
        /// <summary>
        /// The position of the start of the wire.
        /// </summary>
        Vector2 FromWorldPoint { get; set; }

        /// <summary>
        /// The position of the end of the wire.
        /// </summary>
        Vector2 ToWorldPoint { get; set; }
    }
}
