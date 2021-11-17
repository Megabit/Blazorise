---
title: "Localization"
permalink: /docs/helpers/localization/
excerpt: "How to use localization with Blazorise."
toc: true
toc_label: "Guide"
---

## Overview

Localization within Blazorise can be done in multiple ways:

- By using `ITextLocalizer` and `ITextLocalizerService`.
- By using custom localizer handler(s) `TextLocalizerHandler`

List of predefined language that comes with Blazorise is:

- `de-DE` German
- `en-US` English
- `es-ES` Spanish
- `fr-FR` French
- `hr-HR` Croatian
- `nl-NL` Dutch
- `pt-PT` Portuguese

**Note:** If not specified, a default language(or culture) will be `en-US`.
{: .notice--info}

In this guide we will explain both ways.

## ITextLocalizerService

This is the preferred way and the easiest way of changing the components display text. To change a language you just need to inject `ITextLocalizerService` into your component or page and then call the `ChangeLanguage` method.

### Example

```cs
[Inject] ITextLocalizerService LocalizationService { get; set; }

Task OnButtonClick()
{
    return SelectCulture( "de-DE" );
}

Task SelectCulture( string name )
{
    LocalizationService.ChangeLanguage( name );

    return Task.CompletedTask;
}
```

## TextLocalizerHandler

`TextLocalizerHandler` handler is used to control the localization for each component individually. Once used it will override any localizations done by the `ITextLocalizer` or `ITextLocalizerService`.

While this is the good approach to handle localization, it is not very flexible since you need to do it on every component. But, for a quick override it is fairly easy. Just need to remember that.

### Example

```html
<FileEdit BrowseButtonLocalizer="@((name, arguments)=>"My custom browse button")" />
```

## Custom languages?

While Blazorise will have all standard languages built-in it cannot have every one out there. So, if you want to have your own language injected into the component you can add it with `AddLanguageResource` method, found on `ITextLocalizer`.

### Example

```cs
// By using FileEdit as generic typeparam, Blazorise will know
// what component need to update localization resources.
[Inject] ITextLocalizer<FileEdit> FileEditLocalizer { get; set; }

protected override Task OnInitializedAsync()
{
    FileEditLocalizer.AddLanguageResource( new TextLocalizationResource
    {
        Culture = "pl-PL",
        Translations = new Dictionary<string, string>()
        {
            { "Choose file", "Wybierz plik" },
            { "Choose files", "Wybierz pliki" },
        }
    } );

    return base.OnInitializedAsync();
}
```

After that you just need to call `ChangeLanguage` on `ITextLocalizerService` as usual.

```cs
LocalizationService.ChangeLanguage( "pl-PL" );
```