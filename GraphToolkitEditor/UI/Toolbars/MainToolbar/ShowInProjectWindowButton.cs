using System;
using Unity.GraphToolsAuthoringFramework.InternalEditorBridge;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Toolbar button to focus the current graph's asset in the Project Window.
    /// </summary>
    [EditorToolbarElement(id, typeof(GraphViewEditorWindow))]
    [UnityRestricted]
    internal class ShowInProjectWindowButton : MainToolbarButton
    {
        public const string id = "GraphToolkit/Main/ShowInProjectWindow";

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowInProjectWindowButton"/> class.
        /// </summary>
        public ShowInProjectWindowButton()
        {
            name = "ShowInProjectWindow";
            tooltip = L10n.Tr("Show in Project Window");
            clicked += OnClick;
            icon = EditorGUIUtilityBridge.LoadIcon($"Packages/com.unity.graphtoolkit/GraphToolkitEditor/Icons/MainToolbar_Overlay/FileAccess@4x.png");
        }

        /// <inheritdoc />
        protected override void OnClick()
        {
            var graphEditorWindow = containerWindow as GraphViewEditorWindow;
            if (graphEditorWindow is null)
                return;

            var graphObject = graphEditorWindow.GraphView?.GraphViewModel.GraphModelState.GraphModel.GraphObject;
            if (graphObject != null)
            {
                EditorUtility.FocusProjectWindow();
                var obj = AssetDatabase.LoadMainAssetAtPath(graphObject.FilePath);
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);
            }
        }
    }
}
