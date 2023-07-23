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
    private const BindingFlags INTERNAL_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private const string FIELD_STATES = "_fieldStates";
    private const string FIELD_VALIDATION_MESSAGE_STORES = "_validationMessageStores";
    private static Func<EditContext, IDictionary> fieldStatesGetter;
    private static Func<object, HashSet<ValidationMessageStore>> validationMessageStoresGetter;
    private static readonly MethodInfo clearMethodInfo = typeof( HashSet<ValidationMessageStore> ).GetMethod( nameof( HashSet<ValidationMessageStore>.Clear ), INTERNAL_BINDING_FLAGS );

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
        fieldStatesGetter ??= ExpressionCompiler.CreateFieldGetter<IDictionary>( editContext, FIELD_STATES );

        var fieldStates = fieldStatesGetter( editContext );

        foreach ( DictionaryEntry kv in fieldStates )
        {
            if ( kv.Key is FieldIdentifier fieldIdentifier2
                && fieldIdentifier2.FieldName == fieldIdentifier.FieldName )
            {
                validationMessageStoresGetter ??= ExpressionCompiler.CreateFieldGetter<HashSet<ValidationMessageStore>>( kv.Value, FIELD_VALIDATION_MESSAGE_STORES );

                var messageStores = validationMessageStoresGetter( kv.Value );
                clearMethodInfo.Invoke( messageStores, null );
            }
        }

        if ( markAsUnmodified )
            editContext.MarkAsUnmodified();

        if ( revalidate )
            editContext.Validate();
    }


}
