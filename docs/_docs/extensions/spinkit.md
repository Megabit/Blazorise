---
title: "SpinKit extension"
permalink: /docs/extensions/spinkit/
excerpt: "Learn how to use SpinKit component."
toc: true
toc_label: "Guide"
---

## Overview

A component used to show loading indicators animated with CSS.

## Installation

### NuGet

Install SpinKit extension from NuGet.

```
Install-Package Blazorise.SpinKit
```

### Imports

In your main _Imports.razor_ add:

```cs
@using Blazorise.SpinKit
```

### Static files

Include CSS link into your `index.html` or `_Host.cshtml` file, depending if you're using a Blazor WebAssembly or Blazor Server side project.

```html
<link href="_content/Blazorise.SpinKit/blazorise.spinkit.css" rel="stylesheet" />
```

## Usage

### Basic example

A basic spinner with default settings.

```html
<SpinKit Type="SpinKitType.Plane" />
```

### Color

The color can be changed with HEX value.

```html
<SpinKit Type="SpinKitType.Plane" Color="#ff4a3d" />
```

### Size

Size can be changed using any unit type. In this example we're using `px`.

```html
<SpinKit Type="SpinKitType.Plane" Size="20px" />
```

## Attributes

| Name                      | Type                                                                                     | Default      | Description                                                                                                                                      |
|---------------------------|------------------------------------------------------------------------------------------|--------------|--------------------------------------------------------------------------------------------------------------------------------------------------|
| Type                      | [SpinKitType]({{ "/docs/helpers/enums/#spinkittype" | relative_url }})                   | `Plane`      | Defines the spinner type.                                                                                                                        |
| Color                     | `string`                                                                                 | `null`       | Defines the custom spinner color.                                                                                                                |
| Size                      | `string`                                                                                 | `null`       | Defines the custom spinner size.                                                                                                                 |
| Centered                  | `bool`                                                                                   | false        | Position the spinner to the center of its container.                                                                                             |