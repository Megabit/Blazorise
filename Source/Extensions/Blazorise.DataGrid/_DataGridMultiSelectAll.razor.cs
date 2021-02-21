﻿#region Using directives
using System.Linq;
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

            return MultiSelectAll.InvokeAsync( IsChecked );
        }

        protected override Task OnParametersSetAsync()
        {
            if ( ParentDataGrid.PageSize == ParentDataGrid.SelectedRows?.Count )
                IsChecked = true;

            if ( ParentDataGrid.SelectedRows?.Count == 0 )
                IsChecked = false;

            return base.OnParametersSetAsync();
        }

        #endregion

        #region Properties

        internal bool IsChecked { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public bool IsIndeterminate { get; set; }

        [Parameter] public EventCallback<bool> MultiSelectAll { get; set; }

        #endregion
    }
}