#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridMultiSelectAll<TItem> : ComponentBase
    {
        #region Methods

        internal Task IsCheckedChanged( bool e )
        {
            IsChecked = e;
            return MultiSelectAll.InvokeAsync( e );
        }

        protected override Task OnParametersSetAsync()
        {
            //IsChecked = ( ParentDataGrid.PageSize == ParentDataGrid.SelectedRows.Count );
            return base.OnParametersSetAsync();
        }

        #endregion

        #region Properties

        internal bool IsChecked { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public EventCallback<bool> MultiSelectAll { get; set; }

        #endregion
    }
}