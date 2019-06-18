---
title: "Validation"
permalink: /docs/components/validation/
excerpt: "Learn how to use automatic and manual validation for input components."
toc: true
toc_label: "Guide"
---

Validation component is used to provide simple form validation for Blazorise input components. The basic structure for validation component is:

- `<Validations>` optional container for manual validation
  - `<Validation>` input container
    - `<Feedback>` messages placeholder
      - `<ValidationSuccess>` success message
      - `<ValidationError>` error message

## Basic validation

For the most part you will need to use just the `<Validation>` component along with `<ValidationSuccess>` and `<ValidationError>`. By default every validation will run automatically when input value changes. You must set the `Validator` event handler where you can define the validation rules and return the validation result.

### Example

Here you can see the basic example for automatic validation and a custom function for checking the email.

```html
<Validation Validator="@ValidateEmail">
    <TextEdit Placeholder="Enter email">
        <Feedback>
            <ValidationSuccess>Email is good.</ValidationSuccess>
            <ValidationError>Enter valid email!</ValidationError>
        </Feedback>
    </TextEdit>
</Validation>
@code{
    void ValidateEmail( ValidatorEventArgs e )
    {
        var email = Convert.ToString( e.Value );

        e.Status = string.IsNullOrEmpty( email ) ? ValidationStatus.None :
            email.Contains( "@" ) ? ValidationStatus.Success : ValidationStatus.Error;
    }
}
```

The same structure is for all **Edit** components(check, radio, select, etc). Note that for some components there are some special rules when defining the validation structure. For example for **CheckEdit** you must use `ChildContent` tag along with the `<Feedback>` tag. This is a limitation in Blazor, hopefully it will be fixed in the future.

```html
<Validation Validator="@ValidateCheck">
    <CheckEdit>
        <ChildContent>
            Check me out
        </ChildContent>
        <Feedback>
            <ValidationError>You must check me out!</ValidationError>
        </Feedback>
    </CheckEdit>
</Validation>
```

## Manual validation

Sometimes you don't want to do validation on every input change. In that case you use `<Validations>` component to group multiple validations and then run the validation manually.

### Example

In this example you can see how the `<Validations>` component is used to enclose multiple validation components and the `Mode` attribute is set to Manual. Validation is executed only when clicked on submit button.

```html
<Validations ref="validations" Mode="ValidationMode.Manual">
    <Validation Validator="@ValidateEmail">
        ...
    </Validation>
    <Validation Validator="@ValidatePassword">
        ...
    </Validation>
    <SimpleButton Color="Color.Primary" Clicked="@Submit">Submit</SimpleButton>
</Validation>
@code{
    Validations validations;

    void Submit()
    {
        validations.ValidateAll();
    }
}
```

## Pattern validation

If you want to validate input by using regular expression instead of `Validator` handlers you can use `Pattern` attribute. Components that supports pattern attribute are `TextEdit`, `NumericEdit` and `DateEdit`.

### Example

```html
<Validation UsePattern="true">
    <TextEdit Pattern="[A-Za-z]{3}">
        <Feedback>
            <ValidationError>Pattern does not match!</ValidationError>
        </Feedback>
    </TextEdit>
</Validation>
```

## Validation rules

In Blazorise you can use some of the predefined validation rules. eg

```html
<Validation Validator="@ValidationRule.IsNotEmpty">
```

Here is a list of the validators currently available.

| Validator                    | Description                                                        |
|------------------------------|--------------------------------------------------------------------|
| IsEmpty                      | Check if the string is null or empty.                              |
| IsNotEmpty                   | Check if the string is not null or empty.                          |
| IsEmail                      | Check if the string is an email.                                   |
| IsAlpha                      | Check if the string contains only letters (a-zA-Z).                |
| IsAlphanumeric               | Check if the string contains only letters and numbers.             |
| IsAlphanumericWithUnderscore | Check if the string contains only letters, numbers and underscore. |
| IsUppercase                  | Check if the string is uppercase.                                  |
| IsLowercase                  | Check if the string is lowercase.                                  |