---
title: "Theming"
permalink: /docs/theming/
excerpt: "Customize Blazorise with our new built-in ThemeProvider for global style preferences for easy theming and component changes."
toc: true
toc_label: "Guide"
---

## Overview

Blazorise offers its own theme provider to control your application look and feel. Generally, if you stick to the built-in provider(s) and it's CSS classes and variants, there isn't anything else you need to do to use a custom theme with Blazorise. It just works. But we also make coloring outside the lines easy to do.

## Start

To prepare your application for theming you need to wrap your root component with the `ThemeProvider` component tag. The best option is to add it to the **App.razor** where you wrap the `Router` tag with the `ThemeProvider` tag. Next you must set the `Theme` attribute where you can configure _colors_, _borders_, _margins_, _paddings_ etc. 

If no properties are specified, default settings will be used.

```cs
<Blazorise.ThemeProvider Theme="@theme">
    <Router AppAssembly="typeof(Program).Assembly" />
</Blazorise.ThemeProvider>

@code{
    private Theme theme = new Theme
    {
        // theme settings
    };
}
```

What this little peace of code is doing behind the scene? Basically it's generating a fully customized CSS and styles based on the setting provided in the Theme attribute. It will also generate for you a list of CSS variables that you can use later if you want to expand your applications styles even further. The things like colors and element settings will be saved as CSS variables to the **:root**.

### Runtime change

To dynamically change theme settings you can use `Theme` from anywhere in your application.

```cs
<Button Clicked="@(()=>OnThemeColorChanged("#d16f9e"))">Change theme</Button>

void OnThemeColorChanged( string value )
{
    if ( Theme?.ColorOptions != null )
        Theme.ColorOptions.Primary = value;

    if ( Theme?.BackgroundOptions != null )
        Theme.BackgroundOptions.Primary = value;

    Theme.ThemeHasChanged();
}

[CascadingParameter] Theme Theme { get; set; }
```

### Colors

With `ColorOptions` you can customize your application variant colors. Note that colors must be defined in hex format!

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

### Gradient

Enables the gradient background colors.

```cs
<Blazorise.ThemeProvider Theme="@theme">
    <Router AppAssembly="typeof(Program).Assembly" />
</Blazorise.ThemeProvider>

@code{
    private Theme theme = new Theme
    {
        IsGradient = true,
        ...
    };
}
```

### Border Radius

Globally enable or disable rounded elements.

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

## Theme structure

- `White` light color used to calculate theme contrast
- `Black` dark color used to calculate theme contrast
- `IsGradient` enable gradient background colors of common elements.
- `IsRounded` enables roundness of common elements, such as buttons.
- `ColorOptions` various colors used through different elements.
  - `Primary` primary color for your app, usually your brand color.
  - `Secondary` secondary color for your app which complements the primary color.
  - `Success` 
  - `Info`
  - `Warning`
  - `Danger`
  - `Light`
  - `Dark`
- `BackgroundOptions` various colors used for background elements like tables.
- `ButtonOptions` various settings for button element
  - `Padding`
  - `Margin`
  - `BoxShadowSize`
  - `BoxShadowTransparency`
  - `HoverDarkenColor`
  - `HoverLightenColor`
  - `ActiveDarkenColor`
  - `ActiveLightenColor`
  - `LargeBorderRadius`
  - `SmallBorderRadius`
- `DropdownOptions` various settings for dropdown element
- `InputOptions` various settings for input fields
  - `Color` default text color for inputs
- `CardOptions` various settings for card containers
  - `ImageTopRadius`
- `ModalOptions` various settings for modal dialogs
- `TabsOptions` various settings for tabs
- `ProgressOptions` various settings for progress bar
- `AlertOptions` various settings for alert component
  - `BackgroundLevel`
  - `BorderLevel`
  - `ColorLevel`
- `TableOptions` various settings for table component
  - `BackgroundLevel`
  - `BorderLevel`
- `BreadcrumbOptions` various settings for breadcrumbs
- `BadgeOptions` various settings for badges
- `PaginationOptions` various settings for paginations
  - `LargeBorderRadius`
- `SidebarOptions` various settings for sidebar component
- `SnackbarOptions` various settings for snackbar  component

There are some more options to choose when setting the Theme so feel free to explore it all. Please note that in time there will be many more options added.