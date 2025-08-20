# Changelog

## [0.3.0-exp.1] - 2025-08-20

### Added

* Added a shortcut to collapse or expand the selected nodes. Default value: `Ctrl/Cmd + Shift + O`.
* Added a shortcut Extract Contents To Placemat that applies on the selected subgraph. Default value: `Ctrl/Cmd + Shift + U`.
* Added a shortcut to convert the selected wires to portals. Default value: `Ctrl + Shift + P`.
* Added a default shortcut for toggling the Blackboard overlay on and off: B.
* Added a default shortcut for toggling the Graph Inspector overlay on and off: `I`.
* Added a default shortcut for toggling the Minimap overlay on and off: `M`.
* Added a shortcut to delete all wires on a node. Default value: `Ctrl/Cmd + Shift + W`.

### Changed

* Minor tweaks to the layout of the graph inspector
* Node Options are now defined via a builder, similar to ports.
* Node outputs no longer have a dark background around them

### Fixed

* Fixed issue where port fields would not be updated if the port data type changed
* MacOS: Shortcut `Convert Variable and Constant` is set to `Cmd + Shift + T`.
* After creating a read or write graph variable in a subgraph the parent graph's subgraph node's ports will immediately be updated.
* Removed border between inputs and outputs on context nodes
* Fixed some layout issues in context nodes
* In a Graph with two context types, the item library now show the correct block list for each context type.

### Removed

* Node Options can no longer include any attribute. Supported attributes are defined as builder methods (ex: `Delayed()`).
* Node Options no longer have a parameter for `order`, their order is defined by the order they're defined in.
* Additional CSS margin specific to 'execution flow' port labels. We no longer impose semantics (like 'execution flow') on ports.

## [0.2.0-exp.1] - 2025-07-29

### Added
* Set default shortcut to Create Sticky Note: `` Alt + ` ``.
* Set default shortcut for converting Variable node to Constant node (and vice-versa): `Ctrl + Shift + T`.

### Changed
* [Breaking] Inverted logic for `GraphOptions.AutoIncludeNodesFromGraphAssembly` (used as [Flags]). It's now `GraphOptions.DisableAutoInclusionOfNodesFromGraphAssembly` with the default value set to false instead of true. This prevents unintentionally clearing the flag when setting other flags, as it no longer defaults to 1. Update existing usages to explicitly include this flag if needed.
* Importing samples will now check for any missing package dependencies and prompt the user to install them if necessary.
* Reduced padding around visual elements in the Blackboard.

### Fixed
* Fixed the documentation page `Implement node options` where two parameters within `OnDefinePorts()` in one of the code snippets were inverted: `"Port Count"` and `k_PortCountName`.
* Fixed an issue where tab navigation did not work correctly within ContextNodes. Focus now moves sequentially through all visible fields.
* Fixed a visual artifact that appeared when hovering over a Sticky Note with an empty title.
* Fixed missing shortcut indicators for some actions in the Canvas context menu.
* Fixed variable names and colors not displaying correctly on variable instances when reloading the window (applies to Unity 6000.2.0b12 and later).
* Fixed missing visual cue (disc inside the circle) on ports when hovered by the cursor (applies to Unity 6000.2.0b12 and later).
* Fixed a UI issue with option-less and port-less nodes would be shrunk and not display its title (applies to Unity 6000.2.0b12 and later).

### Removed
* Removed the Tooltip field from the Variable quick access settings in the Blackboard panel.

## [0.1.0-exp.1] - 2025-07-15

### Added
* First experimental release of Graph Toolkit (UGTK)
