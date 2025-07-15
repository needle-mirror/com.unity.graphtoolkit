using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Unity.GraphToolkit.ItemLibrary.Editor
{
    /// <summary>
    /// Description of alignment for the <see cref="ItemLibraryWindow"/>.
    /// </summary>
    struct ItemLibraryWindowAlignment
    {
        /// <summary>
        /// Type of horizontal alignment.
        /// </summary>
        [UnityRestricted]
        internal enum HorizontalAlignment
        {
            /// <summary>
            /// Horizontally align to the left.
            /// </summary>
            Left = 0,
            /// <summary>
            /// Horizontally align to the center.
            /// </summary>
            Center,
            /// <summary>
            /// Horizontally align to the right.
            /// </summary>
            Right
        }

        /// <summary>
        /// Type of vertical alignment.
        /// </summary>
        [UnityRestricted]
        internal enum VerticalAlignment
        {
            /// <summary>
            /// Vertically align to the Top.
            /// </summary>
            Top = 0,
            /// <summary>
            /// Vertically align to the Center.
            /// </summary>
            Center,
            /// <summary>
            /// Vertically align to the Bottom.
            /// </summary>
            Bottom
        }

        /// <summary>
        /// The vertical alignment.
        /// </summary>
        public readonly VerticalAlignment Vertical;

        /// <summary>
        /// The horizontal alignment.
        /// </summary>
        public readonly HorizontalAlignment Horizontal;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemLibraryWindowAlignment"/> class.
        /// </summary>
        /// <param name="v">The vertical alignment.</param>
        /// <param name="h">The horizontal alignment.</param>
        public ItemLibraryWindowAlignment(VerticalAlignment v, HorizontalAlignment h)
        {
            Vertical = v;
            Horizontal = h;
        }

        public Vector2 AlignPosition(Vector2 position, Vector2 size)
        {
            switch (Horizontal)
            {
                case HorizontalAlignment.Center:
                    position.x -= size.x / 2;
                    break;

                case HorizontalAlignment.Right:
                    position.x -= size.x;
                    break;
            }

            switch (Vertical)
            {
                case VerticalAlignment.Center:
                    position.y -= size.y / 2;
                    break;

                case VerticalAlignment.Bottom:
                    position.y -= size.y;
                    break;
            }

            return position;
        }
    }
}
