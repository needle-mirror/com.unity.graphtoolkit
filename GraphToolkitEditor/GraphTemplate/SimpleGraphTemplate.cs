using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// A graph template that gets its properties from a <see cref="SimpleGraphTool"/>.
    /// </summary>
    [UnityRestricted]
    internal class SimpleGraphTemplate : GraphTemplate
    {
        SimpleGraphTool m_Tool;

        /// <summary>
        /// Creates a new instance of the <see cref="SimpleGraphTemplate"/> class.
        /// </summary>
        /// <param name="tool">The GraphTool associated with this template.</param>
        public SimpleGraphTemplate(SimpleGraphTool tool)
            : base(tool.GenericGraphObjectName, tool.DefaultAssetExtension)
        {
            m_Tool = tool;
        }

        /// <inheritdoc />
        public override Type GraphModelType => m_Tool.DefaultGraphModelType;
    }
}
