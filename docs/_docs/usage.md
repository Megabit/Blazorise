---
title: "Usage"
permalink: /docs/usage/
excerpt: "Available CSS providers for Blazorise and how to use them."
toc: true
toc_label: "Guide"
---

## CSS Providers

List of supported providers

- [Bootstrap]({{ "/docs/usage/bootstrap/" | relative_url }})
- [Bulma]({{ "/docs/usage/bulma/" | relative_url }})
- [Material]({{ "/docs/usage/material/" | relative_url }})

### Empty provider

**New:** version 0.5.0
{: .notice--info}

Generally you will always want to use one of the provided CSS frameworks. But in the case that you only want to use custom Blazorise extension like Chart or Sidebar you can register an "Empty" provider. This way the extensions will still work but the default Blazorise components will be unused.

```cs
public void ConfigureServices( IServiceCollection services )
{
  services
    .AddEmptyProviders();
}
```