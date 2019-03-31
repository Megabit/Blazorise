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
    } ) // from v0.6.0-preview4
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

### Razor Components

Since Razor Components still doesn't support static files inside of class library you will need to manually add required js and css files. First you must download **bundle.zip** from the release tab and extract it to your _wwwroot_ folder. After extraction you will have to include files in your Index.cshtml eg.

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