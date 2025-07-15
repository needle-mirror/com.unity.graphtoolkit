using System;
using Unity.GraphToolsAuthoringFramework.InternalEditorBridge;
using UnityEditor;
using UnityEditor.Toolbars;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Toolbar element to navigate to the previous error in the graph.
    /// </summary>
    [EditorToolbarElement(id, typeof(GraphViewEditorWindow))]
    [UnityRestricted]
    internal class PreviousErrorButton : ErrorToolbarButton
    {
        public const string id = "GraphToolkit/Error/Previous";

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviousErrorButton"/> class.
        /// </summary>
        public PreviousErrorButton()
        {
            name = "PreviousError";
            tooltip = L10n.Tr("Previous Error");
            icon = EditorGUIUtilityBridge.LoadIcon("Packages/com.unity.graphtoolkit/GraphToolkitEditor/Icons/ErrorToolbar/PreviousError.png");
        }

        /// <inheritdoc />
        protected override void OnClick()
        {
            var state = GraphView?.GraphViewModel.GraphViewState;
            if (state != null)
                FrameAndSelectError(state.ErrorIndex - 1);
        }
    }
}
