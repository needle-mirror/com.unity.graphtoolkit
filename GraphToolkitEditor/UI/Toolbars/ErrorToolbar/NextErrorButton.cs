using System;
using Unity.GraphToolsAuthoringFramework.InternalEditorBridge;
using UnityEditor;
using UnityEditor.Toolbars;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Toolbar element to navigate to the next error in the graph.
    /// </summary>
    [EditorToolbarElement(id, typeof(GraphViewEditorWindow))]
    [UnityRestricted]
    internal class NextErrorButton : ErrorToolbarButton
    {
        public const string id = "GraphToolkit/Error/Next";

        /// <summary>
        /// Initializes a new instance of the <see cref="NextErrorButton"/> class.
        /// </summary>
        public NextErrorButton()
        {
            name = "NextError";
            tooltip = L10n.Tr("Next Error");
            icon = EditorGUIUtilityBridge.LoadIcon("Packages/com.unity.graphtoolkit/GraphToolkitEditor/Icons/ErrorToolbar/NextError.png");
        }

        /// <inheritdoc />
        protected override void OnClick()
        {
            var state = GraphView?.GraphViewModel.GraphViewState;
            if (state != null)
                FrameAndSelectError(state.ErrorIndex + 1);
        }
    }
}
