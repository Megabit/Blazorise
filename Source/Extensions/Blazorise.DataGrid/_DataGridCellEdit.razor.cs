﻿#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Internal component for editing the row item cell value.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public partial class _DataGridCellEdit<TItem> : ComponentBase
    {
        /// <summary>
        /// Updated the internal cell values.
        /// </summary>
        /// <typeparam name="T">Type of the value being changed.</typeparam>
        /// <param name="value">Value that is updating.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task OnEditValueChanged<T>( T value )
        {
            CellEditContext.CellValue = value;

            Column.SetValue( ValidationItem, value );

            return CellValueChanged.InvokeAsync( value );
        }

        /// <summary>
        /// Column that this cell belongs to.
        /// </summary>
        [Parameter] public DataGridColumn<TItem> Column { get; set; }

        /// <summary>
        /// Field name that this cell belongs to.
        /// </summary>
        [Parameter] public string Field { get; set; }

        /// <summary>
        /// Instance of the currently editing row item.
        /// </summary>
        [Parameter] public TItem Item { get; set; }

        /// <summary>
        /// Instance of the currently validating row item.
        /// </summary>
        [Parameter] public TItem ValidationItem { get; set; }

        /// <summary>
        /// Value data type.
        /// </summary>
        [Parameter] public Type ValueType { get; set; }

        /// <summary>
        /// Currently editing cell value.
        /// </summary>
        [Parameter] public CellEditContext<TItem> CellEditContext { get; set; }

        /// <summary>
        /// Prevents user from editing the cell value.
        /// </summary>
        [Parameter] public bool Readonly { get; set; }

        /// <summary>
        /// Raises when cell value changes.
        /// </summary>
        [Parameter] public EventCallback<object> CellValueChanged { get; set; }

        /// <summary>
        /// Specifies the interval between valid values.
        /// </summary>
        [Parameter] public decimal? Step { get; set; }

        /// <summary>
        /// Maximum number of decimal places after the decimal separator.
        /// </summary>
        [Parameter] public int Decimals { get; set; } = 2;

        /// <summary>
        /// String to use as the decimal separator in numeric values.
        /// </summary>
        [Parameter] public string DecimalsSeparator { get; set; } = ".";

        /// <summary>
        /// Helps define the language of an element.
        /// </summary>
        /// <remarks>
        /// https://www.w3schools.com/tags/ref_language_codes.asp
        /// </remarks>
        [Parameter] public string Culture { get; set; }

        /// <summary>
        /// If true, step buttons will be visible.
        /// </summary>
        [Parameter] public bool? ShowStepButtons { get; set; }

        /// <summary>
        /// If true, enables change of numeric value by pressing on step buttons or by keyboard up/down keys.
        /// </summary>
        [Parameter] public bool? EnableStep { get; set; }
    }
}