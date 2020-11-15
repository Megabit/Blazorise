#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

#endregion Using directives

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridRowMultiSelect<TItem> : ComponentBase
    {
        [Parameter] public TItem Item { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public string Width { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public string Style { get; set; }

        [Parameter] public TextAlignment TextAlignment { get; set; }

        [Parameter] public EventCallback<bool> OnSelectedChanged { get; set; }

        public bool IsChecked { get; set; }

        public Task IsCheckedChanged( bool e )
        {
            IsChecked = e;
            return OnSelectedChanged.InvokeAsync( e );
        }
    }
}