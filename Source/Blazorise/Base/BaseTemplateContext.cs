namespace Blazorise;

/// <summary>
/// Holds the template context for an Item based component.
/// </summary>
/// <typeparam name="TItem">Type of an item.</typeparam>
public abstract class BaseTemplateContext<TItem>
{
    /// <summary>
    /// Initializes BaseContext.
    /// </summary>
    /// <param name="item">Holds the Item.</param>
    public BaseTemplateContext( TItem item )
    {
        Item = item;
    }

    /// <summary>
    /// The contextual Item.
    /// </summary>
    public TItem Item { get; }
}