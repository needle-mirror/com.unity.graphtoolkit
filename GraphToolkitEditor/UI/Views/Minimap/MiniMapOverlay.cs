using System;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    [Overlay(typeof(GraphViewEditorWindow), idValue, "MiniMap",
        defaultDockZone = DockZone.LeftColumn, defaultDockPosition = DockPosition.Bottom,
        defaultLayout = Layout.Panel
        , defaultWidth = 200, defaultHeight = 150
     )]
    [Icon("Packages/com.unity.graphtoolkit/GraphToolkitEditor/Icons/PanelsToolbar/MiniMap.png")]
    [UnityRestricted]
    internal sealed class MiniMapOverlay : OverlayWithView
    {
        public const string idValue = "gtf-minimap";

        MiniMapView m_MiniMapView;

        public override RootView rootView => m_MiniMapView;

        public MiniMapOverlay()
        {
            minSize = new Vector2(100, 100);
            maxSize = Vector2.positiveInfinity;
        }

        /// <inheritdoc />
        public override VisualElement CreatePanelContent()
        {
            var window = containerWindow as GraphViewEditorWindow;
            if (window != null && window.GraphView != null)
            {
                if (m_MiniMapView != null)
                    window.UnregisterView(m_MiniMapView);

                m_MiniMapView?.Dispose();
                m_MiniMapView = window.CreateAndSetupMiniMapView();
                if (m_MiniMapView != null)
                {
                    return m_MiniMapView;
                }
            }

            var placeholder = new VisualElement();
            placeholder.AddToClassList(MiniMapView.ussClassName);
            placeholder.AddPackageStylesheet("MiniMapView.uss");
            return placeholder;
        }

        /// <inheritdoc />
        public override void OnWillBeDestroyed()
        {
            base.OnWillBeDestroyed();

            if (m_MiniMapView != null)
            {
                var window = containerWindow as GraphViewEditorWindow;
                if (window != null)
                    window.UnregisterView(m_MiniMapView);

                m_MiniMapView.Dispose();

                m_MiniMapView = null;
            }
        }
    }
}
