---
layout: default
title: Material
parent: Usage
nav_order: 2
---

### Material

---

Since Material CSS is based on a Bootstrap you only need to change the css and js sources. The code in _ViewImports.cshtml_ will stay the same as in the [Bootstrap]({{ site.baseurl }}{% link docs/usage/bootstrap.md %}).

1. Install Material provider from nuget.

```
Install-Package Blazorise.Material
```

2. In your index.html just add 

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

3. In Startup.cs add:

```cs
services
    .AddMaterialProviders()
    .AddIconProvider( IconProvider.FontAwesome );
```
