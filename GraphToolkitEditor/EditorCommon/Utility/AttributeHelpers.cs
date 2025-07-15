using System;

namespace Unity.GraphToolkit.Editor
{
    static class AttributeHelpers
    {
        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            var attrs = type.GetCustomAttributes(typeof(T), false);
            if (attrs.Length == 0)
                return null;
            return attrs[0] as T;
        }
    }
}
