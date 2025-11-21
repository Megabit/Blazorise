namespace Blazorise.DataGrid;

public class DataGridCheckColumn<TItem> : DataGridColumn<TItem>
{
    /// <inheritdoc/>
    internal override DataGridColumnFilterMethod GetDefaultFilterMethod()
        => DataGridColumnFilterMethod.Equals;

    /// <inheritdoc/>
    public override DataGridColumnType ColumnType
        => DataGridColumnType.Check;
}