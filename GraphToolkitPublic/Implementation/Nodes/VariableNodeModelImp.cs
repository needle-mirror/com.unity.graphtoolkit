using System;
using UnityEngine;

namespace Unity.GraphToolkit.Editor.Implementation
{
    [Serializable]
    class VariableNodeModelImp : VariableNodeModel, IVariableNode
    {
        public IVariable variable => VariableDeclarationModel;
    }
}
