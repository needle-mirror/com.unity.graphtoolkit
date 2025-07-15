using System;
using Unity.GraphToolkit.ItemLibrary.Editor;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// <see cref="ItemLibraryItem"/> representing a Type.
    /// </summary>
    [UnityRestricted]
    internal class TypeLibraryItem : ItemLibraryItem, IItemLibraryDataProvider
    {
        /// <summary>
        /// <see cref="TypeHandle"/> of the item.
        /// </summary>
        public TypeHandle Type => ((TypeItemLibraryData)Data).Type;

        /// <summary>
        /// Custom data for the item.
        /// </summary>
        public IItemLibraryData Data { get; }

        /// <summary>
        /// Initializes a new instance of the TypeLibraryItem class.
        /// </summary>
        /// <param name="name">The name used to search the item.</param>
        /// <param name="type">The type represented by the item.</param>
        public TypeLibraryItem(string name, TypeHandle type)
            : base(name)
        {
            Data = new TypeItemLibraryData(type);
        }
    }
}
