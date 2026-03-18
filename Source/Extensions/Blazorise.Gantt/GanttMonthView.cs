#region Using directives
namespace Blazorise.Gantt;
#endregion

/// <summary>
/// Month view configuration.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public partial class GanttMonthView<TItem> : BaseGanttView<TItem>
{
    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Gantt?.NotifyGanttMonthView( this );

        base.OnInitialized();
    }
}