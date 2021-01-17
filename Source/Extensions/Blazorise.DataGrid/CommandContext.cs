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

    public class NewCommandContext : CommandContext
    {

    }

    public class EditCommandContext<TItem> : CommandContext<TItem>
    {

    }

    public class DeleteCommandContext<TItem> : CommandContext<TItem>
    {

    }

    public class ButtonRowContext<TItem> 
    {
        public NewCommandContext NewCommand { get; set; }
        public EditCommandContext<TItem> EditCommand { get; set; }
        public DeleteCommandContext<TItem> DeleteCommand { get; set; }
        public CommandContext ClearFilterCommand { get; set; }
    }
}
