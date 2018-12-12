---
title: "FAQ"
permalink: /docs/faq/
excerpt: "Frequently asked questions (FAQ) about Blazorise."
toc: true
toc_label: "Questions"
---

### What is Blazorise?

Blazorise is a user interface component library made on top of a web framework called [Blazor](https://blazor.net), and CSS frameworks like Boostrap, Bulma or Material. Blazorise has two core principles: 
1. keep stuff simple
2. be extendable

### Why should I use Blazorise?

It gives you a set of components that you can use to easily create a _single page_ application. That way you can save time and don't need to waste it on building your own set of components. Just import Blazorise and start now!

### How can I try out Blazorise?

To try Blazorise please check out our [quick start]({{ "/docs/quick-start/" | relative_url }}) guide.

### Why are some components named TextEdit, CheckEdit, etc.?

This is a known limitation in Razor and Visual Studio, and not problem by the Razor itself. Basically it's not not possible to have components that have the same name as regular `html` tags(form, button, text). Likely this will change in the future but until then we're stuck with this "weird" names for components.