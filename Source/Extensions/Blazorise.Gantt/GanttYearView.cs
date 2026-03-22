#region Using directives
namespace Blazorise.Gantt;
#endregion

/// <summary>
/// Year view configuration.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public partial class GanttYearView<TItem> : BaseGanttView<TItem>
{
    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Gantt?.NotifyGanttYearView( this );

        base.OnInitialized();
    }
}