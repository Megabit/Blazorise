---
title: "Buttons"
permalink: /docs/components/buttons/
excerpt: "Buttons."
toc: true
---

The button is an essential element of any design. It's meant to look and behave as an interactive element of your page.

## Single button

To create a basic button you need to use a SimpleButton component.

```html
<SimpleButton>Click me</SimpleButton>
```

### Colored buttons

To define button color use a `Color` attribute.

```html
<SimpleButton Color="Color.Primary">PRIMARY</SimpleButton>
<SimpleButton Color="Color.Secondary">SECONDARY</SimpleButton>
```

To find the list of supported colors please look at the [colors]({{ "/docs/helpers/colors/" | relative_url }}) page.

### Block button

```html
<SimpleButton IsBlock="true">Button</SimpleButton>
```

### Active button

```html
<SimpleButton IsActive="true">Button</SimpleButton>
```

### Disabled button

```html
<SimpleButton IsDisabled="true">Button</SimpleButton>
```


## Button group

If you want to group buttons together on a single line, use the `Buttons` tag.

```html
<Buttons>
    <SimpleButton Color="Color.Secondary">LEFT</SimpleButton>
    <SimpleButton Color="Color.Secondary">CENTER</SimpleButton>
    <SimpleButton Color="Color.Secondary">RIGHT</SimpleButton>
</Buttons>
```