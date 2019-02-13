---
title: "Material"
permalink: /docs/usage/material/
excerpt: "Learn all the steps on how to quickly install and setup Blazorise for Material css framework and material icons."
toc: true
toc_label: "Steps"
---

Since Material CSS is based on a Bootstrap you only need to change the css and js sources. The code in _ViewImports.cshtml_ will stay the same as in the [Bootstrap]({{ "/docs/usage/bootstrap/" | relative_url }}).

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

### Sources files

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

### Usings

In your main _ViewImports.cshtml add:

```cs
@addTagHelper *, Blazorise

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
    .AddMaterialProviders()
    .AddMaterialIcons();
}

public void Configure( IComponentsApplicationBuilder app )
{
  app
    .UseMaterialProviders()
    .UseMaterialIcons();

  app.AddComponent<App>( "app" );
}
```
