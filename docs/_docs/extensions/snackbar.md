---
title: "Snackbar extension"
permalink: /docs/extensions/snackbar/
excerpt: "Learn how to use Snackbar component."
toc: true
toc_label: "Guide"
---

## Overview

Snackbar provide brief messages about app processes. The component is also known as a toast.

## Structure

The snackbar extension is defined of several different components:

- `Snackbar` main snackbar component
  - `SnackbarBody` container for the snackbar content
  - `SnackbarAction` snackbar action button

## Installation

### NuGet

Install snackbar extension from NuGet.

```
Install-Package Blazorise.Snackbar
```

### Imports

In your main _Imports.razor_ add:

```cs
@using Blazorise.Snackbar
```

### Static files

Include CSS link into your `index.html` or `_Host.cshtml` file, depending if you're using a Blazor WebAssembly or Blazor Server side project.

```html
<link href="_content/Blazorise.Snackbar/blazorise.snackbar.css" rel="stylesheet" />
```

## Usage

### Basic example

A basic snackbar that aims to reproduce standard snackbar behavior.

```html
<Button Clicked="@(()=>snackbar.Show())">Snackbar</Button>

<Snackbar @ref="snackbar">
  <SnackbarBody>
    Single line of text directly related to the operation performed
  </SnackbarBody>
</Snackbar>

@code{
    Snackbar snackbar;
}
```

### Variant snackbars

You can also define variant [colors]({{ "/docs/helpers/colors/#snackbarcolor" | relative_url }}) to override default snackbar style.

```html
<Button Color="Color.Primary" Clicked="@(()=>snackbarPrimary.Show())">Primary</Button>
<Button Color="Color.Secondary" Clicked="@(()=>snackbarSecondary.Show())">Secondary</Button>

<Snackbar @ref="snackbarPrimary" Color="SnackbarColor.Primary">
  <SnackbarBody>
    Single line of text directly related to the operation performed
    <SnackbarAction Clicked="@(()=>snackbarPrimary.Hide())">ACTION</SnackbarAction>
  </SnackbarBody>
</Snackbar>
<Snackbar @ref="snackbarSecondary" Color="SnackbarColor.Secondary">
  <SnackbarBody>
    Single line of text directly related to the operation performed
    <SnackbarAction Clicked="@(()=>snackbarSecondary.Hide())">ACTION</SnackbarAction>
  </SnackbarBody>
</Snackbar>
```

### Stacked snackbars

When you want to show multiple snackbars stacked on top of each other you can use a wrapper component `SnackbarStack`.

```html
<Button Color="Color.Primary" Clicked="@(()=>snackbarStack.PushAsync("Current time is: " + DateTime.Now, SnackbarColor.Info))">Primary</Button>

<Button Color="Color.Info" Clicked="@(()=>snackbarStack.PushAsync("Some info message! Timeout: " + intervalBeforeMsgClose, SnackbarColor.Info, options => { IntervalBeforeClose = intervalBeforeMsgClose; } ))">Show Info</Button>

<SnackbarStack @ref="snackbarStack" Location="SnackbarStackLocation.Right" />
@code{
    SnackbarStack snackbarStack;
    int intervalBeforeMsgClose = 2000;
}
```

## Attributes

| Name                      | Type                                                                                     | Default      | Description                                                                                                                                      |
|---------------------------|------------------------------------------------------------------------------------------|--------------|--------------------------------------------------------------------------------------------------------------------------------------------------|
| Location                  | [SnackbarLocation]({{ "/docs/helpers/enums/#snackbarlocation" | relative_url }})         | `None`       | Defines the snackbar location.                                                                                                                   |
| Color                     | [SnackbarColor]({{ "/docs/helpers/colors/#snackbarcolor" | relative_url }})              | `None`       | Defines the snackbar color.                                                                                                                      |
| Visible                   | bool                                                                                     | false        | Defines the visibility of snackbar.                                                                                                              |
| Multiline                 | bool                                                                                     | false        | Allow snackbar to show multiple lines of text.                                                                                                   |
| DefaultInterval           | double                                                                                   | 5000         | Defines the interval (in milliseconds) after which the snackbar will be automatically closed.                                                    |
| DelayCloseOnClick         | bool                                                                                     | false        | If clicked on snackbar, a close action will be delayed by increasing the DefaultInterval time (used if no value is provided in the Push method). |
| DelayCloseOnClickInterval | double                                                                                   | 'null'       | Defines the interval(in milliseconds) by which the snackbar will be delayed from closing.                                                        |
| Closed                    | `EventCallback<SnackbarClosedEventArgs>`                                                 |              | Occurs after the snackbar has closed.                                                                                                            |