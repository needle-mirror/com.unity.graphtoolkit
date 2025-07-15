# Subgraph features

This page outlines the available [subgraph](glossary.md#subgraph) capabilities in Graph Toolkit.

| Feature                     | Description                                                                                                  | API Access |
|-----------------------------|--------------------------------------------------------------------------------------------------------------|------------|
| Creation                    | Create [subgraphs](glossary.md#subgraph) from selections of [nodes](glossary.md#node).                      | Complete   |
| Navigation                  | Navigate between graph levels using [breadcrumbs](glossary.md#breadcrumbs).                      | Not available*      |
| Local subgraphs             | Create [local subgraphs](glossary.md#local-subgraph) to group logic within a single graph as a unique, non-reusable instance.                         | Not available*      |
| Asset subgraphs             | Create reusable [asset subgraphs](glossary.md#asset-subgraph) for cross-graph reference.                     | Not available*      |
| Setting for item library path | Configure [subgraph](glossary.md#subgraph) categorization in the [item library](glossary.md#graph-item-library). | Not available*      |
| Extract                     | Expand [subgraphs](glossary.md#subgraph) into [placemats](glossary.md#placemat).                           | Not available*      |
| Convert from local to asset      | Transform [local subgraphs](glossary.md#local-subgraph) into reusable [asset subgraphs](glossary.md#asset-subgraph). | Not available*      |
| Unpack asset into Local       | Convert [asset subgraph](glossary.md#asset-subgraph) references into [local subgraphs](glossary.md#local-subgraph). | Not available*      |

*Features available in the editor interface but that don't have corresponding public API access.

## Related scripting API documentation

* [GraphOptions.SupportsSubGraphs](../api/Unity.GraphToolkit.Editor.GraphOptions.html)
* [SubgraphAttribute](../api/Unity.GraphToolkit.Editor.SubgraphAttribute.html)
