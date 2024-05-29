---
title: How to create social media share buttons
description: Discover how to create share buttons for your Blazor app!
permalink: /blog/how-to-create-social-media-share-buttons
canonical: /blog/how-to-create-social-media-share-buttons
image-url: /img/blog/2024-05-17/how-to-create-social-media-share-buttons.png
image-title: How to create social media share buttons with Blazorise
author-name: Giorgi
author-image: giorgi
posted-on: May 21st, 2024
read-time: 5 min
---

# How to create a ShareButton component with Blazorise!

Are you ready to sprinkle some Blazorise magic into your Blazor app? Adding share buttons for social media platforms can give your users an easy way to spread the word about your awesome content. It's easier than you think, thanks to Blazorise!

---

## Let's dive in and jazz up your app with these fantastic buttons.

## Installing Blazorise

You can follow [this](/blog/how-to-create-a-blazorise-application-beginners-guide) guide to install Blazorise into your project.

---

## Adding static files in the index.html

```html|HeadContent
<html>
<head>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.1/dist/css/bootstrap.min.css" 
        integrity="sha384-zCbKRCUGaJDkqS1kPbPd7TveP5iyJE0EjAuZQTgFLD2ylzuqKfdKlfG/eSrtxUkn" 
        crossorigin="anonymous">

  <link href="_content/Blazorise.Icons.FontAwesome/v6/css/all.min.css" rel="stylesheet">
  <link href="_content/Blazorise/blazorise.css" rel="stylesheet" />
  <link href="_content/Blazorise.Bootstrap/blazorise.bootstrap.css" rel="stylesheet" />
</head>
</html>
```

We can see that we are adding a few links to our head tag, let's break them down:
- The first link, adds a link to BootstrapCSS, this is required in order for our buttons to work! as we are using Bootstrap for this tutorial.
- The rest of the links, add Blazorise's dependencies to the web app. This is also very important!

---

## Adding Imports inside of the `_Imports.razor`

```html|UsingStatement
@using Blazorise
```

---

## Registering services

Add the following line at the top of your `Program.cs` file:

```cs|ProgramCsUsingStatements
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
```

and somewhere inside the file, register Blazorise's services like so:

```cs|ServiceRegistration
builder.Services
    .AddBlazorise()
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();
```

---

## Creating the brand record

We can create a record, that will hold all the information related to each social media platform we want to support sharing to!

`Platform.cs` 
```cs|Platform
public record Platform(string Name, string TextColor, string BackgroundColor, string IconName, string Href)
{
    public static Platform X => new Platform("X", "white", "x", "fa-brands fa-x", "https://twitter.com/intent/tweet");
    
    // your social media platform can go here.
}
```

> If you don't know what records are, you can read about then [here](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record). 

> They are a very useful concept, this is a perfect use for a record - an immutable data class.

## Creating the ShareButton component

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

### Button

First thing's first, we have the Button component, notice that it is typed as **B**utton and not button that is because it is not an ordinary HTML button, it is a Blazorise button component! This means it can take parameters, to allow us to customize it further!

> Just like other Blazorise components, this button is framework-agnostic, meaning you may use Bootstrap, TailwindCSS, or any other supported frameworks!

### The parameters
We have just enough parameters to allow for the exact customization necessary,

Here is a breakdown of what each one does:
1. `Platform` - The platform of the share button. The user will pass the platforms which will are statically defined inside the Platform record.
2. `ChildContent` - The markup displayed inside the button. See [blazor-university](https://blazor-university.com/templating-components-with-renderfragements/).
3. `Size` - The size of the button, this is a Blazorise class, so we can use Small, Medium, Large etc.
4. `AdditionalAttributes` - Any additional attributes the user passes to the button. Will directly be applied to the underlying button component. See [blazor-university](https://blazor-university.com/components/capturing-unexpected-parameters/).

---

## Define the brand colors in brands.css
Here are some colors, you may expand this further as you need

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
> The `!important` property, this is necessary as, by default the Bootstrap icons will have the `Color` property set to `primary`, this will shadow our custom background colors, so adding `!important` at the end of them will fix this.

---

## Include the brands.css file in your app

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

Inside your page, add the freshly created buttons

```html|ShareButtonUsage
<ShareButton Brand="@Platform.X">
    Share on
</ShareButton>
```
> Notice how the text inside the button says `Share on` instead of `Share on X`, this is because X's logo is literally the latin letter X, so it would not make sense, so have `Share on X ✖`

---

## Congratulations! You can now create ShareButtons in your web application!

![Share buttons](img/blog/2024-05-17/share-buttons.png)

---

Sharing your content with the world is important, it allows your users to show their friends what your page is all about! 

Blazorise makes it easy to develop framework-agnostic UI quickly.

Thanks for reading! 💗