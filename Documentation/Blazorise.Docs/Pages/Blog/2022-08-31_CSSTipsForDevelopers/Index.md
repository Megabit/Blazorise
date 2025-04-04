---
title: Practical CSS Tips For Developers
description: In this blog post we will learn some practical tips for applying custom CSS to Blazor and Blazorise components.
permalink: /blog/practical-css-tips-for-developers
canonical: /blog/practical-css-tips-for-developers
image-url: /img/blog/2022-08-31/css.png
image-text: Practical CSS Tips For Developers
author-name: James Amattey
author-image: james
posted-on: August1 31st, 2022
read-time: 3 min
---

# Practical CSS Tips For Developers

[Cascading Style Sheet](https://developer.mozilla.org/en-US/docs/Web/CSS), the beauty behind the web, friend to some, a devil to many but a necessary evil. Cascading Style Sheets, at their root, are intended to separate the style of a webpage from the content.

Whether you are using Blazorise with [Bootstrap](https://bootstrapdemo.blazorise.com/), [Bulma](https://bulmademo.blazorise.com/) or [AntDesign](https://antdesigndemo.blazorise.com/), CSS will be a huge part of your job. 

Fortunately, here are some great CSS tips for developers.

---

## Center anything in three lines of code

To center one `div` inside another, we make the containing `div` a flex container. Next, set `align-items` to center to perform centering on the block axis, and `justify-content` to center to perform centering on the inline axis.

In the future, we may be able to center elements without needing to turn the parent into a flex container, as the Box Alignment properties used here are specified to apply to block layout too.

The CSS `justify-content` property defines how the browser distributes space between and around content items along the main axis of a flex container, and the inline axis of a grid container.

The CSS `justify-items` property defines the default justify-self for all items of the box, giving them all a default way of justifying each box along the appropriate axis.

```css
.center {
    width: 300px;
    height: 400px;
    display: flex;
    justify-content: center;
    align-items: center;
}
```

---

## Resize an image to fit

Images can be a tough customer. When you create a container for a div, you usually want an image to fit in without stretching. Luckily, we can achieve that with 4 lines of code. 

Using a [Blazorise Card Component](docs/components/card), we will demonstrate how we fit an image with 4 lines of code. 

```html
<Card Margin="Margin.Is4.OnY">
    <CardImage Source="/img/gallery/7.jpg" Alt="Placeholder image" Class="card-image-full">
    </CardImage>
    <CardBody>
        <CardTitle Size="5">Card title</CardTitle>
        <CardText>
            Some quick example text to build on the card title and make up the bulk of the card's content.
        </CardText>
        <Button Color="Color.Primary">Button</Button>
    </CardBody>
</Card>
```

The `object-fit` CSS property sets how the content of a replaced element, such as an `img` should be resized to fit its container

```css
.card-image-full {
    height: 100%;
    width: 100%;
    object-fit: contain
}
```

---

## Truncate text with ellipsis

If you have a [TextEdit](docs/components/textedit) or [FieldLabel](docs/components/field) Component, you truncate the text content to limit the number of words so it does not expand beyond a certain limit. 

```css
.truncate {
    width: 250px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}
```

> Extra tip: You can use a toltip to display additional information that is required by the user. In the demo below, we use a tooltip to demonstrate how you can use the [Blazorise Tooltip](docs/components/tooltip) component to display additional information, without increasing the size of a widget. 

```html
<Div ElementId="tooltip-custom-target">
    <Field>
        <FieldLabel>Enter CVV</FieldLabel>
            <Tooltip Text="A three digit pin behind your credit card" TriggerTargetId="tooltip-custom-target" Inline>
                <Badge Color="Color.Warning">What's That?</Badge>
            </Tooltip>
        </FieldLabel>
    </Field>
</Div>
```

When you truncate a text, it means that you make it shorter so that it takes less resources to save or load. The process of truncating text is called Ellipsing and this is usually denoted by 3 dots at the end of the last text.

## Set limited content in paragraph

For a paragraph element, you can set the exact number of texts you want in the paragraph. This is an explicit form of truncating as this is more specific. If you set a paragraph to contain 5 letters, it will contain up to 5 letters and nothing more. 

Using the example from our truncating example, we can set the number of the CVV field to three, as most CVV codes are three digits

```css
.field-label {
    -webkit-line-clamp: 2;
}
```

The `-webkit-line-clamp` CSS property allows limiting of the contents of a block container to the specified number of lines.

---

## Conclusion

Blazorise is compatible with your favorite CSS frameworks such as [Bootstrap](https://bootstrapdemo.blazorise.com/), [AntDesign](https://antdesigndemo.blazorise.com/) and [Material](https://materialdemo.blazorise.com/). Read our [documentation](https://blazorise.com/docs/components) and learn how you can add Blazorise UI Component to your Blazor project for rapid application development.

In one of the next blog posts we will show you how to work with the [Blazorise Utilities](docs/helpers/utilities) instead of writing custom CSS rules.