# Blazorise.AntDesign

This provider is not a thin copy of Ant Design React. It is a Blazorise provider that tries to feel like Ant Design v6 while still fitting Blazorise component APIs, markup patterns, lifecycle, and extension points.

The goal is:

- use Ant Design v6 class structure where it maps cleanly
- use Ant Design CSS variables and tokens whenever possible
- keep provider-owned hooks only where Blazorise behavior does not match Ant Design directly
- avoid stale Ant Design v4 selector assumptions
- keep the codebase maintainable for future provider work

## Mental Model

There are three layers in this provider:

1. Blazorise component API
2. AntDesign provider mapping
3. Ant Design v6 visual language

We do not try to reproduce the React implementation literally. We reproduce the rendered behavior and appearance as closely as is practical inside Blazorise.

That means:

- some components use native browser controls under an AntD shell
- some components use provider-specific Razor components because the shared Blazorise markup is not enough
- some controls need provider-owned CSS because AntD React behavior depends on JS/layout logic we do not have

## Design Decisions

### 1. Prefer AntD v6 classes first

If Ant Design already has a stable class for the rendered element, use it.

Examples:

- `ant-btn`
- `ant-input`
- `ant-picker`
- `ant-select`
- `ant-modal`
- `ant-tabs`
- `ant-table`

This lets `antd.css` do most of the work and reduces provider maintenance.

### 2. Use provider-owned `b-ant-*` classes only when needed

Provider classes are for cases where:

- Blazorise markup differs from AntD markup
- Blazorise exposes behavior that AntD does not have directly
- we need a safe hook for styling without fighting AntD selectors
- we need a wrapper/helper class for layout or integration

Typical provider hooks:

- `b-ant-*` for component-specific provider behavior
- `b-ant-<component>-<state>` for provider-only state hooks

Do not invent fake `ant-*` classes that do not exist in Ant Design.

### 3. Prefer tokens over hardcoded Sass colors

For styling, prefer CSS variables from `Styles/_variables.scss` and AntD tokens over Sass color maps or hardcoded values.

Prefer:

- `var(--ant-color-primary)`
- `var(--ant-color-text)`
- `var(--ant-color-bg-elevated)`
- `var(--ant-control-item-bg-hover)`
- `var(--ant-box-shadow-secondary)`

Avoid:

- `map-get($theme-colors, "primary")`
- hardcoded hex values unless there is no usable token path

Reason:

- runtime theming must keep working
- tokens make the provider consistent with Ant Design v6
- selector-based theme overrides are harder to maintain

### 4. Structure first, styling second

When a component looks wrong, first check whether the DOM structure and class mapping match AntD expectations.

Do not immediately patch SCSS if the real problem is:

- wrong wrapper element
- wrong class on the wrong node
- missing AntD state class
- extra markup that AntD CSS does not expect

Many visual bugs were fixed by correcting markup and class placement, not by adding more CSS.

### 5. Match AntD behavior, not just screenshots

Try to preserve:

- hover state
- active state
- selected state
- focus state
- disabled state
- animation/motion feel
- spacing rhythm

If behavior conflicts with exact AntD implementation details, choose the best Blazorise-compatible approximation and document the reason.

## Naming Conventions

### Use real AntD classes for real AntD surfaces

If a node is intended to be styled by `antd.css`, it should receive the correct AntD class directly.

Good:

- `ant-radio`
- `ant-radio-inner`
- `ant-breadcrumb-item`
- `ant-progress-track`

Bad:

- custom wrapper with no AntD class while expecting `antd.css` to style it
- fake AntD names that do not exist upstream

### Use `b-ant-*` only for provider hooks

These are acceptable:

- `b-ant-color-input`
- `b-ant-addons`
- `b-ant-file-picker-item`
- `b-ant-layout-root`

These should not become the main styling path if AntD already has a class for that node.

### State naming

For provider-specific states, use explicit names:

- `b-ant-modal-opening`
- `b-ant-modal-closing`
- `b-ant-offcanvas-opening`
- `b-ant-offcanvas-closing`

Do not hide provider behavior inside ambiguous generic names.

### Utility naming

If AntD already exposes a meaningful class, prefer it.

If Blazorise needs a provider-only utility, use `b-ant-*`.

Examples:

- keep `ant-typography-*` for typography colors
- keep `b-ant-typography-wrap` / `nowrap` / `italic` for provider-only utility hooks that AntD does not provide directly

## Styling Guidelines

### 1. Keep SCSS split by responsibility

Use:

- `Styles/components/` for provider component styling
- `Styles/utilities/` for provider utility classes
- `Styles/vendors/` for vendor skins or vendor integration styling
- `Styles/extensions/` only for true extensions that do not fit vendor/component buckets

If a style becomes the primary styling source for a vendor integration, it probably belongs in `vendors/`, not `extensions/`.

### 2. Avoid giant catch-all files

`_form.scss` used to accumulate too much. Prefer smaller component files when a control grows beyond shared form layout concerns.

Leave only shared rules in broad files.

### 3. Do not duplicate dead selectors

If a class is:

- no longer emitted by `AntDesignClassProvider`
- no longer referenced in Razor
- no longer needed by JS

remove the SCSS selector too.

We already removed several dead hooks during this provider pass. Keep doing that.

### 4. Do not style generated CSS directly

Only edit SCSS. Do not manually edit generated CSS files.

### 5. Be careful with specificity

AntD often styles elements through fairly specific selectors.

If a provider override is not applying, check:

- whether the AntD selector is more specific
- whether the class is on the correct node
- whether the visible border/background is actually on a child element

Common example:

- `Select` visual border is on `.ant-select-selector`, not the outer `.ant-select`

## Component Guidelines

### Inputs and pickers

- Prefer AntD input shell structure.
- For native controls or Flatpickr-based controls, style the visible shell to match AntD.
- Hidden/native helper inputs must not affect layout.

### Popups and overlays

- Use AntD popup tokens for shadow, radius, border, and z-index.
- Match open/close motion when practical.
- Keep provider popup state classes explicit.

### Complex controls

For components like:

- `FilePicker`
- `RangeSlider`
- `Carousel`
- `Tabs`
- `Table`

assume that structure matters as much as CSS.

Before changing styles, inspect:

- rendered markup
- provider classes
- where AntD expects active/selected/focus state

### JS-enhanced behavior

Use JS only when CSS/Blazor alone does not provide a credible AntD result.

Examples where JS is justified:

- wave effect
- segmented thumb movement
- some carousel behavior

Examples where JS should not be the first choice:

- simple spacing
- borders/radius
- static hover/selected colors

Keep JS modules small, provider-specific, and focused.

## Theming Guidelines

### Prefer token generation, not old selector theming

`AntDesignThemeGenerator` should push values into CSS variables/tokens first.

Do not reintroduce large blocks of old v4-style selector overrides unless there is no token path.

### When adding a new visual feature

Ask in this order:

1. Is there already an AntD token for this?
2. Is there already a provider token in `_variables.scss`?
3. Can this be expressed through existing AntD variables?
4. Only then add a provider-specific token or style hook

## Flatpickr / vendor integrations

When styling Flatpickr for this provider:

- keep the integration in `Styles/vendors/_flatpickr.scss`
- use AntD picker tokens for popup visuals
- do not split popup styling across multiple files unless there is a very strong reason

Remember:

- behavior still comes from Flatpickr
- appearance should feel like AntD
- locale/order/month formatting issues are usually config problems, not SCSS problems

## Common Pitfalls

### 1. Accidentally using old AntD v4 class names

Always verify against Ant Design v6, not memory.

### 2. Styling the wrong node

Many AntD controls put the visible border/background on an inner child.

Examples:

- `Select` -> `.ant-select-selector`
- `Radio` -> `.ant-radio-inner`
- `Checkbox` -> `.ant-checkbox-inner`

### 3. Adding redundant provider classes

If AntD already styles the node correctly, do not add a provider hook just because it feels safer.

### 4. Leaving dead hooks behind

If you remove a class from Razor or the class provider, check for:

- SCSS leftovers
- JS selectors
- theme generator references

### 5. Fixing a structural problem with more CSS

This usually creates technical debt.

First verify:

- markup
- class placement
- state classes
- wrapper ownership

## How to Continue Work Safely

When implementing or fixing a component:

1. Check the rendered Blazorise markup.
2. Compare it with the expected AntD structure.
3. Fix class placement and structure first.
4. Use AntD tokens and variables for styling.
5. Add `b-ant-*` hooks only for provider-specific gaps.
6. Remove obsolete selectors/classes after the change stabilizes.

## Final Rule

The provider should look like Ant Design v6, but it must still be understandable to a Blazorise maintainer.

If a solution is visually accurate but fragile, overly React-specific, or impossible to maintain in Blazorise, prefer the simpler provider-friendly version and document the tradeoff.