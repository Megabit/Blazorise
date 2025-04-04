#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Defines the drag&amp;drop transaction.
/// </summary>
/// <typeparam name="TItem">Type of drag&amp;drop item.</typeparam>
public class DraggableTransaction<TItem>
{
    #region Members

    private Func<Task> Commited;

    private Func<Task> Canceled;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="DraggableTransaction{TItem}"/> constructor.
    /// </summary>
    /// <param name="item">The dropped item during the transaction.</param>
    /// <param name="sourceZoneName">Name of the zone where the transaction started.</param>
    /// <param name="index">Index of the draggable item.</param>
    /// <param name="commited">Callback that will be called after the successful transaction.</param>
    /// <param name="canceled">Callback that will be called when the transaction has been cancelled.</param>
    public DraggableTransaction( TItem item, string sourceZoneName, int index, Func<Task> commited, Func<Task> canceled )
    {
        Item = item;
        SourceZoneName = sourceZoneName;
        CurrentZoneName = sourceZoneName;
        Index = index;
        SourceIndex = index;

        Commited = commited;
        Canceled = canceled;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Cancel the transaction.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Cancel()
    {
        if ( Canceled is not null )
            return Canceled.Invoke();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Commit this transaction as succesful
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Commit()
    {
        if ( Commited is not null )
            return Commited.Invoke();

        return Task.CompletedTask;
    }


    internal bool UpdateIndex( int index )
    {
        if ( Index == index )
            return false;

        Index = index;

        return true;
    }

    internal bool UpdateZoneName( string zoneName )
    {
        if ( CurrentZoneName == zoneName )
            return false;

        CurrentZoneName = zoneName;
        Index = -1;

        return true;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the item that is being dragged during the transaction.
    /// </summary>
    public TItem Item { get; init; }

    /// <summary>
    /// The index of the draggable item.
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// The index of the item when the transaction started.
    /// </summary>
    public int SourceIndex { get; private set; }

    /// <summary>
    /// Gets the name of the drop zone where the transaction has started.
    /// </summary>
    public string SourceZoneName { get; private set; }

    /// <summary>
    /// Gets the name of the drop zone where the transaction is currently executing.
    /// </summary>
    public string CurrentZoneName { get; private set; }

    #endregion
}