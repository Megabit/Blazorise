namespace Blazorise.DataGrid;

/// <summary>
/// Defines the Edit State for a Batch Edit Item.
/// </summary>
public enum DataGridBatchEditItemState
{
    /// <summary>
    /// This is a new item.
    /// </summary>
    New = 0,

    /// <summary>
    /// This is an existing item with changes.
    /// </summary>
    Edit = 1,

    /// <summary>
    /// This is a deleted item.
    /// </summary>
    Delete = 2
}

