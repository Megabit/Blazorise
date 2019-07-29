---
title: "Badge component"
permalink: /docs/components/badge/
excerpt: "Learn how to use badge components."
toc: true
toc_label: "Guide"
---

## Badge

Simply set the Color attribute and you're good to go.

```html
<Badge Color="Color.Primary">Primary</Badge>
<Badge Color="Color.Secondary">Secondary</Badge>
```

<iframe class="frame" src="/examples/elements/badge/" frameborder="0" scrolling="no" style="width:100%;height:35px;"></iframe>

## Attributes

| Name         | Type                                                         | Default          | Description                                                                                 |
|--------------|--------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| IsPill       | boolean                                                      | false            | Makes badges more rounded.                                                                  |
| Link         | string                                                       | null             | Create a badge link and provide actionable badges with hover and focus states.              |
| Color        | [Colors]({{ "/docs/helpers/colors/#color" | relative_url }}) | `None`           | Component visual or contextual style variants.                                              |