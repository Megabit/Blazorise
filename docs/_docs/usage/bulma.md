---
title: "Bulma"
permalink: /docs/usage/bulma/
excerpt: "How to quickly install and setup Blazorise using bulma provider."
toc: true
toc_label: "Steps"
---

**Note:** Bulma provider is still work in progress so some of the features may not work.
{: .notice--warning}

---

## Installations

### NuGet packages

First step is to install a Bulm provider for Blazorise:

Install Bulma provider from nuget.

```
Install-Package Blazorise.Bulma
```

You also need to install the icon package:

```
Install-Package Blazorise.Icons.FontAwesome
```

### Sources files

The next step is to change your `index.html` file and include the css and js source files:

```html
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bulma/0.7.2/css/bulma.min.css">
<script defer src="https://use.fontawesome.com/releases/v5.3.1/js/all.js"></script>
```

### Usings

In your main _ViewImports.cshtml_ add:

```cs
@addTagHelper *, Blazorise

@using Blazorise
```

### Registrations

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
    } )
    .AddBulmaProviders()
    .AddFontAwesomeIcons();
}

public void Configure( IComponentsApplicationBuilder app )
{
  app
    .UseBulmaProviders()
    .UseFontAwesomeIcons();

  app.AddComponent<App>( "app" );
}
```