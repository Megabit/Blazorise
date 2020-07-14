﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public class DataGridAggregate<TItem> : BaseDataGridComponent
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

        /// <summary>
        /// Gets the formated display value.
        /// </summary>
        /// <param name="item">Item the contains the value to format.</param>
        /// <returns></returns>
        internal string FormatDisplayValue( object value )
        {
            if ( DisplayFormat != null )
            {
                return string.Format( DisplayFormatProvider ?? CultureInfo.CurrentCulture, DisplayFormat, value );
            }

            return value?.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// To bind a column to a data source field, set this property to the required data field name.
        /// </summary>
        [Parameter] public string Field { get; set; }

        /// <summary>
        /// Type of aggregate calculation.
        /// </summary>
        [Parameter] public DataGridAggregateType Aggregate { get; set; }

        /// <summary>
        /// Optional display template for aggregate values.
        /// </summary>
        [Parameter] public RenderFragment<AggregateContext> DisplayTemplate { get; set; }

        /// <summary>
        /// Defines the format for display value.
        /// </summary>
        [Parameter] public string DisplayFormat { get; set; }

        /// <summary>
        /// Defines the format provider info for display value.
        /// </summary>
        [Parameter] public IFormatProvider DisplayFormatProvider { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        #endregion
    }
}
