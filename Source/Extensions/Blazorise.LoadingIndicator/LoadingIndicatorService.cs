using System.Collections.Generic;
using System.Threading;

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

        private ReaderWriterLockSlim hashLock = new();
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
            hashLock.EnterWriteLock();
            try
            {
                indicators.Add( indicator );
                BusyChanged += indicator.Service_BusyChanged;
                LoadedChanged += indicator.Service_LoadedChanged;
            }
            finally
            {
                hashLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Unsubscribe indicator from change events and remove reference
        /// </summary>
        /// <param name="indicator"></param>
        internal void Unsubscribe( LoadingIndicator indicator )
        {
            hashLock.EnterWriteLock();
            try
            { 
                BusyChanged -= indicator.Service_BusyChanged;
                LoadedChanged -= indicator.Service_LoadedChanged;
                indicators.Remove( indicator );
            }
            finally
            {
                hashLock.ExitWriteLock();
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
                hashLock.EnterReadLock();
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
                hashLock.ExitReadLock();
                return val;
            }
        }

        /// <summary>
        /// Returns Loaded state shared by all indicator instances or null if the state is not the same for all indicators.
        /// </summary>
        public bool? Loaded
        {
            get
            {
                hashLock.EnterReadLock();
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
                hashLock.ExitReadLock();
                return val;
            }
        }

        #endregion
    }
}