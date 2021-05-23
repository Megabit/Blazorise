---
title: "Blazorise 0.8.7"
permalink: /docs/release-notes/release087/
excerpt: "Release notes for Blazorise 0.8.7"
toc: true
toc_label: "Features"
---

## Overview

Finally, after more than two months I'm proud to announce the new version of Blazorise.

**0.8.7** is both long overdue and accordingly packed with important bug fixes and enhancements.

I have intended to release the 0.8.x versions more regularly(every month or so) but then the life happened. I left the company where I worked for the last 9 years and moved to a new job. I must say that it wasn't easy to leave but I felt like it was needed for me personally and professionally. Now that I have settled I must say the change went more than good.

## Changes

As mentioned, this release includes many bug fixes, usability enhancements and documentation improvements.

### Refactoring input fields

This one gave me a lot of headache. While for the most part input fields worked just fine, there were some special cases where they would just not update the value or react to any change. Especially when working with `Validation` component. So after a while I knew I had to bite it and do the refactoring. It wasn't easy but it finally paid off.

I must thank [brettwinters](https://github.com/brettwinters) for his help and very detailed bug reports and how to reproduce them.

### Data Annotations Validator

This feature was requested several times but it just wasn't possible in an easy way in previous versions of Blazorise. But after all the hard work refactoring the input fields I realized it could finally be done. While the old _delegate validator_ is very flexible and powerful enough to be used for almost every scenario, the new validator type is very easy and also it's familiar to more people so I believe it will be of great use.

But, the code can speak for itself. so please look at the [validation page]({{ "/docs/components/validation/#data-annotations" | relative_url }}).

### Attributes splatting

A lot of people wanted this feature and I wasn't so sure about it. So, even if it feels like a hack to me, I implemented it so that Blazorise components could be more flexible. For those of you not familiar with _attributes-splatting_, essentially it's a way to pass the list of additional parameters to the underline element(s) that Blazorise component can represent. To learn more you can visit [official documentations](https://docs.microsoft.com/en-us/aspnet/core/blazor/components?view=aspnetcore-3.0#attribute-splatting-and-arbitrary-parameters).

While it's for sure a helpful feature I'm still not convinced this is the right way for Blazorise. With Blazorise I wish to have an abstraction over any html element. With attribute-splatting I'm giving a way to use component(s) _low-level_ attributes. Anyways, the feature is here and we'll see how it goes.

### DataGrid

First I need to mention some breaking changes on `DataGrid`. It's nothing to be afraid of. Essentially some of the attributes have being renamed so if you've being using them you just need to replace them with new names:

| Old name       | New name      |
|----------------|---------------|
| AllowEdit      | Editable      |
| AllowSort      | Sortable      |
| AllowFilter    | Filterable    |

That's it!

Now back to the improvements.

1. `DataGridColumn` now have additional parameters named `CellsEditableOnNewCommand` and `CellsEditableOnEditCommand`. They're used for scenarios where you need to control when and how the cell values will be edited. So now you can disable or enable editing on `New` or `Edit` grid command.
2. `DataGridCommandColumn` also have new parameters like `NewCommandAllowed`, `EditCommandAllowed`, etc. They're used to handle visibility of command buttons. So for example you can hide a `New` command for the grid, or maybe you can hide `Delete` command by setting the `{Name}CommandAllowed` to `false`. Whatever you choose you have the freedom to do so.
3. Added `RowSelectable` event handler. It's used to handle the selection of the clicked row.

### Other

- Added `VisibleCharacters` attribute on `TextEdit`. It specifies the visible width, in characters, of an `<input>` element. 

### Bug Fixes

 - [#251](https://github.com/Megabit/Blazorise/issues/251) Fixed two-way binding issue on `SelectEdit`
 - [#263](https://github.com/Megabit/Blazorise/issues/263) Fixed `Tooltip`
 - [#290](https://github.com/Megabit/Blazorise/issues/290) `DataGridCommandColumn` attribute `Width` was unused
 - [#295](https://github.com/Megabit/Blazorise/issues/295) Remove lock from CreateDotNetObjectRef
 - [#342](https://github.com/Megabit/Blazorise/issues/342) `SelectEdit` int valued items ignores 0 when IsMultiple is true
 - [#320](https://github.com/Megabit/Blazorise/issues/320) Validation not updating after reference is changed
 - [#285](https://github.com/Megabit/Blazorise/issues/285) Autocomplete
 - [#267](https://github.com/Megabit/Blazorise/issues/267) `DataGridNumericColumn` does not sort numerically

## Contributors

I also need to mention these people that took some time and helped with this release.

 - [#311](https://github.com/Megabit/Blazorise/pull/311) Chart Options Not Updating [@andrewwilkin](https://github.com/andrewwilkin)
 - [#310](https://github.com/Megabit/Blazorise/pull/310) Fix so that Autocomplete dropdown doesn't show empty dropdown [@robalexclark](https://github.com/robalexclark)
 - [#321](https://github.com/Megabit/Blazorise/pull/321) Fix custom class provider registration [@mszyszko](https://github.com/mszyszko)
 - [#299](https://github.com/Megabit/Blazorise/pull/299) Removing unused classes [@WillianGruber](https://github.com/WillianGruber)
 - [#347](https://github.com/Megabit/Blazorise/pull/347) Font Awesome icon styles [@cassioesp](https://github.com/cassioesp)

Guys, thank you!

## Closing notes

Right now, I'm going to give my best to finish next minor version(0.8.8) as soon as possible. The very next thing after that is going to be a long awaited **0.9** that will bring some major upgrades. Hopefully it will not take as long as 0.8.7.

And as always if you enjoy working with Blazorise please leave a star on [GitHub](https://github.com/Megabit/Blazorise) or click on the star-badge bellow. Also consider becoming a [Patron](https://www.patreon.com/mladenmacanovic) or donate via [Buy me a Coffee](https://www.buymeacoffee.com/mladenmacanovic) or [PayPal](https://www.paypal.me/mladenmacanovic)!

Thanks!

<iframe src="https://ghbtns.com/github-btn.html?user=Megabit&repo=Blazorise&type=star&count=true" frameborder="0" scrolling="0" width="170px" height="20px"></iframe>