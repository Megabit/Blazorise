---
title: "Quick-Start Guide"
permalink: /docs/quick-start/
excerpt: "How to quickly install and setup Minimal Mistakes for use with GitHub Pages."
last_modified_at: 2018-11-25T22:21:33-05:00
redirect_from:
  - /theme-setup/
toc: true
toc_label: "Steps"
---

**Note:** Before continuing please make sure that you already have a Blazor project created. If not please go to the [official Blazor site](https://blazor.net/docs/get-started.html){:target="_blank"} and create one.
{: .notice}

### Installing NuGet packages

Blazorise is designed to work with different css frameworks. Each of the supported css framework is defined by a different nuget package. To install them you must run one of the following commands:

```
Install-Package Blazorise.Bootstrap
or
Install-Package Blazorise.Material
or
Install-Package Blazorise.Bulma
```

**Note:** Bulma provider is still work in progress so some of the features may not work.
{: .notice--warning}

## Usage

The setup process is the same for all supported frameworks, just replace Bootstrap with the one you need.

Now, for a bootstrap framework you must do:

### Index

in index.html add:

```html
<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css" integrity="sha384-MCw98/SFnGE8fJT3GXwEOngsV7Zt27NXFoaoApmYm81iuXoPkFOJwJ8ERdknLPMO" crossorigin="anonymous">
<link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.4.1/css/all.css" integrity="sha384-5sAR7xN1Nv6T6+dT2mhtzEpVJvfS3NScPQTrOxhwjIuvcA67KV2R5Jz6kr4abQsz" crossorigin="anonymous">

<script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js" integrity="sha384-ZMP7rVo3mIykV+2+9J3UJ46jBk0WLaUAdn689aCwoqbBJiSnjAK/l8WvCWPIPm49" crossorigin="anonymous"></script>
<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/js/bootstrap.min.js" integrity="sha384-ChfqqxuZUCnJSK3+MXmPNIyE6ZbWh2IMqE241rYiqJxyMiZ6OW/JmZQ5stwEULTy" crossorigin="anonymous"></script>
```

### Import

In your main _ViewImports.cshtml add:

```cs
@addTagHelper *, Blazorise

@using Blazorise
@using Blazorise.Bootstrap
```

### Startup

In Startup.cs add:

```cs
services
    .AddBootstrapProviders()
    .AddIconProvider( IconProvider.FontAwesome );
```

To setup Blazorise for other frameworks please refer the [Usage page](/docs/usage/)