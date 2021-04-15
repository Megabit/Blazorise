---
title: "v0.9.4 release notes"
permalink: /news/release-notes/094/
classes: wide
categories:
  - Release Notes
tags:
  - blazorise
  - release notes
  - 0.9.4
---

## Migration

- In `RichTextEditOptions`, rename `DynamicLoadReferences` to `DynamicallyLoadReferences`

## Highlights 🚀

### Async Validation

There are situations when you need to do validation by using the external method or a service. Since calling them can take some time it not advised to do it synchronously as that can lead to pretty horrible UI experience. So to handle those scenarios we have added support for awaitable validation handlers and basically enabling the asynchronous validation. Using them is similar to regular validator. Instead of using `Validator` we need to use `AsyncValidator` parameter.

For more information and an example just look at the [Async Validation]({{ "/docs/components/validation/#async-validation" | relative_url }}) page section.