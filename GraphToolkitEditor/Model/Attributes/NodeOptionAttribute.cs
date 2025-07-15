using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Attribute to mark a field as being a node option, one that appear in the Node Options section
    /// of the model inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [UnityRestricted]
    internal class NodeOptionAttribute : DisplayNameAttribute
    {
        /// <summary>
        /// Whether the node option should only be shown in the inspector.
        /// </summary>
        /// <remarks>All node options should show up in the inspector, but not all node options should show up on the node.</remarks>
        public bool ShowInInspectorOnly { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeOptionAttribute"/> class.
        /// </summary>
        public NodeOptionAttribute()
            : base(null)
        {
            ShowInInspectorOnly = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeOptionAttribute"/> class.
        /// </summary>
        /// <param name="showInInspectorOnly">Whether the node option should only be shown in the inspector.</param>
        /// <param name="displayName">The displayed name of the node option</param>
        public NodeOptionAttribute(bool showInInspectorOnly = false, string displayName = null)
            : base(displayName)
        {
            ShowInInspectorOnly = showInInspectorOnly;
        }
    }
}
