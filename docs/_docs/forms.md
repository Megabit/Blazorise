---
title: "Form components"
permalink: /docs/forms/
excerpt: "Learn how to use form components like text, memo, select, check, radio, date."
toc: true
toc_label: "Guide"
redirect_from: /docs/components/forms/
---

## Input controls

Most of the basic input types are available as a standalone components.

- [Text]({{ "/docs/components/text/" | relative_url }})
- [Memo]({{ "/docs/components/memo/" | relative_url }})
- [Select]({{ "/docs/components/select/" | relative_url }})
- [Date]({{ "/docs/components/date/" | relative_url }})
- [Check & Radio]({{ "/docs/components/check/" | relative_url }}) 

**For future reference:** While most of the input controls are named with ***Edit** suffix, this will likely be changed in the future(**v1.0**). The reason is that Razor & VS doesn't allow for Blazor components to be named as regular html tags, see [#5550](https://github.com/aspnet/AspNetCore/issues/5550)
{: .notice--warning}

## Containers

Containers are used to properly style input controls and also to keep the right spacing between controls.

- [Field]({{ "/docs/components/field/" | relative_url }})
- [Addon]({{ "/docs/components/addon/" | relative_url }})

## Usage

Using input controls is the same for all input types, the only difference is that value attributes will be named accordingly to the input type eg. (`Text` for `TextEdit`, `Date` for `DateEdit`, etc.)

The following is the example for `TextEdit`:

### Events

To use event you must provide both `Text` value attribute and `TextChanged` event function.

```cs
<TextEdit Text="@name" TextChanged="@OnNameChanged" />

@code{
    string name;

    void OnNameChanged( string value )
    {
        name = value;
    }
}
```

### Binding

Blazorise also supports automatic binding via `bind-*` attribute to keep it all much simpler.

```cs
<TextEdit @bind-Text="@name" />

@code{
    string name;
}
```

If you want to learn more about binding please go to the [official Blazor site](https://docs.microsoft.com/en-us/aspnet/core/blazor/data-binding).