#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Internal;

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
    /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    [Parameter] public bool IsIndeterminate { get; set; }

    [Parameter] public bool IsChecked { get; set; }

    #endregion
}