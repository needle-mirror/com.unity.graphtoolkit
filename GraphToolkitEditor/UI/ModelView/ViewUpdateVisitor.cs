using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Abstract class for recursively updating a <see cref="ChildView"/> and its <see cref="ModelViewPart"/>s.
    /// </summary>
    [UnityRestricted]
    internal abstract class ViewUpdateVisitor
    {
        /// <summary>
        /// Updates the view.
        /// </summary>
        /// <param name="view">The view to update.</param>
        public abstract void Update(ChildView view);

        /// <summary>
        /// Updates the part.
        /// </summary>
        /// <param name="part">The part to update.</param>
        public abstract void Update(ModelViewPart part);
    }
}
