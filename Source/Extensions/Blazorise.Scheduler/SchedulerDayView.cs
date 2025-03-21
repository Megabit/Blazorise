#region Using directives
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents a view for scheduling events on a daily basis within a scheduler framework.
/// </summary>
/// <typeparam name="TItem">Represents the type of items that can be scheduled, such as events or tasks.</typeparam>
public partial class SchedulerDayView<TItem> : BaseSchedulerView<TItem>
{
    #region Members

    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        Scheduler?.NotifySchedulerDayView( this );

        base.OnInitialized();
    }

    #endregion

    #region Properties

    #endregion
}
