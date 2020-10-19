---
title: "Link"
permalink: /docs/components/link/
excerpt: "Learn how to link component."
toc: true
toc_label: "Guide"
---

`<Link>` is the building block for most Blazorise components that offer link functionality. A `Link` component behaves like an `<a>` element, except it toggles an active CSS class based on whether its `href` matches the current URL.

## Example

By specifying a value in the `To` property, a standard link (`<a>`) element will be rendered.

```html
<Link To="user/settings">
    Link text
</Link>
```

## Anchor Links

A `#` in front of a link location specifies that the link is pointing to an anchor on a page. (Anchor meaning a specific place in the middle of your page).

Typically `<a href="#">` will cause the document to scroll to the top of page when clicked. `<Link>` addresses this by preventing the default action (scroll to top) when `To` is set to `#`. If you need scroll to top behavior, use a standard `<Link To="#">...</Link>` tag.

```html
<Link To="#foo">
    Link text
</Link>
```

## Attributes

| Name       | Type                                                        | Default    | Description                                                          |
|------------|-------------------------------------------------------------|------------|----------------------------------------------------------------------|
| To         | string                                                      | null       | Path to the destination page.                                        |
| Target     | [Target]({{ "/docs/helpers/enums/#target" | relative_url }})| `None`     | The target attribute specifies where to open the linked document.    |
| Match      | [Match]({{ "/docs/helpers/enums/#match" | relative_url }})  | `Prefix`   | URL matching behavior for a link.                                    |
| Title      | string                                                      | null       | Defines the title of a link, which appears to the user as a tooltip. |
| Clicked    | event                                                       |            | Occurs when the link is clicked.                                     |