using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Extension methods to build value editors for constants.
    /// </summary>
    [GraphElementsExtensionMethodsCache(typeof(RootView), GraphElementsExtensionMethodsCacheAttribute.lowestPriority)]
    [UnityRestricted]
    internal static class ConstantEditorExtensions
    {
        /// <summary>
        /// Factory method to create default constants editors.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="constants">The constants for which to build an editor.</param>
        /// <returns>The editor.</returns>
        public static BaseModelPropertyField BuildDefaultConstantEditor(this ConstantEditorBuilder builder, IReadOnlyList<Constant> constants)
        {
            return new ConstantField(constants, builder.ConstantOwners, builder.CommandTarget, builder.Label);
        }
    }
}
