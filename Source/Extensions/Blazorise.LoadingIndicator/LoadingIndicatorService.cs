#region Using directives
using System;
#endregion

namespace Blazorise.LoadingIndicator
{
    /// <summary>
    /// A service to control LoadingIndicator components
    /// </summary>
    internal class LoadingIndicatorService : ILoadingIndicatorService
    {
        #region Members

        private LoadingIndicator indicator;

        #endregion

        #region Methods

        /// <summary>
        /// Set indicator busy state to true
        /// </summary>
        public void Show() => indicator?.Show();

        /// <summary>
        /// Set indicator busy state to false
        /// </summary>
        public void Hide() => indicator?.Hide();

        /// <summary>
        /// Set Loaded state
        /// </summary>
        /// <param name="value">true or false</param>
        public void SetLoaded( bool value ) => indicator?.SetLoaded( value );

        /// <summary>
        /// Subscribe indicator to change events and save reference
        /// </summary>
        /// <param name="indicator"></param>
        void ILoadingIndicatorService.Subscribe( LoadingIndicator indicator )
        {
            if ( this.indicator != null && this.indicator != indicator )
            {
                throw new InvalidOperationException( $"{nameof( LoadingIndicatorService )} already initialized. The service supports only one loading indicator." );
            }
            this.indicator = indicator;
        }

        /// <summary>
        /// Unsubscribe indicator from change events and remove reference
        /// </summary>
        /// <param name="indicator"></param>
        void ILoadingIndicatorService.Unsubscribe( LoadingIndicator indicator )
        {
            if ( this.indicator == indicator )
            {
                this.indicator = null;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns indicator Busy state
        /// </summary>
        public bool? Busy => indicator?.Busy;

        /// <summary>
        /// Returns indicator Loaded state
        /// </summary>
        public bool? Loaded => indicator?.Loaded;

        #endregion
    }
}