#region Using directives
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents a monthly view in a scheduler, allowing customization of the displayed month.
/// </summary>
/// <typeparam name="TItem">This type parameter is used to define the type of items that will be scheduled and displayed in the view.</typeparam>
public partial class SchedulerMonthView<TItem> : BaseSchedulerView<TItem>
{
    #region Methods

    protected override void OnInitialized()
    {
        Scheduler?.NotifySchedulerMonthView( this );

        base.OnInitialized();
    }

    #endregion

    #region Properties

    #endregion
}
