using System;
using Unity.GraphToolsAuthoringFramework.InternalEditorBridge;
using UnityEditor;
using UnityEditor.Toolbars;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Toolbar element to display the option menu built by <see cref="GraphView.BuildOptionMenu"/>.
    /// </summary>
    [EditorToolbarElement(id, typeof(GraphViewEditorWindow))]
    [UnityRestricted]
    internal sealed class OptionDropDownMenu : EditorToolbarDropdown, IAccessContainerWindow
    {
        public const string id = "GraphToolkit/Main/Options";

        /// <inheritdoc />
        public EditorWindow containerWindow { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionDropDownMenu"/> class.
        /// </summary>
        public OptionDropDownMenu()
        {
            name = "Options";
            tooltip = L10n.Tr("Options");
            clicked += OnClick;
            icon = EditorGUIUtilityBridge.LoadIcon("_Menu");
        }

        void OnClick()
        {
            var graphViewWindow = containerWindow as GraphViewEditorWindow;

            if (graphViewWindow == null)
                return;

            GenericMenu menu = new GenericMenu();
            graphViewWindow.GraphView?.BuildOptionMenu(menu);
            menu.ShowAsContext();
        }
    }
}
