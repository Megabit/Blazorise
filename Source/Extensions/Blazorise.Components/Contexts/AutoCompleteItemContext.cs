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
    public ItemContext( TItem item, TValue value, string text )
        : this( item, value, text, null, false, false, false, false )
    {
    }

    /// <summary>
    /// Initializes ItemContext.
    /// </summary>
    /// <param name="item">Holds current Autocomplete Item Data.</param>
    /// <param name="value">Holds current Autocomplete Item Value.</param>
    /// <param name="text">Holds current Autocomplete Item Text.</param>
    /// <param name="index">Holds the current item index.</param>
    /// <param name="active">Indicates if the item is selected.</param>
    /// <param name="focused">Indicates if the item is focused.</param>
    /// <param name="disabled">Indicates if the item is disabled.</param>
    /// <param name="checkbox">Indicates if the item is rendered with a checkbox.</param>
    public ItemContext( TItem item, TValue value, string text, int? index, bool active, bool focused, bool disabled, bool checkbox )
        : base( item )
    {
        Value = value;
        Text = text;
        Index = index;
        Active = active;
        Focused = focused;
        Disabled = disabled;
        Checkbox = checkbox;
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
    /// Holds the current item index.
    /// </summary>
    public int? Index { get; }

    /// <summary>
    /// Indicates if the item is selected.
    /// </summary>
    public bool Active { get; }

    /// <summary>
    /// Indicates if the item is focused.
    /// </summary>
    public bool Focused { get; }

    /// <summary>
    /// Indicates if the item is disabled.
    /// </summary>
    public bool Disabled { get; }

    /// <summary>
    /// Indicates if the item is rendered with a checkbox.
    /// </summary>
    public bool Checkbox { get; }
}