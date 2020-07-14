---
title: "Typography"
permalink: /docs/components/typography/
excerpt: "Documentation and examples for typography, including paragraphs, headings, text, and more."
toc: true
toc_label: "Guide"
---

Documentation and examples for typography, including paragraphs, headings, text, and more.

## Text

Displays a simple static text on a page.

```html
<Text Color="TextColor.Primary">
    Lorem ipsum dolor sit amet.
</Text>
<Text Color="TextColor.Secondary">
    Cursus euismod quis viverra nibh cras.
</Text>
```

<iframe src="/examples/typography/text/" frameborder="0" scrolling="no" style="width:100%;height:55px;"></iframe>

## Paragraph

Block of text separated from adjacent blocks by blank lines and/or first-line indentation.

```html
<Paragraph>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Cras fermentum odio eu feugiat pretium nibh ipsum consequat. Pulvinar etiam non quam lacus suspendisse faucibus interdum posuere. Nunc sed velit dignissim sodales ut. Mi bibendum neque egestas congue quisque egestas diam in arcu. Quis vel eros donec ac odio tempor. Fermentum posuere urna nec tincidunt praesent. Eget velit aliquet sagittis id consectetur. Molestie at elementum eu facilisis sed odio morbi quis commodo. Ut consequat semper viverra nam libero justo laoreet sit.
</Paragraph>
<Paragraph>
    Cursus euismod quis viverra nibh cras. Fermentum posuere urna nec tincidunt praesent semper feugiat nibh. Varius quam quisque id diam vel quam. Eget sit amet tellus cras adipiscing enim eu turpis. In est ante in nibh mauris cursus mattis. Interdum velit laoreet id donec ultrices tincidunt. Sollicitudin aliquam ultrices sagittis orci a. Turpis egestas sed tempus urna et pharetra pharetra. Felis bibendum ut tristique et egestas quis ipsum suspendisse. Ipsum dolor sit amet consectetur adipiscing elit ut. Enim eu turpis egestas pretium aenean pharetra. Diam sit amet nisl suscipit adipiscing bibendum est. Turpis massa sed elementum tempus egestas. Accumsan in nisl nisi scelerisque eu ultrices vitae. Purus ut faucibus pulvinar elementum integer. Id interdum velit laoreet id donec ultrices tincidunt arcu. Aliquam vestibulum morbi blandit cursus risus at ultrices.
</Paragraph>
```

<iframe src="/examples/typography/paragraph/" frameborder="0" scrolling="no" style="width:100%;height:460px;"></iframe>

## Headings

```html
<Heading Size="HeadingSize.Is1">h1. Blazorise heading</Heading>
<Heading Size="HeadingSize.Is2">h2. Blazorise heading</Heading>
<Heading Size="HeadingSize.Is3">h3. Blazorise heading</Heading>
<Heading Size="HeadingSize.Is4">h4. Blazorise heading</Heading>
<Heading Size="HeadingSize.Is5">h5. Blazorise heading</Heading>
<Heading Size="HeadingSize.Is6">h6. Blazorise heading</Heading>
```

<iframe src="/examples/typography/heading/" frameborder="0" scrolling="no" style="width:100%;height:240px;"></iframe>

## Display Headings

```html
<DisplayHeading Size="DisplayHeadingSize.Is1">Display 1</DisplayHeading>
<DisplayHeading Size="DisplayHeadingSize.Is2">Display 2</DisplayHeading>
<DisplayHeading Size="DisplayHeadingSize.Is3">Display 3</DisplayHeading>
<DisplayHeading Size="DisplayHeadingSize.Is4">Display 4</DisplayHeading>
```

<iframe src="/examples/typography/displayheading/" frameborder="0" scrolling="no" style="width:100%;height:420px;"></iframe>

## Attributes

### Text

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Color         | [TextColor]({{ "/docs/helpers/colors/#textcolor" | relative_url }})        | `None`           | Sets the text color.                                                                        |
| Alignment     | [TextAlignment]({{ "/docs/helpers/enums/#textalignment" | relative_url }}) | `Left`           | Sets the text alignment.                                                                    |
| Transform     | [TextTransform]({{ "/docs/helpers/enums/#texttransform" | relative_url }}) | `None`           | Sets the text transformation.                                                               |
| Weight        | [TextWeight]({{ "/docs/helpers/enums/#textweight" | relative_url }})       | `None`           | Sets the text weight.                                                                       |
| Italic        | bool                                                                       | false            | Italicize text if set to true.                                                              |

### Paragraph

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Color         | [TextColor]({{ "/docs/helpers/colors/#textcolor" | relative_url }})        | `None`           | Sets the text color.                                                                        |
| Alignment     | [TextAlignment]({{ "/docs/helpers/enums/#textalignment" | relative_url }}) | `Left`           | Sets the text alignment.                                                                    |
| Transform     | [TextTransform]({{ "/docs/helpers/enums/#texttransform" | relative_url }}) | `None`           | Sets the text transformation.                                                               |
| Weight        | [TextWeight]({{ "/docs/helpers/enums/#textweight" | relative_url }})       | `None`           | Sets the text weight.                                                                       |
| Italic        | bool                                                                       | false            | Italicize text if set to true.                                                              |

### Heading

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Size          | [HeadingSize]({{ "/docs/helpers/enums/#headingsize" | relative_url }})     | `Is3`            | Sets the heading size.                                                                      |
| Color         | [TextColor]({{ "/docs/helpers/colors/#textcolor" | relative_url }})        | `None`           | Sets the heading text color.                                                                |

### DisplayHeading

| Name          | Type                                                                                     | Default          | Description                                                                                 |
|---------------|------------------------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Size          | [DisplayHeadingSize]({{ "/docs/helpers/enums/#displayheadingsize" | relative_url }})     | `Is2`            | Sets the display heading size.                                                              |