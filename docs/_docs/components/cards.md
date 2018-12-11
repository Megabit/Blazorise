---
title: "Cards"
permalink: /docs/components/cards/
excerpt: "Cards."
toc: true
---

### Cards

The card component comprises several elements that you can mix and match:

- `Card`
  - `CardHeader`
    - `CardTitle`
    - `CardSubtitle`
  - `CardImage`
  - `CardBody`
  - `CardFooter`

```html
<Card>
    <CardImage Source="assets/images/gallery/9.jpg" Alt="Placeholder image">
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