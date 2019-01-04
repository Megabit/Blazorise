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
<TextEdit IsPlaintext="true" />
```

<iframe src="/examples/forms/text-plain/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Disabled

A disabled input element is unusable and un-clickable.

```html
<TextEdit IsDisabled="true" />
```

<iframe src="/examples/forms/text-disabled/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Readonly

If you use the readonly attribute, the input text will look similar to a normal one, but is not editable.

```html
<TextEdit IsReadonly="true" />
```

<iframe src="/examples/forms/text-readonly/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Sizing

Sets the heights of input elements.

```html
<TextEdit Size="Size.Small" />
<TextEdit Size="Size.Large" />
```

<iframe src="/examples/forms/text-sizing/" frameborder="0" scrolling="no" style="width:100%;height:107px;"></iframe>

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
<TextEdit bind-Text="@name" />

@functions{
    string name;
}
```

### With event

When using the event `TextChanged`, you also must define the `Text` value attribute.

```html
<TextEdit Text="@name" TextChanged="@OnNameChanged" />

@functions{
    string name;

    void OnNameChanged( string value )
    {
        name = value;
    }
}
```