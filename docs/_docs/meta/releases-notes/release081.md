---
title: "Blazorise 0.8.1"
permalink: /docs/release-notes/release081/
excerpt: "Release notes for Blazorise 0.8.1"
toc: true
toc_label: "Features"
---

## Overview

This release is bringing some smaller fixes and improvements.

### DataGrid

The grid now supports nested field. Than means that it's now possible to define a column than can get or set a value from the sub-model. [more]({{ "/docs/extensions/datagrid/#datagrid" | relative_url }})

### Charts

The nasty bug with wrong color filing for charts in server-side Blazor is finally fixed. The issue was present for a long time and just couldn't find the solution. Finally thanks to [jdtcn](https://github.com/jdtcn) the issue is fixed. The problem was in formatting color alpha value to string. Instead of formatting it as decimal with the '.' as a separator it was formatted according to current culture info which in some regions is ','. The simple `ToString( CultureInfo.InvariantCulture )` was enough.

Also added `Animation` to the chart options to be able to control the animation duration or to disable it completely.

### Autocomplete

- Added an option to clear selected value and search field in the `Autocomplete` component.
- Added keyboard navigation for filtered items.
- Added Size attribute for search field.

## Contributors

- Thanks to [jdtcn](https://github.com/jdtcn) for finding a solution for bug in server-side charts. [#115](https://github.com/stsrki/Blazorise/issues/115)