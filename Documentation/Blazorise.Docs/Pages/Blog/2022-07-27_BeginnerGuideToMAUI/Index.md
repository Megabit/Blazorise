---
title: A Beginner's Guide To .NET MAUI
description: Learn all about the MAUI, a Microsoft's latest multi-platform app UI framework for building Blazor and mobile apps.
permalink: /blog/a-beginners-guide-to-maui
canonical: /blog/a-beginners-guide-to-maui
image-url: img/blog/2022-07-27/A_Beginners_Guide_To_NET_MAUI.png
image-title: A Beginner's Guide To .NET MAUI
author-name: James Amattey
author-image: james
posted-on: July 27, 2022
read-time: 4 min
---

# A Beginner's Guide To .NET MAUI

This article provides a beginner-level introduction to .NET MAUI. It explores the history behind its evolution and what makes it a compelling choice for teams developing with tools in the .NET ecosystem.

---

## Introduction

When I first heard about MAUI, I kept referring to the shape-shifter, demigod of the wind and sea, and hero of men from my favorite animated movie, Moana.

Initially, I thought it was Microsoft's attempt to venture into animation movies, but at during the announcement of .NET 6, I realized that MAUI is an attempt to conquer the multi-platform market with the emergence of tools such as electron.js and increase market adoption for the C# language.

.NET MAUI or MAUI is a multi-platform framework for native desktop and mobile applications.

MAUI is an acronym that means Multi-platform App UI and expands on its definition and what it was created to do.

Like the demigod, MAUI is a shape-shifting UI framework that changes UI icons to match the native environments by relying on native system UI components.

With MAUI, developers can build applications that run on Android, iOS, Windows, and macOS while sharing a single source code.

---

## Origin of MAUI

MAUI is a progression from Microsoft's deprecated Xamarin project, which was created for cross-platform mobile application development for both Android and iOS platforms.

The lines between native and cross-platform developments are getting blurry with tools that help you to deploy a single codebase for different platforms.

Frameworks such as React Native and Flutter have championed the cross-platform market, with Xamarin following closely behind.


---

## What Happened To Xamarin.Forms

Xamarin was founded in 2011 and saw an increased community support because it allows developers to quickly start making apps without having to find out multiple languages.

Xamarin forms were deprecated in November 2021 and moved into the core product offering, starting with the discharge of .NET 6.

If you previously worked with Xamarin.Forms, getting started with MAUI will be seamless as it still utilizes the same C# and XAML markup language.

In a subsequent blog, we will discuss how to move your Xamarin.Forms project to MAUI.

---

## What's New In MAUI

Starting from .NET 6, MAUI shipped with fully integrated tooling and support in Visual Studio 2022, Visual Studio for Mac 2022, and JetBrains Rider.

You can either create a standalone MAUI app or an MAUI Blazor application. Further details on building an MAUI app to follow subsequently.

![Microsoft's new UI framework for multi-platform development](img/blog/2022-07-27/maui.png)

---

## What Can You Build With MAUI

MAUI extends Xamarin forms to build multi-platform apps. This means that with one code-base, your app can run on iOS and Android as mobile apps while running on Windows and macOS as desktop apps.

.NET MAUI provides a collection of controls that can be used to display data, initiate actions, indicate activity, display collections, pick data, and more. In addition to a collection of controls, .NET MAUI also provides:

- An elaborate layout engine for designing pages.
- Multiple page types for creating rich navigation types, like drawers.
- Support for data-binding for more elegant and maintainable development patterns.
- The ability to customize handlers to enhance the way in which UI elements are presented.
- Cross-platform APIs for accessing native device features. These APIs enable apps to access device features such as the GPS, the accelerometer, and battery and network states.
- Cross-platform graphics functionality provides a drawing canvas that supports drawing and painting shapes and images, compositing operations, and graphical object transforms.
- A single project system that uses multi-targeting to target Android, iOS, macOS, and Windows.
- .NET hot reload so that you can modify both your XAML and your managed source code while the app is running, then observe the result of your modifications without rebuilding the app.

---

## Conclusion

Cross-platform and multi-platform frameworks are here to stay as the lines of native platform development get blurry with every innovation and are a worthy alternative to rapid application development and speed to market for small teams on limited resources.

This is MAUI's first stable release so we will keep an eye on updates from Microsoft with regards to future roadmap of MAUI.

If you want to build out rich user interfaces with MAUI Blazor quickly, [reach out to our team](contact), and we will help you get started quickly with our [tested and production-ready UI components](https://blazorise.com/docs/components).