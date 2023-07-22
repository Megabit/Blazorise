#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Components.Forms;
#endregion

namespace Blazorise.Extensions;

/// <summary>
/// Contains extension methods for working with the <see cref="EditForm"/> class.
/// </summary>
internal static class EditContextExtensions
{
    /// <summary>
    /// Clears all validation messages from the <see cref="EditContext"/> of the given <see cref="FieldIdentifier"/>.
    /// </summary>
    /// <param name="editContext">The <see cref="EditContext"/> to use.</param>
    /// <param name="fieldIdentifier">The field for which to clear messages.</param>
    /// <param name="revalidate">
    /// Specifies whether the <see cref="EditContext"/> should revalidate after all validation messages have been cleared.
    /// </param>
    /// <param name="markAsUnmodified">
    /// Specifies whether the <see cref="EditContext"/> should be marked as unmodified. This will affect the assignment of css classes to a form's input controls in Blazor.
    /// </param>
    public static void ClearValidationMessages( this EditContext editContext, FieldIdentifier fieldIdentifier, bool revalidate = false, bool markAsUnmodified = false )
    {
        const BindingFlags InternalBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        object GetInstanceField( Type type, object instance, string fieldName )
        {
            var fieldInfo = type.GetField( fieldName, InternalBindingFlags );
            return fieldInfo.GetValue( instance );
        }

        var fieldStates = GetInstanceField( typeof( EditContext ), editContext, "_fieldStates" );
        var clearMethodInfo = typeof( HashSet<ValidationMessageStore> ).GetMethod( "Clear", InternalBindingFlags );

        foreach ( DictionaryEntry kv in (IDictionary)fieldStates )
        {
            if ( kv.Key is FieldIdentifier fieldIdentifier2
                && fieldIdentifier2.FieldName == fieldIdentifier.FieldName )
            {
                var messageStores = GetInstanceField( kv.Value.GetType(), kv.Value, "_validationMessageStores" );
                clearMethodInfo.Invoke( messageStores, null );
            }
        }

        if ( markAsUnmodified )
            editContext.MarkAsUnmodified();

        if ( revalidate )
            editContext.Validate();
    }
}
