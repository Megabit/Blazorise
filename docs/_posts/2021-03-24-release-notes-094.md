---
title: "v0.9.4 release notes"
permalink: /news/release-notes/094/
classes: wide
categories:
  - Release Notes
tags:
  - blazorise
  - release notes
  - 0.9.4
---

## Breaking changes

- `Dialog` parameter is removed from `ModalContent`. It was used only by the Bulma provider and so we made it to be used implicitly by the framework if some conditions are met.

## Migration

- In `RichTextEditOptions`, rename `DynamicLoadReferences` to `DynamicallyLoadReferences`
- Typography utilities are renamed and now they have a `Text*` prefix. Affected components are `DisplayHeading`, `Heading`, `Paragraph`, `Text`, `CardSubtitle`, `CardTitle`, and `CardText`. The changed parameters are:
  - `Color` to `TextColor`
  - `Alignment` to `TextAlignment`
  - `Transform` to `TextTransform`
  - `Weight` to `TextWeight`
- For `Progress` component
  - Use only `Progress` component because `ProgressBar` is now needed only for multiple stacked bars, eg. `<Progress Value="50" />`
  - Instead of `Background` parameter use the `Color` parameter
- Remove usage of `Dialog` parameter for `ModalContent`
- On DataGridColumn, rename `Direction` parameter to `SortDirection`
- For DataGrid `ReadData` event callback, rename `Direction` attribute to `SortDirection`
- DataGrid `FilteredDataChanged` now accepts the `DataGridFilteredDataEventArgs<TItem>` as the argument instead of `IEnumerable<TItem>`

## Highlights 🚀

### Async Validation

There are situations when you need to do validation by using the external method or a service. Since calling them can take some time it not advised to do it synchronously as that can lead to pretty horrible UI experience. So to handle those scenarios we have added support for awaitable validation handlers and basically enabling the asynchronous validation. Using them is similar to regular validator. Instead of using `Validator` we need to use `AsyncValidator` parameter.

For more information and an example just look at the [Async Validation]({{ "/docs/components/validation/#async-validation" | relative_url }}) page section.

### Flex utilities

I think this is our most advanced _fluent builder_ so far. If you're familiar with [Bootstrap Flex](https://getbootstrap.com/docs/4.5/utilities/flex/) utilities you will find our new feature quite similar. We support **all** Bootstrap Flex utilities, including the media breakpoints. The same feature is also done for all other providers, Bulma, AntDesign, Material and eFrolic.

One example of how new Flex utility works:

```html
<Div Flex="Flex.JustifyContent.Start">
    ...
</Div>

<Div Flex="Flex.InlineFlex.AlignItems.Center">
    ...
</Div>

<Div Flex="Flex.JustifyContent.Start.OnTablet.JustifyContent.End.OnDesktop">
    ...
</Div>
```

### Simplified Progress component

We have worked hard to make the `Progress` bar simpler to use to keep your fingers rested. Hey, every letter counts :)

Previously the progress component was used like:

```html
<Progress>
  <ProgressBar Value="50" />
</Progress>
```

and now you only need to write the following(for a single value progress).

```html
<Progress Value="50">
```

The old way of using the `Progress` is now reserved for stacked bars when you need to show multiple values.

### Typography utilities

All Typography utilities are now moved to the `BaseComponent` class so they can be used with any Blazorise component. This should allow for more flexibility when building and designing the UI. While it is a breaking change it shouldn't be a problem to replace all the usages. The affected components are `DisplayHeading`, `Heading`, `Paragraph`, `Text`, `CardSubtitle`, `CardTitle`, and `CardText`.

### Background color

Having the ability to set background color to any component is a must-have feature. That's why from now on, the `Background` parameter is available to all components making it much easier to customize the look of your application.

### Global Theme Size

Now it is possible to globally change input and button sizes. This will greatly improve the ability to customize the application look and feel.

### Dropping eFrolic support

Since eFrolic author has stopped maintaining eFrolic, I have decided to stop supporting it and move my focus elsewhere. This will allow me to make more work on new providers that are actively worked on.