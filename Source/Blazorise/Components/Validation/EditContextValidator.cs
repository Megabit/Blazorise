#region Using directives
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components.Forms;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Validator for <see cref="EditContext"/> that can be used by the third-party applications
    /// to override default data-annotation validation logic. It can be used for example to provide 
    /// custom localization for each field.
    /// </summary>
    public interface IEditContextValidator
    {
        /// <summary>
        /// Validate the field based on the edit context.
        /// </summary>
        /// <param name="editContext">Edit context</param>
        /// <param name="messages">Holds the list of error messages if any error is found.</param>
        /// <param name="fieldIdentifier">Identifies the field for validation.</param>
        /// <param name="forLocalization">If true, error messages will be returned as raw messages that needs to be manually localized.</param>
        void ValidateField( EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier, Func<string, IEnumerable<string>, string> messageLocalizer );
    }

    public class EditContextValidator : IEditContextValidator
    {
        #region Members       

        protected readonly IValidationMessageLocalizerAttributeFinder validationMessageLocalizerAttributeFinder;

        protected readonly ConcurrentDictionary<(Type ModelType, string FieldName), ValidationPropertyInfo> propertyInfoCache
           = new ConcurrentDictionary<(Type, string), ValidationPropertyInfo>();

        protected class ValidationPropertyInfo
        {
            public PropertyInfo PropertyInfo { get; set; }

            public ValidationAttribute[] ValidationAttributes { get; set; }

            public ValidationAttribute[] FormatedValidationAttributes { get; set; }
        }

        #endregion

        #region Constructors

        public EditContextValidator( IValidationMessageLocalizerAttributeFinder validationMessageLocalizerAttributeFinder )
        {
            this.validationMessageLocalizerAttributeFinder = validationMessageLocalizerAttributeFinder;
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual void ValidateField( EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier, Func<string, IEnumerable<string>, string> messageLocalizer )
        {
            if ( TryGetValidatableProperty( fieldIdentifier, out var validationPropertyInfo, messageLocalizer != null ) )
            {
                var propertyValue = validationPropertyInfo.PropertyInfo.GetValue( fieldIdentifier.Model );
                var validationContext = new ValidationContext( fieldIdentifier.Model )
                {
                    MemberName = validationPropertyInfo.PropertyInfo.Name,
                };

                var results = new List<ValidationResult>();

                messages.Clear( fieldIdentifier );

                if ( messageLocalizer != null )
                {
                    // In this case we need to validate by using TryValidateValue because we need 
                    // to have custom messages on validation attributes
                    Validator.TryValidateValue( propertyValue, validationContext, results, validationPropertyInfo.ValidationAttributes );

                    // OPTIMIZE THIS: we run two validations because we need to have the formatted 
                    // and non-formatted error messages in the same order so we can extract message attribute names.
                    var formatedResults = new List<ValidationResult>();

                    Validator.TryValidateValue( propertyValue, validationContext, formatedResults, validationPropertyInfo.FormatedValidationAttributes );

                    // We will assume that both validation will return the same number of errors 
                    // and that they will be in the same order.
                    for ( int i = 0; i < results.Count; ++i )
                    {
                        var errorMessage = results[i].ErrorMessage;
                        var errorMessageString = ValidationAttributeHelper.RevertErrorMessagePlaceholders( formatedResults[i].ErrorMessage );

                        // Compare both error messages and find the differences. This should later be used
                        // for manual formatting by the library users.
                        var errorMessageArguments = validationMessageLocalizerAttributeFinder.FindAll( errorMessage, errorMessageString )
                            ?.OrderBy( x => x.Index ) // sort arguments by index in the message eg, {0}, {1}, {2}
                            ?.Select( x => x.Argument );

                        var localizedErrorMessage = messageLocalizer.Invoke( errorMessageString, errorMessageArguments );

                        messages.Add( fieldIdentifier, localizedErrorMessage );
                    }
                }
                else
                {
                    Validator.TryValidateProperty( propertyValue, validationContext, results );

                    messages.Add( fieldIdentifier, results.Select( x => x.ErrorMessage ) );
                }

                // We have to notify even if there were no messages before and are still no messages now,
                // because the "state" that changed might be the completion of some async validation task
                editContext.NotifyValidationStateChanged();
            }
        }

        protected virtual bool TryGetValidatableProperty( in FieldIdentifier fieldIdentifier, out ValidationPropertyInfo validationPropertyInfo, bool forLocalization )
        {
            var cacheKey = (ModelType: fieldIdentifier.Model.GetType(), fieldIdentifier.FieldName);

            if ( !propertyInfoCache.TryGetValue( cacheKey, out validationPropertyInfo ) )
            {
                // DataAnnotations only validates public properties, so that's all we'll look for
                // If we can't find it, cache 'null' so we don't have to try again next time
                var propertyInfo = cacheKey.ModelType.GetProperty( cacheKey.FieldName );

                // This is used only for custom localization. We assume that unformatted ErrorMessage will be
                // used as an localization key, so we need to replace it in case it is undefined with
                // the internal ErrorMessageString that has unformatted message. eg. "The field {0} is invalid."
                var validationAttributes = ValidationAttributeHelper.GetValidationAttributes( propertyInfo );
                var formatedValidationAttributes = ValidationAttributeHelper.GetValidationAttributes( propertyInfo );

                if ( forLocalization )
                {
                    foreach ( var validationAttribute in formatedValidationAttributes )
                    {
                        // In case the ErrorMessageResourceName is set, validation will fail if the ErrorMessage
                        // is also set.
                        // In case a custom ErrorMessage in the DataAnnotation like [Required(ErrorMessage="{0} is very important"]
                        // the ErrorMessage is not initialized with null.
                        if ( validationAttribute.ErrorMessageResourceName == null )
                        {
                            ValidationAttributeHelper.SetDefaultErrorMessage( validationAttribute );
                        }
                    }
                }

                validationPropertyInfo = new ValidationPropertyInfo
                {
                    PropertyInfo = propertyInfo,
                    ValidationAttributes = validationAttributes,
                    FormatedValidationAttributes = formatedValidationAttributes,
                };

                // No need to lock, because it doesn't matter if we write the same value twice
                propertyInfoCache[cacheKey] = validationPropertyInfo;
            }

            return validationPropertyInfo != null;
        }

        #endregion
    }
}