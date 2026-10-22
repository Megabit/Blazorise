![Blazorise](https://user-images.githubusercontent.com/900302/147649481-11ca2931-34cd-4e24-8035-fe757cf9d744.png)

# Blazorise: Blazor UI Components for .NET

[![NuGet](https://img.shields.io/nuget/vpre/Blazorise.svg)](https://www.nuget.org/packages/Blazorise)
[![NuGet downloads](https://img.shields.io/nuget/dt/Blazorise.svg)](https://www.nuget.org/packages/Blazorise)
[![Discord](https://img.shields.io/discord/761589226965696552?color=%237289da&label=Discord&logo=discord&logoColor=%237289da&style=flat-square)](https://discord.gg/cVmq8xBSnG)
[![License](https://img.shields.io/badge/License-Dual%20License-blue.svg)](LICENSE.md)

Blazorise is a source-available Blazor UI component library for building .NET web applications in C#. Its consistent component API lets you choose the CSS framework that fits your application, from Bootstrap and Tailwind CSS to Bulma, Material, Ant Design, and Fluent 2.

[Documentation](https://blazorise.com/docs/) · [Quick start](#quick-start) · [Demos](#css-providers-and-demos) · [Contributing](.github/CONTRIBUTING.md)

## Highlights

- Forms, validation, navigation, dialogs, and other everyday UI components.
- DataGrid, charts, and extensions for applications that work with data.
- A consistent C# API across CSS framework providers.
- Theming and support for right-to-left layouts.
- Blazor WebAssembly, Blazor Server, and interactive Blazor Web Apps.

## Documentation

Browse the [component documentation](https://blazorise.com/docs/), follow a [provider integration guide](https://blazorise.com/docs/usage), or start a new application with the [Blazorise templates](https://blazorise.com/docs/templates).

## CSS providers and demos

Choose a provider package, then follow its integration guide. Each provider has a live WebAssembly demo.

| CSS framework | NuGet package | Setup | Demo |
| --- | --- | --- | --- |
| Bootstrap 5 | `Blazorise.Bootstrap5` | [Guide](https://blazorise.com/docs/usage/bootstrap5) | [Demo](https://demos.blazorise.com/wasm/bootstrap5/) |
| Bootstrap 4 | `Blazorise.Bootstrap` | [Guide](https://blazorise.com/docs/usage/bootstrap4) | [Demo](https://demos.blazorise.com/wasm/bootstrap4/) |
| Tailwind CSS | `Blazorise.Tailwind` | [Guide](https://blazorise.com/docs/usage/tailwind) | [Demo](https://demos.blazorise.com/wasm/tailwind/) |
| Bulma | `Blazorise.Bulma` | [Guide](https://blazorise.com/docs/usage/bulma) | [Demo](https://demos.blazorise.com/wasm/bulma/) |
| Material | `Blazorise.Material` | [Guide](https://blazorise.com/docs/usage/material) | [Demo](https://demos.blazorise.com/wasm/material/) |
| Ant Design | `Blazorise.AntDesign` | [Guide](https://blazorise.com/docs/usage/ant-design) | [Demo](https://demos.blazorise.com/wasm/antdesign/) |
| Fluent 2 | `Blazorise.FluentUI2` | [Guide](https://blazorise.com/docs/usage/fluent2) | [Demo](https://demos.blazorise.com/wasm/fluentui2/) |

A [Bootstrap 4 Blazor Server demo](https://demos.blazorise.com/server/bootstrap4/) is also available.

## Quick start

This example adds **Bootstrap 5** and **FontAwesome 6** icons to an existing Blazor project.

### Prerequisites

Use a Blazor project targeting .NET 8, .NET 9, .NET 10, or .NET 11, with an SDK and IDE that support your chosen target framework. Keep the Blazorise provider and icon packages on the same version. The library targets in this checkout are maintained in [Build/Blazorise.props](Build/Blazorise.props).

For a new project, see the [Blazorise templates](https://blazorise.com/docs/templates). Building this repository has separate SDK requirements; see [Local development](#local-development).

### 1. Install packages

Run these commands in your application's project directory:

```bash
dotnet add package Blazorise.Bootstrap5
dotnet add package Blazorise.Icons.FontAwesome
```

### 2. Add stylesheets

Add these links inside the host page's `<head>` section:

| Project type | Host file |
| --- | --- |
| Blazor WebAssembly | `wwwroot/index.html` |
| Blazor Server | `_Host.cshtml` or `_Layout.cshtml`, depending on the template |
| Blazor Web App (.NET 8+) | `Components/App.razor` in the server project |

```html
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-sRIl4kxILFvY47J16cr9ZwB07vP4J8+LH7qKQnuqkuIAvNWLzeN8tE5YBujZqJLB" crossorigin="anonymous">
<link href="_content/Blazorise.Icons.FontAwesome/v6/css/all.min.css?v=2.4.1.0" rel="stylesheet">

<link href="_content/Blazorise/blazorise.css?v=2.4.1.0" rel="stylesheet" />
<link href="_content/Blazorise.Bootstrap5/blazorise.bootstrap5.css?v=2.4.1.0" rel="stylesheet" />
```

Replace older Bootstrap or FontAwesome stylesheet references supplied by your project template with these links.

The `?v=2.4.1.0` query values correspond to Blazorise package version `2.4.1`. Use the four-part numeric version of your installed packages and update it when upgrading to refresh cached assets. Append `.0` to a three-part version or retain an existing fourth part. In Razor host files, you can inject `@inject Blazorise.IVersionProvider VersionProvider` and use `?v=@VersionProvider.Version` instead. Keep `@page` first in `_Host.cshtml`.

Blazorise loads its component JavaScript modules automatically. Keep your host's static asset setup enabled so package resources can be served. See the [integration guide](https://blazorise.com/docs/usage#javascript-resources) for troubleshooting and the [PWA guide](https://blazorise.com/docs/pwa) for offline support.

### 3. Add imports

Add this namespace to your application's `_Imports.razor`:

```razor
@using Blazorise
```

### 4. Register services

In `Program.cs`, add the namespaces and register Blazorise before `builder.Build()`:

```csharp
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

builder.Services
    .AddBlazorise()
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();
```

Register these services in the project where the components run. For Blazor Web Apps using Interactive WebAssembly or Auto with prerendering, register them in both the server and `.Client` projects.

### 5. Check your project's interactivity

Follow the guidance for your project type:

| Project type | Next step |
| --- | --- |
| Standalone Blazor WebAssembly | Continue to step 6. The app already runs interactively in the browser. |
| Blazor Server | Continue to step 6. The app already uses interactive server rendering. |
| Blazor Web App (.NET 8+) | Ensure the pages using Blazorise have an interactive render mode. If this is already configured, continue to step 6. |

For a Blazor Web App, the example below configures **Interactive Server** for the whole app. If you chose **Interactive WebAssembly** or **Auto**, follow Microsoft's [render mode guidance](https://learn.microsoft.com/aspnet/core/blazor/components/render-modes) for that mode and use the service registrations from step 4.

#### 5.1. Configure Interactive Server for a Blazor Web App

In the server project's `Program.cs`, update the existing Razor component service registration before `builder.Build()`:

```csharp
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
```

In the same file, update the existing endpoint mapping after `builder.Build()`:

```csharp
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
```

In the server project's `Components/App.razor`, set the render mode on `HeadOutlet` and `Routes`:

```razor
<HeadOutlet @rendermode="RenderMode.InteractiveServer" />
<Routes @rendermode="RenderMode.InteractiveServer" />
```

`RenderMode` is in the `Microsoft.AspNetCore.Components.Web` namespace, which the standard Blazor Web App template imports.

### 6. Try the components

Add a page such as `Profile.razor` to your application's pages folder. This example binds a labeled input to C# state and displays the saved name:

```razor
@page "/profile"

<Heading Size="HeadingSize.Is3">Profile</Heading>

<Field>
    <FieldLabel>Display name</FieldLabel>
    <TextInput @bind-Value="displayName" Placeholder="Ada Lovelace" />
    <FieldHelp>Choose the name shown on your profile.</FieldHelp>
</Field>

<Button Color="Color.Primary" Clicked="OnSaveHandler" Disabled="@string.IsNullOrWhiteSpace( displayName )">
    Save
</Button>

<Alert Color="Color.Success" Visible="@(!string.IsNullOrWhiteSpace( savedName ))" Margin="Margin.Is3.FromTop">
    Saved name: @savedName
</Alert>

@code
{
    private string displayName = string.Empty;
    private string savedName = string.Empty;

    private void OnSaveHandler()
    {
        savedName = displayName;
    }
}
```

`FieldLabel` and `FieldHelp` are associated with the input through Blazorise's [default accessibility integrations](https://blazorise.com/docs/usage#accessibility-defaults).

## Licensing

Blazorise is dual-licensed. Use is governed by [LICENSE.md](LICENSE.md) or a commercial license, depending on your scenario.

Commercial plans provide support and additional benefits according to the selected tier. [Themes](https://blazorise.com/themes) and [Blocks](https://blazorise.com/blocks) are available with Enterprise and Enterprise Plus. See [pricing and plan details](https://blazorise.com/pricing) for the current features and support terms.

## Local development

To build this checkout, install the .NET SDK used by [CI](.github/workflows/blazorise-ci-basic.yml), currently `11.0.100-rc.2.26504.105`.

Use the `DebugNet11` configuration to build libraries, providers, and extensions only for `net11.0`, with debugging symbols and hot reload support for demos. Generators, analyzers, and weavers retain their existing target frameworks. Standard `Debug` and `Release` configurations target .NET 8 through .NET 11.

From the repository root:

```powershell
dotnet build Blazorise.slnx -c DebugNet11

dotnet run --project Demos/Blazorise.Demo.Bootstrap5/Blazorise.Demo.Bootstrap5.csproj -c DebugNet11
```

The development framework is defined by `PrimaryTargetFramework` in [Directory.Build.props](Directory.Build.props). See [CONTRIBUTING.md](.github/CONTRIBUTING.md) for documentation and test workflows.

### Development packages

Development builds are published to the [MyGet feed](https://www.myget.org/gallery/blazorise) for trying upcoming changes and providing feedback. Use stable NuGet packages for production applications.

## Contributing

Contributions, bug reports, and feature requests are welcome. Read [CONTRIBUTING.md](.github/CONTRIBUTING.md) for development setup, branch selection, and the pull request workflow. The [contributing wiki](https://github.com/Megabit/Blazorise/wiki/Contributing) covers coding conventions.

Contributors must agree to the [Contributor License Agreement](https://gist.github.com/stsrki/abfa5ce0f4a5cf1e6ac67b92f8eb5d63), which is presented when a pull request is submitted.

## Supporting Blazorise

Blazorise is a source-available project, and its ongoing development is made possible through the support of its partners, subscribers, and community contributors.

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

## Copyright

Copyright (c) Megabit d.o.o.

Dual-licensed. See [LICENSE.md](LICENSE.md).