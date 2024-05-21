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

Run the following commands in your terminal to add Blazorise as a dependency to your Blazor app.

```bash|InstallBlazorise
Install-Package Blazorise.Bootstrap
Install-Package Blazorise.Icons.FontAwesome
```

Alternatively you can use your favorite IDE's nuget package manager and add the packages automatically!

---

## Adding static files in `wwwroot/index.html`

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

```cs|
builder.Services
    .AddBlazorise()
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();
```

---

## Creating the ShareButton component

```html|ShareButtonComponentMarkup
<Button TextColor="@TextColor" Background="@(new Background(BackgroundColor))" To="@To" 
        Type="@ButtonType.Link" Size="Size.Large" @attributes="@AdditionalAttributes">

  @ChildContent

  <Icon Name="@IconName" IconStyle="IconStyle.Light"/>
</Button>
```
```cs|ShareButtonComponentCode
@code 
{
  [Parameter]
  public string TextColor { get; set; }

  [Parameter]
  public string BackgroundColor { get; set; }

  [Parameter]
  public string IconName { get; set; }

  [Parameter, EditorRequired]
  public string To { get; set; }

  [Parameter, EditorRequired]
  public RenderFragment ChildContent { get; set; }

  [Parameter(CaptureUnmatchedValues = true)]
  public Dictionary<string, object> AdditionalAttributes { get; set; } = [];
}
```

## Let's break down the component

### Button

First thing's first, we have the Button component, notice that it is typed as **B**utton and not button that is because it is not an ordinary HTML button, it is a Blazorise button component! This means it can take parameters, to allow us to customize it further!

> Just like other Blazorise components, this button is framework-agnostic, meaning you may use Bootstrap, TailwindCSS, or any other supported frameworks!

### The parameters
We have just enough parameters to allow for the exact customization necessary,

Here is a breakdown of what each one does:
- `TextColor` - The button's text color.
- `BackgroundColor` - The button's background color, we will define custom brand colors soon.
- `IconName` - The name of the icon displayed on the button, in this case we are using [FontAwesome](https://fontawesome.com/).
- `To` - The link where the user is navigated to, will bind to the `href` attribute on a regular link.
- `ChildContent` - The markup displayed inside the button. See [blazor-university](https://blazor-university.com/templating-components-with-renderfragements/).
- `AdditionalAttributes` - Any additional attributes the user passes to the button. Will directly be applied to the underlying button component. See [blazor-university](https://blazor-university.com/components/capturing-unexpected-parameters/).

---

## Define the brand colors in `wwwroot/brands.css`
Here are some colors, you may expand this further as you need

```html|Brands
.bg-snapchat {
  background-color: #FFFC00 !important;
}

.bg-discord {
  background-color: #5865F2 !important;
}

.bg-github {
  background-color: #0D1117 !important;
}

.bg-x {
  background-color: #000000 !important;
}

/* other brand colors. add the brands you need here! */
```
> The `!important` property, this is necessary as, by default the Bootstrap icons will have the `Color` property set to `primary`, this will shadow our custom background colors, so adding `!important` at the end of them will fix this.

---

## Include the `brands.css` in your app

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
<ShareButton TextColor="white" BackgroundColor="x" IconName="fa-brands fa-x" To="https://twitter.com/intent/tweet">
    Share on
</ShareButton>
```
> Notice how the `BackgroundColor` is assigned `"x"` and not `"bg-x"`, this is because Blazorise inserts the `bg-` prefix automatically for us. This is happening because for normal components, we would have Bootstrap colors so if we passed `"primary"` to the `BackgroundColor`, we would get `bg-primary`.

> Also notice how the text inside the button says `Share on` instead of `Share on X`, this is because X's logo is the letter X, so it would not make sense, so have X twice!

---

## Congratulations! You can now create ShareButtons in your web application!

![Share buttons](img/blog/2024-05-17/share-buttons.png)

---

Sharing your content with the world is important, it allows your users to show their friends what your page is all about! 

Blazorise makes it easy to develop framework-agnostic UI quickly.

Thanks for reading! 💗