using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Flags that define configuration options that affect the behavior and capabilities of a <see cref="Graph"/> class.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="GraphOptions"/> enum in conjunction with the <see cref="GraphAttribute"/> to customize how a graph behaves,
    /// including support for subgraphs and automatic node discovery. The default value is <see cref="GraphOptions.Default"/>, which enables
    /// standard behavior such as allowing nodes defined in the same assembly as the graph to be automatically included in the graph item library.
    /// Combine flags to customize behavior. This enum is marked with
    /// <see cref="System.FlagsAttribute"/>, so you can combine values using bitwise operations to enable multiple options.
    /// </remarks>
    /// <example>
    /// <code>
    /// [Graph(".mygraph", GraphOptions.Default | GraphOptions.SupportsSubgraphs)]
    /// public class MyGraph : Graph { }
    /// </code>
    /// <para>
    /// This example keeps the default behavior and adds support for subgraphs by combining <see cref="GraphOptions.Default"/> with <see cref="GraphOptions.SupportsSubgraphs"/>.
    /// </para>
    /// </example>
    [Flags]
    public enum GraphOptions
    {
        /// <summary>
        /// Indicates that this graph supports subgraphs.
        /// </summary>
        /// <remarks>
        /// When enabled, the “Convert Selection to Subgraph” item will be available in the right click menu of a selection of elements in the graph.
        /// </remarks>
        SupportsSubgraphs = 1<<0,

        /// <summary>
        /// Indicates that user-defined nodes (i.e., subclasses of <see cref="Node"/>) located in the same assembly as the graph will be automatically added to the graph item library.
        /// </summary>
        /// <remarks>
        /// This makes it easier to discover nodes without needing to manually annotate each one with <see cref="UseWithGraphAttribute"/>.
        /// Developers who want full control over what appears in the graph item library might choose to disable this option.
        /// </remarks>
        AutoIncludeNodesFromGraphAssembly = 1<<2,

        /// <summary>
        /// The default graph configuration. It currently includes <see cref="AutoIncludeNodesFromGraphAssembly"/>.
        /// </summary>
        /// <remarks>
        /// This default is helpful for onboarding: if users forget to mark nodes with <see cref="UseWithGraphAttribute"/>, they will still appear in the graph item library
        /// as long as they are defined in the same assembly as the graph.
        /// </remarks>
        Default = AutoIncludeNodesFromGraphAssembly,

        /// <summary>
        /// No graph options enabled.
        /// </summary>
        /// <remarks>
        /// This disables all optional features, including subgraph support and automatic node inclusion.
        /// </remarks>
        None = 0
    }
}
