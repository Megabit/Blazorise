using System.Threading.Tasks;

namespace Blazorise.LoadingIndicator;

/// <summary>
/// Provides shared control over registered <see cref="LoadingIndicator"/> components.
/// </summary>
public interface ILoadingIndicatorService
{
    /// <summary>
    /// Gets the shared Visible state for all subscribed indicators, or null when they differ.
    /// </summary>
    bool? Visible { get; }

    /// <summary>
    /// Gets the shared Initializing state for all subscribed indicators, or null when they differ.
    /// </summary>
    bool? Initializing { get; }

    /// <summary>
    /// Gets the shared status for all subscribed indicators, or null when they differ.
    /// </summary>
    LoadingIndicatorStatus Status { get; }

    /// <summary>
    /// Shows all subscribed loading indicators.
    /// </summary>
    Task Show();

    /// <summary>
    /// Hides all subscribed loading indicators.
    /// </summary>
    Task Hide();

    /// <summary>
    /// Sets the Initializing state for all subscribed indicators.
    /// </summary>
    /// <param name="value">True to show initializing state; otherwise false.</param>
    Task SetInitializing( bool value );

    /// <summary>
    /// Sets the status for all subscribed indicators.
    /// </summary>
    /// <param name="status">Status data.</param>
    Task SetStatus( LoadingIndicatorStatus status );

    /// <summary>
    /// Sets the status for all subscribed indicators.
    /// </summary>
    /// <param name="text">Optional status text.</param>
    /// <param name="progress">Optional progress value.</param>
    Task SetStatus( string text = null, int? progress = null );

    /// <summary>
    /// Subscribes a <see cref="LoadingIndicator"/> to shared state updates.
    /// </summary>
    /// <param name="indicator">The indicator to subscribe.</param>
    public void Subscribe( LoadingIndicator indicator );

    /// <summary>
    /// Unsubscribes a <see cref="LoadingIndicator"/> from shared state updates.
    /// </summary>
    /// <param name="indicator">The indicator to unsubscribe.</param>
    public void Unsubscribe( LoadingIndicator indicator );
}