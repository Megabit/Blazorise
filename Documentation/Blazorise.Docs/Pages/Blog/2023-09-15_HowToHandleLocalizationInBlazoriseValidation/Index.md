---
title: How to handle Localization in Blazorise Validation
description: In this blog post we'll teach you how to properly localize Blazorise Validation messages by using the inbuilt MessageLocalizer.
permalink: /blog/how-to-handle-localization-in-blazorise-validation
canonical: /blog/how-to-handle-localization-in-blazorise-validation
image-url: /img/blog/2023-09-15/how_to_handle_localization_in_blazorise_validation.png
image-title: How to handle Localization in Blazorise Validation
author-name: David Moreira
author-image: david
posted-on: September 15th, 2023
read-time: 8 min
---

# How to handle Localization in Blazorise Validation

In this blog post we'll teach you how to properly localize Blazorise Validation messages by using the inbuilt `MessageLocalizer`.

## The Problem

Let's start with a simple example. We have a `ValidationLocalizationExample` class with a `PhoneCountryCode` property that has a `DataAnnotations` validation to validate whether the input would be a valid phone country code.

We want to validate this property and display a localized error message if the validation fails. We do so by providing the `Feedback` and `ValidationError` components in addition to the regular [Blazorise Validation](docs/components/validation) components.

```html|ValidationLocalizationExample

<Validations @ref=_validationsRef HandlerType="ValidationHandlerType.DataAnnotation" Model="_model">
    <Validation>
        <Field>
            <FieldLabel>Phone Country Code</FieldLabel>
            <TextEdit @bind-Value="@_model.PhoneCountryCode">
                <Feedback>
                    <ValidationError />
                </Feedback>
            </TextEdit>
        </Field>
    </Validation>
</Validations>

<Button Clicked="Submit">Submit</Button>
@code {
    private ValidationLocalizationExample _model = new();
    private Validations _validationsRef;
    public class ValidationLocalizationExample
    {
        [RegularExpression( @"^(\+?\d{1,3}|\d{1,4})$" )]
        public string PhoneCountryCode { get; set; }
    }

    private async Task Submit()
    {
        if (await _validationsRef.ValidateAll())
        {
            Console.WriteLine( "Validation Success!" );
        }
    }
}

```

![Validation Standard Error Message](img/blog/2023-09-15/validation-fail-standard-error-message.png)

As we can see in the screenshot above, by default, the `DataAnnotations` validation will display the error message in English, with two arguments that have been inserted in the message

-  The name of the field that failed, and
 - The regular expression for the validation that failed.

But what if we want to display the error message in another language? Or just change the message to something else? Enter the `MessageLocalizer`!

### What is & How to use the Message Localizer

The `MessageLocalizer` is a parameter that can be set in the `Validation` component. It is used to localize the error messages that are displayed when a validation fails.

This parameter is of type `Func<string, IEnumerable<string>? arguments, string>` and it receives the error message and corresponding arguments, if any, and returns the localized error message.

So by providing any delegate that adheres to this signature, we can localize the error messages. We'll go one step further and show you an example of how you can create an abstraction that utilizes the .NET `IStringLocalizer` to localize the error messages.

### Creating a custom Message Localizer

We'll start by creating a helper class that will be used to localize the error messages. Let's call it `MessageLocalizerHelper`.

This class will receive a `Microsoft.Extensions.Localization.IStringLocalizer<T>` in the constructor and will have a `Localize` method that will receive the error message and arguments and return the localized error message.

```cs|MessageLocalizerHelperExample
public class MessageLocalizerHelper<T>
{
    private readonly Microsoft.Extensions.Localization.IStringLocalizer<T> stringLocalizer;

    public MessageLocalizerHelper( Microsoft.Extensions.Localization.IStringLocalizer<T> stringLocalizer )
    {
        this.stringLocalizer = stringLocalizer;
    }

    public string Localize( string message, IEnumerable<string>? arguments )
    {
        try
        {
            return arguments?.Count() > 0
                ? stringLocalizer[message, LocalizeMessageArguments( arguments )?.ToArray()!]
                : stringLocalizer[message];
        }
        catch
        {
            return stringLocalizer[message];
        }
    }

    private IEnumerable<string> LocalizeMessageArguments( IEnumerable<string> arguments )
    {
        foreach (var argument in arguments)
        {
            // first try to localize with "DisplayName:{Name}"
            var localization = stringLocalizer[$"DisplayName:{argument}"];

            if (localization.ResourceNotFound)
            {
                // then try to localize with just "{Name}"
                localization = stringLocalizer[argument];

                yield return localization;
            }
        }
    }
}
```

### Applying a custom Message Localizer

All that's left to do is to inject your Message Localizer in your page or component where the validations are being run and set the Localize method as the `MessageLocalizer` parameter.

The following shows a full example of how this would work:

**NOTE:** This example assumes you're using Dependency Injection. Do not forget to register your dependencies!

```html|ValidationLocalizationFullExample
<Validations @ref=_validationsRef HandlerType="ValidationHandlerType.DataAnnotation" Model="_model">
    <Validation MessageLocalizer="MessageLocalizer.Localize">
        <Field>
            <FieldLabel>Phone Country Code</FieldLabel>
            <TextEdit @bind-Value="@_model.PhoneCountryCode">
                <Feedback>
                    <ValidationError />
                </Feedback>
            </TextEdit>
        </Field>
    </Validation>
</Validations>

<Button Clicked="Submit">Submit</Button>
@code {
    [Inject] public MessageLocalizerHelper<Dashboard> MessageLocalizer { get; set; }

    private ValidationLocalizationExample _model = new();
    private Validations _validationsRef;

    private async Task Submit()
    {
        if (await _validationsRef.ValidateAll())
        {
            Console.WriteLine( "Validation Success!" );
        }
    }

    public class ValidationLocalizationExample
    {
        [RegularExpression( @"^(\+?\d{1,3}|\d{1,4})$" )]
        public string PhoneCountryCode { get; set; }
    }


    public class MessageLocalizerHelper<T>
    {
        private readonly Microsoft.Extensions.Localization.IStringLocalizer<T> stringLocalizer;

        public MessageLocalizerHelper( Microsoft.Extensions.Localization.IStringLocalizer<T> stringLocalizer )
        {
            this.stringLocalizer = stringLocalizer;
        }

        public string Localize( string message, IEnumerable<string>? arguments )
        {
            try
            {
                return arguments?.Count() > 0
                    ? stringLocalizer[message, LocalizeMessageArguments( arguments )?.ToArray()!]
                    : stringLocalizer[message];
            }
            catch
            {
                return stringLocalizer[message];
            }
        }

        private IEnumerable<string> LocalizeMessageArguments( IEnumerable<string> arguments )
        {
            foreach (var argument in arguments)
            {
                // first try to localize with "DisplayName:{Name}"
                var localization = stringLocalizer[$"DisplayName:{argument}"];

                if (localization.ResourceNotFound)
                {
                    // then try to localize with just "{Name}"
                    localization = stringLocalizer[argument];

                    yield return localization;
                }
            }
        }
    }
}

```
## Conclusion

We've looked at a simple example of how to localize the error messages that are displayed when a validation fails. We've also shown how to create a custom Message Localizer that utilizes the .NET `IStringLocalizer` to localize the error messages.

With this knowledge you should be able to localize the error messages in your Blazorise Validation components. We hope this blog post was helpful and that you've learned something new about Blazorise! 