---
title: "Bar component"
permalink: /docs/components/bar/
excerpt: "Learn how to use bars."
toc: true
toc_label: "Guide"
redirect_from: /docs/components/bars/
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
      - `BarDropdown` dropdown container
        - `BarDropdownToggle` dropdown trigger
        - `BarDropdownMenu` the dropdown menu, which can include bar items and dividers
          - `BarDropdownItem` each single item of the dropdown menu

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
                <Button Color="Color.Primary">Sign up</Button>
                <Button Color="Color.Secondary">Log in</Button>
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
                <BarDropdown>
                    <BarDropdownToggle>Dropdown</BarDropdownToggle>
                    <BarDropdownMenu>                        
                        <BarDropdownItem>Action</BarDropdownItem>
                        <BarDropdownItem>Another action</BarDropdownItem>
                    </BarDropdownMenu>
                </BarDropdown>
            </BarItem>
        </BarStart>
    </BarMenu>
</Bar>
```

<iframe src="/examples/bars/dropdown/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

## Attributes

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Visible       | boolean                                                                    | false            | Controls the state of toggle and the menu.                                                  |
| Breakpoint    | [Breakpoint]({{ "/docs/helpers/enums/#breakpoint" | relative_url }})       | `None`           | Defines the media breakpoint.                                                               |
| ThemeContrast | [ThemeContrast]({{ "/docs/helpers/enums/#themecontrast" | relative_url }}) | `Light`          | Adjusts the contrast for light or dark themes.                                              |
| Background    | [Background]({{ "/docs/helpers/colors/#background" | relative_url }})      | `None`           | Sets the bar background color.                                                              |
| Color         | [Colors]({{ "/docs/helpers/colors/#color" | relative_url }})               | `None`           | Component visual or contextual style variants.                                              |