---
title: How to style Blazorise components
description: Discover
permalink: /blog/how-to-style-blazorise-components
canonical: /blog/how-to-style-blazorise-components
image-url: /img/blog/2024-08-25/how-to-style-blazorise-components.png
image-title: How to style Blazorise components
author-name: Giorgi
author-image: giorgi
posted-on: Aug 25th, 2024
read-time: 7 min
---

# Styling Blazorise components

Learn how to use CSS with Blazor, how to style Blazorise components, and the limitations of CSS isolation.

Blazorise is an amazing component library that, is not really tied to any front-end framework. This means we have a lot of options when it comes to choosing which framework we want to use, for example: Bootstrap, Tailwind, Material and many others. check out the full list [here](https://blazorise.com/docs/usage/tailwind/)

You can check out our [quick start](https://blazorise.com/docs/start) guide, to get started with Blazorise.

---

So let's dive into the topics and explore our options when it comes to styling Blazorise components.

## How does CSS work with Blazorise?

Blazorise supports every CSS property that is supported by the browser. There are no special CSS properties that only apply to Blazor or Blazorise.

### Inline styling

Inline CSS styling can be applied to any Blazor element directly.

```html|InlineClass
<div style="color: red;">
    Hello from Blazorise!
</div>
```

Just like regular html elements, Blazor elements can receive all attributes such as `style`, `class`, `type`, and so on.

### CSS classes

Let's take a look at how we can use CSS classes to style our Blazor app.

First we should create a `styles.css` file inside the `wwwroot` folder and link it inside `App.razor` like so:

Create styles.css
```css|StylesCss
.bg-red {
    background-color: red;
}
```

Insert the link tag inside App.razor
```html|AppRazor
<html>
<head>
    <link rel="stylesheet" href="styles.css" />
</head>
</html>
```

After adding this, we can use these classes like usual with our Blazor elements.

Let's see an example:
```html|UseClassesCss
<div class="bg-red">
    ...
</div>
```

The class will be applied to our div element, and we will see the background change to red.

This is pretty much all the basics covered for plain Blazor.

## How to style Blazorise components

Styling Blazorise components is really straightforward.

There are some options on how to go about this:

### Inline styling

Here is a small example that shows how we can use inline styles with Blazorise.

```html|BlazoriseInlineStylesExample
<Alert Color="Color.Success" Visible>
    <AlertMessage>Well done!</AlertMessage>
    <AlertDescription Style="color:red; font-size:46px">
        You successfully read this important alert message.
    </AlertDescription>
</Alert>
```

If we take look at [Blazorise/Base/BaseComponent.razor](https://github.com/Megabit/Blazorise/blob/master/Source/Blazorise/Base/BaseComponent.cs#L379), we can see that all Blazorise components have Class, Style, and other parameters that we can use. Those values will get added directly to the underlying html elements. 

### CSS classes

Applying CSS classes to Blazorise is as straight forward as plain Blazor - we just supply the class parameter, like so:

```html|BlazoriseCSSExample
<Alert Color="Color.Success" Visible>
    <AlertMessage>Well done!</AlertMessage>
    <AlertDescription Class="bg-topo">You successfully read this important alert message.</AlertDescription>
</Alert>
```

> Notice that the parameter is spelled in Uppercase, that is because it is a Blazor parameter!

### CSS Isolation

To use CSS isolation, we can create a CSS file with the same name as our Blazor component.

So if our component here is named `Alert.razor`:
```html|IsolationComponent
<div>
    Hello!
</div>
```

The scoped CSS file for it would be `Alert.razor.css`

```css|IsolationComponentCss
div {
    background-color: blue;
}
```

To link these classes to our HTML document, we should reference out `{ASSEMBLY NAME}.styles.css` in `App.razor`

```html|BlazorHtmlStylesLink
<link href="{ASSEMBLY NAME}.styles.css" rel="stylesheet">
```

### Theming

Blazorise has support for themes. You can customize Blazorise with your own theme! Change the colors, the typography and much more using themes.

Using Blazorise themes, you can easily change the colors of your application programmatically. Rebuild the default stylesheet, customize various aspects of the framework for your particular needs, and much, much more.

To learn more about theming, head over to the [documentation page](https://blazorise.com/docs/theming)

Thank you for reading! we expect you in the next blog post!