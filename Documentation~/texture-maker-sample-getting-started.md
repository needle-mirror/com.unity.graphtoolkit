# Getting Started with Texture Maker Sample

## Prerequisites

* Unity 6000.2 or later
* Unity Graph ToolKit 0.1.0 or later

## Import the Sample

1. Open Package Manager
1. Select the Graph Toolkit package
1. Navigate to the Samples tab
1. Import the Texture Maker sample

Package Manager copies the sample files to your Assets folder. They are ready to use.

## Create Your First Texture

1. Click **Assets > Create > Graph Toolkit Samples > Texture Maker Graph**
1. Name the new graph asset, for example, `MyFirstTextureGraph`
1. Double-click the new graph asset to open the Graph Editor

At this point, there's a by-design error: `Assets/<name>.texmkr: Add a CreateTextureNode in your Texture graph.`

## Complete Your Graph

1. Right-click on the canvas and select **Create Node**
1. Double-click on **CreateTextureNode**
1. Add a **Uniform node** to the canvas
1. Select a color in the Uniform node
1. Connect the Uniform node output to the CreateTextureNode input
1. Save your graph

## Test Your Texture

Select `MyFirstTextureGraph.texmkr` in the Project window. The inspector displays the texture preview in the Inspector.

Drag this graph asset onto a GameObject to apply your texture. The GameObject now displays your selected color.

To modify the texture, change the Uniform node's color and save the graph to apply the updates in the Scene view.
