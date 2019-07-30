---
title: "Material"
permalink: /docs/usage/material/
excerpt: "Learn all the steps on how to quickly install and setup Blazorise for Material css framework and material icons."
toc: true
toc_label: "Steps"
---

Since Material CSS is based on a Bootstrap you only need to change the css and js sources. The code in _Imports.razor_ will stay the same as in the [Bootstrap]({{ "/docs/usage/bootstrap/" | relative_url }}).

## Installations

### Nuget packages

First step is to install a Material provider for Blazorise:

```
Install-Package Blazorise.Material
```

You also need to install the icon package:

```
Install-Package Blazorise.Icons.Material
```

### Download CSS

Material css is not available through the cdn so you must download it yourself from [daemonite](http://daemonite.github.io/material/) web page. After the download is finished just must extract the _css_ and _js_ to the **wwwrooot** folder inside of you Blazor project.

The folder structure should be:

```text
blazorproject.client/
└── wwwroot/
    ├── css/
    └── js/
```

### Source files

The next step is to change your `index.html` file and include the css and js source files:

```html
<!-- Material CSS -->
<link href="css/material.min.css" rel="stylesheet">

<!-- Add Material font (Roboto) and Material icon as needed -->
<link href="https://fonts.googleapis.com/css?family=Roboto:300,300i,400,400i,500,500i,700,700i|Roboto+Mono:300,400,700|Roboto+Slab:300,400,700" rel="stylesheet">
<link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

<!-- Optional JavaScript -->
<!-- jQuery first, then Popper.js, then Material JS -->
<script src="https://code.jquery.com/jquery-3.3.1.slim.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js"></script>
<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/js/bootstrap.min.js"></script>
<script src="js/material.min.js"></script>
```

**Note:** Don't forget to remove default **bootstrap** css and js files that comes with the Blazor/RC project template. If you forget to remove them it's possible that some of component will not work as they should be.
{: .notice--info}

### Usings

In your main _Imports.razor add:

```cs
@using Blazorise
```

### Registrations

Finally in the Startup.cs you must tell the Blazor to register Material provider and extensions:

```cs
using Blazorise;
using Blazorise.Material;
using Blazorise.Icons.Material;

public void ConfigureServices( IServiceCollection services )
{
  services
    .AddBlazorise( options =>
    {
      options.ChangeTextOnKeyPress = true;
    } ) // from v0.6.0-preview4
    .AddMaterialProviders()
    .AddMaterialIcons();
}
```

### Client-Side

```
public void Configure( IComponentsApplicationBuilder app )
{
  app.Services
    .UseMaterialProviders()
    .UseMaterialIcons();

  app.AddComponent<App>( "app" );
}
```

### Server-Side

```
public void Configure( IComponentsApplicationBuilder app )
{
  ...
  app.UseRouting();
  
  app.ApplicationServices
    .UseMaterialProviders()
    .UseMaterialIcons();

  app.UseEndpoints( endpoints =>
  {
      endpoints.MapBlazorHub();
      endpoints.MapFallbackToPage( "/_Host" );
  } );
}
```

This step is mandatory only for projects built on top of Razor Components! For Blazor projects this step is not required! Normally these files would be downloaded automatically by the framework but since Razor Components still doesn't support static files inside of class library you will need to manually include required js and css files into your project. Once the Blazor/RC team implements this feature this step will not we required.

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