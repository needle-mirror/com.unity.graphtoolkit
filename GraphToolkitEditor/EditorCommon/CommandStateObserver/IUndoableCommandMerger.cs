namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface for merging multiple commands into a single undo operation.
    /// </summary>
    [UnityRestricted]
    internal interface IUndoableCommandMerger
    {
        /// <summary>
        /// Indicate that you want to merge the next undoable commands into one undo.
        /// </summary>
        void StartMergingUndoableCommands();

        /// <summary>
        /// Ends the merging of undoables commands into one undo.
        /// </summary>
        void StopMergingUndoableCommands();
    }
}
