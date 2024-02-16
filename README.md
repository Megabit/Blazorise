![Blazorise](https://user-images.githubusercontent.com/900302/147649481-11ca2931-34cd-4e24-8035-fe757cf9d744.png)

# Components for Blazor

[![NuGet](https://img.shields.io/nuget/vpre/Blazorise.svg)](https://www.nuget.org/profiles/Megabit)
![Nuget](https://img.shields.io/nuget/dt/Blazorise.svg)
[![Discord](https://img.shields.io/discord/761589226965696552?color=%237289da&label=Discord&logo=discord&logoColor=%237289da&style=flat-square)](https://discord.io/blazorise)
[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](LICENSE.md)
[![Tip Me via PayPal](https://img.shields.io/badge/PayPal-tip%20me-green.svg?logo=paypal)](https://www.paypal.me/mladenmacanovic)
[![Patreon](https://img.shields.io/badge/Patreon-donate-yellow.svg)](https://www.patreon.com/mladenmacanovic)

Blazorise is the only [Blazor](https://blazor.net/) component library offering development independent of CSS frameworks, exclusively using C#. It provides support for various frameworks including Bootstrap, Tailwind and Material.

## Commercial usage

Blazorise, as a commercial product, offers a range of subscription plans that include premium support. When you choose a commercial license, you gain several advantages:

The benefits of the commercial licenses are:

- Access to [Blazorise Themes](https://blazorise.com/themes).
- Access to [Blazorise Blocks](https://blazorise.com/blocks).
- Premium support via [Blazorise Support](https://blazorise.com/support) forum.
- Dedicated customer support with swift response times of either 24 or 16 hours.
- Priority fixes and feature requests.

> \* Some features may be exclusive to specific subscription tiers.

## Supporting Blazorise

Blazorise is an open source project with its ongoing development made possible entirely by the support of these awesome backers.

### Special Partners

<!--platinum start-->
<table>
  <tbody>
    <tr>
      <td align="center" valign="middle">
        <a href="https://volosoft.com/" target="_blank">
          <img width="222px" src="https://volosoft.com/assets/logos/volosoft-logo-dark.svg">
        </a>
      </td>
      <td align="center" valign="middle">
        <a href="https://www.pebble.tv/" target="_blank">
          <img width="222px" src="https://www.pebble.tv/wp-content/uploads/2020/10/logo.svg">
        </a>
      </td>
    </tr>
    <tr></tr>
  </tbody>
</table>
<!--platinum end-->

## Demos

### Blazor WebAssembly

- [Tailwind Demo](https://tailwinddemo.blazorise.com)
- [Bootstrap 4 Demo](https://bootstrapdemo.blazorise.com)
- [Bootstrap 5 Demo](https://bootstrap5demo.blazorise.com)
- [Material Demo](https://materialdemo.blazorise.com/)
- [Bulma Demo](https://bulmademo.blazorise.com/)
- [AntDesign Demo](https://antdesigndemo.blazorise.com/)

### Blazor Server

- [Bootstrap Demo](https://rcbootstrapdemo.blazorise.com/)

## Documentation

For full documentation, with code samples and a detailed explanation of each component please visit the Blazorise [official documentation pages](https://blazorise.com/docs/).

Continuing reading below for a quick start guide.

## Installations

### Prerequisites

Before you continue, please make sure you have the latest version of Visual Studio and .NET installed. Visit an official [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/client) site to learn more.

### Available packages

There are currently 6 different NuGet packages for each of the supported CSS frameworks.

Available Blazorise packages are:

```
1. Blazorise.Tailwind
2. Blazorise.Bootstrap
3. Blazorise.Bootstrap5
4. Blazorise.Bulma
5. Blazorise.Material
6. Blazorise.AntDesign
```

This guide will show you how to setup Blazorise with **Bootstrap 4** and **FontAwesome** icons. To setup Blazorise for other CSS frameworks, please refer the [Usage](https://blazorise.com/docs/usage/) page in the documentation.

### 1. NuGet packages

First step is to install a Bootstrap provider for Blazorise:

```bash
dotnet add package Blazorise.Bootstrap
```

And FontAwesome icon package:

```bash
dotnet add package Blazorise.Icons.FontAwesome
```

### 2. Source files

Add the following to `index.html` (Blazor WebAssembly) or `_Host.cshtml` (Blazor Server) in the `head` section.

```html
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/css/bootstrap.min.css" integrity="sha384-B0vP5xmATw1+K9KRQjQERJvTumQW0nPEzvF6L/Z6nronJ3oUOFUFpCjEUQouq2+l" crossorigin="anonymous">
<link href="_content/Blazorise.Icons.FontAwesome/v6/css/all.min.css" rel="stylesheet">

<link href="_content/Blazorise/blazorise.css?v=1.4.1.0" rel="stylesheet" />
<link href="_content/Blazorise.Bootstrap/blazorise.bootstrap.css?v=1.4.1.0" rel="stylesheet" />
```

Add the following to `index.html` or `_Host.cshtml` in the `body` section.

```html
<script src="https://code.jquery.com/jquery-3.5.1.slim.min.js" integrity="sha384-DfXdz2htPH0lsSSs5nCTpuj/zy4C+OGpamoFVy38MVBnE+IbbVYUew+OrCXaRkfj" crossorigin="anonymous"></script>
<script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.1/dist/umd/popper.min.js" integrity="sha384-9/reFTGAW83EW2RDu2S0VKaIzap3H66lZH81PoYlFhbGU+6BZp6G7niu735Sk7lN" crossorigin="anonymous"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/js/bootstrap.min.js" integrity="sha384-+YQ4JLhjyBLPDQt//I+STsc9iw4uQqACwlvpslubQzn4u2UU2UFM80nGisd026JF" crossorigin="anonymous"></script>
```

Please note, that these are the Blazorise explicit dependencies, you still need to add framework specific dependencies.

#### 2.1 JavaScript resources

Blazorise loads any additional JavaScript it needs dynamically once a component needs it. This means that Blazorise expects that the resources are available and placed relative to the app root. You can configure this by using the `app.UseStaticFiles();` and it does not need any other additional configuration from your part.

If you're having any difficulties, please refer to the following issues:

- [#3122](https://github.com/Megabit/Blazorise/issues/3122)
- [#3150](https://github.com/Megabit/Blazorise/issues/3150)

We are also aware that there might need to be extra setup when dealing with PWA and offline capabilities if you want your app to remain responsive. Please check our [PWA docs](https://blazorise.com/docs/pwa) for more information. 

### 3. Using's

In your main `_Imports.razor` add:

```cs
@using Blazorise
```

### 4. Registrations

Add the following lines to the relevant sections of `Program.cs`.

```cs
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
```

```cs
builder.Services
  .AddBlazorise()
  .AddBootstrapProviders()
  .AddFontAwesomeIcons();
```

## Usage

```razor
@page "/counter"

<Heading Size="HeadingSize.Is1">Counter</Heading>

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

## Development version

We release a development version of Blazorise on a regular basis. This version is available on the [MyGet](https://www.myget.org/gallery/blazorise) feed. 
This release is not recommended for production use, but it's a great way to test the latest features, bug fixes and gives us some feedback ahead of the upcoming release.


## Contributing

We welcome contributions and any suggestions or feature requests you might have. Contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. This will be signed once you submit a PullRequest on our repository. For details about our CLA, please visit: [Contributor License Agreement](https://gist.github.com/stsrki/abfa5ce0f4a5cf1e6ac67b92f8eb5d63).

For our code conventions and guidelines please visit: [Contributing Guide](https://github.com/Megabit/Blazorise/wiki/Contributing)