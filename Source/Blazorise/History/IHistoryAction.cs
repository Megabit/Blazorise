namespace Blazorise.History;

/// <summary>
/// Represents a reversible operation that can be applied to a mutable state object.
/// </summary>
/// <typeparam name="TState">The state object type that the action mutates.</typeparam>
public interface IHistoryAction<TState>
{
    /// <summary>
    /// Gets the display name of the history action.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Applies the action to the supplied state.
    /// </summary>
    /// <param name="state">The state object to mutate.</param>
    void Do( TState state );

    /// <summary>
    /// Reverts the action from the supplied state.
    /// </summary>
    /// <param name="state">The state object to mutate.</param>
    void Undo( TState state );
}