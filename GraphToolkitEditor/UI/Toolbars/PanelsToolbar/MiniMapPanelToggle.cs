using System;
using Unity.GraphToolsAuthoringFramework.InternalEditorBridge;
using UnityEditor;
using UnityEditor.Toolbars;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Toolbar button to toggle the display of the minimap.
    /// </summary>
    [EditorToolbarElement(id, typeof(GraphViewEditorWindow))]
    [UnityRestricted]
    internal sealed class MiniMapPanelToggle : PanelToggle
    {
        public const string id = "GraphToolkit/Overlay Windows/MiniMap";

        /// <inheritdoc />
        protected override string WindowId => MiniMapOverlay.idValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="MiniMapPanelToggle"/> class.
        /// </summary>
        public MiniMapPanelToggle()
        {
            name = "MiniMap";
            tooltip = L10n.Tr("MiniMap");
            icon = EditorGUIUtilityBridge.LoadIcon("Packages/com.unity.graphtoolkit/GraphToolkitEditor/Icons/PanelsToolbar/MiniMap.png");
        }
    }
}
