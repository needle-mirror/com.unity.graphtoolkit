using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolsAuthoringFramework.InternalEditorBridge;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Base class for overlay toolbars.
    /// </summary>
    [UnityRestricted]
    internal class Toolbar : Overlay, ICreateToolbar
    {
        /// <summary>
        /// Whether the overlay toolbar should be enabled or not.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// The graph tool.
        /// </summary>
        protected GraphTool GraphTool => (containerWindow as GraphViewEditorWindow)?.GraphTool;

        /// <inheritdoc />
        public virtual IEnumerable<string> toolbarElements => GraphTool?.GetToolbarDefinition(this)?.ElementIds ?? Enumerable.Empty<string>();

        /// <inheritdoc />
        public override VisualElement CreatePanelContent()
        {
            return OverlayBridge.CreateOverlay(toolbarElements, containerWindow);
        }

        /// <summary>
        /// Adds a stylesheet to the toolbar root visual element.
        /// </summary>
        /// <param name="stylesheet">The stylesheet to add.</param>
        /// <remarks>
        /// 'AddStylesheet' adds a stylesheet to the toolbar's root visual element, which enables custom styling of the toolbar.
        /// By adding a stylesheet, you can apply Unity Style Sheet (USS) styles to the visual elements, so the
        /// toolbar adheres to the desired design and layout specifications.
        /// </remarks>
        protected void AddStylesheet(StyleSheet stylesheet)
        {
            rootVisualElement.styleSheets.Add(stylesheet);
        }
    }
}
