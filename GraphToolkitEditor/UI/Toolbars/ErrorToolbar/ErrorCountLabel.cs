using System;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Toolbar element that displays the number of errors in the graph.
    /// </summary>
    [EditorToolbarElement(id, typeof(GraphViewEditorWindow))]
    [UnityRestricted]
    internal class ErrorCountLabel : Label, IAccessContainerWindow, IToolbarElement
    {
        public const string id = "GraphToolkit/Error/Count";

        ErrorToolbarUpdateObserver m_UpdateObserver;

        /// <inheritdoc />
        public EditorWindow containerWindow { get; set; }

        /// <summary>
        /// The graph tool.
        /// </summary>
        protected GraphTool GraphTool => (containerWindow as GraphViewEditorWindow)?.GraphTool;

        /// <summary>
        /// The graph view.
        /// </summary>
        protected GraphView GraphView => (containerWindow as GraphViewEditorWindow)?.GraphView;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCountLabel"/> class.
        /// </summary>
        protected ErrorCountLabel()
        {
            RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        /// <summary>
        /// Event handler for <see cref="AttachToPanelEvent"/>.
        /// </summary>
        /// <param name="evt">The event to handle.</param>
        protected void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            if (GraphTool != null)
            {
                if (m_UpdateObserver == null)
                {
                    m_UpdateObserver = new ErrorToolbarUpdateObserver(this, GraphTool.ToolState, GraphView.GraphViewModel.ProcessingErrorsState);
                    GraphTool.ObserverManager?.RegisterObserver(m_UpdateObserver);
                }
            }

            Update();
        }

        /// <summary>
        /// Event handler for <see cref="DetachFromPanelEvent"/>.
        /// </summary>
        /// <param name="evt">The event to handle.</param>
        protected void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            GraphTool?.ObserverManager?.UnregisterObserver(m_UpdateObserver);
            m_UpdateObserver = null;
        }

        /// <inheritdoc />
        public virtual void Update()
        {
            if (GraphView is null)
                return;

            var errorCount = 0;
            for (var i = 0; i < GraphView.GraphViewModel.ProcessingErrorsState.Errors.Count; i++)
            {
                if (GraphView.GraphViewModel.ProcessingErrorsState.Errors[i].GetParentModel(GraphView.GraphModel) is not null)
                    errorCount++;
            }
            text = errorCount == 1 ? $"{errorCount} error" : $"{errorCount} errors";
        }
    }
}
