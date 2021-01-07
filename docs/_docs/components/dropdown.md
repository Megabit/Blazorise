---
title: "Dropdown component"
permalink: /docs/components/dropdown/
excerpt: "Learn how to use dropdowns."
toc: true
toc_label: "Guide"
---

## Basics

The dropdown component is a container for a dropdown button and a dropdown menu.

- `Dropdown` the **main** container
  - `DropdownToggle` toggle button
  - `DropdownMenu` the toggable menu, **hidden** by default
    - `DropdownItem` each **single** item of the dropdown menu
    - `DropdownDivider` a **horizontal line** to separate dropdown items
          

## Dropdown

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

### Split dropdown

Just add another `Button` to have a split dropdown.

```html
<Dropdown>
    <Button>Split Dropdown</Button>
    <DropdownToggle Split="true"/>
    <DropdownMenu>
        <DropdownItem>Action</DropdownItem>
        <DropdownDivider />
        <DropdownItem>Another Action</DropdownItem>
    </DropdownMenu>
</Dropdown>
```

<iframe src="/examples/buttons/splitdropdown/" frameborder="0" scrolling="no" style="width:100%;height:150px;"></iframe>

## Usage

By default a dropdown toggle will open and close a dropdown menu without the need to do it manually. In case you need to control the menu programmatically you have to use the Dropdown reference.

```html
<Dropdown @ref="dropdown">
    <DropdownToggle />
    <DropdownMenu>
        ...
    </DropdownMenu>
</Dropdown>

<Button Clicked="@ShowMenu">Show Menu</Button>
```

```cs
@code{
    Dropdown dropdown;

    void ShowMenu()
    {
        dropdown.Open();
    }
}
```

## Functions

| Name         | Description                                                                                 |
|--------------|---------------------------------------------------------------------------------------------|
| Open()       | Open the dropdown menu.                                                                     |
| Close()      | Close the dropdown menu.                                                                    |
| Toggle()     | Switches the visibility of dropdown menu.                                                   |

## Attributes

### Dropdown

| Name           | Type                                                                   | Default   | Description                                                                                                                    |
|----------------|------------------------------------------------------------------------|-----------|--------------------------------------------------------------------------------------------------------------------------------|
| Visible        | boolean                                                                | false     | Handles the visibility of dropdown menu.                                                                                       |
| RightAligned   | boolean                                                                | false     | Right aligned dropdown menu.                                                                                                   |
| Disabled       | boolean                                                                | false     | Disabled the button or toggle button that are placed inside of dropdown.                                                       |
| Direction      | [Direction]({{ "/docs/helpers/enums/#direction" | relative_url }})     | `Down`    | Direction of an dropdown menu.                                                                                                 |
| Toggled        | event                                                                  |           | Occurs after the dropdown menu visibility has changed.                                                                         |

### DropdownMenu

| Name           | Type                                                                   | Default   | Description                                                                                                                    |
|----------------|------------------------------------------------------------------------|-----------|--------------------------------------------------------------------------------------------------------------------------------|
| RightAligned   | boolean                                                                | false     | Right aligned dropdown menu.                                                                                                   |

### DropdownItem

| Name           | Type                                                                   | Default   | Description                                                                                                                    |
|----------------|------------------------------------------------------------------------|-----------|--------------------------------------------------------------------------------------------------------------------------------|
| Value          | object                                                                 | null      | Holds the item value.                                                                                                          |
| Clicked        | event                                                                  |           | Occurs when the item is clicked.                                                                                               |
| Active         | boolean                                                                | false     | Marks the item with an state.                                                                                                  |
| Disabled       | boolean                                                                | false     | Marks the item with disabled state and doesn't allow the click event.                                                          |

### DropdownToggle

| Name           | Type                                                                   | Default   | Description                                                                                                                    |
|----------------|------------------------------------------------------------------------|-----------|--------------------------------------------------------------------------------------------------------------------------------|
| Color          | [Colors]({{ "/docs/helpers/colors/#color" | relative_url }})           | `None`    | Component visual or contextual style variants.                                                                                 |
| Size           | [Size]({{ "/docs/helpers/sizes/#size" | relative_url }})               | `None`    | Button size variations.                                                                                                        |
| Split          | boolean                                                                | false     | Handles the visibility of split button.                                                                                        |
| Outline        | boolean                                                                | false     | Outlined button                                                                                                                |
| Disabled       | boolean                                                                | false     | Makes toggle look inactive.                                                                                                    |