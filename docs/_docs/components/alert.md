---
title: "Alert component"
permalink: /docs/components/alert/
excerpt: "Learn how to use Alert components."
toc: true
toc_label: "Guide"
---

## Overview

Alert component for feedback.

- `Alert` main container.
  - `AlertMessage` content of Alert.
  - `AlertDescription` additional content of Alert.
  - [`CloseButton`](/docs/components/close-button) an optional button to close the Alert.

### When to use

- When you need to show alert messages to users.
- When you need a persistent static container which is closable by user actions.

## Examples

### Basic usage

```html
<Alert Color="Color.Success">
    <AlertMessage>Well done!</AlertMessage>
    <AlertDescription>You successfully read this important alert message.</AlertDescription>
</Alert>
```

<iframe class="frame" src="/examples/elements/alert/" frameborder="0" scrolling="no" style="width:100%;height:60px;"></iframe>

### With close

You can also add a [`CloseButton`](/docs/components/close-button).

```html
<Alert Color="Color.Success" @bind-Visible="@visible">
    <AlertDescription>
        Lorem ipsum dolor sit amet, consectetur adipisicing elit.
    </AlertDescription>
    <AlertMessage>
        Alert Link.
    </AlertMessage>
    <CloseButton />
</Alert>
@code {
    bool visible = true;
}
```

<iframe class="frame" src="/examples/elements/alert-close/" frameborder="0" scrolling="no" style="width:100%;height:100px;"></iframe>

### With content

You can also add a [`CloseButton`](/docs/components/close-button).

```html
<Alert Color="Color.Info" @bind-Visible="@visible">
    <Heading Size="HeadingSize.Is4" TextColor="TextColor.Success">
        Big one!
        <CloseButton />
    </Heading>
    <Paragraph>
        Lorem ipsum dolor sit amet, consectetur adipisicing elit. Duis mollis, est non commodo luctus, nisi erat porttitor ligula, eget lacinia odio sem nec elit. Cras mattis consectetur purus sit amet fermentum.
    </Paragraph>
    <Paragraph>
        <Button Color="Color.Info">Wanna do this</Button>
        <Button Color="Color.Light">Or do this</Button>
    </Paragraph>
</Alert>
@code {
    bool visible = true;
}
```

<iframe class="frame" src="/examples/elements/alert-close-big/" frameborder="0" scrolling="no" style="width:100%;height:265px;"></iframe>

### How to use

To show alert just set `Visible` attribute to true.

```html
<Alert Color="Color.Success" @bind-Visible="@visible">
    ...
</Alert>
@code {
    bool visible = true;
}
```

or programmatically

```cs
<Alert @ref="myAlert" Color="Color.Success">
    ...
</Alert>

<Button Clicked="@OnButtonClick">Show alert</Button>

@code{
    Alert myAlert;

    void OnButtonClick()
    {
        myAlert.Show();
    }
}
```

## Functions

| Name         | Description                                                                                 |
|--------------|---------------------------------------------------------------------------------------------|
| Show()       | Makes the alert visible.                                                                    |
| Hide()       | Hides the alert.                                                                            |
| Toggle()     | Switches the alert visibility.                                                              |

## Attributes

| Name              | Type                                                         | Default          | Description                                                                                 |
|-------------------|--------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Dismisable        | boolean                                                      | false            | Enables the alert to be closed by placing the padding for close button.                     |
| Visible           | boolean                                                      | false            | Sets the alert visibility.                                                                  |
| VisibleChanged    | `EventCallback<bool>`                                        |                  | Occurs when the alert visibility changes.                                                   |
| Color             | [Colors]({{ "/docs/helpers/colors/#color" | relative_url }}) | `None`           | Component visual or contextual style variants.                                              |