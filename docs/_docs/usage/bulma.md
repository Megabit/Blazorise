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

Install Bulma provider from nuget.

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
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bulma/0.7.2/css/bulma.min.css">
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