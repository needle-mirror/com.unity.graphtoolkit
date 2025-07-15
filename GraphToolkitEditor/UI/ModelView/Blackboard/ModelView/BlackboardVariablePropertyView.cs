using System;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// A BlackboardElement to display the properties of a <see cref="VariableDeclarationModelBase"/>.
    /// </summary>
    [UnityRestricted]
    internal class BlackboardVariablePropertyView : BlackboardElement
    {
        public new static readonly string ussClassName = "ge-blackboard-variable-property-view";
        public static readonly string inspectorPartName = "inspectorPart";

        /// <summary>
        /// Initializes a new instance of the <see cref="BlackboardVariablePropertyView"/> class.
        /// </summary>
        public BlackboardVariablePropertyView()
        {
        }

        protected override void BuildPartList()
        {
            if (Model is VariableDeclarationModelBase variableDeclarationModel)
                PartList.AppendPart(VariableFieldsInspector.Create(
                    inspectorPartName,
                    new[] { variableDeclarationModel },
                    this,
                    ussClassName,
                    null,
                    VariableFieldsInspector.DisplayFlags.QuickSettings));
        }

        /// <inheritdoc />
        protected override void PostBuildUI()
        {
            base.PostBuildUI();

            AddToClassList(ussClassName);
        }
    }
}
