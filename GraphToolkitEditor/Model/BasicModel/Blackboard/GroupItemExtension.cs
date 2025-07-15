using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Extension methods for <see cref="IGroupItemModel"/>.
    /// </summary>
    [UnityRestricted]
    internal static class GroupItemExtension
    {
        /// <summary>
        /// Returns the section for a given item.
        /// </summary>
        /// <param name="itemModel">The item.</param>
        /// <returns>The section for a given item.</returns>
        public static SectionModel GetSection(this IGroupItemModel itemModel)
        {
            if (itemModel is SectionModel section)
                return section;
            IGroupItemModel current = itemModel;
            while (current.ParentGroup != null)
                current = current.ParentGroup;
            return current as SectionModel;
        }

        /// <summary>
        /// Returns whether a given item is contained in another given item.
        /// </summary>
        /// <param name="itemModel">The item that might be contained.</param>
        /// <param name="graphElementModel">The container item.</param>
        /// <returns>Whether a given item is contained in another given item.</returns>
        public static bool IsIn(this IGroupItemModel itemModel, IGroupItemModel graphElementModel)
        {
            if (ReferenceEquals(graphElementModel, itemModel)) return true;

            GroupModelBase group = itemModel.ParentGroup;
            while (group != null)
            {
                if (ReferenceEquals(group, graphElementModel))
                    return true;
                group = group.ParentGroup;
            }

            return false;
        }
    }
}
