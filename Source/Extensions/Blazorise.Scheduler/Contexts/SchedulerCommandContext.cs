#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Carries the item, callback, and localized text for a scheduler command.
/// </summary>
/// <typeparam name="TItem">The type of the scheduled item.</typeparam>
public class SchedulerCommandContext<TItem>
{
    /// <summary>
    /// Gets or sets the item associated with the command.
    /// </summary>
    public TItem Item { get; set; }

    /// <summary>
    /// Gets or sets the callback that invokes the command's existing action.
    /// </summary>
    public EventCallback Clicked { get; set; }

    /// <summary>
    /// Gets or sets the localized text for the command.
    /// </summary>
    public string LocalizationString { get; set; }
}