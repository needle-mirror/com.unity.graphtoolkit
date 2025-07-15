using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// <see cref="VariableLibraryItem"/> representing a Variable to be created.
    /// </summary>
    [UnityRestricted]
    internal class VariableLibraryItem : TypeLibraryItem
    {
        /// <summary>
        /// The type of the created variable that must derived from VariableDeclarationModel.
        /// </summary>
        public Type VariableType { get; set; }

        /// <summary>
        /// The scope of the created variable.
        /// </summary>
        public VariableScope Scope { get; set; }

        /// <summary>
        /// The modifier flags of the created variable.
        /// </summary>
        public ModifierFlags ModifierFlags { get; set; }

        /// <summary>
        /// Initializes a new instance of the VariableLibraryItem class.
        /// </summary>
        /// <param name="name">The name used to search the item.</param>
        /// <param name="type">The type represented by the item.</param>
        /// <param name="variableType">The type that must derived from VariableDeclarationModel.</param>
        public VariableLibraryItem(string name, TypeHandle type, Type variableType = null)
            : base(name, type)
        {
            VariableType = variableType;
        }
    }
}
