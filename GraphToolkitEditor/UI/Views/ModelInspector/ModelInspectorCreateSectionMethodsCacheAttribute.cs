using System;
using JetBrains.Annotations;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// An attribute that marks classes containing extension methods for creating sections in the model inspector.
    /// </summary>
    /// <remarks>
    /// This attribute is used to mark classes that define extension methods for `<see cref="ElementBuilder"/>`.
    /// For example, see `<see cref="ModelInspectorCreateSectionFactoryExtensions"/>`.
    /// This enables section creation within the inspector, providing flexibility in modifying the UI based on different model types.
    /// When building the inspector UI, the system retrieves all types marked with this attribute and extracts their methods.
    /// The retrieved methods must have exactly two parameters:
    /// - The first parameter must be `<see cref="ElementBuilder"/>`, which is used to construct the section.
    /// - The second parameter must represent the models for which the section is built.
    /// </remarks>
    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    [UnityRestricted]
    internal class ModelInspectorCreateSectionMethodsCacheAttribute : Attribute
    {
        internal const int lowestPriority = 0;

        /// <summary>
        /// Default extension method priority for methods provided by tools.
        /// </summary>
        public const int toolDefaultPriority = 1000;

        /// <summary>
        /// The priority of the extension methods.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// The type of view to which the extension methods apply.
        /// </summary>
        public Type ViewDomain { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelInspectorCreateSectionMethodsCacheAttribute"/> class.
        /// </summary>
        /// <param name="viewDomain">The view domain to use</param>
        /// <param name="priority">The priority of the extension methods.</param>
        public ModelInspectorCreateSectionMethodsCacheAttribute(Type viewDomain, int priority = toolDefaultPriority)
        {
            ViewDomain = viewDomain;
            Priority = priority;
        }
    }
}
