---
title: 'Bar component'
permalink: /docs/components/bar/
excerpt: 'Learn how to use bars.'
toc: true
toc_label: 'Guide'
redirect_from: /docs/components/bars/
---

## Basics

The `Bar` component is a responsive and versatile navigation bar that can be used as a top menu in **Horizontal** mode or as a sidebar in one of the three **Vertical** modes.

The bar component has the following structure:

- `Bar` the main container
  - `BarBrand` **Horizontal**: the left side, always visible. **Vertical**: top of Bar branding.
  - `BarToggler` **Horizontal**: toggles the bar. **Vertical**: read more [below]({{"#vertical-bar-toggler"}}).
  - `BarMenu` **Horizontal**: the right side, hidden on breakpoint. **Vertical**: contains the core menu elements.
    - `BarStart` **Horizontal**: left side menu. **Vertical**: sticky top menu.
    - `BarEnd` **Horizontal**: right side menu. **Vertical**: sticky bottom menu.
      - `BarItem` each single item of the bar menu
        - `BarLink` item link or button
          - `BarIcon` icon for Bar item (required for BarMode.VerticalSmall)
        - `BarDropdown` dropdown container (or popout for BarMode.VerticalPopout)
          - `BarDropdownToggle` dropdown trigger
          - `BarDropdownMenu` the dropdown menu, which can include bar items and dividers
            - `BarDropdownItem` each single item of the dropdown menu
            - `BarDropdown` by adding this again inside the menu, you will create nested dropdowns

### Top Bar (with dropdown)

```html
<Bar
  Breakpoint="Breakpoint.Desktop"
  Background="Background.Light"
  ThemeContrast="ThemeContrast.Light"
>
  <BarBrand>
    Brandname
  </BarBrand>
  <BarToggler />
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

### Vertical Bar (Sidebar)

When using the Bar in one of the `Vertical` modes for the purpose of a Sidebar, you need to place it inside the `LayoutSiderContent` component.

Please see the [Layout docs]({{"/docs/components/layout/#with-sider"}}) for more information on setup with Sidebar.

```html
<Bar
  Mode="BarMode.VerticalInline"
  CollapseMode="BarCollapseMode.Small"
  Breakpoint="Breakpoint.Desktop"
  NavigationBreakpoint="Breakpoint.Tablet"
  ThemeContrast="ThemeContrast.Dark"
>
  <BarBrand>
    <BarItem>
      <BarLink To="">
        <BarIcon IconName="IconName.Dashboard" />
        Blazorise Demo
      </BarLink>
    </BarItem>
  </BarBrand>
  <BarMenu>
    <BarStart>
      <BarItem>
        <BarLink To="#home">
          <BarIcon IconName="IconName.Dashboard" />
          Home
        </BarLink>
      </BarItem>
      <BarItem>
        <BarLink To="#docs">Documentation</BarLink>
      </BarItem>
      <BarItem>
        <BarDropdown>
          <BarDropdownToggle>
            <BarIcon IconName="IconName.Edit" />
            Dropdown
          </BarDropdownToggle>
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

_Note:_ The `BarIcon` is required for each `BarLink` or `BarDropdownToggle` when using `BarMode.VerticalSmall` or `BarCollapseMode.Small`, in order for it work correctly.

For a demo of the Vertical bar in action, please check out [this Blazorise demo](https://bootstrapdemo.blazorise.com/).

### Right-aligned Vertical Bar

You can create a right-aligned `Vertical` style `Bar` easily by following two key principals.

- Using the `Layout` to accommodate the right-align bar, by putting the `LayoutSider` section after the `Layout` section (see in example below).
- Adding `RightAligned="true"` to ALL `BarDropdownMenu` within the `Vertical` bar.

Right-aligned vertical bar example:

```html
<Layout Sider="true">
  <LayoutSider>
    <LayoutSiderContent>
      <Bar Mode="BarMode.VerticalInline" CollapseMode="BarCollapseMode.Small">
        <BarBrand>
          ...
        </BarBrand>
        <BarMenu>
          <BarStart>
            <BarItem>
              <BarDropdown>
                <BarDropdownToggle>
                  ...
                </BarDropdownToggle>
                <BarDropdownMenu RightAligned="true">
                  ...
                </BarDropdownMenu>
              </BarDropdown>
            </BarItem>
          </BarStart>
        </BarMenu>
      </Bar>
    </LayoutSiderContent>
  </LayoutSider>
  <Layout>
    <LayoutHeader Fixed="true">
      Header
    </LayoutHeader>
    <LayoutContent>
      Content
    </LayoutContent>
  </Layout>
</Layout>
```

### Vertical Bar Toggler

The `BarToggler` for Vertical Bar supports 2 modes:
- `BarTogglerMode.Normal` (default)
- `BarTogglerMode.Popout`

These 2 modes, along with an external implementation, provides 3 easy to configure implementations.

**1. Inline**

By simply adding the `BarToggler` anywhere inside the Vertical `Bar` component (at the top level), you will have an inline toggler added to your bar at this location.

Example (used by demo application):
```html
<Bar Mode="BarMode.VerticalInline">
  <BarToggler />
  <BarBrand>
    ...
  </BarBrand>
  <BarMenu>
    ...
  </BarMenu>
</Bar>
```

**2. Popout**

Once again, by simply adding the `BarToggler` anywhere inside the Vertical `Bar` component (at the top level), and setting the `Mode` to `Popout` you will have an popout toggler added to your bar at this location.

Example:
```html
<Bar Mode="BarMode.VerticalInline">
  <BarBrand>
    ...
  </BarBrand>
  <BarToggler Mode="BarTogglerMode.Popout" />
  <BarMenu>
    ...
  </BarMenu>
</Bar>
```

**3. External** (controlled by Top Bar)

For an externally controlled `BarToggler`, you need to be using the top bar.

Where you have a top bar and vertical bar, you can add multiple `BarTogglers` to the top bar and choose to have one of these control the vertical bar by simply setting the `Bar` property.

In the example below, we will create two `BarTogglers` inside the top bar.
- One to control the Vertical Bar (closest to it)
- One to control the Top Bar (at the opposite end)

Example:
```html
<Layout Sider="true">
  <LayoutSider>
    <LayoutSiderContent>
      <Bar @ref="@sidebar" Mode="BarMode.VerticalInline">
        ...
      </Bar>
    </LayoutSiderContent>
  </LayoutSider>
  <Layout>
    <LayoutHeader Fixed="true">
      <Bar Mode="BarMode.Horizontal">
        <BarToggler Bar="@sidebar" />
        <BarBrand>
          ...
        </BarBrand>
        <BarMenu>
          ...
        </BarMenu>
        <BarToggler />
      </Bar>
    </LayoutHeader>
    <LayoutContent>
      ...
    </LayoutContent>
  </Layout>
</Layout>

@code {
  private Bar sidebar;
}
```

### Nested Dropdowns

To create nested `BarDropdowns`, you can simply add a `BarDropdown` component inside the structure of an existing `BarDropdownMenu`.

Example:
```html
<BarMenu>
  <BarStart>
    <BarItem>
      <BarDropdown>
        <BarDropdownToggle>
          Top-level toggler
        </BarDropdownToggle>
        <BarDropdownMenu>
          <BarDropdownItem>
            Top-level item
          </BarDropdownItem>
          <BarDropdown>
            <BarDropdownToggle>
              Nested toggler
            </BarDropdownToggle>
            <BarDropdownMenu>
              <BarDropdownItem>
                Nested item
              </BarDropdownItem>
            </BarDropdownMenu>
          </BarDropdown>
        </BarDropdownMenu>
      </BarDropdown>
    </BarItem>
  </BarStart>
</BarMenu>
```

### Header on top

For an example where the horizontal `Bar` is "on top" of the vertical `Bar`, please look at [this example]({{"/docs/components/layout/#sider-with-header-on-top"}}) in the `Layout` docs.

## Attributes

## Bar

| Name                 | Type                                                        | Default          | Description                                |
| -------------------- | ----------------------------------------------------------- | ---------------- | ------------------------------------------ |
| Mode                 | [BarMode]({{ "/docs/helpers/enums/#barmode"                 | relative_url }}) | `Horizontal`                               | Bar mode (`Vertical*` for Sidebar). |
| CollapseMode         | [BarCollapseMode]({{ "/docs/helpers/enums/#barcollapsemode" | relative_url }}) | `Hide`                                     | What the Bar will collapse to when `Visible` toggled. |
| Visible              | boolean                                                     | false            | Controls the state of toggle and the menu. |
| Breakpoint           | [Breakpoint]({{ "/docs/helpers/enums/#breakpoint"           | relative_url }}) | `None`                                     | Defines the media breakpoint. |
| NavigationBreakpoint | [Breakpoint]({{ "/docs/helpers/enums/#breakpoint"           | relative_url }}) | `None`                                     | Defines the media breakpoint on navigation. |
| ThemeContrast        | [ThemeContrast]({{ "/docs/helpers/enums/#themecontrast"     | relative_url }}) | `Light`                                    | Adjusts the contrast for light or dark themes. |
| Background           | [Background]({{ "/docs/helpers/colors/#background"          | relative_url }}) | `None`                                     | Sets the bar background color. |
| Color                | [Colors]({{ "/docs/helpers/colors/#color"                   | relative_url }}) | `None`                                     | Component visual or contextual style variants. |

### BarLink

| Name       | Type                                                        | Default | Description                                                              |
|------------|-------------------------------------------------------------|---------|--------------------------------------------------------------------------|
| To         | string                                                      | null    | Path to the destination page.                                            |
| Target     | [Target]({{ "/docs/helpers/enums/#target" | relative_url }})| `None`  | The target attribute specifies where to open the linked document.        |
| Match      | [Match]({{ "/docs/helpers/enums/#match" | relative_url }})  | `All`   | URL matching behavior for a link.                                        |
| Title      | string                                                      | null    | Defines the title of a link, which appears to the user as a tooltip.     |