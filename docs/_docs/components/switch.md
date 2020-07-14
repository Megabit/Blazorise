---
title: "Switch component"
permalink: /docs/components/switch/
excerpt: "Learn how to use switch component."
toc: true
toc_label: "Guide"
---

Use `Switch` to toggle the state of a single setting on or off.

Switches are the preferred way to adjust settings on mobile. The option that the switch controls, as well as the state it’s in, should be made clear from the corresponding inline label.

## Examples

```html
<Switch TValue="bool">Remember me</Switch>
```

<iframe src="/examples/forms/switch/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Usage

### Two-way binding

```html
<Switch TValue="bool" @bind-Checked="@rememberMe">Remember Me</Switch>

@code{
    bool rememberMe;
}
```

### Manual binding

```html
<Switch TValue="bool" Checked="@rememberMe" CheckedChanged="@OnRememberMeChanged">Remember Me</Switch>

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
| Color                   | [Colors]({{ "/docs/helpers/colors/#color" | relative_url }})               | `None`       | Component visual or contextual style variants.                                        |