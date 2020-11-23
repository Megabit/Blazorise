---
title: "v0.9.3 release notes"
permalink: /news/release-notes/093/
classes: wide
categories:
  - Release Notes
tags:
  - blazorise
  - release notes
  - 0.9.3
---

## Breaking changes

### .Net 5

Blazorise is now running fully on .Net 5, and .Net Core 3.1 is not supported any more. This decision was hard but needed, mainly because of a new IComponentActivator that finally gave me a way to initialize components the way I wanted it from the start. Until now I had to do a lot of workarounds to make every component overridden for each of the supported CSS providers. While it worked for the most part, it was slow and unoptimized, meaning each component had to be created twice. Not any more. IComponentActivator gave me a way to register custom components through DI and then initialize only those components that I want. As a result Blazorise should now be a lot faster, and finally I can implement some outstanding features that were impossible until now.

## Features

### Snackbar

A lot of UX and internal improvements are done on `Snackbar` component. We added an option to delay closing of the snackbar automatically if user is clicked on a snackbar directly. First click will delay the close event and second click will close it immediately. The behavior can be controller by new `DelayCloseOnClick` and `DelayCloseOnClickInterval` parameters.

Other changes are the introduction of new `SnackbarHeader` and `SnackbarFooter` component. This will allow more freedom to make snackbar to appear as classic Toast component. This change also bring slight breaking change because from now on `SnackbarAction` must be placed inside of `SnackbarBody`, `SnackbarHeader` and `SnackbarFooter`.

`SnackbarStack` has also received some of the changes. For start, `Push` method is removed and it is replaced with new `PushAsync` method. It can now receive `message` and a `title` parameters, while other options are controlled through the `options` builder.