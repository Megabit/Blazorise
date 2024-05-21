---
title: How to create social media share buttons
description: Discover how to create share buttons for your Blazor app!
permalink: /blog/how-to-create-social-media-share-button
canonical: /blog/how-to-create-social-media-share-button
image-url: /img/blog/2024-05-17/how-to-create-social-media-share-buttons.png
image-title: How to create social media share buttons
author-name: Giorgi
author-image: giorgi
posted-on: May 17th, 2024
read-time: 5 min
---

# How to create a ShareButton component with [Blazorise](https://blazorise.com/)!

Are you ready to sprinkle some Blazorise magic into your Blazor app?<br/>
Adding share buttons for social media platforms can give your users an easy<br/>
way to spread the word about your awesome content. It's easier than you think, thanks to Blazorise!<br/>

---

## Let's dive in and jazz up your app with these fantastic buttons.

## Install Blazorise

```bash
Install-Package Blazorise.Bootstrap
Install-Package Blazorise.Icons.FontAwesome
```
Alternatively you can use your favorite IDE's nuget package manager and add the packages manually!

---

## Add Static Files

```html
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

---

## Add Imports inside `_Imports.razor`

```html
@using Blazorise
```

---

## Register Services

```cs
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

builder.Services
    .AddBlazorise()
    .AddTailwindProviders()
    .AddFontAwesomeIcons();
```

---

## Create the ShareButton component

```jsx
<Button TextColor="@TextColor" Background="@(new Background(BackgroundColor))" To="@To" 
        Type="@ButtonType.Link" Size="Size.Large" @attributes="@AdditionalAttributes">
  @ChildContent

  <Icon Name="@IconName" IconStyle="IconStyle.Light"/>
</Button>
```
```cs
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
First thing's first, we have the Button component, notice that it is typed as <**B**utton> and not \<button\>,
that is because it is not an ordinary HTML button, it is a Blazorise button component! This means it can take parameters,
to allow us to customize it further!

> INFO: Just like other Blazorise components, this button is framework agnostic, meaning you may use Bootstrap,
> TailwindCSS, or Material.

### The parameters
We have just enough parameters to allow for the exact customization necessary,

Here is a breakdown of what each one does:
- `TextColor` - The button's text color.
- `BackgroundColor` - The button's background color, we will define custom brand colors soon.
- `IconName` - The name of the icon displayed on the button, in this case we are using [FontAwesome](https://fontawesome.com/)
- `To` - The link where the user is navigated to, same as the `href` attribute on a regular link
- `ChildContent` - The markup displayed inside the button. See [blazor-university](https://blazor-university.com/templating-components-with-renderfragements/)
- `AdditionalAttributes` - Any additional attributes the user passes to the button. Will directly be applied to the underlying button component. See [blazor-university]( https://blazor-university.com/components/capturing-unexpected-parameters/)

---

## Define the brand colors in `wwwroot/brands.css`
Here are some colors, you may expand this further as you need

```css
.bg-snapchat {
  background-color: #FFFC00 !important;
}

.bg-discord {
  background-color: #5865F2 !important;
}

.bg-github {
  background-color: #0D1117 !important;
}

/* your colors */
```
> NOTE: the `!important` property, this is necessary as, by default the Bootstrap icons will have the `Color` property
> set to primary, this will shadow our custom background colors, so adding `!important` at the end of them will fix this

---

## Include the `brands.css` in your app

```html
<html>
<head>
  <link href="brands.css" rel="stylesheet" />
  ...
</head>
</html>
```

---

## Create the ShareButtons!
Inside your page, add the freshly created icon

```html
<ShareButton TextColor="white" BackgroundColor="github" IconName="fa-brands fa-github" To="https://github.com/ddjerqq">
    Share on GitHub
</ShareButton>
```
> NOTE notice how the `BackgroundColor` is assigned `"github"` and not `"bg-github"`, this is because Blazorise inserts
> the `bg-` prefix automatically for us. This is happening because for normal components, we would have Bootstrap colors
> so if we passed `"primary"` to the `BackgroundColor`, we would get `bg-primary`.

---

## Congratulations! You can now create ShareButtons in your Blazor Application.

![image](https://gist.github.com/assets/57017344/9c5f2314-dc46-42ef-b629-1067a950f9bb)

---

Sharing your content with the world is important,
it allows your users to show their friends what your page is all about!
Blazorise makes it easy to develop framework agnostic UI quickly.

Thanks for reading this blog 💗