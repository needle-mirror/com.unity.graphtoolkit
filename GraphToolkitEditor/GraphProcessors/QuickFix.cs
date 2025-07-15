using System;
using Unity.GraphToolkit.CSO;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// A way to fix an error.
    /// </summary>
    [UnityRestricted]
    internal class QuickFix
    {
        /// <summary>
        /// The description of the fix.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The action to execute to fix the error.
        /// </summary>
        public Action<ICommandTarget> QuickFixAction { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickFix" /> class.
        /// </summary>
        /// <param name="description">The description of the fix.</param>
        /// <param name="quickFixAction">The action to execute to fix the error.</param>
        public QuickFix(string description, Action<ICommandTarget> quickFixAction)
        {
            Description = description;
            QuickFixAction = quickFixAction;
        }
    }
}
