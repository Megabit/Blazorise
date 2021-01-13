---
title: "Close Button component"
permalink: /docs/components/close-button/
excerpt: "Learn how to use Close Button component."
toc: true
toc_label: "Guide"
---

The `CloseButton` is a simple button that highlights to the user that a part of the current UI can be dismissed such as an [`Alert`](/docs/components/Alert) or a [`Modal`](/docs/components/Modal). When used in an [`Alert`](/docs/components/Alert) or a [`Modal`](/docs/components/Modal) the component will be dismissed, if not inside one of these components the `Clicked` EventCallback must be set for it to be useful.

## Close Button

```html
<Alert Visible="true">
    ...
    <CloseButton />
</Alert>
```

<iframe src="/examples/elements/close" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Usage with other components
```html
@if (_visible)
{
    <div>
        ...
        <CloseButton Clicked="@OnClicked" />
    </div>
}
@code {
    bool _visible = true;

    void OnClicked()
    {
        _visible = false;
    }
}
```

## Attributes

| Name                    | Type                                                                       | Default      | Description                                                                           |
|-------------------------|----------------------------------------------------------------------------|--------------|---------------------------------------------------------------------------------------|
| Clicked                 | `EventCallback`                                                            |              | Occurs when the Button is clicked     .                                               |
