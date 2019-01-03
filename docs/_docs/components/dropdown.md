---
title: "Dropdown component"
permalink: /docs/components/dropdown/
excerpt: "Learn how to use dropdowns."
toc: true
toc_label: "Guide"
---

## Single button dropdown

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

## Split button dropdown

Just add another `Button` to have a split dropdown.

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

## How to use

By default a dropdown toggle will open and close a dropdown menu without the need to do it manually. In case you need to control the menu programatically you have to use the Dropdown reference.

```html
<Dropdown ref="dropdown">
    <DropdownToggle />
    <DropdownMenu>
        ...
    </DropdownMenu>
</Dropdown>

<SimpleButton Clicked="@ShowMenu">Show Menu</SimpleButton>
```

```cs
@functions{
    Dropdown dropdown;

    void ShowMenu()
    {
        dropdown.Open();
    }
}
```