using System.Collections.Generic;

namespace Blazorise.LoadingIndicator
{
    /// <summary>
    /// A service to control LoadingIndicator components
    /// </summary>
    public class LoadingIndicatorService
    {
        #region Events

        internal delegate void StateChanged( bool val );
        internal event StateChanged BusyChanged;
        internal event StateChanged LoadedChanged;

        #endregion

        #region Members

        private object hashLock = new();
        private HashSet<LoadingIndicator> indicators = new();

        #endregion

        #region Methods

        /// <summary>
        /// Set Busy state
        /// </summary>
        /// <param name="val">true or false</param>
        public void SetBusy( bool val ) => BusyChanged.Invoke( val );

        /// <summary>
        /// Set Loaded state
        /// </summary>
        /// <param name="val">true or false</param>
        public void SetLoaded( bool val ) => LoadedChanged.Invoke( val );

        /// <summary>
        /// Subscribe indicator to change events and save reference
        /// </summary>
        /// <param name="indicator"></param>
        internal void Subscribe( LoadingIndicator indicator )
        {
            lock ( hashLock )
            {
                indicators.Add( indicator );
            }
            BusyChanged += indicator.Service_BusyChanged;
            LoadedChanged += indicator.Service_LoadedChanged;
        }

        /// <summary>
        /// Unsubscribe indicator from change events and remove reference
        /// </summary>
        /// <param name="indicator"></param>
        internal void Unsubscribe( LoadingIndicator indicator )
        {
            BusyChanged -= indicator.Service_BusyChanged;
            LoadedChanged -= indicator.Service_LoadedChanged;
            lock ( hashLock )
            {
                indicators.Remove( indicator );
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns Busy state shared by all indicator instances or null if the state is not the same for all indicators.
        /// </summary>
        public bool? Busy
        {
            get
            {
                lock ( hashLock )
                {
                    bool? val = null;
                    foreach ( var indicator in indicators )
                    {
                        if ( val == null )
                        {
                            val = indicator.Busy;
                        }
                        else
                        {
                            if ( val != indicator.Busy )
                            {
                                return null;
                            }
                        }
                    }
                    return val;
                }
            }
        }

        /// <summary>
        /// Returns Loaded state shared by all indicator instances or null if the state is not the same for all indicators.
        /// </summary>
        public bool? Loaded
        {
            get
            {
                lock ( hashLock )
                {
                    bool? val = null;
                    foreach ( var indicator in indicators )
                    {
                        if ( val == null )
                        {
                            val = indicator.Loaded;
                        }
                        else
                        {
                            if ( val != indicator.Loaded )
                            {
                                return null;
                            }
                        }
                    }
                    return val;
                }
            }
        }

        #endregion
    }
}