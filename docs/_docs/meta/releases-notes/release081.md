---
title: "Blazorise 0.8.1"
permalink: /docs/release-notes/release081/
excerpt: "Release notes for Blazorise 0.8.1"
toc: true
toc_label: "Features"
---

## Overview

This release is bringing some smaller fixes and improvements.

## Enhancements

### DataGrid

The grid now supports nested field. Than means that it's now possible to define a column than can get or set a value from the sub-model. [more]({{ "/docs/extensions/datagrid/#datagrid" | relative_url }})

## Bug Fixes

### Charts

The bug with wrong color filing for charts in server-side Blazor is finally fixed.

## Contributors

- Thanks to [jdtcn](https://github.com/jdtcn) for finding a solution for bug in server-side charts. [#115](https://github.com/stsrki/Blazorise/issues/115)