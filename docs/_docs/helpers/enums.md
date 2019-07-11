---
title: "Enums"
permalink: /docs/helpers/enums/
excerpt: "Blazorise have it's own set of enums that you can use instead of manually writing css classnames."
toc: true
toc_label: "Enums"
---

## Breakpoint

Defines the media breakpoint.

- `None` Undefined.
- `Mobile` Valid on all devices. (extra small)
- `Tablet` Breakpoint on tablets (small).
- `Desktop` Breakpoint on desktop (medium).
- `Widescreen` Breakpoint on widescreen (large).
- `FullHD` Breakpoint on large desktops (extra large).

## ThemeContrast

Adjusts the contrast for light or dark themes.

- `None` Undefined.
- `Light` Adjusts the theme for a light colors.
- `Dark` Adjusts the theme for a dark colors.

## Match

Modifies the URL matching behavior for a link.

- `Prefix` Specifies that the link should be active when it matches any prefix of the current URL.
- `All` Specifies that the link should be active when it matches the entire current URL.

## ButtonType

Defines the button type and behaviour.

- `Button` The button is a clickable button.
- `Submit` The button is a submit button (submits form-data).
- `Reset` The button is a reset button (resets the form-data to its initial values).

## Cursor

Defines the mouse cursor.

- `Default` Default behaviour, nothing will be changed.
- `Pointer` The cursor is a pointer and indicates a link.

## Direction

Direction of an dropdown menu.

- `None` Same as `Down`.
- `Down` Trigger dropdown menus bellow an element (default behaviour).
- `Up` Trigger dropdown menus above an element.
- `Right` Trigger dropdown menus to the right of an element.
- `Left` Trigger dropdown menus to the left of an element.

## JustifyContent

Aligns the flexible container's items when the items do not use all available space on the main-axis (horizontally).

- `None` Sets this property to its default value.
- `Start` Items are positioned at the beginning of the container.
- `End` Items are positioned at the end of the container.
- `Center` Items are positioned at the center of the container.
- `Between` Items are positioned with space between the lines.
- `Around` Items are positioned with space before, between, and after the lines.

## Alignment

Defines the alignment of an element.

- `None` No alignment will be applied.
- `Start` Aligns an element to the left.
- `Center` Aligns an element on the center.
- `End` Aligns an element to the right.

## ValidationStatus

Defines the validation results.

- `None` No validation.
- `Success` Validation has passed the check.
- `Error` Validation has failed.