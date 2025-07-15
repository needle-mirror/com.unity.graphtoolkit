using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace Unity.GraphToolkit.CSO
{
    /// <summary>
    /// Interface for changesets of <see cref="IStateComponent"/>.
    /// </summary>
    [UnityRestricted]
    internal interface IChangeset
    {
        private static List<IChangeset> s_SingleChangesetList = new(1);

        /// <summary>
        /// Makes this changeset a copy of <paramref name="changeset"/>.
        /// </summary>
        /// <param name="changeset">The changesets to copy.</param>
        void Copy(IChangeset changeset)
        {
            s_SingleChangesetList.Add(changeset);
            AggregateFrom(s_SingleChangesetList);
            s_SingleChangesetList.Clear();
        }

        /// <summary>
        /// Makes the changeset empty.
        /// </summary>
        void Clear();

        /// <summary>
        /// Makes this changeset a changeset that summarize <paramref name="changesets"/>.
        /// </summary>
        /// <param name="changesets">The changesets to summarize.</param>
        void AggregateFrom(IReadOnlyList<IChangeset> changesets);

        /// <summary>
        /// Reverse the direction of the changeset.
        /// </summary>
        /// <returns>True it the changeset could be reversed, false otherwise.</returns>
        /// <remarks>
        /// For example, if the changeset contains a list of created objects, a list of changed objects
        /// and a list of deleted objects, the created and deleted lists should be swapped, and the
        /// changed list remains the same.
        /// </remarks>
        bool Reverse();
    }
}
