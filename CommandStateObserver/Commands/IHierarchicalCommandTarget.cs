using Unity.GraphToolkit.Editor;

namespace Unity.GraphToolkit.CSO
{
    /// <summary>
    /// A <see cref="ICommandTarget"/> that also send the command to its parent.
    /// </summary>
    [UnityRestricted]
    internal interface IHierarchicalCommandTarget : ICommandTarget
    {
        /// <summary>
        /// The parent target.
        /// </summary>
        IHierarchicalCommandTarget ParentTarget { get; }

        /// <summary>
        /// Dispatches a command to this target, without sending it to its parent.
        /// </summary>
        /// <param name="command">The command to dispatch.</param>
        /// <param name="diagnosticsFlags">Diagnostic flags for the dispatch process.</param>
        void DispatchToSelf(ICommand command, Diagnostics diagnosticsFlags = Diagnostics.None);
    }
}
