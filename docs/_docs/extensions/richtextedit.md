---
title: "RichTextEdit extension"
permalink: /docs/extensions/richtextedit/
excerpt: "Learn how to use RichTextEdit components."
toc: true
toc_label: "Guide"
---

## Overview

The `RichTextEdit` component allows you to add a 'WYSIWYG' rich text editor. The Blazorise RichTextEdit is based on the [QuillJS](https://quilljs.com/) javascript library.

### Structure

- `<RichTextEdit>` the root editor component
  - `Editor` _(Optional)_ the editor part with displayed html content
  - `Toolbar` _(Optional)_ the editor toolbar definition
    - `<RichTextEditToolbarGroup>` toolbar group
        - `<RichTextEditToolbarButton>` toolbar button
        - `<RichTextEditToolbarSelect>` toolbar selection dropdown
            - `<RichTextEditToolbarSelectItem>` toolbar selection item
        - _any custom button or component_

## Installation

RichTextEdit component is created as an extension for Blazorise so before you continue you need to first get it from NuGet.

### NuGet

Install RichTextEdit extension from NuGet.

```
Install-Package Blazorise.RichTextEdit
```

### Imports

In your main _Imports.razor_ add:

```cs
@using Blazorise.RichTextEdit
```

### Configuration

In your Blazor `StartUp` add the following statement

```cs
builder.Services
    .AddBlazoriseRichTextEdit( options => { ... } );
```
#### Configuration options

| Name                  | Type   | Default | Description                                              |
|-----------------------|--------|---------|----------------------------------------------------------|
| UseShowTheme          | bool   | true    | Load the QuillJs snow theme related resources.           |
| UseBubbleTheme        | bool   | false   | Load the QuillJs bubble theme related resources.         |
| QuillJsVersion        | string | 1.3.7   | The QuillJs version to load.                             |
| DynamicLoadReferences | bool   | true    | Load the RichTextEdit scripts and stylesheets on demand. |

## Usage

### Markup

```html
<RichTextEdit @ref="richTextEditRef"
              Theme="RichTextEditTheme.Snow"
              ContentChanged="@OnContentChanged"
              PlaceHolder="Type your post here..."
              ReadOnly="@readOnly"
              SubmitOnEnter="false"
              EnterPressed="@OnSave"
              ToolbarPosition="Placement.Bottom">
    <Editor>My example content</Editor>
    <Toolbar>
        <RichTextEditToolbarGroup>
            <RichTextEditToolbarButton Action="RichTextEditAction.Bold" />
            <RichTextEditToolbarButton Action="RichTextEditAction.Italic" />
            <RichTextEditToolbarSelect Action="RichTextEditAction.Size">
                <RichTextEditToolbarSelectItem Value="small" />
                <RichTextEditToolbarSelectItem IsSelected="true" />
                <RichTextEditToolbarSelectItem Value="large" />
                <RichTextEditToolbarSelectItem Value="huge">Very Big</RichTextEditToolbarSelectItem>
            </RichTextEditToolbarSelect>
             <RichTextEditToolbarButton Action="RichTextEditAction.List" Value="ordered" />
            <RichTextEditToolbarButton Action="RichTextEditAction.List" Value="bullet" />
        </RichTextEditToolbarGroup>
        <!-- Custom toolbar content -->
        <RichTextEditToolbarGroup Float="Float.Right">
            <Button onClick="window.open('https://www.quilljs.com/','quilljs')"><Icon Name="IconName.InfoCircle" /></Button>
            <Button Clicked="@OnSave"><Icon Name="IconName.Save" /></Button>
        </RichTextEditToolbarGroup>
    </Toolbar>
</RichTextEdit>
```

### Data binding

```cs
@code{
    private RichTextEdit richTextEditRef;
    private bool readOnly;
    private string contentAsHtml;
    private string contentAsDeltaJson;
    private string contentAsText;
    private string savedContent;

    public async Task OnContentChanged()
    {
        contentAsHtml = await richTextEditRef.GetHtmlAsync();
        contentAsDeltaJson = await richTextEditRef.GetDeltaAsync();
        contentAsText = await richTextEditRef.GetTextAsync();
    }

    public async Task OnSave()
    {
        savedContent = await richTextEditRef.GetHtmlAsync();
        await richTextEditRef.ClearAsync();
    }
}
```

## Editor customization

### Theming

The `RichTextEdit` comes default with 2 themes `Snow` and `Bubble`. The `Snow` theme is a simple flat toolbar theme and the `Bubble` theme is a tooltip based theme where the toolbar will be displayed in the tooltip.

See [QuillJS Themes](https://quilljs.com/docs/themes/) for more information.

### Toolbar

The `RichTextEdit` toolbar can be completely customized. QuillJs defines a number of default actions that can be used through the `RichTextEditToolbarButton` and `RichTextEditToolbarSelect` components

See [QuillJS Toolbar module](https://quilljs.com/docs/modules/toolbar/) for more information.


## Attributes

### RichTextEdit

| Name                   | Type               | Default    | Description                                           |
|------------------------|--------------------|------------|-------------------------------------------------------|
| Toolbar                | markup             |            | The custom toolbar definition.                        |
| Editor                 | markup             |            | The editor content.                                   |
| ReadOnly               | bool               | false      | Editor readonly flag.                                 |
| Theme                  | RichTextEditTheme  | Snow       | The editor theme.                                     |
| PlaceHolder            | string             |            | Placeholder text for empty editor.                    |
| ToolbarPosition        | Placement          | Top        | Toolbar position (top or bottom).                     |
| SubmitOnEnter          | bool               | false      | Call _EnterPressed_ event when pressing ENTER key.    |
| ContentChanged         | event              |            | Occurs when the content changes.                      |
| EnterPressed           | event              |            | Occurs when ENTER key is pressed and _SubmitOnEnter_. |
| ConfigureQuillJsMethod | string             |            | The javascript method to call to configure additional QuillJs modules and or add custom bindings. |

### RichTextEditToolbarGroup

| Name                 | Type               | Default    | Description                                           |
|----------------------|--------------------|------------|-------------------------------------------------------|
| Float                | Float              |            | The float position on the toolbar.                    |
| ChildContent         | markup             |            | The group content.                                    |

### RichTextEditToolbarButton

| Name                 | Type               | Default    | Description                                           |
|----------------------|--------------------|------------|-------------------------------------------------------|
| Action               | RichTextEditAction |            | The QuillJs action associated with the select.        |
| Value                | RichTextEditAction |            | The QuillJS action selected value.                    |
| ChildContent         | markup             |            | The custom markup/text to display.                    |

### RichTextEditToolbarSelect

| Name                 | Type               | Default    | Description                                           |
|----------------------|--------------------|------------|-------------------------------------------------------|
| Action               | RichTextEditAction |            | The QuillJs action associated with the select.        |
| ChildContent         | markup             |            | The `RichTextEditToolbarSelectItem` items.            |

### RichTextEditToolbarSelectItem

| Name                 | Type               | Default    | Description                                           |
|----------------------|--------------------|------------|-------------------------------------------------------|
| Value                | string             |            | The QuillJS action selected value.                    |
| IsSelected           | bool               | false      | Is the select item selected.                          |
| ChildContent         | markup             |            | The custom markup/text to display.                    |
