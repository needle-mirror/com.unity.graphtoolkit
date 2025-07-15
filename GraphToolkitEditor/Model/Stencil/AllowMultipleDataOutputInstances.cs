using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Options to allow multiple data output instances.
    /// </summary>
    /// <remarks>
    /// 'AllowMultipleDataOutputInstances' defines options to control whether multiple data output instances are permitted.
    /// Use this enum when configuring data output behavior to ensure that the appropriate restrictions or warnings are enforced.
    /// </remarks>
    [UnityRestricted]
    internal enum AllowMultipleDataOutputInstances
    {
        /// <summary>
        /// Permits multiple data output instances without restrictions.
        /// </summary>
        Allow,
        /// <summary>
        /// Prevents multiple data output instances.
        /// </summary>
        Disallow,
        /// <summary>
        /// Permits multiple data output instances but issues a warning.
        /// </summary>
        AllowWithWarning
    }
}
