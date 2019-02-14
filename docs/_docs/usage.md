---
title: "Usage"
permalink: /docs/usage/
excerpt: "There are multiple available CSS providers for Blazorise that each have it's own set of rules and resources needed to be applied."
toc: true
toc_label: "Guide"
---

## Suppported CSS Providers

- [Bootstrap]({{ "/docs/usage/bootstrap/" | relative_url }})
- [Bulma]({{ "/docs/usage/bulma/" | relative_url }})
- [Material]({{ "/docs/usage/material/" | relative_url }})

**Note:** Bulma provider is still work in progress so some of the features may not work.
{: .notice--warning}

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