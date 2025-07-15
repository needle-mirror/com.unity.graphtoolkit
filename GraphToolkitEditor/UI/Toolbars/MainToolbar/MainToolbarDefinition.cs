using System.Collections.Generic;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Class that defines the content of the main toolbar.
    /// </summary>
    [UnityRestricted]
    internal class MainToolbarDefinition : ToolbarDefinition
    {
        /// <inheritdoc />
        public override IEnumerable<string> ElementIds => new[] { NewGraphButton.id, SaveButton.id, ShowInProjectWindowButton.id };
    }
}
