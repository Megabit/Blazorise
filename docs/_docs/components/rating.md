---
title: "Rating component"
permalink: /docs/components/rating/
excerpt: "Learn how to use rating component."
toc: true
toc_label: "Guide"
---

## Basic Memo

Rating is used to create multiline text input (text-area).

```html
<Rating @bind-SelectedValue="@SelectedValue" />
```

## Usage

### With bind attribute

By using `bind-*` attribute the text will be automatically assigned to the member variable.

```html
<Rating @bind-SelectedValue="@SelectedValue" MaxValue="10" />

@code{
    int SelectedValue;
}
```
## Settings

**Note:** All of the options above can also be defined on each `MemoEdit` individually. Defining them on `MemoEdit` will override any global settings.
{: .notice--info}

## Attributes

| Name                          | Type                                                         | Default | Description                                                                                          |
|-------------------------------|--------------------------------------------------------------|---------|------------------------------------------------------------------------------------------------------|
| MaxValue                          | int                                                       | 5        | Maximum rating value that is allowed to be selected.                                                                                         |
| ReadOnly                   | boolean                                                        | false        | Prevents modification of the input’s value.                                                                       |
| Disabled                     | boolean                                                      | false   | Prevent the user interactions and make it appear lighter.                   |
| ReadOnly                      | boolean                                                      | false   | Prevents modification of the input’s value.                                                          |
| Disabled                      | boolean                                                      | false   | Prevents user interactions and make it appear lighter.                                               |
| FullIcon                     | `object`                                                       | IconName.Star    | Defines the selected icon name.                             |
| EmptyIcon                   | `object`                                                       | IconName.Star    | Defines the non-selected icon name.                                                             |                                                    |
| FullIconStyle                     | `IconStyle?`                                                       | IconStyle.Solid    | Defines the selected icon style.                             |
| EmptyIconStyle                   | `IconStyle?`                                                       | IconStyle.Regular        | Defines the non-selected icon style.                                                             | 
| Size                          | [Sizes]({{ "/docs/helpers/sizes/#size" | relative_url }})    | `None`  | Component size variations.                                                                           |
