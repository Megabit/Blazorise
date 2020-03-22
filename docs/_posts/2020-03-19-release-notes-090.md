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

## Overview

Many new components and improvements on existing components have being made.

## Breaking changes

Before we continue it's good to mention that with this release comes a lot of breaking changes. I know this is not a popular decision but Blazorise being still in development stage and 1.0 behind a corner I feel this is the perfect time to clean some decisions from the past and introduce some new APIs. So without further ado let us start:

### Boolean properties

This is by far the biggest refactor in this release and a lot of components is touched with this change. Basically this is one of the first [issues](https://github.com/stsrki/Blazorise/issues/4) created after the Blazorise was first released. Back then Blazor did not have case-sensitive support when naming components and properties. So whenever there was a clash like `button` and `Button` or `disabled` and `Disabled` it would just break. So I had to introduce prefixes to component properties like `IsDisabled` or `IsActive`. Personally I hated it but it was necessary back then. Now that Blazor has fixed this limitation it was the perfect time to also go through all of the components and remove the prefixes. As a consequence I think the API is now a lot cleaner and easier to write. Since the change is too big, listing every change in this post will not make too much sense. Instead you can go to this [PR](https://github.com/stsrki/Blazorise/pull/536) and see all changes listed.

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