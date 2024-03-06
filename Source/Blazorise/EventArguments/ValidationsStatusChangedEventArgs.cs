#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Provides data for the <see cref="Validations.StatusChanged"/> event.
/// </summary>
public class ValidationsStatusChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the default <see cref="ValidationsStatusChangedEventArgs"/>.
    /// </summary>
    public static new readonly ValidationsStatusChangedEventArgs Empty = new( ValidationStatus.None, null, null );

    /// <summary>
    /// A default <see cref="ValidationsStatusChangedEventArgs"/> constructor.
    /// </summary>
    public ValidationsStatusChangedEventArgs( ValidationStatus status, IReadOnlyCollection<string> messages, IValidation validation )
    {
        Status = status;
        Messages = messages;
    }

    /// <summary>
    /// Gets the validation result.
    /// </summary>
    public ValidationStatus Status { get; set; }

    /// <summary>
    /// Gets the custom validation message.
    /// </summary>
    public IReadOnlyCollection<string> Messages { get; }

    /// <summary>
    /// Gets the <see cref="IValidation"/> reference that initiated status changed event. If <c>null</c>, it means the 
    /// raise happened from the parent <see cref="Validations"/> component.
    /// </summary>
    public IValidation Validation { get; }
}