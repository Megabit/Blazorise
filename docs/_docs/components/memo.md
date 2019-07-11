---
title: "Memo component"
permalink: /docs/components/memo/
excerpt: "Learn how to use memo component."
toc: true
toc_label: "Guide"
---

## Basic Memo

MemoEdit is used to create multiline text input (textarea).

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

## Attributes

| Name        | Type                                                         | Default | Description                                                                                          |
|-------------|--------------------------------------------------------------|---------|------------------------------------------------------------------------------------------------------|
| Text        | string                                                       |         | Input value.                                                                                         |
| TextChanged | event                                                        |         | Occurs after text has changed.                                                                       |
| IsPlaintext | boolean                                                      | false   | Remove the default form field styling and preserve the correct margin and padding.                   |
| IsReadonly  | boolean                                                      | false   | Prevents modification of the input’s value.                                                          |
| IsDisabled  | boolean                                                      | false   | Prevents user interactions and make it appear lighter.                                               |
| MaxLength   | int?                                                         | null    | Specifies the maximum number of characters allowed in the input element.                             |
| Placeholder | string                                                       |         | Sets the placeholder for the empty text.                                                             |
| Rows        | int?                                                         | null    | Specifies the number lines in the input element.                                                     |
| Size        | [Sizes]({{ "/docs/helpers/sizes/#size" | relative_url }})    | `None`  | Component size variations.                                                                           |