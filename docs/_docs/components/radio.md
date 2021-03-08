---
title: "Radio component"
permalink: /docs/components/radio/
excerpt: "Learn how to use radio component."
toc: true
toc_label: "Guide"
---

Use `Radio` buttons when the user needs to see all available options.

## Structure

- `<RadioGroup>`
  - `<Radio>`

## Examples

### RadioGroup

`RadioGroup` is a helpful wrapper used to group `Radio` components that provides an easier API, and proper keyboard accessibility to the group.

```html
<RadioGroup TValue="string" Name="colors">
    <Radio TValue="string" Value="@("red")">Red</Radio>
    <Radio TValue="string" Value="@("green")">Green</Radio>
    <Radio TValue="string" Value="@("blue")">Blue</Radio>
</RadioGroup>
```

<iframe src="/examples/forms/radio-group/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Standalone Radio

`Radio` can also be used standalone, without the `RadioGroup` wrapper.

```html
<Radio TValue="string" Group="colors" Value="@("red")">Red</Radio>
<Radio TValue="string" Group="colors" Value="@("green")">Green</Radio>
<Radio TValue="string" Group="colors" Value="@("blue")">Blue</Radio>
```

<iframe src="/examples/forms/radio-standalone/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### RadioGroup Buttons

By setting the `Buttons` flag, radios will be grouped together and will appear as buttons.

```html
<RadioGroup TValue="string" Name="colors" Buttons="true">
    <Radio TValue="string" Value="@("red")">Red</Radio>
    <Radio TValue="string" Value="@("green")">Green</Radio>
    <Radio TValue="string" Value="@("blue")">Blue</Radio>
</RadioGroup>
```

<iframe src="/examples/forms/radio-group-buttons/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Usage

### With bind attribute

```html
<RadioGroup TValue="string" Name="colors" @bind-CheckedValue="@checkedValue">
    <Radio TValue="string" Value="@("red")">Red</Radio>
    <Radio TValue="string" Value="@("green")">Green</Radio>
    <Radio TValue="string" Value="@("blue")">Blue</Radio>
</RadioGroup>

@code{
    string checkedValue = "green";
}
```

### With event

```html
<RadioGroup TValue="string" Name="colors" CheckedValue="@checkedValue" CheckedValueChanged="@OnCheckedValueChanged">
    <Radio TValue="string" Value="@("red")">Red</Radio>
    <Radio TValue="string" Value="@("green")">Green</Radio>
    <Radio TValue="string" Value="@("blue")">Blue</Radio>
</RadioGroup>

@code{
    string checkedValue = "green";

    void OnCheckedValueChanged( string value )
    {
        checkedValue = value;
    }
}
```

## Attributes

### RadioGroup

| Name                    | Type                                                                       | Default      | Description                                                                           |
|-------------------------|----------------------------------------------------------------------------|--------------|---------------------------------------------------------------------------------------|
| TValue                  | generic type                                                               |              | `CheckedValue` data type.                                                             |
| CheckedValue            | boolean                                                                    | false        | Gets or sets the checked value.                                                       |
| CheckedValueChanged     | event                                                                      |              | Occurs when the checked value is changed.                                             |
| Name                    | string                                                                     | null         | Sets the radio group name.                                                            |
| Orientation             | [Orientation]({{ "/docs/helpers/sizes/#orientation" | relative_url }})     | `Horizontal` | Defines the orientation of the radio elements.                                        |
| Color                   | [Color]({{ "/docs/helpers/colors/#color" | relative_url }})                | `Secondary`  | Defines the color or radio buttons(only when `Buttons` is true).                      |

### Radio

| Name                    | Type                                                                       | Default      | Description                                                                           |
|-------------------------|----------------------------------------------------------------------------|--------------|---------------------------------------------------------------------------------------|
| TValue                  | generic type                                                               |              | Data type of `Checked` value. Support types are `bool` and `bool?`.                   |
| Checked                 | boolean                                                                    | false        | Gets or sets the checked flag.                                                        |
| CheckedChanged          | event                                                                      |              | Occurs when the check state is changed.                                               |
| Group                   | string                                                                     | null         | Sets the radio group name.                                                            |
| Inline                  | boolean                                                                    | false        | Group radios on the same horizontal row.                                              |
| Cursor                  | [Cursor]({{ "/docs/helpers/enums/#cursor" | relative_url }})               | `Default`    | Defines the mouse cursor based on the behavior by the current CSS framework.          |