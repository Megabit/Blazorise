---
title: "Theming"
permalink: /docs/theming/
excerpt: "Customize Blazorise with our new built-in ThemeProvider for global style preferences for easy theming and component changes."
toc: true
toc_label: "Guide"
---

## Theming and Customizing styles.

Generally, if you stick to the CSS provider classes and variants, there isn't anything you need to do to use a custom theme with Blazorise. It just works. But we also make coloring outside the lines easy to do.

### Modify colors

Just add the following in **App.razor**

```cs
<Blazorise.ThemeProvider Theme="@theme">
    <Router AppAssembly="typeof(Program).Assembly" />
</Blazorise.ThemeProvider>

@code{
    private Theme theme = new Theme
    {
        ColorOptions = new ThemeColorOptions
        {
            Primary = "#45B1E8",
            Secondary = "#A65529",
            ...
        }
    };
}
```

### Rounded

Globaly enable or disable rounded elements.

```cs
<Blazorise.ThemeProvider Theme="@theme">
    <Router AppAssembly="typeof(Program).Assembly" />
</Blazorise.ThemeProvider>

@code{
    private Theme theme = new Theme
    {
        IsRounded = false,
        ...
    };
}
```