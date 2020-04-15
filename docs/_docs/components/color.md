---
title: "Color component"
permalink: /docs/components/color/
excerpt: "Documentation and examples for color component."
toc: true
toc_label: "Guide"
---

Native color picker allows you to select different colors.

## Basic Example

Click the red area to activate the color picker. This is the basic variation of it.

```html
<ColorEdit Color="#00000" />
```

<iframe src="/examples/forms/color/" frameborder="0" scrolling="no" style="width:100%;height:55px;"></iframe>

## Attributes

| Name           | Type                                                                       | Default      | Description                                                                                                                    |
|----------------|----------------------------------------------------------------------------|--------------|--------------------------------------------------------------------------------------------------------------------------------|
| Color          | string                                                                     | null         | Gets or sets the input color value in hex format.                                                                              |
| ColorChanged   | event                                                                      |              | Occurs when the color has changed.                                                                                             |