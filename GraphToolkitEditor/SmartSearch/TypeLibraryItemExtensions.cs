using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.GraphToolkit.ItemLibrary.Editor;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Extension methods for <see cref="TypeLibraryItem"/>.
    /// </summary>
    [PublicAPI]
    [UnityRestricted]
    internal static class TypeLibraryItemExtensions
    {
        const string k_Class = "Classes";

        /// <summary>
        /// Creates a <see cref="ItemLibraryDatabase"/> for types.
        /// </summary>
        /// <param name="types">Types to create the <see cref="ItemLibraryDatabase"/> from.</param>
        /// <returns>A <see cref="ItemLibraryDatabase"/> containing the types passed in parameter.</returns>
        public static ItemLibraryDatabaseBase ToDatabase(this IEnumerable<Type> types)
        {
            return ToDatabase(types, t => t.GenerateTypeHandle());
        }

        /// <summary>
        /// Creates a <see cref="ItemLibraryDatabase"/> for types.
        /// </summary>
        /// <param name="types">Types to create the <see cref="ItemLibraryDatabase"/> from.</param>
        /// <returns>A <see cref="ItemLibraryDatabase"/> containing the types passed in parameter.</returns>
        public static ItemLibraryDatabaseBase ToDatabase(this IEnumerable<TypeHandle> types)
        {
            return ToDatabase(types, t => t);
        }

        static ItemLibraryDatabaseBase ToDatabase<T>(this IEnumerable<T> types, Func<T, TypeHandle> func)
        {
            var items = new List<ItemLibraryItem>();
            foreach (var item in types)
            {
                var typeHandle = func(item);
                var type = typeHandle.Resolve();
                if (type.IsClass || type.IsValueType)
                {
                    var classItem = new TypeLibraryItem(TypeHelpers.GetFriendlyName(type), typeHandle);
                    items.Add(classItem);
                }
            }
            return new ItemLibraryDatabase(items);
        }
    }
}
