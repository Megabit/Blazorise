using Microsoft.AspNetCore.Components;

namespace Blazorise.Components.Autocomplete;

/// <summary>
/// Holds the Tag Context for the current Autocomplete item.
/// </summary>
public class AutocompleteTagContext<TItem, TValue> : BaseTemplateContext<TItem>
{
    /// <summary>
    /// Initializes AutocompleteTagContext.
    /// </summary>
    /// <param name="item">Holds current Autocomplete Item Data.</param>
    /// <param name="value">Holds current Autocomplete Item Value.</param>
    /// <param name="text">Holds current Autocomplete Item Text.</param>
    /// <param name="removeCallback">Removes Item from selection callback.</param>
    public AutocompleteTagContext( TItem item, TValue value, string text, EventCallback removeCallback ) : base( item )
    {
        Value = value;
        Text = text;
        Remove = removeCallback;
    }

    /// <summary>
    /// Holds current Autocomplete Item Value.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Holds current Autocomplete Item Text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Removes Item from selection.
    /// </summary>
    public EventCallback Remove { get; set; }
}