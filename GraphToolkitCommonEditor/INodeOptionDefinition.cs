using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface that provides methods to declare node options inside a node.
    /// </summary>
    /// <remarks>
    /// Use to add node options on nodes. Node options appear under the node header and in the inspector when a node is selected. They are appropriate for parameters that affect how a node behaves or changes its topology,
    /// such as modifying the number of ports.
    /// </remarks>
    public interface INodeOptionDefinition
    {
        /// <summary>
        /// Adds a node option to the node.
        /// </summary>
        /// <typeparam name="T">The data type of the node option.</typeparam>
        /// <param name="optionName">The name of the node option, used for identification.</param>
        /// <param name="optionDisplayName">The name of the option shown in the UI, if specified.</param>
        /// <param name="tooltip">The tooltip to display when hovering over the option in the UI.</param>
        /// <param name="showInInspectorOnly">If true, shows the option only in the Inspector. By default, the option is shown in the Inspector and on the node.</param>
        /// <param name="order">The order index used to position this option relative to others. Lower values appear first.</param>
        /// <param name="attributes">An array of attributes to apply to the option field.</param>
        /// <param name="defaultValue">The default value assigned to the option when the node is created.</param>
        /// <remarks>
        /// Call this method in <c>Node.OnDefineOptions</c> to declare node-level settings.
        /// Use <c>Node.GetNodeOptionByName(string)</c> or <c>Node.GetNodeOption(int)</c> to retrieve the option.
        /// The <c>optionName</c> is unique within the node's input ports and node options.
        /// </remarks>
        /// <example>
        /// <code>
        /// protected override void OnDefineOptions(INodeOptionDefinition context)
        /// {
        ///     context.AddNodeOption&lt;string&gt;(
        ///         optionName: "Label",
        ///         optionDisplayName: "My Label",
        ///         tooltip: "A label.",
        ///         defaultValue: "Default Value");
        /// }
        /// </code>
        /// </example>
        void AddNodeOption<T>(string optionName, string optionDisplayName = null, string tooltip = null, bool showInInspectorOnly = false, int order = 0, Attribute[] attributes = null, T defaultValue = default)
        {
            AddNodeOption(optionName, typeof(T), optionDisplayName, tooltip, showInInspectorOnly, order, attributes, defaultValue);
        }

        /// <summary>
        /// Adds a node option to the node.
        /// </summary>
        /// <param name="optionName">The name of the node option, used for identification.</param>
        /// <param name="dataType">The data type of the node option.</param>
        /// <param name="optionDisplayName">The name of the option shown in the UI, if specified.</param>
        /// <param name="tooltip">The tooltip to display when hovering over the option in the UI.</param>
        /// <param name="showInInspectorOnly">If true, shows the option only in the Inspector. By default, the option is shown in the Inspector and on the node.</param>
        /// <param name="order">The order index used to position this option relative to others. Lower values appear first.</param>
        /// <param name="attributes">An array of attributes to apply to the option field.</param>
        /// <param name="defaultValue">The default value assigned to the option when the node is created.</param>
        /// <remarks>
        /// Call this method in <c>Node.OnDefineOptions</c> to declare node-level settings.
        /// Use <c>Node.GetNodeOptionByName(string)</c> or <c>Node.GetNodeOption(int)</c> to retrieve the option.
        /// The <c>optionName</c> is unique within the node's input ports and node options.
        /// </remarks>
        /// <example>
        /// <code>
        /// protected override void OnDefineOptions(INodeOptionDefinition context)
        /// {
        ///     context.AddNodeOption(
        ///         optionName: "Label",
        ///         dataType: typeof(string),
        ///         optionDisplayName: "My Label",
        ///         tooltip: "A label.",
        ///         defaultValue: "Default Value");
        /// }
        /// </code>
        /// </example>
        void AddNodeOption(string optionName, Type dataType, string optionDisplayName = null, string tooltip = null, bool showInInspectorOnly = false, int order = 0, Attribute[] attributes = null, object defaultValue = null);
    }
}
