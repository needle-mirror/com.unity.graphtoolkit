using System;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Base implementation for constants.
    /// </summary>
    /// <typeparam name="T">The type of the value of the constant.</typeparam>
    [Serializable]
    [UnityRestricted]
    internal class Constant<T> : Constant
    {
        [SerializeField]
        protected T m_Value;

        /// <summary>
        /// The constant value.
        /// </summary>
        public virtual T Value
        {
            get => m_Value;
            set
            {
                m_Value = value;
                OwnerModel?.GraphModel?.CurrentGraphChangeDescription.AddChangedModel(OwnerModel, ChangeHint.Data);
                // If OwnerModel is a PortModel, the graph object will not be marked as dirty (since PortModels are not serialized).
                // Make sure the asset is marked as dirty so the changes to the Constant are saved.
                OwnerModel?.GraphModel?.SetGraphObjectDirty();
            }
        }

        /// <inheritdoc />
        public override object ObjectValue
        {
            get => Value;
            set => Value = FromObject(value);
        }

        /// <inheritdoc />
        public override object DefaultValue
        {
            get
            {
                if (Type == typeof(string))
                    return "";
                if (Type == typeof(Quaternion))
                    return Quaternion.identity;
                return default(T);
            }
        }

        /// <inheritdoc />
        public override Type Type => typeof(T);

        /// <summary>
        /// Converts an object to a value of the type {T}.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <returns>The object cast to type {T}.</returns>
        protected virtual T FromObject(object value) => (T)value;

        /// <inheritdoc />
        public override Constant Clone()
        {
            var copy = (Constant<T>)Activator.CreateInstance(GetType());
            copy.m_Value = m_Value;
            return copy;
        }
    }
}
