---
title: "Bulma"
permalink: /docs/usage/bulma/
excerpt: "Learn all the steps on how to quickly install and setup Blazorise for Bulma CSS framework and FontAwesome icons."
toc: true
toc_label: "Steps"
---

**Note:** Bulma provider is still work in progress so some of the features may not work.
{: .notice--warning}

---

## Installations

### 1. NuGet packages

First step is to install a Bulma provider for Blazorise:

Install Bulma provider from NuGet.

```
Install-Package Blazorise.Bulma
```

You also need to install the icon package:

```
Install-Package Blazorise.Icons.FontAwesome
```

### 2. Source files

The next step is to change your `index.html` or `_Host.cshtml` file and include the CSS and JS source files:

```html
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bulma@0.9.1/css/bulma.min.css">
<link href="_content/Blazorise/blazorise.css" rel="stylesheet" />
<link href="_content/Blazorise.Bulma/blazorise.bulma.css" rel="stylesheet" />

<script defer src="https://use.fontawesome.com/releases/v5.3.1/js/all.js"></script>
<script src="_content/Blazorise/blazorise.js"></script>
<script src="_content/Blazorise.Bulma/blazorise.bulma.js"></script>
```

**Note:** When Blazor project is created it will also include it's own **Bootstrap** and **FontAwesome** files that can sometime be of older versions. To ensure we're using the appropriate bootstrap and FontAwesome files, you need remove them or replace them with the links from above. If you forget to remove them it's possible that some components will not work as expected.
{: .notice--info}

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
using Blazorise.Bulma;
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
      .AddBulmaProviders()
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
using Blazorise.Bulma;
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
      .AddBulmaProviders()
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