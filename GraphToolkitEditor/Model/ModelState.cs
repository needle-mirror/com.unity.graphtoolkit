using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// The state of a graph element model.
    /// </summary>
    [UnityRestricted]
    internal enum ModelState
    {
        /// <summary>
        /// The model is enabled. This is the default value.
        /// </summary>
        Enabled = 0, // default value

        /// <summary>
        /// The model is disabled.
        /// </summary>
        Disabled,
    }
}
