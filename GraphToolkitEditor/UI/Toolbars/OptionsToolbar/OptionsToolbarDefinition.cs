using System;
using System.Collections.Generic;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Class that defines the content of the options toolbar.
    /// </summary>
    [UnityRestricted]
    internal class OptionsToolbarDefinition : ToolbarDefinition
    {
        /// <inheritdoc />
        public override IEnumerable<string> ElementIds => new[] { OptionDropDownMenu.id };
    }
}
