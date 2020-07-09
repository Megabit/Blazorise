---
title: "Memo component"
permalink: /docs/components/memo/
excerpt: "Learn how to use memo component."
toc: true
toc_label: "Guide"
---

## Basic Memo

MemoEdit is used to create multiline text input (text-area).

```html
<MemoEdit Rows="5" />
```

<iframe src="/examples/forms/memo/" frameborder="0" scrolling="no" style="width:100%;height:143px;"></iframe>

## Usage

### With bind attribute

By using `bind-*` attribute the text will be automatically assigned to the member variable.

```html
<MemoEdit @bind-Text="@description" />

@code{
    string description;
}
```

### With event

When using the event `TextChanged`, you also must define the `Text` value attribute.

```html
<MemoEdit Text="@description" TextChanged="@OnDescriptionChanged" />

@code{
    string description;

    void OnDescriptionChanged( string value )
    {
        description = value;
    }
}
```

## Settings

### Text Changed mode

By default the `TextChanged` event will be raised on every keypress. To override default behavior of `TextChanged` event and to disable the change on every keypress you must set the `ChangeTextOnKeyPress` to `false` on application start. After setting it to `false` the event will be raised only after the input loses focus.

```cs
public void ConfigureServices( IServiceCollection services )
{
  services
    .AddBlazorise( options =>
    {
      options.ChangeTextOnKeyPress = false;
    } );
}
```

### Text Delay mode

Because of some limitations in Blazor, sometimes there can be problems when `ChangeTextOnKeyPress` is enabled. Basically if you try to type too fast into the text field the caret can jump randomly from current selection to the end of the text. To prevent that behaviour you need to enable `DelayTextOnKeyPress`. Once enabled it will slightly delay every value entered into the field to allow the Blazor engine to do it's thing. By default this option is disabled.

```cs
public void ConfigureServices( IServiceCollection services )
{
  services
    .AddBlazorise( options =>
    {
      options.DelayTextOnKeyPress = true;
      options.DelayTextOnKeyPressInterval = 300;
    } );
}
```

**Note:** All of the options above can also be defined on each `MemoEdit` individually. Defining them on `MemoEdit` will override any global settings.
{: .notice--info}

## Attributes

| Name                          | Type                                                         | Default | Description                                                                                          |
|-------------------------------|--------------------------------------------------------------|---------|------------------------------------------------------------------------------------------------------|
| Text                          | string                                                       |         | Input value.                                                                                         |
| TextChanged                   | event                                                        |         | Occurs after text has changed.                                                                       |
| Plaintext                     | boolean                                                      | false   | Remove the default form field styling and preserve the correct margin and padding.                   |
| ReadOnly                      | boolean                                                      | false   | Prevents modification of the input’s value.                                                          |
| Disabled                      | boolean                                                      | false   | Prevents user interactions and make it appear lighter.                                               |
| MaxLength                     | `int?`                                                       | null    | Specifies the maximum number of characters allowed in the input element.                             |
| Placeholder                   | string                                                       |         | Sets the placeholder for the empty text.                                                             |
| Rows                          | `int?`                                                       | null    | Specifies the number lines in the input element.                                                     |
| Size                          | [Sizes]({{ "/docs/helpers/sizes/#size" | relative_url }})    | `None`  | Component size variations.                                                                           |
| ChangeTextOnKeyPress          | `bool?`                                                      |  null   | If true the text in will be changed after each key press (overrides global settings).                |
| DelayTextOnKeyPress           | `bool?`                                                      |  null   | If true the entered text will be slightly delayed before submitting it to the internal value.        |
| DelayTextOnKeyPressInterval   | `int?`                                                       |  null   | Interval in milliseconds that entered text will be delayed from submitting to the internal value.    |