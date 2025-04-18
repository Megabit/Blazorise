---
title: Blazor and Tailwind - Quick Setup Without npm
description: Learn how to seamlessly integrate TailwindCSS with Blazor for a streamlined and efficient web development experience without using npm.
permalink: /blog/blazor-and-tailwind-quick-setup-without-npm
canonical: /blog/blazor-and-tailwind-quick-setup-without-npm
image-url: /img/blog/2024-08-18/blazor-plus-tailwind.png
image-text: Blazor and Tailwind - Quick Setup Without npm
author-name: Jan Tesař
author-image: tesy
posted-on: August 18th, 2024
read-time: 8 min
---

# Blazor and Tailwind - Quick Setup Without npm

This guide will show you how to set up TailwindCSS in your Blazor application without using npm, explains why TailwindCSS is beneficial, compares it to Bootstrap, walks you through setting up the Tailwind CLI, and offers tips for streamlining your development process and building a production-ready pipeline.

If you want to quickly try TailwindCSS in your Blazor application, you can easily do so using the CDN. Just add the following script to your `App.razor` (Blazor WebApp) or `index.html` (Blazor WASM Standalone app):

```html
  <script src="https://cdn.tailwindcss.com"></script>
```
Now use any tailwind class. And it will just work. 

```html
<div class="w-fit rounded-lg bg-cyan-500 px-5 py-4 text-6xl text-[#aa22ff]">
  Hello Blazor and Tailwind
</div>
```

![hello](img/blog/2024-08-18/img.png)

## TailwindCSS CDN is Not for Production

The CDN file is a 112KB (compressed) js (not css) file that scans for any Tailwind classes and adds their definitions directly as a `<style>` element in the `<head>` of the current page. You can even configure it by adding values inside a `<script>` tag in the `<head>`. While this approach works well for initial testing, it has limitations.

The biggest concerns are file size and performance—especially as noted [here](https://github.com/tailwindlabs/tailwindcss/discussions/7637), "...noticeable if you ever have elements that are dynamically added".

You'll also receive a warning in the developer console:

> cdn.tailwindcss.com should not be used in production. To use Tailwind CSS in production, install it as a PostCSS plugin or use the Tailwind CLI: https://tailwindcss.com/docs/installation

The Tailwind CLI mentioned here is the approach we'll be using.

## Why TailwindCSS?

**TL;DR:** It's efficient. You gonna develop faster...

TailwindCSS simplifies your development process through its utility-based class approach.

- You can see exactly which styles are applied directly in your markup.
- Eliminates the headache of naming CSS classes and avoids the common issue of wondering whether you're allowed to change a specific color or style in a certain context.
- Usually results in a smaller CSS payload since many classes are reused throughout your project.
- The flexibility of arbitrary classes and arbitrary variants means you can style almost anything. 
- You won't have to worry about "dead" CSS classes that you're hesitant to delete.

However, TailwindCSS does come with a few drawbacks. One notable downside is that it can lead to longer markup code, as you're applying multiple utility classes directly in your markup rather than abstracting them into a separate CSS file:

```html
<button class="bg-blue-500 text-white py-2 px-4 rounded">
```
versus the traditional CSS approach:

```html
<button class="myButton">
```

Other drawbacks are:
- Need for some kind of build tool (which is what this article is about)
- Learning curve (as always with new stuff)
- Lack of concern separation—elements and their styles are in one place. This can be an issue when different people are responsible for HTML and CSS.



### Tailwind vs Bootstrap

There are many similarities between TailwindCSS and the well-known and established CSS framework [Bootstrap](https://getbootstrap.com/). Both frameworks even share several class names like `border`, `opacity-50`, `top-0`, and `visible`. However, TailwindCSS takes the concept of utility classes to the extreme compared to Bootstrap.

Bootstrap sits somewhere in the middle between custom CSS and TailwindCSS:

- With Tailwind, you usually don't need to write any custom CSS. You can define almost everything directly in your HTML, with the help of arbitrary classes and variants.
- Bootstrap provides classes for components like alerts, buttons, and modals. Tailwind doesn’t offer these out of the box, and while you can create similar components in CSS, it's generally not recommended as it goes against Tailwind’s utility-first approach.
- Tailwind allows for greater customization, enabling unique designs, whereas Bootstrap often results in more uniform and similar-looking pages.

## TailwindCSS CLI

The standard way to use TailwindCSS is by installing it via npm, creating a configuration file, and setting up build actions. If you're already comfortable with npm and don't mind adding another package, this approach works well.

However, this article focuses on using TailwindCSS without requiring npm, which is common in .NET projects. TailwindCSS offers a [standalone CLI executable](https://tailwindcss.com/blog/standalone-cli) to help with this.


### Installation

You can download the TailwindCSS standalone executable [here](https://github.com/tailwindlabs/tailwindcss/releases).

On Windows, you can install it with a single command using winget :

```bash
winget install TailwindLabs.TailwindCSS
```

Afterward, you'll need to reload the PATH (simply by closing and reopening the terminal tab).

On Linux, use the following commands:

```bash
mkdir -p ~/.local/bin
wget https://github.com/tailwindlabs/tailwindcss/releases/latest/download/tailwindcss-linux-x64 -O ~/.local/bin/tailwindcss
chmod +x ~/.local/bin/tailwindcss
```

Then, test the installation with:

```bash
tailwindcss
```
Now, you should see the TailwindCSS CLI is working.

## Adding it to Blazor

I will describe the scenario where you have a .NET 8 Blazor Web App, building on the official TailwindCSS [installation guide](https://tailwindcss.com/docs/installation).

First, navigate to the root of your Blazor project (where the `.csproj` file is located. If you also have the WASM client part, navigate to the server project) and:

```bash
tailwindcss init
```

This will create the `tailwind.config.js` file (or you can do it manually).

(BTW this step won't be necessary with [upcoming v4](https://tailwindcss.com/blog/tailwindcss-v4-alpha#css-first-configuration), where the config will happen through the CSS file.)

Open the `tailwind.config.js` and make it look akin to this:

```js
/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './Components/**/*.razor',
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}
```

This adds paths for the Tailwind CLI to look for the Tailwind classes. You need to add all the paths where your Razor files are. For example, if you also have a WASM client project, you probably want to add something like: `'../NameOfYourProj.Client/**/*.razor'`.

Then, go to your `wwwroot/app.css` and add this to the top of the file:

```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

Now run `tailwindcss` in your terminal. It will scan through your Razor files, find Tailwind classes, and create a new CSS file called `app.min.css`:

```bash
tailwindcss -i wwwroot/app.css -o wwwroot/app.min.css -w
```

You will get output similar to this:

```bash
Rebuilding...

Done in 187ms.
```

Tailwind will copy all the content from `app.css` into `app.min.css` and also add all the definitions of the classes you are using (the outputted file should be ignored by git, more about that later). Now we need to use the file, so change the `<link>` tag inside `App.razor` (or `index.html`):

```razor
@* <link rel="stylesheet" href="app.css" /> *@
   <link rel="stylesheet" href="app.min.css" />
```

Now this code:

```html
<div class="w-fit rounded-lg bg-cyan-500 px-5 py-4 text-6xl text-[#aa22ff]">
  Hello Blazor and Tailwind
</div>
```

Should result in:

![hello](img/blog/2024-08-18/img.png)


## Workflow with TailwindCSS

Now that TailwindCSS is set up, what's the best workflow for using it?

### Running it in the Terminal

From my experience, the most efficient workflow is to open the terminal in the directory where `tailwind.config.js` resides and run the following command:

```bash
tailwindcss -i wwwroot/app.css -o wwwroot/app.min.css -w
```

The `-w` parameter stands for `watch`, which keeps the program running. Every time you save a file, TailwindCSS will search for classes and recreate the final CSS (`app.min.css`).

You can simplify this command to just two letters (`tw`) by editing your PowerShell profile:

```bash
code $Profile # opens powershell profile with vscode
```

Add the following function:

```ps1
function tw {tailwindcss -i .\wwwroot\app.css -o .\wwwroot\app.min.css -w}
```

For Linux:

```bash
nano ~/.bashrc
```

Add the alias:

```bash
alias tw='tailwindcss -i ./wwwroot/app.css -o ./wwwroot/app.min.css -w'
```

Then running `tw` in your terminal will do the job.

### Adding a Target in .csproj

You can configure your `.csproj` file to automatically run TailwindCSS every time the project is compiled:

```xml
<Target Name="UpdateTailwindCSS" BeforeTargets="Compile">
  <Exec Command="./tailwindcss .\wwwroot\app.css -o .\wwwroot\app.min.css" ContinueOnError="true">
    <Output TaskParameter="ExitCode" PropertyName="ErrorCode"/>
  </Exec>
  <Error Condition="'$(ErrorCode)' != '0'" Text="Error building CSS file"/>
</Target>
```

The drawback with this approach is that it only runs once during compilation. If you modify your code and hot-reload kicks in, TailwindCSS won't update automatically. Adding the `-w` parameter would start a new process every time you recompile, which isn’t ideal.

### IDE-Based Solutions

You can also create a Run Configuration in your IDE to run the command. However, this approach has the same limitation as the `.csproj` solution—TailwindCSS won’t update automatically with hot-reloads.

If you have any ideas on how to streamline this process further, feel free to share them.

## Build Pipeline for Production

As mentioned earlier, the generated `app.min.css` file should not be included in your git repository. To ensure this, add the following line to your `.gitignore` file:

```bash
YourProject/wwwroot/app.min.css
```

Changes to this file are not important because it will be regenerated each time someone runs the Tailwind CLI.

For production, you should set up a build pipeline similar to this:

```yml
- name: Tailwind - Download and Run CLI
  run: |
    wget https://github.com/tailwindlabs/tailwindcss/releases/latest/download/tailwindcss-linux-x64 -O /usr/local/bin/tailwindcss
    chmod +x /usr/local/bin/tailwindcss
    cd ./YourProject # where the tailwind.config.js is located
    tailwindcss -o ./wwwroot/app.css -o ./wwwroot/app.min.css --minify
```

This script mirrors the Linux command I described earlier, with the added step of minifying the CSS file. You’ll want to run this step early in your pipeline (mainly before running `dotnet publish`).

## Tailwind Integration with Blazorise

There are several ways to integrate Tailwind into a project that uses Blazorise components.

First, there is the [Tailwind provider](https://blazorise.com/docs/usage/tailwind/) for Blazorise, which utilizes Flowbite components under the hood.

There are three ways to work with the Tailwind provider and your custom classes.

### Tailwind CDN for Quick Testing

To get started quickly, just add the following to your `<head>` tag:

```html
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&amp;display=swap" rel="stylesheet">
<link rel="stylesheet" href="https://unpkg.com/flowbite@1.5.4/dist/flowbite.min.css" />
<link href="_content/Blazorise.Icons.FontAwesome/v6/css/all.min.css" rel="stylesheet">

<!-- CDN to enable Tailwind classes -->
<script src="https://cdn.tailwindcss.com"></script>
<!-- Config for custom Tailwind classes, e.g., primary, secondary, success, etc. -->
<script src="_content/Blazorise.Tailwind/blazorise.tailwind.config.js?v=1.7.6.0"></script>

<!-- Custom CSS shared among all style providers, usually classes with 'b-' prefix (like '.b-input-color-picker') -->
<link href="_content/Blazorise/blazorise.css" rel="stylesheet" />

<!-- Specific classes related to Tailwind -->
<link href="_content/Blazorise.Tailwind/blazorise.tailwind.css" rel="stylesheet" />
```

This allows you to add your own classes, with the CDN handling them. However, as mentioned earlier, this is not suitable for production.

### Production-Ready CSS

For a production environment, simply update the links in your `<head>` tag. I'll comment on the changes from the previous code block:

```html
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&amp;display=swap" rel="stylesheet">
<link rel="stylesheet" href="https://unpkg.com/flowbite@1.5.4/dist/flowbite.min.css" />
<link href="_content/Blazorise.Icons.FontAwesome/v6/css/all.min.css" rel="stylesheet">
<link href="_content/Blazorise/blazorise.css" rel="stylesheet" />

<!-- Bundled CSS using tailwind.config.js, blazorise.tailwind.css, 
     and all Tailwind classes used for Blazorise components (no need for Tailwind CDN) -->
<link href="_content/Blazorise.Tailwind/blazorise.tailwind.prod.css" rel="stylesheet" />
```

All the classes that describe Blazorise components are bundled in `blazorise.tailwind.prod.css`.

With production-ready CSS, you achieve the same setup as using a provider like Bootstrap—just a few links to ensure everything works.

### Production-Ready CSS + Additional Tailwind Classes

If you need additional Tailwind classes, follow the general Blazor & Tailwind guide mentioned earlier. You'll end up with `tailwind.config.js` in your project root and a build action to generate your final CSS, which you'll also add to the `<head>` tag (`<link rel="stylesheet" href="app.min.css"/>`). Some classes might be duplicated in `blazorise.tailwind.prod.css` and your `app.min.css`, which isn't an issue for running the app but might affect your goal of minimizing payload size.

### Production-Ready CSS + Additional Tailwind Classes in One Bundle

For this approach, you'll need some extra configuration. First, download the following files to your project root:

- [Class Safelist](https://github.com/Megabit/Blazorise/blob/master/Source/Blazorise.Tailwind/wwwroot/tailwind.safelist.config.js) containing the classes used by Blazorise components.
- [Tailwind Config](https://github.com/Megabit/Blazorise/blob/master/Source/Blazorise.Tailwind/wwwroot/tailwind.config.js) which uses the safelisted classes and includes settings to ensure all necessary css is generated.

Use the `tailwind.config.js` as your main config file. Don't forget to update the 'content' section to fit your project's needs.

The final HTML inside your `<head>` tag will look like this:

```html
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&amp;display=swap" rel="stylesheet">
<link rel="stylesheet" href="https://unpkg.com/flowbite@1.5.4/dist/flowbite.min.css" />
<link href="_content/Blazorise.Icons.FontAwesome/v6/css/all.min.css" rel="stylesheet">
<link href="_content/Blazorise/blazorise.css" rel="stylesheet" />
<link href="_content/Blazorise.Tailwind/blazorise.tailwind.css" rel="stylesheet" />

<link rel="stylesheet" href="app.min.css"/>
```

### Using Tailwind with Different Style Providers

You can also use Tailwind with different style providers by following the guide above. In this case, I recommend using a prefix for your Tailwind classes to avoid conflicts with classes from other frameworks (e.g., Bootstrap).

```js
// tailwind.config.js
module.exports = {
  prefix: 'tw-',
}
```

Then, update all your Tailwind classes like this:

```html
<div class="tw-bg-blue-500 tw-p-4">Hello!</div>
```

## Conclusion

With this guide, you should be well-equipped to start leveraging the advantages of Blazor and TailwindCSS combo. You can see these in action in the [BlazorAndTailwind repository](https://github.com/tesar-tech/BlazorAndTailwind), which includes the full build pipeline, additional tips, and useful links.