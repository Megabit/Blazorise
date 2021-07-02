---
title: "Markdown extension"
permalink: /docs/extensions/markdown/
excerpt: "Learn how to use Markdown components."
toc: true
toc_label: "Guide"
---

## Overview

The `Markdown` component allows you to edit markdown strings. The Blazorise Markdown is based on the [Easy MarkDown Editor](https://easy-markdown-editor.tk/) JavaScript library.

## Installation

Markdown component is created as an extension for Blazorise so before you continue you need to first get it from NuGet.

### NuGet

Install Markdown extension from NuGet.

```
Install-Package Blazorise.Markdown
```

### Imports

In your main _Imports.razor_ add:

```cs
@using Blazorise.Markdown
```

### Static Files

Include CSS and JS links into your `index.html` or `_Host.cshtml` file, depending if you’re using a Blazor WebAssembly or Blazor Server side project.

```html
<link href="https://unpkg.com/easymde/dist/easymde.min.css" rel="stylesheet">
<script src="https://unpkg.com/easymde/dist/easymde.min.js"></script>
<script src="https://cdn.jsdelivr.net/highlight.js/latest/highlight.min.js"></script>

<script src="_content/Blazorise.Markdown/blazorise.markdown.js"></script>
```

## Usage

```html
<Markdown Value="@markdownValue" ValueChanged="@OnMarkdownValueChanged" />
```

```cs
@code{
    string markdownValue = "# EasyMDE \n Go ahead, play around with the editor! Be sure to check out **bold**, *italic*, [links](https://google.com) and all the other features. You can type the Markdown syntax, use the toolbar, or use shortcuts like `ctrl-b` or `cmd-b`.";

    string markdownHtml;

    protected override void OnInitialized()
    {
        markdownHtml = Markdig.Markdown.ToHtml( markdownValue ?? string.Empty );

        base.OnInitialized();
    }

    Task OnMarkdownValueChanged( string value )
    {
        markdownValue = value;

        markdownHtml = Markdig.Markdown.ToHtml( markdownValue ?? string.Empty );

        return Task.CompletedTask;
    }
}
```

For transforming markdown value into HTML we used an excellent [Markdig](https://github.com/xoofx/markdig) library.
{: .notice--info}

## Attributes

| Name                   | Type               | Default    | Description                                                                    |
|------------------------|--------------------|------------|--------------------------------------------------------------------------------|
| Value                  | string             | `null`     | Gets or sets the markdown value.                                               |
| ValueChanged           | event              |            | An event that occurs after the markdown value has changed.                     |