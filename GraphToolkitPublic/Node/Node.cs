using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor.Implementation;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// The base class for all user-accessible nodes in a graph.
    /// </summary>
    /// <remarks>
    /// Inherit from this class to define custom node types that appear in the graph. The <see cref="Node"/> class provides
    /// lifecycle hooks, serialization support, and the structure needed to define ports, UI behaviors, and custom logic.
    /// This class forms the foundation of all user-defined nodes in a graph-based tool, including variable nodes, context nodes,
    /// and subgraph nodes.
    /// <br/>
    /// <br/>
    /// To create a custom node, derive from <see cref="Node"/>, define its input and output ports using a port builder in <see cref="OnDefinePorts"/>,
    /// and define its <see cref="INodeOption"/>s in <see cref="OnDefineOptions"/>.
    /// <br/>
    /// <br/>
    /// This class is used in combination with other types like <see cref="INode"/>, <see cref="IPort"/>, and <see cref="Graph"/>
    /// to construct and manage node-based workflows.
    /// <br/>
    /// <br/>
    /// See also:
    /// <list type="bullet">
    /// <item><description><see cref="INode"/> for the interface this class implements</description></item>
    /// <item><description><see cref="ContextNode"/> and <see cref="BlockNode"/> for composition patterns</description></item>
    /// <item><description><see cref="IVariableNode"/> for how to work with variable-based nodes</description></item>
    /// <item><description><see cref="ISubgraphNode"/> for how to work with subgraph-based nodes</description></item>
    /// </list>
    /// </remarks>
    [Serializable]
    public abstract partial class Node : INode
    {
        /// <summary>
        /// Interface that provides methods to define input and output ports during <see cref="Node.OnDefinePorts"/> execution.
        /// </summary>
        /// <remarks>
        /// Use this interface within <see cref="Node.OnDefinePorts"/> to declare the ports a node exposes.
        /// Ports define how the node connects to other nodes by specifying inputs and outputs.
        /// </remarks>
        public interface IPortDefinitionContext
        {
            /// <summary>
            /// Adds a new input port.
            /// </summary>
            /// <param name="portName">The unique identifier of the input port.</param>
            /// <returns>An <see cref="IInputPortBuilder"/> to further configure the input port.</returns>
            /// <remarks>
            /// <c>portName</c> is used to identify the port. It must be unique among input ports and node options on the node. This name is used as the ID when calling <see cref="GetInputPortByName(string)"/>.
            /// If <see cref="IPortBuilder{T}.WithDisplayName(string)"/> is not used, this name is also used as the port's display label.
            /// <br/>
            /// <br/>
            /// <b>Warning:</b> Changing a port's name will break any existing connections, as the name is used as the port's unique ID.
            /// <br/>
            /// <br/>
            /// Use the returned builder to configure port properties and then call <see cref="IPortBuilder{T}.Build"/> to create the port.
            /// </remarks>
            /// <example>
            /// <code>
            /// var port = context.AddInputPort("myInput")
            ///     .WithDisplayName("My Input Port")
            ///     .WithDataType&lt;int&gt;()
            ///     .WithConnectorUI(PortConnectorUI.Circle)
            ///     .Build();
            /// </code>
            /// </example>
            IInputPortBuilder AddInputPort(string portName);

            /// <summary>
            /// Adds a new output port with the specified name.
            /// </summary>
            /// <param name="portName">The unique identifier of the output port.</param>
            /// <returns>An <see cref="IOutputPortBuilder"/> to further configure the output port.</returns>
            /// <remarks>
            /// <c>portName</c> is used to identify the port. It must be unique among output ports on the node. This name is used as the ID when calling <see cref="GetOutputPortByName(string)"/>.
            /// If <see cref="IPortBuilder{T}.WithDisplayName(string)"/> is not used, this name is also used as the port's display label.
            /// <br/>
            /// <br/>
            /// <b>Warning:</b> Changing a port's name will break any existing connections, as the name is used as the port's unique ID.
            /// <br/>
            /// <br/>
            /// Use the returned builder to configure port properties and then call <see cref="IPortBuilder{T}.Build"/> to create the port.
            /// </remarks>
            /// <example>
            /// <code>
            /// var port = context.AddOutputPort("myOutput")
            ///     .WithDisplayName("My Output Port")
            ///     .WithDataType(typeof(float))
            ///     .WithConnectorUI(PortConnectorUI.Arrowhead)
            ///     .Build();
            /// </code>
            /// </example>
            IOutputPortBuilder AddOutputPort(string portName);

            /// <summary>
            /// Adds a new typed input port with the specified name.
            /// </summary>
            /// <typeparam name="T">The data type of the input port.</typeparam>
            /// <param name="portName">The unique identifier of the input port.</param>
            /// <returns>An <see cref="IInputPortBuilder{T}"/> to further configure the typed input port.</returns>
            /// <remarks>
            /// <c>portName</c> is used to identify the port. It must be unique among input ports on the node. This name is used as the ID when calling <see cref="GetInputPortByName(string)"/>.
            /// If <see cref="IPortBuilder{T}.WithDisplayName(string)"/> is not used, this name is also used as the port's display label.
            /// <br/>
            /// <br/>
            /// <b>Warning:</b> Changing a port's name will break any existing connections, as the name is used as the port's unique ID.
            /// <br/>
            /// <br/>
            /// Use the returned builder to configure port properties and then call <see cref="IPortBuilder{T}.Build"/> to create the port.
            /// </remarks>
            /// <example>
            /// <code>
            /// var port = context.AddInputPort&lt;string&gt;("stringInput")
            ///     .WithDisplayName("String Input")
            ///     .WithDefaultValue("default text")
            ///     .Build();
            /// </code>
            /// </example>
            IInputPortBuilder<T> AddInputPort<T>(string portName)
            {
                return AddInputPort(portName).WithDataType<T>();
            }

            /// <summary>
            /// Adds a new typed output port with the specified name.
            /// </summary>
            /// <typeparam name="T">The data type of the output port.</typeparam>
            /// <param name="portName">The unique identifier of the output port.</param>
            /// <returns>An <see cref="IOutputPortBuilder{T}"/> to further configure the typed output port.</returns>
            /// <remarks>
            /// <c>portName</c> is used to identify the port. It must be unique among output ports on the node. This name is used as the ID when calling <see cref="GetOutputPortByName(string)"/>.
            /// If <see cref="IPortBuilder{T}.WithDisplayName(string)"/> is not used, this name is also used as the port's display label.
            /// <br/>
            /// <br/>
            /// <b>Warning:</b> Changing a port's name will break any existing connections, as the name is used as the port's unique ID.
            /// <br/>
            /// <br/>
            /// Use the returned builder to configure port properties and then call <see cref="IPortBuilder{T}.Build"/> to create the port.
            /// </remarks>
            /// <example>
            /// <code>
            /// var port = context.AddOutputPort&lt;bool&gt;("boolOutput")
            ///     .WithDisplayName("Boolean Output")
            ///     .Build();
            /// </code>
            /// </example>
            IOutputPortBuilder<T> AddOutputPort<T>(string portName)
            {
                return AddOutputPort(portName).WithDataType<T>();
            }
        }

        /// <summary>
        /// Called when the node is created or when the graph is enabled.
        /// </summary>
        /// <remarks>
        /// Use this method to perform initialization logic.
        /// </remarks>
        public virtual void OnEnable() {}

        /// <summary>
        /// Called when the node is removed or when the graph is disabled.
        /// </summary>
        /// <remarks>
        /// Use this method to perform any cleanup logic.
        /// </remarks>
        public virtual void OnDisable() {}

        /// <summary>
        /// Defines the structure of the node by building its ports and options.
        /// </summary>
        /// <remarks>
        /// This method calls both <see cref="OnDefineOptions"/> and <see cref="OnDefinePorts"/>
        /// to allow custom definition of the node.
        /// </remarks>
        public void DefineNode()
        {
            (m_Implementation as NodeModel)?.DefineNode();
        }

        /// <summary>
        /// Called during <see cref="DefineNode"/> to define the options available on the node.
        /// </summary>
        /// <param name="context">Provides methods for defining node options.</param>
        /// <remarks>
        /// This method is called before <see cref="OnDefinePorts"/>. Override this method to add node options using the provided <see cref="INodeOptionDefinition"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// protected override void OnDefineOptions(INodeOptionDefinition context)
        /// {
        ///     context.AddNodeOption&lt;bool&gt;(
        ///         optionName: "My Bool",
        ///         optionDisplayName: "myBoolId");
        ///
        ///     context.AddNodeOption(
        ///         optionName: "Label",
        ///         dataType: typeof(string),
        ///         optionDisplayName: "labelId",
        ///         tooltip: "A label.",
        ///         defaultValue: "Default Value");
        /// }
        /// </code>
        /// </example>
        protected virtual void OnDefineOptions(INodeOptionDefinition context) {}

        /// <summary>
        /// Called during <see cref="DefineNode"/> to define the input and output ports of the node.
        /// </summary>
        /// <param name="context">Provides methods for defining input and output ports.</param>
        /// <remarks>
        /// This method is called after <see cref="OnDefineOptions"/> and is used to declare the structure of the node's connectivity.
        /// Use the provided <see cref="IPortDefinitionContext"/> to define input and output ports using a builder pattern.
        /// The port builder pattern enables fluent configuration of ports by chaining methods such as
        /// <c>WithDisplayName</c>, <c>WithDefaultValue</c>, or <c>WithConnectorUI</c>, followed by <c>Build()</c> to finalize the port.
        /// The <c>portName</c> parameter passed to <c>AddInputPort</c> or <c>AddOutputPort</c> serves as the port's unique identifier.
        /// The <c>portName</c> parameter must be unique within its direction (input or output) on the node and is also used as the display name unless you explicitly call <c>WithDisplayName</c>.
        /// You also use the <c>portName</c> identifier to call <see cref="Node.GetInputPortByName"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// protected override void OnDefinePorts(IPortDefinitionContext context)
        /// {
        ///     var inputPort = context.AddInputPort&lt;string&gt;("stringInput")
        ///         .WithDisplayName("String Input")
        ///         .WithDefaultValue("Default Value")
        ///         .Build();
        ///
        ///     var outputPort = context.AddOutputPort("myOutput")
        ///         .WithDisplayName("My Output Port")
        ///         .WithDataType(typeof(float))
        ///         .WithConnectorUI(PortConnectorUI.Arrowhead)
        ///         .Build();
        /// }
        /// </code>
        /// </example>
        protected virtual void OnDefinePorts(IPortDefinitionContext context) {}

        /// <summary>
        /// The number of node options defined in the node.
        /// </summary>
        public int nodeOptionCount => m_Implementation is InputOutputPortsNodeModel ioNodeModel ? ioNodeModel.NodeOptions.Count : 0;

        /// <summary>
        /// Retrieves a node option using its zero-based index.
        /// </summary>
        /// <param name="index">Index of the node option, based on display order.</param>
        /// <returns>The node option at the specified index.</returns>
        /// <remarks>
        /// The index is zero-based.
        /// </remarks>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown if the index is out of bounds.
        /// </exception>
        public INodeOption GetNodeOption(int index)
        {
            if (m_Implementation is IUserNodeModelImp customNodeModel)
            {
               return customNodeModel.NodeOptions[index];
            }

            throw new ArgumentOutOfRangeException(nameof(index), index, "Index is out of range for the node options in this node.");
        }

        /// <summary>
        /// The node options defined on this node.
        /// </summary>
        public IEnumerable<INodeOption> nodeOptions => m_Implementation is InputOutputPortsNodeModel ioNodeModel ? ioNodeModel.NodeOptions : Array.Empty<INodeOption>();

        /// <summary>
        /// Retrieves a node option using its name.
        /// </summary>
        /// <param name="name">The unique name of the node option.</param>
        /// <returns>The node option with the specified name, or null if none is found.</returns>
        /// <remarks>The node option's name is unique within the node's input ports and node options.</remarks>
        public INodeOption GetNodeOptionByName(string name) => m_Implementation is IUserNodeModelImp customNodeModel ? customNodeModel.GetNodeOptionByName(name) : null;

        /// <inheritdoc />
        public int inputPortCount => ((INode)m_Implementation).inputPortCount;

        /// <inheritdoc />
        public IPort GetInputPort(int index) => ((INode)m_Implementation).GetInputPort(index);

        /// <inheritdoc />
        public IEnumerable<IPort> GetInputPorts() => ((INode)m_Implementation).GetInputPorts();

        /// <inheritdoc />
        public IPort GetInputPortByName(string name) => ((INode)m_Implementation).GetInputPortByName(name);

        /// <inheritdoc />
        public int outputPortCount => ((INode)m_Implementation).outputPortCount;

        /// <inheritdoc />
        public IPort GetOutputPort(int index) => ((INode)m_Implementation).GetOutputPort(index);

        /// <inheritdoc />
        public IEnumerable<IPort> GetOutputPorts() => ((INode)m_Implementation).GetOutputPorts();

        /// <inheritdoc />
        public IPort GetOutputPortByName(string name) => ((INode)m_Implementation).GetOutputPortByName(name);
    }
}
