![Blazorise](docs/assets/images/blazorise.png)

# Components for Blazor

[![NuGet](https://img.shields.io/nuget/vpre/Blazorise.svg)](https://www.nuget.org/profiles/stsrki) ![Nuget](https://img.shields.io/nuget/dt/Blazorise.svg)
[![Join the chat at https://gitter.im/stsrki/Blazorise](https://badges.gitter.im/stsrki/Blazorise.svg)](https://gitter.im/stsrki/Blazorise?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Slack](https://img.shields.io/badge/chat-on%20Slack-green.svg)](https://join.slack.com/t/blazorise/shared_invite/enQtNzQ2MjQxNDI4MzQxLThjZWM2YjRjMzg1OTlmMWY5NTBlNGRhYmQxOWZhY2Q2ZDcwYjRiMTQzZWZiOTAzMmE5YmNjNzMzYjY5YWRkZjg)
[![MIT](https://img.shields.io/github/license/stsrki/Blazorise.svg)](LICENSE)
[![Tip Me via PayPal](https://img.shields.io/badge/PayPal-tip%20me-green.svg?logo=paypal)](https://www.paypal.me/mladenmacanovic)
[![Buy me a Coffee](https://img.shields.io/badge/Buy%20me%20a%20coffee-donate-yellow.svg)](https://www.buymeacoffee.com/mladenmacanovic)
[![Ko-fi](https://img.shields.io/badge/Ko--fi-donate-9cf.svg)](https://ko-fi.com/mladenmacanovic)

Blazorise is a component library built on top of [Blazor](https://blazor.net/) and CSS frameworks like Bootstrap, Bulma and Material.

## Demos

### Blazor WebAssembly

- [Bootstrap Demo](https://bootstrapdemo.blazorise.com)
- [Material Demo](https://materialdemo.blazorise.com/)
- [Bulma Demo](https://bulmademo.blazorise.com/)
- [eFrolic Demo](https://efrolicdemo.blazorise.com/)

### Blazor Server

- [Bootstrap Demo](https://rcbootstrapdemo.blazorise.com/)

```
Note: This project is still experimental so it's possible that some components will be removed or refactored.
```

[Releases](https://blazorise.com/docs/releases/) and [Roadmap](https://github.com/stsrki/Blazorise/issues/88)

## Prerequisites

Before you continue please make sure you have upgraded your project for the latest version of [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/client). Please look at the [official](https://devblogs.microsoft.com/aspnet/blazor-now-in-official-preview/) blog post to find all the required steps.

## Installations

There are 4 diferent NuGet packages for each of the supported CSS frameworks. Available packages are:

```
Install-Package Blazorise.Bootstrap
Install-Package Blazorise.Bulma
Install-Package Blazorise.Material
Install-Package Blazorise.Frolic
```

Choose one of them and modify your source files and your code accordingly. This guide will show you how to setup Blazorise with **Bootstrap** provider and FontAwesome icons.

### 1. NuGet packages

First step is to install a Bootstrap provider for Blazorise:

```
Install-Package Blazorise.Bootstrap
```

You also need to install the icon package:

```
Install-Package Blazorise.Icons.FontAwesome
```

### 2. Source files

The next step is to change your `index.html` file located in the _wwwroot_folder and include the following css and js source files:

```html
<!-- inside of head section -->
<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" integrity="sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T" crossorigin="anonymous">
<link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.4.1/css/all.css" integrity="sha384-5sAR7xN1Nv6T6+dT2mhtzEpVJvfS3NScPQTrOxhwjIuvcA67KV2R5Jz6kr4abQsz" crossorigin="anonymous">

<!-- inside of body section and after the <app> tag  -->
<script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js" integrity="sha384-UO2eT0CpHqdSJQ6hJty5KVphtPhzWj9WO1clHTMGa3JDZwrnQq4sF86dIHNDz0W1" crossorigin="anonymous"></script>
<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js" integrity="sha384-JjSmVgyd0p3pXB1rRibZUAYoIIy6OrQ6VrjIEaFf/nJGzIxFDsf4x0xIM+B07jRM" crossorigin="anonymous"></script>
```

---
**NOTE**
 Don't forget to remove default **bootstrap** css and js files that comes with the Blazor/RC project template. If you forget to remove them it's possible that some of component will not work as they should be.

---

### 3. Usings

In your main _Imports.razor add:

```cs
@using Blazorise
```

### 4. Registrations

Finally in the Startup.cs you must tell the Blazor to register Bootstrap provider and extensions:

```cs
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

public void ConfigureServices( IServiceCollection services )
{
  services
    .AddBlazorise( options =>
    {
      options.ChangeTextOnKeyPress = true; // optional
    } )
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();
}
```

### 4.a Blazor WebAssembly

```cs
public void Configure( IComponentsApplicationBuilder app )
{
  app.Services
    .UseBootstrapProviders()
    .UseFontAwesomeIcons();

  app.AddComponent<App>( "app" );
}
```

### 4.b Blazor Server

```cs
public void Configure( IComponentsApplicationBuilder app )
{
  // other settings
  
  app.UseRouting();
  
  app.ApplicationServices
    .UseBootstrapProviders()
    .UseFontAwesomeIcons();

  app.UseEndpoints( endpoints =>
  {
      endpoints.MapBlazorHub();
      endpoints.MapFallbackToPage( "/_Host" );
  } );
}
```

### 5. Static files

The final step is mandatory for all Blazor project types. Unlike in previous versions of Blazorise from now on you must set the path for static file manually. When consuming nuget packages that contains static files you must follow the convention `_content/{LIBRARY.NAME}/{FILE.NAME}`. So for this guide the required files are:

```html
<link href="_content/Blazorise/blazorise.css" rel="stylesheet" />
<link href="_content/Blazorise.Bootstrap/blazorise.bootstrap.css" rel="stylesheet" />

<script src="_content/Blazorise/blazorise.js"></script>
<script src="_content/Blazorise.Bootstrap/blazorise.bootstrap.js"></script>
```

> If you want to learn more about the reason behind this decision please look at the [oficial Blazor documentation.](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class?view=aspnetcore-3.0&tabs=visual-studio#consume-content-from-a-referenced-rcl)

## Usage

```html
@page "/counter"

<Heading Size="HeadingSize.Is1">Counter</Heading>

<Paragraph>Current count: @currentCount</Paragraph>

<Button Color="Color.Primary" Clicked="IncrementCount">Click me</Button>

@code {
    int currentCount = 0;

    void IncrementCount()
    {
        currentCount++;
    }
}
```

## Other frameworks

To setup Blazorise for other css frameworks please refer the [Usage](https://blazorise.com/docs/usage/) page in the documentation.
