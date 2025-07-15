using Unity.GraphToolkit.Editor;

namespace Unity.GraphToolkit.CSO
{
    /// <summary>
    /// Extension methods for <see cref="IHierarchicalCommandTarget"/>.
    /// </summary>
    [UnityRestricted]
    internal static class HierarchicalCommandTargetExtensions
    {
        /// <summary>
        /// Dispatches a command to a command target and its ancestors.
        /// </summary>
        /// <param name="self">The command target to dispatch the command to.</param>
        /// <param name="command">The command to dispatch.</param>
        /// <param name="diagnosticsFlags">Diagnostic flags for the dispatch process.</param>
        public static void DispatchToHierarchy(this IHierarchicalCommandTarget self, ICommand command, Diagnostics diagnosticsFlags = Diagnostics.None)
        {
            while (self != null)
            {
                self.DispatchToSelf(command, diagnosticsFlags);
                self = self.ParentTarget;
            }
        }
    }
}
