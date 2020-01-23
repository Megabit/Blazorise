#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridDetailRow<TItem> : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        #endregion

        #region Properties

        protected bool HasCommandColumn => Columns.Any( x => x.ColumnType == DataGridColumnType.Command );

        protected string CommandColumnLocation
        {
            get
            {
                if ( Columns.Count > 1 && HasCommandColumn )
                {
                    if ( Columns[0].ColumnType == DataGridColumnType.Command )
                        return "start";
                    else if ( Columns[Columns.Count - 1].ColumnType == DataGridColumnType.Command )
                        return "end";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Item associated with the data set.
        /// </summary>
        [Parameter] public TItem Item { get; set; }

        /// <summary>
        /// List of columns used to build this row.
        /// </summary>
        [Parameter] public IReadOnlyList<BaseDataGridColumn<TItem>> Columns { get; set; }

        [CascadingParameter] protected BaseDataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
