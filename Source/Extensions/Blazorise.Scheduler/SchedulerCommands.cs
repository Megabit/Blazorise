#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Configures command visibility and templates for scheduler items and editing dialogs.
/// This component does not render any markup.
/// </summary>
/// <typeparam name="TItem">The type of the items used in the scheduler.</typeparam>
public class SchedulerCommands<TItem> : ComponentBase, IAsyncDisposable
{
    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        Scheduler<TItem> previousScheduler = Scheduler;
        bool commandsChanged = parameters.IsParameterChanged( DeleteCommandAllowed )
            || parameters.IsParameterChanged( SaveCommandAllowed )
            || parameters.IsParameterChanged( CancelCommandAllowed )
            || parameters.IsParameterChanged( DeleteCommandTemplate )
            || parameters.IsParameterChanged( SaveCommandTemplate )
            || parameters.IsParameterChanged( CancelCommandTemplate );

        await base.SetParametersAsync( parameters );

        if ( previousScheduler is not null && !ReferenceEquals( previousScheduler, Scheduler ) )
        {
            await previousScheduler.RemoveSchedulerCommands( this );
        }

        if ( Scheduler is not null )
        {
            await Scheduler.NotifySchedulerCommands( this, commandsChanged );
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if ( Scheduler is not null )
        {
            await Scheduler.RemoveSchedulerCommands( this );
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the scheduler component that the commands belong to.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Controls the visibility of delete commands on items and in editing dialogs. Defaults to true.
    /// This setting does not prevent programmatic deletion.
    /// </summary>
    [Parameter] public bool DeleteCommandAllowed { get; set; } = true;

    /// <summary>
    /// Controls the visibility of the save command in editing dialogs. Defaults to true.
    /// </summary>
    [Parameter] public bool SaveCommandAllowed { get; set; } = true;

    /// <summary>
    /// Controls the visibility of the cancel command in editing dialogs. Defaults to true.
    /// </summary>
    [Parameter] public bool CancelCommandAllowed { get; set; } = true;

    /// <summary>
    /// Customizes the delete command on items and in editing dialogs.
    /// Rendered only when <see cref="DeleteCommandAllowed"/> is true.
    /// </summary>
    [Parameter] public RenderFragment<SchedulerDeleteCommandContext<TItem>> DeleteCommandTemplate { get; set; }

    /// <summary>
    /// Customizes the save command in editing dialogs.
    /// Rendered only when <see cref="SaveCommandAllowed"/> is true.
    /// </summary>
    [Parameter] public RenderFragment<SchedulerCommandContext<TItem>> SaveCommandTemplate { get; set; }

    /// <summary>
    /// Customizes the cancel command in editing dialogs.
    /// Rendered only when <see cref="CancelCommandAllowed"/> is true.
    /// </summary>
    [Parameter] public RenderFragment<SchedulerCommandContext<TItem>> CancelCommandTemplate { get; set; }

    #endregion
}