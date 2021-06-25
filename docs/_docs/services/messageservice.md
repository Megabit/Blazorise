---
title: "Message service"
permalink: /docs/services/message/
excerpt: "Learn how to use Message service."
toc: true
toc_label: "Guide"
---

## Overview

The `IMessageService` is a powerful helper utility built on top of `Modal` component and is used for showing the messages and confirmation dialogs to the user.

## Usage

### Wrapper

`IMessageService` is automatically registered by Blazorise but it needs just one thing on your side to make it work. You need to place `<MessageAlert />` somewhere in your application razor code. It can be placed anywhere, but a good approach is to place it in `App.razor` like in the following example.

```html
<Router AppAssembly="typeof(App).Assembly">
    ...
</Router>
<MessageAlert />
```

Once you're done you can start using it by injecting the `IMessageService` in your page and then simple calling the built-in methods.

### Basic example

```html
<Button Color="Color.Primary" Clicked="@ShowInfoMessage">Say hi!</Button>
<Button Color="Color.Danger" Clicked="@ShowConfirmMessage">Confirm</Button>

@code{
    [Inject] IMessageService MessageService { get; set; }

    Task ShowInfoMessage()
    {
        return MessageService.Info( "This is a simple info message!", "Hello" );
    }

    async Task ShowConfirmMessage()
    {
        if ( await MessageService.Confirm( "Are you sure you want to confirm?", "Confirmation" ) )
        {
            Console.WriteLine( "OK Clicked" );
        }
        else
        {
            Console.WriteLine( "Cancel Clicked" );
        }
    }
}
```