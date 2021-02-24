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

## Breaking changes

### .Net 5

Blazorise is now running fully on .Net 5, and .Net Core 3.1 is not supported any more. This decision was hard but needed, mainly because of a new IComponentActivator that finally gave me a way to initialize components the way I wanted it from the start. Until now I had to do a lot of workarounds to make every component overridden for each of the supported CSS providers. While it worked for the most part, it was slow and unoptimized, meaning each component had to be created twice. Not any more. IComponentActivator gave me a way to register custom components through DI and then initialize only those components that I want. As a result Blazorise should now be a lot faster, and finally I can implement some outstanding features that were impossible until now.

### ModalBackdrop removed

The placement of backdrop `div` container is very important for some CSS providers like Bootstrap. If placed inside of Modal `div`(like it was done so far) it can break scroll-bar or have some other undesired side effects. So, `ModalBackdrop` component had to go and it is now completely handled by the `Modal` component. This way we can have full control over where to place the backdrop container. In case there is a need to hide `ModalBackdrop`, the new `ShowBackdrop` parameter can be used.

### CloseButton behavior

A default behavior of `CloseButton` is now changed so that it will auto-close it's parent component. For example if you place `<CloseButton />` inside of `Alert` or `Modal`, once clicked it will now automatically close them.

This behavior can be controlled by setting the `AutoClose="false"` parameter(default is `AutoClose="true"`), or it can be changed with global settings:  `.AddBlazorise( options => { options.AutoCloseParent = false; } )`.

### Datagrid new dependencies

Datagrid now has new resource dependencies.
When using Datagrid, you must now additionally add the Datagrid resources to your app:

`<link href="_content/Blazorise.DataGrid/blazorise.datagrid.css" rel="stylesheet" />`

`<script src="_content/Blazorise.DataGrid/blazorise.datagrid.js"></script>`

### Other

- `RadioGroup`: `bool Inline` parameter replaced with enum `Orientation`.

## Features

### Snackbar

A lot of UX and internal improvements are done on `Snackbar` component. We added an option to delay closing of the snackbar automatically if user is clicked on a snackbar directly. First click will delay the close event and second click will close it immediately. The behavior can be controller by new `DelayCloseOnClick` and `DelayCloseOnClickInterval` parameters.

Other changes are the introduction of new `SnackbarHeader` and `SnackbarFooter` component. This will allow more freedom to make snackbar to appear as classic Toast component. This change also bring slight breaking change because from now on `SnackbarAction` must be placed inside of `SnackbarBody`, `SnackbarHeader` and `SnackbarFooter`.

`SnackbarStack` has also received some of the changes. For start, `Push` method is removed and it is replaced with new `PushAsync` method. It can now receive `message` and a `title` parameters, while other options are controlled through the `options` builder.

### Datagrid: Multiple Selection

Introduced multiple selection support for Datagrid. This was done by introducing a new enum `DataGridSelectionMode` defined by the `SelectionMode` parameter. You can now select between the default `DataGridSelectionMode.Single` or the new `DataGridSelectionMode.Multiple`.

Use the new `DataGridSelectionMode.Multiple` to enable multiple selection on Datagrid. Clicking rows will now select multiple records at a time. You can now access them by using the `SelectedRows` and also bind to the `SelectedRowsChanged` event callback. The single selection behavior is maintained, so the last clicked record will still register as the selected record provided by the `SelectedRow` parameter.

Optionally you can use the new Datagrid column `<DataGridMultiSelectColumn>` to enable a checkbox column that works exclusively with multiple selection. 
You can either use your own `MultiSelectTemplate` render fragment to customize the input that will appear in the column and trigger the multiple selection by then binding to the provided `SelectedChanged` event callback or just use the provided default by not specifying a `MultiSelectTemplate` render fragment. When using this extra column, the top row column, will provide the ability to select or unselect all rows.

An example can be found in [DataGrid]({{ "/docs/extensions/datagrid/#datagrid-multiple-selection" | relative_url }}) page section.

### Datagrid: Button Row

Introduced a new Button Row to Datagrid. You can now provide a Button Row Template that will render your template in the pager section.
The template has access to the internal commands so you're also able to construct your own buttons on the pager that can also trigger the Datagrid's CRUD and clear filter operations.

A new enum `DataGridCommandMode` was also introduced so you are able to control if you'd like to show both the commands and the new Button Row or just either one of them.

An example can be found in [DataGrid]({{ "/docs/extensions/datagrid/#datagrid-buttonrow" | relative_url }}) page section.

### Datagrid: Resizable

Introduced resize feature to Datagrid. You are now able to set the new `Resizable` parameter to `true` and you'll be able to resize the datagrid columns. 

### Localization

Localization _s*ck_. There, I said it. Well, at least a default .Net localization, more specifically `IStringLocalizer`. I guess it is OK for regular .Net Core server-side apps but for Blazor single-page apps where everything has to be dynamic, it just doesn't make sense. All I wanted was just a basic feature. The ability to change languages while the app is running. No-can-do! `IStringLocalizer` caches language on app startup and there is no way to change it later. Whenever you change the language you must refresh and reload the page 🤯. I hate it.

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

Along with the already mentioned `IValidationHandler`, a lot of previously known bugs were also fixed. Notably, the nasty bug with the `DateEdit` component was when you tried to enter the year part of the date wasn't possible [#1515](https://github.com/stsrki/Blazorise/issues/1515). Also, `Validation` component can now work with `EditContext` coming from Blazor native `EditForm` [#996](https://github.com/stsrki/Blazorise/issues/996).