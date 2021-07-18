---
title: "Blazorise 0.9.4 is out and ready for big leagues"
permalink: /news/release-notes/094/
classes: wide
categories:
  - Release Notes
tags:
  - blazorise
  - release notes
  - 0.9.4
---

No matter how fast I think each Blazorise version will be done it always comes to be a quarter of the year. This time it's no different and based on the experience so far, I guess we'll have to follow the same release cycle in the future. Only the time will tell.

In any case the new major Blazorise `v0.9.4` is here.

Don't let yourself fooled into thinking `v0.9.4` is just a small update from `v0.9.3`. The road to `v1.0` is long and steady, and the main reason for small numbers for major releases is for poor planning at the beginning and letting myself to lost version numbers. But hey, it can happen to anyone 🤷‍♂️.

---

A lot of good things has happened before the last major release. The biggest change is that recently Blazorise has [went commercial]({{ "/news/changes-to-blazorise-license" | relative_url }}) and we are now offering Blazorise support and licensing for commercial organizations. I would say the start was good, and while there were some negative feedback, the change was mainly positive. One of the good things is that we were able to hire [@David-Moreira](https://github.com/David-Moreira) to become the first official Blazorise team member. David was already quite active with submitting to Blazorise codebase and it was only natural to offer him a new role. As a result he has done some very good things with the new DataGrid features 💪.

## Breaking changes

Unfortunately, with every new major release we introduce some breaking changes. This time is no exception so we'll try to at least offer some background and explain the reasons.

1. Textual utilities also went through the changes, so instead of having `Color` or `Alignment` parameter we now have `TextColor` and `TextAlignment`. While it is _still_ not a breaking change the original parameters will be removed in the `v0.9.5`. The good news is that now text utilities can be used on **every** component and not just on typography components like it was until now.
2. We made some small optimizations on the Modal dialog and as a result, `Dialog` parameter is removed from `ModalContent`. It was used only by the Bulma provider and so we made it to be used implicitly by the framework if some conditions are met.
3. We changed the `SearchValue` on DataGrid filter from `string` to `object`. This was needed so that you could implement better filtering without DataGrid formating your numeric or date values into the unsupported format values.
4. `Progress` component went through some optimizations and as a result we removed the `Background` parameter and replaced it with the `Color` parameter.

## Migration

- In `RichTextEditOptions`, rename `DynamicLoadReferences` to `DynamicallyLoadReferences`
- Typography utilities are renamed and now they have a `Text*` prefix. Affected components are `DisplayHeading`, `Heading`, `Paragraph`, `Text`, `CardSubtitle`, `CardTitle`, and `CardText`. The changed parameters are:
  - `Color` to `TextColor`
  - `Alignment` to `TextAlignment`
  - `Transform` to `TextTransform`
  - `Weight` to `TextWeight`
- `Progress` component:
  - Use only `Progress` component because `ProgressBar` is now needed only for multiple stacked bars, eg. `<Progress Value="50" />`
  - Replace `Background` parameter with the `Color` parameter
- Remove usage of `Dialog` parameter for `ModalContent`.
- Change `Tooltip` `Placement` parameter to `TooltipPlacement`, eg. `Placement="Placement.Left"` to `Placement="TooltipPlacement.Left"`
- For DataGrid `ReadData` event callback, rename `Direction` attribute to `SortDirection`
- Datagrid: Due to a refactoring of the resizable feature to the Table component, you should note the following:
  - The datagrid resources(JS and CSS) we had previously introduced, do not need to be added to your application in this version. However we kept the blazorise.datagrid.js, even if blank for increased flexibility in supporting the datagrid throughout the release.
    - If you had any of the resources below, you can now safely remove them:
      - `<script src="_content/Blazorise.DataGrid/blazorise.datagrid.js"></script>`
  - The `DataGridResizeMode` enum no longer exists, and you will need to use `TableResizeMode` enum instead.
  - DataGrid `FilteredDataChanged` now accepts the `DataGridFilteredDataEventArgs<TItem>` as the argument instead of `IEnumerable<TItem>`
  - On `DataGridColumn`, rename `Direction` parameter to `SortDirection`
  - Whenever you use filter `SearchValue` change its type from `string` to `object` type
- Charts
  - Move `BarThickness` and `MaxBarThickness` from `BarChartOptions` to the `Axis` option.

## Highlights 🚀

### Async Validation

There are situations when you need to do validation by using the external method or a service. Since calling them can take some time it not advised to do it synchronously as that can lead to pretty horrible UI experience. So to handle those scenarios we have added support for awaitable validation handlers and basically enabling the asynchronous validation. Using them is similar to regular validator. Instead of using `Validator` we need to use `AsyncValidator` parameter.

For more information and an example just look at the [Async Validation]({{ "/docs/components/validation/#async-validation" | relative_url }}) page section.

### Services

For the long time this was one of the most requested features and we finally introduce it.

- Message Service is used to show simple messages and confirmation dialogs to which the user can respond. It contains some of the standard methods like `Info`, `Success` or `Warning`, and also `Confirm` method for use cases when you need to wait for the user action.

- Notification Service is used to show simple alerts and notifications with a small timeout after which it will auto-close.

- PageProgress Service is used to show simple progress bar at the top of the page.

To learn more about both components please visit [Message Service]({{ "/docs/services/message" | relative_url }}), [Notification Service]({{ "/docs/services/notification" | relative_url }}) and [PageProgress Service]({{ "/docs/services/page-progress" | relative_url }}) pages.

### Rating component

A lot of times you need an ability to allow users to rate your product or a service. With our new `Rating` component you will be able to do so. The usage is very simple and can be done with just one line of code, eg.

```html
<Rating Color="Color.Primary" @bind-SelectedValue="@SelectedValue" MaxValue="10" />
```

<img src="/assets/images/news/094/rating.jpg" alt="Rating example" />

To learn more about the component please visit [Rating]({{ "/docs/components/rating" | relative_url }}) page.

### Markdown component

New Markdown component is also part of the release. It is based on the excellent and well maintained [Easy MarkDown Editor](https://easy-markdown-editor.tk/) JavaScript library. It has all the basic editing features like bold, italic, code snippets, headings, etc. With time we hope to introduce and support even more once the people start using it.

<img src="/assets/images/news/094/markdown.jpg" alt="Markdown example" />

To learn more about the component please visit [Markdown]({{ "/docs/extensions/markdown" | relative_url }}) page.

### DataGrid data-annotations

This feature was requested for far too many times and we can finally say it is now ready. Data-annotations can now be used for validating the DataGrid edit fields. This feature is now the default option once `UseValidation` on `DataGrid` is enabled. If you want to have any other validation method, like `Validator` for example, you just need to define it on `DataGridColumn` and it will override default data-annotation. We hope this new feature will help you even more in building your applications.

### Carousel animations

A lot of refactoring went into Carousel component and we now fully support slide animations. Crossfade animations are also supported.

<video width="480" height="270" controls>
  <source src="/assets/images/news/094/carousel.webm" type="video/webm">
  Your browser does not support the video tag.
</video>

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

The old way of using the `Progress` will still work but is now better suited for stacked bars when you need to show multiple values.

### Typography utilities

All Typography utilities are now moved to the `BaseComponent` class so they can be used with any Blazorise component. This should allow for more flexibility when building and designing the UI. While it is a breaking change it shouldn't be a problem to replace all the usages. The affected components are `DisplayHeading`, `Heading`, `Paragraph`, `Text`, `CardSubtitle`, `CardTitle`, and `CardText`.

### Background color

Having the ability to set background color to any component is a must-have feature. That's why from now on, the `Background` parameter is available to all components making it much easier to customize the look of your application.

### Global Theme Size

Now it is possible to globally change input and button sizes. This will greatly improve the ability to customize the application look and feel.

### Dropping eFrolic support

Since eFrolic author has stopped maintaining eFrolic, I have decided to stop supporting it and move my focus elsewhere. This will allow me to make more work on new providers that are actively worked on.

### Autocomplete improvements

-  `FreeTyping` binding support && `FreeTyping` validation support
   - You are now able to use `Autocomplete` as a suggestions source to the user. Meaning you can enable this feature, and `Autocomplete` will accept any value introduced by the user and bind it on the `SelectedText` parameter.
   - By enabling this feature, you will still have validation support, now on `SelectedText`.

-  `NotFound` EventCallback && `NotFoundContent`
   -  You will now be able to handle cases where the value introduced by the user is not found on the data source that has been provided to `Autocomplete`. Either by providing some feedback to the user, by using the `NotFoundContent` or just handling it as you wish by listening to the `NotFound` EventCallback.

-  Show all items
   -  This can be done by just setting the `Autocomplete`'s MinLength to 0. We've made sure to provide initial styling to accommodate all items with a scrollbar.

-  Add support for a custom filter
   -  `Autocomplete` already provides filtering capabilities out of the box. However, you may now provide a custom filter that's based on the currently text being searched and the data-source items.

-  Improve the selection box to have same width as text field by default && Able to limit suggestions shown at a time with a scroll bar
   -  These two improvements will provide a better user experience by having the selection box be compliant to the text field's width, as well as handling multiple items, by providing a scroll bar, which would otherwise go out of screen.

### Datagrid Virtualization Support

With .NET5.0 Blazor brought us built-in virtualization support with the Virtualize component. Now Datagrid brings that same Virtualization support to you!

By setting `Virtualize`, you will enable virtualize capabilities on the datagrid, meaning that instead of having pagination, you'll be able to scroll across the data with perceived improved performance.

Virtualization is a technique for limiting UI rendering to just the parts that are currently visible. For example, virtualization is helpful when the app must render a long list of items and only a subset of items is required to be visible at any given time.

You will still have access to every available datagrid feature.
`VirtualizeOptions` allows to further customize the `Virtualize` feature.

### Table FixedHeader ScrollTo support

You are now able to use the table's internal API to either scroll to a row or pixel offset from the start of `tbody`.

Here's an example, of how to scroll to the first row on the table.

```csharp
private Task ScrollToRow()
    => tableRef.ScrollToRow( 1 ).AsTask();
```

And an example, of how to scroll to the position of 250px.

```csharp
private Task ScrollToPixels()
    => tableRef.ScrollToPixels( 250 ).AsTask();
```