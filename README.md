![Blazorise](docs/assets/images/blazorise.png)

# Components for Blazor

[![NuGet](https://img.shields.io/nuget/vpre/Blazorise.svg)](https://www.nuget.org/profiles/stsrki) ![Nuget](https://img.shields.io/nuget/dt/Blazorise.svg)
[![Join the chat at https://gitter.im/stsrki/Blazorise](https://badges.gitter.im/stsrki/Blazorise.svg)](https://gitter.im/stsrki/Blazorise?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Slack](https://img.shields.io/badge/chat-on%20Slack-green.svg)](https://join.slack.com/t/blazorise/shared_invite/enQtNjQ3NTcyNzM2OTk0LTIyNDBiYTRkNmI4NDU0NzU5NTEyOGFlMGE1ZmQ2YzI4ZTc1OWE3NDkwMGRjOGEwOTY2YjQ1NWQxZDhiN2Q1Yjk)
[![MIT](https://img.shields.io/github/license/stsrki/Blazorise.svg)](LICENSE)
[![Tip Me via PayPal](https://img.shields.io/badge/PayPal-tip%20me-green.svg?logo=paypal)](https://www.paypal.me/mladenmacanovic)

Blazorise is a component library built on top of [Blazor](https://blazor.net/) and CSS frameworks like Bootstrap, Bulma and Material.

## Demos

### Client-Side Blazor

- [Bootstrap Demo](https://bootstrapdemo.blazorise.com)
- [Material Demo](https://materialdemo.blazorise.com/)
- [Bulma Demo](https://bulmademo.blazorise.com/)
- [eFrolic Demo](https://efrolicdemo.blazorise.com/)

### Server-Side Razor Components

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

### NuGet packages

First step is to install a Bootstrap provider for Blazorise:

```
Install-Package Blazorise.Bootstrap
```

You also need to install the icon package:

```
Install-Package Blazorise.Icons.FontAwesome
```

### Source files

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

### Usings

In your main _Imports.razor add:

```cs
@using Blazorise
```

### Registrations

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

public void Configure( IComponentsApplicationBuilder app )
{
  app
    .UseBootstrapProviders()
    .UseFontAwesomeIcons();

  app.AddComponent<App>( "app" );
}
```

### Server-Side Blazor

This step is mandatory only for projects built on top of Server-Side Blazor(Razor Components)! For Client-Side Blazor projects this step is not required! Normally these files would be downloaded automatically by the framework but since Razor Components still doesn't support static files inside of class library you will need to manually include required js and css files into your project. Once the Blazor/RC team implements this feature this step will not we required.

First you must download **bundle.zip** from the [release](https://github.com/stsrki/Blazorise/releases) tab and extract it to your _wwwroot_ folder. After extraction you will have to include files in your Index.cshtml eg.

```
<link href="blazorise.css" rel="stylesheet" />
<link href="blazorise.bootstrap.css" rel="stylesheet" />
<link href="blazorise.sidebar.css" rel="stylesheet" />
<link href="blazorise.snackbar.css" rel="stylesheet" />

<script src="blazorise.js"></script>
<script src="blazorise.bootstrap.js"></script>
<script src="blazorise.charts.js"></script>
<script src="blazorise.sidebar.js"></script>

etc.
```

There is also another option. You can try the library [BlazorEmbedLibrary](https://github.com/SQL-MisterMagoo/BlazorEmbedLibrary). Full instruction on how to use it can be found on their project page.

## Other frameworks

To setup Blazorise for other css frameworks please refer the [Usage](https://blazorise.com/docs/usage/) page in the documentation.
