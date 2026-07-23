using System.Collections.Generic;

namespace Blazorise.History;

/// <summary>
/// Groups multiple history actions so they are undone and redone as a single operation.
/// </summary>
/// <typeparam name="TState">The state object type that the action mutates.</typeparam>
public sealed class HistoryTransaction<TState> : IHistoryAction<TState>
{
    private readonly List<IHistoryAction<TState>> actions = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryTransaction{TState}"/> class.
    /// </summary>
    /// <param name="name">The display name of the transaction.</param>
    public HistoryTransaction( string name )
    {
        Name = name;
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <summary>
    /// Gets the actions included in this transaction.
    /// </summary>
    public IReadOnlyList<IHistoryAction<TState>> Actions => actions;

    /// <summary>
    /// Gets a value indicating whether the transaction contains any actions.
    /// </summary>
    public bool HasActions => actions.Count > 0;

    /// <summary>
    /// Adds an action to the transaction.
    /// </summary>
    /// <param name="action">The action to add.</param>
    public void Add( IHistoryAction<TState> action )
    {
        if ( action is not null )
            actions.Add( action );
    }

    /// <inheritdoc/>
    public void Do( TState state )
    {
        foreach ( IHistoryAction<TState> action in actions )
        {
            action.Do( state );
        }
    }

    /// <inheritdoc/>
    public void Undo( TState state )
    {
        for ( int i = actions.Count - 1; i >= 0; i-- )
        {
            actions[i].Undo( state );
        }
    }
}