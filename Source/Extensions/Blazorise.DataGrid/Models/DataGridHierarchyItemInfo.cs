namespace Blazorise.DataGrid;

internal sealed class DataGridHierarchyItemInfo<TItem>
{
    public DataGridHierarchyItemInfo( TItem item, int level, bool expandable, bool expanded )
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