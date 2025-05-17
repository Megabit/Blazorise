#region Using directives
using System;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Event arguments for the <see cref="Scheduler{TItem}.ItemClicked"/> event.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class SchedulerItemClickedEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    public SchedulerItemClickedEventArgs( TItem item )
    {
        Item = item;
    }

    /// <summary>
    /// Gets the item that was clicked.
    /// </summary>
    public TItem Item { get; }
}