---
title: "Grid system"
permalink: /docs/components/grid/
excerpt: "Documentation and examples for grid layout system components."
toc: true
toc_label: "Guide"
---


## Grid
The grid system is used to help layout components with Blazorise. It is modelled after Bootstraps Grid system and uses a `flex` layout system.

### Rows & Columns

As the grid can be divided into 12 columns, there are size classes for each division.
These can be set using `ColumnSize` followed by `.Is1` -> `.Is12`.

```html
 <Row>
    <Column ColumnSize="ColumnSize.Is12">
        <Alert Color="Color.Primary" Visible="true">
            Is12
        </Alert>
    </Column>
</Row>
<Row>
    <Column ColumnSize="ColumnSize.Is8">
        <Alert Color="Color.Primary" Visible="true">
            Is8
        </Alert>
    </Column>
    <Column ColumnSize="ColumnSize.Is4">
        <Alert Color="Color.Secondary" Visible="true">
            Is4
        </Alert>
    </Column>
</Row>
```

### Offset

Move columns to the right using `WithOffset` attribute.

```html
<Row>
    <Column ColumnSize="ColumnSize.Is4">
        <Alert Color="Color.Primary" Visible="true">
            Is4
        </Alert>
    </Column>
    <Column ColumnSize="ColumnSize.Is4.WithOffset">
        <Alert Color="Color.Primary" Visible="true">
            Is4.WithOffset
        </Alert>
    </Column>
</Row>
<Row>
    <Column ColumnSize="ColumnSize.Is3.Is3.WithOffset">
        <Alert Color="Color.Primary" Visible="true">
            Is3.Is3.WithOffset
        </Alert>
    </Column>
    <Column ColumnSize="ColumnSize.Is3.Is3.WithOffset">
        <Alert Color="Color.Primary" Visible="true">
            Is3.Is3.WithOffset
        </Alert>
    </Column>
</Row>
```

### Gutter

Gutter can be used to set small spacing between `Columns` within a `Row`, without breaking the Grid wrapping rules (this is done by offsetting margins).

You can use it by setting the `Gutter` attribute on the `Row`. The `Columns` will automatically inherit this sapcing and apply it.

`Gutter` is a tuple, which is `(int Horizontal, int Vertical)` based off `pixel` spacing.

```html
<Row Gutter=(32, 16)>
    <Column ColumnSize="ColumnSize.Is8">
        <Alert Color="Color.Primary" Visible="true">
            I have padding
        </Alert>
    </Column>
    <Column ColumnSize="ColumnSize.Is4">
        <Alert Color="Color.Secondary" Visible="true">
            I also have padding
        </Alert>
    </Column>
</Row>
```
In this example, each `Column` will get `16px` of padding left and right, as well as `8px` of padding top and bottom. The `Row` will offset the margin accordingly.

## Containers

Containers can be used as an easy and helpful way to display content with some default padding and margins for a clean UI.

### Container

This container will be centered on desktop.

```html
<Container>
    <Alert Color="Color.Primary" Visible="true">
        Suspendisse vel quam malesuada, aliquet sem sit amet, fringilla elit. Morbi tempor tincidunt tempor. Etiam id turpis viverra, vulputate sapien nec, varius sem. Curabitur ullamcorper fringilla eleifend. In ut eros hendrerit est consequat posuere et at velit.
    </Alert>
</Container>
```

### Fluid Container

If you don't want to have a maximum width but want to keep the some margin on the left and right sides, add the `Fluid` modifier:

```html
<Container Fluid="true">
    <Alert Color="Color.Primary" Visible="true">
        Suspendisse vel quam malesuada, aliquet sem sit amet, fringilla elit. Morbi tempor tincidunt tempor. Etiam id turpis viverra, vulputate sapien nec, varius sem. Curabitur ullamcorper fringilla eleifend. In ut eros hendrerit est consequat posuere et at velit.
    </Alert>
</Container>
```

## Attributes

### Row

| Name              | Type          | Default | Description                                                                                                                     |
|-------------------|---------------|---------|---------------------------------------------------------------------------------------------------------------------------------|
| Gutter            | (int, int)    |         | Row grid spacing - we recommend setting Horizontal and/or Vertical it to (16 + 8n). (n stands for natural number.)              |

### Column

| Name              | Type          | Default | Description                                                                                                                     |
|-------------------|---------------|---------|---------------------------------------------------------------------------------------------------------------------------------|
| Gutter            | (int, int)    |         | Column grid spacing, we recommend setting it to (16 + 8n). (n stands for natural number.)                                       |

### Container

| Name              | Type          | Default | Description                                                                                                                     |
|-------------------|---------------|---------|---------------------------------------------------------------------------------------------------------------------------------|
| Fluid             | bool          | false   | Makes a full width container, spanning the entire width of the viewport.                                                        |