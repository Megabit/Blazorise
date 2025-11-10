---
title: Blazorise Migration to v2.0
description: Track and review changes to the Blazorise APIs, documentation, and components to help you migrate from v1.x to v2.0.
permalink: /blog/migration-to-2.0
canonical: /blog/migration-to-2.0
image-url: /img/news/empty.png
image-text: Blazorise Migration to v2.0
author-name: Mladen Macanović
author-image: mladen
posted-on: January 15th, 2025
read-time: 4 min
---

# Migrating to 2.0

Blazorise 2.0 is a major release that brings a lot of changes to the Blazorise APIs, documentation, and components. This article will help you track and review these changes to help you migrate from v1.x to v2.0.

## Breaking Changes

The following is a list of breaking changes that you need to be aware of when migrating to Blazorise 2.0:

## Input Components

All input components have been updated to use a single `Value` parameter instead of multiple parameters. This change was made to simplify the API and make it more consistent across all components.

### Check

- Dropping the `Checked` parameter. Use the `Value` parameter instead.

### ColorEdit

- Dropping the `Color` parameter. Use the `Value` parameter instead.

### ColorPicker

- Dropping the `Color` parameter. Use the `Value` parameter instead.

### DateInput

- Dropping the `Date` parameter. Use the `Value` parameter instead.

### DatePicker

- Dropping the `Date` and `Dates` parameters. Use the `Value` parameter instead.

### MemoInput

- Dropping the `Text` parameter. Use the `Value` parameter instead.

### Select

- Dropping the `SelectedValue` and `SelectedValues` parameters. Use the `Value` parameter instead.

### Switch

- Dropping the `Checked` parameter. Use the `Value` parameter instead.

### TextInput

- Dropping the `Text` parameter. Use the `Value` parameter instead.

### TimeInput

- Dropping the `Time` parameter. Use the `Value` parameter instead.

### TimePicker

- Dropping the `Time` parameter. Use the `Value` parameter instead.

### Link

Removed `Match.Custom` enum value from the `Match` parameter. This change was made to simplify the API and make it more consistent across all components.

## Validations

When using the `Validator` rule of the `Validation` component, you need to update the use of the `Value` argument of `ValidatorEventArgs`. In Blazorise 2.0, the `Value` will match the `TValue` parameter of the input component, so you need to update your code accordingly.

For example, if you were using the `Value` argument of `ValidatorEventArgs` like this:

```csharp
private void OnValidateStartDate( ValidatorEventArgs e )
{
    var startValue = ( e.Value as DateOnly[] )?[0];

    // ...
}
```

You need to update it to use the `Value` parameter directly, like this:

```csharp
private void OnValidateStartDate( ValidatorEventArgs e )
{
    var startValue = e.Value as DateOnly?;

    // ...
}
```

## Removed BLMouseEventArgs

The `BLMouseEventArgs` class has been removed in Blazorise 2.0. You should use the `MouseEventArgs` class from the `Microsoft.AspNetCore.Components.Web` namespace instead.