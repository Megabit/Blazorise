#region Using directives
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
#endregion

namespace Blazorise.Utilities
{
    public static class ValidationAttributeHelper
    {
        public const string PlaceholderPrefix = "{";
        public const string PlaceholderSuffix = "}";

        public const string TempPlaceholderPrefix = "[[[";
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
        /// Replaces the placeholder characters for string formating, ie. {0} to [0]
        /// </summary>
        /// <param name="errorMessage">String to be formated.</param>
        /// <returns>Returns the formated string.</returns>
        public static string SetErrorMessagePlaceholders( string errorMessage )
        {
            if ( errorMessage != null )
            {
                errorMessage = errorMessage.Replace( PlaceholderPrefix, TempPlaceholderPrefix );
                errorMessage = errorMessage.Replace( PlaceholderSuffix, TempPlaceholderSuffix );
            }

            return errorMessage;
        }

        /// <summary>
        /// Revert the formated string into the original with the placeholder characters for string formating, ie. [0] to {0}
        /// </summary>
        /// <param name="errorMessage">String to be formated.</param>
        /// <returns>Returns the formated string.</returns>
        public static string RevertErrorMessagePlaceholders( string errorMessage )
        {
            if ( errorMessage != null )
            {
                errorMessage = errorMessage.Replace( TempPlaceholderPrefix, PlaceholderPrefix );
                errorMessage = errorMessage.Replace( TempPlaceholderSuffix, PlaceholderSuffix );
            }

            return errorMessage;
        }

        public static readonly PropertyInfo ValidationAttributeErrorMessageStringProperty = typeof( ValidationAttribute )
            .GetProperty( "ErrorMessageString", BindingFlags.Instance | BindingFlags.NonPublic );

        public static readonly PropertyInfo ValidationAttributeCustomErrorMessageSetProperty = typeof( ValidationAttribute )
            .GetProperty( "CustomErrorMessageSet", BindingFlags.Instance | BindingFlags.NonPublic );

        public static ValidationAttribute[] GetValidationAttributes( PropertyInfo propertyInfo )
        {
            return propertyInfo
                .GetCustomAttributes<Attribute>()
                .OfType<ValidationAttribute>()
                .ToArray();
        }
    }
}
