---
title: "List Group"
permalink: /docs/components/list-group/
excerpt: "Learn how to use ListGroup component."
toc: true
toc_label: "Guide"
---

List groups are a flexible and powerful component for displaying a series of content. Modify and extend them to support just about any content within.

## Examples

### Basic

The most basic list group is an unordered list with list items and the proper classes. Build upon it with the options that follow, or with your own CSS as needed.

```html
<ListGroup>
    <ListGroupItem>An item</ListGroupItem>
    <ListGroupItem>A second item</ListGroupItem>
    <ListGroupItem>A third item</ListGroupItem>
    <ListGroupItem>A fourth item</ListGroupItem>
    <ListGroupItem Disabled="true">A disabled item</ListGroupItem>
</ListGroup>
```

<iframe src="/examples/listgroup/basic/" frameborder="0" scrolling="no" style="width:100%;height:260px;"></iframe>

### Selectable items

Add `SelectedItem` to indicate the current active selection.

```html
<ListGroup Mode="ListGroupMode.Selectable" @bind-SelectedItem="selectedItem">
    <ListGroupItem Name="first">An item</ListGroupItem>
    <ListGroupItem Name="second">A second item</ListGroupItem>
    <ListGroupItem Name="third">A third item</ListGroupItem>
    <ListGroupItem Name="fourth">A fourth item</ListGroupItem>
    <ListGroupItem Name="fifth" Disabled="true">A disabled item</ListGroupItem>
</ListGroup>
@code {
    private string selectedItem = "first";
}
```

<iframe src="/examples/listgroup/selectable/" frameborder="0" scrolling="no" style="width:100%;height:260px;"></iframe>

### Flush

Add `Flush` to remove some borders and rounded corners to render list group items edge-to-edge in a parent container (e.g., cards).

```html
<ListGroup Flush="true">
    <ListGroupItem>An item</ListGroupItem>
    <ListGroupItem>A second item</ListGroupItem>
    <ListGroupItem>A third item</ListGroupItem>
    <ListGroupItem>A fourth item</ListGroupItem>
    <ListGroupItem Disabled="true">A disabled item</ListGroupItem>
</ListGroup>
```

<iframe src="/examples/listgroup/flush/" frameborder="0" scrolling="no" style="width:100%;height:260px;"></iframe>

### Contextual colors

Use contextual utilities to style list items with a stateful background and color.

```html
<ListGroup>
    <ListGroupItem Color="Color.None">None</ListGroupItem>
    <ListGroupItem Color="Color.Primary">Primary</ListGroupItem>
    <ListGroupItem Color="Color.Secondary">Secondary</ListGroupItem>
    <ListGroupItem Color="Color.Success">Success</ListGroupItem>
    <ListGroupItem Color="Color.Danger">Danger</ListGroupItem>
    <ListGroupItem Color="Color.Warning">Warning</ListGroupItem>
    <ListGroupItem Color="Color.Info">Info</ListGroupItem>
    <ListGroupItem Color="Color.Light">Light</ListGroupItem>
    <ListGroupItem Color="Color.Dark">Dark</ListGroupItem>
</ListGroup>
```

<iframe src="/examples/listgroup/contextual-colors/" frameborder="0" scrolling="no" style="width:100%;height:460px;"></iframe>

---

Contextual classes also work with `Selectable` items. Note the addition of the hover styles here not present in the previous example. Also supported is the `active` state; apply it to indicate an active selection on a contextual list group item.

```html
<ListGroup Mode="ListGroupMode.Selectable">
    <ListGroupItem Name="none" Color="Color.None">None</ListGroupItem>
    <ListGroupItem Name="primary" Color="Color.Primary">Primary</ListGroupItem>
    <ListGroupItem Name="secondary" Color="Color.Secondary">Secondary</ListGroupItem>
    <ListGroupItem Name="success" Color="Color.Success">Success</ListGroupItem>
    <ListGroupItem Name="danger" Color="Color.Danger">Danger</ListGroupItem>
    <ListGroupItem Name="warning" Color="Color.Warning">Warning</ListGroupItem>
    <ListGroupItem Name="info" Color="Color.Info">Info</ListGroupItem>
    <ListGroupItem Name="light" Color="Color.Light">Light</ListGroupItem>
    <ListGroupItem Name="dark" Color="Color.Dark">Dark</ListGroupItem>
</ListGroup>
```

<iframe src="/examples/listgroup/contextual-colors-selectable/" frameborder="0" scrolling="no" style="width:100%;height:460px;"></iframe>

### With badges

Add badges to any list group item to show unread counts, activity, and more with the help of some utilities.

```html
<ListGroup Flush="flush">
    <ListGroupItem Flex="Flex.JustifyContent.Between.AlignItems.Center">
        A list item
        <Badge Color="Color.Primary" Pill="true">14</Badge>
    </ListGroupItem>
    <ListGroupItem Flex="Flex.JustifyContent.Between.AlignItems.Center">
        A second list item
        <Badge Color="Color.Primary" Pill="true">2</Badge>
    </ListGroupItem>
    <ListGroupItem Flex="Flex.JustifyContent.Between.AlignItems.Center">
        A third list item
        <Badge Color="Color.Primary" Pill="true">1</Badge>
    </ListGroupItem>
</ListGroup>
```

<iframe src="/examples/listgroup/badges/" frameborder="0" scrolling="no" style="width:100%;height:160px;"></iframe>

### Custom content

Add nearly any HTML within, even for linked list groups like the one below, with the help of flexbox utilities.

```html
<ListGroup Flush="true">
    <ListGroupItem>
        <Div Flex="Flex.JustifyContent.Between" Width="Width.Is100">
            <Heading Size="HeadingSize.Is5" Margin="Margin.Is1.FromBottom">List group item heading</Heading>
            <Small>3 days ago</Small>
        </Div>
        <Paragraph Margin="Margin.Is1.FromBottom">Some placeholder content in a paragraph.</Paragraph>
        <Small>And some small print.</Small>
    </ListGroupItem>
    <ListGroupItem>
        <Div Flex="Flex.JustifyContent.Between" Width="Width.Is100">
            <Heading Size="HeadingSize.Is5" Margin="Margin.Is1.FromBottom">List group item heading</Heading>
            <Small TextColor="TextColor.Muted">3 days ago</Small>
        </Div>
        <Paragraph Margin="Margin.Is1.FromBottom">Some placeholder content in a paragraph.</Paragraph>
        <Small TextColor="TextColor.Muted">And some muted small print.</Small>
    </ListGroupItem>
    <ListGroupItem>
        <Div Flex="Flex.JustifyContent.Between" Width="Width.Is100">
            <Heading Size="HeadingSize.Is5" Margin="Margin.Is1.FromBottom">List group item heading</Heading>
            <Small TextColor="TextColor.Muted">3 days ago</Small>
        </Div>
        <Paragraph Margin="Margin.Is1.FromBottom">Some placeholder content in a paragraph.</Paragraph>
        <Small TextColor="TextColor.Muted">And some muted small print.</Small>
    </ListGroupItem>
</ListGroup>
```

<iframe src="/examples/listgroup/custom-content/" frameborder="0" scrolling="no" style="width:100%;height:340px;"></iframe>

## Attributes

### ListGroup

| Name                  | Type                                                                              | Default       | Description                                                                                                                    |
|-----------------------|-----------------------------------------------------------------------------------|---------------|--------------------------------------------------------------------------------------------------------------------------------|
| Flush                 | bool                                                                              | false         | Remove some borders and rounded corners to render list group items edge-to-edge in a parent container (e.g., cards).           |
| SelectedItem          | string                                                                            | null          | Gets or sets currently selected item name.                                                                                     |
| SelectedItemChanged   | `EventCallback<string>`                                                           |               | An event raised when SelectedItem is changed.                                                                                  |
| Mode                  | [`ListGroupMode`]({{ "/docs/helpers/enums/#listgroupmode" | relative_url }})      | `Static`      | Defines the list-group behavior mode.                                                                                          |

### ListGroupItem

| Name                  | Type                                                                              | Default       | Description                                                                                                                    |
|-----------------------|-----------------------------------------------------------------------------------|---------------|--------------------------------------------------------------------------------------------------------------------------------|
| Name                  | string                                                                            |               | Defines the item name.                                                                                                         |
| Disabled              | bool                                                                              | false         | Makes the item to make it appear disabled.                                                                                     |
| Clicked               | `EventCallback`                                                                   |               | Occurs when the item is clicked.                                                                                               |
| Color                 | [`Color`]({{ "/docs/helpers/colors/#color" | relative_url }})                     | `None`        | Gets or sets the list-group-item color.                                                                                        |