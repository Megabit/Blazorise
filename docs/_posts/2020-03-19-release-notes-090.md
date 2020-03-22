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