#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridModal<TItem> : ComponentBase
    {
        [Parameter] public TItem EditItem { get; set; }

        [Parameter] public IEnumerable<BaseDataGridColumn<TItem>> Columns { get; set; }

        [Parameter] public IReadOnlyDictionary<string, CellEditContext> EditItemCellValues { get; set; }

        [Parameter] public bool PopupVisible { get; set; }

        [Parameter] public DataGridEditState EditState { get; set; }

        [Parameter] public EventCallback Save { get; set; }

        [Parameter] public EventCallback Cancel { get; set; }

        [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }
    }
}
