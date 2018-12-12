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

### Colored buttons

To define button color use a `Color` attribute.

```html
<SimpleButton Color="Color.Primary">PRIMARY</SimpleButton>
<SimpleButton Color="Color.Secondary">SECONDARY</SimpleButton>
```

To find the list of supported colors please look at the [colors]({{ "/docs/helpers/colors/" | relative_url }}) page.

### Block button

```html
<SimpleButton IsBlock="true">Button</SimpleButton>
```

### Active button

```html
<SimpleButton IsActive="true">Button</SimpleButton>
```

### Disabled button

```html
<SimpleButton IsDisabled="true">Button</SimpleButton>
```


## Button group

If you want to group buttons together on a single line, use the `Buttons` tag.

```html
<Buttons>
    <SimpleButton Color="Color.Secondary">LEFT</SimpleButton>
    <SimpleButton Color="Color.Secondary">CENTER</SimpleButton>
    <SimpleButton Color="Color.Secondary">RIGHT</SimpleButton>
</Buttons>
```

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

## Dropdown

The dropdown component is a container for a dropdown button and a dropdown menu.

```html
<Dropdown>
    <DropdownToggle Color="Color.Primary">
        DROPDOWN
    </DropdownToggle>
    <DropdownMenu>
        <DropdownItem>Action</DropdownItem>
        <DropdownDivider />
        <DropdownItem>Another Action</DropdownItem>
    </DropdownMenu>
</Dropdown>
```

### Split

Just add a buttons to have a split dropdown.

```html
<Dropdown>
    <SimpleButton>SPLIT DROPDOWN</SimpleButton>
    <DropdownToggle />
    <DropdownMenu>
        <DropdownItem>Action</DropdownItem>
        <DropdownDivider />
        <DropdownItem>Another Action</DropdownItem>
    </DropdownMenu>
</Dropdown>
```