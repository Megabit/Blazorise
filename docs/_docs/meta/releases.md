---
title: "Releases"
permalink: /docs/releases/
excerpt: "Complete list of Blazorise versions so far and a short description of added features, fixed bugs, breaking changes and how to apply them."
toc: true
toc_label: "Version history"
---

## 0.8.8.4 (quick fix)

- Upgrade all projects to .Net Core 3.1.2 and Blazor 3.2-preview2

For detail description of changes please look at [v0.8.8.4 release page]({{ "/docs/release-notes/release0884/" | relative_url }})

## 0.8.8.3 (quick fix)

Reupload NuGet package.

For detail description of changes please look at [v0.8.8.3 release page]({{ "/docs/release-notes/release0883/" | relative_url }})

## 0.8.8.2 (quick fix)

### Bug Fixes

- [#518](https://github.com/stsrki/Blazorise/issues/518) Alerts are always visible
- [#517](https://github.com/stsrki/Blazorise/issues/517) Style missing from Table components

For detail description of changes please look at [v0.8.8.2 release page]({{ "/docs/release-notes/release0882/" | relative_url }})

## 0.8.8.1 (quick fix)

### Bug Fixes

- [#494](https://github.com/stsrki/Blazorise/issues/494) Missing SortDirection.None
- [#496](https://github.com/stsrki/Blazorise/issues/496) InvalidOperationException: Unsupported type System.UInt64 for NumericEdit

For detail description of changes please look at [v0.8.8.1 release page]({{ "/docs/release-notes/release0881/" | relative_url }})

## 0.8.8

### Breaking changes

- [#478](https://github.com/stsrki/Blazorise/issues/478) Refactor DataGrid PageChanged event
- [#441](https://github.com/stsrki/Blazorise/issues/441) RowInserting and RowUpdating event handlers

### Features

- [#477](https://github.com/stsrki/Blazorise/issues/477) DataGrid loading for large data-source
- [#343](https://github.com/stsrki/Blazorise/issues/343) Better support for icons styles
- [#345](https://github.com/stsrki/Blazorise/issues/345) Control Gradient Colors in Theme generator
- [#446](https://github.com/stsrki/Blazorise/issues/446) Support for character casing
- [#332](https://github.com/stsrki/Blazorise/issues/332) Added Focus() for input component
- [#481](https://github.com/stsrki/Blazorise/issues/481) Text attributes for text-based components

### Bug Fixes

- [#433](https://github.com/stsrki/Blazorise/issues/433) Modal default button for ENTER
- [#328](https://github.com/stsrki/Blazorise/issues/328) Implement Skip().Take() on DataGrid
- [#296](https://github.com/stsrki/Blazorise/issues/296) DataGrid no longer scrolls after Popup
- [#352](https://github.com/stsrki/Blazorise/issues/352) BarBrand theme color
- [#368](https://github.com/stsrki/Blazorise/issues/368) Outlined Button Color after click
- [#337](https://github.com/stsrki/Blazorise/issues/337) Fixed IsRounded in theme generator
- [#346](https://github.com/stsrki/Blazorise/issues/346) Checkbox component color (material)
- [#357](https://github.com/stsrki/Blazorise/issues/357) Snackbar Location offset
- [#307](https://github.com/stsrki/Blazorise/issues/307) Tooltip on Button in ButtonGroup breaks ButtonGroup
- [#326](https://github.com/stsrki/Blazorise/issues/326) RowRemoved EventCallback still called if RowRemoving Action is cancelled
- [#344](https://github.com/stsrki/Blazorise/issues/344) ModalBody with MaxHeight vertical scroll position is not reset on 2nd show
- [#360](https://github.com/stsrki/Blazorise/issues/360) NumericEdit not working with @bind-Value
- [#300](https://github.com/stsrki/Blazorise/issues/300) Autocomplete not calling SelectedValueChanged
- [#471](https://github.com/stsrki/Blazorise/issues/471) Alert Close Button
- [#329](https://github.com/stsrki/Blazorise/issues/329) Better handling of DataGrid pagination links

For detail description of changes please look at [v0.8.8 release page]({{ "/docs/release-notes/release088/" | relative_url }})

## 0.8.7.2

## Changes

 - [#408](https://github.com/stsrki/Blazorise/issues/408) Upgrade to .Net Core 3.1

For detail description of changes please look at [v0.8.7.2 release page]({{ "/docs/release-notes/release0872/" | relative_url }})

## 0.8.7.1

## Bug Fixes

 - [#384](https://github.com/stsrki/Blazorise/issues/384) Fix favicon in demo apps
 - [#395](https://github.com/stsrki/Blazorise/issues/395) Autocomplete no longer works in 0.8.7

For detail description of changes please look at [v0.8.7.1 release page]({{ "/docs/release-notes/release0871/" | relative_url }})

## 0.8.7

### Breaking changes

- Renamed DataGrid attributes

  | Old name       | New name      |
  |----------------|---------------|
  | AllowEdit      | Editable      |
  | AllowSort      | Sortable      |
  | AllowFilter    | Filterable    |

### Enhancements

- Refactoring input fields
- [#120](https://github.com/stsrki/Blazorise/issues/120) Data Annotations Validator support
- [#236](https://github.com/stsrki/Blazorise/issues/236) Attributes splattering for all components
- [#236](https://github.com/stsrki/Blazorise/issues/236) VisibleCharacters attribute on TextEdit
- [#234](https://github.com/stsrki/Blazorise/issues/234) Control over Edit and New command on DataGrid
- Ability to handle the editing of cell values by `CellsEditableOnNewCommand` and `CellsEditableOnEditCommand`
- Ability to handle visibility of command buttons by `NewCommandAllowed`, `EditCommandAllowed`, etc.
- Ability to control `DataGrid` row select state

### Bug Fixes

 - [#251](https://github.com/stsrki/Blazorise/issues/251) Fixed two-way binding issue on `SelectEdit`
 - [#263](https://github.com/stsrki/Blazorise/issues/263) Fixed `Tooltip`
 - [#290](https://github.com/stsrki/Blazorise/issues/290) `DataGridCommandColumn` attribute `Width` was unused
 - [#295](https://github.com/stsrki/Blazorise/issues/295) Remove lock from CreateDotNetObjectRef
 - [#342](https://github.com/stsrki/Blazorise/issues/342) `SelectEdit` int valued items ignores 0 when IsMultiple is true
 - [#320](https://github.com/stsrki/Blazorise/issues/320) Validation not updating after reference is changed
 - [#285](https://github.com/stsrki/Blazorise/issues/285) Autocomplete
 - [#267](https://github.com/stsrki/Blazorise/issues/267) `DataGridNumericColumn` does not sort numerically

For detail description of changes please look at [v0.8.7 release page]({{ "/docs/release-notes/release087/" | relative_url }})

## 0.8.6.4

### Breaking changes

- Upgraded to the .NET Core 3.1 Preview 3

For detail description of changes please look at [v0.8.6.4 release page]({{ "/docs/release-notes/release0864/" | relative_url }})

## 0.8.6.3

### Breaking changes

- Upgraded to the .NET Core 3.1 Preview 2

For detail description of changes please look at [v0.8.6.3 release page]({{ "/docs/release-notes/release0863/" | relative_url }})

## 0.8.6.2

### Breaking changes

- Upgraded to the .NET Core 3.1 Preview 1

For detail description of changes please look at [v0.8.6.2 release page]({{ "/docs/release-notes/release0862/" | relative_url }})

## 0.8.6

### Breaking changes

- Upgraded to the .NET Core 3.0
- Manual static files

For detail description of changes please look at [v0.8.6 release page]({{ "/docs/release-notes/release086/" | relative_url }})

## 0.8.5

### Breaking changes

- [#258](https://github.com/stsrki/Blazorise/issues/210) Upgrade to 3.0.0-preview9.19457.4

### Enhancements

- [#191](https://github.com/stsrki/Blazorise/issues/191) Optimize classname builders
- [#145](https://github.com/stsrki/Blazorise/issues/145) Using 'enter' as a keypress for SimpleButton
- [#252](https://github.com/stsrki/Blazorise/issues/252) SelectEdit : Handling for No-Match
- [#167](https://github.com/stsrki/Blazorise/issues/167) Chart Events Support
- [#225](https://github.com/stsrki/Blazorise/issues/225) Add missing sub-component to Figure
- [#226](https://github.com/stsrki/Blazorise/issues/226) Feature Request : `<ValidationNone></ValidationNone>`

### Bug Fixes

- [#244](https://github.com/stsrki/Blazorise/issues/244) Dropdown going out of bounds
- [#162](https://github.com/stsrki/Blazorise/issues/162) Snackbar not closing (Server-side)
- [#248](https://github.com/stsrki/Blazorise/issues/248) launchSettings.json warning
- [#222](https://github.com/stsrki/Blazorise/issues/222) Validations.ClearAll() Fails to clear validations for second field
- [#230](https://github.com/stsrki/Blazorise/issues/230) NumericEdit Decimals Property Handling

For detail description of changes please look at [v0.8.5 release page]({{ "/docs/release-notes/release085/" | relative_url }})

## 0.8.4

### Breaking changes

- Upgraded to the Blazor preview 9
- Renamed `MouseEventArgs` to `BLMouseEventArgs`

For detail description of changes please look at [v0.8.4 release page]({{ "/docs/release-notes/release084/" | relative_url }})

## 0.8.3

### Breaking changes

- Components should be case sensitive [#8](https://github.com/stsrki/Blazorise/issues/8)

### Enhancements

- New Tooltip component [#113](https://github.com/stsrki/Blazorise/issues/113)
- Filter method added to DataGrid [#169](https://github.com/stsrki/Blazorise/issues/169)
- DataGrid styles and missing features [#204](https://github.com/stsrki/Blazorise/issues/204)

### Bug Fixes

- Every chart shows up at Line Chart on latest package upgrade [#210](https://github.com/stsrki/Blazorise/issues/210)

For detail description of changes please look at [v0.8.3 release page]({{ "/docs/release-notes/release083/" | relative_url }})

## 0.8.2

### Breaking changes

- Upgrade to Blazor preview 8 [#199](https://github.com/stsrki/Blazorise/issues/199)

### Enhancements

- Added DetailRowTemplate to DataGrid [#184](https://github.com/stsrki/Blazorise/issues/184)
- Dynamic building of Sidebar [#157](https://github.com/stsrki/Blazorise/issues/157)

### Bug Fixes

- Fixed first time selection from SelectList [#188](https://github.com/stsrki/Blazorise/issues/188)

For detail description of changes please look at [v0.8.2 release page]({{ "/docs/release-notes/release082/" | relative_url }})

## 0.8.1

### Enhancements

- Added support for nested fields in datagrid column [#165](https://github.com/stsrki/Blazorise/issues/165)
- Added an option to clear selected value and search field in the `Autocomplete` component. [#150](https://github.com/stsrki/Blazorise/issues/150)
- Added keyboard navigation for filtered items.
- Added Size attribute for search field.
- Theming options for Snackbar

### Bug Fixes

- Fixed bug with wrong color filing for charts in server-side Blazor [#115](https://github.com/stsrki/Blazorise/issues/115)
- Visibility not working for Edit components [#183](https://github.com/stsrki/Blazorise/issues/183)
- CardTitle "Size" property ignored was ignored for Material CSS [#149](https://github.com/stsrki/Blazorise/issues/149)

For detail description of changes please look at [v0.8.1 release page]({{ "/docs/release-notes/release081/" | relative_url }})

## 0.8

### Enhancements

- Upgraded to the Blazor preview 7.
- New [DataGrid]({{ "/docs/extensions/datagrid/" | relative_url }}) component extension.
- New [ThemeProvider]({{ "/docs/theming/" | relative_url }}).
- EditMask added to TextEdit
- New Accordion component
- Added SelectGroup for SelectEdit

### Bug Fixes

- Fixed bug when serializing chart Data and Options

For detail description of changes please look at [v0.8 release page]({{ "/docs/release-notes/release080/" | relative_url }})

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

- Fixed loading buttons for material and eFrolic CSS.

## 0.7.5

### Enhancements

- A new attribute `NullableChecked` on `CheckEdit` component is introduced to handle nullable boolean values (bool?).

### Bug Fixes

- Fixed bug when first value in `SelectEdit` was null while validation is set to automatic.
- Fixed CSS for vertical `Tabs` so they don't appear as broken.

## 0.7.4

### Enhancements

- New Autocomplete component
- New DropdownList component
- New SelectList component

## 0.7.3

### Enhancements

- Minor optimizations in _ClassMapper_ so that the initialization of components should now be little faster.

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
- Action converted to EventCallback for `TextEdit` and `Button` components. This means that StateHasChanged will be called automatically for you.
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
