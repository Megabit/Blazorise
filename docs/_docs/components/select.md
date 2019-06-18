---
title: "Select component"
permalink: /docs/components/select/
excerpt: "Learn how to use form select component."
toc: true
toc_label: "Guide"
---

**Update:** As of **v0.6-preview6** `SelectEdit` and `SelectItem` are generic components. 
{: .notice--warning}

Use SelectEdit to combine many choices into one menu.

- `<SelectEdit>`
  - `<SelectItem>`

SelectEdit and SelectItem are generic components and they support all of the basic value types line int, string, enum, etc. Nullable types are also supported. Since they are generic component they also come with some special rules that must be followed:

- Value type must be known. When using member variable on `bind-*` or `SelectedValue` attributes, the value type will be recognized automatically. Otherwise you must use TValue to define it eg (TValue="int").
- Value type must be the **same** in both `SelectEdit` and `SelectItem`.
- String values must be defined with special syntax eg. "**@("hello")**", see [#7785](https://github.com/aspnet/AspNetCore/issues/7785).

## Basic Select

```html
<SelectEdit TValue="int">
    <SelectItem Value="1">1</SelectItem>
    <SelectItem Value="2">2</SelectItem>
    <SelectItem Value="3">3</SelectItem>
    <SelectItem Value="4">4</SelectItem>
</SelectEdit>
```

<iframe src="/examples/forms/select-basic/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Multiple Select

Add the `IsMultiple` attribute to allow more than one option to be selected.

```html
<SelectEdit TValue="int" IsMultiple="true">
    <SelectItem Value="1">1</SelectItem>
    <SelectItem Value="2">2</SelectItem>
    <SelectItem Value="3">3</SelectItem>
    <SelectItem Value="4">4</SelectItem>
</SelectEdit>
```

<iframe src="/examples/forms/select-multiple/" frameborder="0" scrolling="no" style="width:100%;height:112px;"></iframe>

## Usage

The process is basically the same for the single and for multiple select. The only diference is that `SelectedValue` attribute is used for single select mode, and `SelectedValues` attribute is used for multi-selection. Keep in mind that `IsMultiple` must be set to **true** for multi-selection to work properly.

**Note:** The `Value` attribute is required on the `SelectItem`. Otherwise the `SelectEdit` will not behave as expected.
{: .notice--info}

### With bind attribute

By using `bind-*` attribute the selected item value will be automatically assigned to the member variable.

```html
<SelectEdit bind-SelectedValue="@selectedValue">
    <SelectItem Value="1">1</SelectItem>
    <SelectItem Value="2">2</SelectItem>
    <SelectItem Value="3">3</SelectItem>
    <SelectItem Value="4">4</SelectItem>
</SelectEdit>

@code{
    int selectedValue;
}
```

### With event

When using the event `SelectedValueChanged`, you also must define the `SelectedValue` attribute.

```html
<SelectEdit SelectedValue="@selectedValue" SelectedValueChanged="@OnSelectedValueChanged">
    <SelectItem Value="1">1</SelectItem>
    <SelectItem Value="2">2</SelectItem>
    <SelectItem Value="3">3</SelectItem>
    <SelectItem Value="4">4</SelectItem>
</SelectEdit>

@code{
    int selectedValue;

    void OnSelectedValueChanged( int value )
    {
        selectedValue = value;
        Console.WriteLine( selectedValue );
    }
}
```

## Props

| Name                  | Type      | Default | Description                                                                                  |
|-----------------------|-----------|---------|----------------------------------------------------------------------------------------------|
| IsMultiple            | boolean   | false   | Specifies that multiple items can be selected.                                               |
| SelectedValue         | generic   |         | Selected item value when in single edit mode.                                                |
| selected item value.  | generic[] |         | Selected item value when in multi edit mode.                                                 |
| SelectedValueChanged  | action    |         | Occurs when the selected item value has changed.                                             |
| SelectedValuesChanged | action    |         | Occurs when the selected items value has changed (only when IsMultiple==true).               |