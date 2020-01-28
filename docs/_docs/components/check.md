---
title: "Check component"
permalink: /docs/components/check/
excerpt: "Learn how to use check component."
toc: true
toc_label: "Guide"
---

The `CheckEdit` component is another basic element for user input. You can use this to supply a way for the user to toggle an option.

## Check

**Note:** As of **v0.9** it is required to define `CheckEdit` value type by settings the `TValue` attribute.
{: .notice--warning}

```html
<CheckEdit TValue="bool">Check me out</CheckEdit>
```

<iframe src="/examples/forms/check/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Radio

For radio control you must set the `RadioGroup` attribute.

```html
<CheckEdit TValue="bool" RadioGroup="someGroupName">Select this</CheckEdit>
<CheckEdit TValue="bool" RadioGroup="someGroupName">Or this</CheckEdit>
```

<iframe src="/examples/forms/radio/" frameborder="0" scrolling="no" style="width:100%;height:55px;"></iframe>

## Usage

### With bind attribute

```html
<CheckEdit TValue="bool" @bind-Checked="@rememberMe">Remember Me</CheckEdit>

@code{
    bool rememberMe;
}
```

### With event

```html
<CheckEdit TValue="bool" Checked="@rememberMe" CheckedChanged="@OnRememberMeChanged">Remember Me</CheckEdit>

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
| Checked                 | boolean                                                                    | false        | Gets or sets the checked flag.                                                        |
| CheckedChanged          | event                                                                      |              | Occurs when the check state is changed.                                               |
| RadioGroup              | string                                                                     | null         | Sets the radio group name.                                                            |
| Inline                  | boolean                                                                    | false        | Group checkboxes or radios on the same horizontal row.                                |
| Cursor                  | [Cursor]({{ "/docs/helpers/enums/#cursor" | relative_url }})               | `Default`    | Defines the mouse cursor based on the behavior by the current CSS framework.          |