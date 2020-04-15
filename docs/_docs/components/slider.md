---
title: "Slider component"
permalink: /docs/components/slider/
excerpt: "Documentation and examples for slider component."
toc: true
toc_label: "Guide"
---

Slider allow users to make selections from a range of values.

## Example

```html
<Slider TValue="decimal" Value="25m" Max="100m" />
```

**Note:** Since Slider is a generic component you will have to specify the exact data type for the value. Most of the time it will be recognized automatically when you set the `Value` attribute, but if not you will just use the `TValue` attribute and define the type manually.
{: .notice--info}

<iframe src="/examples/forms/slider-basic/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Attributes

| Name              | Type      | Default | Description                                                                                          |
|-------------------|-----------|---------|------------------------------------------------------------------------------------------------------|
| Value             | string    |         | The value that the tick represents.                                                                  |
| ValueChanged      | event     |         | Occurs after the value has changed.                                                                  |
| Step              | TValue    | 1       | Specifies the interval between valid values.                                                         |
| Min               | TValue    | null    | Minimum value.                                                                                       |
| Max               | TValue    | null    | Maximum value.                                                                                       |