namespace Blazorise.Components.ListView;

/// <summary>
/// Holds the ItemContext for the current ListView item.
/// </summary>
/// <typeparam name="TItem">Type of an item.</typeparam>
public class ItemContext<TItem> : BaseTemplateContext<TItem>
{
    public ItemContext( TItem item ) : base( item )
    {

    }

    public ItemContext( TItem item, string text, string value ) : base( item )
    {
        Text = text;
        Value = value;
    }

    /// <summary>
    /// Holds current ListView Item Text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Holds current ListView Item Value.
    /// </summary>
    public string Value { get; }
}