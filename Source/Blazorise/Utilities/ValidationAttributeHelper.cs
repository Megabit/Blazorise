#region Using directives
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Helper methods for the validation process.
/// </summary>
public static class ValidationAttributeHelper
{
    /// <summary>
    /// Original placeholder prefix.
    /// </summary>
    public const string PlaceholderPrefix = "{";

    /// <summary>
    /// Original placeholder suffix.
    /// </summary>
    public const string PlaceholderSuffix = "}";

    /// <summary>
    /// Replaced placeholder prefix.
    /// </summary>
    public const string TempPlaceholderPrefix = "[[[";

    /// <summary>
    /// Replaced placeholder suffix.
    /// </summary>
    public const string TempPlaceholderSuffix = "]]]";

    /// <summary>
    /// Sets the default error message on a <see cref="ValidationAttribute"/> by replacing it with
    /// modified ErrorMessageString.
    /// </summary>
    /// <param name="validationAttribute">Validation attributes to modify.</param>
    public static void SetDefaultErrorMessage( ValidationAttribute validationAttribute )
    {
        if ( validationAttribute is StringLengthAttribute stringLengthAttribute && stringLengthAttribute.MinimumLength != 0 )
        {
            var customErrorMessageSet = ValidationAttributeCustomErrorMessageSetProperty.GetValue( validationAttribute ) as bool?;

            if ( customErrorMessageSet != true )
            {
                // This message is used by StringLengthAttribute internally so we need to copy the save behavior here.
                validationAttribute.ErrorMessage = SetErrorMessagePlaceholders( "The field {0} must be a string with a minimum length of {2} and a maximum length of {1}." );
                return;
            }
        }

        // We need to replace placeholders with temporary characters so that Blazor validation will not override
        // our messages.
        validationAttribute.ErrorMessage = SetErrorMessagePlaceholders( ValidationAttributeErrorMessageStringProperty.GetValue( validationAttribute ) as string );
    }

    /// <summary>
    /// Replaces the placeholder characters for string formatting, ie. {0} to [0]
    /// </summary>
    /// <param name="errorMessage">String to be formatted.</param>
    /// <returns>Returns the formatted string.</returns>
    public static string SetErrorMessagePlaceholders( string errorMessage )
    {
        if ( errorMessage is not null )
        {
            errorMessage = errorMessage.Replace( PlaceholderPrefix, TempPlaceholderPrefix );
            errorMessage = errorMessage.Replace( PlaceholderSuffix, TempPlaceholderSuffix );
        }

        return errorMessage;
    }

    /// <summary>
    /// Revert the formatted string into the original with the placeholder characters for string formatting, ie. [0] to {0}
    /// </summary>
    /// <param name="errorMessage">String to be formatted.</param>
    /// <returns>Returns the formatted string.</returns>
    public static string RevertErrorMessagePlaceholders( string errorMessage )
    {
        if ( errorMessage is not null )
        {
            errorMessage = errorMessage.Replace( TempPlaceholderPrefix, PlaceholderPrefix );
            errorMessage = errorMessage.Replace( TempPlaceholderSuffix, PlaceholderSuffix );
        }

        return errorMessage;
    }

    /// <summary>
    /// Gets the internal ErrorMessageString of the ValidationAttribute.
    /// </summary>
    public static readonly PropertyInfo ValidationAttributeErrorMessageStringProperty = typeof( ValidationAttribute )
        .GetProperty( "ErrorMessageString", BindingFlags.Instance | BindingFlags.NonPublic );

    /// <summary>
    /// Gets the internal CustomErrorMessageSet of the ValidationAttribute.
    /// </summary>
    public static readonly PropertyInfo ValidationAttributeCustomErrorMessageSetProperty = typeof( ValidationAttribute )
        .GetProperty( "CustomErrorMessageSet", BindingFlags.Instance | BindingFlags.NonPublic );

    /// <summary>
    /// Gets the list of <see cref="ValidationAttribute"/>s in a given property.
    /// </summary>
    /// <param name="propertyInfo">Property object.</param>
    /// <returns>List of found attributes.</returns>
    public static ValidationAttribute[] GetValidationAttributes( PropertyInfo propertyInfo )
    {
        return propertyInfo
            .GetCustomAttributes<Attribute>()
            .OfType<ValidationAttribute>()
            .ToArray();
    }
}