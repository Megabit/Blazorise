#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

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

/// <summary>
/// Carries command state and callbacks.
/// </summary>
public class CommandContext<TItem> : CommandContext
{
    /// <summary>
    /// Item consumed by the command context.
    /// </summary>
    public TItem Item { get; set; }
}

/// <summary>
/// Carries new command state and callbacks.
/// </summary>
public class NewCommandContext<TItem> : CommandContext
{
}

/// <summary>
/// Carries edit command state and callbacks.
/// </summary>
public class EditCommandContext<TItem> : CommandContext<TItem>
{
}

/// <summary>
/// Carries delete command state and callbacks.
/// </summary>
public class DeleteCommandContext<TItem> : CommandContext<TItem>
{
}

/// <summary>
/// Carries button row state and callbacks.
/// </summary>
public class ButtonRowContext<TItem>
{
    /// <summary>
    /// Context for rendering the new-item command.
    /// </summary>
    public NewCommandContext<TItem> NewCommand { get; set; }
    /// <summary>
    /// Context for rendering the edit command.
    /// </summary>
    public EditCommandContext<TItem> EditCommand { get; set; }
    /// <summary>
    /// Context for rendering the delete command.
    /// </summary>
    public DeleteCommandContext<TItem> DeleteCommand { get; set; }
    /// <summary>
    /// Context for rendering the clear-filter command.
    /// </summary>
    public CommandContext<TItem> ClearFilterCommand { get; set; }
    /// <summary>
    /// Context for rendering the save-batch command.
    /// </summary>
    public CommandContext<TItem> SaveBatchCommand { get; set; }
    /// <summary>
    /// Context for rendering the cancel-batch command.
    /// </summary>
    public CommandContext<TItem> CancelBatchCommand { get; set; }
}