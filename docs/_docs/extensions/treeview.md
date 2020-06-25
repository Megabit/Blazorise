---
title: "TreeView extension"
permalink: /docs/extensions/treeview/
excerpt: "Learn how to use TreeView component."
toc: true
toc_label: "Guide"
---

## Overview

The TreeView component is a graphical control element that presents a hierarchical view of information. 

## Installation

### NuGet

Install TreeView extension from NuGet.

```
Install-Package Blazorise.TreeView
```

### Imports

In your main _Imports.razor_ add:

```cs
@using Blazorise.TreeView
```

### Static files

Include CSS link into your `index.html` or `_Host.cshtml` file, depending if you're using a Blazor WebAssembly or Blazor Server side project.

```html
<link href="_content/Blazorise.TreeView/blazorise.treeview.css" rel="stylesheet" />
```

## Usage

### Basic example

A basic TreeView that aims to reproduce standard tree-view behavior.

```html
 <TreeView Nodes="Items"
    GetChildNodes="@(item => item.Children)"
    HasChildNodes="@(item => item.Children?.Any() == true)"
    @bind-SelectedNode="selectedNode"
    @bind-ExpandedNodes="ExpandedNodes" >
    <NodeContent>@context.Text</NodeContent>
</TreeView>

@code{
 public class Item
    {
        public string Text { get; set; }
        public IEnumerable<Item> Children { get; set; }
    }

    IEnumerable<Item> Items = new[]
    {
        new Item { Text = "Item 1" },
        new Item {
            Text = "Item 2",
            Children = new []
            {
                new Item { Text = "Item 2.1" },
                new Item { Text = "Item 2.2", Children = new []
                {
                    new Item { Text = "Item 2.2.1" },
                    new Item { Text = "Item 2.2.2" },
                    new Item { Text = "Item 2.2.3" },
                    new Item { Text = "Item 2.2.4" }
                }
            },
            new Item { Text = "Item 2.3" },
            new Item { Text = "Item 2.4" }
            }
        },
        new Item { Text = "Item 3" },
    };

    IList<Item> ExpandedNodes = new List<Item>();
    Item selectedNode;
}
```

## Attributes

| Name                  | Type                                                                                     | Default      | Description                                                                                  |
|-----------------------|------------------------------------------------------------------------------------------|--------------|----------------------------------------------------------------------------------------------|
| Nodes                 | IEnumerable<TNode>                                                                       |              | Collection of child TreeView items (child nodes). If null/empty then this node won't expand. |
| NodeContent           | `RenderFragment<TNode>`                                                                  |              | Template to display content for the node.                                                    |
| SelectedNode          | TNode                                                                                    |              | The currently selected TreeView item/node.                                                   |
| SelectedNodeChanged   | event                                                                                    |              | Occurs when the selected TreeView node has changed.                                          |
| ExpandedNodes         | List<TNode>                                                                              |              | List of currently expanded TreeView items (child nodes).                                     |
| ExpandedNodeChanged   | event                                                                                    |              | Occurs when the collection of expanded nodes has changed.                                    |
| GetChildNodes         | expression                                                                               |              | Expression  that allows the child nodes to be identifies for a particular node               |
| HasChildNodes         | expression                                                                               | true         | Expression that indicates whether the current node has any children nodes?                   |