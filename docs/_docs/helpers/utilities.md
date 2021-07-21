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

## Flex

Quickly manage the layout, alignment, and sizing of grid columns, navigation, components, and more with a full suite of responsive flexbox utilities. For more complex implementations, custom CSS may be necessary.

```html
<Div Flex="Flex.JustifyContent.Start">
    ...
</Div>

<Div Flex="Flex.AlignItems.Center">
    ...
</Div>
```

## Border

Use border utilities to quickly style the border and border-radius of an element. Great for images, buttons, or any other element.

```html
<Span Border="Border.Rounded">Rounded</Span>

<Span Border="Border.Is1">Border on all sides</Span>

<Span Border="Border.Primary">Borders with primary color</Span>
```

**Note:** Please note that if your element doesn't have any styles you will not be able to see any changes once you apply the `Borders`. You still need to add your own CSS rules like `background-color` so that visually you can see the applied borders on an element.
{: .notice--info}

## Overflow

Use overflow shorthand utilities for quickly configuring how content overflows an element.

```html
<Div Overflow="Overflow.Auto">...</Div>
<Div Overflow="Overflow.Hidden">...</Div>
<Div Overflow="Overflow.Visible">...</Div>
<Div Overflow="Overflow.Scroll">...</Div>
```

## Breakpoints by frameworks

| Blazorise     | Bootstrap     | Material      | Bulma         |
| ------------- |:-------------:|:-------------:| -------------:|
| Mobile        | xs            | xs            | mobile        |
| Tablet        | sm            | sm            | tablet        |
| Desktop       | md            | md            | desktop       |
| Widescreen    | lg            | lg            | widescreen    |
| FullHD        | xl            | xl            | fullhd        |