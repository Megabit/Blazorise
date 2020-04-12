#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public class DataGridAggregateColumn<TItem> : BaseDataGridColumn<TItem>
    {
        #region Members

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentDataGrid != null )
            {
                // connect column to the parent datagrid
                ParentDataGrid.Hook( this );
            }

            base.OnInitialized();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Type of aggregate calculation.
        /// </summary>
        [Parameter] public DataGridAggregateType AggregateType { get; set; }

        /// <summary>
        /// Optional display template for aggregate values.
        /// </summary>
        [Parameter] public RenderFragment<AggregateContext> DisplayTemplate { get; set; }

        #endregion
    }
}
