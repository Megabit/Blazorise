---
title: "Blazorise 0.8.5"
permalink: /docs/release-notes/release085/
excerpt: "Release notes for Blazorise 0.8.5"
toc: true
toc_label: "Features"
---

## Overview

For this release the main focus was on performance optimizations. But unlike previous 0.8.x versions in this release there's more new features and bug fixes.

### Breaking changes

Although it wasn't supposed to be any new preview of Blazor before hitting the final 3.0, we got the new RC1. So before we continue make sure to update your _Visual Studio to 16.3 preview 4_ and install newest _.Net Core SDK_ before you install the latest Blazorise. Please follow the [official guide](https://devblogs.microsoft.com/aspnet/asp-net-core-and-blazor-updates-in-net-core-3-0-release-candidate-1/). If you have followed all the steps you should be safe to get the latest Blazorise 0.8.5.

## Enhancements

### Classname builder

This is the biggest and the most important change in this release. The main plan for the **0.9** milestone is to refactor and optimize some parts of Blazorise. With this release we're one step closer to that goal. 

For the long time generating CSS class-names was a pretty a big bottleneck. Since class-names had to be regenerated every time a component is initialized the process can noticeably slow down rendering of the page. For regular-sized pages this is not the problem. But if the page contains a large number of components it can become a problem.

After a lot of refactoring the old `ClassMapper` is replaced with the `ClassBuilder`. Based on my benchmark calculations the new builder is around _40%_ faster for the worst-case scenarios where a lot of classes need to be randomly generated based on some condition. It could be speed for up to _2-3%_ more but that would require the change to the builder API and it would be a lot more difficult to use. These optimizations can be safely be said to be final as there is in my opinion no more noticeable room to improve that is worth doing.

The change is visible only in the code-behind so nothing needs to be changed on your side.

### Submit button

When using a submit button inside of `<Form>` element the browser will automatically try to post the page. This is the default browser behavior. Because of this a new attribute is introduced to the `<Button>` element, called `PreventDefaultOnSubmit`. Basically it prevents a default browser behavior when clicking the submit button. So instead of posting the page it will stop it and just call the `Clicked` event handler. Pressing the `Enter` key will still work just as it's supposed to do. [more]({{ "/docs/components/button/#submit-button" | relative_url }})

### Chart events

Chart extension now supports `Clicked` and `Hovered` event callbacks. When clicked on data points in chart, all required details will be returned by the callback that can be used to extract information about the actual data-set. [more]({{ "/docs/extensions/chart/#events" | relative_url }})

### Figure

Introduced new `<FigureImage>` and `<FigureCaption>` components that can be used along the existing `<Figure>` component. [more]({{ "/docs/extensions/figure/" | relative_url }})

### Misc

- [#252](https://github.com/Megabit/Blazorise/issues/252) Added `Visibility` attribute on `<SelectItem>` components to allow definition of _default item_ that can be selected if `SelectedValue` is null or empty.
- [#226](https://github.com/Megabit/Blazorise/issues/226) New `<ValidationNone>` component for default message when validation is not yet done.

## Bug Fixes

- [#244](https://github.com/Megabit/Blazorise/issues/244) Dropdown going out of bounds
- [#162](https://github.com/Megabit/Blazorise/issues/162) Snackbar not closing (Server-side)
- [#248](https://github.com/Megabit/Blazorise/issues/248) `launchSettings.json` warning
- [#222](https://github.com/Megabit/Blazorise/issues/222) Validations.ClearAll() Fails to clear validations for second field
- [#230](https://github.com/Megabit/Blazorise/issues/230) NumericEdit Decimals Property Handling

## Final notes

As always, if you enjoy working with Blazorise please leave a star on [GitHub](https://github.com/Megabit/Blazorise) or click on the star-link bellow. Also consider becoming a [Patron](https://www.patreon.com/mladenmacanovic) or donate via [Buy me a Coffee](https://www.buymeacoffee.com/mladenmacanovic) or [PayPal](https://www.paypal.me/mladenmacanovic) and help Blazorise developer to work full time on the project!

<iframe src="https://ghbtns.com/github-btn.html?user=Megabit&repo=Blazorise&type=star&count=true" frameborder="0" scrolling="0" width="170px" height="20px"></iframe>