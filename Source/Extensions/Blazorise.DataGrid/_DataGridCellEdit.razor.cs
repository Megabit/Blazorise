#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public partial class _DataGridCellEdit<TItem> : ComponentBase
    {
        protected Task OnEditValueChanged<T>( T value )
        {
            CellEditContext.CellValue = value;

            return CellValueChanged.InvokeAsync( value );
        }

        /// <summary>
        /// Row model.
        /// </summary>
        [Parameter] public TItem Item { get; set; }

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
    }
}
