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
- `<CardDeck>`

## Examples

### Card

```html
<Card>
    <CardImage Source="/assets/images/gallery/9.jpg" Alt="Placeholder image">
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

<iframe src="/examples/cards/basic/" frameborder="0" scrolling="no" style="width:100%;height:625px;"></iframe>

### Card Deck

A set of equal width and height cards that aren’t attached to one another.

```html
<CardDeck>
    <Card>
        <CardImage Source="/assets/images/gallery/9.jpg" Alt="Card image cap 3"></CardImage>
        <CardBody>
            <CardTitle Size="5">Card title 1</CardTitle>
            <CardText>
                This is a longer card with supporting text below as a natural lead-in to additional content. This content is a little bit longer.
            </CardText>
            <Button Color="Color.Primary">Button</Button>
        </CardBody>
    </Card>
    <Card>
        <CardImage Source="/assets/images/gallery/9.jpg" Alt="Card image cap 9"></CardImage>
        <CardBody>
            <CardTitle Size="5">Card title 2</CardTitle>
            <CardText>
                This card has supporting text below as a natural lead-in to additional content.
            </CardText>
            <Button Color="Color.Primary">Button</Button>
        </CardBody>
    </Card>
    <Card>
        <CardImage Source="/assets/images/gallery/9.jpg" Alt="Card image cap 12"></CardImage>
        <CardBody>
            <CardTitle Size="5">Card title 3</CardTitle>
            <CardText>
                This is a wider card with supporting text below as a natural lead-in to additional content. This card has even longer content than the first to show that equal height action.
            </CardText>
            <Button Color="Color.Primary">Button</Button>
        </CardBody>
    </Card>
<CardDeck>
```

<iframe src="/examples/cards/carddeck/" frameborder="0" scrolling="no" style="width:100%;height:625px;"></iframe>

## Attributes

### Card

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| WhiteText     | boolean                                                                    | false            | Sets the white text when using the darker background.                                       |
| Background    | [Background]({{ "/docs/helpers/colors/#background" | relative_url }})      | `None`           | Sets the bar background color.                                                              |

### CardText

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Color         | [TextColor]({{ "/docs/helpers/colors/#textcolor" | relative_url }})        | `None`           | Sets the text color.                                                                        |
| Alignment     | [TextAlignment]({{ "/docs/helpers/enums/#textalignment" | relative_url }}) | `Left`           | Sets the text alignment.                                                                    |
| Transform     | [TextTransform]({{ "/docs/helpers/enums/#texttransform" | relative_url }}) | `None`           | Sets the text transformation.                                                               |
| Weight        | [TextWeight]({{ "/docs/helpers/enums/#textweight" | relative_url }})       | `None`           | Sets the text weight.                                                                       |
| Italic        | bool                                                                       | false            | Italicize text if set to true.                                                              |

### CardTitle

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Size          | int?                                                                       | null             | Number from 1 to 6 that defines the title size where the smaller number means larger text.  |
| Color         | [TextColor]({{ "/docs/helpers/colors/#textcolor" | relative_url }})        | `None`           | Sets the text color.                                                                        |
| Alignment     | [TextAlignment]({{ "/docs/helpers/enums/#textalignment" | relative_url }}) | `Left`           | Sets the text alignment.                                                                    |
| Transform     | [TextTransform]({{ "/docs/helpers/enums/#texttransform" | relative_url }}) | `None`           | Sets the text transformation.                                                               |
| Weight        | [TextWeight]({{ "/docs/helpers/enums/#textweight" | relative_url }})       | `None`           | Sets the text weight.                                                                       |
| Italic        | bool                                                                       | false            | Italicize text if set to true.                                                              |

### CardSubtitle

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Size          | int                                                                        | 6                | Number from 1 to 6 that defines the subtitle size where the smaller number means larger text.  |
| Color         | [TextColor]({{ "/docs/helpers/colors/#textcolor" | relative_url }})        | `None`           | Sets the text color.                                                                        |
| Alignment     | [TextAlignment]({{ "/docs/helpers/enums/#textalignment" | relative_url }}) | `Left`           | Sets the text alignment.                                                                    |
| Transform     | [TextTransform]({{ "/docs/helpers/enums/#texttransform" | relative_url }}) | `None`           | Sets the text transformation.                                                               |
| Weight        | [TextWeight]({{ "/docs/helpers/enums/#textweight" | relative_url }})       | `None`           | Sets the text weight.                                                                       |
| Italic        | bool                                                                       | false            | Italicize text if set to true.                                                              |

### CardLink

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Source        | string                                                                     | null             | Link URL.                                                                                   |
| Alt           | string                                                                     | null             | Alternative link text.                                                                      |

### CardImage

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Source        | string                                                                     | null             | Image URL.                                                                                  |
| Alt           | string                                                                     | null             | Alternative image text.                                                                     |