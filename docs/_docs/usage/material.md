---
title: "Material"
permalink: /docs/usage/material/
excerpt: "Learn all the steps on how to quickly install and setup Blazorise for Material CSS framework and material icons."
toc: true
toc_label: "Steps"
---

Since Material CSS is based on a Bootstrap you only need to change the CSS and JS sources. The code in _Imports.razor_ will stay the same as in the [Bootstrap]({{ "/docs/usage/bootstrap/" | relative_url }}).

## Installations

### 1. Nuget packages

First step is to install a Material provider for Blazorise:

```
Install-Package Blazorise.Material
```

You also need to install the icon package:

```
Install-Package Blazorise.Icons.Material
```

### 2. Download CSS

Material CSS is not available through the CDN so you must download it yourself from [djibe](https://djibe.github.io/material/) web page. After the download is finished just must extract the _CSS_ and _JS_ to the **wwwroot** folder inside of you Blazor project.

The folder structure should be:

```text
blazorproject.client/
└── wwwroot/
    ├── css/
    └── js/
```

### 3. Source files

The next step is to change your `index.html` or `_Host.cshtml` file and include the css and js source files:

```html
<!-- Material CSS -->
<link href="css/material.min.css" rel="stylesheet">

<!-- Add Material font (Roboto) and Material icon as needed -->
<link href="https://fonts.googleapis.com/css?family=Roboto:300,300i,400,400i,500,500i,700,700i|Roboto+Mono:300,400,700|Roboto+Slab:300,400,700" rel="stylesheet">
<link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

<link href="_content/Blazorise/blazorise.css" rel="stylesheet" />
<link href="_content/Blazorise.Material/blazorise.material.css" rel="stylesheet" />

<!-- Optional JavaScript -->
<!-- jQuery first, then Popper.js, then Material JS -->
<script src="https://code.jquery.com/jquery-3.3.1.slim.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js"></script>
<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/js/bootstrap.min.js"></script>
<script src="js/material.min.js"></script>

<script src="_content/Blazorise/blazorise.js"></script>
<script src="_content/Blazorise.Material/blazorise.material.js"></script>
```

**Note:** When Blazor project is created it will also include it's own **Bootstrap** and **FontAwesome** files that can sometime be of older versions. To ensure we're using the appropriate bootstrap and FontAwesome files, you need remove them or replace them with the links from above. If you forget to remove them it's possible that some components will not work as expected.
{: .notice--info}

### 4. Using's

In your main _Imports.razor add:

```cs
@using Blazorise
```

### 5. Registrations

Starting from **.Net Core 3.2** there was some changes regarding the setup process for **Blazor WebAssembly** project types. Specifically the **Startup.cs** file is removed and all registrations are now done in the **Program.cs**.

---
Depending on the hosting model of your Blazor project you only need to apply either step **5.a** or **5.b**. You should not include both of them as that is generally not supported.

To Learn more about the different project types you can go to the official [documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-models).

### 5.a Blazor WebAssembly

```cs
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
      .AddMaterialProviders()
      .AddMaterialIcons();

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

### 5.b Blazor Server

```cs
// other usings
using Blazorise;
using Blazorise.Material;
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
      .AddMaterialProviders()
      .AddMaterialIcons();

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