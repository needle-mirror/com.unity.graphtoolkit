# Instantiate and manage block nodes

This procedure assumes you have already:

* instantiated a context node in your canvas, as described in [Instantiate context nodes in your graph](context-node-instantiate-context-node.md).
* implemented a block node in your graph tool, as described in [Implement a block node](context-node-implement-block-node.md).

To add a block node inside a context node:

1. Click the `Add a Block` button in the context node to open the graph item library window.
   In this context, the window displays all available blocks that you can add to the context node.
1. Double-click to add your desired block node in the context node.

After you add it, you can disable, remove or connect a block node the same way as other nodes.

You can also drag the block node:

* to rearrange it within the context node
* to move it to another compatible context node

A context node doesn't allow you to add an incompatible block node. When you attempt to drag an incompatible block to a context node, here's what happened:

* the context node displays a red outline,
* at drop, the incompatible block automatically returns to its original position.

A block node can only exist within a context node and you can't add it to the graph directly.
