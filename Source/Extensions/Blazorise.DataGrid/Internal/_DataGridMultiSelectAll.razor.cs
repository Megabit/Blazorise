#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Internal;

/// <summary>
/// Supports base data grid multi select all behavior in DataGrid components.
/// </summary>
public abstract class _BaseDataGridMultiSelectAll<TItem> : ComponentBase
{
    #region Methods

    internal Task IsCheckedChanged( bool e )
    {
        IsChecked = e;

        return ParentDataGrid.OnMultiSelectAll( IsChecked );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    /// <summary>
    /// Shows a mixed state when only some rows are selected.
    /// </summary>
    [Parameter] public bool IsIndeterminate { get; set; }

    /// <summary>
    /// Indicates whether all eligible rows are selected.
    /// </summary>
    [Parameter] public bool IsChecked { get; set; }

    #endregion
}