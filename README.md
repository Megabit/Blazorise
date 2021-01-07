![Blazorise](docs/assets/images/blazorise.png)

# Components for Blazor

[![NuGet](https://img.shields.io/nuget/vpre/Blazorise.svg)](https://www.nuget.org/profiles/stsrki)
[![MyGet](https://img.shields.io/myget/blazorise/vpre/blazorise.svg?label=myget)](https://www.myget.org/gallery/blazorise)
![Nuget](https://img.shields.io/nuget/dt/Blazorise.svg)
[![Join the chat at https://gitter.im/stsrki/Blazorise](https://badges.gitter.im/stsrki/Blazorise.svg)](https://gitter.im/stsrki/Blazorise?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Slack](https://img.shields.io/badge/chat-on%20Slack-green.svg)](https://join.slack.com/t/blazorise/shared_invite/enQtNzQ2MjQxNDI4MzQxLThjZWM2YjRjMzg1OTlmMWY5NTBlNGRhYmQxOWZhY2Q2ZDcwYjRiMTQzZWZiOTAzMmE5YmNjNzMzYjY5YWRkZjg)
[![MIT](https://img.shields.io/github/license/stsrki/Blazorise.svg)](LICENSE)
[![Tip Me via PayPal](https://img.shields.io/badge/PayPal-tip%20me-green.svg?logo=paypal)](https://www.paypal.me/mladenmacanovic)
[![Buy me a Coffee](https://img.shields.io/badge/Buy%20me%20a%20coffee-donate-yellow.svg)](https://www.buymeacoffee.com/mladenmacanovic)
[![Patreon](https://img.shields.io/badge/Patreon-donate-yellow.svg)](https://www.patreon.com/mladenmacanovic)

Blazorise is a component library built on top of [Blazor](https://blazor.net/) and CSS frameworks like Bootstrap, Bulma and Material.

## Demos

### Blazor WebAssembly

- [Bootstrap Demo](https://bootstrapdemo.blazorise.com)
- [Material Demo](https://materialdemo.blazorise.com/)
- [Bulma Demo](https://bulmademo.blazorise.com/)
- [eFrolic Demo](https://efrolicdemo.blazorise.com/)
- [AntDesign Demo](https://antdesigndemo.blazorise.com/)

### Blazor Server

- [Bootstrap Demo](https://rcbootstrapdemo.blazorise.com/)

```
Note: This project is still experimental so it's possible that some components will be removed or refactored.
```

[Releases](https://blazorise.com/docs/releases/) and [Roadmap](https://github.com/stsrki/Blazorise/issues/304)

## Documentation

For full documentation, please visit the [Blazorise official documentation page](https://blazorise.com/docs/).

Continuing reading below for a quick start guide.

## Prerequisites

Before you continue, please make sure you have the latest version of Visual Studio and .Net Core installed. Visit an official [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/client) site to learn more.

## Installations

There are currently 5 different NuGet packages for each of the supported CSS frameworks. Available packages are:

```
- Blazorise.Bootstrap
- Blazorise.Bulma
- Blazorise.Material
- Blazorise.AntDesign
- Blazorise.Frolic
```

This guide will show you how to setup Blazorise with **Bootstrap** and **FontAwesome** icons. To setup Blazorise for other CSS frameworks, please refer the [Usage](https://blazorise.com/docs/usage/) page in the documentation.

### 1. NuGet packages

First step is to install a Bootstrap provider for Blazorise:

```
Install-Package Blazorise.Bootstrap
```

And FontAwesome icon package:

```
Install-Package Blazorise.Icons.FontAwesome
```

### 2. Source files

The next step is to define links to Bootstrap and FontAwesome _CSS_ or _JS_ files. If you're using **Blazor WebAssembly** project template, those links will go to the `index.html` located inside of `wwwroot` folder. Otherwise, if you're using a **Blazor Server** project template you will place the links into the `_Host.cshtml`.

In this step we're also going to define the links for Blazorise content files that comes with NuGet packages. You must follow the naming convention `_content/{LIBRARY.NAME}/{FILE.NAME}`. 

```html
<html>
<head>
  <!-- inside of head section -->
  <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css" integrity="sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk" crossorigin="anonymous">
  <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.12.0/css/all.css">

  <link href="_content/Blazorise/blazorise.css" rel="stylesheet" />
  <link href="_content/Blazorise.Bootstrap/blazorise.bootstrap.css" rel="stylesheet" />
</head>
<body>
  <div id="app"></div>

  <!-- inside of body section and after the div/app tag  -->
  <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js" integrity="sha384-DfXdz2htPH0lsSSs5nCTpuj/zy4C+OGpamoFVy38MVBnE+IbbVYUew+OrCXaRkfj" crossorigin="anonymous"></script>
  <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.0/dist/umd/popper.min.js" integrity="sha384-Q6E9RHvbIyZFJoft+2mJbHaEWldlvI9IOYy5n3zV9zzTtmI3UksdQRVvoxMfooAo" crossorigin="anonymous"></script>
  <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.min.js" integrity="sha384-OgVRvuATP1z7JjHLkuOU7Xw704+h835Lr+6QL9UvYjZE3Ipu6Tp75j7Bh/kR0JKI" crossorigin="anonymous"></script>

  <script src="_content/Blazorise/blazorise.js"></script>
  <script src="_content/Blazorise.Bootstrap/blazorise.bootstrap.js"></script>
</body>
</html>
```

---
**NOTE**
 When Blazor project is created it will also include it's own **Bootstrap** and **FontAwesome** files that can sometime be of older versions. To ensure we're using the appropriate Bootstrap and FontAwesome files, you need to remove them or replace them with the links from above. If you forget to remove them it's possible that some of components will not work as expected.

---

### 3. Using's

In your main `_Imports.razor` add:

```cs
@using Blazorise
```

### 4. Registrations

Starting from **.Net Core 3.2** there was some changes regarding the setup process for **Blazor WebAssembly** project types. Specifically the **Startup.cs** file is removed and all registrations are now done in the **Program.cs**.

---
Depending on the hosting model of your Blazor project you only need to apply either step **4.a** or **4.b**. You should not include both of them as that is generally not supported.

To Learn more about the different project types you can go to the official [documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-3.0).

---

#### 4.a Blazor WebAssembly

This step is mandatory for **Blazor WebAssembly**(client-side) and also for **ASP.NET Core hosted** project types. You should place the code into the **Program.cs** of your **client** project.

```cs
// other usings
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

public class Program
{
  public static async Task Main( string[] args )
  {
    var builder = WebAssemblyHostBuilder.CreateDefault( args );

    builder.Services
      .AddBlazorise( options =>
      {
          options.ChangeTextOnKeyPress = true;
      } )
      .AddBootstrapProviders()
      .AddFontAwesomeIcons();

    builder.Services.AddSingleton( new HttpClient
    {
      BaseAddress = new Uri( builder.HostEnvironment.BaseAddress )
    } );

    builder.RootComponents.Add<App>( "#app" );

    var host = builder.Build();

    await host.RunAsync();
  }
}
```

#### 4.b Blazor Server

This step is going only into the **Startup.cs** of your **Blazor Server** project.

```cs
// other usings
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

public class Startup
{
  public void ConfigureServices( IServiceCollection services )
  {
    services
      .AddBlazorise( options =>
      {
        options.ChangeTextOnKeyPress = true; // optional
      } )
      .AddBootstrapProviders()
      .AddFontAwesomeIcons();

      // other services
  }

  public void Configure( IComponentsApplicationBuilder app )
  {
    // other settings
    
    app.UseRouting();
    
    app.UseEndpoints( endpoints =>
    {
        endpoints.MapBlazorHub();
        endpoints.MapFallbackToPage( "/_Host" );
    } );
  }
}
```

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

## Try Preview

If you're willing to try preview versions of Blazorise all you need to do is to setup Visual Studio so it knows how to use Blazorise [MyGet feed](https://www.myget.org/feed/Details/blazorise). The easies way to do this is to create `NuGet.config` file and place it into your solution root folder. Then you copy the following content and paste it to the `NuGet.config`.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="BlazoriseMyGet" value="https://www.myget.org/F/blazorise/api/v3/index.json" />
  </packageSources>
</configuration>
```

Now you will be able to get preview versions of Blazorise with the latest changes and bug fixes.
