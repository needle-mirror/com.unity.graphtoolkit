# Explore the code of the Texture Maker sample

## Use a custom asset

Unity differentiates between built-in and custom assets through unique file extensions.

Texture Maker requires its own file extension because it operates with a custom asset. That's why the `TextureMakerGraph` class is decorated with the `[Graph(AssetExtension)]` attribute with `AssetExtension = "texmkr"`, indicating that it uses the `.texmkr` file extension. This extension is essential for Unity to recognize and handle the asset correctly.

When you implement a custom extension, Graph Toolkit automatically provides standard Unity asset behaviors:

* Auto-saving your custom asset when you select Save Scene
* Reloading your asset from disk when Unity detects external file changes
* Removing the asset and closing associated windows when you delete the file in Project Browser

## Graph runtime and scripted importer

Texture Maker runtime works through a `ScriptedImporter` (`TextureMakerImporter.cs`). This importer observes changes to the `.texmkr` graph asset. When the asset is imported, the importer finds the first `CreateTextureNode`. This is the entry point for evaluation, 'pulling' data from upstream nodes (Uniform, MeanColor, CreateTexture) to produce the final `Texture2D` asset.

This describes a [pull execution model](glossary.md#graph-pull-model).

A `ScriptedImporter` is a common method to bridge Graph Toolkit's editor-time graph assets with your custom runtime logic. It defines how your custom graph file processes and converts into a usable runtime format.

In Texture Maker, the `ScriptedImporter` transforms the graph asset into a built-in Unity runtime object (a `Texture2D`). For an example of a `ScriptedImporter` converting a graph asset into a custom runtime object, check out the [Visual Novel Director sample](visual-novel-director-explore-the-code.md).

## Log message, warning and error

'Texture Maker' sample also demonstrates how to log messages, warnings, and errors related to the graph. This is crucial for providing feedback to users about the state of their graph and guiding them in making necessary corrections.

To do so, the `TextureMakerGraph` class overrides the `OnGraphChanged` method. This method is called automatically whenever a user modifies the graph, making it the right place for real-time validation of the graph state and appropriate message logging.

You can use the `GraphLogger` to log messages, warnings, and errors.

When you log messages:

* All message appears in the Unity Console while the graph editor window is open
* Messages linked to specific nodes display visual markers on those nodes in the graph editor
