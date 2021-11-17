---
title: "Divider"
permalink: /docs/components/divider/
excerpt: "Learn how to use divider component."
toc: true
toc_label: "Guide"
---

Dividers are used to create horizontal line which works as separator.

## Examples

### Basic

Default line style is solid.

```html
<Divider />
```

<iframe src="/examples/elements/divider/" frameborder="0" scrolling="no" style="width:100%;height:35px;"></iframe>

### Dashed

```html
<Divider DividerType="DividerType.Dashed" />
```

<iframe src="/examples/elements/divider-dashed/" frameborder="0" scrolling="no" style="width:100%;height:35px;"></iframe>

### Dotted

```html
<Divider DividerType="DividerType.Dotted" />
```

<iframe src="/examples/elements/divider-dotted/" frameborder="0" scrolling="no" style="width:100%;height:35px;"></iframe>

### Text Content

```html
<Divider DividerType="DividerType.TextContent" Text="Hello Blazorise" />
```

<iframe src="/examples/elements/divider-text/" frameborder="0" scrolling="no" style="width:100%;height:35px;"></iframe>

## Attributes

| Name        | Type                                                                   | Default   | Description                                        |
|-------------|------------------------------------------------------------------------|-----------|----------------------------------------------------|
| DividerType | [DividerType]({{ "/docs/helpers/enums/#dividertype" | relative_url }}) | `Solid`   | Specifies horizontal line style variants.          |
| Text        | string                                                                 | null      | Label that will appear between the solid lines.    |
