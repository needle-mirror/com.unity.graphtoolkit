using System;
using Unity.GraphToolkit.ItemLibrary.Editor;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface to define custom data for an <see cref="ItemLibraryItem"/>.
    /// </summary>
    [UnityRestricted]
    internal interface IItemLibraryData
    {
    }

    /// <summary>
    /// Tag for specific <see cref="ItemLibraryItem"/>.
    /// </summary>
    [UnityRestricted]
    internal enum CommonLibraryTags
    {
        StickyNote
    }

    /// <summary>
    /// Data for a <see cref="ItemLibraryItem"/> tagged by a <see cref="CommonLibraryTags"/>.
    /// </summary>
    [UnityRestricted]
    internal readonly struct TagItemLibraryData : IItemLibraryData
    {
        public CommonLibraryTags Tag { get; }

        public TagItemLibraryData(CommonLibraryTags tag)
        {
            Tag = tag;
        }
    }

    /// <summary>
    /// Data for a <see cref="ItemLibraryItem"/> linked to a type.
    /// </summary>
    [UnityRestricted]
    internal readonly struct TypeItemLibraryData : IItemLibraryData
    {
        /// <summary>
        /// The type associated with the item.
        /// </summary>
        public TypeHandle Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeItemLibraryData"/> class.
        /// </summary>
        /// <param name="type">The type associated with the item library data.</param>
        /// <remarks>
        /// Use this constructor when you need to create a new instance of <see cref="TypeItemLibraryData"/> for a specific type. The type parameter links the data to a particular type.
        /// </remarks>
        public TypeItemLibraryData(TypeHandle type)
        {
            Type = type;
        }
    }

    /// <summary>
    /// Data for a <see cref="ItemLibraryItem"/> linked to a node.
    /// </summary>
    [UnityRestricted]
    internal readonly struct NodeItemLibraryData : IItemLibraryData
    {
        /// <summary>
        /// The type of the node represented by the item.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The port to which the node will be connected.
        /// </summary>
        public PortModel PortToConnect { get; }

        /// <summary>
        /// The subgraph reference when the item is associated with a subgraph.
        /// </summary>
        public GraphReference SubgraphReference { get; }

        /// <summary>
        /// Initializes a new instance of the NodeItemLibraryData class.
        /// </summary>
        /// <param name="type">Type of the node represented by the item.</param>
        /// <param name="subgraphReference">A reference to the subgraph to create.</param>
        public NodeItemLibraryData(Type type, GraphReference subgraphReference)
        {
            Type = type;
            PortToConnect = null;
            SubgraphReference = subgraphReference;
        }

        /// <summary>
        /// Initializes a new instance of the NodeItemLibraryData class.
        /// </summary>
        /// <param name="type">Type of the node represented by the item.</param>
        /// <param name="portToConnect">The port to which the node will be connected, if created.</param>
        public NodeItemLibraryData(Type type, PortModel portToConnect)
        {
            Type = type;
            PortToConnect = portToConnect;
            SubgraphReference = default;
        }

        /// <summary>
        /// Initializes a new instance of the NodeItemLibraryData class.
        /// </summary>
        /// <param name="type">Type of the node represented by the item.</param>
        public NodeItemLibraryData(Type type)
        {
            Type = type;
            PortToConnect = null;
            SubgraphReference = default;
        }
    }
}
