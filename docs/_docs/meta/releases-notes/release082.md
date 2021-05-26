---
title: "Blazorise 0.8.2"
permalink: /docs/release-notes/release082/
excerpt: "Release notes for Blazorise 0.8.2"
toc: true
toc_label: "Features"
---

## Overview

This release was mainly focused on code cleanup and refactoring based on the changes in the Blazor preview 8 with some minor features and bug fixes.

## Breaking changes

### Blazor preview 8

This version of Blazorise is upgraded to the latest version of Blazor preview 8. If you haven't already, please read official [migration steps](https://devblogs.microsoft.com/aspnet/asp-net-core-and-blazor-updates-in-net-core-3-0-preview-8/) and follow the instructions before you continue here. After you upgrade most of the issues should be gone but in case something is wrong and you cannot start the project here are the solution for the most often problems:

- you need to add `@ref:suppressField` wherever you have `@ref` 

## Code Cleanup

### Code Behind

Most of the changes are done under the hood. Previously the components code-behind files were in the separate subfolder so it was tedious to navigate the code since there is a lot of files. The solution was to rename all of the code-behind files according to the rule [ComponentName]_.razor.cs_ and move them to the same location as the component _.razor_ files. Visual Studio automatically places _.razor.cs_ under the _.razor_ files so it's a lot easier now to navigate. Also the shortcut key (F7) works when switching between component and it's code-behind file.

### Public Parameters

The next big change is based on the new feature in Blazor preview 8. In previous versions of Blazor all `[Parameter]` attributes had to have private or protected access modifier. Finally this restriction is gone and we're allowed to have public access modifier on component parameters. A lot of components had to be changed by hand to replace the modifier keyword but it was worth it.

## Enhancements

### DataGrid

Added new `RowDetailTemplate` to the DataGrid which can be used to define nested structure for every row in the grid. A detail example can be found in the [documentation]({{ "/docs/extensions/datagrid/#rowdetailtemplate" | relative_url }}).

### Sidebar

It's now possible to create sidebar structure dynamically based on the supplied data. This feature was requested several times and it's finally done. To find more please visit the [documentation]({{ "/docs/extensions/sidebar/#usage" | relative_url }}) to see how it can be used. 

## Other

- Fixed bug with first time selection in SelectList component [#188](https://github.com/Megabit/Blazorise/issues/188)
- Fixed bug when setting the icon name manually [#95](https://github.com/Megabit/Blazorise/issues/95)