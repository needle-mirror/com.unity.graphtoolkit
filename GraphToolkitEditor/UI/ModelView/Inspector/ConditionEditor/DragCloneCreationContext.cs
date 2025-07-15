using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// A IVewContext used to create a clone of the condition <see cref="ModelView"/>s for drag and drop.
    /// </summary>
    [UnityRestricted]
    internal class DragCloneCreationContext : IViewContext
    {
        public static readonly DragCloneCreationContext Default = new DragCloneCreationContext();

        public bool Equals(IViewContext other)
        {
            return ReferenceEquals(this, other);
        }
    }
}
