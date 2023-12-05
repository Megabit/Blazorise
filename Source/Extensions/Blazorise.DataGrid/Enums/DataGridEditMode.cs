namespace Blazorise.DataGrid;

/// <summary>
/// Specifes the grid editing modes.
/// </summary>
public enum DataGridEditMode
{
    /// <summary>
    /// Values will be edited in the edit form.
    /// </summary>
    Form,

    /// <summary>
    /// Values will be edited within the inline edit row.
    /// </summary>
    Inline,

    /// <summary>
    /// Values will be edited in the modal dialog.
    /// </summary>
    Popup,

    /// <summary>
    /// Any cell value can be edited directly allowing for rapid editing of data.
    /// </summary>
    Cell
}