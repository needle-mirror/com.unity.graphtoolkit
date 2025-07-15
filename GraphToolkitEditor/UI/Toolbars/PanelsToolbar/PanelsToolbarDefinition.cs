using System;
using System.Collections.Generic;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Class that defines the content of the panels toolbar.
    /// </summary>
    [UnityRestricted]
    internal class PanelsToolbarDefinition : ToolbarDefinition
    {
        /// <inheritdoc />
        public override IEnumerable<string> ElementIds => new[] { BlackboardPanelToggle.id, InspectorPanelToggle.id, MiniMapPanelToggle.id };
    }
}
