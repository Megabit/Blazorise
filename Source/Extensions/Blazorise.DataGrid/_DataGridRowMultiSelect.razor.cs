#region Using directives
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridRowMultiSelect<TItem> : ComponentBase
    {
        #region Methods

        internal Task IsCheckedChanged( bool e )
        {
            IsChecked = e;
            return OnSelectedChanged.InvokeAsync( e );
        }

        protected override async Task OnParametersSetAsync()
        {
            if ( ParentDataGrid.SelectedAllRows )
            {
                //Double checks if this has been selected for coherence with filtering and sorting
                if ( ParentDataGrid.SelectedRows.Any( x => (object)x == (object)Item ) )
                    IsChecked = true;

                await InvokeAsync( () => StateHasChanged() );
            }

            if ( ParentDataGrid.UnSelectAllRows )
            {
                IsChecked = false;
                await InvokeAsync( () => StateHasChanged() );
            }

            await base.OnParametersSetAsync();
        }

        #endregion

        #region Properties

        internal bool IsChecked { get; set; }

        [Parameter] public TItem Item { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public string Width { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public string Style { get; set; }

        [Parameter] public TextAlignment TextAlignment { get; set; }

        [Parameter] public EventCallback<bool> OnSelectedChanged { get; set; }

        #endregion
    }
}