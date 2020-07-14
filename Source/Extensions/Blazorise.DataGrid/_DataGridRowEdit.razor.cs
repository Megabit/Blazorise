﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridRowEdit<TItem> : ComponentBase
    {
        protected bool CellAreEditable( DataGridColumn<TItem> column )
        {
            return column.Editable &&
                ( ( column.CellsEditableOnNewCommand && ParentDataGrid?.EditState == DataGridEditState.New )
                || ( column.CellsEditableOnEditCommand && ParentDataGrid?.EditState == DataGridEditState.Edit ) );
        }


        [Parameter] public TItem Item { get; set; }

        [Parameter] public IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

        [Parameter] public Dictionary<string, CellEditContext> CellValues { get; set; }

        [Parameter] public DataGridEditMode EditMode { get; set; }

        [Parameter] public EventCallback Save { get; set; }

        [Parameter] public EventCallback Cancel { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public string Width { get; set; }
    }
}
