#region Using directives
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
            var hasSelectedRows = ParentDataGrid.SelectedRows?.Any() ?? false;

            if ( hasSelectedRows )
            {
                var hasData = ParentDataGrid.DisplayData.Any();
                var unselectedRows = ParentDataGrid.DisplayData.Except( ParentDataGrid.SelectedRows ).Any();

                if ( hasSelectedRows && !unselectedRows && hasData )
                    IsChecked = true;

                if ( !hasSelectedRows || unselectedRows )
                    IsChecked = false;
            }

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