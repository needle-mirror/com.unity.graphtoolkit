using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// UI for a <see cref="VariableNodeModel"/>.
    /// </summary>
    /// <remarks>
    /// 'VariableNode' is the UI representation of a <see cref="VariableNodeModel"/>. It extends <see cref="CapsuleNodeView"/>, which
    /// provides a distinct capsule-like visual style and behavior for nodes. They both only feature a single port.
    /// </remarks>
    [UnityRestricted]
    internal class VariableNodeView : CapsuleNodeView
    {
        /// <summary>
        /// The USS class name added to the scope image.
        /// </summary>
        public static readonly string scopeImageUssClassName = ussClassName.WithUssElement("scope-image");

        VariableScopeImage m_ScopeImage;

        /// <inheritdoc />
        public override void ActivateRename()
        {
            // The only moment that a variable node is renamable is when creating it from a port
            // AND it is only renamable from its declaration on the BB, not on the node itself
            if (NodeModel is VariableNodeModel variableNode && GraphView.Window is GraphViewEditorWindow window)
            {
                var variableDeclarationModel = variableNode.VariableDeclarationModel;
                if (variableDeclarationModel != null)
                {
                    var variableDeclarationField = variableDeclarationModel.GetView<BlackboardField>(window.BlackboardView, BlackboardCreationContext.VariableCreationContext);
                    variableDeclarationField?.ActivateRename();
                }
            }
        }

        /// <inheritdoc />
        public override void BuildUITree()
        {
            base.BuildUITree();

            m_ScopeImage = new VariableScopeImage();
            m_ScopeImage.AddToClassList(scopeImageUssClassName);
            Insert(childCount - 2, m_ScopeImage); // Insert just before the node cache.
        }

        /// <inheritdoc />
        public override void UpdateUIFromModel(UpdateFromModelVisitor visitor)
        {
            base.UpdateUIFromModel(visitor);

            SinglePortContainerPart portContainer;

            if (NodeModel is VariableNodeModel variableNodeModel)
            {
                var variableDeclarationModel = variableNodeModel.VariableDeclarationModel;
                if (variableDeclarationModel == null)
                    return;

                if (variableNodeModel.OutputPort != null)
                    portContainer = PartList.GetPart(outputPortContainerPartName) as SinglePortContainerPart;
                else
                    portContainer = PartList.GetPart(inputPortContainerPartName) as SinglePortContainerPart;

                m_ScopeImage.Scope = variableDeclarationModel.Scope;
                m_ScopeImage.ReadWriteModifiers = variableDeclarationModel.Modifiers;

                // We have to wait for the style to be resolved to get the right Port Color.
                schedule.Execute(_ =>
                    m_ScopeImage.Color = portContainer?.Port?.PortColor ?? Port.DefaultPortColor).ExecuteLater(0);
            }
        }
    }
}
