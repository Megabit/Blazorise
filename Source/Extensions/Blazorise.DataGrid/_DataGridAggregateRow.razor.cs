#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public partial class _DataGridAggregateRow<TItem> : BaseDataGridComponent
    {
        #region Members

        #endregion

        #region Methods

        protected object Calculate( DataGridAggregate<TItem> aggregate, DataGridColumn<TItem> column )
            => aggregate?.AggregationFunction?.Invoke( Data, column );

        #endregion

        #region Properties

        protected IEnumerable<TItem> Data
            => ParentDataGrid.ManualReadMode ? ParentDataGrid.AggregateData : ParentDataGrid.Data;

        /// <summary>
        /// List of columns used to build this row.
        /// </summary>
        [Parameter] public IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

        /// <summary>
        /// List of aggregate columns used to build this row.
        /// </summary>
        [Parameter] public IEnumerable<DataGridAggregate<TItem>> Aggregates { get; set; }

        /// <summary>
        /// Custom css classname.
        /// </summary>
        [Parameter] public string Class { get; set; }

        /// <summary>
        /// Custom html style.
        /// </summary>
        [Parameter] public string Style { get; set; }

        /// <summary>
        /// Custom background.
        /// </summary>
        [Parameter] public Background Background { get; set; }

        /// <summary>
        /// Custom color.
        /// </summary>
        [Parameter] public Color Color { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        #endregion
    }
}
