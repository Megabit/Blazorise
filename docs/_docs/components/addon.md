---
title: "Addons"
permalink: /docs/components/addon/
excerpt: "Learn how to use addons."
toc: true
toc_label: "Guide"
redirect_from: /docs/components/addons/
---

## Structure

Addons are used to attach controls together.

- `<Addons>`
  - `<Addon>`

To define addon location you have to set the `AddonType` attribute:

- `.Start` will place addon on the left side.
- `.Body` will place it in the middle.
- `.End` will place it at the right side.

## Static addon

```html
<Addons>
    <Addon AddonType="AddonType.Start">
        <AddonLabel>@@</AddonLabel>
    </Addon>
    <Addon AddonType="AddonType.Body">
        <TextEdit Placeholder="Username" />
    </Addon>
</Addons>
```

<iframe src="/examples/addons/static/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### With multiple addons

Addons are used to attach controls together.

```html
<Addons>
    <Addon AddonType="AddonType.Start">
        <AddonLabel>Start</AddonLabel>
    </Addon>
    <Addon AddonType="AddonType.Body">
        <TextEdit Placeholder="Placeholder" />
    </Addon>
    <Addon AddonType="AddonType.End">
        <AddonLabel>End</AddonLabel>
    </Addon>
</Addons>
```

<iframe src="/examples/addons/static2/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Button addon

```html
<Addons>
    <Addon AddonType="AddonType.Body">
        <TextEdit Placeholder="Recipient's username" />
    </Addon>
    <Addon AddonType="AddonType.End">
        <Button Color="Color.Secondary">Button</Button>
    </Addon>
</Addons>
```

<iframe src="/examples/addons/button/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Dropdown addon

```html
<Addons>
    <Addon AddonType="AddonType.Start">
        <Dropdown>
            <DropdownToggle Color="Color.Primary">Dropdown</DropdownToggle>
            <DropdownMenu>
                <DropdownItem>Action</DropdownItem>
                <DropdownItem>Another action</DropdownItem>
                <DropdownItem>Something else here</DropdownItem>
                <DropdownDivider />
                <DropdownItem>Separated link</DropdownItem>
            </DropdownMenu>
        </Dropdown>
    </Addon>
    <Addon AddonType="AddonType.Body">
        <TextEdit />
    </Addon>
</Addons>
```

<iframe src="/examples/addons/dropdown/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Attributes

| Name        | Type                  | Default | Description                                                                                 |
|-------------|-----------------------|---------|---------------------------------------------------------------------------------------------|
| AddonType   | AddonType             | `Body`  | Defines the location and behaviour of addon container.                                      |