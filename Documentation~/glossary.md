# Glossary of Graph Toolkit

## A

### Asset subgraph

A [subgraph](#subgraph) serialized as its own independent asset file. Asset subgraphs are reusable across multiple graphs. A change made to the asset subgraph propagate to all its instances.

## B

### Blackboard

A panel that displays and manages variables available for use in the [graph](#graph). Users can create, edit, delete, organize variables and drag them directly into the graph [canvas](#canvas) for immediate use.

### Block node

A special type of [node](#node) usable only inside a [context node](#context-node). Block nodes can't exist on their own in the graph.

### Breadcrumbs

A navigation aid that shows where you're in the [graph](#graph) hierarchy. It gives you a direct access to jump to parent or child graphs.

## C

### Canvas

The main area of the [graph editor window](#graph-editor-window) where users can add and manipulate [nodes](#node), [wires](#wire), and other graph elements.

### Connector

The interactive element of a [port](#port) that serves as the attachment point for starting or ending a [wire](#wire) connection.

### Context node

A specialized [node](#node) that serves as a dynamic container for compatible [block nodes](#block-node).

### Constant node

A specialized [node](#node) that outputs a fixed value of a specific data type. You can convert constant nodes to [variable node](#variable-node).

## F

### Framework

A reusable collection of pre-built components, tools, and architecture patterns for building application. Unlike a simple library of functions, a framework defines how components interact and allows the execution of your custom code via specific extension points.

## G

### Graph

A collection of [nodes](#node) linked together by [wires](#wire).

### Graph editor window

The Unity Editor window that contains the [canvas](#canvas), the [blackboard](#blackboard), the [minimap](#minimap), the [graph inspector](#graph-inspector) and the [toolbars](#toolbar).

### Graph inspector

A panel that provides detailed information about the selected graph elements, such as [nodes](#node), [wires](#wire), [placemats](#placemat) or [sticky notes](#sticky-note). When you select nothing, it shows the properties of the [graph](#graph) itself.

### Graph item library

A catalog of graph elements (nodes, variables, etc.) that you can add to the current [graph](#graph) based on context. The term also refers to the user interface that allows browsing, searching, and inserting these items directly into the [canvas](#canvas) or in the [blackboard](#blackboard).

### Graph pull model

An execution model where a request for data or execution originates from a graph's output, and nodes retrieve inputs from their predecessors as needed to fulfill the request.

### Graph push model

An execution model where nodes actively process data and send outputs to connected nodes, propagating data from inputs to a final output.

## I

### Input port

A [port](#port) on a [node](#node) you can connect to [output ports](#output-port) using [wires](#wire).

### Input Port field

An editor component that provides direct value editing for an input [port](#port). This field becomes unavailable when the port is connected to an output via a [wire](#wire).

### Input/Output variable kind

A category of [variables](#variable). A variable of this kind represents data that are accessible outside the [graph](#graph) such as [subgraphs](#subgraph) node ports, shader properties or component fields. Input variables read data from external elements, while output variables write data back to external elements. Variable with [input/output variable kind](#inputoutput-variable-kind) feature color indicators (left for input, right for output) to denote their I/O status.

## L

### Local subgraph

A [subgraph](#subgraph) that's serialized directly within its parent [graph](#graph). Each local subgraph is a unique instance. Changes to one local subgraph don't affect other local subgraphs, even if you duplicated them from the same source.

### Local variable kind

A category of [variables](#variable). A variable of this kind is defined and serialized within the current [graph](#graph), and you can only access from within this graph.

## M

### Minimap

A small overview of the entire [graph](#graph), providing an alternative way to navigate and visualize the graph's structure.

## N

### Node

A fundamental building block of a [graph](#graph). It represents a specific operation or piece of logic in the graph. Nodes have [options](#node-option) and [ports](#port) that you can connect to each other via [wires](#wire).

### Node header

High-level information to help users quickly identify the [node](#node) like the title, a subtitle or category, a color and an icon.

### Node option

A typed property of a [node](#node) that can't receive values from other graph elements via wires, unlike ports. Node options typically control structural or functional aspects of the node, or serve as global settings for the node's behavior.

## O

### Output port

A [port](#port) on a [node](#node) that you can connect to [input ports](#input-port) using [wires](#wire).

## P

### Placemat

A visual background element that you can use to organize graph elements such as [nodes](#node) or [sticky notes](#sticky-note) in a graph. You can use it to show related elements, provide context, or improve the visual layout of the graph.

### Port

A connection point on a [node](#node). You can connect ports with [wires](#wire) to establish relationships between nodes. A port can be either an [input port](#input-port) or an [output port](#output-port). Ports are type-specific and you can only connect them to compatible ports or variables of the same type.

### Portal

A connection point that works in pairs and replace visible [wires](#wire) with wireless pathways to reduce visual clutter. They respect the same constraints as the wires they replace.

### Port name

A text description of the [port](#port).

## S

### Sticky note

A visual element you can use to add comments or annotations to a [graph](#graph).

### Subgraph

A [graph](#graph) that's nested within another graph, allowing for organization and reusability of graph logic. A subgraph can exist either as a [Local subgraph](#local-subgraph) or as a separate [asset subgraph](#asset-subgraph).

### Subgraph Node

A specialized node that references a subgraph and exposes its input and output variables as ports.

## T

### Toolbar

A customizable panel that provides direct access to common actions in the [graph editor window](#graph-editor-window) through button controls. Implemented using Unity's overlay system.

### Toolbar button

A button in the [toolbar](#toolbar) that performs a specific action in the [graph editor window](#graph-editor-window).

## V

### Variable

A data container accessible throughout a graph. Each variable has a name, type, and value. Variables appear in the blackboard and are represented as [variable nodes](#variable-node) when placed in a graph. All variables belong to either [local variable kind](#local-variable-kind) or [input/output variable kind](#inputoutput-variable-kind).

### Variable node

A specialized [node](#node) that references a [variable](#variable) and produces its value as output. Compatible with conversion to [constant nodes](#constant-node).

## W

### Wire

A line that creates a connection between [nodes](#node). Usually referred to as arcs or directed edges in Graph theory.
