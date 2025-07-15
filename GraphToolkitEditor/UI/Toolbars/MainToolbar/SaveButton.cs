using Unity.GraphToolsAuthoringFramework.InternalEditorBridge;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Toolbar button to save all graphs.
    /// </summary>
    [EditorToolbarElement(id, typeof(GraphViewEditorWindow))]
    [UnityRestricted]
    internal class SaveButton : VisualElement, IAccessContainerWindow
    {
        public const string id = "GraphToolkit/Main/Save";

        public static readonly string ussClassName = "ge-save-button";
        public static readonly string buttonUssClassName = ussClassName.WithUssElement("button");

        /// <inheritdoc />
        public EditorWindow containerWindow { get; set; }

        EditorToolbarDropdownToggle m_ButtonDropdown;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveButton"/> class.
        /// </summary>
        public SaveButton()
        {
            this.AddPackageStylesheet("SaveButton.uss");
            AddToClassList(ussClassName);
            tooltip = L10n.Tr("Save");

            m_ButtonDropdown = new EditorToolbarDropdownToggle();
            m_ButtonDropdown.AddToClassList(buttonUssClassName);
            m_ButtonDropdown.icon = EditorGUIUtilityBridge.LoadIcon($"Packages/com.unity.graphtoolkit/GraphToolkitEditor/Icons/MainToolbar_Overlay/Save@4x.png");
            Add(m_ButtonDropdown);

            m_ButtonDropdown.RegisterCallback<ChangeEvent<bool>>(OnButtonClick);
            m_ButtonDropdown.dropdownClicked += OnDropDownClick;
        }

        /// <summary>
        /// Handles clicks to the button.
        /// </summary>
        protected virtual void OnButtonClick(ChangeEvent<bool> evt)
        {
            var graphEditorWindow = containerWindow as GraphViewEditorWindow;
            graphEditorWindow?.GraphView?.ClickSave();

            m_ButtonDropdown.SetValueWithoutNotify(false);
        }

        /// <summary>
        /// Handles clicks on the drop down.
        /// </summary>
        protected virtual void OnDropDownClick()
        {
            var graphEditorWindow = containerWindow as GraphViewEditorWindow;
            if (graphEditorWindow is null)
                return;

            var menu = new GenericMenu();
            graphEditorWindow.GraphView?.BuildSaveMenu(menu);
            menu.ShowAsContext();
        }
    }
}
