#region Using directives
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridRowMultiSelect<TItem> : ComponentBase
    {
        #region Methods

        internal Task OnCheckedChanged( bool @checked )
        {
            Checked = @checked;
            return CheckedChanged.InvokeAsync( @checked );
        }

        #endregion

        #region Properties

        [Parameter] public TItem Item { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public string Width { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public string Style { get; set; }

        [Parameter] public TextAlignment TextAlignment { get; set; }

        [Parameter] public bool Checked { get; set; }

        [Parameter] public EventCallback<bool> CheckedChanged { get; set; }

        #endregion
    }
}