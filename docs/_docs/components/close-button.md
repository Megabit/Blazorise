---
title: "CloseButton component"
permalink: /docs/components/close-button/
excerpt: "Learn how to use CloseButton component."
toc: true
toc_label: "Guide"
---

The `CloseButton` is a simple button that highlights to the user that a part of the current UI can be dismissed such as an [`Alert`](/docs/components/Alert) or a [`Modal`](/docs/components/Modal). When used in an [`Alert`](/docs/components/Alert) or a [`Modal`](/docs/components/Modal) the component will be dismissed, if not inside one of these components the `Clicked` EventCallback must be set for it to be useful.

## Examples

### With manual close

```html
<Alert Visible="@visible">
    ...
    <CloseButton Clicked="@OnClicked" />
</Alert>
@code {
    bool visible = true;

    Task OnClicked()
    {
        visible = false;

        return Task.CompletedTask;
    }
}
```

<iframe src="/examples/elements/close" frameborder="0" scrolling="no" style="width:100%;height:80px;"></iframe>

### With auto close

```html
<Alert @bind-Visible="@visible">
    ...
    <CloseButton AutoClose="true" />
</Alert>
@code {
    bool visible = true;
}
```

<iframe src="/examples/elements/close" frameborder="0" scrolling="no" style="width:100%;height:80px;"></iframe>

### Usage with other components

```html
@if ( visible )
{
    <div>
        ...
        <CloseButton Clicked="@OnClicked" />
    </div>
}
@code {
    bool visible = true;

    Task OnClicked()
    {
        visible = false;

        return Task.CompletedTask;
    }
}
```

## Attributes

| Name                    | Type                                                                       | Default      | Description                                                                           |
|-------------------------|----------------------------------------------------------------------------|--------------|---------------------------------------------------------------------------------------|
| Clicked                 | `EventCallback`                                                            |              | Occurs when the Button is clicked.                                                    |
| AutoClose               | bool                                                                       | `true`       | If true, the parent Alert or Modal with be automatically closed.                      |
