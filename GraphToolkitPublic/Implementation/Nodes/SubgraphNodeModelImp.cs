using System;
using UnityEngine;

namespace Unity.GraphToolkit.Editor.Implementation
{
    [Serializable]
    class SubgraphNodeModelImp : SubgraphNodeModel, ISubgraphNode
    {
        public Graph GetSubgraph()
        {
            var graphModel = GetSubgraphModel();

            return (graphModel as GraphModelImp)?.Graph;
        }
    }
}
