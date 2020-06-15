---
title: "Utilities"
permalink: /docs/helpers/utilities/
excerpt: "Blazorise have it's own set of helpers classes and tools that you can use to organize you application without writing CSS class-names."
toc: true
toc_label: "Helpers"
---

## Spacing

To define spacing between components you have an option to use a fluent-builder pattern. This way you can combine multiple spacings at once.

The following example will set the margins for mobile(xs) and desktop(md) breakpoints:

```html
<CardBody Margin="Margin.Is2.OnMobile.Is5.OnDesktop">
```

**Note:** The same rules can also be applied for paddings.
{: .notice--info}

## Display

Quickly and responsively toggle the display value of components and more with display utilities.

```html
<Paragraph Display="Display.None.Block.OnFullHD">
    hide on screens smaller than lg
</Paragraph>
```

## ColumnSize

Similar to the spacing builder you can also define column sizes using the same pattern.

```html
<Column ColumnSize="ColumnSize.Is4.OnTablet.Is3.OnWidescreen.Is12.OnMobile">
```

## Breakpoints by frameworks

| Blazorise     | Bootstrap     | Material      | Bulma         |
| ------------- |:-------------:|:-------------:| -------------:|
| Mobile        | xs            | xs            | mobile        |
| Tablet        | sm            | sm            | tablet        |
| Desktop       | md            | md            | desktop       |
| Widescreen    | lg            | lg            | widescreen    |
| FullHD        | xl            | xl            | fullhd        |