using System.Collections.Generic;

namespace Blazorise.DataGrid;

internal sealed class DataGridHierarchyNodeState<TItem>
{
    public DataGridHierarchyNodeState( TItem item )
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