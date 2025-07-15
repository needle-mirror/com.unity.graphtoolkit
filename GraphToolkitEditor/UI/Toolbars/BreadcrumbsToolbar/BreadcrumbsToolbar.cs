using System;
using UnityEditor.Overlays;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// The toolbar that displays the breadcrumbs.
    /// </summary>
    [Overlay(typeof(GraphViewEditorWindow), toolbarId, "Breadcrumbs", true,
        defaultDockZone = DockZone.TopToolbar, defaultDockPosition = DockPosition.Top,
        defaultDockIndex = 1000, defaultLayout = Layout.HorizontalToolbar)]
    [Icon("Packages/com.unity.graphtoolkit/GraphToolkitEditor/Icons/BreadcrumbsToolbar/Breadcrumb.png")]
    [UnityRestricted]
    internal sealed class BreadcrumbsToolbar : Toolbar
    {
        public const string toolbarId = "gtf-breadcrumbs";

        /// <inheritdoc />
        protected override Layout supportedLayouts => Layout.HorizontalToolbar;
    }
}
