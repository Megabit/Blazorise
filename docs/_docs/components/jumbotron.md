---
title: "Jumbotron"
permalink: /docs/components/jumbotron/
excerpt: "Learn how to use jumbotron components."
toc: true
toc_label: "Guide"
---

Lightweight, flexible component for showcasing hero unit style content.

## Structure

- `<Jumbotron>` main container
  - `<JumbotronTitle>` main title text
  - `<JumbotronSubtitle>` text located bellow title to give more context about the jumbotron

## Example

```html
<Jumbotron Background="Background.Primary" Margin="Margin.Is4.FromBottom">
    <JumbotronTitle Size="JumbotronTitleSize.Is4">Hello, world!</JumbotronTitle>
    <JumbotronSubtitle>
        This is a simple hero unit, a simple jumbotron-style component for calling extra attention to featured content or information.
    </JumbotronSubtitle>
    <Divider></Divider>
    <Paragraph>
        It uses utility classes for typography and spacing to space content out within the larger container.
    </Paragraph>
</Jumbotron>
```

<iframe src="/examples/jumbotron/basic/" frameborder="0" scrolling="no" style="width:100%;height:380px;"></iframe>

## Attributes

| Name                  | Type                                                                       | Default          | Description                                                                                                   |
|-----------------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------------------------|
| Background            | [Background]({{ "/docs/helpers/colors/#background" | relative_url }})      | `None`           | Sets the bar background color.                                                                                |