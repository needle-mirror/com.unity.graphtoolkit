using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.GraphToolkit
{
    /// <summary>
    /// Extension methods for <see cref="Type"/>.
    /// </summary>
    [UnityRestricted]
    internal static class TypeHelpers
    {
        static readonly Dictionary<Type, string> k_TypeToFriendlyName = new Dictionary<Type, string>
        {
            { typeof(string),  "String" },
            { typeof(object),  "System.Object" },
            { typeof(bool),    "Boolean" },
            { typeof(byte),    "Byte" },
            { typeof(char),    "Char" },
            { typeof(decimal), "Decimal" },
            { typeof(double),  "Double" },
            { typeof(short),   "Short" },
            { typeof(int),     "Integer" },
            { typeof(long),    "Long" },
            { typeof(sbyte),   "SByte" },
            { typeof(float),   "Float" },
            { typeof(ushort),  "Unsigned Short" },
            { typeof(uint),    "Unsigned Integer" },
            { typeof(ulong),   "Unsigned Long" },
            { typeof(void),    "Void" },
            { typeof(Color),   "Color"},
            { typeof(Object), "UnityEngine.Object"},
            { typeof(Vector2), "Vector 2"},
            { typeof(Vector3), "Vector 3"},
            { typeof(Vector4), "Vector 4"}
        };

        /// <summary>
        /// Returns a human-readable name for a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type for which to get the name.</param>
        /// <param name="expandGeneric">Set to true if generic parameters should be included.</param>
        /// <returns>The human-readable name for the <see cref="Type"/></returns>
        public static string GetFriendlyName(Type type, bool expandGeneric = true)
        {
            if (k_TypeToFriendlyName.TryGetValue(type, out var friendlyName))
            {
                return friendlyName;
            }

            friendlyName = type.Name;

            if (type.IsGenericType && expandGeneric)
            {
                int backtick = friendlyName.IndexOf('`');
                if (backtick > 0)
                {
                    friendlyName = friendlyName.Remove(backtick);
                }
                friendlyName += " of ";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; i++)
                {
                    string typeParamName = GetFriendlyName(typeParameters[i]);
                    friendlyName += (i == 0 ? typeParamName : " and " + typeParamName);
                }
            }

            if (type.IsArray)
            {
                return GetFriendlyName(type.GetElementType()) + "[]";
            }

            return friendlyName;
        }

        /// <inheritdoc cref="TypeHandleHelpers.GenerateTypeHandle(Type)"/>
        public static TypeHandle GenerateTypeHandle(this Type t)
        {
            return TypeHandleHelpers.GenerateTypeHandle(t);
        }
    }
}
