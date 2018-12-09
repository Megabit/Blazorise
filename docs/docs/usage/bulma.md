---
layout: default
title: Bulma
parent: Usage
nav_order: 2
---

### Bulma
1. Install Bulma provider from nuget.
```markdown
Install-Package Blazorise.Bulma
```

2. in index.html add:
```html
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bulma/0.7.2/css/bulma.min.css">
<script defer src="https://use.fontawesome.com/releases/v5.3.1/js/all.js"></script>
```

3. In your main _ViewImports.cshtml_ add:
```markdown
@addTagHelper *, Blazorise

@using Blazorise
@using Blazorise.Bulma
```

4. In Startup.cs add:
```markdown
services
    .AddBulmaProviders()
    .AddIconProvider( IconProvider.FontAwesome );
```
