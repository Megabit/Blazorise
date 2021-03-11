---
title: "RichTextEdit extension"
permalink: /docs/extensions/richtextedit/
excerpt: "Learn how to use RichTextEdit components."
toc: true
toc_label: "Guide"
---

## Overview

The `RichTextEdit` component allows you to add and use a 'WYSIWYG' rich text editor. The Blazorise RichTextEdit is based on the [QuillJS](https://quilljs.com/) JavaScript library.

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

### Static Files

Include CSS link into your index.html or _Host.cshtml file, depending if you’re using a Blazor WebAssembly or Blazor Server side project.

```html
<script src="_content/Blazorise.RichTextEdit/blazorise.richtextedit.js"></script>
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
| UseShowTheme          | bool   | true    | Load the QuillJS snow theme related resources.           |
| UseBubbleTheme        | bool   | false   | Load the QuillJS bubble theme related resources.         |
| QuillJSVersion        | string | 1.3.7   | The QuillJS version to load.                             |
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
                <RichTextEditToolbarSelectItem Selected="true" />
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

The `RichTextEdit` toolbar can be completely customized. QuillJS defines a number of default actions that can be used through the `RichTextEditToolbarButton` and `RichTextEditToolbarSelect` components

See [QuillJS Toolbar module](https://quilljs.com/docs/modules/toolbar/) for more information.

### QuillJS Configuration

The `RichTextEdit` has the option to inject additional QuillJS configuration logic or load additional [modules](https://github.com/quilljs/awesome-quill). Use the `ConfigureQuillJSMethod` property to indicate which javascript method needs to be called during initialization. 

If you for example want to change the way how links are sanitized you can use the following logic. Default all user typed url's are relative to your pages base url. So when a user types `google.com` this will result in something like `https://baseurl/google.com`, but if you would probably like `https://google.com` then use the following configuration routine.

```html
<RichTextEdit ConfigureQuillJsMethod="myComponent.configureQuillJs" />
```

```javascript
window.myComponent = {
    configureQuillJs: () => {
        var link = Quill.import("formats/link");

        link.sanitize = url => {
            let newUrl = window.decodeURIComponent(url);
            newUrl = newUrl.trim().replace(/\s/g, "");

            if (/^(:\/\/)/.test(newUrl)) {
                return `http${newUrl}`;
            }

            if (!/^(f|ht)tps?:\/\//i.test(newUrl)) {
                return `http://${newUrl}`;
            }

            return newUrl;
        }
    }
}
```

## Attributes

### RichTextEdit

| Name                   | Type               | Default    | Description                                           |
|------------------------|--------------------|------------|-------------------------------------------------------|
| Toolbar                | markup             |            | The custom toolbar definition.                        |
| Editor                 | markup             |            | The editor content.                                   |
| ReadOnly               | bool               | false      | Editor read-only flag.                                |
| Theme                  | RichTextEditTheme  | Snow       | The editor theme.                                     |
| PlaceHolder            | string             |            | Placeholder text for empty editor.                    |
| ToolbarPosition        | Placement          | Top        | Toolbar position (top or bottom).                     |
| SubmitOnEnter          | bool               | false      | Call _EnterPressed_ event when pressing ENTER key.    |
| ContentChanged         | event              |            | Occurs when the content changes.                      |
| EnterPressed           | event              |            | Occurs when ENTER key is pressed and _SubmitOnEnter_. |
| ConfigureQuillJSMethod | string             |            | The JavaScript method to call to configure additional QuillJS modules and or add custom bindings. |

### RichTextEditToolbarGroup

| Name                 | Type               | Default    | Description                                           |
|----------------------|--------------------|------------|-------------------------------------------------------|
| Float                | Float              |            | The float position on the toolbar.                    |
| ChildContent         | markup             |            | The group content.                                    |

### RichTextEditToolbarButton

| Name                 | Type               | Default    | Description                                           |
|----------------------|--------------------|------------|-------------------------------------------------------|
| Action               | RichTextEditAction |            | The QuillJS action associated with the select.        |
| Value                | RichTextEditAction |            | The QuillJS action selected value.                    |
| ChildContent         | markup             |            | The custom markup/text to display.                    |

### RichTextEditToolbarSelect

| Name                 | Type               | Default    | Description                                           |
|----------------------|--------------------|------------|-------------------------------------------------------|
| Action               | RichTextEditAction |            | The QuillJS action associated with the select.        |
| ChildContent         | markup             |            | The `RichTextEditToolbarSelectItem` items.            |

### RichTextEditToolbarSelectItem

| Name                 | Type               | Default    | Description                                           |
|----------------------|--------------------|------------|-------------------------------------------------------|
| Value                | string             |            | The QuillJS action selected value.                    |
| Selected             | bool               | false      | Is the select item selected.                          |
| ChildContent         | markup             |            | The custom markup/text to display.                    |
