---
title: "Breadcrumbs"
permalink: /docs/components/breadcrumb/
excerpt: "Learn how to use breadcrumb components."
toc: true
toc_label: "Guide"
---

Breadcrumbs are used to indicate the current page's location. Add `Active` attribute to active `BreadcrumbItem`.

## Structure

- `<Breadcrumb>`
  - `<BreadcrumbItem>`
    - `<BreadcrumbLink>`

## Example

### Manual mode

This is the default mode. Meaning you need to set `BreadcrumbItem.Active` property implicitly.

```html
<Breadcrumb>
    <BreadcrumbItem>
        <BreadcrumbLink To="#">Home</BreadcrumbLink>
    </BreadcrumbItem>
    <BreadcrumbItem>
        <BreadcrumbLink To="#">Library</BreadcrumbLink>
    </BreadcrumbItem>
    <BreadcrumbItem Active="true">
        <BreadcrumbLink To="#">Data</BreadcrumbLink>
    </BreadcrumbItem>
</Breadcrumb>
```

<iframe class="frame" src="/examples/elements/breadcrumb/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Auto mode

In this mode breadcrumb items will respond to navigation changes and will be activates automatically.

```html
<Breadcrumb Mode="BreadcrumbMode.Auto">
    <BreadcrumbItem>
        <BreadcrumbLink To="">Home</BreadcrumbLink>
    </BreadcrumbItem>
    <BreadcrumbItem>
        <BreadcrumbLink To="account">Account</BreadcrumbLink>
    </BreadcrumbItem>
    <BreadcrumbItem>
        <BreadcrumbLink To="account/settings">Settings</BreadcrumbLink>
    </BreadcrumbItem>
</Breadcrumb>
```

## Attributes

### Breadcrumb

| Name       | Type                                                                          | Default | Description                                                          |
|------------|-------------------------------------------------------------------------------|---------|----------------------------------------------------------------------|
| Mode       | [BreadcrumbMode]({{ "/docs/helpers/enums/#breadcrumbmode" | relative_url }})  | `None`  | Defines the breadcrumb items activation mode.                        |


### BreadcrumbItem

| Name     | Type    | Default | Description                                     |
|----------|---------|---------|-------------------------------------------------|
| Active | boolean | false   | If set to true, renders  `span` instead of  `a` |

### BreadcrumbLink

| Name       | Type                                                        | Default | Description                                                              |
|------------|-------------------------------------------------------------|---------|--------------------------------------------------------------------------|
| To         | string                                                      | null    | Path to the destination page.                                            |
| Target     | [Target]({{ "/docs/helpers/enums/#target" | relative_url }})| `None`  | The target attribute specifies where to open the linked document.        |
| Match      | [Match]({{ "/docs/helpers/enums/#match" | relative_url }})  | `All`   | URL matching behavior for a link.                                        |
| Title      | string                                                      | null    | Defines the title of a link, which appears to the user as a tooltip.     |