using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Helper methods to clone graph elements.
    /// </summary>
    [UnityRestricted]
    internal static class CloneHelpers
    {
        class Holder : ScriptableObject
        {
            [SerializeReference]
            public Model m_Model;
        }

        /// <summary>
        /// Clones a graph element model.
        /// </summary>
        /// <param name="element">The element to clone.</param>
        /// <typeparam name="T">The type of the element to clone.</typeparam>
        /// <returns>A clone of the element.</returns>
        public static T Clone<T>(this T element) where T : Model
        {
            if (element is ICloneable cloneable)
            {
                T copy = (T)cloneable.Clone();
                copy.OnAfterClone();
                return copy;
            }

            return CloneUsingScriptableObjectInstantiate(element);
        }

        /// <summary>
        /// Clones a graph element model.
        /// </summary>
        /// <param name="element">The element to clone.</param>
        /// <typeparam name="T">The type of the element to clone.</typeparam>
        /// <returns>A clone of the element.</returns>
        public static T CloneUsingScriptableObjectInstantiate<T>(T element) where T : Model
        {
            var clone = (T)Activator.CreateInstance(element.GetType());
            EditorUtility.CopySerializedManagedFieldsOnly(element, clone);

            clone.OnAfterClone();

            if (clone is IGraphElementContainer container)
                foreach (var subElement in container.GetGraphElementModels())
                {
                    if (subElement is ICloneable)
                        Debug.LogError("ICloneable is not supported on elements in IGraphElementsContainer");
                }

            return clone;
        }
    }
}
