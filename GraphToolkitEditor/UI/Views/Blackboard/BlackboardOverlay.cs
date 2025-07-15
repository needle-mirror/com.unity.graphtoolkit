using System;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    [Overlay(typeof(GraphViewEditorWindow), idValue, "Blackboard", defaultDisplay = true,
        defaultDockZone = DockZone.LeftColumn, defaultDockPosition = DockPosition.Top,
        defaultLayout = Layout.Panel
        , defaultWidth = 300, defaultHeight = 400
     )]
    [Icon("Packages/com.unity.graphtoolkit/GraphToolkitEditor/Icons/PanelsToolbar/Blackboard.png")]
    sealed class BlackboardOverlay : OverlayWithView
    {
        public const string idValue = "gtf-blackboard";

        BlackboardView m_BlackboardView;

        public override RootView rootView => m_BlackboardView;

        public BlackboardOverlay()
        {
            minSize = new Vector2(100, 100);
            maxSize = Vector2.positiveInfinity;
        }

        /// <inheritdoc />
        public override VisualElement CreatePanelContent()
        {
            var window = containerWindow as GraphViewEditorWindow;
            if (window != null)
            {
                if (m_BlackboardView != null)
                    window.UnregisterView(m_BlackboardView);

                m_BlackboardView?.Dispose();
                m_BlackboardView = window.CreateAndSetupBlackboardView();
                if (m_BlackboardView != null)
                {
                    return m_BlackboardView;
                }
            }

            var placeholder = new VisualElement();
            placeholder.AddToClassList(BlackboardView.ussClassName);
            placeholder.AddPackageStylesheet("BlackboardView.uss");
            return placeholder;
        }

        /// <inheritdoc />
        public override void OnWillBeDestroyed()
        {
            base.OnWillBeDestroyed();

            if (m_BlackboardView != null)
            {
                var window = containerWindow as GraphViewEditorWindow;
                if (window != null)
                    window.UnregisterView(m_BlackboardView);

                m_BlackboardView.Dispose();

                m_BlackboardView = null;
            }
        }
    }
}
