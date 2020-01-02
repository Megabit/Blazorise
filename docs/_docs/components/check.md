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
<CheckEdit @bind-Checked="@rememberMe">Remember Me</CheckEdit>

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

## Attributes

| Name                    | Type                                                                       | Default      | Description                                                                           |
|-------------------------|----------------------------------------------------------------------------|--------------|---------------------------------------------------------------------------------------|
| Checked                 | boolean                                                                    | false        | Gets or sets the checked flag.                                                        |
| NullableChecked         | boolean                                                                    | false        | Gets or sets the nullable value for checked flag.    **[WILL BE REMOVED]**            |
| CheckedChanged          | event                                                                      |              | Occurs when the check state is changed.                                               |
| NullableCheckedChanged  | event                                                                      |              | Occurs when the check state of nullable value is changed. **[WILL BE REMOVED]**       |
| RadioGroup              | string                                                                     | null         | Sets the radio group name.                                                            |
| IsInline                | boolean                                                                    | false        | Group checkboxes or radios on the same horizontal row.                                |
| Cursor                  | [Cursor]({{ "/docs/helpers/enums/#cursor" | relative_url }})               | `Default`    | Defines the mouse cursor based on the behavior by the current CSS framework.         |