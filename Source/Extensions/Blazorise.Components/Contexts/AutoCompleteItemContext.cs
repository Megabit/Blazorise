namespace Blazorise.Components.Autocomplete;

/// <summary>
/// Holds the ItemContext for the current Autocomplete item.
/// </summary>
/// <typeparam name="TItem">Type of an item filtered by the autocomplete component.</typeparam>
/// <typeparam name="TValue">Type of a SelectedValue field by the autocomplete component.</typeparam>
public class ItemContext<TItem, TValue> : BaseTemplateContext<TItem>
{
    /// <summary>
    /// Initializes ItemContext.
    /// </summary>
    /// <param name="item">Holds current Autocomplete Item Data.</param>
    /// <param name="value">Holds current Autocomplete Item Value.</param>
    /// <param name="text">Holds current Autocomplete Item Text.</param>
    public ItemContext( TItem item, TValue value, string text ) : base( item )
    {
        Value = value;
        Text = text;
    }

    /// <summary>
    /// Holds current Autocomplete Item Value.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Holds current Autocomplete Item Text.
    /// </summary>
    public string Text { get; }
}
