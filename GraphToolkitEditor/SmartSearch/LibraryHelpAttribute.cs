using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Adds a text to a class that is used in a library to display help text in the details panel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [UnityRestricted]
    internal class LibraryHelpAttribute : Attribute
    {
        /// <summary>
        /// Help text related to the class tagged by the attribute.
        /// </summary>
        public string HelpText { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryHelpAttribute"/> class.
        /// </summary>
        public LibraryHelpAttribute(string helpText)
        {
            HelpText = helpText;
        }
    }
}
