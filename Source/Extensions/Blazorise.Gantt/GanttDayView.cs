#region Using directives
namespace Blazorise.Gantt;
#endregion

/// <summary>
/// Day view configuration.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public partial class GanttDayView<TItem> : BaseGanttView<TItem>
{
    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Gantt?.NotifyGanttDayView( this );

        base.OnInitialized();
    }
}