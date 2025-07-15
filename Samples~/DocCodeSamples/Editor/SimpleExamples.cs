#region Imports
using System;
using UnityEditor;
using UnityEngine;
using Unity.GraphToolkit.Editor;
#endregion Imports

namespace Samples.DocCodeSamples.Editor
{
    #region SimpleGraph
    [Graph(AssetExtension)]
    [Serializable]
    class MySimpleGraph : Graph
    {
        public const string AssetExtension = "simpleg";

        [MenuItem("Assets/Create/Graph Toolkit Samples/Simple Graph", false)]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<MySimpleGraph>();
        }
    }
    #endregion SimpleGraph

    class MyCustomType : ScriptableObject
    {
    }

    #region SimpleNode
    [Serializable]
    class MySimpleNode : Node {}
    #endregion SimpleNode

    #region SimpleNodeWithPorts
    [Serializable]
    class MySimpleNodeWithPorts : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<float>("Input").Build();
            context.AddOutputPort<MyCustomType>("Output").Build();
        }
    }
    #endregion SimpleNodeWithPorts
}
