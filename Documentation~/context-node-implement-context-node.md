# Implement a context node

Before you start, read [Introduction to context nodes](context-node-introduction.md) to understand the core concepts.

To implement a context node, define a class that inherits from the `ContextNode` base class:

[!code-csharp[](../Samples/DocCodeSamples/Editor/ContextNodeExamples.cs#MyContextNode)]

Context nodes inherit all features available in its parent `Node` class. Review [Implement a node](node-implement.md) to learn how to customize your node with node options, ports, and orientation settings.

## Next steps

- [Instantiate context nodes in your graph](context-node-instantiate-context-node.md)
- [Implement a block node](context-node-implement-block-node.md)

## Related scripting API documentation

- [ContextNode](../api/Unity.GraphToolkit.Editor.ContextNode.html)
