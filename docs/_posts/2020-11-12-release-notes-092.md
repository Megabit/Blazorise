---
title: "v0.9.2 release notes"
permalink: /news/release-notes/092/
classes: wide
categories:
  - Release Notes
tags:
  - blazorise
  - release notes
  - 0.9.2
---

## Preface

Who would have thought the new version would take so long to finish?! 😅 I definitely thought it would be a lot faster. Initially, `0.9.2` was planned to be a small milestone, mainly focusing on stability improvements and bug fixes, with a minimal amount of new features. For the most part, it’s still that way. There are some new features(more on that later), but the majority of work was focus on bug fixes and optimizations. But, as always, things happen.

First I started work on a new project(which is still a secret btw), and so I didn’t have that much time to work on Blazorise.

Second, a lot more people started to use Blazorise. Naturally, bug reports and feature requests kept piling up. As a result, I couldn't say no every time, and I kept adding new tickets to the `0.9.2` milestone. I was constantly jumping between Blazorise and _other project_. Note to me - say no more often 🙃.

One good thing happened recently though. I started working with [Volosoft](https://volosoft.com/) on their open-source [ABP](https://abp.io/) framework to work on integration of Blazorise as the main Blazor UI for ABP. For the most part, their goal and my plan with Blazorise were on the same track so as a result, I managed to finish the Blazorise `0.9.2` milestone a lot faster than I would have done otherwise. Big thanks to my new friends at Volosoft!

And that concludes this section. It's time to see what has changed.

## Breaking changes

As always the first thing we need to do is to cover all of the breaking changes.

### Update to .Net 5

It's not really a breaking change but more of a _warning_. Currently, Blazorise works in multi-target framework mode, so both .Net Core 3.1.x and .Net 5 are supported in `0.9.2`. This was needed so that people code would not break with the release and give people more time to upgrade their projects. But starting from the next `0.9.3` milestone, the plan is to completely go with .Net 5. A lot of new things are planned, especially implementing the new _IComponentActivator_. This is something I wanted from the first version of Blazorise and this feature alone will improve Blazorise considerably. Not to mention speed improvement and a possibility to add some long-standing features like constructor DI, custom component IDs, element refs, and a lot more.

So it's advised to upgrade your projects as soon as possible. It would be worth it.

### Property unification and custom input sizes

This was part of the biggest change in this release(but more on that in the feature section) and a lot of breaking changes are going with it. Namely by renaming some attributes or removing them.

- Renamed Heading `TextColor` property to `Color`
- Removed enums  `ButtonSize`  and  `ButtonsSize`  and replaced them with  `Size` enum
- `<Divider>` property `Type` renamed to `DividerType?`
- Removed `ModalSize.None`
- ChartOptions renames
	- `AxeTicks`  to  `AxisTicks`
	- `AxeGridLines`  to  `AxisGridLines`
	- `AxeScaleLabel` to `AxisScaleLabel`
	- `AxeMinorTick` to `AxisMinorTick`
	- `AxeMajorTick` to `AxisMajorTick`

### Generic SelectList

The `Select` list is converted to a generic component to be more flexible. So now it's needed to define `TItem` and `TValue`. eg.

```html
<SelectList TItem="MySelectModel"
            TValue="object"
            Data="@myDdlData"
            TextField="@((item)=>item.MyTextField)"
            ValueField="@((item)=>item.MyValueField)"
            SelectedValue="@selectedListValue"
            SelectedValueChanged="@MyListValueChangedHandler" />
```

## Major New Features

### Custom Input Sizes

I already mentioned this in _breaking changes_ section, but it deserves a further explanation. With this feature all input components and buttons now use the same `Size` enum. eg.

- `<Button Size="Size.Small">Hello</Button>`
- `<TextEdit Size="Size.Small" />`

It all started with [#1131](https://github.com/stsrki/Blazorise/issues/1131). Initially all I wanted was to remove sizes like `ExtraSmall` or `ExtraLarge`, but as soon as I started working I realized it's better to keep them and instead have support for all sizes in all providers. But to have it supported by every provider it required a lot of work. Some providers like Bulma or AntDesign does not have sizes that matches Bootstrap, and vice versa. Not to mention `Check` and `Radio` boxes, `Switch`, or `Pagination` which almost didn't have any sizes. A lot of manual tweaks and custom CSS styles were required. In the end I think it was worth it and hopefully people will appreciate it and work with it.

### Bar Improvements

I think the work done by [MitchellNZ](https://github.com/MitchellNZ) deserves all the praise. Once again he selflessly worked to bring new features to the Bar component. The amount of work he put into this so far is astounding. I will just list some of the major features in this release.

- Multi-level `BarDropdownMenu`
- Closing of popup menu when clicked outside of it
- Theming support
- Bar sizing
- ...and a lot more

The full list of changes can be found at [#1042](https://github.com/stsrki/Blazorise/issues/1042)

I cannot thank [MitchellNZ](https://github.com/MitchellNZ) enough times for all his work so far.

### RichTextEdit

A community member, [njannink](https://github.com/njannink),  added support for RichTextEdit component based on QuillJS in his [PR](https://github.com/stsrki/Blazorise/pull/941). It's a really nice feature that was created even before `0.9.1` but since it was not stable enough at that time we decided to push it for `0.9.2` milestone. A lot of more work was being put into bringing it to finish and finally it was ready to be merged. So big thanks to [njannink](https://github.com/njannink) and hopefully this is not the last of his work.

The full usage and example can be found on [RichTextEdit]({{ "/docs/extensions/richtextedit" | relative_url }}) page.

### DataGrid page sizes

Another nice feature by [NoOneKnows92](https://github.com/NoOneKnows92), one of our community members. The ability to define DataGrid page sizes and pagination placement. With his [PR](https://github.com/stsrki/Blazorise/pull/1271) it's now possible:

- To have dropdown option to choose number of rows per page
- Placement of pagination buttons(top or bottom)
- Show page position and total number of rows

Big thanks to [NoOneKnows92](https://github.com/NoOneKnows92) for his work at this.

### Link Button

Maybe it is a small feature but a very useful. So far Blazorise lacked support for proper link buttons with `<a>` element. Not any more. With this release this is now possible and it's very easy to use, eg.

`<Button Type="ButtonType.Link" Color="Color.Primary" To="https://www.google.com/">Google</Button>`

The link button will appear as regular button but will behave as an ordinary link or `<a>` element with `href` attribute. You're welcome 😉!

### Validation Improvements

Probably the most complicated part of Blazorise. And the one that happen to have most bugs and feature requests. A lot of improvements was done in this release that are not apparent at first.

- Data-annotations are now working properly when `Model` changes
- Ability to define custom `EditContext` for `Validations` component
- Custom localization can be done with `MessageLocalizer` handler on `Validation` component
- `StatusChanged` event added to `Validation` component
- No breaking changes!

And a lot more of small internal improvements.

### Other Features

- [#917](https://github.com/stsrki/Blazorise/issues/917) Allow Popup Size to be Set for DataGrid edit/new Modals
- [#918](https://github.com/stsrki/Blazorise/issues/918) DataGrid. Title property for modal dialog
- [#1064](https://github.com/stsrki/Blazorise/issues/1064) DataGrid component editing mode needs to be customized
- [#1061](https://github.com/stsrki/Blazorise/issues/1061) Alert visibility two-way binding
- [#1023](https://github.com/stsrki/Blazorise/issues/1023) Add `Stacked` to chart options
- [#929](https://github.com/stsrki/Blazorise/issues/929) Theme Color Alpha
- [#870](https://github.com/stsrki/Blazorise/issues/870) DividerType in Theme DividerOptions
- [#934](https://github.com/stsrki/Blazorise/issues/934) DataGrid: Define default value for input on RowInserting
- [#949](https://github.com/stsrki/Blazorise/issues/949) Overriding NewCommand button in Grid
- [#998](https://github.com/stsrki/Blazorise/issues/998) Refactor the TextChanged mode for TextEdit and NumericEdit to be configured for each instance
- [#982](https://github.com/stsrki/Blazorise/issues/982) Is it possible to put an image in the datagrid Header instead of a string for the caption? similar to the new command in the `DataGridCommandColumn`.
- [#1089](https://github.com/stsrki/Blazorise/issues/1089) Layout Improvements
- [#1084](https://github.com/stsrki/Blazorise/issues/1084) Whole layout overlay
- [#663](https://github.com/stsrki/Blazorise/issues/663) Switch Component needs Color Property
- [#1204](https://github.com/stsrki/Blazorise/issues/1204) Implement custom Check for Bulma provider
- [#1212](https://github.com/stsrki/Blazorise/issues/1212) Implemented store on ListGroup and fixed Clicked event on ListGroupItem
- [#1205](https://github.com/stsrki/Blazorise/issues/1205) Add Disabled property to Dropdown
- [#1179](https://github.com/stsrki/Blazorise/issues/1179) Autocomplete - add local ChangeTextOnKeyPress property
- [#1177](https://github.com/stsrki/Blazorise/issues/1177) Add Reset() method to FileEdit
- [#900](https://github.com/stsrki/Blazorise/issues/900) Size Helpers
- [#1087](https://github.com/stsrki/Blazorise/issues/1087) Typography unification
- [#1243](https://github.com/stsrki/Blazorise/issues/1243) Handle OnFocus Events on Inputs
- [#1152](https://github.com/stsrki/Blazorise/issues/1152) Pop-Up Menu Width property in Vertical Bar
- [#1329](https://github.com/stsrki/Blazorise/issues/1329) Added StatusChanged event to Validation component
- [#1343](https://github.com/stsrki/Blazorise/issues/1343) Chart Ability To Remove And Set Data On Datasets
- [#967](https://github.com/stsrki/Blazorise/issues/967) DataGrid - Is it possible to force single-column sorting
- [#1000](https://github.com/stsrki/Blazorise/issues/1000) DataGrid TextAlignment does not affect caption
- [#1390](https://github.com/stsrki/Blazorise/issues/1390) BarLink Still Ignores Attributes
- [#983](https://github.com/stsrki/Blazorise/issues/983) Add multiline labels support for ChartJS component
- [#1129](https://github.com/stsrki/Blazorise/issues/1129) Expose ColumnType in the Columns property of DataGridReadDataEventArgs

## Bug Fixes

- [#1065](https://github.com/stsrki/Blazorise/issues/1065) Bootstrap ModalContent generating invalid class name
- [#1043](https://github.com/stsrki/Blazorise/issues/1043) Bar component : BarEnd is not pushed on right side if there's no BarStart Element in BarMenu
- [#1011](https://github.com/stsrki/Blazorise/issues/1011) Text-Alignment on DataGridCommandColumn seems not to work
- [#893](https://github.com/stsrki/Blazorise/issues/893) ColorEdit not aligned with horizontal label
- [#1008](https://github.com/stsrki/Blazorise/issues/1008) Chart : still some references to `Axe`
- [#952](https://github.com/stsrki/Blazorise/issues/952) FileEdit does not show filename(s) after browsing
- [#1081](https://github.com/stsrki/Blazorise/issues/1081) Addons with Select component not working for AntDesign
- [#857](https://github.com/stsrki/Blazorise/issues/857) Datagrid pager
- [#1069](https://github.com/stsrki/Blazorise/issues/1069) The LayoutFooter component is not pinned to the bottom of the window
- [#1038](https://github.com/stsrki/Blazorise/issues/1038) Sidebar under header
- [#1053](https://github.com/stsrki/Blazorise/issues/1053) TextEdit jump caret to the end of the text for every typed char, when ChangeTextOnKeyPress is true
- [#1120](https://github.com/stsrki/Blazorise/issues/1120) Material and eFrolic tab buttons don't change cursor
- [#1149](https://github.com/stsrki/Blazorise/issues/1149) Bug: DecimalsSeparator of NumericEdit does not affect Value
- [#1203](https://github.com/stsrki/Blazorise/issues/1203) In DataGrid DisplayFormat disappears when Editing Inline
- [#1185](https://github.com/stsrki/Blazorise/issues/1185) ListGroupItem Clicked event is not working
- [#1183](https://github.com/stsrki/Blazorise/issues/1183) RadioGroup CheckedValue property change is not handled
- [#1262](https://github.com/stsrki/Blazorise/issues/1262) Default column class-name needs to be removed in some cases
- [#1264](https://github.com/stsrki/Blazorise/issues/1264) Fixed ListGroup selecting mode
- [#1093](https://github.com/stsrki/Blazorise/issues/1093) Problem with vertical BarItem longer text
- [#1091](https://github.com/stsrki/Blazorise/issues/1091) Bar expand/collapse not working on mobile devices
- [#1223](https://github.com/stsrki/Blazorise/issues/1223) DataGridColumn Displayable="false" shows column in DataGridEditMode.Inline edit mode
- [#1249](https://github.com/stsrki/Blazorise/issues/1249) Non-displayable DataGridColumns are displayed when editing
- [#1092](https://github.com/stsrki/Blazorise/issues/1092) Bar default vertical scrollbar in Firefox and Edge
- [#1114](https://github.com/stsrki/Blazorise/issues/1114) BarDropdown is not working in Bulma
- [#1180](https://github.com/stsrki/Blazorise/issues/1180) BarDropdownToggle component hover/click does not work over chevron icon
- [#1332](https://github.com/stsrki/Blazorise/issues/1332) BarLink Ignores Attributes
- [#1335](https://github.com/stsrki/Blazorise/issues/1335) DateEdit ignores `Readonly` attribute
- [#1384](https://github.com/stsrki/Blazorise/issues/1384) `nav-pills` & `nav-tabs` should not be using together
- [#758](https://github.com/stsrki/Blazorise/issues/758) Optimize demo apps for mobile
- [#1433](https://github.com/stsrki/Blazorise/issues/1433) BarDropdownItem Ignores Target
- [#1441](https://github.com/stsrki/Blazorise/issues/1441) DataGrid pagination does not working
- [#1450](https://github.com/stsrki/Blazorise/issues/1450) Validation issue in Blazorise 0.9.2 rc-2
- [#1461](https://github.com/stsrki/Blazorise/issues/1461) Fixed row un-selection

## PRs

The `0.9.2` release really shows how much Blazorise has grown. It has by far the most PRs from community members. A big thanks to all of them and for all the hard work!

- [#1022](https://github.com/stsrki/Blazorise/pull/1022) Fix Convertor.ToDictionary for Arrays and Lists
- [#1023](https://github.com/stsrki/Blazorise/pull/1023) Add `Stacked` to Axis options
- [#1055](https://github.com/stsrki/Blazorise/pull/1055) Option to hide the toggle icon in a dropdown
- [#1070](https://github.com/stsrki/Blazorise/pull/1070) DataGrid. Title property for modal dialog
- [#1071](https://github.com/stsrki/Blazorise/pull/1071) DataGrid Popup sizing
- [#1077](https://github.com/stsrki/Blazorise/pull/1077) DataGrid default value setter for new items
- [#1085](https://github.com/stsrki/Blazorise/pull/1085) Add CaptionTemplate to DataGrid column header
- [#1090](https://github.com/stsrki/Blazorise/pull/1090) Layout Improvements
- [#1082](https://github.com/stsrki/Blazorise/pull/1082) Fix and extend data grid paging
- [#1098](https://github.com/stsrki/Blazorise/pull/1098) Fix: Resolved Bar issue relating to recent Layout Improvements
- [#1097](https://github.com/stsrki/Blazorise/pull/1097) Add color for switches
- [#1110](https://github.com/stsrki/Blazorise/pull/1110) Adding some kind of lazy loading to the tree view
- [#1130](https://github.com/stsrki/Blazorise/pull/1130) Page button documentation
- [#1161](https://github.com/stsrki/Blazorise/pull/1161) Fix typo in ThemeContainerMaxWidthOptions
- [#1163](https://github.com/stsrki/Blazorise/pull/1163) Fix typo in ValidBreakpoints and ValidContainerMaxWidths
- [#1159](https://github.com/stsrki/Blazorise/pull/1159) Removed duplicate Info badge and pill from demo page
- [#1132](https://github.com/stsrki/Blazorise/pull/1132) DataGrid validations
- [#1176](https://github.com/stsrki/Blazorise/pull/1176) Add VisibleCharacters to NumericEdit (#1210)
- [#941](https://github.com/stsrki/Blazorise/pull/941) RichTextEdit based on QuillJS
- [#1227](https://github.com/stsrki/Blazorise/pull/1227) Generic Typing for SelectList bound value
- [#1269](https://github.com/stsrki/Blazorise/pull/1269) Remove a validation from the parent validation set when it is disposed (@Dedac and @peterlentine)
- [#1235](https://github.com/stsrki/Blazorise/pull/1235) Ant design select component improvements
- [#1252](https://github.com/stsrki/Blazorise/pull/1252) Adds focus events for all inputs
- [#1311](https://github.com/stsrki/Blazorise/pull/1311) Vertical Bar fixes
- [#1279](https://github.com/stsrki/Blazorise/pull/1279) Added support for DateTimeOffset conversions
- [#1302](https://github.com/stsrki/Blazorise/pull/1302) Added 'Disabled' and 'Clicked'-attributes to Tab.md docs
- [#1297](https://github.com/stsrki/Blazorise/pull/1297) Update Slider value when dragged by mouse
- [#1303](https://github.com/stsrki/Blazorise/pull/1303) Fix RadioGroup bindings
- [#1325](https://github.com/stsrki/Blazorise/pull/1325) Fix for Displayable column in inline edit mode
- [#1271](https://github.com/stsrki/Blazorise/pull/1271) Added the possibility to change the page size and the position of the pager.
- [1344](https://github.com/stsrki/Blazorise/pull/1344) Add SetData on BaseChart
- [#1356](https://github.com/stsrki/Blazorise/pull/1356) Added sort mode to data grid
- [#1357](https://github.com/stsrki/Blazorise/pull/1357) TextAlign also for the table header of data grid
- [#1375](https://github.com/stsrki/Blazorise/pull/1375) fix bad author names in NuGet package

## Support

Working on Blazorise takes time and I have to make a lot of sacrifices to keep it free for all of you. My biggest wish is to work full time on Blazorise but until that is possible a little support is always appreciated. Consider becoming a [Patron](https://www.patreon.com/mladenmacanovic) or donate via [Buy me a Coffee](https://www.buymeacoffee.com/mladenmacanovic) or [PayPal](https://www.paypal.me/mladenmacanovic)!

Also help by raising a voice to Microsoft or GitHub so that Blazorise can enter their sponsorship programs.

And don't forget to leave a star on [GitHub](https://github.com/stsrki/Blazorise).

## Closing Words

What else is left to say? Other than thank you all once again! It was hard but fun ride, and to see how much Blazorise has grown recently it can only bring joy to my face. I'm happy to help all of you by doing my best.

Keep safe and see you next time! Cheers ☕