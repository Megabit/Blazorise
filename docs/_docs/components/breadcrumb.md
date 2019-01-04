---
title: "Breadcrumbs"
permalink: /docs/components/breadcrumb/
excerpt: "Learn how to use breadcrumb components."
toc: true
toc_label: "Guide"
---

Breadcrumbs are used to indicate the current page's location. Add `IsActive` attribute to active `BreadcrumbItem`.

## Structure

- `<Breadcrumb>`
  - `<BreadcrumbItem>`
    - `<BreadcrumbLink>`

## Example

```html
<Breadcrumb>
    <BreadcrumbItem>
        <BreadcrumbLink To="#">Home</BreadcrumbLink>
    </BreadcrumbItem>
    <BreadcrumbItem>
        <BreadcrumbLink To="#">Library</BreadcrumbLink>
    </BreadcrumbItem>
    <BreadcrumbItem IsActive="true">
        <BreadcrumbLink To="#">Data</BreadcrumbLink>
    </BreadcrumbItem>
</Breadcrumb>
```

<iframe class="frame" src="/examples/elements/breadcrumb/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>


## Props

### Breadcrumb

`Breadcrumb` component itself doesn't have any specific public properties.

### BreadcrumbItem

| Name     | Type    | Default | Description                                     |
|----------|---------|---------|-------------------------------------------------|
| IsActive | boolean | false   | If set to true, renders  `span` instead of  `a` |