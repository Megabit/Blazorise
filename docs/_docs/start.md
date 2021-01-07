---
title: "Start"
permalink: /docs/start/
excerpt: "Learn how to quickly install and setup Blazorise using Bootstrap CSS framework and FontAwesome icons extension."
redirect_from:
  - /theme-setup/
toc: true
toc_label: "Steps"
redirect_from: /docs/quick-start/
---

**Note:** Before continuing please make sure that you already have a Blazor project created. If not please go to the [official Blazor site](https://blazor.net/docs/get-started.html){:target="_blank"} and learn how to create one.
{: .notice--info}

Blazorise is designed to work with different CSS frameworks. Each of the supported CSS framework is defined by a different NuGet package. Please see the [usage page]({{ "/docs/usage/" | relative_url }}) to find a list of supported frameworks and how to use them.

The setup process is similar for all of the supported frameworks, you will just replace Bootstrap sources with the one you need.

## Installations

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

The next step is to change your `index.html` or `_Host.cshtml` file and include the CSS and JS source files:

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

**Note:**  When Blazor project is created it will also include it's own **Bootstrap** and **FontAwesome** files that can sometime be of older versions. To ensure we're using the appropriate bootstrap and FontAwesome files, you need remove them or replace them with the links from above. If you forget to remove them it's possible that some components will not work as expected.
{: .notice--info}

### 3. Using's

In your main _Imports.razor add:

```cs
@using Blazorise
```

### 4. Registrations

Starting from **.Net Core 3.2** there was some changes regarding the setup process for **Blazor WebAssembly** project types. Specifically the **Startup.cs** file is removed and all registrations are now done in the **Program.cs**.

---
Depending on the hosting model of your Blazor project you only need to apply either step **4.a** or **4.b**. You should not include both of them as that is generally not supported.

To Learn more about the different project types you can go to the official [documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-models).

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