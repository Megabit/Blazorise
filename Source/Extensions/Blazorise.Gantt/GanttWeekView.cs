#region Using directives
namespace Blazorise.Gantt;
#endregion

/// <summary>
/// Week view configuration.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public partial class GanttWeekView<TItem> : BaseGanttView<TItem>
{
    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Gantt?.NotifyGanttWeekView( this );

        base.OnInitialized();
    }
}