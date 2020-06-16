---
title: "TreeView extension"
permalink: /docs/extensions/TreeView/
excerpt: "Learn how to use TreeView component."
toc: true
toc_label: "Guide"
---

## Overview

The treeview component a graphical control element that presents a hierarchical view of information. 

## Installation

### NuGet

Install treeview extension from NuGet.

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

A basic treeview that aims to reproduce standard treeview behavior.

```html
 <TreeView Nodes="Items" ChildSelector="@(item => item.Children)" @bind-SelectedNode="selectedNode" @bind-ExpandedNodes="ExpandedNodes" >
    <TextContent>@context.Text</TextContent>
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
| Nodes                 | IEnumerable<TNode>                                                                       |              | Collection of child treeview items (child nodes). If null/empty then this node won't expand. |
| TextContent           | string                                                                                   |              | Text string to display for the node.                                                         |
| SelectedNode          | TNode                                                                                    |              | The currently selected treeview item/node.                                                   |
| SelectedNodeChanged   | event                                                                                    |              | Occurs when the selected treeview node has changed.                                          |
| SetChildNodes         | action                                                                                   |              | Action that allows the child nodes to be set for a particular node                           |
| ExpandedNodes         | List<TNode>                                                                              |              | List of currently expanded treeview items (child nodes).                                     |
| ExpandedNodeChanged   | event                                                                                    |              | Occurs when the collection of expanded nodes has changed.                                    |
| ExpandNodeIconClass   | string                                                                                   |              | Class representing a closed treeview node.                                                   |
| CollapseNodeIconClass | string                                                                                   |              | Class representing an open treeview node                                                     |
| NodeTitleClass        | string                                                                                   |              | Class representing style for node content                                                    |
| NodeTitleSelectedClass| string                                                                                   |              | Class representing style for a selected node content                                         |
| Visible               | bool                                                                                     | true         | Is the treeview (or child treeview node) visible?                                            |
| HasChildNodes         | action                                                                                   | true         | Does the node have any child nodes?                                                          |