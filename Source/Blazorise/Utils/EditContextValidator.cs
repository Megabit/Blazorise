#region Using directives
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Components.Forms;
#endregion

namespace Blazorise.Utils
{
    internal static class EditContextValidatorExtensions
    {
        private static ConcurrentDictionary<(Type ModelType, string FieldName), PropertyInfo> propertyInfoCache
            = new ConcurrentDictionary<(Type, string), PropertyInfo>();

        internal static void ValidateField( this EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier )
        {
            if ( TryGetValidatableProperty( fieldIdentifier, out var propertyInfo ) )
            {
                var propertyValue = propertyInfo.GetValue( fieldIdentifier.Model );
                var validationContext = new ValidationContext( fieldIdentifier.Model )
                {
                    MemberName = propertyInfo.Name
                };
                var results = new List<ValidationResult>();

                Validator.TryValidateProperty( propertyValue, validationContext, results );
                messages.Clear( fieldIdentifier );
                messages.Add( fieldIdentifier, results.Select( result => result.ErrorMessage ) );

                // We have to notify even if there were no messages before and are still no messages now,
                // because the "state" that changed might be the completion of some async validation task
                editContext.NotifyValidationStateChanged();
            }
        }

        private static bool TryGetValidatableProperty( in FieldIdentifier fieldIdentifier, out PropertyInfo propertyInfo )
        {
            var cacheKey = (ModelType: fieldIdentifier.Model.GetType(), fieldIdentifier.FieldName);

            if ( !propertyInfoCache.TryGetValue( cacheKey, out propertyInfo ) )
            {
                // DataAnnotations only validates public properties, so that's all we'll look for
                // If we can't find it, cache 'null' so we don't have to try again next time
                propertyInfo = cacheKey.ModelType.GetProperty( cacheKey.FieldName );

                // No need to lock, because it doesn't matter if we write the same value twice
                propertyInfoCache[cacheKey] = propertyInfo;
            }

            return propertyInfo != null;
        }
    }
}
