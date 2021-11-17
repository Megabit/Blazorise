---
title: "Blazorise 0.8.3"
permalink: /docs/release-notes/release083/
excerpt: "Release notes for Blazorise 0.8.3"
toc: true
toc_label: "Features"
---

## Overview

### Goal

Starting from **0.8.1** and up to **0.9** the plan is to refactor some parts of Blazorise to clean the code from the design decisions introduced in the past. With this relase we're steadily moving toward the clean **0.9** milestone. Again a lot or refactoring was done based on the features introduced in the latest release of Blazor preview 8.

### Patreon

If you enjoy working with Blazorise please leave a star on [GitHub](https://github.com/Megabit/Blazorise) or click on the star-link bellow. Also consider becoming a [Patron](https://www.patreon.com/mladenmacanovic) or donate via [Buy me a Coffee](https://www.buymeacoffee.com/mladenmacanovic) or [PayPal](https://www.paypal.me/mladenmacanovic) and help Blazorise developer to work full time on the project!

<iframe src="https://ghbtns.com/github-btn.html?user=Megabit&repo=Blazorise&type=star&count=true" frameborder="0" scrolling="0" width="170px" height="20px"></iframe>

## Breaking changes

### Components Renamings

In the older versions of Blazor, component names were _case-insensitive_ so it was imposible to have component names similar to the native html elements. For example `Button` component was recognized as `<button>` element so to overcome this the `Button` component had to be named `SimpleButton`. The same rule had to be applied to all other similar components.

Latest **Blazor preview 8** finally removed this limitation which brings us here. Component names are now _case-sensitive_. A lot of components are being renamed so there is going to be some changes in your project(s). For the most part you can just do a quick _find-and-replace_ tool to refactor your code or you can use _old-and-proven_ manual refactoring.

Full list of renamed components:

| < 0.8.3           | >= 0.8.3         |
|-------------------|------------------|
| `SimpleButton`    | `Button`         |
| `FormLabel`       | `Label`          |
| `TableContainer`  | `Table`          |
| `SimpleFigure`    | `Figure`         |
| `SimpleForm`      | `Form`           |
| `SimpleText`      | `Text`           |
| `ProgressGroup`   | `Progress`       |

### Other

DataGrid edit mode `InRow` is renamed to `Inline` so make sure to update that as well.

## Enhancements

### Tooltip

[Tooltip]({{ "/docs/components/tooltip/" | relative_url }}) is the only new component in this release. It was requested several times but since not all of the css providers have support for tooltips I had to find out the best way to handle it. Also I wanted to do it without _any_ javascript. Final solution was to handle it with custom css that is placed in the Blazorise root project. For now it supports only text as tooltip content but I plan to extend it to accept any type of content.

### DataGrid

The biggest new feature is the `Popup` [edit mode]({{ "/docs/extensions/datagrid/#editing" | relative_url }}). In this mode the cell values can be edited in the modal dialog. Just as in other edit modes the editing fields are being generated dynamically based on the grid settings.

### Templates

[Template parameters]({{ "/docs/extensions/datagrid/#command-templates" | relative_url }}) for command buttons is the next big feature. With this feature it's now possible to customize the look and feel of command buttons like `New`, `Edit`, `Cancel`, etc.

### Other

Other smaller features are:

- Styling of grid table is now possible thanks to the new attributes on DataGrid component. 
  - `IsStriped`
  - `IsBordered`
  - `IsBorderless`
  - `IsHoverable`
  - `IsNarrow`
- It's now possible to un-select row while holding the Ctrl key and clicking on row.
- Row selection is disabled if any row is currently being edited.
- Added new [FilterMethod]({{ "/docs/extensions/datagrid/#filtering" | relative_url }}) attribute.

## Other

- Every chart shows up as Line Chart on latest package upgrade [#210](https://github.com/Megabit/Blazorise/issues/210)

## Final notes

Again, if you like Blazorise please leave a star on [GitHub](https://github.com/Megabit/Blazorise) or click on the star-link bellow. Thanks!

<iframe src="https://ghbtns.com/github-btn.html?user=Megabit&repo=Blazorise&type=star&count=true" frameborder="0" scrolling="0" width="170px" height="20px"></iframe>


Thats all for now. Have fun with Blazorise 0.8.3 new features!