#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridCell<TItem> : ComponentBase
    {
        [Parameter] public BaseDataGridColumn<TItem> Column { get; set; }

        [Parameter] public TItem Item { get; set; }

        [Parameter] public CellEditContext CellEditContext { get; set; }

        [Parameter] public EventCallback<object> CellValueChanged { get; set; }
    }
}
