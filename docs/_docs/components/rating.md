---
title: "Rating component"
permalink: /docs/components/rating/
excerpt: "Learn how to use rating component."
toc: true
toc_label: "Guide"
---

## Rating

Ratings provide insight regarding others opinions and experiences with a product. Users can also rate products they’ve purchased.

```html
<Rating />
```

## Usage

### With bind attribute

By using `bind-*` attribute the selected value will be automatically assigned to the member variable.

```html
<Rating @bind-SelectedValue="@SelectedValue" MaxValue="10" />

@code{
    int SelectedValue;
}
```

## Attributes

| Name                          | Type                                                                          | Default           | Description                                                                                          |
|-------------------------------|-------------------------------------------------------------------------------|-------------------|------------------------------------------------------------------------------------------------------|
| MaxValue                      | int                                                                           | 5                 | Maximum rating value that is allowed to be selected.                                                 |
| ReadOnly                      | boolean                                                                       | false             | Prevents modification of the input’s value.                                                          |
| Disabled                      | boolean                                                                       | false             | Prevent the user interactions and make it appear lighter.                                            |
| ReadOnly                      | boolean                                                                       | false             | Prevents modification of the input’s value.                                                          |
| Disabled                      | boolean                                                                       | false             | Prevents user interactions and make it appear lighter.                                               |
| FullIcon                      | `object`                                                                      | `IconName.Star`   | Defines the selected icon name.                                                                      |
| EmptyIcon                     | `object`                                                                      | `IconName.Star`   | Defines the non-selected icon name.                                                                  |
| FullIconStyle                 | [`IconStyle?`]({{ "/docs/helpers/enums/#iconstyle" | relative_url }})         | `Solid`           | Defines the selected icon style.                                                                     |
| EmptyIconStyle                | [`IconStyle?`]({{ "/docs/helpers/enums/#iconstyle" | relative_url }})         | `Regular`         | Defines the non-selected icon style.                                                                 |
| Color                         | [`Color`]({{ "/docs/helpers/colors/#color" | relative_url }})                 | `Warning`         | Defines the color or icons.                                                                          |
