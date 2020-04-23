---
title: "Text component"
permalink: /docs/components/text/
excerpt: "Learn how to use form text component."
toc: true
toc_label: "Guide"
---

## Text

Use TextEdit to have a basic input.

```html
<TextEdit />
```

<iframe src="/examples/forms/text-basic/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Placeholder

```html
<TextEdit Placeholder="Some text value..." />
```

<iframe src="/examples/forms/text-placeholder/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Static text

Static text removes the background, border, shadow, and horizontal padding, while maintaining the vertical spacing so you can easily align the input in any context, like a horizontal form.

```html
<TextEdit Plaintext="true" />
```

<iframe src="/examples/forms/text-plain/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Disabled

A disabled input element is unusable and un-clickable.

```html
<TextEdit Disabled="true" />
```

<iframe src="/examples/forms/text-disabled/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Readonly

If you use the read-only attribute, the input text will look similar to a normal one, but is not editable.

```html
<TextEdit ReadOnly="true" />
```

<iframe src="/examples/forms/text-readonly/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Sizing

Sets the heights of input elements.

```html
<TextEdit Size="Size.Small" />
<TextEdit Size="Size.Large" />
```

<iframe src="/examples/forms/text-sizing/" frameborder="0" scrolling="no" style="width:100%;height:107px;"></iframe>

### Pattern

Use pattern attribute to specify regular expression that will be used while validating the input text value.

```html
<Validation UsePattern="true">
    <TextEdit Pattern="[A-Za-z]{3}">
        <Feedback>
            <ValidationError>Pattern does not match!</ValidationError>
        </Feedback>
    </TextEdit>
</Validation>
```

### EditMask

Edit masks are used to prevent user from entering an invalid values and when entered string must match a specific format. For example you can limit input to only accept numeric string, date string or if you want full control you can use RegEx format.

```html
<TextEdit MaskType="MaskType.RegEx" EditMask="^[a-zA-Z ]*$" />
```

## Roles

Use Role to define text value.

```html
<TextEdit Role="TextRole.Email" />
<TextEdit Role="TextRole.Password" />
```

<iframe src="/examples/forms/text-roles/" frameborder="0" scrolling="no" style="width:100%;height:100px;"></iframe>

## Usage

TextEdit by itself is not really usable if you can't use it. To get or set the input value you have two options. You can use `bind-*` attribute or you can use a `TextChanged` event.

### With bind attribute

By using `bind-*` attribute the text will be automatically assigned to the member variable.

```html
<TextEdit @bind-Text="@name" />

@code{
    string name;
}
```

### With event

When using the event `TextChanged`, you also must define the `Text` value attribute.

```html
<TextEdit Text="@name" TextChanged="@OnNameChanged" />

@code{
    string name;

    void OnNameChanged( string value )
    {
        name = value;
    }
}
```

## Settings

### TextChanged mode

By default the TextChanged event will be raised on every keypress. To override default behavior of `TextChanged` event and to disable the change on every keypress you must set the `ChangeTextOnKeyPress` to `false` on application start. After setting it to `false` the event will be raised only after the input loses focus.

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

## Attributes

| Name              | Type                                                                | Default | Description                                                                                          |
|-------------------|---------------------------------------------------------------------|---------|------------------------------------------------------------------------------------------------------|
| Role              | TextRole                                                            | `Text`  | The role of the input text.                                                                          |
| Text              | string                                                              |         | Input value.                                                                                         |
| TextChanged       | event                                                               |         | Occurs after text has changed.                                                                       |
| Plaintext         | boolean                                                             | false   | Remove the default form field styling and preserve the correct margin and padding.                   |
| ReadOnly          | boolean                                                             | false   | Prevents modification of the input’s value.                                                          |
| Disabled          | boolean                                                             | false   | Prevents user interactions and make it appear lighter.                                               |
| MaxLength         | int?                                                                | null    | Specifies the maximum number of characters allowed in the input element.                             |
| Placeholder       | string                                                              |         | Sets the placeholder for the empty text.                                                             |
| Pattern           | string                                                              |         | Specifies a regular expression that the input element's value is checked against on form validation. |
| Color             | [Colors]({{ "/docs/helpers/colors/#color" | relative_url }})        | `None`  | Component visual or contextual style variants.                                                       |
| Size              | [Sizes]({{ "/docs/helpers/sizes/#size" | relative_url }})           | `None`  | Component size variations.                                                                           |
| EditMask          | string                                                              |         | A string representing a edit mask expression.                                                        |
| MaskType          | [MaskType]({{ "/docs/helpers/enums/#masktype" | relative_url }})    | `None`  | Specify the mask type used by the editor.                                                            |
| VisibleCharacters | int?                                                                |  null   | Specifies the visible width, in characters, of an `<input>` element.                                   |