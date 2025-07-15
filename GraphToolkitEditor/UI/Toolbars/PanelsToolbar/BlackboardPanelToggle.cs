using System;
using Unity.GraphToolsAuthoringFramework.InternalEditorBridge;
using UnityEditor;
using UnityEditor.Toolbars;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Toolbar button to toggle the display of the blackboard.
    /// </summary>
    [EditorToolbarElement(id, typeof(GraphViewEditorWindow))]
    [UnityRestricted]
    internal sealed class BlackboardPanelToggle : PanelToggle
    {
        public const string id = "GraphToolkit/Overlay Windows/Blackboard";

        /// <inheritdoc />
        protected override string WindowId => BlackboardOverlay.idValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlackboardPanelToggle"/> class.
        /// </summary>
        public BlackboardPanelToggle()
        {
            name = "Blackboard";
            tooltip = L10n.Tr("Blackboard");
            icon = EditorGUIUtilityBridge.LoadIcon("Packages/com.unity.graphtoolkit/GraphToolkitEditor/Icons/PanelsToolbar/Blackboard.png");
        }
    }
}
