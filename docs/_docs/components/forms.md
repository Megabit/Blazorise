---
title: "Forms"
permalink: /docs/components/forms/
excerpt: "Forms."
toc: true
---

## Text input

To have a basic input text you only need to write this

```html
<TextEdit />
```

### Binding

Text by itself is not really usable if you can't use it. To get or set the input value you have two options. You can use _bind-*_ attribute or you can use a _TextChanged_ event.

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

```html
<TextEdit IsDisabled="true" />
```

### Readonly text

If you use the readonly attribute, the input text will look similar to a normal one, but is not editable.

```html
<TextEdit IsReadonly="true" />
```

### Sizing

```html
<TextEdit Size="Size.Small" />
<TextEdit Size="Size.Large" />
```

## Memo input

```html
<MemoEdit Rows="5" />
```