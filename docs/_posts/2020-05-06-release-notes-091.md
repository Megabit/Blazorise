---
title: "v0.9.1 release notes"
permalink: /news/release-notes/091/
classes: wide
categories:
  - Release Notes
tags:
  - blazorise
  - release notes
  - 0.9.1
  - changes
  - quick fix
---

An old Croatian saying _Ispeci pa reci_, would roughly translate _Think before you speak_. That perfectly explains what happened with Blazorise v0.9.1. Initially, I said it would take me about a month to finish it, and it's been over two months now. I guess I should be more careful with predictions in the future.

The good news is that new version is finally done, and with more features and bug fixes than it was initially planned. Thanks to the help of  many contributors.

So without further ado, let's get into what's new in this release of Blazorise.

## Breaking changes

First we need to cover breaking changes. There's not many of them but they need to be mentioned at least.

- `DataGrid` styling properties are replaced with new styling actions so it's now possible to dynamically control the row styling
  - `RowClass` and `RowStyle` replaced with `RowStyling`
  - `FilterRowClass` and `FilterRowStyle` replaced with `FilterRowStyling `
  - `GroupRowClass` and `GroupRowStyle` replaced with `GroupRowStyling`
- All properties from `DropdownMenu` moved to it's parent component `Dropdown`
- `Visibility` property replaced with new `Display` utilities
- ChartOptions `Axe` field renamed into `Axis`

## Major New Features

### Vertical Bar

Probably the biggest and most complicated change in this version is the new vertical mode for [Bar]({{ "/docs/components/bar" | relative_url }}) component. For a long time I wanted to have it, but knowing it would be really complex I just postponed it every time. 

So I finally decided to give it a go for v0.9.1. At the same time [@MitchellNZ](https://github.com/MitchellNZ), one of the community members, contacted me and asked to help me with some of the features scheduled for Bar component. In the end all the work on vertical mode and improvements for Bar component was done by him, with me only guiding and helping with final optimization regarding the theme support and other smaller issues. The amount of work he has put into it, all in his spare time, long nights and on weekends, is mind-blowing. Really I cannot thank him enough times.

Some major new features of vertical bar:

- Collapse mode
- Small icons in collapsed mode
- Popup menus
- Theming support

Describing all of the features of new vertical mode would not be enough so it's best to just show it in action. 

<img src="/assets/news/release-091/bar-example.gif" alt="Vertical bar example" />

### Carousel

The new `Carousel` component was requested multiple times over [Gitter](https://gitter.im/stsrki/Blazorise), so I guess it was only fair to finally make it. 

Initially I though it would be fairly easy, but I must admit it was harder than I anticipated. As always AntDesign had it's own strange logic for the carousel component, so that's one problem I had to solve. 
Another problem was support for animating slides, I didn't want to use any JavaScript so this one is still unsolved. But even in this non-animated state, I think it's ready to be used.

_So what does it have?_ It has all the basic features like looping panels, clickable indicators, previous/next buttons, and configurable loop time.

To learn more, just visit the [Carousel]({{ "/docs/components/carousel" | relative_url }}) page.

### Jumbotron

Blazorise components were always more of "admin-page" oriented. So having Carousel component already done, I figured another component for "frontend" pages wouldn't hurt. There's not much I can say about it, except that it allows you to add a **full width banner**  to your webpage, which can optionally cover the full height of the page as well.

Example of [Jumbotron]({{ "/docs/components/jumbotron" | relative_url }}) can be found in the documentation.

### TreeView

This component is actually a new extension for Blazorise done by [@robalexclark](https://github.com/robalexclark), also one of the community members. He created the TreeView component near the end of v0.9.1 development stage, just in time to make it part of the release. The component is fairly easy to use, so just go to the [TreeView]({{ "/docs/extensions/treeview" | relative_url }}) page to learn how to get started with it. 

Big thanks to Robin for making it!

### Validation summary

Instead of having validation message for each validated field, now it is possible to place a `<ValidationSummary>` component that will act as a placeholder for all the errors and messaged raised by the `<Validations>` component. It is used in places where you don't want to break your UI with too many error messages.

### Closable Badge

Not that big of a change but it is good to have. You can define `CloseClicked` event on [Badge]({{ "/docs/components/badge" | relative_url }}) component, and it will automatically show you the close button.

### Theming

A lot of work was put to theming support in this release. 

- Change color of vertical Bar
- Option to define Sidebar width and height
- Luminance threshold for calculating the text color contrast
- Fixed modal footer buttons for Material provider
- Options to define media breakpoints
- Default colors for AntDesign when Theme is not used

### Display utilities

Similar to `Margin` and `Padding` utilities, the new `Display` utility is also created with fluent syntax in mind. It is inspired by the Bootstrap [display](https://getbootstrap.com/docs/4.5/utilities/display/) and [flex](https://getbootstrap.com/docs/4.5/utilities/flex/) utilities, but all packed under the one Display utility.

Just a basic usage of new utility:

```html
<Paragraph Display="Display.None.Block.OnFullHD">
    hide on screens smaller than lg
</Paragraph>
```

## Cleaning & Optimizations

### Delete unused components

- Panel
- Navigation
- NavigationItem

### Cascading stores instead of separate properties

## Other Changes

### Features

- [#758](https://github.com/stsrki/Blazorise/issues/758) Optimize demo apps for mobile
- [#760](https://github.com/stsrki/Blazorise/issues/760) Upgrade Bootstrap provider
- [#658](https://github.com/stsrki/Blazorise/issues/658) Add validation support to work similar to ValidationSummary.
- [#567](https://github.com/stsrki/Blazorise/issues/567) Implement Bar Color as a specific parameter in BarThemeOptions
- [#763](https://github.com/stsrki/Blazorise/issues/763) Option to make vertical Bar
- [#766](https://github.com/stsrki/Blazorise/issues/766) Toggle Animation not working with Bar component
- [#281](https://github.com/stsrki/Blazorise/issues/281) Add ability to display close button on badge
- [#350](https://github.com/stsrki/Blazorise/issues/350) Sidebar width and Bar height
- [#369](https://github.com/stsrki/Blazorise/issues/369) Button text colors for success and info
- [#407](https://github.com/stsrki/Blazorise/issues/407) Datagrid mobile friendly
- [#413](https://github.com/stsrki/Blazorise/issues/413) Button colors
- [#412](https://github.com/stsrki/Blazorise/issues/412) Error when using SelectEdit with dictionary in DataGrid
- [#430](https://github.com/stsrki/Blazorise/issues/430) Modal Footer Buttons do not follow theme
- [#447](https://github.com/stsrki/Blazorise/issues/447) DetailRowTemplate - Remove empty td or change size
- [#483](https://github.com/stsrki/Blazorise/issues/483) Scroll not working after NavigationManager navigation
- [#564](https://github.com/stsrki/Blazorise/issues/564) How should these theme attributes be filled?
- [#672](https://github.com/stsrki/Blazorise/issues/672) Dropdown and DropdownMenu have a RightAligned property
- [#676](https://github.com/stsrki/Blazorise/issues/676) Add a SetFocus method
- [#693](https://github.com/stsrki/Blazorise/issues/693) ChartOptions improvement
- [#708](https://github.com/stsrki/Blazorise/issues/708) Add Display utilities
- [#660](https://github.com/stsrki/Blazorise/issues/660) Theme options to define layout grid breakpoints
- [#790](https://github.com/stsrki/Blazorise/issues/790) Figure is not responsive
- [#834](https://github.com/stsrki/Blazorise/issues/834) Default colors for AntDesign
- [#851](https://github.com/stsrki/Blazorise/issues/851) Expose EditContext as an attribute of Validations
- [#792](https://github.com/stsrki/Blazorise/issues/792) error with charts after redrawing
- [#847](https://github.com/stsrki/Blazorise/issues/847) Modal scrolls to top of
- [#955](https://github.com/stsrki/Blazorise/issues/955) Remove Visibility property

### Bug Fixes

- [#622](https://github.com/stsrki/Blazorise/issues/622) [DataGrid] [Material] Issue with DetailRowTemplate in Editable DataGrid
- [#664](https://github.com/stsrki/Blazorise/issues/664) Switch Component Cursor Property doesn't work
- [#646](https://github.com/stsrki/Blazorise/issues/646) Theme ColorOptions Primary also changes breadcrumb link color
- [#686](https://github.com/stsrki/Blazorise/issues/686) Null reference exception when DataGridColumn Field is not set
- [#661](https://github.com/stsrki/Blazorise/issues/661) Auto Validation: Form Validation, error message is displayed on first load.
- [#871](https://github.com/stsrki/Blazorise/issues/871) Sidebar not colapsing on media breakpoints
- [#875](https://github.com/stsrki/Blazorise/issues/875) Streaming charts - howto assign specific "datastreams" to specific datasets
- [#898](https://github.com/stsrki/Blazorise/issues/898) Column Width in Material
- [#923](https://github.com/stsrki/Blazorise/issues/923) Don't call JSRunner during prerendering
- [#655](https://github.com/stsrki/Blazorise/issues/655) Memo cursor jumps to end when enclosed in a field and Text is bound
- [#733](https://github.com/stsrki/Blazorise/issues/733) TextEdit jump caret to the end of the text for every typed char
- [#946](https://github.com/stsrki/Blazorise/issues/946) Unhandled Exception behavior in UI events
- [#961](https://github.com/stsrki/Blazorise/issues/961) ChartOptions : Scales object has typo in "Axe"

## PRs

- [#785](https://github.com/stsrki/Blazorise/pull/785) fix: layout issue for responsive design
- [#896](https://github.com/stsrki/Blazorise/pull/896) datagriddetail columnspan fix
- [#928](https://github.com/stsrki/Blazorise/pull/928) Check if component is rendered before calling JSRunner in Dispose()
- [#947](https://github.com/stsrki/Blazorise/pull/947) Add flask icon to Font Awesome
- [#948](https://github.com/stsrki/Blazorise/pull/948) DataGrid aggregate column shift fix
- [#958](https://github.com/stsrki/Blazorise/pull/958) Add ChartColor.FromHtmlColorCode() method
- [#968](https://github.com/stsrki/Blazorise/pull/968) EmptyTemplate and LoadingTemplate
- [#962](https://github.com/stsrki/Blazorise/pull/962) Add CardDeck
- [#987](https://github.com/stsrki/Blazorise/pull/987) TreeView component extension
- [#985](https://github.com/stsrki/Blazorise/pull/985) ChartJS : convert Options object to Dictionary to fix null values issue