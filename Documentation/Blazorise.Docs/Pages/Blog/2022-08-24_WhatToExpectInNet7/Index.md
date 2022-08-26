---
title: What To Expect In .NET 7
description: we analyze new features currently available in preview 7 of .NET 7, the last preview before the official stable release in November 2022
permalink: blog/what-to-expect-in-net7
canonical: blog/what-to-expect-in-net7
image-url: img/blog/2022-08-24/NET7.png
image-title:What To Expect In .NET7
author-name: James Amattey
author-image: james
posted-on: August 23rd, 2022
read-time: 5 min

---


## Microsoft's .NET Release Cycle
In 2016, Microsoft released .NET 3.0 as a cross-platform framework. Since the official release of .NET 5 in November, Microsoft has committed to an annual release cycle for .NET as it focuses on making .NET a multiplatform environment.  The "new .NET" combines the power of Mono and .NET Core into a single framework that allows developers to create applications for any platform forming the basis for the launch of products such as  .NET MAUI.


![Release Schedule](img/blog/2022-08-24/ReleaseCycle.png align="left")

.NET releases in even numbers are the targetted for LTS and receive support and updates for 3 years, whilst odd number versions have short-term support for 18 months. This information is critical as it allows you to determine how far ahead you need to plan for a major upgrade of your applications and the version compatibility of your Nuget packages. 


## Notable Changes 
In this section, we will highlight the most significant changes you can expect from .NET 7. It is becoming noticeable that Short Term Release versions will have fewer major features than versions with LTS, with a majority of the focus going towards improvement and enhancement. 

### C# 11
Every new release of .NET ships with a corresponding release of C#. Available with the [preview SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0), C# 11 comes with new features such as 

#### Required members
You can set the required modifier to properties of a class or a struct that must be initialized.

![Required Members](img/blog/2022-08-24/RequiredMembers.png align="left")

This means that without declaring the Name variable, the compiler will throw an error. 

![Compiler Error](img/blog/2022-08-24/Error.png align="left")

You can use the serialized attribute [SetsRequiredMember] to parse the RequiredMember parameter into the constructor and set it with init. 

![SetsRequiredMembers.png](img/blog/2022-08-24/SetsRequiredMembers.png align="left")

This indicates that a constructor sets all required members so callers using that constructor must initialize all required members using object initializers.


#### Auto-default struct

In C# 10, if a struct has a constructor, we have to set the properties within it.  In C# 11, if a property is not set, it will be set as its default value. 

In our example below, the number will be set to zero 

![Auto-Default Struct](img/blog/2022-08-24/AutoDefaultStruct.png align="left")

#### Generic Attributes
An attribute with a generic type can be used as part of an attribute within a class as a fully constructed type.


![Generic Attributes](GenericAttributeL.png align="left")

### Extended MAUI Support and Blazor Hybrid

At the time of writing, .NET MAUI is in preview 13. The official release of MAUI will ship with .NET 7 but based on .NET 6 with tooling and performance improvements.

> Suggested Reading - [What Is .NET MAUI](/blog/a-beginners-guide-to-maui)

Blazor Hybrid Support allows us to take existing Blazor Components such as Blazorise (https://blazorise.com/docs/components/) or a WPF or WinForms application and bundle them into a desktop application using a webview control with access to all underlying hardware APIs. It will allow developers to use web technologies to build desktop applications with access to system resources such as the local file system or a webcam.

### Containerized Images

In .NET 7, the developer's experience with building and deploying containerized applications will be simplified regarding the setup and configuration required to implement secure authentication and authorization while improving the performance of application startup and runtime execution. 


![Containerized Images](img/blog/2022-08-24/ContainerizedImages.png align="left")

We also expect to see a preview of Microsoft Orleans, a cross-platform framework for distributed applications. 

## Other Improvements & features
Let's take a look at some improvements enhancements in .NET 7

### Performance Improvements
Additional performance improvements in .NET 7 have made it faster and more efficient. Already, .Net 6 has huge improvements in performance with JIT, AOT, inlining method calls, and devirtualization, and .Net 7 will take this even further. This will improve the performance of containerized applications without altering your source code running in the cloud.

### Official HTTP 3 Support
Http 3 was shipped as a preview feature in .Net 6 & will be a part of .Net 7 & enabled by default. In future .Net 7 preview versions, we’ll see performance improvements & additional TLS features. The HTTP/3 specification isn't finalized and behavioral or performance issues may exist in HTTP/3.


![HTTP 3](img/blog/2022-08-24/Http3.png align="left")

### Minimal APIs Improvements
Minimal APIs were introduced in .Net 6 to allow you to create lightweight APIs without the overhead of controllers. These APIs have seen massive improvements with the introduction of the new Microsoft.AspNetCore.OpenApi package to provide APIs for interacting with the OpenAPI specification in minimal APIs. 

Also, endpoint filters will allow you to implement crosscutting concerns that you can only do with controllers using action filters today.

## Should You Migrate To .NET 7?

Applications running in .NET Core 3.0 or 3.1 must migrate to .NET 6 at the very least, as it is the current version with LTS. Support for .NET Core 3.1 will end in December 2022.

Applications written in .NET 5 should also migrate to .NET 6 as support for .NET 5 ended in May 2022.

Applications running on .NET 6 do not need to migrate unless your you want to take advantage of specific features .NET7

You can use the preview SDK for .NET 7 to test compatibility for your application and Nuget packages before you make migration decisions.

