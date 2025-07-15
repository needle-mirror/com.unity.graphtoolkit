using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Pool;

namespace Unity.GraphToolkit.Editor.Implementation
{
    static class UserNodeHelper
    {
        public static Type GetNodeImpType(Type nodeType)
        {
            if (typeof(ContextNode).IsAssignableFrom(nodeType))
            {
                return typeof(UserContextNodeModelImp);
            }
            if (typeof(BlockNode).IsAssignableFrom(nodeType))
            {
                return typeof(UserBlockNodeModelImp);
            }
            return typeof(UserNodeModelImp);
        }
    }
}
