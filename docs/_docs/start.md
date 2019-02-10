---
title: "Start"
permalink: /docs/start/
excerpt: "Learn how to quickly install and setup Blazorise using Bootstrap css framework and FontAwesome icons extension."
redirect_from:
  - /theme-setup/
toc: true
toc_label: "Steps"
redirect_from: /docs/quick-start/
---

**Note:** Before continuing please make sure that you already have a Blazor project created. If not please go to the [official Blazor site](https://blazor.net/docs/get-started.html){:target="_blank"} and learn how to create one.
{: .notice--info}

Blazorise is designed to work with different css frameworks. Each of the supported css framework is defined by a different nuget package. Please see the [usage page]({{ "/docs/usage/" | relative_url }}) to find a list of supported frameworks and how to use them.

The setup process is simillar for all of the supported frameworks, you will just replace Bootstrap sources with the one you need.

## Installations

### NuGet packages

First step is to install a Bootstrap provider for Blazorise:

```
Install-Package Blazorise.Bootstrap
```

You also need to install the icon package:

```
Install-Package Blazorise.Icons.FontAwesome
```

### Sources files

The next step is to change your `index.html` file and include the css and js source files:

```html
<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.2.1/css/bootstrap.min.css" integrity="sha384-GJzZqFGwb1QTTN6wy59ffF1BuGJpLSa9DkKMp0DgiMDm4iYMj70gZWKYbI706tWS" crossorigin="anonymous">
<link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.4.1/css/all.css" integrity="sha384-5sAR7xN1Nv6T6+dT2mhtzEpVJvfS3NScPQTrOxhwjIuvcA67KV2R5Jz6kr4abQsz" crossorigin="anonymous">

<script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.6/umd/popper.min.js" integrity="sha384-wHAiFfRlMFy6i5SRaxvfOCifBUQy1xHdJ/yoi7FRNXMRBu5WHdZYu1hA6ZOblgut" crossorigin="anonymous"></script>
<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.2.1/js/bootstrap.min.js" integrity="sha384-B0UglyR+jN6CkvvICOB2joaf5I4l3gm9GU6Hc1og6Ls7i6U/mkkaduKaBhlAXv9k" crossorigin="anonymous"></script>
```

### Usings

In your main _ViewImports.cshtml add:

```cs
@addTagHelper *, Blazorise

@using Blazorise
```

### Registrations

Finally in the Startup.cs you must tell the Blazor to register Bootstrap provider and extensions:

```cs
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

public void ConfigureServices( IServiceCollection services )
{
  services
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();
}

public void Configure( IComponentsApplicationBuilder app )
{
  app
    .UseBootstrapProviders()
    .UseFontAwesomeIcons();

  app.AddComponent<App>( "app" );
}
```