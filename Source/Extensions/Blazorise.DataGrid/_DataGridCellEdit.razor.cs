#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Parameter] public EventCallback<object> CellValueChanged { get; set; }
    }
}
