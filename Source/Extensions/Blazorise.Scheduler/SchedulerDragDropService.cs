#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.DeepCloner;
#endregion

namespace Blazorise.Scheduler;

public class SchedulerDragDropService<TItem>
{
    private Scheduler<TItem> scheduler;

    private TItem draggedItem;
    private TItem originalState;

    public event Action<TItem> DragStarted;
    public event Action<TItem> DragCancelled;
    public event Action<TItem, DateTime, DateTime> Dropped;

    public SchedulerDragDropService( Scheduler<TItem> scheduler )
    {
        this.scheduler = scheduler;
    }

    public void StartDrag( TItem item )
    {
        draggedItem = item;
        originalState = item.DeepClone();
        DragStarted?.Invoke( item );
    }

    public void CancelDrag()
    {
        draggedItem = default;
        originalState = default;
        DragCancelled?.Invoke( draggedItem );
    }

    public async Task<bool> Drop( TItem item, DateTime newStart, DateTime newEnd )
    {
        try
        {
            var duration = scheduler.GetItemDuration( item );
            scheduler.SetItemDates( item, newStart, newStart.Add( duration ) );

            // Validate and commit changes
            Dropped?.Invoke( item, newStart, newEnd );
            return true;
        }
        catch
        {
            // Rollback on error
            CancelDrag();
            return false;
        }
        finally
        {
            await scheduler.Refresh();
        }
    }
}
