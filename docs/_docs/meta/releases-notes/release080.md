---
title: "Blazorise 0.8"
permalink: /docs/release-notes/release080/
excerpt: "Release notes for Blazorise 0.8"
toc: true
toc_label: "Features"
---

## Overview

It took more time than it was planned but it was worth it. This release is bringing some of the biggest and the most requested features.

## Breaking changes

### Blazor preview 7

Before continuing I must say that the new version is also upgraded to the Blazor **preview 7**. While not much has changed with the new version of Blazor it has finally fixed bug with _EventCallback_ and generic components. This means that from now it is possible to use things like `SelectedValueChanged` on `SelectEdit` component without getting a compile error.

Blazor Preview 7 also introduced some breaking changes with project dependencies. I had to remove target framework `netcoreapp3.0` and also FrameworkReference `Microsoft.AspNetCore.App`. So because of that Blazorise initialization is slightly different. You can find examples in the [readme](https://github.com/Megabit/Blazorise#client-side) or in [usage]({{ "/docs/usage/" | relative_url }}) section

## Enhancements

### ThemeProvider

One of the most requested features from Blazorise community was theming support. The result is [ThemeProvider]({{ "/docs/theming/" | relative_url }}) that can give a fresh look to Blazorise application by customizing colors, elements, borders, etc.

I was planning to add theming for a long time but could not find best way to do it. It needed to be flexible and easy enough to support future modifications for all of the current CSS providers and also for those coming in the future. Finally it is done, so now it is left for community to test it and see where it goes from here. It will definitely not going to stop here as there is going to be many more features and ideas to expand theming.

### DataGrid

[DataGrid]({{ "/docs/extensions/datagrid/" | relative_url }}) was also one of the most requested component so here it is. It supports all of the things that are usually associated with data grid components. The things like sorting, filtering, paging and editing. All of that can be done in the new DataGrid component.

This component is the perfect example of how flexible Blazorise can be. It is done 100% with only Blazorise component. There's not a single line of _html_ tags other than the built-in Blazorise components.

### Edit Mask

While it looks like it doesn't do much, it can be very useful. With this feature now it's possible to prevent users from entering invalid values into text and numeric fields. For example you can limit user to enter strings like phone numbers, email address, etc. by using RegEx. You can also define decimal places and decimal separator for numeric fields. More features will also come for the EditMask in the future.

### Smaller features

- _Action_ event upgraded to _EventCallback_ now that Blazor preview 7 has fixed the bug.
- Added `SelectGroup` component that can be used to group select items into categories for better UX.
- Added `Accordion` component to expand onto existing `Collapse` components.
- Support for boolean values on `SelectEdit`
- Added RowSpan and ColumnSpan on TableRowCell

### Bug Fixes

- Fixed bug with Guid values on `SelectEdit`
- Fixed memory leak when using _DotNewObjectRef_
- Removed unused classes
- Fixed bug when serializing chart Data and Options

## Contributors

Also, thanks to [John Bird](https://github.com/johnbirdau) for taking time to fix bug with serializing chart `Data` and `Options` attributes.