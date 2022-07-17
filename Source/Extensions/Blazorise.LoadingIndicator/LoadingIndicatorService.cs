using System.Collections.Generic;
using System.Threading;

namespace Blazorise.LoadingIndicator
{
    /// <summary>
    /// A service to control LoadingIndicator components
    /// </summary>
    public class LoadingIndicatorService
    {
        #region Members

        private object hashLock = new();
        private HashSet<LoadingIndicator> indicators = new();

        #endregion

        #region Methods

        /// <summary>
        /// Set Busy state
        /// </summary>
        /// <param name="val">true or false</param>
        public void SetBusy( bool val )
        {
            lock ( hashLock )
            {
                foreach ( var indicator in indicators )
                {
                    indicator.SetBusy( val );
                }
            }
        }

        /// <summary>
        /// Set Loaded state
        /// </summary>
        /// <param name="val">true or false</param>
        public void SetLoaded( bool val )
        {
            lock ( hashLock )
            {
                foreach ( var indicator in indicators )
                {
                    indicator.SetLoaded( val );
                }
            }
        }

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
        }

        /// <summary>
        /// Unsubscribe indicator from change events and remove reference
        /// </summary>
        /// <param name="indicator"></param>
        internal void Unsubscribe( LoadingIndicator indicator )
        {
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
                bool? val = null;
                lock ( hashLock )
                {
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
                }
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
                bool? val = null;
                lock ( hashLock )
                {
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
                }
                return val;
            }
        }

        #endregion
    }
}