#region Using directives
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components.Forms;
#endregion

namespace Blazorise;

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
    /// <param name="messageLocalizer">Handler for message localizer..</param>
    void ValidateField( EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier, Func<string, IEnumerable<string>, string> messageLocalizer );
}

/// <summary>
/// Default Blazorise implementation of <see cref="IEditContextValidator"/>.
/// </summary>
public class EditContextValidator : IEditContextValidator
{
    #region Members

    /// <summary>
    /// Comparer for message localizer.
    /// </summary>
    private readonly IValidationMessageLocalizerAttributeFinder validationMessageLocalizerAttributeFinder;

    /// <summary>
    /// Cached list of fields for validation.
    /// </summary>
    private readonly ConcurrentDictionary<(Type ModelType, string FieldName), ValidationPropertyInfo> propertyInfoCache = new();

    /// <summary>
    /// Service Provider for validation context.
    /// </summary>
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Helper object to hold all information about validated field.
    /// </summary>
    protected class ValidationPropertyInfo
    {
        /// <summary>
        /// Gets or sets the property data.
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Gets or sets the array of validation attributes.
        /// </summary>
        public ValidationAttribute[] ValidationAttributes { get; set; }

        /// <summary>
        /// Gets or sets the array of formatted validation attributes for the localization.
        /// </summary>
        public ValidationAttribute[] FormattedValidationAttributes { get; set; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="EditContextValidator"/> constructor.
    /// </summary>
    /// <param name="validationMessageLocalizerAttributeFinder">Comparer for message localizer.</param>
    /// <param name="serviceProvider">Service provider for custom validators.</param>
    public EditContextValidator( IValidationMessageLocalizerAttributeFinder validationMessageLocalizerAttributeFinder, IServiceProvider serviceProvider )
    {
        this.validationMessageLocalizerAttributeFinder = validationMessageLocalizerAttributeFinder;
        this.serviceProvider = serviceProvider;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual void ValidateField( EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier, Func<string, IEnumerable<string>, string> messageLocalizer )
    {
        if ( TryGetValidatableProperty( fieldIdentifier, out var validationPropertyInfo, messageLocalizer != null ) )
        {
            var propertyValue = validationPropertyInfo.PropertyInfo.GetValue( fieldIdentifier.Model );
            var validationContext = new ValidationContext( fieldIdentifier.Model, serviceProvider, null )
            {
                MemberName = validationPropertyInfo.PropertyInfo.Name,
            };

            var results = new List<ValidationResult>();

            messages.Clear( fieldIdentifier );

            // Clear any previous message for the given field.
            editContext.ClearValidationMessages( fieldIdentifier );

            if ( messageLocalizer != null )
            {
                // In this case we need to validate by using TryValidateValue because we need
                // to have custom messages on validation attributes
                Validator.TryValidateValue( propertyValue, validationContext, results, validationPropertyInfo.ValidationAttributes );

                // OPTIMIZE THIS: we run two validations because we need to have the formatted
                // and non-formatted error messages in the same order so we can extract message attribute names.
                var formattedResults = new List<ValidationResult>();

                Validator.TryValidateValue( propertyValue, validationContext, formattedResults, validationPropertyInfo.FormattedValidationAttributes );

                // We will assume that both validation will return the same number of errors
                // and that they will be in the same order.
                for ( int i = 0; i < results.Count; ++i )
                {
                    var errorMessage = results[i].ErrorMessage;
                    var errorMessageString = ValidationAttributeHelper.RevertErrorMessagePlaceholders( formattedResults[i].ErrorMessage );

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

                // We don't know what fields user can validate in the Model. So we need to run the IValidatableObject.Validate every time
                // and then check if any of the validated fields matches the current field name.
                if ( editContext.Model is IValidatableObject validatableObject )
                {
                    var validateResult = validatableObject.Validate( validationContext );

                    if ( validateResult is not null && validateResult.Any( x => x.MemberNames.Contains( validationContext.MemberName ) ) )
                    {
                        messages.Add( fieldIdentifier, validateResult.Where( x => x.MemberNames.Contains( validationContext.MemberName ) ).Select( x => x.ErrorMessage ) );
                    }
                }
            }

            // We have to notify even if there were no messages before and are still no messages now,
            // because the "state" that changed might be the completion of some async validation task
            editContext.NotifyValidationStateChanged();
        }
    }

    /// <summary>
    /// Gets the <see cref="ValidationPropertyInfo"/> for a given <see cref="FieldIdentifier"/>.
    /// </summary>
    /// <param name="fieldIdentifier">Field identifier.</param>
    /// <param name="validationPropertyInfo">When this method returns it will return the information for the found property.</param>
    /// <param name="forLocalization">True if method should also handle localization.</param>
    /// <returns>True if property is found.</returns>
    protected virtual bool TryGetValidatableProperty( in FieldIdentifier fieldIdentifier, out ValidationPropertyInfo validationPropertyInfo, bool forLocalization )
    {
        validationPropertyInfo = null;
        if ( fieldIdentifier.FieldName is not null )
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
                var formattedValidationAttributes = ValidationAttributeHelper.GetValidationAttributes( propertyInfo );

                if ( forLocalization )
                {
                    foreach ( var validationAttribute in formattedValidationAttributes )
                    {
                        // In case the ErrorMessageResourceName is set, validation will fail if the ErrorMessage
                        // is also set.
                        // In case a custom ErrorMessage in the DataAnnotation like [Required(ErrorMessage="{0} is very important"]
                        // the ErrorMessage is not initialized with null.
                        if ( validationAttribute.ErrorMessageResourceName is null )
                        {
                            ValidationAttributeHelper.SetDefaultErrorMessage( validationAttribute );
                        }
                    }
                }

                validationPropertyInfo = new()
                {
                    PropertyInfo = propertyInfo,
                    ValidationAttributes = validationAttributes,
                    FormattedValidationAttributes = formattedValidationAttributes,
                };

                // No need to lock, because it doesn't matter if we write the same value twice
                propertyInfoCache[cacheKey] = validationPropertyInfo;
            }
        }

        return validationPropertyInfo != null;
    }

    #endregion
}