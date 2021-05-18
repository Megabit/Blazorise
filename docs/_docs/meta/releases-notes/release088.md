---
title: "Blazorise 0.8.8"
permalink: /docs/release-notes/release088/
excerpt: "Release notes for Blazorise 0.8.8"
toc: true
toc_label: "Features"
---

## Overview

Happy new year! After a brief holiday I'm pleased to announce the finished Blazorise **0.8.8**. This release comes packed with many new features and bug fixes.

## Features

### DataGrid

Since DataGrid is the biggest and most complex component within Blazorise it is only natural that it will be worked on and improved in every release. 

The biggest change in `0.8.8` is the ability to work with large datasets without loading them entirely in memory. This is accomplished by introducing the new `ReadData` event handler. The new event is responsible for loading and assigning the data back to the DataGrid. Blazorise isn't opinionated on how to load the data. So you're free to use whatever. Be it a call to Rest-API, database call, etc. [see more]({{ "/docs/extensions/datagrid/#large-data" | relative_url }})

### Icon Styles

Thanks to the help from [@WillianGruber](https://github.com/WillianGruber) now it's possible to define icon style along with the icon name. Currently supported styles are `Solid`, `Regular`, `Light` and `DuoTone`. If nothing is specified the default will be `Solid`.

## Breaking changes

### PageChanged

The signature of DataGrid `PageChanged` event is changed from using a string as an event argument to using of the new `DataGridPageChangedEventArgs`. This way it's easier to handle the paginations.

| Before           | After                                         |
|------------------|-----------------------------------------------|
| `Action<string>` | `EventCallback<DataGridPageChangedEventArgs>` |

### RowSaving

Removed `RowSaving` event and replaced it with two additional events. `RowInserting` and `RowUpdating` are now used to handle separate operations that were previously handled by one event and it was difficult to distinguish the insert from update. With these new events this will now be much more easier.

## All Changes

List of all bug fixes and features in this release.

### Features

- [#345](https://github.com/Megabit/Blazorise/issues/345) Control Gradient Colors in Theme generator
- [#446](https://github.com/Megabit/Blazorise/issues/446) Support for character casing
- [#332](https://github.com/Megabit/Blazorise/issues/332) Added Focus() for input component
- [#481](https://github.com/Megabit/Blazorise/issues/481) Text attributes for text-based components

### Bug Fixes

- [#433](https://github.com/Megabit/Blazorise/issues/433) Modal default button for ENTER
- [#328](https://github.com/Megabit/Blazorise/issues/328) Implement Skip().Take() on DataGrid
- [#296](https://github.com/Megabit/Blazorise/issues/296) DataGrid no longer scrolls after Popup
- [#352](https://github.com/Megabit/Blazorise/issues/352) BarBrand theme color
- [#368](https://github.com/Megabit/Blazorise/issues/368) Outlined Button Color after click
- [#337](https://github.com/Megabit/Blazorise/issues/337) Fixed IsRounded in theme generator
- [#346](https://github.com/Megabit/Blazorise/issues/346) Checkbox component color (material)
- [#357](https://github.com/Megabit/Blazorise/issues/357) Snackbar Location offset
- [#307](https://github.com/Megabit/Blazorise/issues/307) Tooltip on Button in ButtonGroup breaks ButtonGroup
- [#326](https://github.com/Megabit/Blazorise/issues/326) RowRemoved EventCallback still called if RowRemoving Action is cancelled
- [#344](https://github.com/Megabit/Blazorise/issues/344) ModalBody with MaxHeight vertical scroll position is not reset on 2nd show
- [#360](https://github.com/Megabit/Blazorise/issues/360) NumericEdit not working with @bind-Value
- [#300](https://github.com/Megabit/Blazorise/issues/300) Autocomplete not calling SelectedValueChanged
- [#471](https://github.com/Megabit/Blazorise/issues/471) Alert Close Button
- [#329](https://github.com/Megabit/Blazorise/issues/329) Better handling of DataGrid pagination links

## Contributors

Big thanks to all the contributors!

- [#460](https://github.com/Megabit/Blazorise/pull/460) DataGrid improvements with selected rows [@Herdo](https://github.com/Herdo)
- [#464](https://github.com/Megabit/Blazorise/pull/464) Added flag to collapse Sidebar item dynamically [@koryphaee](https://github.com/koryphaee)
- [#425](https://github.com/Megabit/Blazorise/pull/425) Fixed bug with too many DataGrid paginations links [@ricardoromaobr](https://github.com/ricardoromaobr)
- [#347](https://github.com/Megabit/Blazorise/pull/347) FontAwesome Icon Styles [@WillianGruber](https://github.com/WillianGruber)

## Closing notes

That leaves us with **0.8.8**. From now on, all my effort will be put to the next [0.9 milestone](https://github.com/Megabit/Blazorise/milestone/23). It will be the biggest release in awhile, and will bring many new components, new CSS provider(s) and a lot of refactoring. Right now I don't have plan to work on any 0.8.**x** unless there are some major bugs that cannot be waited until 0.9.

And as always if you enjoy working with Blazorise please leave a star on [GitHub](https://github.com/Megabit/Blazorise) or click on the star-badge bellow. Also consider becoming a [Patron](https://www.patreon.com/mladenmacanovic) or donate via [Buy me a Coffee](https://www.buymeacoffee.com/mladenmacanovic) or [PayPal](https://www.paypal.me/mladenmacanovic)!

Thanks!

<iframe src="https://ghbtns.com/github-btn.html?user=Megabit&repo=Blazorise&type=star&count=true" frameborder="0" scrolling="0" width="170px" height="20px"></iframe>