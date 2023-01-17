---
title: What is Blazor WASM?
description: In this article we will discover what Blazor WASM is, its features and what the kind of applications it can build.
permalink: /blog/what-is-blazor-wasm
canonical: /blog/what-is-blazor-wasm
image-url: img/blog/2022-07-20/What_Is_Blazor_WASM.png
image-title: What is Blazor WASM
author-name: James Amattey
author-image: james
posted-on: July 20th, 2022
read-time: 5 min
---

# What Is Blazor WASM?

This article builds on the [concepts of WebAssembly](blog/exploring-webassembly-the-underlying-technology-behind-blazor-wasm) as an integral part of Blazor WASM and explores what Blazor WASM is, how it works, the problems it solves, its features, and the type of applications it can be used to build.

Blazor WASM (or Blazor WebAssembly) is a single-page web application framework built by Microsoft that allows you to build single-page web applications. Built as part of the .NET Core ecosystem, Blazor uses C# to generate dynamic content for a rich client experience.

---

## How Does Blazor Work?

Traditionally, all web pages are structured with HTML, styled with CSS, and use Javascript to introduce dynamic interactivity. C# as a language was not built to run natively in browsers. However, [with the presence of WebAssembly](https://blazorise.com/blog/exploring-webassembly-the-underlying-technology-behind-blazor-wasm), the browser can now host the .NET runtime, which makes it possible to run and execute C# Code.

![Blazor WebAssembly Source: Microsoft](img/blog/2022-07-20/blazor-webassembly.png)

---

## Do We Really Need C# In The Browser?

It is true that Javascript is the native language of web browsers and can run in the front-end and backend. But is it really necessary to run C# in the browser?

The following highlight the benefits of running C# in the browser.

### Full Stack Enablement

The .NET ecosystem provides one of the most secure and most resilient server-side implementations. It ranks highly among the most popular backend frameworks with its implementation of ASP.NET.

By extending the .NET runtime into the browser, Microsoft enabled an actual full stack experience to allow teams to use the same knowledge and class libraries they are familiar with from the C# language on both stacks.

### Easier Integration

In web development, some developers focus on either the client side or server-side. While code classes do not need to know the implementation details, developers on the client side need knowledge about server-side implementation. This can sometimes prove difficult using two languages, especially when one is strongly typed and the other is not.

By using Blazor WASM, the client-side and server-side can share a common code base in the same language, which can integrate easily with each other. Client-side developers will have an understanding of server-side logic and how to seamlessly integrate and implement it.

### Reduced Development Costs

Programming languages take years to learn and master. As developers do not come cheap, finding and paying developers is a strategic decision for small teams. Having a team of skilled developers who can switch between client and server-side applications without losing any understanding of both sides enhances collaboration which is vital for small teams.

> [Click here to discover how a Blazorise Mentor](commercial/enterprise-plus) can quickly help your development teams scale up rapid application development using Blazorise Components.

### Code Reuse

The DRY Principle is one of the simplest ways to prevent and reduce code smells and spaghetti code. By not having to rewrite new code, coupling is reduced and future changes do not affect other parts of the codebase.

With Blazor WASM, entire .NET libraries can be shared, and consumed in the browser and are not required to re-invent the wheel.

### Features Of Blazor WASM

Blazor WASM has a unique set of features that make it a compelling choice for front-end development. 

### Reusable Components

With Blazor, you can create components for use across your application. Components are building blocks for a user interface such as a form, cards, tables, grids, and many more.

These components can be defined once, and called multiple times. At the very core, components are a group of HTML elements that specify the structure of a site.

```html
<Dropdown>
    <DropdownToggle Color="Color.Primary">
        Dropdown
    </DropdownToggle>
    <DropdownMenu>
        <DropdownItem>Action</DropdownItem>
        <DropdownDivider />
        <DropdownItem>Another Action</DropdownItem>
    </DropdownMenu>
</Dropdown>
```

The above is [dropdown menu component with Blazorise](docs/components/dropdown). This makes development easier and faster and can be called anytime you want to create a dropdown menu, without having to rewrite a lot of code.

### Hot Reload

Hot reload triggers the browser to refresh automatically when changes are made to the code base. This helps to improve developer productivity, as they do need to restart the application to see the effects of those changes.

### Fast Performance

Blazor WASM on the first load takes a while as the runtime environment and other dependencies have to be downloaded. Then onwards, Blazor WASM relies on the downloaded runtime to execute at near native speeds in the browser, making applications faster

## What Can Be Built With Blazor

This section highlights some of the best use cases for Blazor

### Progressive Web Application

A progressive web application is an application that can be installed and run like a native application. Building PWAs helps teams provide native desktop and mobile experiences for their applications without building specifically for those environments.

For small teams, Blazor can be a way to save up costs without sacrificing performance and delivery.

### Single Page Applications

A single-page application (SPA) is a web application or website that interacts with the user by dynamically rewriting the current web page with new data from the web server, instead of the default method of a web browser loading entire new pages

In Blazor WebAssembly, when the client makes a request, it is served up as a bit of HTML, CSS, and JavaScript — like all other web apps. The `blazor.webassembly.js` file bootstraps the app and starts loading .NET binaries which can be viewed coming over the wire in the browser's Network tab.

### Offline Applications

Because Blazor WASM downloads the runtime and dependencies on startup, most of the rendering is done on the client side. As a result, it can function well in scenarios where there is no internet or limited connection.

## Debunking Common Misconceptions

Just like all other SPA frameworks, Blazor WASM comes with its shortcomings. Here are a few to take note off

### Huge Payload and Slow Initial Load

Blazor WASM applications can get big quickly due to the runtimes required to run C# in the browser. This big size can cause the initial startup to be slow, and this can take a toll on a poor internet connection. Fortunately, this only happens once therefore subsequent loads are faster and smaller as the majority of the heavy lifting has already occurred.

> Large file sizes can be improved with [Trimmer for Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/configure-trimmer?view=aspnetcore-6.0), which can significantly decrease the size of the assemblies required to download the application.

### No More Javascript

The goal of Blazor WASM is to allow C# to run in the browser, but that does not mean you will not require javascript at all. Blazor WASM cannot interact directly with the DOM or have access to Browser APIs, and as a result these need to be called using javascript, in a process called Javascript Interop. As Javascript is the native language for browsers, it will be required to access browser-based functionalities and

## Conclusion

Blazor helps teams build rich UI interfaces. If your team uses Blazor, [reach out to us](commercial/contact) on how we can help you speed up your development with our [production-ready UI components](docs/components).