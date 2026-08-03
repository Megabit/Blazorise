namespace Blazorise.DataGrid;

/// <summary>
/// Provides data grid check column behavior within a DataGrid.
/// </summary>
public class DataGridCheckColumn<TItem> : DataGridColumn<TItem>
{
    /// <inheritdoc/>
    internal override DataGridColumnFilterMethod GetDefaultFilterMethod()
        => DataGridColumnFilterMethod.Equals;

    /// <inheritdoc/>
    public override DataGridColumnType ColumnType
        => DataGridColumnType.Check;
}