# Getting started with the Visual Novel Director sample

## Prerequisites

This sample requires the following:

* Unity 6000.2 or later
* Unity Graph ToolKit 0.1.0 or later
* Input System 1.1.4 or later

## Import the sample

1. Open Package Manager
1. Select the Graph Toolkit package
1. Navigate to the Samples tab
1. Import the Visual Novel Director sample

The Package Manager copies sample files in your Assets folder, that's now ready to use.

## Test the sample

1. Open `VisualNovelScene.unity` (`Assets/Samples/Graph Toolkit/<installed package version>/Visual Novel Director Sample/Assets/VisualNovelScene.unity`).
1. Enter Play Mode to watch the novel.

> Thanks to Olivia You-Tuon (@shumijin) for the visual novel assets used in this sample.

The novel uses the `My Visual Novel Graph.vnd` graph, located in the same folder. Double-click it to open the graph editor.

You can now explore the graph and discover how it works.

## Create your own visual novel

1. Click **Assets > Create > Graph Toolkit Samples > Visual Novel Director Graph**.
1. Name the new graph asset (for example, `My Own Visual Novel Graph`).
1. Double-click the new graph asset to open the Graph Editor.

Right-click in the graph to create and connect nodes. Saving the graph automatically builds the runtime asset and links it to the same disk asset. Drag the graph asset onto a `VisualNovelDirector` component in the scene.

The `BasicVisualNovelCanvas` prefab contains the UI elements for displaying the visual novel, with references set up for the `VisualNovelDirector`.
