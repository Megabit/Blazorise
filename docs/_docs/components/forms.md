---
title: "Forms"
permalink: /docs/components/forms/
excerpt: "Forms."
toc: true
toc_label: "Components"
---

## Text

Use TextEdit to have a basic input.

```html
<TextEdit />
```

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
<TextEdit Placeholder="Name" />
```

### Static text

Static text removes the background, border, shadow, and horizontal padding, while maintaining the vertical spacing so you can easily align the input in any context, like a horizontal form.

```html
<TextEdit IsPlaintext="true" />
```

### Disabled text

A disabled input element is unusable and un-clickable.

```html
<TextEdit IsDisabled="true" />
```

### Readonly text

If you use the readonly attribute, the input text will look similar to a normal one, but is not editable.

```html
<TextEdit IsReadonly="true" />
```

### Sizing

Sets the heights of input elements.

```html
<TextEdit Size="Size.Small" />
<TextEdit Size="Size.Large" />
```

### Roles

Use Role to define text value.

```html
<TextEdit Role="TextRole.Email" />
<TextEdit Role="TextRole.Password" />
```

## Memo

MemoEdit is used to create multiline text input (textarea).

```html
<MemoEdit Rows="5" />
```

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