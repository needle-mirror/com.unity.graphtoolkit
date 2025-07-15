using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// The cache that appear in front of a node's UI when the zoom is below a certain level.
    /// </summary>
    [UnityRestricted]
    internal class NodeLodCachePart : GraphElementPart
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NodeLodCachePart"/> class.
        /// </summary>
        /// <param name="name">The name of the part.</param>
        /// <param name="model">The model displayed in this part.</param>
        /// <param name="ownerElement">The owner of the part.</param>
        /// <param name="parentClassName">The class name of the parent.</param>
        /// <returns>A new instance of <see cref="NodeLodCachePart"/>.</returns>
        public static NodeLodCachePart Create(string name, Model model, ChildView ownerElement, string parentClassName)
        {
            if (model is IHasTitle)
            {
                return new NodeLodCachePart(name, model, ownerElement, parentClassName);
            }

            return null;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="NodeLodCachePart"/> class.
        /// </summary>
        /// <param name="name">The name of the part.</param>
        /// <param name="model">The model displayed in this part.</param>
        /// <param name="ownerElement">The owner of the part.</param>
        /// <param name="parentClassName">The class name of the parent.</param>
        protected NodeLodCachePart(string name, Model model, ChildView ownerElement, string parentClassName)
            : base(name, model, ownerElement, parentClassName) {}

        /// <summary>
        /// The label containing the title;
        /// </summary>
        protected Label m_Label;

        /// <summary>
        /// The element filled with the node color.
        /// </summary>
        protected VisualElement m_ColorLine;

        /// <summary>
        /// The root element of the cache.
        /// </summary>
        protected VisualElement m_Root;

        /// <inheritdoc />
        public override VisualElement Root => m_Root;

        /// <inheritdoc />
        protected override void BuildUI(VisualElement parent)
        {
            m_Root = new VisualElement();

            m_ColorLine = new VisualElement();
            m_ColorLine.AddToClassList(m_ParentClassName.WithUssElement("color-line"));
            m_Root.Add(m_ColorLine);

            m_Label = new Label();
            m_Label.AddToClassList(m_ParentClassName.WithUssElement("cache-label"));
            m_Root.Add(m_Label);

            parent.Add(m_Root);
            m_Root.AddToClassList(m_ParentClassName.WithUssElement("cache"));
        }

        /// <inheritdoc />
        public override void UpdateUIFromModel(UpdateFromModelVisitor visitor)
        {
            if (m_Model is IHasTitle titled && visitor.ChangeHints.HasChange(ChangeHint.Data))
            {
                m_Label.text = titled.Title;
            }

            if (m_Model is AbstractNodeModel node && visitor.ChangeHints.HasChange(ChangeHint.Style))
            {
                if (node.ElementColor.HasUserColor)
                {
                    m_ColorLine.style.backgroundColor = node.ElementColor.Color;
                }
                else
                {
                    m_ColorLine.style.backgroundColor = StyleKeyword.Null;
                }

                m_Label.tooltip = node.Tooltip;
                m_Root.tooltip = node.Tooltip;
                m_ColorLine.tooltip = node.Tooltip;
            }
        }

        /// <inheritdoc />
        public override void SetLevelOfDetail(float zoom, GraphViewZoomMode newZoomMode, GraphViewZoomMode oldZoomMode)
        {
            base.SetLevelOfDetail(zoom, newZoomMode, oldZoomMode);

            if (newZoomMode != oldZoomMode)
            {
                if (newZoomMode >= GraphViewZoomMode.Small)
                {
                    m_Root.style.visibility = StyleKeyword.Null;
                }
                else
                {
                    m_Root.style.visibility = Visibility.Hidden;
                }

                if (newZoomMode >= GraphViewZoomMode.VerySmall)
                {
                    m_Label.style.visibility = Visibility.Hidden;
                }
                else
                {
                    m_Label.style.visibility = StyleKeyword.Null;
                }
            }
        }

        /// <inheritdoc />
        public override bool SupportsCulling()
        {
            return false;
        }
    }
}
