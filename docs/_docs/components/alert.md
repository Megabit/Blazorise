---
title: "Alert component"
permalink: /docs/components/alert/
excerpt: "Learn how to use Alert components."
toc: true
toc_label: "Guide"
---

## Alert

```html
<Alert Color="Color.Success">
    <strong>Well done!</strong> You successfully read this important alert message.
</Alert>
```

<iframe class="frame" src="/examples/elements/alert/" frameborder="0" scrolling="no" style="width:100%;height:60px;"></iframe>

### With close

You can also add a close button.

```html
<Alert Color="Color.Success">
    Lorem ipsum dolor sit amet, consectetur adipisicing elit. <strong>Alert Link.</strong>
    <CloseButton />
</Alert>
```

<iframe class="frame" src="/examples/elements/alert-close/" frameborder="0" scrolling="no" style="width:100%;height:60px;"></iframe>

### With content

You can also add a close button.

```html
<Alert Color="Color.Info" IsShow="true">
    <Heading Size="HeadingSize.Is4" TextColor="TextColor.Success">
        Big one!
        <CloseButton />
    </Heading>
    <Paragraph>
        Lorem ipsum dolor sit amet, consectetur adipisicing elit. Duis mollis, est non commodo luctus, nisi erat porttitor ligula, eget lacinia odio sem nec elit. Cras mattis consectetur purus sit amet fermentum.
    </Paragraph>
    <Paragraph>
        <SimpleButton Color="Color.Info">Wanna do this</SimpleButton>
        <SimpleButton Color="Color.Light">Or do this</SimpleButton>
    </Paragraph>
</Alert>
```

<iframe class="frame" src="/examples/elements/alert-close-big/" frameborder="0" scrolling="no" style="width:100%;height:215px;"></iframe>

### How to use

To show alert just set `IsShow` attribute to true.

```html
<Alert Color="Color.Success" IsShow="true">
    ...
</Alert>
```

or programatically

```cs
<Alert ref="myAlert" Color="Color.Success">
    ...
</Alert>

@code{
    Alert myAlert;

    void ButtonClick()
    {
        myAlert.Show();
    }
}
```