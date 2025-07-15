using System;
using System.Linq;

namespace Unity.GraphToolkit.Editor
{
    [UnityRestricted]
    internal class TransitionSupportInspector : ModelInspector
    {
        public new static readonly string ussClassName = "ge-transition-inspector";

        static readonly string propertiesContainerName = "properties-container";

        ModelView m_TransitionSupportEditor;

        TransitionSupportModel TransitionSupportModel => Models.First() as TransitionSupportModel;

        protected override void BuildUI()
        {
            if (Context is InspectorSectionContext inspectorSectionContext)
            {
                if (inspectorSectionContext.Section.SectionType == SectionType.Properties)
                {
                    m_TransitionSupportEditor = ModelViewFactory.CreateUI<ModelView>(RootView, TransitionSupportModel, null, this);
                    m_TransitionSupportEditor.AddToClassList(ussClassName.WithUssElement(propertiesContainerName));
                    Add(m_TransitionSupportEditor);
                }
            }
        }

        protected override void PostBuildUI()
        {
            base.PostBuildUI();
            AddToClassList(ussClassName);
        }

        public override void UpdateUIFromModel(UpdateFromModelVisitor visitor)
        {
            base.UpdateUIFromModel(visitor);

            m_TransitionSupportEditor?.UpdateUIFromModel(visitor);
        }

        public override bool IsEmpty()
        {
            if (Context is InspectorSectionContext inspectorSectionContext)
            {
                switch (inspectorSectionContext.Section.SectionType)
                {
                    case SectionType.Properties:
                        return false;
                    case SectionType.Options:
                    case SectionType.Advanced:
                        return true;
                }
            }
            return true;
        }

        public override void RemoveFromRootView()
        {
            m_TransitionSupportEditor.RemoveFromRootView();
            base.RemoveFromRootView();
        }
    }
}
