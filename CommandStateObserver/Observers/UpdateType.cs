using System;
using Unity.GraphToolkit.Editor;

namespace Unity.GraphToolkit.CSO
{
    /// <summary>
    /// The type of update an observer should do.
    /// </summary>
    [UnityRestricted]
    internal enum UpdateType
    {
        /// <summary>
        /// The <see cref="IStateComponent"/> has not changed since the last observation.
        /// </summary>
        None,

        /// <summary>
        /// The <see cref="IStateComponent"/> can provide a description of what changed since the last observation.
        /// </summary>
        Partial,

        /// <summary>
        /// The <see cref="IStateComponent"/> cannot provide a description of what changed.
        /// Any data held by the <see cref="IStateComponent"/> may have changed.
        /// </summary>
        Complete,
    }
}
