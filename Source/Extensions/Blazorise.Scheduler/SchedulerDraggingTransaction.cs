#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.DeepCloner;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// A scheduler transaction representing an item being dragged or edited.
/// </summary>
/// <typeparam name="TItem">The type of the scheduler item.</typeparam>
public class SchedulerDraggingTransaction<TItem> : SchedulerTransaction<TItem>
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerDraggingTransaction{TItem}"/> class.
    /// </summary>
    public SchedulerDraggingTransaction( Scheduler<TItem> scheduler, TItem item, SchedulerSection section )
        : base( scheduler, section )
    {
        OriginalItem = item;
        Item = item.DeepClone();
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override async Task CommitImpl()
    {
        var isRecurrenceItem = scheduler.IsRecurrenceItem( Item );

        var editItemClone = isRecurrenceItem
            ? scheduler.GetParentItem( Item ).DeepClone()
            : Item;

        if ( isRecurrenceItem && editItemClone is not null )
        {
            scheduler.AddRecurrenceException( editItemClone, Item );
        }

        if ( scheduler.DropAllowed is not null
            && await scheduler.DropAllowed.Invoke( new SchedulerDragEventArgs<TItem>( editItemClone ) ) == false )
        {
            throw new OperationCanceledException( "Drop was not allowed." );
        }

        var success = await scheduler.SaveImpl( editItemClone );

        if ( !success )
            throw new InvalidOperationException( "Saving the item failed." );
    }

    /// <inheritdoc />
    protected override Task RollbackImpl()
    {
        Item = OriginalItem.DeepClone();

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the original item that was passed into the transaction.
    /// </summary>
    protected TItem OriginalItem { get; init; }

    /// <summary>
    /// Gets the mutable working copy of the item used during the transaction.
    /// </summary>
    public TItem Item { get; protected set; }

    #endregion

}
