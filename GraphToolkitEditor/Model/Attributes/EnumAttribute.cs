using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Attribute used to emulate an enum.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [UnityRestricted]
    internal class EnumAttribute : Attribute
    {
        /// <summary>
        /// The values of the enum.
        /// </summary>
        public readonly string[] Values;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumAttribute"/> class.
        /// </summary>
        public EnumAttribute(string[] values)
        {
            Values = values;
        }
    }
}
