---
title: "Utilities"
permalink: /docs/helpers/utilities/
excerpt: "Utilities"
toc: true
toc_label: "Helpers"
---

### Spacing

To define spacing between components you have an option to use a fluent-builder patern. This way you can combine multiple spacings at once.

The following example will set the margins for mobile(xs) and desktop(md) breakpoints:

```cs
<CardBody Margin="Margin.Is2.OnMobile.Is5.OnDesktop">
```

**Note:** The same rules can also be applied for paddings.
{: .notice--info}

### Column Size

Similar to the spacing builder you can also define column sizes using the same pattern.

```cs
<Column ColumnSize="ColumnSize.Is4.OnTablet.Is3.OnWidescreen.Is12.OnMobile">
```

### Breakpoints by frameworks

| Blazorise     | Bootstrap     | Material      | Bulma         |
| ------------- |:-------------:|:-------------:| -------------:|
| Mobile        | xs            | xs            | mobile        |
| Tablet        | sm            | sm            | tablet        |
| Desktop       | md            | md            | desktop       |
| Widescreen    | lg            | lg            | widescreen    |
| FullHD        | xl            | xl            | fullhd        |