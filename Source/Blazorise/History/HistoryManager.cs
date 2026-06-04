using System;
using System.Collections.Generic;

namespace Blazorise.History;

/// <summary>
/// Manages undo and redo stacks for reversible state transitions.
/// </summary>
/// <typeparam name="TState">The state object type that history actions mutate.</typeparam>
public sealed class HistoryManager<TState>
{
    private readonly int maxHistoryActions;

    private readonly List<IHistoryAction<TState>> undoStack = [];

    private readonly List<IHistoryAction<TState>> redoStack = [];

    private HistoryTransaction<TState> currentTransaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryManager{TState}"/> class.
    /// </summary>
    /// <param name="maxHistoryActions">The maximum number of undo actions retained by the manager.</param>
    public HistoryManager( int maxHistoryActions = 50 )
    {
        if ( maxHistoryActions <= 0 )
            throw new ArgumentOutOfRangeException( nameof( maxHistoryActions ) );

        this.maxHistoryActions = maxHistoryActions;
    }

    /// <summary>
    /// Gets a value indicating whether an undo action is available.
    /// </summary>
    public bool CanUndo => undoStack.Count > 0;

    /// <summary>
    /// Gets a value indicating whether a redo action is available.
    /// </summary>
    public bool CanRedo => redoStack.Count > 0;

    /// <summary>
    /// Gets the display name of the next undo action.
    /// </summary>
    public string UndoName => undoStack.Count > 0 ? undoStack[^1].Name : null;

    /// <summary>
    /// Gets the display name of the next redo action.
    /// </summary>
    public string RedoName => redoStack.Count > 0 ? redoStack[^1].Name : null;

    /// <summary>
    /// Gets a value indicating whether a transaction is currently active.
    /// </summary>
    public bool IsInTransaction => currentTransaction is not null;

    /// <summary>
    /// Executes an action and records it in history.
    /// </summary>
    /// <param name="action">The action to execute and record.</param>
    /// <param name="state">The state object to mutate.</param>
    public void Execute( IHistoryAction<TState> action, TState state )
    {
        if ( action is null )
            return;

        action.Do( state );
        Record( action );
    }

    /// <summary>
    /// Records an action that has already been applied.
    /// </summary>
    /// <param name="action">The action to record.</param>
    public void Record( IHistoryAction<TState> action )
    {
        if ( action is null )
            return;

        if ( currentTransaction is not null )
        {
            currentTransaction.Add( action );
            return;
        }

        undoStack.Add( action );
        redoStack.Clear();
        Trim();
    }

    /// <summary>
    /// Starts a transaction that groups subsequent actions into a single history item.
    /// </summary>
    /// <param name="name">The display name of the transaction.</param>
    public void BeginTransaction( string name )
    {
        currentTransaction ??= new( name );
    }

    /// <summary>
    /// Commits the active transaction to the undo stack.
    /// </summary>
    public void CommitTransaction()
    {
        if ( currentTransaction is null )
            return;

        if ( currentTransaction.HasActions )
        {
            undoStack.Add( currentTransaction );
            redoStack.Clear();
            Trim();
        }

        currentTransaction = null;
    }

    /// <summary>
    /// Cancels the active transaction and reverts any actions it contains.
    /// </summary>
    /// <param name="state">The state object to mutate.</param>
    public void CancelTransaction( TState state )
    {
        if ( currentTransaction is not null )
        {
            currentTransaction.Undo( state );
            currentTransaction = null;
        }
    }

    /// <summary>
    /// Reverts the latest undo action.
    /// </summary>
    /// <param name="state">The state object to mutate.</param>
    public void Undo( TState state )
    {
        if ( undoStack.Count == 0 )
            return;

        IHistoryAction<TState> action = undoStack[^1];
        undoStack.RemoveAt( undoStack.Count - 1 );
        action.Undo( state );
        redoStack.Add( action );
    }

    /// <summary>
    /// Reapplies the latest redo action.
    /// </summary>
    /// <param name="state">The state object to mutate.</param>
    public void Redo( TState state )
    {
        if ( redoStack.Count == 0 )
            return;

        IHistoryAction<TState> action = redoStack[^1];
        redoStack.RemoveAt( redoStack.Count - 1 );
        action.Do( state );
        undoStack.Add( action );
        Trim();
    }

    /// <summary>
    /// Clears all undo, redo, and active transaction state.
    /// </summary>
    public void Clear()
    {
        undoStack.Clear();
        redoStack.Clear();
        currentTransaction = null;
    }

    private void Trim()
    {
        if ( undoStack.Count > maxHistoryActions )
            undoStack.RemoveAt( 0 );
    }
}