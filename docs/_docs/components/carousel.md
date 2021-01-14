---
title: "Carousel"
permalink: /docs/components/carousel/
excerpt: "Learn how to use carousel components."
toc: true
toc_label: "Guide"
---

A slideshow component for cycling through elements - images or slides of text - like a carousel.

## Structure

- `<Carousel>` main container
  - `<CarouselSlide>` wrapper for slide content

## Example

```html
<Carousel @bind-SelectedSlide="@selectedSlide">
    <CarouselSlide Name="1">
        <Image Source="..." Text="City Skyline" Display="Display.Block" Style="width: 100%;" />
    </CarouselSlide>
    <CarouselSlide Name="2">
        <Image Source="..." Text="Coffee" Display="Display.Block" Style="width: 100%;" />
    </CarouselSlide>
    <CarouselSlide Name="3">
        <Image Source="..." Text="Mountain" Display="Display.Block" Style="width: 100%;" />
    </CarouselSlide>
</Carousel>
@code{
    private string selectedSlide = "2";
}
```

<iframe src="/examples/carousel/basic/" frameborder="0" scrolling="no" style="width:100%;height:260px;"></iframe>

## Attributes

### Carousel

| Name                      | Type                                                                       | Default          | Description                                                                                                           |
|---------------------------|----------------------------------------------------------------------------|------------------|-----------------------------------------------------------------------------------------------------------------------|
| Autoplay                  | boolean                                                                    | true             | Autoplays the carousel slides from left to right.                                                                     |
| ShowIndicators            | boolean                                                                    | true             | Specifies whether to show an indicator for each slide.                                                                |
| ShowControls              | boolean                                                                    | true             | Specifies whether to show the controls that allows the user to navigate to the next or previous slide.                |
| SelectedSlide             | string                                                                     |                  | Gets or sets currently selected slide name.                                                                           |
| SelectedSlideChanged      | event                                                                      |                  | Occurs after the selected slide has changed.                                                                          |
| PreviousButtonLocalizer   | `TextLocalizerHandler`                                                     |                  | Function used to handle custom localization for previous button that will override a default `ITextLocalizer`.        |
| NextButtonLocalizer       | `TextLocalizerHandler`                                                     |                  | Function used to handle custom localization for next button that will override a default `ITextLocalizer`.            |