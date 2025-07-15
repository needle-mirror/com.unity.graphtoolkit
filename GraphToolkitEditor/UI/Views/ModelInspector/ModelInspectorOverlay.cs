using System;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    [Overlay(typeof(GraphViewEditorWindow), idValue, "Graph Inspector", defaultDisplay = true,
        defaultDockZone = DockZone.RightColumn, defaultDockPosition = DockPosition.Top,
        defaultLayout = Layout.Panel
        , defaultWidth = 300, defaultHeight = 400
     )]
    [Icon("Packages/com.unity.graphtoolkit/GraphToolkitEditor/Icons/PanelsToolbar/Inspector.png")]
    sealed class ModelInspectorOverlay : OverlayWithView
    {
        public const string idValue = "gtf-inspector";

        ModelInspectorView m_ModelInspectorView;

        public override RootView rootView => m_ModelInspectorView;

        public ModelInspectorOverlay()
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
                if (m_ModelInspectorView != null)
                {
                    window.UnregisterView(m_ModelInspectorView);
                    m_ModelInspectorView.Dispose();
                }


                m_ModelInspectorView = window.CreateAndSetupInspectorView();
                if( m_ModelInspectorView != null)
                    return m_ModelInspectorView;
            }

            var placeholder = new VisualElement();
            placeholder.AddToClassList(ModelInspectorView.ussClassName);
            placeholder.AddPackageStylesheet("ModelInspector.uss");
            return placeholder;
        }

        /// <inheritdoc />
        public override void OnWillBeDestroyed()
        {
            base.OnWillBeDestroyed();

            if (m_ModelInspectorView != null)
            {
                var window = containerWindow as GraphViewEditorWindow;
                if (window != null)
                    window.UnregisterView(m_ModelInspectorView);

                m_ModelInspectorView.Dispose();
                m_ModelInspectorView = null;
            }
        }
    }
}
