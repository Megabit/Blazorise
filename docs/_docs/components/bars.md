---
title: "Bars"
permalink: /docs/components/bars/
excerpt: "Learn how to use bars."
toc: true
toc_label: "Guide"
---

## Basics

The bar component is a responsive and versatile horizontal navigation bar with the following structure:

- `Bar` the main container
  - `BarBrand` the left side, **always visible**, usually contains the logo
  - `BarToggler` toggles the bar menu on the touch devices
  - `BarMenu` the right side, hidden on touch devices, visible on desktop
  - `BarStart` (left side of the menu)
  - `BarEnd` (right side of the menu)
    - `BarItem` each single item of the bar menu
      - `BarLink` item link or button
      - `BarDropdown` the dropdown menu, which can include bar items and dividers
        - `BarDropdownItem` each single item of the dropdown

### Basic Bar

```html
<Bar Breakpoint="Breakpoint.Desktop" Background="Background.Light" Theme="Theme.Light">
    <BarBrand>
        Brandname
    </BarBrand>
    <BarToggler>
    </BarToggler>
    <BarMenu>
        <BarStart>
            <BarItem>
                <BarLink To="#home">Home</BarLink>
            </BarItem>
            <BarItem>
                <BarLink To="#docs">Documentation</BarLink>
            </BarItem>
        </BarStart>
        <BarEnd>
            <BarItem>
                <SimpleButton Color="Color.Primary">Sign up</SimpleButton>
                <SimpleButton Color="Color.Secondary">Log in</SimpleButton>
            </BarItem>
        </BarEnd>
    </BarMenu>
</Bar>
```

<iframe src="/examples/bars/basic/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### With dropdown

```html
<Bar Breakpoint="Breakpoint.Desktop" Background="Background.Light" Theme="Theme.Light">
    <BarBrand>
        Brandname
    </BarBrand>
    <BarToggler>
    </BarToggler>
    <BarMenu>
        <BarStart>
            <BarItem>
                <BarLink To="#home">Home</BarLink>
            </BarItem>
            <BarItem>
                <BarLink To="#docs">Documentation</BarLink>
            </BarItem>
            <BarItem>
                <BarDropdownToggler>Dropdown</BarDropdownToggler>
                <BarDropdown>
                    <BarDropdownItem>Action</BarDropdownItem>
                    <BarDropdownItem>Another action</BarDropdownItem>
                </BarDropdown>
            </BarItem>
        </BarStart>
    </BarMenu>
</Bar>
```

<iframe src="/examples/bars/dropdown/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>