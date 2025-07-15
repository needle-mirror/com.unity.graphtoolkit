using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Unity.GraphToolkit.ItemLibrary.Editor
{
    /// <summary>
    /// Data listing some terms to search in a <see cref="ItemLibraryItem"/> and how important they are.
    /// </summary>
    [UnityRestricted]
    internal struct ItemLibraryItemTermsCategory
    {
        /// <summary>
        /// Terms that should be searched.
        /// </summary>
        public IEnumerable<string> Terms;

        /// <summary>
        /// How these terms are important compared to the rest.
        /// </summary>
        /// <remarks>0 means those terms are ignored. 1 should be the highest.</remarks>
        public float Multiplier;
    }

    /// <summary>
    /// Represents an item to display in the Item Library.
    /// </summary>
    [Serializable]
    [UnityRestricted]
    internal class ItemLibraryItem
    {
        [SerializeField] string m_CategoryPath;
        [SerializeField] string m_Name;
        [SerializeField] string m_Help;
        [SerializeField] string[] m_Synonyms;
        [SerializeField] string m_StyleName;
        [SerializeField] int m_Priority;

        /// <summary>
        /// The character used to separate categories by path.
        /// </summary>
        public static readonly char CategorySeparator = '/';

        /// <summary>
        /// Name of the item.
        /// </summary>
        /// <remarks>Used to find the item during search.</remarks>
        public virtual string Name => m_Name ?? "";

        /// <summary>
        /// Full name of the item including its path as a searchable string.
        /// i.e. path separated by spaces rather than /.
        /// </summary>
        /// <remarks>e.g. <c>"Food Fruits Berries Strawberry"</c></remarks>
        public string SearchableFullName => FullName.Replace(CategorySeparator, ' ');

        /// <summary>
        /// Name of the item including its category path.
        /// </summary>
        public string FullName => CategoryPath == "" ? Name : Name == "" ? CategoryPath : CategoryPath + CategorySeparator + Name;

        /// <summary>
        /// Gets all parents categories names for this item.
        /// </summary>
        /// <returns>The names of the parents categories, in path order.</returns>
        /// <remarks>e.g. <c>"Food/Fruits/Strawberry"</c> gives <c>["Food", "Fruits"]</c></remarks>
        public string[] GetParentCategories() => CategoryPath.Split(CategorySeparator);

        /// <summary>
        /// The category in which this item belongs, in a directory format.
        /// </summary>
        /// <remarks>e.g. <c>"Food/Fruits/Berries"</c></remarks>
        public string CategoryPath
        {
            get => m_CategoryPath ?? "";
            set => m_CategoryPath = value;
        }

        /// <summary>
        /// Help content to display about this item.
        /// </summary>
        public string Help
        {
            get => m_Help;
            set => m_Help = value;
        }

        /// <summary>
        /// Synonyms of this item.
        /// </summary>
        /// <remarks> Might be used to find the item by an alternate name.</remarks>
        public string[] Synonyms
        {
            get => m_Synonyms;
            set => m_Synonyms = value;
        }

        /// <summary>
        /// Custom name used to generate USS styles when creating UI for this item.
        /// </summary>
        public string StyleName
        {
            get => m_StyleName;
            set => m_StyleName = value;
        }

        /// <summary>
        /// Number to allow some items to come before others.
        /// </summary>
        /// <remarks>The lower, the higher the priority is.</remarks>
        public int Priority
        {
            get => m_Priority;
            set => m_Priority = value;
        }

        static (Func<ItemLibraryItem, IEnumerable<string>> getSearchData, float ratio)[] s_SearchKeysRatios =
        {
            (si => Enumerable.Repeat(si.Name, 1), 1f),
            (si => Enumerable.Repeat(si.SearchableFullName, 1), 0.5f),
            (si => si.Synonyms ?? Enumerable.Empty<string>(), 0.5f),
        };

        /// <summary>
        /// Gets the Data to apply search query on, with ratios of importance for each one.
        /// </summary>
        public virtual IEnumerable<ItemLibraryItemTermsCategory> GetSearchData()
        {
            return s_SearchKeysRatios
                .Select(tu => new ItemLibraryItemTermsCategory
                { Terms = tu.getSearchData(this), Multiplier = tu.ratio }
                );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemLibraryItem"/> class.
        /// </summary>
        public ItemLibraryItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemLibraryItem"/> class.
        /// </summary>
        /// <param name="name">The name used to search the item.</param>
        public ItemLibraryItem(string name)
        {
            m_Name = name;
        }

        /// <summary>
        /// Splits a full path into a category path and a name. Path elements should be separated by
        /// <see cref="CategorySeparator"/>. The last element of the full path is the name. All other elements constitute the category path.
        /// </summary>
        /// <param name="fullName">The full name from which to extract a category path and a name.</param>
        /// <param name="categoryPath">The extracted category path, or the empty string if no path was found.</param>
        /// <param name="name">The extracted name.</param>
        /// <returns>True if extraction was successful, false otherwise.</returns>
        public static bool ExtractPathAndNameFromFullName(string fullName, out string categoryPath, out string name)
        {
            categoryPath = "";
            name = fullName;
            var nameParts = fullName.Split(CategorySeparator);
            if (nameParts.Length > 1)
            {
                name = nameParts[nameParts.Length - 1];
                categoryPath = fullName.Substring(0, fullName.Length - name.Length - 1);
                return true;
            }
            return nameParts.Length == 1;
        }

        /// <summary>
        /// Builds data for this item.
        /// </summary>
        /// <remarks>
        /// Called during the Database Indexing.
        /// The item can be created with a lightweight representation and only gather expensive data when this is called.
        /// </remarks>
        public virtual void Build()
        {
        }

        /// <summary>
        /// String representation of this item.
        /// </summary>
        /// <returns>The representation of this item as a string.</returns>
        public override string ToString()
        {
            return $"{nameof(FullName)}: {FullName}";
        }
    }
}
