---
title: "Blazorise 0.8.6"
permalink: /docs/release-notes/release086/
excerpt: "Release notes for Blazorise 0.8.6"
toc: true
toc_label: "Features"
---

## Overview

A long awaited [release of .NET Core 3.0](https://devblogs.microsoft.com/dotnet/announcing-net-core-3-0/) is finally here. And with it comes the new version of Blazorise. Unfortunately this update also comes with some braking changes that needs to be resolved but they should not be that difficult.

## Breaking changes

### .NET Core 3.0

You need to update your _Visual Studio to 16.3_ and also install newest _.Net Core SDK_ before you get the latest Blazorise. Please follow [official guide](https://devblogs.microsoft.com/aspnet/asp-net-core-and-blazor-updates-in-net-core-3-0/) before you continue. After you follow all the steps you should be safe to get the latest Blazorise 0.8.6.

### Static files

Along with the new Blazorise comes the new way of consuming static files. Unlike in previous versions of Blazorise from now on you must set the static file manually. When consuming NuGet packages that contains static files you must follow the convention `_content/{LIBRARY.NAME}/{FILE.NAME}`.

To upgrade your existing projects please look for more in the setup pages for your provider:

- [Bootstrap]({{ "/docs/usage/bootstrap/#5-static-files" | relative_url }})
- [Bulma]({{ "/docs/usage/bulma/#5-static-files" | relative_url }})
- [Material]({{ "/docs/usage/material/#6-static-files" | relative_url }})

> If you want to learn more about the reason behind this decision please look at the [official Blazor documentation.](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class?view=aspnetcore-3.0&tabs=visual-studio#consume-content-from-a-referenced-rcl)

## Closing notes

I really like to work on this project, and seeing it in its close-to-mature form as it is currently is a delight. I plan to finish the remaining bits in the next few weeks and then I will focus on **0.9** which is going to bring some new CSS providers.

And as always if you enjoy working with Blazorise please leave a star on [GitHub](https://github.com/stsrki/Blazorise) or click on the star-link bellow. Also consider becoming a [Patron](https://www.patreon.com/mladenmacanovic) or donate via [Buy me a Coffee](https://www.buymeacoffee.com/mladenmacanovic) or [PayPal](https://www.paypal.me/mladenmacanovic) and help Blazorise developer to work full time on the project!

<iframe src="https://ghbtns.com/github-btn.html?user=stsrki&repo=Blazorise&type=star&count=true" frameborder="0" scrolling="0" width="170px" height="20px"></iframe>