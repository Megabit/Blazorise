---
title: "Check component"
permalink: /docs/components/check/
excerpt: "Learn how to use check component."
toc: true
toc_label: "Guide"
---

## Check

```html
<CheckEdit>Check me out</CheckEdit>
```

<iframe src="/examples/forms/check/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Radio

For radio control you must set the `RadioGroup` attribute.

```html
<CheckEdit RadioGroup="someGroupName">Select this</CheckEdit>
<CheckEdit RadioGroup="someGroupName">Or this</CheckEdit>
```

<iframe src="/examples/forms/radio/" frameborder="0" scrolling="no" style="width:100%;height:55px;"></iframe>

## Usage

### With bind attribute

```html
<CheckEdit bind-Checked="@rememberMe">Remember Me</CheckEdit>

@code{
    bool rememberMe;
}
```

### With event

```html
<CheckEdit Checked="@rememberMe" CheckedChanged="@OnRememberMeChanged">Remember Me</CheckEdit>

@code{
    bool rememberMe;

    void OnRememberMeChanged( bool value )
    {
        rememberMe = value;
    }
}
```