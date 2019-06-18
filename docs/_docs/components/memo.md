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
<MemoEdit bind-Text="@description" />

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