using System.Threading.Tasks;

namespace Blazorise.LoadingIndicator;

/// <summary>
/// A service to control LoadingIndicator components
/// </summary>
public interface ILoadingIndicatorService
{
    /// <summary>
    /// Returns indicator Visible state shared by all indicator instances or null if the state is not the same for all indicators.
    /// </summary>
    bool? Visible { get; }

    /// <summary>
    /// Returns Initializing state shared by all indicator instances or null if the state is not the same for all indicators.
    /// </summary>
    bool? Initializing { get; }

    /// <summary>
    /// Returns status shared by all indicator instances or null if the state is not the same for all indicators.
    /// </summary>
    LoadingIndicatorStatus Status { get; }

    /// <summary>
    /// Show loading indicator
    /// </summary>
    Task Show();

    /// <summary>
    /// Hide loading indicator
    /// </summary>
    Task Hide();

    /// <summary>
    /// Set Initializing state
    /// </summary>
    /// <param name="value">true or false</param>
    Task SetInitializing( bool value );

    /// <summary>
    /// Set indicator status.
    /// </summary>
    /// <param name="status">Status data.</param>
    Task SetStatus( LoadingIndicatorStatus status );

    /// <summary>
    /// Set indicator status.
    /// </summary>
    /// <param name="text">Optional status text.</param>
    /// <param name="progress">Optional progress value.</param>
    Task SetStatus( string text = null, int? progress = null );

    public void Subscribe( LoadingIndicator indicator );
    public void Unsubscribe( LoadingIndicator indicator );
}