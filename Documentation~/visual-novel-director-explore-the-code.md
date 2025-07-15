# Explore the code of the Visual Novel Director sample

## Custom runtime

Graph Toolkit doesn't come with a runtime, so this sample demonstrates how to create a simple custom runtime.

This custom runtime can be found in the Runtime folder (`Unity.GraphToolkit.Samples.VisualNovelDirector.asmdef` assembly) and is divided into two parts:

1. The runtime data model: `VisualNovelRuntimeGraph`, which stores a list of `VisualNovelRuntimeGraphNode`s.
1. The runtime execution engine: `VisualNovelDirector` and `VisualNovelRuntimeGraphNodeExecutor`, which execute the graph at runtime.

### Runtime data model

The Graph Toolkit data model has references to the editor and can't be used directly at runtime. This means you have to implement your own data model.

In the Visual Novel Director example runtime, the `VisualNovelRuntimeGraph` is a `[ScriptableObject]` that stores a list of `VisualNovelRuntimeGraphNode`s using `[SerializeReference]`.

This custom runtime model allows you:

* to store the minimal data required for the runtime without unnecessary authoring information, such as node positions.
  * As an example, the `WaitForInputNode` is completely empty because it acts as a marker in the `VisualNovelRuntimeGraph` without any actual state.
* to use any preferred loading and serialization mechanism (for example MonoBehaviours or ECS).
  * As an example, `VisualNovelDirector` sample uses a `MonoBehaviour` (the `VisualNovelDirector`) which stores a direct reference to the `VisualNovelRuntimeGraph`. This lets Unity handle the loading of the asset at runtime.
* to create complex, modular runtime behaviour from simple authoring nodes (i.e. authoring and runtime doesn't have to be 1:1).
### Runtime execution

Graph Toolkit doesn't have a graph-based runtime engine. This means you have to implement your own runtime, which can be node-based or not.

In this sample, the `VisualNovelDirector` `MonoBehaviour` executes nodes linearly. For simplicity, the sample excludes branching or choices. The `VisualNovelDirector` uses the loaded `VisualNovelRuntimeGraph`, public configuration fields, and asset references for all required UI elements (background image, dialogue text box, etc).

This custom runtime executes the graph from any defined entry-point. The `VisualNovelDirector` is a `MonoBehaviour` and uses Unity events. On Start, it instantiates `VisualNovelRuntimeGraphNodeExecutor`s and calls them in sequence, using the data in the runtime nodes.

Sample runtime executors execute one or more specific node types. For example, `SetBackgroundExecutor` executes `SetBackgroundNode`.

For more details on how the example runtime works (accepting input, etc), review the code and its comments.
