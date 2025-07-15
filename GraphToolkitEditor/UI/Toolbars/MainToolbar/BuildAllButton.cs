using System;
using Unity.GraphToolsAuthoringFramework.InternalEditorBridge;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Toolbar button to build the graph.
    /// </summary>
    [EditorToolbarElement(id, typeof(GraphViewEditorWindow))]
    [UnityRestricted]
    internal class BuildAllButton : MainToolbarButton
    {
        public const string id = "GraphToolkit/Main/Build All";

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildAllButton"/> class.
        /// </summary>
        public BuildAllButton()
        {
            name = "BuildAll";
            tooltip = L10n.Tr("Build All");
            icon = EditorGUIUtilityBridge.LoadIcon("Packages/com.unity.graphtoolkit/GraphToolkitEditor/Icons/MainToolbar_Overlay/BuildAll.png");
        }

        /// <inheritdoc />
        protected override void OnClick()
        {
            try
            {
                GraphTool?.Dispatch(new BuildAllEditorCommand());
            }
            catch (Exception e) // so the button doesn't get stuck
            {
                Debug.LogException(e);
            }
        }
    }
}
