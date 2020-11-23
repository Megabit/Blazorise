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