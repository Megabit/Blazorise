---
title: "Notification service"
permalink: /docs/services/notification/
excerpt: "Learn how to use Notification service."
toc: true
toc_label: "Guide"
---

## Overview

The `INotificationService` is built on top of `Snackbar` component and is used for showing simple alerts and notifications.

## Installation

Notification service is part of the **Blazorise.Components** NuGet package.
{: .notice--info}

### NuGet

Install extension from NuGet.

```
Install-Package Blazorise.Components
```

## Usage

### Wrapper

`INotificationService` is automatically registered by Blazorise but it needs just one thing on your side to make it work. You need to place `<NotificationAlert />` somewhere in your application razor code. It can be placed anywhere, but a good approach is to place it in `App.razor` like in the following example.

```html
<Router AppAssembly="typeof(App).Assembly">
    ...
</Router>
<NotificationAlert DefaultInterval="10000" />
```

### Basic example

Once you're done you can start using it by injecting the `INotificationService` in your page and then simple calling the built-in methods.

```html
<Button Color="Color.Warning" Clicked="@ShowWarningNotification">Show alert!</Button>

@code{
    [Inject] INotificationService NotificationService { get; set; }

    Task ShowWarningNotification()
    {
        return NotificationService.Warning( "This is a simple notification message!", "Hello" );
    }
}
```