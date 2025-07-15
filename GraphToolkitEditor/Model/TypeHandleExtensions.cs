using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// <see cref="TypeHandle"/> extension methods.
    /// </summary>
    static class TypeHandleExtensions
    {
        /// <summary>
        /// Returns whether the type represented by <paramref name="self"/> is assignable from the type
        /// represented by <paramref name="other"/>.
        /// </summary>
        /// <param name="self">The type we want to query.</param>
        /// <param name="other">The type we want to know if <paramref name="self"/> is assignable to.</param>
        /// <returns>True if <paramref name="self"/> is assignable from <paramref name="other"/>, false otherwise.</returns>
        public static bool IsAssignableFrom(this TypeHandle self, TypeHandle other)
        {
            var selfType = self.Resolve();
            var otherType = other.Resolve();
            return selfType.IsAssignableFrom(otherType);
        }
    }
}
