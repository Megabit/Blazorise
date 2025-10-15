---
title: How to create a Blazorise WASM application
description: In this article we will learn how to create a Blazorise WebAssembly (WASM) application. As an example, we will also use basic Blazorise components to setup a simple form.
permalink: /blog/how-to-create-a-blazorise-application-beginners-guide1
canonical: /blog/how-to-create-a-blazorise-application-beginners-guide1
image-url: img/blog/2022-06-08/How_to_create_a_Blazorise_application_A_Beginners_Guide.png
image-title: Blazorise WASM application: A Beginner's Guide
author-name: Mladen Macanović
author-image: mladen
posted-on: June 8th, 2022
read-time: 5 min
---

# How to create a Blazorise WASM application: A Beginner's Guide

In this article we will learn how to create a Blazorise WebAssembly (WASM) application. As an example, we will also use basic Blazorise components to setup a simple form.

## Prerequisites

To work on a Blazor app, you can start by taking of the following approaches:

- [.NET CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/) and [Visual Studio Code](https://code.visualstudio.com/): Preferred for Linux.
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) and [Visual Studio Code](https://code.visualstudio.com/): Preferred for Windows and macOS.

In this tutorial, we are going to use [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/). Please install the latest version of Visual Studio 2022. While installing, make sure you have selected the ASP.NET and web development workload.

## Creating the Blazorise WebAssembly application

First, we'll create a Blazor WebAssembly app. Please follow these steps to do so:

1. Open Visual Studio 2022 and click on the **Create a new Project** option.
2. In the Create a new Project dialog that opens, search for **Blazor** and select **Blazor WebAssembly App** from the search results. Then, click **Next**. Refer to the following image. ![Create a new project dialog](img/blog/2022-06-08/Create-a-new-project-dialog.png)
3. Now you will be at the **Configure your new project** dialog. Provide the name for your application. Here, we are naming the application **BlazoriseSampleApplication**. Then, click **Next**. Refer to the following image. ![Configure your new project dialog](img/blog/2022-06-08/Configure-your-new-project-dialog.png)
4. On the **Additional information** page, select the target framework **.NET 6.0** and set the authentication type to **None**. Also, check the options **Configure for HTTPS** and uncheck **ASP.NET Core hosted**, and then click on **Create**. Refer to the following image. ![Additional information dialog](img/blog/2022-06-08/Additional-information-dialog.png)

## Installing the Blazorise packages

We have now completed our Blazor WebAssembly project. Continue by installing the **Blazorise NuGet** packages and configuring the project to use Blazorise.

1. Right click on the project in solution explorer and click on **Manage NuGet Packages** from the dropdown menu. ![Manage NuGet Packages](img/blog/2022-06-08/Manage-NuGet-Packages.png)
2. Navigate to the **Browse** tab and search for **Blazorise**. To install it, use the **Blazorise.Bootstrap5** package. Repeat for **Blazorise.Icons.FontAwesome** package. ![Install Blazorise NuGet](img/blog/2022-06-08/Install-Blazorise-NuGet.png)
3. The next step is to change your **index.html** and include the Blazorise CSS source files: 
    ```html|StaticFilesExample
    <!DOCTYPE html>
    <html lang="en">

    <head>
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
        <title>BlazoriseSampleApplication</title>
        <base href="/" />
        <link href="css/bootstrap/bootstrap.min.css" rel="stylesheet" />

        <link href="_content/Blazorise/blazorise.css" rel="stylesheet" />
        <link href="_content/Blazorise.Bootstrap5/blazorise.bootstrap5.css" rel="stylesheet" />

        <link href="css/app.css" rel="stylesheet" />
        <link href="BlazoriseSampleApplication.styles.css" rel="stylesheet" />
    </head>

    <body>
        <div id="app">Loading...</div>

        <div id="blazor-error-ui">
            An unhandled error has occurred.
            <a href="" class="reload">Reload</a>
            <a class="dismiss">🗙</a>
        </div>
        <script src="_framework/blazor.webassembly.js"></script>
    </body>

    </html>
    ```
4. Next, define the Blazorise using in your main **_Imports.razor** file. This will instruct Visual Studio IntelliSense to suggest Blazorise components to us.
    ```html|UsingsExample
    @using Blazorise
    ```
5. Go to the **Client** folder and define the following in **Program.cs**.
    ```cs|ServicesExample
    using Blazorise;
    using Blazorise.Bootstrap5;
    using Blazorise.Icons.FontAwesome;
    using BlazoriseSampleApplication;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

    namespace Company.WebApplication1
    {
        public class Program
        {
            public static async Task Main( string[] args )
            {
                var builder = WebAssemblyHostBuilder.CreateDefault( args );
                builder.RootComponents.Add<App>( "#app" );
                builder.RootComponents.Add<HeadOutlet>( "head::after" );

                builder.Services.AddScoped( sp => new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) } );

                builder.Services
                    .AddBlazorise( options =>
                    {
                        options.Immediate = true;
                    } )
                    .AddBootstrap5Providers()
                    .AddFontAwesomeIcons();

                await builder.Build().RunAsync();
            }
        }
    }
    ```

## Setting the Simple Example

The last step is to adjust a default Blazor example to use Blazorise components.

Go the **Counter.razor** under the **Pages** folder and copy/paste the following snippet.

```html|CounterExample
@page "/counter"

<Heading Size="HeadingSize.Is1">Counter with Blazorise</Heading>

<Paragraph>Current count: @currentCount</Paragraph>

<Button Color="Color.Primary" Clicked="IncrementCount">Click me</Button>

@code {
    int currentCount = 0;

    void IncrementCount()
    {
        currentCount++;
    }
}
```

## Executing the demo

You should now be able to run the Blazorise sample project without incident. Press **F5** on your keyboard, or select **Start Debugging** from the Debug menu.

Wait for VisualStudio to complete the build process, and you should see the new application running in your browser. To see an example of a counter, click on the Counter button in the sidebar.

![Counter Example](img/blog/2022-06-08/Counter-Example.png)

## Resource

Also, you can get the source code of the sample from the [BlazoriseSampleApplication](https://github.com/Megabit/Blazorise-Samples) in Blazor demo on GitHub.

## Summary

Thanks for reading! In this blog, we learned how to create and setup Blazorise in a Blazor WebAssembly app. We have also modified default Counter example to make use of Blazorise components. Try out this demo and let us know what you think!

Blazorise provides more than 80 high-performance, lightweight, and responsive web UI components in a single package. Create charming web applications with them!