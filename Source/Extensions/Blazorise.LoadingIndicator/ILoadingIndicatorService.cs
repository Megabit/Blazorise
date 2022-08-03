using System.Threading.Tasks;

namespace Blazorise.LoadingIndicator
{
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
        /// Returns Loaded state shared by all indicator instances or null if the state is not the same for all indicators.
        /// </summary>
        bool? Loaded { get; }

        /// <summary>
        /// Show loading indicator
        /// </summary>
        Task Show();

        /// <summary>
        /// Hide loading indicator
        /// </summary>
        Task Hide();

        /// <summary>
        /// Set Loaded state
        /// </summary>
        /// <param name="value">true or false</param>
        Task SetLoaded( bool value );

        internal void Subscribe( LoadingIndicator indicator );
        internal void Unsubscribe( LoadingIndicator indicator );
    }
}