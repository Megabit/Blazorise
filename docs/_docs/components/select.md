---
title: "Select component"
permalink: /docs/components/select/
excerpt: "Learn how to use form select component."
toc: true
toc_label: "Guide"
---

Use Select to combine many choices into one menu.

- `<Select>`
  - `<SelectItem>`
  - `<SelectGroup>` Optional tag used to group select items
    - `<SelectItem>`

`Select` and `SelectItem` are generic components and they support all of the basic value types line int, string, enum, etc. Nullable types are also supported. Since they are generic component they also come with some special rules that must be followed:

- Value type must be known. When using member variable on `bind-*` or `SelectedValue` attributes, the value type will be recognized automatically. Otherwise you must use TValue to define it eg (TValue="int").
- Value type must be the **same** in both `Select` and `SelectItem`.
- String values must be defined with special syntax eg. "**@("hello")**", see [#7785](https://github.com/aspnet/AspNetCore/issues/7785).

## Basic Select

```html
<Select TValue="int">
    <SelectItem Value="1">1</SelectItem>
    <SelectItem Value="2">2</SelectItem>
    <SelectItem Value="3">3</SelectItem>
    <SelectItem Value="4">4</SelectItem>
</Select>
```

<iframe src="/examples/forms/select-basic/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Multiple Select

Add the `Multiple` attribute to allow more than one option to be selected.

```html
<Select TValue="int" Multiple="true">
    <SelectItem Value="1">1</SelectItem>
    <SelectItem Value="2">2</SelectItem>
    <SelectItem Value="3">3</SelectItem>
    <SelectItem Value="4">4</SelectItem>
</Select>
```

<iframe src="/examples/forms/select-multiple/" frameborder="0" scrolling="no" style="width:100%;height:112px;"></iframe>

## Select Groups

You can also group items into categories for better user experience.

```html
<Select TValue="int">
    <SelectGroup Label="Group 1">
        <SelectItem Value="1">1</SelectItem>
        <SelectItem Value="2">2</SelectItem>
    </SelectGroup>
    <SelectGroup Label="Group 2">
        <SelectItem Value="3">3</SelectItem>
        <SelectItem Value="4">4</SelectItem>
    </SelectGroup>
</Select>
```

<iframe src="/examples/forms/select-group/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Usage

The process is basically the same for the single and for multiple select. The only difference is that `SelectedValue` attribute is used for single select mode, and `SelectedValues` attribute is used for multi-selection. Keep in mind that `Multiple` must be set to **true** for multi-selection to work properly.

**Note:** The `Value` attribute is required on the `SelectItem`. Otherwise the `Select` will not behave as expected.
{: .notice--info}

### With bind attribute

By using `bind-*` attribute the selected item value will be automatically assigned to the member variable.

```html
<Select @bind-SelectedValue="@selectedValue">
    <SelectItem Value="1">1</SelectItem>
    <SelectItem Value="2">2</SelectItem>
    <SelectItem Value="3">3</SelectItem>
    <SelectItem Value="4">4</SelectItem>
</Select>

@code{
    int selectedValue;
}
```

### With event

When using the event `SelectedValueChanged`, you also must define the `SelectedValue` attribute.

```html
<Select TValue="int" SelectedValue="@selectedValue" SelectedValueChanged="@OnSelectedValueChanged">
    <SelectItem Value="1">1</SelectItem>
    <SelectItem Value="2">2</SelectItem>
    <SelectItem Value="3">3</SelectItem>
    <SelectItem Value="4">4</SelectItem>
</Select>

@code{
    int selectedValue;

    void OnSelectedValueChanged( int value )
    {
        selectedValue = value;
        Console.WriteLine( selectedValue );
    }
}
```

## Attributes

### Select

| Name                  | Type      | Default | Description                                                                                  |
|-----------------------|-----------|---------|----------------------------------------------------------------------------------------------|
| Multiple              | boolean   | false   | Specifies that multiple items can be selected.                                               |
| SelectedValue         | generic   |         | Selected item value when in single edit mode.                                                |
| SelectedValues        | generic[] |         | Selected item value when in multi edit mode.                                                 |
| SelectedValueChanged  | action    |         | Occurs when the selected item value has changed.                                             |
| SelectedValuesChanged | action    |         | Occurs when the selected items value has changed (only when Multiple==true).                 |
| MaxVisibleItems       | int?      | null    | Specifies how many options should be shown at once..                                         |

### SelectItem

| Name                  | Type      | Default | Description                                                                                  |
|-----------------------|-----------|---------|----------------------------------------------------------------------------------------------|
| Value                 | generic   |         | Gets or sets the item value.                                                                 |
| Disabled              | boolean   | false   | Disable the item from mouse click.                                                           |

### SelectGroup

| Name                  | Type      | Default | Description                                                                                  |
|-----------------------|-----------|---------|----------------------------------------------------------------------------------------------|
| Label                 | string    |         | Gets or sets the group label.                                                                |