---
title: "Check component"
permalink: /docs/components/check/
excerpt: "Learn how to use check component."
toc: true
toc_label: "Guide"
---

The `Check` component is another basic element for user input. You can use this to supply a way for the user to toggle an option.

## Check

**Note:** As of **v0.9** it is required to define `Check` value type by settings the `TValue` attribute.
{: .notice--warning}

```html
<Check TValue="bool">Check me out</Check>
```

<iframe src="/examples/forms/check/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Usage

### With bind attribute

```html
<Check TValue="bool" @bind-Checked="@rememberMe">Remember Me</Check>

@code{
    bool rememberMe;
}
```

### With event

```html
<Check TValue="bool" Checked="@rememberMe" CheckedChanged="@OnRememberMeChanged">Remember Me</Check>

@code{
    bool rememberMe;

    void OnRememberMeChanged( bool value )
    {
        rememberMe = value;
    }
}
```

## Attributes

| Name                    | Type                                                                       | Default      | Description                                                                           |
|-------------------------|----------------------------------------------------------------------------|--------------|---------------------------------------------------------------------------------------|
| TValue                  | generic type                                                               |              | Data type of `Checked` value. Support types are `bool` and `bool?`.                   |
| Checked                 | `TValue`                                                                   | `default`    | Gets or sets the checked flag.                                                        |
| CheckedChanged          | `EventCallback<TValue>`                                                    |              | Occurs when the check state is changed.                                               |
| Indeterminate           | `bool?`                                                                    | `null`       | The indeterminate property can help you to achieve a 'check all' effect.              |
| Inline                  | `bool`                                                                     | `false`      | Group checkboxes on the same horizontal row.                                          |
| Cursor                  | [Cursor]({{ "/docs/helpers/enums/#cursor" | relative_url }})               | `Default`    | Defines the mouse cursor based on the behavior by the current CSS framework.          |