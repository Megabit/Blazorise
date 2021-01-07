---
title: "AntDesign"
permalink: /docs/usage/ant-design/
excerpt: "Learn all the steps on how to quickly install and setup Blazorise for AntDesign CSS framework and FontAwesome icons."
toc: true
toc_label: "Steps"
---

## Installations

### 1. NuGet packages

First step is to install a AntDesign provider for Blazorise:

Install AntDesign provider from NuGet.

```
Install-Package Blazorise.AntDesign
```

You also need to install the icon package:

```
Install-Package Blazorise.Icons.FontAwesome
```

### 2. Source files

The next step is to change your `index.html` or `_Host.cshtml` file and include the CSS and JS source files:

```html
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/antd/4.0.0/antd.css" integrity="sha256-nzhI/tsi9npc5ir08wCgBpg43SEIrc7crRJLsHE0/60=" crossorigin="anonymous" />
<link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.12.0/css/all.css">

<link href="_content/Blazorise/blazorise.css" rel="stylesheet" />

<script src="_content/Blazorise/blazorise.js"></script>
<script src="_content/Blazorise.AntDesign/blazorise.antdesign.js"></script>
```

### 3. Using's

In your main _Imports.razor_ add:

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
using Blazorise.AntDesign;
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
      .AddAntDesignProviders()
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

### 4.b Blazor Server

```cs
// other usings
using Blazorise;
using Blazorise.AntDesign;
using Blazorise.Icons.FontAwesome;

public class Startup
{
  public void ConfigureServices( IServiceCollection services )
  {
    services
      .AddBlazorise( options =>
      {
        options.ChangeTextOnKeyPress = true;
      } )
      .AddAntDesignProviders()
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