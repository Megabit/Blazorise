---
title: "Page Progress service"
permalink: /docs/services/page-progress/
excerpt: "Learn how to use Page Progress service."
toc: true
toc_label: "Guide"
---

## Overview

The `IPageProgressService` is built on top of `PageProgress` component and is used to show small progress bar at the top of the page.

## Usage

### Wrapper

`IPageProgressService` is automatically registered by Blazorise but it needs just one thing on your side to make it work. You need to place `<PageProgressAlert />` somewhere in your application razor code. It can be placed anywhere, but a good approach is to place it in `App.razor` like in the following example.

```html
<Router AppAssembly="typeof(App).Assembly">
    ...
</Router>
<PageProgressAlert />
```

### Basic example

Once you're done you can start using it by injecting the `IPageProgressService` in your page and then simple calling the built-in methods.

```html
<Button Color="Color.Primary" Clicked="@SetPageProgress25">25 %</Button>
<Button Color="Color.Primary" Clicked="@SetPageProgress50">50 %</Button>
<Button Color="Color.Primary" Clicked="@SetPageProgress75">75 %</Button>
<Button Color="Color.Primary" Clicked="@SetPageProgress100">100 %</Button>

<Button Color="Color.Secondary" Clicked="@SetPageProgressIndeterminate">Indeterminate</Button>

<Button Color="Color.Secondary" Clicked="@SetPageProgressHidden">Hide</Button>

@code{
    [Inject] IPageProgressService PageProgressService { get; set; }

    Task SetPageProgress25()
    {
        return PageProgressService.Go( 25, options => { options.Color = Color.Warning; } );
    }

    Task SetPageProgress50()
    {
        return PageProgressService.Go( 50, options => { options.Color = Color.Warning; } );
    }

    Task SetPageProgress75()
    {
        return PageProgressService.Go( 75, options => { options.Color = Color.Warning; } );
    }

    Task SetPageProgress100()
    {
        return PageProgressService.Go( 100, options => { options.Color = Color.Warning; } );
    }

    Task SetPageProgressIndeterminate()
    {
        return PageProgressService.Go( null, options => { options.Color = Color.Warning; } );
    }

    Task SetPageProgressHidden()
    {
        // setting it to -1 will hide the progress bar
        return PageProgressService.Go( -1 );
    }
}
```