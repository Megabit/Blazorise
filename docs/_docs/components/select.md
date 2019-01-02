---
title: "Select"
permalink: /docs/components/select/
excerpt: "Learn how to use form select component."
toc: true
toc_label: "Guide"
---

## Basic Select

Use SelectEdit to combine many choices into one menu.

```html
<SelectEdit>
    <SelectItem>1</SelectItem>
    <SelectItem>2</SelectItem>
    <SelectItem>3</SelectItem>
    <SelectItem>4</SelectItem>
</SelectEdit>
```

<iframe src="/examples/forms/select/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Usage

**Note:** The `Value` attribute is required on the `SelectItem`. Otherwise the `SelectEdit` will not behave as expected.
{: .notice--info}

### With bind attribute

By using bind-* keyword the selected item value will be automatically assigned to the member variable.

```html
<SelectEdit bind-SelectedValue="@selectedValue">
    <SelectItem Value="1">1</SelectItem>
    <SelectItem Value="2">2</SelectItem>
    <SelectItem Value="3">3</SelectItem>
    <SelectItem Value="4">4</SelectItem>
</SelectEdit>

@functions{
    string selectedValue;
}
```

### With event

```html
<SelectEdit Text="@name" SelectedValue="@selectedValue" SelectedValueChanged="@OnSelectedValueChanged">
    <SelectItem Value="1">1</SelectItem>
    <SelectItem Value="2">2</SelectItem>
    <SelectItem Value="3">3</SelectItem>
    <SelectItem Value="4">4</SelectItem>
</SelectEdit>

@functions{
    string selectedValue;

    void OnSelectedValueChanged( string value )
    {
        selectedValue = value;
        Console.WriteLine( selectedValue );
    }
}
```

## Multiple Select

Add the `IsMultiple` attribute to allow more than one option to be selected.

```html
<SelectEdit IsMultiple="true">
    <SelectItem Value="1">1</SelectItem>
    <SelectItem Value="2">2</SelectItem>
    <SelectItem Value="3">3</SelectItem>
    <SelectItem Value="4">4</SelectItem>
</SelectEdit>
```

<iframe src="/examples/forms/select-multiple/" frameborder="0" scrolling="no" style="width:100%;height:112px;"></iframe>

Usage for multiple select is basically the same as for the ordinal select. The only diference is that `SelectedValue` attribute will/must be defined as a `CSV` value eg: "1;2;4"