---
title: "Blazorise 0.9 release notes"
classes: wide
categories:
  - Release Notes
tags:
  - blazorise
  - release notes
  - 0.9
  - changes
---

Time surely flies. It's already being three months since the last major release. Many new components and improvements on existing components have being made.

## Breaking changes

Before we continue it's good to mention that with this release comes a lot of breaking changes. I know this is not a popular decision but Blazorise being still in development stage and **1.0** behind a corner I feel this is the perfect time to clean some decisions from the past and introduce some new APIs. So without further ado let us start:

### Renamed properties

This is by far the largest refactor in this release and a lot of components is touched with this change. Basically this is one of the first [issues](https://github.com/stsrki/Blazorise/issues/4) created after the Blazorise was first released. Back then Blazor did not have case-sensitive support when naming components and properties. So whenever there was a clash like `button` and `Button` or `disabled` and `Disabled` it would just break. So I had to introduce prefixes to component properties like `IsDisabled` or `IsActive`. Personally I hated it but it was necessary back then. Now that Blazor has fixed this limitation it was the perfect time to also go through all of the components and remove the prefixes. As a consequence I think the API is now a lot cleaner and easier to write. Since the change is too big, listing every change in this post will not make too much sense. Instead you can go to this [PR](https://github.com/stsrki/Blazorise/pull/536) and see all changes listed.

### Refactored components

- `SelectEdit` component is renamed to `Select`.
- `CheckEdit` component is renamed to `Check` to be more in line with new `Radio` and `Switch` components. It is also converted to generic component so existing properties like `NullableChecked` and `NullableCheckedChanged` are removed as they we're not needed any more.
- Property `ColumnSize` is removed from all input components(TextEdit, NumericEdit, Select, etc.). From now on column sizes must be defined on the container components like `Field` and `FieldBody`.
- `DateEdit` is converted to generic component. Until now it was accepting only nullable `DateTime?` as a value. From now valid types are `DateTime` and `DateTimeOffset`, including nullable types. To upgrade all you need is to add `TValue` parameter, eg.

  ```html
  <DateEdit TValue="DateTime">
  ```

- `Tabs` component now uses new template parameters(`Items` and `Content`). While old examples will still work, it is advised to convert your tabs to the [new structure]({{ "/docs/components/tab/#example" | relative_url }}) as it's much easier to define and handle.

  ```html
  <Tabs>
    <Items>
      ..
    </Items>
    <Content>
      ...
    </Content>
  </Tabs>
  ```

- It's now preferred to place `Field` component inside of `Validation` container.

  ```html
  <Validation Validator="@ValidationRule.IsEmail">
    <Field Horizontal="true">
    ...
    </Field>
  </Validation>
  ```

## New Components

### Live Charts

This one took me a long time to build. I had to come up with a way to handle third party extensions made for [Charts.js](https://www.chartjs.org/) without breaking existing API too much. In the end API changed just slightly in terms that existing chart methods are converted to `async`.

- `void Clear()` > `Task Clear()`
- `void AddLabel()` > `Task AddLabel()`
- `void AddDataSet()` > `Task AddDataSet()`
- `void Update()` > `Task Update()`

This change allowed me better control over the chart data and it's options. But that was just the beginning. Most of the things are done under the hood to allow dynamic changes on the chart data. Also it's now easier to add custom plugins for chartjs. First plugin I decided to add is the [chartjs-plugin-streaming](https://nagix.github.io/chartjs-plugin-streaming/). With the help of this plugin your data can now be animated while data is coming or streaming.

It has it's own NuGet package named `Blazorise.Charts.Streaming`, available [here](https://www.myget.org/feed/blazorise/package/nuget/Blazorise.Charts.Streaming). The streaming API is fairly simple to use and you see an example in the documentation on [chart page]({{ "/docs/extensions/chart/#streaming" | relative_url }}).

### File Upload

FileEdit component was created long time ago but I must admit it wasn't used at all. After Steve Sanderson has posted [file upload](https://blog.stevensanderson.com/2019/09/13/blazor-inputfile/) implementation on his blog I decided to give it a shot and include it into FileEdit. This component is based on his implementation but it isn't a full copy. While Steve's component worked on most of the files it broke randomly on files larger than 25MB, or so. I had to make some tweaks here and there and I managed to create a component that is capable of uploading files of any size. I must also give my thanks to [iberisoft](https://github.com/iberisoft) for testing the component after my changes!

To learn more about file component please look at the [documentation]({{ "/docs/components/file/" | relative_url }}).

### Anchor Link

Another component that is made by the help of [community post](https://mikaberglund.com/2019/12/28/creating-anchor-links-in-blazor-applications/) is the new `Link` component. The new component is used for any navigation on your SPA and also for anchor links on landing pages. The old `LinkBase` is removed and replaced with `Link` component. Please read the [documentation]({{ "/docs/components/link/" | relative_url }}) to learn more.

There is also a great landing page theme made by [richbryant](https://github.com/richbryant) that can be found on [GitHub](https://github.com/richbryant/SinglePage) and that is using a Blazorise Link component to make it work.

### Layout

Originally this was not going to part of a **v0.9**. While I was working on provider for AntDesign I liked how they had special layout component(s) to structure the page. I wanted to see how it would translate to Blazorise so I can use it instead of current custom structure in the demo app. It worked quite good, but I didn't want to loose too much time working on it, so I just stashed it until later. At the same time [@MitchellNZ](https://github.com/MitchellNZ) opened new [ticket](https://github.com/stsrki/Blazorise/issues/700) with the request for the very same component(s). So instead we both agreed for him to join me and to finish the Layout component. I must admit without his help this feature would be laying around for a long time. As a result it's now part of a **v0.9** and as a bonus with it the entire demo app is structured fully by Blazorise components without any help of native elements or custom CSS.

### Radio and Switch

The new `Radio` and `Switch` components are based on the existing `Check` component. But since both of them have their own specific use cases I have decided to split them up. This allowed me to have more flexibility when defining specific features and styles.

[Radio]({{ "/docs/components/radio" | relative_url }}) components is used to select one of multiple choices, so I have also introduced new `RadioGroup` component. The change is not just cosmetics as now the new group can act as a container for radios and it also supports validation.

[Switch]({{ "/docs/components/switch" | relative_url }}) component was requested many times to be created. It is similar to `Check` component but is more suited to toggle the state of a single setting on or off.

### Other components

[TimeEdit]({{ "/docs/components/time" | relative_url }}) is similar to [DateEdit]({{ "/docs/components/date" | relative_url }}) but is limited to only accept time as a value. Just like DateEdit it is also a generic component.

[ColorEdit]({{ "/docs/components/color" | relative_url }}) is a simple component around native element `input type="color"`. For now it just shows default color dialog. In the future the plan is to expand on it and implement custom dialog instead of native(and ugly) one.

[Divider]({{ "/docs/components/divider" | relative_url }}) is a thin line that groups content in lists and layouts.

## List of Features and Bug Fixes

- [#612](https://github.com/stsrki/Blazorise/issues/612) Make BaseAutocomplete.CurrentSearch
- [#358](https://github.com/stsrki/Blazorise/issues/358) Snackbar colors and theme support
- [#576](https://github.com/stsrki/Blazorise/issues/576) Snackbar Closed event
- [#570](https://github.com/stsrki/Blazorise/issues/570) DataGrid events converted to `EventCallback`
- [#296](https://github.com/stsrki/Blazorise/issues/296) Cleaned Tabs API
- [#549](https://github.com/stsrki/Blazorise/issues/549) `CloseReason` for closable components like Modal
- [#493](https://github.com/stsrki/Blazorise/issues/493) DataGrid multi sorting
- [#539](https://github.com/stsrki/Blazorise/issues/539) DataGrid sorting icon
- [#509](https://github.com/stsrki/Blazorise/issues/509) Limit NumericEdit max and min based on it's value type
- [#516](https://github.com/stsrki/Blazorise/issues/516) DataGrid prev and next templates
- [#506](https://github.com/stsrki/Blazorise/issues/506) Support for MVVM with ICommand
- [#617](https://github.com/stsrki/Blazorise/issues/617) DataGrid Row click events
- [#681](https://github.com/stsrki/Blazorise/issues/681) Some Components can not be styled
- [#656](https://github.com/stsrki/Blazorise/issues/656) FileUpload does not allow ValidationErrors to be displayed
- [#668](https://github.com/stsrki/Blazorise/issues/668) Navigation marked as obsolete
- [#492](https://github.com/stsrki/Blazorise/issues/492) [DataGrid] Add First and Last pagination buttons
- [#529](https://github.com/stsrki/Blazorise/issues/529) DataGrid styling
- [#527](https://github.com/stsrki/Blazorise/issues/527) [DataGrid] OnSaveCommand not working when Data is used with EF
- [#633](https://github.com/stsrki/Blazorise/issues/633) DataGridColumn Filter Template (PR #639)
- [#491](https://github.com/stsrki/Blazorise/issues/491) [DataGrid] Add Alignment to column
- [#490](https://github.com/stsrki/Blazorise/issues/490) [DataGrid] Add Format attribute to DataGridColumn
- [#569](https://github.com/stsrki/Blazorise/issues/569) [Bug] Scroll problem with sidebar
- [#662](https://github.com/stsrki/Blazorise/issues/662) Possible memory leak on Dispose
- [#657](https://github.com/stsrki/Blazorise/issues/657) Size-attribute conflict in `<Select>` component
- [#394](https://github.com/stsrki/Blazorise/issues/394) Custom filtering for DataGrid
- [#489](https://github.com/stsrki/Blazorise/issues/489) [DataGrid] Aggregate columns
- [#722](https://github.com/stsrki/Blazorise/issues/722) `.b-body-layout` is not working Bulma
- [#571](https://github.com/stsrki/Blazorise/issues/571) Prevent users from injecting their own components
- [#677](https://github.com/stsrki/Blazorise/issues/677) Slider component not initializing as expected with values over 100 (#727)
- [#351](https://github.com/stsrki/Blazorise/issues/351) Automatically set Breadcrumb active state based on navigation url
- [#605](https://github.com/stsrki/Blazorise/issues/605) Option to control data-grid column visibility in display mode
- [#627](https://github.com/stsrki/Blazorise/issues/627) DataGrid Filter box placement #742
- [#575](https://github.com/stsrki/Blazorise/issues/575) Adding `@attributes` to Blazorise.Components such as SelectList


## Contributors

## Closing notes