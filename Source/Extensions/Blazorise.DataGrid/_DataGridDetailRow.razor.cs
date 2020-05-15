#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridDetailRow<TItem> : BaseDataGridComponent
    {
        #region Members

        #endregion

        #region Methods

        #endregion

        #region Properties

        protected bool HasCommandColumn
            => Columns.Any( x => x.ColumnType == DataGridColumnType.Command );

        protected int ColumnSpan
            => Columns.Count - ( HasCommandColumn && !ParentDataGrid.Editable ? 1 : 0 );

        /// <summary>
        /// Item associated with the data set.
        /// </summary>
        [Parameter] public TItem Item { get; set; }

        /// <summary>
        /// List of columns used to build this row.
        /// </summary>
        [Parameter] public IReadOnlyList<DataGridColumn<TItem>> Columns { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
