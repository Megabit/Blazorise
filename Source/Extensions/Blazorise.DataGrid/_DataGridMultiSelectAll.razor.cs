#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

#endregion Using directives

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridMultiSelectAll<TItem> : ComponentBase
    {
        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public EventCallback<bool> MultiSelectAll { get; set; }

        internal bool IsChecked { get; set; }

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

    }
}