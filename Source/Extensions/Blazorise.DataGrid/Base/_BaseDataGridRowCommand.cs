#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Base
{
    public abstract class _BaseDataGridRowCommand : ComponentBase
    {
        [Parameter] public DataGridEditState EditState { get; set; }

        [Parameter] public EventCallback Edit { get; set; }

        [Parameter] public EventCallback Delete { get; set; }

        [Parameter] public EventCallback Save { get; set; }

        [Parameter] public EventCallback Cancel { get; set; }
    }
}
