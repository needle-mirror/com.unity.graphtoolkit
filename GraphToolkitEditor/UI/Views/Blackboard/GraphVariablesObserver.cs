using System;
using System.Collections.Generic;
using Unity.GraphToolkit.CSO;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// An observer that updates the <see cref="BlackboardView"/> state components when new variable declarations are created in the graph.
    /// </summary>
    [UnityRestricted]
    internal class GraphVariablesObserver : StateObserver
    {
        GraphModelStateComponent m_GraphModelStateComponent;
        BlackboardViewStateComponent m_BlackboardViewStateComponent;
        SelectionStateComponent m_SelectionStateComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphVariablesObserver"/> class.
        /// </summary>
        public GraphVariablesObserver(GraphModelStateComponent graphModelStateComponent, BlackboardViewStateComponent blackboardViewStateComponent,
                                      SelectionStateComponent selectionStateComponent)
            : base(new IStateComponent[] { graphModelStateComponent },
                   new IStateComponent[] { blackboardViewStateComponent, selectionStateComponent })
        {
            m_GraphModelStateComponent = graphModelStateComponent;
            m_BlackboardViewStateComponent = blackboardViewStateComponent;
            m_SelectionStateComponent = selectionStateComponent;
        }

        /// <inheritdoc />
        public override void Observe()
        {
            using (var graphObservation = this.ObserveState(m_GraphModelStateComponent))
            {
                if (graphObservation.UpdateType == UpdateType.None)
                    return;

                var changeset = m_GraphModelStateComponent.GetAggregatedChangeset(graphObservation.LastObservedVersion);
                if (changeset?.NewModels == null)
                    return;

                var newModels = new List<Hash128>(changeset.NewModels);
                if (newModels.Count == 0)
                    return;

                var newVariableDeclarations = new List<VariableDeclarationModelBase>();
                var newGroups = new List<GroupModelBase>();
                foreach (var newModelHash in newModels)
                {
                    var model = m_GraphModelStateComponent.GraphModel.GetModel(newModelHash);
                    if (model is IGroupItemModel groupItemModel)
                    {
                        if (newGroups.Any(t => groupItemModel.IsInGroup(t)))
                            continue;
                        if (model is GroupModelBase groupModelBase)
                            newGroups.Add(groupModelBase);
                        if (model is VariableDeclarationModelBase variableDeclaration)
                            newVariableDeclarations.Add(variableDeclaration);
                    }
                }

                // need to check if a variable was not added before its parent group.
                for (int i = 0; i < newGroups.Count;)
                {
                    if (newGroups.Any(t => ((IGroupItemModel)newGroups[i]).IsInGroup(t)))
                        newGroups.RemoveAt(i);
                    else
                        ++i;
                }

                // need to check if a variable was not added before its parent group.
                for (int i = 0; i < newVariableDeclarations.Count;)
                {
                    if (newGroups.Any(t => ((IGroupItemModel)newVariableDeclarations[i]).IsInGroup(t)))
                        newVariableDeclarations.RemoveAt(i);
                    else
                        ++i;
                }

                using (var bbUpdater = m_BlackboardViewStateComponent.UpdateScope)
                {
                    var modelsToExpand = changeset.ExpandedModels;
                    foreach (var variableDeclaration in newVariableDeclarations)
                    {
                        if (modelsToExpand != null && modelsToExpand.Contains(variableDeclaration.Guid))
                        {
                            bbUpdater.SetVariableDeclarationModelExpanded(variableDeclaration, true);
                        }

                        var current = variableDeclaration.ParentGroup;
                        while (current != null)
                        {
                            bbUpdater.SetGroupModelExpanded(current, true);
                            current = current.ParentGroup;
                        }
                    }
                }

                using (var selectionUpdater = m_SelectionStateComponent.UpdateScope)
                {
                    selectionUpdater.ClearSelection();
                    selectionUpdater.SelectElements(newGroups, true);
                    selectionUpdater.SelectElements(newVariableDeclarations, true);
                }
            }
        }
    }
}
