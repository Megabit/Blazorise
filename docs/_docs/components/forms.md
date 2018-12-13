---
title: "Forms"
permalink: /docs/components/forms/
excerpt: "Forms."
toc: true
toc_label: "Components"
---

## Basic Text

Use TextEdit to have a basic input.

```html
<TextEdit />
```

<iframe src="/examples/forms/text-basic/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Binding

TextEdit by itself is not really usable if you can't use it. To get or set the input value you have two options. You can use _bind-*_ attribute or you can use a _TextChanged_ event.

1. Using bind-*

    ```cs
    <TextEdit bind-Text="@name" />
    @functions{
        string name;
    }
    ```

2. Using TextChanged event

    ```cs
    <TextEdit Text="@name" TextChanged="@OnNameChanged" />

    @functions{
        string name;

        void OnNameChanged( string value )
        {
            name = value;
        }
    }
    ```

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

### Disabled text

A disabled input element is unusable and un-clickable.

```html
<TextEdit IsDisabled="true" />
```

<iframe src="/examples/forms/text-disabled/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Readonly text

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

### Roles

Use Role to define text value.

```html
<TextEdit Role="TextRole.Email" />
<TextEdit Role="TextRole.Password" />
```

<iframe src="/examples/forms/text-roles/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Memo

MemoEdit is used to create multiline text input (textarea).

```html
<MemoEdit Rows="5" />
```

<iframe src="/examples/forms/memo/" frameborder="0" scrolling="no" style="width:100%;height:143px;"></iframe>

## Select

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

### Multiple

Add the `IsMultiple` attribute to allow more than one option to be selected.

```html
<SelectEdit IsMultiple="true">
    <SelectItem>1</SelectItem>
    <SelectItem>2</SelectItem>
    <SelectItem>3</SelectItem>
    <SelectItem>4</SelectItem>
</SelectEdit>
```

<iframe src="/examples/forms/select-multiple/" frameborder="0" scrolling="no" style="width:100%;height:112px;"></iframe>