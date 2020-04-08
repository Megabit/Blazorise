---
title: "Figure component"
permalink: /docs/components/figure/
excerpt: "Learn how to use figure components with caption."
toc: true
toc_label: "Guide"
---

## Overview

Figures are used as container for responsive images.

### Structure

A figure structure is very simple:

- `<Figure>` the main container
  - `<FigureImage>` source image that needs to be displayed
  - `<FigureCaption>` a caption text bellow the image

## Usage

```html
<Figure Size="FigureSize.Is256x256">
    <FigureImage Source="assets/images/empty/256x256.png" AlternateText="256x256" />
    <FigureCaption>A caption for the above image.</FigureCaption>
</Figure>
```

<iframe src="/examples/figure/basic/" frameborder="0" scrolling="no" style="width:100%;height:300px;"></iframe>

## Attributes

### Figure

| Name       | Type                                                                    | Default  | Description                                          |
|------------|-------------------------------------------------------------------------|----------|------------------------------------------------------|
| Size       | [ButtonSize]({{ "/docs/helpers/sizes/#figuresize" | relative_url }})    | `None`   | Figure size variations.                              |

### FigureImage

| Name          | Type                                                                 | Default  | Description                                          |
|---------------|----------------------------------------------------------------------|----------|------------------------------------------------------|
| Source        | string                                                               |          | Image URL.                                           |
| AlternateText | string                                                               |          | Alternate text when image cannot be found.           |
| Rounded       | boolean                                                              | false    | Makes the figure border rounded.                     |