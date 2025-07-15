using System;
using UnityEngine;

namespace Unity.GraphToolkit.Editor.Implementation
{
    [Serializable]
    class ConstantNodeModelImp : ConstantNodeModel, IConstantNode
    {
        public Type dataType => Value.Type;

        public bool TryGetValue<T>(out T value)
        {
            if (Value == null)
            {
                value = default;
                return false;
            }
            return Value.TryGetValue(out value);
        }
    }
}
