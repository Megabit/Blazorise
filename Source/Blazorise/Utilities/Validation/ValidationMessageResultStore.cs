#region Using directives
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Forms;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Holds the list of validation message results and their arguments.
/// </summary>
public class ValidationMessageResultStore
{
    private readonly Dictionary<FieldIdentifier, List<ValidationMessageResult>> messages = new();

    /// <summary>
    /// Clears the messages for the supplied <see cref="FieldIdentifier"/>.
    /// </summary>
    /// <param name="fieldIdentifier">Field identifier to which messages belong.</param>
    public void Clear( FieldIdentifier fieldIdentifier )
        => messages.Remove( fieldIdentifier );

    /// <summary>
    /// Adds the message and apply it to the field identifier.
    /// </summary>
    /// <param name="fieldIdentifier">Field identifier to which messages belong.</param>
    /// <param name="message">Message result.</param>
    public void Add( FieldIdentifier fieldIdentifier, ValidationMessageResult message )
        => GetOrCreateMessagesListForField( fieldIdentifier ).Add( message );

    /// <summary>
    /// Adds the list of messages and apply it to the field identifier.
    /// </summary>
    /// <param name="fieldIdentifier">Field identifier to which messages belong.</param>
    /// <param name="messages"></param>
    public void Add( FieldIdentifier fieldIdentifier, IEnumerable<ValidationMessageResult> messages )
        => GetOrCreateMessagesListForField( fieldIdentifier ).AddRange( messages );

    private List<ValidationMessageResult> GetOrCreateMessagesListForField( in FieldIdentifier fieldIdentifier )
    {
        if ( !messages.TryGetValue( fieldIdentifier, out var messagesForField ) )
        {
            messagesForField = new();
            messages.Add( fieldIdentifier, messagesForField );
        }

        return messagesForField;
    }

    /// <summary>
    /// Gets the list of messages for the supplied field identifier.
    /// </summary>
    /// <param name="fieldIdentifier">Field identifier to which messages belong.</param>
    /// <returns>Return the list of messages.</returns>
    public IEnumerable<ValidationMessageResult> this[FieldIdentifier fieldIdentifier]
        => messages.TryGetValue( fieldIdentifier, out var results ) ? results : Enumerable.Empty<ValidationMessageResult>();
}

/// <summary>
/// Holds the validation message data.
/// </summary>
public struct ValidationMessageResult
{
    /// <summary>
    /// Default validation message constructor.
    /// </summary>
    /// <param name="message">Validation message.</param>
    /// <param name="messageArguments">List of arguments(if any) for manual <see cref="string.Format(string, object[])"/></param>
    /// <param name="memberNames"></param>
    public ValidationMessageResult( string message, string[] messageArguments, string[] memberNames )
    {
        Message = message;
        MessageArguments = messageArguments;
        MemberNames = memberNames;
    }

    /// <summary>
    /// Gets the validation message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the list of arguments that needs to be placed inside of <see cref="Message"/> when formatted.
    /// </summary>
    public string[] MessageArguments { get; }

    /// <summary>
    /// Gets the collection of member names that indicate which fields have validation errors.
    /// </summary>
    public string[] MemberNames { get; }
}