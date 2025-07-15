using System;
using System.Collections.Generic;

namespace Unity.GraphToolkit.Editor
{
    [UnityRestricted]
    internal static class BlackboardCommandsExtensions
    {
        public static void DispatchSelectUnusedVariables(this BlackboardView self)
        {
            var selectables = new List<GraphElementModel>();

            foreach (var variable in self.BlackboardRootViewModel.GraphModelState.GraphModel.VariableDeclarations)
            {
                if (!variable.IsUsed())
                    selectables.Add(variable);
            }

            self.Dispatch(new SelectElementsCommand(SelectElementsCommand.SelectionMode.Replace, selectables));
        }
    }
}
