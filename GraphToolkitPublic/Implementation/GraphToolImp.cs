using System.Collections.Generic;

namespace Unity.GraphToolkit.Editor.Implementation
{
    class GraphToolImp : GraphTool
    {
        class MainToolbarDefinition : ToolbarDefinition
        {
            /// <inheritdoc />
            public override IEnumerable<string> ElementIds => new[] {SaveButton.id, ShowInProjectWindowButton.id };
        }

        protected override ToolbarDefinition CreateToolbarDefinition(string toolbarId)
        {
            switch (toolbarId)
            {
                case MainToolbar.toolbarId:
                    return new MainToolbarDefinition();
                default:
                    return base.CreateToolbarDefinition(toolbarId);
            }
        }
    }
}
