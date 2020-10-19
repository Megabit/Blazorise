#region Using directives
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Blazorise.Utils;
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
        void ValidateField( EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier, bool forLocalization );
    }

    public class EditContextValidator : IEditContextValidator
    {
        #region Members        

        protected readonly ConcurrentDictionary<(Type ModelType, string FieldName), ValidationPropertyInfo> propertyInfoCache
           = new ConcurrentDictionary<(Type, string), ValidationPropertyInfo>();

        protected class ValidationPropertyInfo
        {
            public PropertyInfo PropertyInfo { get; set; }

            public ValidationAttribute[] ValidationAttributes { get; set; }
        }

        #endregion

        #region Methods

        public virtual void ValidateField( EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier, bool forLocalization )
        {
            if ( TryGetValidatableProperty( fieldIdentifier, out var validationPropertyInfo ) )
            {
                var propertyValue = validationPropertyInfo.PropertyInfo.GetValue( fieldIdentifier.Model );
                var validationContext = new ValidationContext( fieldIdentifier.Model )
                {
                    MemberName = validationPropertyInfo.PropertyInfo.Name,
                };

                var results = new List<ValidationResult>();

                messages.Clear( fieldIdentifier );

                if ( forLocalization )
                {
                    // In this case we need to validate by using TryValidateValue because we need 
                    // to have custom messages on validation attributes
                    Validator.TryValidateValue( propertyValue, validationContext, results, validationPropertyInfo.ValidationAttributes );

                    messages.Add( fieldIdentifier, results.Select( result => ValidationAttributeHelper.RevertErrorMessagePlaceholders( result.ErrorMessage ) ) );
                }
                else
                {
                    Validator.TryValidateProperty( propertyValue, validationContext, results );

                    messages.Add( fieldIdentifier, results.Select( result => result.ErrorMessage ) );
                }

                // We have to notify even if there were no messages before and are still no messages now,
                // because the "state" that changed might be the completion of some async validation task
                editContext.NotifyValidationStateChanged();
            }
        }

        protected virtual bool TryGetValidatableProperty( in FieldIdentifier fieldIdentifier, out ValidationPropertyInfo validationPropertyInfo )
        {
            var cacheKey = (ModelType: fieldIdentifier.Model.GetType(), fieldIdentifier.FieldName);

            if ( !propertyInfoCache.TryGetValue( cacheKey, out validationPropertyInfo ) )
            {
                // DataAnnotations only validates public properties, so that's all we'll look for
                // If we can't find it, cache 'null' so we don't have to try again next time
                var propertyInfo = cacheKey.ModelType.GetProperty( cacheKey.FieldName );

                // This is used only for custom localization. We assume that unformated ErrorMessage will be
                // used as an localization key, so we need to replace it in case it is undefined with
                // the internal ErrorMessageString that has unformated message. eg. "The field {0} is invalid."
                var validationAttributes = ValidationAttributeHelper.GetValidationAttributes( propertyInfo );

                foreach ( var validationAttribute in validationAttributes )
                {
                    if ( validationAttribute.ErrorMessage == null )
                    {
                        ValidationAttributeHelper.SetDefaultErrorMessage( validationAttribute );
                    }
                }

                validationPropertyInfo = new ValidationPropertyInfo
                {
                    PropertyInfo = propertyInfo,
                    ValidationAttributes = validationAttributes,
                };

                // No need to lock, because it doesn't matter if we write the same value twice
                propertyInfoCache[cacheKey] = validationPropertyInfo;
            }

            return validationPropertyInfo != null;
        }
    }

    #endregion
}