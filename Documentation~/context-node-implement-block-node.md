# Implement a block node

This procedure assumes you have already implemented a context node, as described in
[Implement a context node](context-node-implement-context-node.md).

To implement a block node:

1. Define a class that inherits from the `BlockNode` base class
1. Add the `UseWithContext` attribute to connect it to your context node

The resulting code looks like:

[!code-csharp[](../Samples/DocCodeSamples/Editor/ContextNodeExamples.cs#MyBlockNode)]

To relate a block node to several context nodes, list them separated with a comma in your `UseWithContext` attribute:

[!code-csharp[](../Samples/DocCodeSamples/Editor/ContextNodeExamples.cs#MyBlockNodeWithMultipleContexts)]

Block nodes inherit all features available in its parent `Node` class. Review [Implement a node](node-implement.md) to learn how to customize your node with node options, ports, and orientation settings.

## Next step

- [Instantiate and manage block nodes](context-node-instantiate-manage-block-node.md)

## Related scripting API documentation

- [ContextNode](../api/Unity.GraphToolkit.Editor.ContextNode.html)
- [BlockNode](../api/Unity.GraphToolkit.Editor.BlockNode.html)
- [UseWithContextAttribute](../api/Unity.GraphToolkit.Editor.UseWithContextAttribute.html)
