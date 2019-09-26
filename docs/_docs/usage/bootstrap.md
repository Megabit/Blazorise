---
title: "Bootstrap"
permalink: /docs/usage/bootstrap/
excerpt: "Learn all the steps on how to quickly install and setup Blazorise for Bootstrap css framework and FontAwesome icons."
toc: true
toc_label: "Steps"
---

**Note:** Before continuing please make sure that you already have a Blazor project created. If not please go to the [official Blazor site](https://blazor.net/docs/get-started.html){:target="_blank"} and learn how to create one.
{: .notice--info}

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

The next step is to change your `index.html` file and include the css and js source files:

```html
<!-- inside of head section -->
<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" integrity="sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T" crossorigin="anonymous">
<link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.4.1/css/all.css" integrity="sha384-5sAR7xN1Nv6T6+dT2mhtzEpVJvfS3NScPQTrOxhwjIuvcA67KV2R5Jz6kr4abQsz" crossorigin="anonymous">

<!-- inside of body section and after the <app> tag  -->
<script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js" integrity="sha384-UO2eT0CpHqdSJQ6hJty5KVphtPhzWj9WO1clHTMGa3JDZwrnQq4sF86dIHNDz0W1" crossorigin="anonymous"></script>
<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js" integrity="sha384-JjSmVgyd0p3pXB1rRibZUAYoIIy6OrQ6VrjIEaFf/nJGzIxFDsf4x0xIM+B07jRM" crossorigin="anonymous"></script>
```

**Note:** Don't forget to remove default **bootstrap** css and js files that comes with the Blazor/RC project template. If you forget to remove them it's possible that some of component will not work as they should be.
{: .notice--info}

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
      options.ChangeTextOnKeyPress = true;
    } ) // from v0.6.0-preview4
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
  ...
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