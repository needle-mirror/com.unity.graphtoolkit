using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    [UnityRestricted]
    class CategoryPart : GraphElementPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryPart"/> class.
        /// </summary>
        /// <param name="name">The name of the part.</param>
        /// <param name="model">The model displayed in this part.</param>
        /// <param name="ownerElement">The owner of the part.</param>
        /// <param name="parentClassName">The class name of the parent.</param>
        /// <param name="topCategory">Pass true if it is the top category, false for the bottom one.</param>
        public CategoryPart(string name, AbstractNodeModel model, ChildView ownerElement, string parentClassName, bool topCategory)
            : base(name, model, ownerElement, parentClassName)
        {
            m_TopCategory = topCategory;
        }

        bool m_TopCategory;
        VisualElement m_Root;

        /// <inheritdoc />
        public override VisualElement Root => m_Root;

        /// <inheritdoc />
        protected override void BuildUI(VisualElement parent)
        {
            m_Root = new VisualElement { pickingMode = PickingMode.Ignore };

            string ussClass = m_ParentClassName.WithUssElement("category");

            m_Root.AddToClassList(ussClass);
            m_Root.AddToClassList(ussClass.WithUssModifier(m_TopCategory ? "top" : "bottom"));

            parent.Add(m_Root);
        }

        /// <inheritdoc />
        public override void UpdateUIFromModel(UpdateFromModelVisitor visitor) {}

        /// <inheritdoc />
        public override bool SupportsCulling() => false;
    }
}
