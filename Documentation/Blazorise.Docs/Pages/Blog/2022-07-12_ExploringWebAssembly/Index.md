---
title: The Underlying Technology Behind Blazor WebAssembly
description: Learn all about the WebAssembly, The Underlying Technology Behind Blazor WASM, and how it can help you to build a rich UI applications.
permalink: /blog/exploring-webassembly-the-underlying-technology-behind-blazor-wasm
canonical: /blog/exploring-webassembly-the-underlying-technology-behind-blazor-wasm
image-url: img/blog/2022-07-12/the_underlying_technology_behind_blazor_wasm.png
image-title: Exploring WebAssembly, The Underlying Technology Behind Blazor WASM
author-name: James Amattey
author-image: james
posted-on: July 13th, 2022
read-time: 4 min
---

# Exploring WebAssembly, The Underlying Technology Behind Blazor WASM

Blazorise as a component library harnesses the power of Blazor WASM to create rich user interfaces. These components are highly customizable, production-grade, and highly customizable, allowing development teams to deliver client-side applications faster.

Built as a feature of the ASP.NET ecosystem, Blazor WASM allows developers to create rich client-side applications with the same C# used with classic ASP.NET server-side applications.

The key unique selling proposition of Blazor WASM is WebAssembly, which is tooted to be faster than Javascript.

In this post, take a deeper dive into WebAssembly and explain what it is and why its integration with Blazor WASM makes it a faster alternative to popular javascript frameworks such as React and Angular.

---

## What Is WebAssembly?

In the words of Jay Phelps, “WebAssembly is neither web nor assembly”. In simple terms, WebAssembly or WASM, is a stack-based virtual machine.

Breaking this even further, WebAssembly is a virtual machine that runs on a block of memory. This means that runtimes are responsible for handling operations at the stack level unlike with VMware or VirtualBox which pretend to be computers and handle operations at the CPU level.

The WebAssembly binary format is a virtual machine format. Virtual machine runtimes are responsible for executing operations within, as the name implies, a virtual machine.

WebAssembly was created to boost performance. It is designed to be a compilation target for low-level languages.

[The WebAssembly specification](https://webassembly.github.io/spec/) maintains that the standards apply to more than just the browser host, but also to any other compliant host runtime (what the specification refers to as an embedder).

---

## Features Of WebAssembly

Now that we have covered the basic architecture of WebAssembly, let's explore why Microsoft's decision to build Blazor WASM with WebAssembly benefits your client's user experience.

### Speed

The job of the most basic WebAssembly interpreter is to read operation codes and, in response, push or pop values on and off the stack. These kinds of operations are incredibly fast and efficient. A WASM interpreter can supply basic math, memory, and stack management capabilities and thus do its job very quickly and with very little overhead.

This means that Blazorise components will load quickly when users visit your web application as interactions happen at near-native speeds.

### Lightweight

An underrated part of WebAssembly is how its operation code are processor and operating system agnostic, a functionality that means that its modules can run and be deployed anywhere, on any architecture, in any operating system (or even microkernel!), so long as the host is a valid WebAssembly runtime.

This means that business logic encoded into the WASM should work anywhere, and even more importantly, the functions your WASM module imports from the host should also be able to work anywhere.

### Small
WebAssembly can produce incredibly small artifacts. This is the reason that the initial download required for Blazor Applications is likely to be small.

The deployment size for WASM is exponentially smaller than most of what we’re building today in the world of containers or serverless “bundles”, and that can have a huge impact on how we think about and plan our distributed applications.

### Secure

Wasm code is also not allowed to escape the confines of its sandbox. There are no language primitives for accessing the operating system, reading from memory that might belong to another process, communicating with a network, communicating with hardware, the kernel, or an operating system of any kind.

Anything a WASM module does outside its sandbox must go through a host import, a function it asks the host to call. This means the host is free to deny that call at any time. Host imports are the lynchpin of nearly all of the actor models and cloud-native functionality. They are as powerful as they are secure.

---

## What WebAssembly Is Not

With the introduction of every new and shiny piece of technology, it’s easy to assume that it will solve all of our problems. But realistically, every new technology has shortcomings and use cases it does not favour.

It’s true that we can do an amazing amount of things with WebAssembly, but hopefully, through exploring its limitations and true boundaries, you’ll have a better appreciation of exactly which problems it is ideal for solving, and which problems it can only solve with the use of additional code generation, shims, and fancy tricks.

Many of the early WebAssembly technology demonstrations are full of “smoke and mirrors”—illusions or hand-waving where a lot of complexity is hidden from the viewer. As you continue to learn about WebAssembly, WASM code, and technology demonstrations, keep a keen and critical eye out for the boundaries. Watch for where the core WebAssembly stops and the duct tape, bubble gum, and mirrors start.

---

## Conclusion

In this post, we have explored what WebAssembly is and why it makes Blazor WebAssembly a very complete option for frontend development. We highlighted the features of WASM along with a few use cases and concluded the post by explaining what WebAssembly is not and where it is not viable.

In our next post, we will take a deeper dive in Blazor WebAssembly and why you should consider it for your client-side applications. 

## Related blogs

- [How to create a Blazorise WASM application: A Beginner's Guide](blog/how-to-create-a-blazorise-application-beginners-guide)