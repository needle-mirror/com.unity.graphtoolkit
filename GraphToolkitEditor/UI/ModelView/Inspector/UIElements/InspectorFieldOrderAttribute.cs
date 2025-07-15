using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Field attribute to control the order of fields in the inspector.
    /// Fields without InspectorFieldOrderAttribute will always be displayed first.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    [UnityRestricted]
    internal class InspectorFieldOrderAttribute : PropertyAttribute
    {
        public int Order { get; }

        /// <summary>
        /// Create an instance of the <see cref="InspectorFieldOrderAttribute"/> class.
        /// </summary>
        /// <param name="order">The order in the inspector.</param>
        public InspectorFieldOrderAttribute(int order = 0)
        {
            Assert.IsTrue(order >= 0, "The order can't be a negative number");
            Order = order;
        }
    }
}
