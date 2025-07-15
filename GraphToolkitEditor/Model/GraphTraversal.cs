using System;
using System.Collections.Generic;

namespace Unity.GraphToolkit.Editor
{
    abstract class GraphTraversal
    {
        public void VisitGraph(GraphModel graphModel)
        {
            var visitedNodes = new HashSet<AbstractNodeModel>();
            foreach (var entryPoint in graphModel.GetEntryPoints())
            {
                VisitNode(entryPoint, visitedNodes);
            }

            // floating nodes
            foreach (var node in graphModel.NodeModels)
            {
                if (node == null || visitedNodes.Contains(node))
                    continue;

                VisitNode(node, visitedNodes);
            }

            foreach (var variableDeclaration in graphModel.VariableDeclarations)
            {
                VisitVariableDeclaration(variableDeclaration);
            }

            foreach (var wireModel in graphModel.WireModels)
            {
                VisitWire(wireModel);
            }
        }

        protected virtual void VisitWire(WireModel wireModel)
        {
        }

        protected virtual void VisitNode(AbstractNodeModel nodeModel, HashSet<AbstractNodeModel> visitedNodes)
        {
            if (nodeModel == null)
                return;

            visitedNodes.Add(nodeModel);

            if (nodeModel is InputOutputPortsNodeModel portHolder)
            {
                foreach (var inputPortModel in portHolder.InputsById.Values)
                {
                    if (inputPortModel.IsConnected())
                        foreach (var connectionPortModel in inputPortModel.GetConnectedPorts())
                        {
                            if (!visitedNodes.Contains(connectionPortModel.NodeModel))
                                VisitNode(connectionPortModel.NodeModel, visitedNodes);
                        }
                }
            }
        }

        protected virtual void VisitVariableDeclaration(VariableDeclarationModelBase variableDeclarationModel) {}
    }
}
