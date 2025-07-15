using System;
using UnityEditor.Overlays;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// The toolbar for the option menu.
    /// </summary>
    [Overlay(typeof(GraphViewEditorWindow), toolbarId, "Options", true,
        defaultDockZone = DockZone.TopToolbar, defaultDockPosition = DockPosition.Bottom,
        defaultDockIndex = 1000, defaultLayout = Layout.HorizontalToolbar)]
    [Icon("_Menu")]
    [UnityRestricted]
    internal sealed class OptionsMenuToolbar : Toolbar
    {
        public const string toolbarId = "gtf-options";
    }
}
