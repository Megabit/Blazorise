#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public class CommandContext
    {
        public EventCallback Clicked { get; set; }
    }

    public class CommandContext<TItem> : CommandContext
    {
        public TItem Item { get; set; }
    }

    public class EditCommandContext<TItem> : CommandContext<TItem>
    {

    }

    public class ButtonRowContext<TItem> 
    {
        public EditCommandContext<TItem> EditCommand { get; set; }
    }
}
