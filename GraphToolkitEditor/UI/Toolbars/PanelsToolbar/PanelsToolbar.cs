using System;
using UnityEditor.Overlays;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// The panel toggle toolbar.
    /// </summary>
    [Overlay(typeof(GraphViewEditorWindow), toolbarId, "Panel Toggles", ussName = "PanelToggles",
        defaultDisplay = true, defaultDockZone = DockZone.TopToolbar, defaultDockPosition = DockPosition.Bottom,
        defaultDockIndex = 1, defaultLayout = Layout.HorizontalToolbar)]
    [Icon("Packages/com.unity.graphtoolkit/GraphToolkitEditor/Icons/PanelsToolbar/Panels.png")]
    [UnityRestricted]
    internal sealed class PanelsToolbar : Toolbar
    {
        public const string toolbarId = "gtf-panel-toggles-toolbar";
    }
}
