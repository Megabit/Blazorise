---
title: "Tooltip component"
permalink: /docs/components/tooltip/
excerpt: "Learn how to use tooltip component."
toc: true
toc_label: "Guide"
---

## Overview

Display a tooltip attached to any kind of element with different positioning.

### Basic

```html
<Tooltip Text="Hello tooltip">
    <Button Color="Color.Primary">Hover me</Button>
</Tooltip>
```

<iframe class="frame" src="/examples/tooltip/basic/" frameborder="0" scrolling="no" style="width:100%;height:130px;"></iframe>

### Positions

You can use one of the following modifiers to change positions of the tooltip:

- `Top`
- `Bottom`
- `Left`
- `Right`

```html
<Tooltip Text="Hello tooltip" Placement="Placement.Top">
    <Button Color="Color.Primary">Top tooltip</Button>
</Tooltip>
<Tooltip Text="Hello tooltip" Placement="Placement.Right">
    <Button Color="Color.Primary">Right tooltip</Button>
</Tooltip>
<Tooltip Text="Hello tooltip" Placement="Placement.Left">
    <Button Color="Color.Primary">Left tooltip</Button>
</Tooltip>
<Tooltip Text="Hello tooltip" Placement="Placement.Bottom">
    <Button Color="Color.Primary">Bottom tooltip</Button>
</Tooltip>
```

<iframe class="frame" src="/examples/tooltip/positions/" frameborder="0" scrolling="no" style="width:100%;height:130px;"></iframe>

## Attributes

| Name              | Type                                                               | Default          | Description                                                               |
|-------------------|--------------------------------------------------------------------|------------------|---------------------------------------------------------------------------|
| Text              | string                                                             | null             | Content displayed in the tooltip.                                         |
| Placement         | [Placement]({{ "/docs/helpers/enums/#placement" | relative_url }}) | `Top`            | Position of the tooltip relative to it's component.                       |
| Multiline         | bool                                                               | false            | Force the multiline display.                                              |
| AlwaysActive      | bool                                                               | false            | Always show tooltip, instead of just when hovering over the element.      |
| Inline            | bool                                                               | false            | Force inline block instead of trying to detect the element block.         |
| Fade              | bool                                                               | false            | Controls the fade effect.                                                 |