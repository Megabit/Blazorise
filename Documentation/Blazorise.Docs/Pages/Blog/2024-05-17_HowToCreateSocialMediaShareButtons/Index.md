---
title: How to create social media share buttons
description: Discover how to create social media share buttons for your Blazor app! using the framework-agnostic component library Blazorise!
permalink: /blog/how-to-create-social-media-share-buttons
canonical: /blog/how-to-create-social-media-share-buttons
image-url: /img/blog/2024-05-17/how-to-create-social-media-share-buttons.png
image-title: How to create social media share buttons with Blazorise
author-name: Giorgi
author-image: giorgi
posted-on: Jun 12th, 2024
read-time: 5 min
---

# How to create a ShareButton component with Blazorise!

Are you ready to sprinkle some Blazorise magic into your Blazor app? Adding share buttons for social media platforms can give your users an easy way to spread the word about your awesome content. It's easier than you think, thanks to Blazorise!

---

## Installing Blazorise

We don't need to explain how to add Blazorise to your project in each blog, so [here](/blog/how-to-create-a-blazorise-application-beginners-guide) is the link to the existing guide on the topic! 

However, one important thing to keep in mind is, that we will need to include the FontAwesome icons in our app. To do this, add the following line to your `wwwroot/index.html`

```html|HeadContentIncludeFontAwesome
<html>
<head>
  <link href="_content/Blazorise.Icons.FontAwesome/v6/css/all.min.css" rel="stylesheet">
  ...
</head>
...
</html>
```

---

## Let's begin!

We can start with creating the class which will hold all the information related to each social media platform we want to support sharing to!

Create `Platform.cs`. We will use a record for this. Choosing the right tool to solve our problems is always a good idea. I think records are ideal for this scenario as they are basically immutable data classes, ie. unchangeable collections of values. You can read more about them [here](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record).

```cs|Platform
public record Platform(string Name, string TextColor, string BackgroundColor, string IconName, string Href)
{
    public static Platform X => new Platform("X", "white", "x", "fa-brands fa-x", "https://twitter.com/intent/tweet");
    
    // your social media platform can go here.
}
```

## Creating the ShareButton component

The next step will be to create the actual component which will be used as the share button. This component can go inside the `Components` folder. This folder is a great place for keeping components which are reused in many different places across your app! 

Create a file named `ShareButtonComponent.razor`, and write the following in it:

```html|ShareButtonComponentMarkup
<Button TextColor="@Platform.TextColor"
        Background="@(new Background(Platform.BackgroundColor))"
        To="@Platform.Href"
        Type="@ButtonType.Link"
        Size="@ButtonSize"
        @attributes="@AdditionalAttributes">

    @ChildContent

    <Icon Name="@($"fa-brands {Platform.IconName}")" IconStyle="IconStyle.Light"/>
</Button>
```
```cs|ShareButtonComponentCode
@code
{
    [Parameter, EditorRequired]
    public Platform Platform { get; set; }

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public Size ButtonSize { get; set; } = Size.Large;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();
}
```

## Let's break down the component

First thing's first, we have the Button component, notice that it is typed as **B**utton and not **b**utton that is because it is not an ordinary HTML button, it is the Blazorise **B**utton component! This means it can take parameters and allow us to customize it!

It's important to note, that all Blazor components must be named in PascalCase, this is necessary for them to be treated as components by the razor compiler. If you don't do this, they will not be treated as razor markup, rather, as regular HTML elements.

Just like other Blazorise components, this button is framework-agnostic, meaning you may use Bootstrap, TailwindCSS, or any other supported frameworks!

### The parameters

We have just enough of them to allow for the exact customization necessary, while keeping the component very simple to use. We will write this code once, and re-use it multiple times all around our application. This is the beauty of DRY and Component based front-end frameworks.

Here is a breakdown of what each parameter does:
1. `Platform` - The platform of the share button. The user will pass the platforms which will are statically defined inside the Platform record.
2. `ChildContent` - The markup displayed inside the button. See [blazor-university](https://blazor-university.com/templating-components-with-renderfragements/).
3. `Size` - The size of the button, this is a Blazorise class, so we can use Small, Medium, Large etc.
4. `AdditionalAttributes` - Any additional attributes the user passes to the button. Will directly be applied to the underlying button component. See [blazor-university](https://blazor-university.com/components/capturing-unexpected-parameters/).

To define parameters in Blazor, we use a publicly accessible property, with the `[Parameter]` attribute!

You may be wondering, what `[EditorRequired]` does, well, it is a really useful attribute. It marks a regular Parameter required, meaning that we will see warnings in our IDE when we don't pass the required parameters to our component. This is extremely useful to prevent accidental bugs while writing Blazor components. Because Blazor components are strongly typed, there is little to no room for making mistakes while using them.

---

## Define the brand colors in brands.css

Let's create another file inside the `/wwwroot/` folder. Let's name it `brands.css`. This CSS file will hold all of our brand colors. Here are some example colors, which you may expand further as you need to add more brands!

```html|Brands
.bg-x {
  background-color: #000000 !important;
}

.bg-discord {
  background-color: #5865F2 !important;
}

.bg-github {
  background-color: #0D1117 !important;
}

/* 
 * other brand colors... add the brands you need here!
 * IMPORTANT NOTE: please make sure, you prefix your class names with `bg-` 
 */
```

> **Note:** The `!important` property, this is necessary as, by default the Bootstrap icons will have the `Color` property set to `primary`, this will shadow our custom background colors, so adding `!important` at the end of them will fix this.

---

## Include the brands.css file in your app

Now, writing a simple CSS file is not really enough, we need to include it in our index.html's head section, so that the browser, can actually fetch our classes. Just add the following line to wwwroot/index.html:

```html|IndexhtmlHeadSection
<html>
<head>
  <link href="brands.css" rel="stylesheet" />
  ...
</head>
</html>
```

---

## Using the ShareButtons!

And already, we are done! It's time to use your freshly created component! Inside `Index.razor` add your component like so:

```html|ShareButtonUsage
<ShareButton Brand="@Platform.X">
    Share on
</ShareButton>
```

> **Note:** Notice how the text inside the button says `Share on` instead of `Share on X`, this is because X's logo is literally the latin letter X, so it would not make sense, so have `Share on X ✖`

> Also, try to omit the Brand parameter, or the ChildContent - which in this case is just the text "Share on". You will see warnings in your code, your IDE will warn you about not passing required parameters to your component.

---

## Congratulations! You can now create ShareButtons in your Blazor web application!

![Share buttons](img/blog/2024-05-17/share-buttons.png)

---

Sharing your content with the world is important, it allows your users to show their friends what your page is all about! 

Blazorise makes it easy to develop framework-agnostic UI quickly.

Thanks for reading! 💗