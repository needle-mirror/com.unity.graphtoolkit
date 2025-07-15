using System;
using Unity.GraphToolkit.InternalBridge;
using UnityEngine;
using UnityEngine.UIElements;
using TextElement = UnityEngine.UIElements.TextElement;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// A part to build the UI for the title of an <see cref="IHasTitle"/> model using an <see cref="EditableLabel"/> to allow editing.
    /// </summary>
    [UnityRestricted]
    internal class EditableTitlePart : GraphElementPart
    {
        float m_TitleMinWidthPadding = 8f;

        /// <summary>
        /// The USS class name added to a <see cref="EditableTitlePart"/>.
        /// </summary>
        public static readonly string ussClassName = "ge-editable-title-part";

        /// <summary>
        /// The USS modifier for a title that can be renamed.
        /// </summary>
        public static readonly string renamableUSSModifier = "renamable";

        /// <summary>
        /// The name of a title label field.
        /// </summary>
        public static readonly string titleLabelName = GraphElementHelper.titleName;


        protected static readonly CustomStyleProperty<float> k_LodMinTextSize = new CustomStyleProperty<float>("--lod-min-text-size");
        protected static readonly CustomStyleProperty<float> k_WantedTextSize = new CustomStyleProperty<float>("--wanted-text-size");
        static readonly CustomStyleProperty<float> k_TitleMinWidthPaddingProperty = new CustomStyleProperty<float>("--title-min-width-padding");

        /// <summary>
        /// Options for configuring the title.
        /// </summary>
        /// <remarks>
        /// The 'Options' class defines configuration options for customizing the title’s appearance and behavior.
        /// These options control various aspects of how the title is displayed and interacted with, which provides flexibility for different use cases.
        /// The options are: <see cref="Options.None"/>, <see cref="Options.UseEllipsis"/>, <see cref="Options.SetWidth"/>, <see cref="Options.Multiline"/>,
        /// <see cref="Options.ClickToEditDisabled"/>, <see cref="Options.AdjustMinWidth"/>, and <see cref="Options.Default"/>.
        /// </remarks>
        [UnityRestricted]
        internal class Options
        {
            /// <summary>
            /// No configuration options are applied to the title.
            /// </summary>
            public const int None = 0;
            /// <summary>
            /// Whether to use ellipsis when the text is too large.
            /// </summary>
            public const int UseEllipsis = 1 << 0;

            /// <summary>
            /// Whether to set the width of the element to its text width at 100%.
            /// </summary>
            public const int SetWidth = 1 << 1;

            /// <summary>
            /// Whether the text should be displayed on multiple lines.
            /// </summary>
            public const int Multiline = 1 << 2;

            /// <summary>
            /// When the title is renamable, whether it isn't allowed to double click to edit the text.
            /// </summary>
            public const int ClickToEditDisabled = 1 << 3;

            /// <summary>
            /// Whether the title's minimum width should be adjusted depending on the zoom level.
            /// </summary>
            public const int AdjustMinWidth = 1 << 4;

            protected const int optionCount = 5;

            /// <summary>
            /// The default configuration of the title.
            /// </summary>
            public const int Default = SetWidth | UseEllipsis;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EditableTitlePart"/> class.
        /// </summary>
        /// <param name="name">The name of the part.</param>
        /// <param name="model">The model displayed in this part.</param>
        /// <param name="ownerElement">The owner of the part.</param>
        /// <param name="parentClassName">The class name of the parent.</param>
        /// <param name="options">The options for this <see cref="EditableTitlePart"/>. See <see cref="Options"/>.</param>
        /// <returns>A new instance of <see cref="EditableTitlePart"/>.</returns>
        public static EditableTitlePart Create(string name, Model model, ChildView ownerElement, string parentClassName, int options = Options.Default)
        {
            if (model is IHasTitle)
            {
                return new EditableTitlePart(name, model, ownerElement, parentClassName, options);
            }

            return null;
        }

        static float GetTextWidthWithFontSize(TextElement element, float fontSize)
        {
            return element.GetTextWidthWithFontSize(fontSize);
        }

        string m_PreviousTitle;

        /// <summary>
        /// The options, see <see cref="Options"/>.
        /// </summary>
        protected int m_Options;

        /// <summary>
        /// The current zoom level of the graph.
        /// </summary>
        protected float m_CurrentZoom;

        /// <summary>
        /// The root element of the part.
        /// </summary>
        protected VisualElement TitleContainer { get; set; }

        /// <summary>
        /// If any, the element inside the Title container that contains the label.
        /// </summary>
        protected VisualElement LabelContainer { get; private set; }

        /// <summary>
        /// The title visual element that can be either a <see cref="Label"/> or an <see cref="EditableLabel"/>.
        /// </summary>
        public VisualElement TitleLabel { get; protected set; }

        /// <summary>
        /// The minimum readable size of the text, lod will try to make the text at least this size.
        /// </summary>
        public float LodMinTextSize { get; protected internal set; } = 12;

        /// <summary>
        /// The wanted text size at 100% zoom.
        /// </summary>
        public float WantedTextSize { get; protected set; }

        /// <inheritdoc />
        public override VisualElement Root => TitleContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableTitlePart"/> class.
        /// </summary>
        /// <param name="name">The name of the part.</param>
        /// <param name="model">The model displayed in this part.</param>
        /// <param name="ownerElement">The owner of the part.</param>
        /// <param name="parentClassName">The class name of the parent.</param>
        /// <param name="options">The <see cref="Options"/> for the title.</param>
        protected EditableTitlePart(string name, Model model, ChildView ownerElement, string parentClassName, int options)
            : base(name, model, ownerElement, parentClassName)
        {
            m_Options = options;
        }

        protected virtual bool HasEditableLabel => (m_Model as GraphElementModel).IsRenamable();

        /// <inheritdoc />
        protected override void BuildUI(VisualElement container)
        {
            if (m_Model is IHasTitle)
            {
                TitleContainer = new VisualElement { name = PartName };
                TitleContainer.AddToClassList(ussClassName);
                TitleContainer.AddToClassList(m_ParentClassName.WithUssElement(PartName));

                CreateTitleLabel();

                container.Add(TitleContainer);

                TitleContainer.RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
            }
        }

        /// <summary>
        /// Creates the title label element.
        /// </summary>
        protected virtual void CreateTitleLabel()
        {
            if (HasEditableLabel)
            {
                TitleLabel = new EditableLabel
                {
                    name = titleLabelName,
                    EditActionName = "Rename",
                    multiline = (m_Options & Options.Multiline) != 0,
                    ClickToEditDisabled = (m_Options & Options.ClickToEditDisabled) != 0
                };
                TitleLabel.RegisterCallback<ChangeEvent<string>>(OnRename);
            }
            else
            {
                TitleLabel = new Label { name = titleLabelName };
            }

            if ((m_Options & Options.UseEllipsis) != 0)
            {
                LabelContainer = new VisualElement();
                LabelContainer.AddToClassList(ussClassName.WithUssElement("label-container"));
                LabelContainer.Add(TitleLabel);
                TitleContainer.Add(LabelContainer);
            }
            else
            {
                TitleContainer.Add(TitleLabel);
            }

            TitleLabel.AddToClassList(ussClassName.WithUssElement(titleLabelName));
            TitleLabel.AddToClassList(m_ParentClassName.WithUssElement(titleLabelName));
        }

        /// <inheritdoc />
        public override void UpdateUIFromModel(UpdateFromModelVisitor visitor)
        {
            if (TitleLabel == null)
                return;

            bool labelTypeChanged = false;
            if ((TitleLabel is EditableLabel && !HasEditableLabel) ||
                (TitleLabel is Label && HasEditableLabel))
            {
                TitleContainer.Remove((m_Options & Options.UseEllipsis) != 0 ? TitleLabel.parent : TitleLabel);
                CreateTitleLabel();
                labelTypeChanged = true;
            }

            if (labelTypeChanged || visitor.ChangeHints.HasChange(ChangeHint.Data))
            {
                var value = (m_Model as IHasTitle)?.Title ?? string.Empty;

                if (value == m_PreviousTitle && !labelTypeChanged)
                    return;

                m_PreviousTitle = value;
                if (TitleLabel is EditableLabel editableLabel)
                    editableLabel.SetValueWithoutNotify(value);
                else if (TitleLabel is Label label)
                    label.text = value;
                SetupWidthFromOriginalSize();

                if (labelTypeChanged)
                    SetupLod();
            }
        }

        /// <inheritdoc />
        protected override void PostBuildUI()
        {
            base.PostBuildUI();
            TitleContainer.AddPackageStylesheet("EditableTitlePart.uss");
        }

        /// <summary>
        /// Manage the custom styles used for this part.
        /// </summary>
        /// <param name="e">The event.</param>
        protected virtual void OnCustomStyleResolved(CustomStyleResolvedEvent e)
        {
            bool changed = false;
            if (e.customStyle.TryGetValue(k_LodMinTextSize, out var value) && value != LodMinTextSize)
            {
                LodMinTextSize = value;
                changed = true;
            }

            if (e.customStyle.TryGetValue(k_WantedTextSize, out value) && value != WantedTextSize)
            {
                WantedTextSize = value;
                changed = true;
            }

            if (e.customStyle.TryGetValue(k_TitleMinWidthPaddingProperty, out value))
            {
                m_TitleMinWidthPadding = value;
                changed = true;
            }

            if (changed)
            {
                SetupWidthFromOriginalSize();
                SetupLod();
            }
        }

        protected void OnRename(ChangeEvent<string> e)
        {
            // Restore the value in the model in case we don't have notification (in case the change of name doesn't change the model).
            if (TitleLabel is EditableLabel editableLabel)
                editableLabel.SetValueWithoutNotify(e.previousValue, true);

            m_OwnerElement.RootView.Dispatch(new RenameElementsCommand(m_Model as IRenamable, e.newValue));
        }

        /// <summary>
        /// Place the focus on the TextField, if any.
        /// </summary>
        public void BeginEditing()
        {
            // Disable culling entirely when renaming
            if (m_OwnerElement is GraphElement ge && ge.IsCulled())
                ge.ClearCulling();

            if (TitleLabel is EditableLabel editableLabel)
                editableLabel.BeginEditing();
        }

        /// <inheritdoc />
        public override void SetLevelOfDetail(float zoom, GraphViewZoomMode newZoomMode, GraphViewZoomMode oldZoomMode)
        {
            m_CurrentZoom = zoom;

            if (TitleLabel != null)
            {
                if (float.IsFinite(TitleLabel.layout.width))
                    SetupLod();
                else
                    Root.schedule.Execute(SetupLod).ExecuteLater(0);
            }
        }

        void SetupLod()
        {
            if (WantedTextSize != 0 && m_CurrentZoom != 0)
            {
                TextElement te = null;
                if (TitleLabel is EditableLabel editableLabel)
                    te = editableLabel.MandatoryQ<Label>();
                else if (TitleLabel is Label label)
                    te = label;

                if (!string.IsNullOrEmpty(te?.text))
                {
                    float inverseZoom = 1 / m_CurrentZoom;

                    if (inverseZoom * LodMinTextSize > WantedTextSize)
                    {
                        TitleLabel.style.fontSize = LodMinTextSize * inverseZoom;
                        if ((m_Options & Options.AdjustMinWidth) != 0)
                            TitleLabel.parent.style.minWidth = new StyleLength(Mathf.Ceil(GetTextWidthWithFontSize(te, LodMinTextSize)) * inverseZoom);
                    }
                    else
                    {
                        if (TitleLabel.style.fontSize.value != WantedTextSize)
                        {
                            TitleLabel.style.fontSize = WantedTextSize;
                        }
                    }
                }
                else
                {
                    TitleLabel.style.fontSize = WantedTextSize;
                }
            }
        }

        /// <inheritdoc />
        public override bool SupportsCulling()
        {
            return TitleLabel is not EditableLabel { IsInEditMode : true };
        }

        /// <summary>
        /// Sets the title minimum width.
        /// </summary>
        protected void SetupWidthFromOriginalSize()
        {
            TextElement te = null;
            if (TitleLabel is EditableLabel editableLabel)
                te = editableLabel.MandatoryQ<Label>();
            else if (TitleLabel is Label label)
                te = label;

            if ((m_Options & Options.SetWidth) == 0)
            {
                TitleLabel.parent.style.flexGrow = 1;
                return;
            }

            if (te is null)
            {
                TitleLabel.parent.style.minWidth = StyleKeyword.Null;
            }
            else
            {
                if (WantedTextSize == 0)
                {
                    // postpone the execution because the first UpdateFromModel is called way to early for the te to be setup and WantedTextSize to be defined.
                    te.schedule.Execute(
                        () =>
                        {
                            SetTitleMinWidth(te);
                        }).ExecuteLater(0);
                }
                else
                {
                    SetTitleMinWidth(te);
                }
            }
        }

        void SetTitleMinWidth(TextElement te)
        {
            TitleLabel.parent.style.minWidth = !string.IsNullOrEmpty(te.text)
                ? new StyleLength(m_TitleMinWidthPadding + Mathf.Ceil(GetTextWidthWithFontSize(te, WantedTextSize)))
                : StyleKeyword.Null;
        }
    }
}
