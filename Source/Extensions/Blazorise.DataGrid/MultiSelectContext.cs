#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public class MultiSelectContext<TItem>
    {
        public EventCallback<bool> SelectedChanged { get; set; }

        public TItem Item { get; set; }
    }
}
