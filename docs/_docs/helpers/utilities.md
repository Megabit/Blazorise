---
title: "Utilities"
permalink: /docs/helpers/utilities/
excerpt: "Utilities"
toc: true
---

### Spacing

To define spacing between components you have an option to use a fluent-builder patern. This way you can combine multiple spacings at once. For example to set a margin you write:

```cs
<CardBody Margin="Margin.Is2.OnMobile.Is5.OnDesktop">
```

The same rules are applied for paddings.

### Column Size

Grid column sizes can also be defined with fluent-builder patern. 

```cs
<Column ColumnSize="ColumnSize.Is4.OnTablet.Is3.OnWidescreen.Is12.OnMobile">
```