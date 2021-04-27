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

## Migration

- In `RichTextEditOptions`, rename `DynamicLoadReferences` to `DynamicallyLoadReferences`
- Typography utilities are renamed and now they have a `Text*` prefix. Affected components are `DisplayHeading`, `Heading`, `Paragraph`, `Text`, `CardSubtitle`, `CardTitle`, and `CardText`. The changed parameters are:
  - `Color` to `TextColor`
  - `Alignment` to `TextAlignment`
  - `Transform` to `TextTransform`
  - `Weight` to `TextWeight`

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

### Typography utilities

All Typography utilities are now moved to the `BaseComponent` class so they can be used with any Blazorise component. This should allow for more flexibility when building and designing the UI. While it is a breaking change it shouldn't be a problem to replace all the usages. The affected components are `DisplayHeading`, `Heading`, `Paragraph`, `Text`, `CardSubtitle`, `CardTitle`, and `CardText`.
