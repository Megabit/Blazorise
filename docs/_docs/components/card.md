---
title: "Card component"
permalink: /docs/components/card/
excerpt: "Learn how to use cards."
toc: true
toc_label: "Guide"
redirect_from: /docs/components/cards/
---

## Structure

The card component comprises several elements that you can mix and match:

- `<Card>`
  - `<CardHeader>`
  - `<CardImage>`
  - `<CardBody>`
    - `<CardTitle>`
    - `<CardSubtitle>`
    - `<CardText>`
  - `<CardFooter>`
- `<CardGroup>`

## Example

```html
<Card>
    <CardImage Source="/assets/images/gallery/9.jpg" Alt="Placeholder image">
    </CardImage>
    <CardBody>
        <CardTitle Size="5">Card title</CardTitle>
        <CardText>
            Some quick example text to build on the card title and make up the bulk of the card's content.
        </CardText>
        <SimpleButton Color="Color.Primary">Button</SimpleButton>
    </CardBody>
</Card>
```

<iframe src="/examples/cards/basic/" frameborder="0" scrolling="no" style="width:100%;height:625px;"></iframe>

## Attributes

### Card

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| IsWhiteText   | boolean                                                                    | false            | Sets the white text when using the darker background.                                       |
| Background    | [Background]({{ "/docs/helpers/colors/#background" | relative_url }})      | `None`           | Sets the bar background color.                                                              |

### CardTitle

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Size          | int?                                                                       | null             | Number from 1 to 6 that defines the title size where the smaller number means larger text.  |

### CardSubtitle

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Size          | int                                                                        | 6                | Number from 1 to 6 that defines the subtitle size where the smaller number means larger text.  |

### CardLink

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Source        | string                                                                     | null             | Link url.                                                                                   |
| Alt           | string                                                                     | null             | Alternative link text.                                                                      |

### CardImage

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Source        | string                                                                     | null             | Image url.                                                                                  |
| Alt           | string                                                                     | null             | Alternative image text.                                                                     |