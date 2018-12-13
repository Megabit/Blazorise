---
title: "Buttons"
permalink: /docs/components/buttons/
excerpt: "Buttons."
toc: true
toc_label: "Components"
---

The button is an essential element of any design. It's meant to look and behave as an interactive element of your page.

## Basic button

To create a basic button you need to use a SimpleButton component.

```html
<SimpleButton>Click me</SimpleButton>
```

<iframe src="/examples/buttons/basic/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Colored

To define button color use a `Color` attribute.

```html
<SimpleButton Color="Color.Primary">Primary</SimpleButton>
<SimpleButton Color="Color.Secondary">Secondary</SimpleButton>
<SimpleButton Color="Color.Warning">Warning</SimpleButton>
<SimpleButton Color="Color.Danger">Danger</SimpleButton>
```

<iframe src="/examples/buttons/colors/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

**Note:** To find the list of supported colors please look at the [colors]({{ "/docs/helpers/colors/" | relative_url }}) page.
{: .notice--info}

### Outlined

To define button color use a `Color` attribute.

```html
<SimpleButton Color="Color.Primary" IsOutline="true">Primary</SimpleButton>
<SimpleButton Color="Color.Secondary" IsOutline="true">Secondary</SimpleButton>
<SimpleButton Color="Color.Warning" IsOutline="true">Warning</SimpleButton>
<SimpleButton Color="Color.Danger" IsOutline="true">Danger</SimpleButton>
```

<iframe src="/examples/buttons/outlined/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Blocked

```html
<SimpleButton Color="Color.Primary" IsBlock="true">Blocked primary</SimpleButton>
<SimpleButton Color="Color.Secondary" IsBlock="true">Blocked secondary</SimpleButton>
```

<iframe src="/examples/buttons/block/" frameborder="0" scrolling="no" style="width:100%;height:95px;"></iframe>

### Active

```html
<SimpleButton IsActive="true">Primary</SimpleButton>
<SimpleButton IsActive="true">Secondary</SimpleButton>
```

<iframe src="/examples/buttons/active/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Disabled

```html
<SimpleButton IsDisabled="true">Primary</SimpleButton>
<SimpleButton IsDisabled="true">Secondary</SimpleButton>
```

<iframe src="/examples/buttons/disabled/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Button group

If you want to group buttons together on a single line, use the `Buttons` tag.

```html
<Buttons>
    <SimpleButton Color="Color.Secondary">LEFT</SimpleButton>
    <SimpleButton Color="Color.Secondary">CENTER</SimpleButton>
    <SimpleButton Color="Color.Secondary">RIGHT</SimpleButton>
</Buttons>
```

<iframe src="/examples/buttons/buttongroup/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Toolbar

To attach buttons together use a Toolbar role.

```html
<Buttons Role="ButtonsRole.Toolbar">
    <Buttons Margin="Margin.Is2.FromRight">
        <SimpleButton Color="Color.Primary">Primary</SimpleButton>
        <SimpleButton Color="Color.Secondary">Secondary</SimpleButton>
        <SimpleButton Color="Color.Info">Info</SimpleButton>
    </Buttons>
    <Buttons>
        <SimpleButton Color="Color.Danger">Danger</SimpleButton>
        <SimpleButton Color="Color.Warning">Warning</SimpleButton>
    </Buttons>
    <Buttons Margin="Margin.Is2.OnX">
        <SimpleButton Color="Color.Light">Light</SimpleButton>
    </Buttons>
</Buttons>
```

<iframe src="/examples/buttons/buttontoolbar/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Dropdown

The dropdown component is a container for a dropdown button and a dropdown menu.

```html
<Dropdown>
    <DropdownToggle Color="Color.Primary">
        Dropdown
    </DropdownToggle>
    <DropdownMenu>
        <DropdownItem>Action</DropdownItem>
        <DropdownDivider />
        <DropdownItem>Another Action</DropdownItem>
    </DropdownMenu>
</Dropdown>
```

<iframe src="/examples/buttons/dropdown/" frameborder="0" scrolling="no" style="width:100%;height:150px;"></iframe>

### Split

Just add a buttons to have a split dropdown.

```html
<Dropdown>
    <SimpleButton>Split Dropdown</SimpleButton>
    <DropdownToggle />
    <DropdownMenu>
        <DropdownItem>Action</DropdownItem>
        <DropdownDivider />
        <DropdownItem>Another Action</DropdownItem>
    </DropdownMenu>
</Dropdown>
```

<iframe src="/examples/buttons/splitdropdown/" frameborder="0" scrolling="no" style="width:100%;height:150px;"></iframe>