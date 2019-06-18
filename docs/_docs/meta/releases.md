---
title: "Releases"
permalink: /docs/releases/
excerpt: "Complete list of Blazorise versions so far and a short description of added features, fixed bugs, breaking changes and how to apply them."
toc: true
toc_label: "Version history"
---

## 0.7.8

### Enhancements

- Added `Style` attribute to `CheckEdit` component. [#112](https://github.com/stsrki/Blazorise/issues/112)
- Added `Cursor` attribute on `CheckEdit` component. [114](https://github.com/stsrki/Blazorise/issues/114)
- html events(_onchange_, _onclick_) renamed to have an @ prefix according to the Blazor preview-6
- Optimized the rendering of custom registered components.
- Added `Closing` attribute on modal to prevent it from closing. [#125](https://github.com/stsrki/Blazorise/issues/125)

## 0.7.7

### Bug Fixes

- Upgraded to the Blazor **v3.0.0-preview6.19307.2**.

## 0.7.6

### Bug Fixes

- Fixed loading buttons for material and eFrolic css.

## 0.7.5

### Enhancements

- A new atribute `NullableChecked` on `CheckEdit` component is introduced to handle nullable boolean values (bool?).

### Bug Fixes

- Fixed bug when first value in `SelectEdit` was null while validation is set to automatic.
- Fixed css for vertical `Tabs` so they don't apear as broken.

## 0.7.4

### Enhancements

- New Autocomplete component
- New DropdownList component
- New SelectList component

## 0.7.3

### Enhancements

- Minor optimizations in _ClassMapper_ so that the initialisation of components should now be little faster.

## 0.7.2

### Breaking Changes

- Upgraded to the latest Blazor **v3.0.0-preview5-19227-01**.

## 0.7.1

### Enhancements

- `BarToggle` is now automatically opening and closing `BarMenu` [#76](https://github.com/stsrki/Blazorise/issues/76)
- `CheckedChanged` event on `CheckEdit` is converted to `EventCallback` [#83](https://github.com/stsrki/Blazorise/issues/83)

### Bug Fixes

- Collapse component was always opened [#77](https://github.com/stsrki/Blazorise/issues/77)
- Server-side Demo application was crashing when navigating between pages [#78](https://github.com/stsrki/Blazorise/issues/78)
- `BarToggler` is not visible on mobile for eFrolic demo application [#80](https://github.com/stsrki/Blazorise/issues/80)

## 0.7.0

### Breaking Changes

- Upgraded to the latest Blazor **v3.0.0-preview4-19216-03**.
- `ButtonSize` instead of `Size` enum when defining the button sizes [#67](https://github.com/stsrki/Blazorise/issues/67)

### Enhancements

- New CSS provider [eFrolic](https://efrolicss.com/). [Demo](https://efrolicdemo.blazorise.com)
- Additional attributes and features for Table components [#66](https://github.com/stsrki/Blazorise/issues/66)
- Close modal by pressing the Esc key [#48](https://github.com/stsrki/Blazorise/issues/48)
- InputMode for TextEdit component [#53](https://github.com/stsrki/Blazorise/issues/53)
- Styling options for Chart components [#42](https://github.com/stsrki/Blazorise/issues/42)
- `Step` attribute on `NumericEdit` converted to decimal type [#74](https://github.com/stsrki/Blazorise/issues/74)

## 0.6.4-preview1-19154-02

**Note:** The preview number (**19154-02**) is used to indicate that the current version of Blazorise is working on the preview version of Blazor and/or .Net Core 3.0!
{: .notice--warning}

### Enhancements

- Clicked event for table row and cell components [#64](https://github.com/stsrki/Blazorise/issues/64)

## 0.6.3-preview1-19154-02

### Enhancements

- Pattern attribute(regex) for TextEdit, NumericEdit and DateEdit components
- EventCallback for DropdownItem and BarDropdownItem [#62](https://github.com/stsrki/Blazorise/issues/62)

### Bug Fixes

- Cannot consume scoped service IJSRuntime [#58](https://github.com/stsrki/Blazorise/issues/58)
- Validation for TextEdit and NumericEdit is now working with default values

## 0.6.2-preview1-19154-02

### Enhancements

- Added support for Razor Components project types
- Added EventCallback to CloseButton component

## 0.6.1-preview1-19154-02

### Enhancements

- Added new [NumericEdit]({{ "/docs/components/numeric/" | relative_url }}) component.
- Action converted to EventCallback for `TextEdit` and `SimpleButton` components. This means that StateHasChanged will be called automatically for you.
- Attribute `IsRightAligned` added on `BarDropdownMenu`. This is to allow the menu to be right-aligned to the parent element.

### Bug Fixes

- Fixed bug with the charts when hovering after the chart is refreshed.

## 0.6.0-preview7-19154-02

### Enhancements

- Upgraded to the newest Blazor v0.9.0-preview3-19154-02.

## 0.6.0-preview(x)-19104-04

### Breaking Changes

- Upgraded to the newest Blazor v0.8-preview-19104-04. The preview number(19104-04) is used to match the preview number of Blazor!

  To upgrade your project to the new version you should first need to install new Blazor before installing Blazorise. To install Blazor look at the [official](https://blogs.msdn.microsoft.com/webdev/2019/02/05/blazor-0-8-0-experimental-release-now-available/) documentation. 

  **Note:** Keep in mind that after upgrading you will only be able to work in Visual Studio 2019!
  {: .notice--info}

- Added registration method for Blazorise with an optional configuration in which it's now possible to configure some of the internal settings and behaviour.

  ```cs
  public void ConfigureServices( IServiceCollection services ) { 
    services
      .AddBlazorise( options =>
      {
        options.ChangeTextOnKeyPress = true;
      } )
      .AddBootstrapProviders()
      .AddFontAwesomeIcons();
  }
  ```
- Since `SelectEdit` and `SelectItem` are now generics there are some special rules to be followed when using them. Please look at the [Select]({{ "/docs/components/select/" | relative_url }}) page to find more.

### Enhancements

- Added new `Validation` component for input fields. [link]({{ "/docs/components/validation/" | relative_url }})
- `SelectEdit` and `SelectItem` are converted to generic components and now they supports all kinds of data types and not just strings.
- Optimized ClassMapper and fluent builders. Now they're about 50-60% percent faster when generating class names.

## 0.5.x

### Breaking Changes

- Changed how to register and use Blazorise CSS providers and icons. eg:

  ```cs
  public void ConfigureServices( IServiceCollection services ) { 
    services
      .AddBootstrapProviders()
      .AddFontAwesomeIcons();
  }

  public void Configure( IComponentsApplicationBuilder app ) {
    app
      .UseBootstrapProviders()
      .UseFontAwesomeIcons();
  }
  ```

  See the [usage page]({{ "/docs/usage/" | relative_url }}) for each provider on how to use it.
- Icons moved to a separate project and now must be installed as a Nuget package.
- `BarDropdown` is refactored to use new `BarDropdownMenu` component. This was necessary to have more control and also for bar-dropdown to have a similar structure as regular dropdown. 
- `BarDropdownToggler` renamed to `BarDropdownToggle`

### Enhancements

- Added "Empty" providers to allow using of extensions without CSS providers, see [usage]({{ "/docs/usage/#empty-provider" | relative_url }})
- Finished chart extension. [link]({{ "/docs/extensions/chart/" | relative_url }})
- `Modal` and `Dropdown` components will now be closed automatically when clicked anywhere on the page.

### Bug Fixes

- Fixed bar-dropdown for Bulma provider.

## 0.4.1

### Bug Fixes

- Blazorise depenedency was not included for NuGet packages (Bootstrap, Material, Charts, etc.)

## 0.4.0

### Enhancements

- Updated to Bootstrap v4.2.1
- Removed gijgo library for date input to make it work in native mode. This is a prerequisite for later to be able to create an extensions for date inputs.
- Chart end Sidebar moved to an extensions subfolder.
- Added support for `bind-*` attribute on `SelectEdit`.

### Bug Fixes

- Modal dialogs for Bulma and Material providers.
- Vertical tabs for Bulma implementation.
- Fixed SelectedValueChanged on `SelectEdit` when multiple select is used.
- Dropdown arrow for Bulma

## 0.3.0

### Breaking Changes

  - To allow better flexibility when defining horizontal fields a new tag `FieldBody` is introduced. Horizontal fields are now defined like this:

  ```html
  <Field IsHorizontal="true">
      <FieldLabel ColumnSize="ColumnSize.Is2">Name</FieldLabel>
      <FieldBody ColumnSize="ColumnSize.Is10">
          <TextEdit Placeholder="Some text value..." />
      </FieldBody>
  </Field>
  ```

### Enhancements

- New Breadcrumb components
- Ability to add custom rules for Margin, Padding and ColumnSize
- Added ModalTitle component, Title argument on ModalHeader is now **obsolete**
- Better implementation of Bulma css framework
- Snackbar moved from Blazorise to the Extensions folder

### Bug Fixes

- Paragraph classname was "pagination"

## 0.2.2

### Enhancements

- TabContent renamed to TabsContent
- Added text color to Heading

### Bug Fixes

- BarLink tag is null

## 0.2.1

### Enhancements

- Renamed breakpoints on fluent builders for column size and margin/padding
- Added Screenreader visibility to labels
- Added comments to enums
- Smaller refactoring and bug fixes

### Bug Fixes

- Fixed active links on SidebarLink and BarLink

## 0.2.0

### Enhancements

- Refactored all components to allow a custom implementations in providers while still having a component with the same name in the base project
