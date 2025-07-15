using System;
using Unity.GraphToolkit.Editor;

namespace Unity.GraphToolkit.CSO
{
    /// <summary>
    /// Base class for undoable commands.
    /// </summary>
    [UnityRestricted]
    internal abstract class UndoableCommand : ICommand
    {
        /// <summary>
        /// The string that should appear in the Edit/Undo menu after this command is executed.
        /// </summary>
        public string UndoString { get; set; }
    }
}
