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

        /// <summary>
        /// Item associated with the data set.
        /// </summary>
        [Parameter] public TItem Item { get; set; }

        /// <summary>
        /// List of columns used to build this row.
        /// </summary>
        [Parameter] public IEnumerable<BaseDataGridColumn<TItem>> Columns { get; set; }

        [CascadingParameter] protected BaseDataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
