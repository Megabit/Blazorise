---
title: How to style Blazorise components
description: Discover
permalink: /blog/how-to-style-blazorise-components
canonical: /blog/how-to-style-blazorise-components
image-url: /img/blog/2024-07-19/how-to-style-blazorise-components.png
image-title: How to style Blazorise components
author-name: Giorgi
author-image: giorgi
posted-on: Jul 19th, 2024
read-time: 7 min
---

# Styling Blazorise components

Hello! today we are going to go over how to style Blazorise components!

As your know, Blazorise is a framework-agnostic library, this means we have a lot of options when it comes to choosing what type of framework we want to use.

Check out the [quick start](https://blazorise.com/docs/start) guide here to get started with Blazorise.

## In this article, we will go over the following:

- Styling Blazorise components#How does CSS work with Blazorise?|How does CSS work with Blazorise?
- Styling Blazorise components#How to style Blazorise components?|How to style Blazorise components?
- Styling Blazorise components#Limitations of CSS Isolation|Limitations of CSS Isolation

---

So let's dive into the topics and explore our options when it comes to styling Blazorise components.

## How does CSS work with Blazorise?

Blazorise, just like plain Blazor, is a framework that helps us generate responsive web UI. This means that Blazorise supports every CSS property that is supported by the browser. There are no special CSS properties that only apply to Blazor or Blazorise.

If you would like to find out how to give your Blazorise application a different theme, check out [the docs about theming](https://blazorise.com/docs/theming)

### Inline styling

Inline CSS styling can be applied to any Blazor component directly.

```html|InlineClass
<div style="color: red;">
    Hello from Blazorise!
</div>
```

Just like regular html elements, Blazor elements can receive all attributes such as style, class, type, and so on.

### CSS classes

Let's take a look at how we can use CSS classes to style our Blazor app.

First we should create a `styles.css` file inside the `wwwroot` folder and add a reference to it inside `App.razor` like so:

create styles.css
```css|StylesCss
.bg-topo {
    background-image: url("...");
}
```

App.razor
```html|AppRazor
<html>
<head>
    <link rel="stylesheet" href="styles.css" />
</head>
</html>
```

After adding this, we can use these classes as usual with our Blazorise components.

Let's see an example:
```html|UseClassesCss
<div Class="bg-topo">
    ...
</div>
```

The class will be applied to our div element, and we will see the background image appear.

This is pretty much all the basics covered for regular Blazor

## How to style Blazorise components

Styling Blazorise components is really straightforward. Blazorise being an abstraction over Blazor means that we can directly access the attributes that will be passed to the underlying HTML elements.

### Inline styling

Here is a small example that shows how we can use inline styles with Blazorise.

```html|BlazoriseInlineStylesExample
<Alert Color="Color.Success" Visible>
    <AlertMessage>Well done!</AlertMessage>
    <AlertDescription Style="color:red; font-size:46px">You successfully read this important alert message.</AlertDescription>
</Alert>
```

As we can see from [BaseComponent.customStyle](https://github.com/Megabit/Blazorise/blob/master/Source/Blazorise/Base/BaseComponent.cs#L22) all Blazorise components have support for the `Style` and `Class` and many other default attributes.

### CSS classes

Applying CSS classes is as straight forward as plain on Blazorise, we just specify the class like so:

```html|BlazoriseCSSExample
<Alert Color="Color.Success" Visible>
    <AlertMessage>Well done!</AlertMessage>
    <AlertDescription Class="bg-topo">You successfully read this important alert message.</AlertDescription>
</Alert>
```

### Theming

Blazorise has support for themes. Customize Blazorise with your theme. You can change the colors, the typography and much more using themes.

To learn more about theming, head over to the [documentation page](https://blazorise.com/docs/theming)

## Limitations of CSS Isolation

Blazorise just like other Blazor frameworks has a limitation with CSS isolation, you see, CSS Isolation works by compiling the CSS styles and bundling them up with the Assembly.

CSS isolation occurs at build time. Blazor rewrites CSS selectors to match markup rendered by the component. The rewritten CSS styles are bundled and produced as a static asset. The stylesheet is referenced inside the `<head>` tag ([location of `<head>` content](https://learn.microsoft.com/en-us/aspnet/core/blazor/project-structure?view=aspnetcore-8.0#location-of-head-and-body-content))

The following `<link>` element is added by default to an app created from the Blazor project templates:

```html|BlazorHtmlStylesLink
<link href="{ASSEMBLY NAME}.styles.css" rel="stylesheet">
```

The `{ASSEMBLY NAME}` placeholder is the project's assembly name.

Within the bundled file, each component is associated with a scope identifier. For each styled component, an HTML attribute is appended with the format `b-{STRING}`, where the `{STRING}` placeholder is a ten-character string generated by the framework. The identifier is unique for each app.

The problem here is that, the bundle is generated at compile time of the CSS.

### Let me illustrate this limitation with an example.

Let's create a component in our project, call it `TestComponent`

`TestComponent.razor`
```html|TestComponentRazor
<Div Class="isolated-class-name">
    ...
</Div>
```

Then create the scoped CSS file for it

`TestComponent.razor.css`
```css|TestComponentRazorCss
.isolated-class-name {
    /* class info here */
}
```

This will not work, because when your application is compiled, the generated `b-{STRING}` will be different than that of Blazorise's. This is a known limitation of Blazor, but the workaround is very simple!

### The workaround:
This is not really a "workaround" rather a different approach at the issue. You cannot make 3rd party libraries work with your isolated CSS classes, **however** what you can do, is move those classes to a separate CSS file hosted under `wwwroot`.

# Thanks for reading this blog and using Blazorise!!!