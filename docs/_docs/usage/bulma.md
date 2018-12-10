---
title: "Bulma"
permalink: /docs/usage/bulma/
excerpt: "Using bulma provider."
toc: true
toc_label: "Steps"
---

**Note:** Bulma provider is still work in progress so some of the features may not work.
{: .notice--warning}

---

### Nuget

Install Bulma provider from nuget.

```
Install-Package Blazorise.Bulma
```

### Index

```html
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bulma/0.7.2/css/bulma.min.css">
<script defer src="https://use.fontawesome.com/releases/v5.3.1/js/all.js"></script>
```

### Imports

In your main _ViewImports.cshtml_ add:

```cs
@addTagHelper *, Blazorise

@using Blazorise
@using Blazorise.Bulma
```

### Startup

In Startup.cs add:

```cs
services
    .AddBulmaProviders()
    .AddIconProvider( IconProvider.FontAwesome );
```