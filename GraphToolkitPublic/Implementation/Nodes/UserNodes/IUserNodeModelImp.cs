using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.GraphToolkit.Editor.Implementation
{

    interface IUserNodeModelImp : INode
    {
        public Node Node { get; }

        Dictionary<string, INodeOption> NodeOptionsByName { get; }
        IReadOnlyList<NodeOption> NodeOptions { get; }

        INodeOption GetNodeOptionByName(string name) => NodeOptionsByName.GetValueOrDefault(name);
        void CustomOnDefineNode(NodeModel.NodeDefinitionScope definitionScope)
        {
            if (Node == null)
                return;
            NodeOptionsByName.Clear();

            try
            {
                Node.CallOnDefineOptions(definitionScope);
                foreach (var nodeOption in NodeOptions)
                {
                    NodeOptionsByName[nodeOption.PortModel.UniqueName] = nodeOption;
                }

                Node.CallOnDefineNode(definitionScope);
            }
            catch (Exception e)
            {
                Debug.LogException(e, ((AbstractNodeModel)this).GraphModel?.GraphObject);
            }
        }

        void CallOnEnable()
        {
            Node?.OnEnable();
        }

        void CallOnDisable()
        {
            Node?.OnDisable();
        }
    }
}
