using System.Collections.Generic;
using Unity.GraphToolkit.CSO;

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
        public override void Dispatch(ICommand command, Diagnostics diagnosticsFlags = Diagnostics.None)
        {
           base.Dispatch(command, diagnosticsFlags);

           if (command is LoadGraphCommand) // Update the tool name when a graph is loaded. For now the tool name is the graph type name.
           {
               Name = (ToolState.GraphModel as GraphModelImp)?.Graph?.GetType()?.Name ?? "Unknown Tool";
           }
        }
    }
}
