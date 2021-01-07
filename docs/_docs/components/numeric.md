---
title: "Numeric component"
permalink: /docs/components/numeric/
excerpt: "Learn how to use form numeric component."
toc: true
toc_label: "Guide"
---

## Basics

Use NumericEdit to have a field for any kind if numeric values. All basic types are supported, including nullable types(`int`, `long`, `float`, `double`, `decimal`, etc.).

```html
<NumericEdit Value="123" />
```

<iframe src="/examples/forms/numeric-basic/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Rules

Since NumericEdit is a generic component you will have to specify the exact data type for the value. Most of the time it will be recognized automatically when you set the `Value` attribute, but if not you will just use the `TValue` attribute and define the type manually eg.

```html
<NumericEdit TValue="int?" />
```

## Attributes

NumericEdit is just a specialized version of `TextEdit` component so all of the rules and styles are still working all the same. See [TextEdit]({{ "/docs/components/text/" | relative_url }}) to find the list of supported attributes.

| Name              | Type                                                         | Default | Description                                                                                          |
|-------------------|--------------------------------------------------------------|---------|------------------------------------------------------------------------------------------------------|
| TValue            | decimal?                                                     | null    | Generic type parameter used for the value attribute.                                                 |
| Value             | string                                                       |         | Gets or sets the value inside the input field.                                                       |
| ValueChanged      | event                                                        |         | Occurs after the value has changed.                                                                  |
| Step              | decimal?                                                     | null    | Specifies the interval between valid values.                                                         |
| Decimals          | int                                                          | 2       | Maximum number of decimal places after the decimal separator.                                        |
| DecimalsSeparator | string                                                       | "."     | String to use as the decimal separator in numeric values.                                            |
| Culture           | string                                                       | null    | Helps define the language of an element.                                                             |
| Min               | TValue                                                       | default | The minimum value to accept for this input.                                                          |
| Max               | TValue                                                       | default | The maximum value to accept for this input.                                                          |
| Autofocus         | `bool`                                                       |  false  | Set's the focus to the component after the rendering is done.                                        |