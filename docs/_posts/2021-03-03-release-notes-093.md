---
title: "v0.9.3 release notes"
permalink: /news/release-notes/093/
classes: wide
categories:
  - Release Notes
tags:
  - blazorise
  - release notes
  - 0.9.3
---

After many months of hard work, I'm proud to announce the very new Blazorise. Four months have passed from the last major release, and I'm not going to lie, it was not easy. Considering a lot of improvements were done, and a lot of bugs were fixed with this release, it was all worth it in the end.

With each new release, I try to clean the API the way I originally planned it. While it is not the most popular thing, in the end, it will bring us closer to `v1.0`. And with this release, `v1.0` is closer than ever.

So, let's get going and see what has changed.

## Breaking changes 💥

As always there are some breaking changes. If you don't want to know the reasons you can skip this section and just read the [Migration]({{ "/news/release-notes/093/#migration-" | relative_url }}) section below.

### .Net 5

Blazorise is now running fully on .Net 5, and .Net Core 3.1 is not supported anymore. This decision was hard but needed, mainly because of a new `IComponentActivator` that finally gave me a way to initialize components the way I wanted it from the start. Until now I had to do a lot of workarounds to make every component overridden for each of the supported CSS providers. While it worked, for the most part, it was slow and unoptimized, meaning each component had to be created twice. Not any more. `IComponentActivator` gave me a way to register custom components through [DI](https://en.wikipedia.org/wiki/Dependency_injection) and then initialize only those components that I want. As a result, Blazorise should now be a lot faster, and finally, I can implement some outstanding features that were impossible until now.

### ModalBackdrop removed

The placement of the backdrop `div` container is very important for some CSS providers like Bootstrap. If placed inside of Modal `div`(like it was done so far) it can break the scroll-bar or have some other undesired side effects. So, `ModalBackdrop` component had to go and it is now completely handled by the `Modal` component. This way we can have full control over where to place the backdrop container. In case there is a need to hide `ModalBackdrop`, the new `ShowBackdrop` parameter can be used.

### CloseButton behavior

The default behavior of `CloseButton` is now changed so that it will auto-close its parent component. For example, if you place `<CloseButton />` inside of `Alert` or `Modal`, once clicked it will now automatically close them.

This behavior can be controlled by setting the `AutoClose="false"` parameter(default is `AutoClose="true"`), or it can be changed with global settings:  `.AddBlazorise( options => { options.AutoCloseParent = false; } )`.

### Datagrid new dependencies

Datagrid now has new resource dependencies.
When using Datagrid, you must now additionally add the Datagrid resources to your app:

`<link href="_content/Blazorise.DataGrid/blazorise.datagrid.css" rel="stylesheet" />`

`<script src="_content/Blazorise.DataGrid/blazorise.datagrid.js"></script>`

## Migration 🛠

- Migrate your .Net Core 3.1 project to .Net 5 by following [Microsoft migration guide](https://docs.microsoft.com/en-us/aspnet/core/migration/31-to-50?view=aspnetcore-5.0&tabs=visual-studio).
- Remove all Blazorise methods that start with `.Use`(`.UseBootstrapProviders()`, `.UseFontAwesomeIcons()`).
- Remove all usages of `<ModalBackdrop>`.
- Disable auto-close of modals and alerts(in case you don't want new behavior). In `Startup.cs ` add `.AddBlazorise( options => { options.AutoCloseParent = false; } )`.
- Add new dependencies for Datagrid in `index.html` or `_Host.cshtml` file:
  - `<link href="_content/Blazorise.DataGrid/blazorise.datagrid.css" rel="stylesheet" />`
  - `<script src="_content/Blazorise.DataGrid/blazorise.datagrid.js"></script>`
- For `RadioGroup` replace `Inline="true"` parameter with `Orientation="Orientation.Horizontal"`
- For `SnackbarStack` replace `Push(...)` method with `PushAsync(...)`.
- `SnackbarAction` button must be placed inside of `SnackbarBody`.
- Add `TValue` and `TItem` to Autocomplete and DropdownList components.

## Highlights 🚀

### Snackbar

A lot of UX and internal improvements are done on `Snackbar` component. We added an option to delay the closing of the snackbar automatically if a user is clicked on a snackbar directly. The first click will delay the close event and the second click will close it immediately. The behavior can be controller by new `DelayCloseOnClick` and `DelayCloseOnClickInterval` parameters.

Other changes are the introduction of new `SnackbarHeader` and `SnackbarFooter` components. This will allow more freedom to make snackbar to appear as a classic Toast component. This change also brings a slight breaking change because from now on `SnackbarAction` must be placed inside of `SnackbarBody`, `SnackbarHeader`, and `SnackbarFooter`.

`SnackbarStack` has also received some of the changes. For start, `Push` method is removed and it is replaced with the new `PushAsync` method. It can now receive `message` and a `title` parameters, while other options are controlled through the `options` builder.

### Datagrid: Multiple Selection

Introduced multiple selection support for DataGrid component. This was done by introducing a new enum `DataGridSelectionMode` defined by the `SelectionMode` parameter. You can now select between the default `DataGridSelectionMode.Single` or the new `DataGridSelectionMode.Multiple`.

Use the new `DataGridSelectionMode.Multiple` to enable multiple selection on Datagrid. Clicking rows will now select multiple records at a time. You can now access them by using the `SelectedRows` and also bind to the `SelectedRowsChanged` event callback. The single selection behavior is maintained, so the last clicked record will still register as the selected record provided by the `SelectedRow` parameter.

Optionally you can use the new Datagrid column `<DataGridMultiSelectColumn>` to enable a checkbox column that works exclusively with multiple selections. 
You can either use your own `MultiSelectTemplate` render fragment to customize the input that will appear in the column and trigger the multiple selections by then binding to the provided `SelectedChanged` event callback or just use the provided default by not specifying a `MultiSelectTemplate` render fragment. When using this extra column, the top row-column, will provide the ability to select or unselect all rows.

An example can be found in [DataGrid]({{ "/docs/extensions/datagrid/#datagrid-multiple-selection" | relative_url }}) page section.

### Datagrid: Button Row

Introduced a new Button Row to Datagrid. You can now provide a Button Row Template that will render your template in the pager section.
The template has access to the internal commands so you're also able to construct your own buttons on the pager that can also trigger the Datagrid's CRUD and clear filter operations.

A new enum `DataGridCommandMode` was also introduced so you are able to control if you'd like to show both the commands and the new Button Row or just either one of them.

An example can be found in [DataGrid]({{ "/docs/extensions/datagrid/#datagrid-button-row" | relative_url }}) page section.

### Datagrid: Resizable

Introduced resize feature to Datagrid. You are now able to set the new `Resizable` parameter to `true` and you'll be able to resize the DataGrid columns. 

### Localization

A default .Net localization, more specifically `IStringLocalizer` is not a good option for single-page apps. I guess it is OK for regular .Net Core server-side apps but for Blazor single-page apps where everything has to be dynamic, it just doesn't make sense. All I wanted was just a basic feature. The ability to change languages while the app is running. No-can-do! `IStringLocalizer` caches language on app startup and there is no way to change it later. Whenever you change the language you must refresh and reload the page 🤯. I hate it.

So, naturally, I had to implement my own localization that will support:

- Dynamically change the language
- JSON files for resources
- Add custom languages while the app is running
- Ability to override localization for each component individually

As a result I created `ITextLocalizer` and `ITextLocalizerService`.

`ITextLocalizer` is used exactly the same as `IStringLocalizer`, but it behaves differently. Instead of caching everything on startup and not able to change anything afterward like `IStringLocalizer`, with `ITextLocalizer` you can read or add additional languages dynamically while the app is running. To change the language, a new `ITextLocalizerService` is used. With a simple `textLocalizerService.ChangeLanguage("fr-FR")` Blazorise components will react and redraw their localization texts, where needed. Best of all, there are no additional setup steps required like for native Blazor localization.

To learn more on how to use new localization please look at the [localization page]({{ "/docs/helpers/localization" | relative_url }})

### Validation

Many improvements have also come to the `Validation` component where a lot of refactoring was done under the hood. From the user perspective, nothing much will change, as the API is all the same. But the validation should now be much more resilient and faster.

The first and foremost change is the new `IValidationHandler` system. The new system is created to be flexible enough to allow any new validation method. This also means that anyone can create any custom validation handler and then tell `Validation` component to use it. For example, if you want to use a [Fluent Validation](https://fluentvalidation.net/) you will now be able to do.

**Example**

```html
<Validation HandlerType="typeof(FluentValidationHandler)" />
```

Along with the already mentioned `IValidationHandler`, a lot of previously known bugs were also fixed. Notably, the nasty bug with the `DateEdit` component where if you tried to enter the year part of the date it would just reset everything([#1515](https://github.com/stsrki/Blazorise/issues/1515)).

Also, `Validation` component can now work with `EditContext` coming from Blazor native `EditForm` [#996](https://github.com/stsrki/Blazorise/issues/996).

### PageProgress

A new `PageProgress` component was created to be used as a loading indicator for pages. You can learn more on [progress page]({{ "/docs/components/progress/#page-progress" | relative_url }})

### Steps

The new `Steps` is a simple navigational component that is meant to be used with the wizard-like parts of the UI. You can learn more about it on the [steps]({{ "/docs/components/step" | relative_url }}) page.

### Animate extension

Starting with this release we also bring you a great [Blazor.Animate](https://github.com/mikoskinen/Blazor.Animate) extension from [@mikoskinen](https://github.com/mikoskinen) that is now fully integrated into Blazorise. I would like to thank [@mikoskinen](https://github.com/mikoskinen) for making it possible!


## List of all changes

Usually, I would copy-paste a list of all changes along with the release notes, but this time the number of items is too big so I decided to just give you a [link](https://github.com/stsrki/Blazorise/issues/1472) instead. So click on it if you want to see every new feature or bug fix in this release.

## PRs

Every new Blazorise release brings many new users. And along with them the number of community PRs has also increased, a lot. It would be a shame to not give them the credits for improving Blazorise.

While I would like to mention all of them it would take too much space for this post 😅.

### Honorable mentions

[David-Moreira](https://github.com/David-Moreira) is one of the biggest contributors in `0.9.3`. He managed to create some great features, some of which I already mentioned. He added support for DataGrid Multiple Selection, Resizable columns, DataGrid Button Row, and many smaller improvements and bug fixes. Without his help, `0.9.3` would take a lot more time to finish.

[@njannink](https://github.com/njannink) continued to work on his `RichTextEdit` extension and improved it even more. All while fixing reported bugs and optimizing the code.

Many thanks to all of you. You're the reason why I continue to work on Blazorise. Thank you, once again!

## Closing Notes

The community around Blazorise has grown considerably and it makes me really proud to be a part of it. When I first started to work on Blazorise I couldn't even imagine how it would turn up. It has exceeded all my expectations and I would like to say thank you, to all of you, the users.

## Support

Blazorise is an open-source project developed in my spare time. It has grown a lot lately and the work I put into it takes more and more time. My future plan with Blazorise is to move it to an organization(instead of my personal account) and to hire someone to help me maintain it. If you'd like to support the project financially and help us secure our future hires, you can do so on [Patron](https://www.patreon.com/mladenmacanovic) or donate via [Buy me a Coffee](https://www.buymeacoffee.com/mladenmacanovic) or [PayPal](https://www.paypal.me/mladenmacanovic).