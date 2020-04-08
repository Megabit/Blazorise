#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridClearFilterCommand<TItem> : ComponentBase
    {
        [Parameter] public EventCallback ClearFilter { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }
    }
}
