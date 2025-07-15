using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Type of dependencies for a <see cref="ModelView"/>.
    /// </summary>
    [Flags]
    [UnityRestricted]
    internal enum DependencyTypes
    {
        None = 0,
        Style = 1,
        Geometry = 2,
        Removal = 4,
        Any = Style | Geometry | Removal
    }

    /// <summary>
    /// Extensions methods for <see cref="DependencyTypes"/>.
    /// </summary>
    static class DependencyTypeExtensions
    {
        /// <summary>
        /// Checks if <paramref name="value"/> has the flag <paramref name="flag"/> set.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="flag">The flag to check.</param>
        /// <returns>True if value has the flag set.</returns>
        public static bool HasFlagFast(this DependencyTypes value, DependencyTypes flag)
        {
            return (value & flag) != 0;
        }
    }
}
