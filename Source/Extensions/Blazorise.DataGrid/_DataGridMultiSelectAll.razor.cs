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

            return ParentDataGrid.OnMultiSelectAll( IsChecked );
        }

        protected override Task OnParametersSetAsync()
        {
            IsChecked = ( ParentDataGrid.SelectedRows?.Any() ?? false )
                && ParentDataGrid.DisplayData.Any()
                && !ParentDataGrid.DisplayData.Except( ParentDataGrid.SelectedRows ).Any();

            return base.OnParametersSetAsync();
        }

        #endregion

        #region Properties

        internal bool IsChecked { get; set; }

        /// <summary>
        /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
        /// </summary>
        [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public bool IsIndeterminate { get; set; }

        #endregion
    }
}