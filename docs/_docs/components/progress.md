---
title: "Progress component"
permalink: /docs/components/progress/
excerpt: "Learn how to use progress component."
toc: true
toc_label: "Guide"
---

Use our progress component for displaying simple or complex progress bars, featuring support for horizontally stacked bars, animated backgrounds, and text labels.

## Structure

- `<Progress>` main component for stacked progress bars
  - `<ProgressBar>` progress bar for single progress value

## Basic

Progress components are built with two components.

- We use the `Progress` as a wrapper to indicate the max value of the progress bar.
- We use the inner `ProgressBar` to indicate the progress so far.

### Single bar

Put that all together, and you have the following examples.

```html
<Progress>
    <ProgressBar Value="25" />
</Progress>
```

<iframe src="/examples/progress/basic/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Multiple bars

Include multiple `ProgressBar` sub-components in a `Progress` component to build a horizontally stacked set of progress bars.

```html
<Progress>
    <ProgressBar Value="15" />
    <ProgressBar Background="Background.Success" Value="30" />
    <ProgressBar Background="Background.Info" Value="20" />
</Progress>
```

<iframe src="/examples/progress/multiple/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Page progress

You can also show a small progress bar at the top of the page. Note that unlike regular `Progress` component, for `PageProgress` you must set the `Visible` parameter to make it active.

### Basic

```html
<PageProgress Visible="true" Value="25" />
```

<iframe src="/examples/progress/page-progress/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Indeterminate

To make an indeterminate progress bar, simply remove `Value` or make it a `null`.

```html
<PageProgress Visible="true" />
```

<iframe src="/examples/progress/page-progress-indeterminate/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Attributes

### Progress

| Name                  | Type                                                                   | Default          | Description                                                                                      |
|-----------------------|------------------------------------------------------------------------|------------------|--------------------------------------------------------------------------------------------------|
| Size                  | [Size]({{ "/docs/helpers/sizes/#size" | relative_url }})               | `None`   	    | Progress size variations.                                                                        |

### ProgressBar

| Name                  | Type                                                                   | Default          | Description                                                                                      |
|-----------------------|------------------------------------------------------------------------|------------------|--------------------------------------------------------------------------------------------------|
| Value                 | `int?`                                                                 | null   	        | The progress value.                                                                              |
| Min                   | int                                                                    | 0                | Minimum value of the progress bar.                                                               |
| Max                   | int                                                                    | 100              | Maximum value of the progress bar.                                                               |
| Background            | [Background]({{ "/docs/helpers/colors/#background" | relative_url }})  | `None`           | Defines the progress bar background color.                                                       |
| Striped               | bool                                                                   | false            | Set to true to make the progress bar stripped.                                                   |
| Animated              | bool                                                                   | false            | Set to true to make the progress bar animated.                                                   |

### PageProgress

| Name                  | Type                                                                   | Default          | Description                                                                                      |
|-----------------------|------------------------------------------------------------------------|------------------|--------------------------------------------------------------------------------------------------|
| Value                 | `int?`                                                                 | null   	        | The progress value.                                                                              |
| Color                 | [Color]({{ "/docs/helpers/colors/#color" | relative_url }})            | `None`           | Defines the progress bar color.                                                                  |
| Visible               | bool                                                                   | false            | Defines the visibility of progress bar.                                                          |
