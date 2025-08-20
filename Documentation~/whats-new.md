# What's New in Unity Graph Toolkit 0.3.0-exp.1

[!INCLUDE [Experimental Warning message](experimental-release.md)]

This update adds default keyboard shortcuts to improve workflow and efficiency for your graphs, introduces a new way to define Node Options, and includes various bug fixes.

Discover new features and performance improvements in the latest update to Unity Graph Toolkit. 

For more information, refer to the [CHANGELOG](../CHANGELOG.md).


## New keyboard shortcuts

Added the following default keyboard shortcuts for graphs. 

|Command      | Shortcut (Windows) | Shortcut (macOS) | Description |
|:-------------|:---------------------|:------------------|:-------------|
|Toggle Blackboard |`B` | `B` | Toggles the Blackboard overlay on and off. |
|Toggle Inspector |`I` | `I` | Toggles the Graph Inspector overlay on and off. |
|Toggle Minimap |`M` | `M` | Toggles the Minimap overlay on and off. |
| Disconnect Wires | `Ctrl + Shift + W` | `Cmd + Shift + W` | Deletes all wires on the selected node. |
| Extract Contents To Placemat | `Ctrl + Shift + U` | `Cmd + Shift + U` | Extracts the contents of the selected subgraph to a new placemat. |
| Convert Variable and Constant | `Ctrl + Shift + T` | `Cmd + Shift + T` | Converts the selected variable nodes to constant nodes, or vice versa. |
| Convert Wires to Portals | `Ctrl + Shift + P` | `Cmd + Shift + P` | Converts the selected wires to portals. |
| Toggle Node Collapse | `Ctrl + Shift + O` | `Cmd + Shift + O` | Collapses or expands the selected nodes. |

Users can customize these shortcuts in the Unity Editor as follows: 
1. From the main menu, go to **Edit** > **Shortcuts** (macOS: **Unity** > **Shortcuts**).
1. In the **Category** column, select the name of the graph to customize the shortcuts bound to that graph.

For more details on how to work with shortcuts, refer to [Keyboard Shortcuts](https://docs.unity3d.com/Manual/ShortcutsManager.html).

## Changes to Node Options 
The following changes have been made to Node Options:

* Node Options are now defined via a builder, similar to ports.
* Node Options can no longer include any attributes. Supported attributes are defined as builder methods (for example, `Delayed()`).
* Node Options no longer have a parameter for `order`. Their order defaults to the order they're defined in.
