using System;
using Unity.GraphToolkit.CSO;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface for state components that store data associated with an graph and/or a view.
    /// </summary>
    [UnityRestricted]
    internal interface IPersistedStateComponent : IStateComponent
    {
        /// <summary>
        /// The unique ID of the associated view. Set to `default` if the state component is not associated with any view.
        /// </summary>
        Hash128 ViewGuid { get; set; }

        /// <summary>
        /// A unique key for the graph associated with this state component. Set to `default` if the state component is not associated with any graph.
        /// </summary>
        string GraphKey { get; set; }
    }
}
