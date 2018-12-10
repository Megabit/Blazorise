---
title: "Buttons"
permalink: /docs/components/buttons/
excerpt: "Buttons."
toc: true
---

## Single button

To create a basic button you need to use a SimpleButton component.

```html
<SimpleButton>Click me</SimpleButton>
```

### Colored buttons

To define button color use a Color attribute.

```html
<SimpleButton Color="Color.Primary">PRIMARY</SimpleButton>
<SimpleButton Color="Color.Secondary">SECONDARY</SimpleButton>
```

To find the list of supported colors please look at the [utilities]({{ "/docs/utilities/" | relative_url }}).

## Button group

```html
<Buttons>
    <SimpleButton Color="Color.Secondary">LEFT</SimpleButton>
    <SimpleButton Color="Color.Secondary">CENTER</SimpleButton>
    <SimpleButton Color="Color.Secondary">RIGHT</SimpleButton>
</Buttons>
```