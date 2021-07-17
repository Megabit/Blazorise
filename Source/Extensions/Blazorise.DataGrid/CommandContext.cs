﻿#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Base class for all <see cref="DataGrid{TItem}"/> commands.
    /// </summary>
    public class CommandContext
    {
        /// <summary>
        /// Activates the click event for a command context.
        /// </summary>
        public EventCallback Clicked { get; set; }

        /// <summary>
        /// Gets the localized string for this command.
        /// </summary>
        public string LocalizationString { get; set; }
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