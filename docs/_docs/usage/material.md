---
title: "Material"
permalink: /docs/usage/material/
excerpt: "How to quickly install and setup Blazorise using material provider."
toc: true
toc_label: "Steps"
---

Since Material CSS is based on a Bootstrap you only need to change the css and js sources. The code in _ViewImports.cshtml_ will stay the same as in the [Bootstrap]({{ "/docs/usage/bootstrap/" | relative_url }}).

### Nuget

Install Material provider from nuget.

```
Install-Package Blazorise.Material
```

Also install icons

```
Install-Package Blazorise.Icons.Material
```

### Index

In your index.html just add 

```html
<!-- CSS -->
<!-- Add Material font (Roboto) and Material icon as needed -->
<link href="https://fonts.googleapis.com/css?family=Roboto:300,300i,400,400i,500,500i,700,700i|Roboto+Mono:300,400,700|Roboto+Slab:300,400,700" rel="stylesheet">
<link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

<!-- Optional JavaScript -->
<!-- jQuery first, then Popper.js, then Bootstrap JS -->
<script src="https://code.jquery.com/jquery-3.3.1.slim.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js"></script>
<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/js/bootstrap.min.js"></script>
```

### Imports

In your main _ViewImports.cshtml_ add:

```cs
@addTagHelper *, Blazorise

@using Blazorise
```

### Startup

In Startup.cs add:

```cs
using Blazorise;
using Blazorise.Material;

public void ConfigureServices( IServiceCollection services )
{
  services
    .AddMaterialProviders()
    .AddMaterialIcons();
}

public void Configure( IBlazorApplicationBuilder app )
{
  app
    .UseMaterialProviders()
    .UseMaterialIcons();

  app.AddComponent<App>( "app" );
}
```