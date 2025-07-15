using System;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface to provide the data needed to create a node in a graph.
    /// </summary>
    [UnityRestricted]
    internal interface IGraphNodeCreationData
    {
        /// <summary>
        /// Option used on node creation.
        /// </summary>
        SpawnFlags SpawnFlags { get; }
        /// <summary>
        /// Graph where to create the node.
        /// </summary>
        GraphModel GraphModel { get; }
        /// <summary>
        /// Position where to create the node.
        /// </summary>
        Vector2 Position { get; }
        /// <summary>
        /// Guid to give to the node on creation.
        /// </summary>
        Hash128 Guid { get; }
    }
}
