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

Learn how CSS works with Blazor, how to style Blazorise components, and the limitations of CSS isolation in Blazor.

Blazorise is an amazing component library that, is not really tied to any front-end framework. This means we have a lot of options when it comes to choosing which framework we want to use, for example: Bootstrap, Tailwind, Material and many others. check out the full list [here](https://blazorise.com/docs/usage/tailwind/)

Check out the [quick start](https://blazorise.com/docs/start) guide here to get started with Blazorise.

## In this article, we will go over the following:

- How does CSS work with Blazorise?
- How to style Blazorise components?
- Limitations of CSS Isolation

---

So let's dive into the topics and explore our options when it comes to styling Blazorise components.

## How does CSS work with Blazorise?

Blazorise supports every CSS property that is supported by the browser. There are no special CSS properties that only apply to Blazor or Blazorise.

We can pass any of these attributes to any Blazorise components, and using the magical `CaptureUnmatchedValues` option, we will capture all the attributes that are not directly caught by our parameters.
```cs|CaptureUnmatched
[Parameter(CaptureUnmatchedValues = true)]
public Dictionary<string, object> AdditionalAttributes { get; set; } = [];
```

To read up about this Blazor feature, head over to [blazor-university](https://blazor-university.com/components/capturing-unexpected-parameters/)


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

Create styles.css
```css|StylesCss
.bg-topo {
    background-image: url("...");
}
```

Inert the link tag inside App.razor
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

All Blazorise components support `Style`, `Class`, and many other attributes. These will get added directly to the underlying HTML element, as we can see from the [BaseComponent.razor](https://github.com/Megabit/Blazorise/blob/master/Source/Blazorise/Base/BaseComponent.cs#L379)

### CSS classes

Applying CSS classes to Blazorise is as straight forward as plain Blazor - we just supply the class parameter, like so:

```html|BlazoriseCSSExample
<Alert Color="Color.Success" Visible>
    <AlertMessage>Well done!</AlertMessage>
    <AlertDescription Class="bg-topo">You successfully read this important alert message.</AlertDescription>
</Alert>
```

> Notice that the parameter is spelled in Uppercase, that is because it is a Blazor parameter!

### Theming

Blazorise has support for themes. You can customize Blazorise with your own theme! Change the colors, the typography and much more using themes.

To learn more about theming, head over to the [documentation page](https://blazorise.com/docs/theming)

## Limitations of CSS Isolation

Blazor at the date of posting this blog, has a limitation with CSS isolation, you see, CSS Isolation works by compiling the CSS styles and bundling them up with the Assembly at compile time.

CSS isolation occurs at compile time. Blazor rewrites CSS selectors to match markup rendered by the component. The rewritten CSS styles are bundled and produced as a static asset. The stylesheet is referenced inside the `<head>` tag ([location of `<head>` content](https://learn.microsoft.com/en-us/aspnet/core/blazor/project-structure?view=aspnetcore-8.0#location-of-head-and-body-content))

The following `<link>` element is added by default to an app created from the Blazor project templates:

```html|BlazorHtmlStylesLink
<link href="{ASSEMBLY NAME}.styles.css" rel="stylesheet">
```

The `{ASSEMBLY NAME}` placeholder is the project's assembly name.

Within the bundled file, each component is associated with a scope identifier. For each styled component, an HTML attribute is appended with the format `b-{STRING}`, where the `{STRING}` placeholder is a ten-character string generated by the framework. The identifier is unique for each app.

### Let me illustrate this limitation with an example.


### Let's see an example, of how it works

Here is an example, of a component that uses CSS isolation and how CSS isolation works.

This is the `Component.razor`
```html|ComponentRazor
<div class="foo">div one</div>
<Blazorise.Div Class="foo">div one</Blazorise.Div>
```

This is the isolated CSS file - `Component.razor.css`
```css|ComponentRazorCss
.foo {
  background-color: purple;
}
```

After our project is compiled, the output `{ASSEMBLY NAME}.styles.css` will contain the following CSS:

```css|GeneratedCss
.foo[b-3xxtam6d07] {
  background-color: purple;
}
```

And the compiled html for our page will look like this:

```html|GeneratedHtml
<div class="foo" b-3xxtam6d07>div one</div>
<Blazorise.Div Class="foo">div one</Blazorise.Div>
```

The problem here is that, the bundle is generated at compile time. The b-string will be different from the b-string of Blazorise,

Because of this, the regular HTML div will be styled correctly, however the Blazorise component, because it has a different b-string, will not.

### The workaround:
This is not really a "workaround" rather a different approach at the issue. You cannot make 3rd party libraries work with your isolated CSS classes, **however** what you can do, is move those classes to a separate CSS file hosted under `wwwroot`.

Thanks for reading! we expect you in the next blog post!