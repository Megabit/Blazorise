---
title: "Start"
permalink: /docs/start/
excerpt: "How to quickly install and setup Blazorise."
redirect_from:
  - /theme-setup/
toc: true
toc_label: "Steps"
redirect_from: /docs/quick-start/
---

**Note:** Before continuing please make sure that you already have a Blazor project created. If not please go to the [official Blazor site](https://blazor.net/docs/get-started.html){:target="_blank"} and learn how to create one.
{: .notice--info}

### Installing NuGet packages

Blazorise is designed to work with different css frameworks. Each of the supported css framework is defined by a different nuget package. To install them you must run one of the following commands:

```
Install-Package Blazorise.Bootstrap
or
Install-Package Blazorise.Material
or
Install-Package Blazorise.Bulma
```

**Keep in mind:** Bulma provider is still work in progress so some of the features may not work.
{: .notice--warning}

## Usage

The setup process is the same for all supported frameworks, just replace Bootstrap with the one you need.

Now, for a bootstrap framework you must do:

### Index

in index.html add:

```html
<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.2.1/css/bootstrap.min.css" integrity="sha384-GJzZqFGwb1QTTN6wy59ffF1BuGJpLSa9DkKMp0DgiMDm4iYMj70gZWKYbI706tWS" crossorigin="anonymous">
<link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.4.1/css/all.css" integrity="sha384-5sAR7xN1Nv6T6+dT2mhtzEpVJvfS3NScPQTrOxhwjIuvcA67KV2R5Jz6kr4abQsz" crossorigin="anonymous">

<script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.6/umd/popper.min.js" integrity="sha384-wHAiFfRlMFy6i5SRaxvfOCifBUQy1xHdJ/yoi7FRNXMRBu5WHdZYu1hA6ZOblgut" crossorigin="anonymous"></script>
<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.2.1/js/bootstrap.min.js" integrity="sha384-B0UglyR+jN6CkvvICOB2joaf5I4l3gm9GU6Hc1og6Ls7i6U/mkkaduKaBhlAXv9k" crossorigin="anonymous"></script>
```

### Import

In your main _ViewImports.cshtml add:

```cs
@addTagHelper *, Blazorise

@using Blazorise
```

### Startup

In Startup.cs add:

```cs
using Blazorise.Bootstrap;

public void ConfigureServices( IServiceCollection services )
{
  services
    .AddBootstrapProviders()
    .AddIconProvider( IconProvider.FontAwesome );
}
```

To setup Blazorise for other frameworks please refer the [Usage page](/docs/usage/)