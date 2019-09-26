---
title: "Bulma"
permalink: /docs/usage/bulma/
excerpt: "Learn all the steps on how to quickly install and setup Blazorise for Bulma css framework and FontAwesome icons."
toc: true
toc_label: "Steps"
---

**Note:** Bulma provider is still work in progress so some of the features may not work.
{: .notice--warning}

---

## Installations

### 1. NuGet packages

First step is to install a Bulm provider for Blazorise:

Install Bulma provider from nuget.

```
Install-Package Blazorise.Bulma
```

You also need to install the icon package:

```
Install-Package Blazorise.Icons.FontAwesome
```

### 2. Source files

The next step is to change your `index.html` file and include the css and js source files:

```html
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bulma/0.7.2/css/bulma.min.css">
<script defer src="https://use.fontawesome.com/releases/v5.3.1/js/all.js"></script>
```

**Note:** Don't forget to remove default **bootstrap** css and js files that comes with the Blazor/RC project template. If you forget to remove them it's possible that some of component will not work as they should be.
{: .notice--info}

### 3. Usings

In your main _Imports.razor_ add:

```cs
@using Blazorise
```

### 4. Registrations

Finally in the Startup.cs you must tell the Blazor to register Bulma provider and extensions:

```cs
using Blazorise;
using Blazorise.Bulma;
using Blazorise.Icons.FontAwesome;

public void ConfigureServices( IServiceCollection services )
{
  services
    .AddBlazorise( options =>
    {
      options.ChangeTextOnKeyPress = true;
    } ) // from v0.6.0-preview4
    .AddBulmaProviders()
    .AddFontAwesomeIcons();
}
```

### 4.a Blazor WebAssembly

```cs
public void Configure( IComponentsApplicationBuilder app )
{
  app.Services
    .UseBulmaProviders()
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
    .UseBulmaProviders()
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
<link href="_content/Blazorise.Bulma/blazorise.bulma.css" rel="stylesheet" />

<script src="_content/Blazorise/blazorise.js"></script>
<script src="_content/Blazorise.Bulma/blazorise.bulma.js"></script>
```