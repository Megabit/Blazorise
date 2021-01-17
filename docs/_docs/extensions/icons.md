---
title: "Icons extension"
permalink: /docs/extensions/icons/
excerpt: "Learn how to use icons extension components."
toc: true
toc_label: "Guide"
---

## Overview

Icons extension is used to have a strongly typed list of icons.

## Installation

### NuGet

Install sidebar extension from NuGet.

```
Install-Package Blazorise.Icons.FontAwesome
```

### CSS

Include CSS link into your `index.html` or `_Host.cshtml` file, depending if you're using a Blazor WebAssembly or Blazor Server side project.

```html
<link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.12.0/css/all.css">
```

### Registrations

```diff
builder.Services
  .AddBlazorise()
  .AddBootstrapProviders()
+  .AddFontAwesomeIcons();
```

## Usage

### Basic

To define an icon it's simple as this.

```html
<Icon Name="IconName.Mail" />
```

### Custom

You can also use a real icon name instead of predefined enum.

```html
<Icon Name="@("fa-phone")" />
```

### Icon Names

Preferred way to define icon is to use an enum `IconName`. That way every icon will be applied automatically based on the icon package that you're using.

In case you cannot find an icon in the provided enum, you can also use prebuilt list of icon names that comes with every icon package. For example for font-awesome you would use `FontAwesomeIcons`, while for material that would be `MaterialIcons`.

```html
<Icon Name="FontAwesomeIcons.Announcement" />
```

### Style

By default all icons will have `Solid` style. To change it you can use one of the supported styles:

- `Solid`
- `Regular`
- `Light`
- `DuoTone`

```html
<Icon Name="IconName.Mail" IconStyle="IconStyle.Light" />
```

## Attributes

| Name              | Type                                                                    | Default  | Description                                                                   |
|-------------------|-------------------------------------------------------------------------|----------|-------------------------------------------------------------------------------|
| Name              | object                                                                  | null     | Icon name.                                                                    |
| IconStyle         | [IconStyle]({{ "/docs/helpers/enums/#iconstyle" | relative_url }})      | `Solid`  | Suggested icon style.                                                         |
