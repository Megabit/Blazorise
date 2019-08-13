#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridCellEdit : ComponentBase
    {
        protected Task OnEditValueChanged<T>( T value )
        {
            //Console.WriteLine( $"value: {value}, {value?.GetType()}" );
            CellEditContext.CellValue = value;

            return CellValueChanged.InvokeAsync( value );
        }

        /// <summary>
        /// Value data type.
        /// </summary>
        [Parameter] public Type ValueType { get; set; }

        /// <summary>
        /// Currently editing cell value.
        /// </summary>
        [Parameter] public CellEditContext CellEditContext { get; set; }

        /// <summary>
        /// Prevents user from editing the cell value.
        /// </summary>
        [Parameter] public bool Readonly { get; set; }

        [Parameter] public EventCallback<object> CellValueChanged { get; set; }
    }
}
