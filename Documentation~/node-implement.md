# Implement a node

Nodes are the fundamental building blocks of any graph. This page shows how to create custom node types with input and output ports. 

It covers the node visual implementation but not the execution logic, as the later depends on your graph tool's purpose.

## Create a new type of node

To create a node, define a class that inherits from the `Node` base class. To ensure automatic discovery, define your node class in the same assembly as your graph tool. The node class name serves as the node's title in the interface.

[!code-csharp[](../Samples/DocCodeSamples/Editor/NodeExamples.cs#BasicNode)]

> [!NOTE]
> Notice that the nodes include a `Serializable` attribute. This attribute enables the tool to write your nodes into the graph asset and deserialize them when you open the graph.

## Define ports

Configure the data connections for your node by overriding the `OnDefinePorts` method and using the `AddInputPort`
and `AddOutputPort` methods of the `IPortDefinitionContext` interface.

[!code-csharp[](../Samples/DocCodeSamples/Editor/NodeExamples.cs#BasicPorts)]

> [!NOTE]
> Ports support any Unity-compatible type, including your custom types. Omitting a type creates connection-only ports that link nodes without transferring data.

> [!TIP]
> Use optional methods such as `WithDisplayName` or `WithConnectorUI` to customize your port.
> ```cs
> context.AddInputPort<int>("a")
>     .WithDisplayName("My Int")
>     .WithConnectorUI(PortConnectorUI.Arrowhead)
>     .Build();
> ```

## Node complete example

[!code-csharp[](../Samples/DocCodeSamples/Editor/NodeExamples.cs#BasicNodeComplete)]

## Next step

[Instantiate the node to your graph](node-instantiate.md)