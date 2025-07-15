using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Inspector for <see cref="WireModel"/>.
    /// </summary>
    [UnityRestricted]
    internal class WireFieldsInspector : SerializedFieldsInspector
    {
        /// <summary>
        /// Creates a new instance of the <see cref="WireFieldsInspector"/> class.
        /// </summary>
        /// <param name="name">The name of the part.</param>
        /// <param name="models">The models displayed in this part.</param>
        /// <param name="ownerElement">The owner of the part.</param>
        /// <param name="parentClassName">The class name of the parent.</param>
        /// <param name="filter">A filter function to select which fields are displayed in the inspector. If null, defaults to <see cref="SerializedFieldsInspector.CanBeInspected"/>.</param>
        /// <returns>A new instance of <see cref="WireFieldsInspector"/>.</returns>
        public static WireFieldsInspector Create(string name, IReadOnlyList<WireModel> models, ChildView ownerElement, string parentClassName, Func<FieldInfo, bool> filter = null)
        {
            return models.Count > 0 ? new WireFieldsInspector(name, models, ownerElement, parentClassName, filter) : null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WireFieldsInspector"/> class.
        /// </summary>
        protected WireFieldsInspector(string name, IReadOnlyList<WireModel> models, ChildView ownerElement, string parentClassName, Func<FieldInfo, bool> filter)
            : base(name, models, ownerElement, parentClassName, filter)
        {
        }
    }
}
