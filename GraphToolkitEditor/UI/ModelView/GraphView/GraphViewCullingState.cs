namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// The state of culling in a <see cref="GraphView"/>.
    /// </summary>
    [UnityRestricted]
    internal enum GraphViewCullingState
    {
        /// <summary>
        /// Culling is disabled.
        /// </summary>
        Disabled,

        /// <summary>
        /// Culling is enabled.
        /// </summary>
        Enabled
    }
}
