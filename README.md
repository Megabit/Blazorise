![Blazorise](https://user-images.githubusercontent.com/900302/147649481-11ca2931-34cd-4e24-8035-fe757cf9d744.png)

# Blazorise: Blazor UI Components for .NET

[![NuGet](https://img.shields.io/nuget/vpre/Blazorise.svg)](https://www.nuget.org/profiles/Megabit)
![Nuget](https://img.shields.io/nuget/dt/Blazorise.svg)
[![Discord](https://img.shields.io/discord/761589226965696552?color=%237289da&label=Discord&logo=discord&logoColor=%237289da&style=flat-square)](https://discord.gg/cVmq8xBSnG)
[![License](https://img.shields.io/badge/License-Dual%20License-blue.svg)](LICENSE.md)

Blazorise is an open-source Blazor UI component library for building modern .NET web apps in C#. It provides a consistent component API across popular CSS frameworks, so you can build with Bootstrap 5, Tailwind CSS, Bulma, Material, AntDesign, or Fluent UI 2 without rewriting your app. Blazorise works with Blazor WebAssembly and Blazor Server, and supports theming, layouts, forms, and rich UI components.

## Highlights

- Provider-agnostic components with multiple CSS framework providers.
- C#-first development with a consistent API across providers.
- Works in Blazor WebAssembly and Blazor Server apps.
- Optional commercial themes, blocks, and priority support.

## Commercial licensing

Blazorise is dual-licensed. Use is governed by the terms in [LICENSE.md](LICENSE.md) or a commercial license, depending on your scenario. For teams that need premium assets and support, commercial subscriptions are available.

Commercial subscriptions include:

- Access to [Blazorise Themes](https://blazorise.com/themes).
- Access to [Blazorise Blocks](https://blazorise.com/blocks).
- Premium support via [Blazorise Support](https://blazorise.com/support) forum.
- Dedicated customer support with response times of either 24 or 16 hours, depending on plan.
- Priority fixes and feature requests.

> \* Some features may be exclusive to specific subscription tiers.

## Supporting Blazorise

Blazorise is an open source project with its ongoing development made possible entirely by the support of these awesome backers.

### Special Partners

<!--platinum start-->
<table>
  <tbody>
    <tr>
      <td align="center" valign="middle">
        <a href="https://volosoft.com/" target="_blank">
          <img width="222px" src="https://volosoft.com/assets/logos/volosoft-logo-dark.svg">
        </a>
      </td>
      <td align="center" valign="middle">
        <a href="https://www.pebble.tv/" target="_blank">
          <img width="222px" src="https://www.pebble.tv/wp-content/uploads/2020/10/logo.svg">
        </a>
      </td>
    </tr>
    <tr></tr>
  </tbody>
</table>
<!--platinum end-->

## Demos

### Blazor WebAssembly

- [Tailwind Demo](https://tailwinddemo.blazorise.com)
- [Bootstrap 4 Demo](https://bootstrapdemo.blazorise.com)
- [Bootstrap 5 Demo](https://bootstrap5demo.blazorise.com)
- [Material Demo](https://materialdemo.blazorise.com/)
- [Bulma Demo](https://bulmademo.blazorise.com/)
- [AntDesign Demo](https://antdesigndemo.blazorise.com/)
- [Fluent 2 Demo](https://fluentui2demo.blazorise.com/)

### Blazor Server

- [Bootstrap Demo](https://rcbootstrapdemo.blazorise.com/)

## Documentation

For full documentation, component API references, and detailed guides, visit the Blazorise [official documentation pages](https://blazorise.com/docs/).

Continue reading below for a quick start guide.

## Installation

### Prerequisites

Before you continue, make sure you have a recent .NET SDK and a supported IDE (Visual Studio or VS Code). Visit the official [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/client) site to learn more.

### Provider packages

There are currently 7 provider packages, one per supported CSS framework.

Available Blazorise packages are:

```
- Blazorise.Tailwind
- Blazorise.Bootstrap
- Blazorise.Bootstrap5
- Blazorise.Bulma
- Blazorise.Material
- Blazorise.AntDesign
- Blazorise.FluentUI2
```

This guide shows how to set up Blazorise with **Bootstrap 5** and **FontAwesome 6** icons. To set up Blazorise for other CSS frameworks, refer to the [Usage](https://blazorise.com/docs/usage/) page in the documentation.

### 1. NuGet packages

Install a provider package and any icon package you want to use. Example for Bootstrap 5 and FontAwesome:

```bash
dotnet add package Blazorise.Bootstrap5
```

And FontAwesome icon package:

```bash
dotnet add package Blazorise.Icons.FontAwesome
```

### 2. Source files

Add the following to `index.html` (Blazor WebAssembly), `_Host.cshtml` (Blazor Server), or `App.razor` (.NET 8+ Blazor Web App) in the `head` section.

```html
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous">
<link href="_content/Blazorise.Icons.FontAwesome/v6/css/all.min.css" rel="stylesheet">

<link href="_content/Blazorise/blazorise.css?v=2.0.0.0" rel="stylesheet" />
<link href="_content/Blazorise.Bootstrap5/blazorise.bootstrap5.css?v=2.0.0.0" rel="stylesheet" />
```

The `?v=2.0.0.0` query string matches the current Blazorise package version (2.0.0) and is used for cache busting. Update it whenever you upgrade Blazorise packages. If you use a different provider, swap the Bootstrap CSS and provider-specific Blazorise CSS file accordingly.

#### 2.1 JavaScript resources

Blazorise loads any additional JavaScript it needs dynamically once a component needs it. Make sure the resources are available and served relative to the app root. For Blazor Server, enable static files with `app.UseStaticFiles();`.

If you're having any difficulties, please refer to the following issues:

- [#3122](https://github.com/Megabit/Blazorise/issues/3122)
- [#3150](https://github.com/Megabit/Blazorise/issues/3150)

We are also aware that there might need to be extra setup when dealing with PWA and offline capabilities if you want your app to remain responsive. Please check our [PWA docs](https://blazorise.com/docs/pwa) for more information.

### 3. Usings

In your main `_Imports.razor`, add:

```cs
@using Blazorise
```

### 4. Service registration

Add the following lines to the relevant sections of `Program.cs`.

```cs
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
```

```cs
builder.Services
  .AddBlazorise()
  .AddBootstrap5Providers()
  .AddFontAwesomeIcons();
```

## Examples

### Counter page

```razor
@page "/counter"

<Heading Size="HeadingSize.Is1">Counter</Heading>

<Paragraph>Current count: @currentCount</Paragraph>

<Button Color="Color.Primary" Clicked="IncrementCount">Click me</Button>

@code {
    private int currentCount;

    private void IncrementCount()
    {
        currentCount++;
    }
}
```

### Simple form

```razor
@page "/profile"

<Heading Size="HeadingSize.Is3">Profile</Heading>

<TextInput @bind-Value="displayName" Placeholder="Ada Lovelace" />

<Button Color="Color.Primary" Clicked="Save">Save</Button>

<Alert Color="Color.Success" Visible="isSaved">
    Saved!
</Alert>

@code {
    private string displayName = string.Empty;
    private bool isSaved;

    private void Save()
    {
        isSaved = true;
    }
}
```

## Development builds

We release a development version of Blazorise on a regular basis. This version is available on the [MyGet](https://www.myget.org/gallery/blazorise) feed.
This release is not recommended for production use, but it's a great way to test the latest features and bug fixes and give us feedback ahead of the next release.

## Contributing

We welcome contributions and any suggestions or feature requests you might have. Contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. This will be signed once you submit a PullRequest on our repository. For details about our CLA, please visit: [Contributor License Agreement](https://gist.github.com/stsrki/abfa5ce0f4a5cf1e6ac67b92f8eb5d63).

For our code conventions and guidelines please visit: [Contributing Guide](https://github.com/Megabit/Blazorise/wiki/Contributing)

## Copyright

Copyright (c) Megabit d.o.o.

Dual-licensed. See [LICENSE.md](LICENSE.md).
