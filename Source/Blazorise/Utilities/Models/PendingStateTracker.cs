#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Represents a pending state snapshot for a tracked item.
/// </summary>
/// <typeparam name="TItem">Type of tracked item.</typeparam>
/// <param name="Item">Tracked item instance.</param>
/// <param name="Timestamp">UTC timestamp when the state was registered.</param>
/// <param name="Pending">Indicates whether a pending state is currently active.</param>
public readonly record struct PendingState<TItem>( TItem Item, DateTime Timestamp, bool Pending );

/// <summary>
/// Tracks a single pending item and allows consuming it within a configured time window.
/// </summary>
/// <typeparam name="TItem">Type of tracked item.</typeparam>
public sealed class PendingStateTracker<TItem>
{
    #region Members

    private readonly TimeSpan pendingWindow;

    private PendingState<TItem> pendingState;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new tracker with the maximum allowed pending window.
    /// </summary>
    /// <param name="pendingWindow">Maximum allowed age for the pending state.</param>
    public PendingStateTracker( TimeSpan pendingWindow )
    {
        this.pendingWindow = pendingWindow;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Registers an item as pending using the current UTC time.
    /// </summary>
    /// <param name="item">Item to mark as pending.</param>
    public void Register( TItem item )
    {
        if ( item is null )
            return;

        pendingState = new( item, DateTime.UtcNow, true );
    }

    /// <summary>
    /// Attempts to consume the pending item if it matches and is still within the pending window.
    /// </summary>
    /// <param name="item">Item expected to match the pending state.</param>
    /// <returns><c>true</c> if the pending state was matched and consumed; otherwise <c>false</c>.</returns>
    public bool TryConsume( TItem item )
    {
        if ( item is null || !pendingState.Pending )
            return false;

        if ( pendingState.Timestamp < DateTime.UtcNow - pendingWindow )
        {
            pendingState = default;
            return false;
        }

        if ( !EqualityComparer<TItem>.Default.Equals( pendingState.Item, item ) )
            return false;

        pendingState = default;

        return true;
    }

    #endregion
}