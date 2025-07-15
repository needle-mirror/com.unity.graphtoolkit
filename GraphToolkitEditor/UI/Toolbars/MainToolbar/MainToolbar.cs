using System;
using UnityEditor.Overlays;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// The main toolbar.
    /// </summary>
    [Overlay(typeof(GraphViewEditorWindow), toolbarId, "Asset Management", ussName = "AssetManagement",
        defaultDisplay = true, defaultDockZone = DockZone.TopToolbar, defaultDockPosition = DockPosition.Top,
        defaultDockIndex = 0, defaultLayout = Layout.HorizontalToolbar)]
    [Icon("Icons/Overlays/ToolsToggle.png")]
    [UnityRestricted]
    internal sealed class MainToolbar : Toolbar
    {
        /// <summary>
        /// The identification of the <see cref="MainToolbar"/>.
        /// </summary>
        public const string toolbarId = "gtf-asset-management";
    }
}
