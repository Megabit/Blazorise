using System.Collections.Generic;

namespace Blazorise.DataGrid;

internal sealed class DataGridExpandNodeState<TItem>
{
    public DataGridExpandNodeState( TItem item )
    {
        Item = item;
    }

    public TItem Item { get; }

    public bool Expanded { get; set; }

    public bool Expandable { get; set; }

    public bool ExpandableResolved { get; set; }

    public bool ChildrenLoaded { get; set; }

    public List<TItem> Children { get; set; } = new();
}