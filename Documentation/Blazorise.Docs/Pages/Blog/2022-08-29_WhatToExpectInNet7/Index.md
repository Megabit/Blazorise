---
title: What To Expect In .NET 7
description: We analyze new features currently available in preview 7 of .NET 7, the last preview before the official stable release in November 2022
permalink: /blog/what-to-expect-in-net7
canonical: /blog/what-to-expect-in-net7
image-url: img/blog/2022-08-29/NET7.png
image-title: What To Expect In .NET 7
author-name: James Amattey
author-image: james
posted-on: August 23rd, 2022
read-time: 5 min
---

# What To Expect In .NET 7

In this blog post we will analyze new features currently available in preview 7 of .NET 7, the last preview before the official stable release in November 2022.

## Microsoft's .NET Release Cycle

In 2016, Microsoft released .NET 3.0 as a cross-platform framework. Since the official release of .NET 5 in November, Microsoft has committed to an annual release cycle for .NET as it focuses on making .NET a multiplatform environment. The "new .NET" combines the power of Mono and .NET Core into a single framework that allows developers to create applications for any platform, forming the basis for the launch of products such as .NET MAUI.

![Release Schedule](img/blog/2022-08-29/ReleaseCycle.png)

.NET releases in even numbers are the targetted for LTS and receive support and updates for three years, while odd number versions have short-term support for 18 months. This information is critical as it allows you to determine how far ahead you need to plan for a significant upgrade of your applications and the version compatibility of your Nuget packages.

## Notable Changes 

This section will highlight the most significant changes you can expect from .NET 7. It is becoming noticeable that STR(Short Term Release) versions will have fewer major features than versions with LTS, with most of the focus on improvement and enhancement.

### C# 11

Every new release of .NET ships with a corresponding release of C#. Available with the [.NET 7 Preview SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0), C# 11 comes with new features such as 

#### Required members

You can set the required modifier to properties of a class or a struct that must be initialized.

```cs
public class Student
{
    public required string Name { get; init; }
}
```

This means that without declaring the `Name` variable, the compiler will throw an error. 

```cs
using Blazorise.CSharp11.Features

var student = new Student { Name = "James" }
```

You can use the serialized attribute [SetsRequiredMember] to parse the `Student` parameter into the constructor and set it with init. 

```cs
public class Student
{
    public required string Name { get; init; }

    [SetsRequiredMembers]
    public Student( string name )
    {
        Name = name
    }
}
```

This indicates that a constructor sets all required members so callers using that constructor must initialize all required members using object initializers.

#### Auto-default struct

In C# 10, if a struct has a constructor, we have to set the properties within it. In C# 11, if a property is not set, it will be set as its default value. 

In our example below, the `Number` will be set to zero once the struct is used in variable.

```cs
public struct AutoDefaultStruct
{
    public int Number { get; set; }

    public AutoDefaultStruct
    {

    }
}
```

Auto-default structs allows your code to be more maintainable. Imagine that you add an extra field to the struct, you will need to change all the places where the Constructor is invoked.



#### Generic Attributes

An attribute with a generic type can be used as part of an attribute within a class as a fully constructed type.

```cs
public class Attr<T1> : Attribute { }

public class GenericAttribute
{
    [Attr<string>]
    public void myClass ()
    {

    }
}
```

This provides a more convenient syntax for attributes that require a System.Type parameter.

### Extended MAUI Support and Blazor Hybrid

At the time of writing, .NET MAUI is in preview 13. The official release of MAUI will ship with .NET 7 but based on .NET 6 with tooling and performance improvements.

> Suggested Reading - [What Is .NET MAUI](/blog/a-beginners-guide-to-maui)

Blazor Hybrid Support allows us to take existing Blazor Components such as [Blazorise components](docs/components) or a WPF or WinForms application and bundle them into a desktop application using a webview control with access to all underlying hardware APIs. It will allow developers to use web technologies to build desktop applications with access to system resources such as the local file system or a webcam.

### Containerized Images

In .NET 7, the developer's experience with building and deploying containerized applications will be simplified regarding the setup and configuration required to implement secure authentication and authorization while improving the performance of application startup and runtime execution. 

```bash
# create a new project and move to its directory
dotnet new blazorwasm -n my-container-app
cd my-container-app

# add a reference to a (temporary) package that creates the container
dotnet add package Microsoft.NET.Build.Containers

# publish your project for linux-x64
dotnet publish --os linux --arch x64 -
p:PublishProfile=DefaultContainer

# run your app using the new container

docker run -it --rm 5010:80 my-container-app:1.0:0
```

We also expect to see a preview of Microsoft Orleans, a cross-platform framework for distributed applications. 

## Other Improvements & features

Let's take a look at some improvements enhancements in .NET 7

### Performance Improvements

Additional performance improvements in .NET 7 have made it faster and more efficient. Already, .NET 6 has huge improvements in performance with JIT, AOT, inlining method calls, and devirtualization, and .NET 7 will take this even further. This will improve the performance of containerized applications without altering your source code running in the cloud.

### Official HTTP 3 Support

Http 3 was shipped as a preview feature in .NET 6 & will be a part of .NET 7 & enabled by default. In future .NET 7 preview versions, we’ll see performance improvements & additional TLS features. The HTTP/3 specification isn't finalized and behavioral or performance issues may exist in HTTP/3.

```cs
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        listenOptions.UseHttps();
    });
});
```

This code configures port 5001 to:

- Use HTTP/3 alongside HTTP/1.1 and HTTP/2 by specifying ```HttpProtocols.Http1AndHttp2AndHttp3```.
- Enable HTTPS with ```UseHttps```. HTTP/3 requires HTTPS.

### Minimal APIs Improvements

Minimal APIs were introduced in .NET 6 to allow you to create lightweight APIs without the overhead of controllers. These APIs have seen massive improvements with the introduction of the new Microsoft.AspNetCore.OpenApi package to provide APIs for interacting with the OpenAPI specification in minimal APIs. 

Also, endpoint filters will allow you to implement crosscutting concerns that you can only do with controllers using action filters today.

## Will Blazorise Components Support .NET 7

Blazorise v1.1 will ship with basic support for .NET 7. We will also implement the `required` keywords in certain places. Other features like `INumber` constraint for generic components like NumericEdit and NumericPicker are considered breaking changes and will be available in Blazorise v2.0.

## Should You Migrate To .NET 7?

Applications running in .NET Core 3.0 or 3.1 must migrate to .NET 6 at the very least, as it is the current version with LTS. Support for .NET Core 3.1 will end in December 2022.
 
Applications written in .NET 5 should also migrate to .NET 6 as support for .NET 5 ended in May 2022.
 
Applications running on .NET 6 do not need to migrate unless you want to take advantage of specific features .NET 7.
 
Before making migration decisions, you can use the preview SDK for .NET 7 to test compatibility for your application and Nuget packages.