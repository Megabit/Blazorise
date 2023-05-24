namespace Blazorise.Components;

/// <summary>
/// Provides all the information about the <see cref="Autocomplete{TItem, TValue}.Closed"/> event in <see cref="Autocomplete{TItem, TValue}"/>.
/// </summary>
public class AutocompleteClosedEventArgs
{
    /// <summary>
    /// Initializes a new instance of closed event argument.
    /// </summary>
    /// <param name="closeReason">The reason why the Autocomplete was closed.</param>
    public AutocompleteClosedEventArgs( CloseReason closeReason )
    {
        CloseReason = closeReason;
    }

    /// <summary>
    /// Gets the reason why the <see cref="Autocomplete{TItem, TValue}"/> closed.
    /// </summary>
    public CloseReason CloseReason { get; }
}
