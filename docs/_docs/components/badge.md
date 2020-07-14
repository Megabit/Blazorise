---
title: "Badge component"
permalink: /docs/components/badge/
excerpt: "Learn how to use badge components."
toc: true
toc_label: "Guide"
---

## Overview

Small and adaptive tag for adding context to just about any content.

## Examples

### Basic usage

Simply set the Color attribute and you're good to go.

```html
<Badge Color="Color.Primary">Primary</Badge>
<Badge Color="Color.Secondary">Secondary</Badge>
```

<iframe class="frame" src="/examples/elements/badge/" frameborder="0" scrolling="no" style="width:100%;height:35px;"></iframe>

### With close button

To enable close button you only need to define the `CloseClicked` event. Blazorise will automatically pickup and show close button.

```html
<Badge Color="Color.Primary" CloseClicked="@(()=>Console.WriteLine("closed"))">Primary</Badge>
```

## Attributes

| Name         | Type                                                         | Default          | Description                                                                                 |
|--------------|--------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Pill         | boolean                                                      | false            | Makes badges more rounded.                                                                  |
| Link         | string                                                       | null             | Create a badge link and provide actionable badges with hover and focus states.              |
| Color        | [Colors]({{ "/docs/helpers/colors/#color" | relative_url }}) | `None`           | Component visual or contextual style variants.                                              |
| CloseClicked | EventCallback                                                |                  | Occurs on close button click.                                                               |