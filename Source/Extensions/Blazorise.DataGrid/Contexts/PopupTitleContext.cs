namespace Blazorise.DataGrid;

/// <summary>
/// Carries popup title state and callbacks.
/// </summary>
public class PopupTitleContext<TItem>
{
    /// <summary>
    /// Creates a popup title context instance.
    /// </summary>
    public PopupTitleContext( TItem item, DataGridEditState editState, string localizationString )
    {
        Item = item;
        EditState = editState;
        LocalizationString = localizationString;
    }

    /// <summary>
    /// Gets the model.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets the edit state of data grid.
    /// </summary>
    public DataGridEditState EditState { get; }

    /// <summary>
    /// Gets the localized text for modal header.
    /// </summary>
    public string LocalizationString { get; }
}