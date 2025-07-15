using System;
using Unity.GraphToolkit.Editor;

namespace Unity.GraphToolkit.CSO
{
    /// <summary>
    /// Interface for state component updaters.
    /// </summary>
    [UnityRestricted]
    internal interface IStateComponentUpdater : IDisposable
    {
        /// <summary>
        /// Initializes the updater with the state to update.
        /// </summary>
        /// <param name="state">The state to update.</param>
        void Initialize(IStateComponent state);

        /// <summary>
        /// Moves the content of a state component loaded from persistent storage into this state component.
        /// </summary>
        /// <param name="other">The source state component.</param>
        /// <remarks>The <paramref name="other"/> state components will be discarded after the call to
        /// <see cref="RestoreFromPersistedState"/>.
        /// This means you do not need to make a deep copy of the data: just copying the references is sufficient.
        /// </remarks>
        void RestoreFromPersistedState(IStateComponent other);

        /// <summary>
        /// Moves the content of a state component obtained from the undo stack into this state component.
        /// </summary>
        /// <param name="other">The source state component.</param>
        /// <param name="changeset">A description of the changes.</param>
        /// <remarks>
        /// The <paramref name="other"/> state components will be discarded after the call to <see cref="RestoreFromUndo"/>.
        /// This means you do not need to make a deep copy of the data: just copying the references is sufficient.
        /// </remarks>
        void RestoreFromUndo(IStateComponent other, IChangeset changeset);
    }
}
