namespace Blazorise.DataGrid;

internal sealed class DataGridExpandItemInfo<TItem>
{
    public DataGridExpandItemInfo( TItem item, int level, bool expandable, bool expanded )
    {
        Item = item;
        Level = level;
        Expandable = expandable;
        Expanded = expanded;
    }

    public TItem Item { get; }

    public int Level { get; }

    public bool Expandable { get; }

    public bool Expanded { get; }
}