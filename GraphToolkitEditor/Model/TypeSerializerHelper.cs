using System;
using System.Collections.Generic;
using Unity.GraphToolkit.InternalBridge;
using UnityEditor;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.GraphToolkit.Editor
{
    [InitializeOnLoad]
    static class TypeSerializerHelper
    {
        static TypeSerializerHelper()
        {
            InternalTypeHelpers.GetMovedFromType = GetMovedFromType;
        }

        static Dictionary<string, Type> s_MovedFromTypes;

        static Dictionary<string, Type> GetMovedFromTypes()
        {
            if (s_MovedFromTypes == null)
            {
                s_MovedFromTypes = new Dictionary<string, Type>();
                var movedFromTypes = TypeCache.GetTypesWithAttribute<MovedFromAttribute>();
                foreach (var t in movedFromTypes)
                {
                    var attributes = Attribute.GetCustomAttributes(t, typeof(MovedFromAttribute), false);
                    foreach (var attribute in attributes)
                    {
                        var movedFromAttribute = (MovedFromAttribute)attribute;

                        movedFromAttribute.GetMovedFromData(out var className, out _,
                            out var nameSpace, out var nameSpaceHasChanged,
                            out var assembly, out _, out _);

                        var currentClassName = GetFullNameNoNamespace(t.FullName, t.Namespace);
                        var currentNamespace = t.Namespace;
                        var currentAssembly = t.Assembly.GetName().Name;

                        var oldNamespace = nameSpaceHasChanged ? nameSpace : currentNamespace;
                        var oldClassName = string.IsNullOrEmpty(className) ? currentClassName : className;
                        var oldAssembly = string.IsNullOrEmpty(assembly) ? currentAssembly : assembly;

                        var oldAssemblyQualifiedName =
                            string.IsNullOrEmpty(oldNamespace) ? $"{oldClassName}, {oldAssembly}" : $"{oldNamespace}.{oldClassName}, {oldAssembly}";

                        s_MovedFromTypes.Add(oldAssemblyQualifiedName, t);
                    }
                }
            }

            return s_MovedFromTypes;
        }

        static Type GetMovedFromType(string typeName)
        {
            int firstComma = typeName.IndexOf(',');
            if (firstComma < 0)
                return null;
            int secondComma = typeName.IndexOf(',', firstComma + 1);

            string typeNameWithoutVersion = secondComma < 0 ? typeName : typeName.Substring(0, secondComma);

            return GetMovedFromTypes().GetValueOrDefault(typeNameWithoutVersion);
        }

        /// <summary>
        /// Gets the full name of a type without the namespace.
        /// </summary>
        /// <remarks>
        /// The full name of a type nested type includes the outer class type name. The type names are normally
        /// separated by '+' but Unity serialization uses the '/' character as separator.
        ///
        /// This method returns the full type name of a class and switches the type separator to '/' to follow Unity.
        /// </remarks>
        /// <param name="typeName">The full type name, including the namespace.</param>
        /// <param name="nameSpace">The namespace to be removed.</param>
        /// <returns>Returns a string.</returns>
        static string GetFullNameNoNamespace(string typeName, string nameSpace)
        {
            if (typeName != null && nameSpace != null && typeName.Contains(nameSpace))
            {
                return typeName.Substring(nameSpace.Length + 1).Replace("+", "/");
            }
            return typeName;
        }
    }
}
