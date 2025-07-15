using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Used to build UI from model. See <see cref="GraphViewFactoryExtensions"/>.
    /// </summary>
    [UnityRestricted]
    internal class ElementBuilder
    {
        /// <summary>
        /// The root view of the element.
        /// </summary>
        public RootView View { get; set; }

        /// <summary>
        /// The context of the element.
        /// </summary>
        public IViewContext Context { get; set; }

        /// <summary>
        /// Optional parent view in case there is a hierarchy of <see cref="ChildView"/>s.
        /// </summary>
        public ChildView ParentView { get; set; }
    }
}
