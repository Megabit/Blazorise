using System;
using System.Collections.Generic;
using System.Linq;

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

        // avoid locking in single indicator (app busy) scenario
        Action<bool> SetBusyAction;
        Action<bool> SetLoadedAction;
        Func<bool?> GetBusyFunc;
        Func<bool?> GetLoadedFunc;

        #endregion

        #region Methods

        /// <inheritdoc />
        public LoadingIndicatorService()
        {
            // default to foreach implementation
            MultiMode();
        }

        // use locking implementation
        private void MultiMode()
        {
            SetBusyAction = SetBusyMulti;
            SetLoadedAction = SetLoadedMulti;
            GetBusyFunc = GetBusyMulti;
            GetLoadedFunc = GetLoadedMulti;
        }

        // no lock implementation
        private void SingleMode( LoadingIndicator indicator )
        {
            SetBusyAction = indicator.SetBusy;
            SetLoadedAction = indicator.SetLoaded;
            GetBusyFunc = () => indicator.Busy;
            GetLoadedFunc = () => indicator.Loaded;
        }

        /// <summary>
        /// Set Busy state
        /// </summary>
        /// <param name="value">true or false</param>
        public void SetBusy( bool value ) => SetBusyAction( value );

        /// <summary>
        /// Set Loaded state
        /// </summary>
        /// <param name="value">true or false</param>
        public void SetLoaded( bool value ) => SetLoadedAction( value );

        /// <summary>
        /// Subscribe indicator to change events and save reference
        /// </summary>
        /// <param name="indicator"></param>
        internal void Subscribe( LoadingIndicator indicator )
        {
            lock ( hashLock )
            {
                if ( indicators.Count == 0 )
                {
                    SingleMode( indicator );
                }
                else if ( indicators.Count == 1 )
                {
                    MultiMode();
                }

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

                if ( indicators.Count == 0 )
                {
                    MultiMode();
                }
                else if ( indicators.Count == 1 )
                {
                    SingleMode( indicators.First() );
                }
            }
        }

        private void SetBusyMulti( bool value )
        {
            lock ( hashLock )
            {
                foreach ( var indicator in indicators )
                {
                    indicator.SetBusy( value );
                }
            }
        }

        private void SetLoadedMulti( bool value )
        {
            lock ( hashLock )
            {
                foreach ( var indicator in indicators )
                {
                    indicator.SetLoaded( value );
                }
            }
        }

        private bool? GetBusyMulti()
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

        private bool? GetLoadedMulti()
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

        #endregion

        #region Properties

        /// <summary>
        /// Returns Busy state shared by all indicator instances or null if the state is not the same for all indicators.
        /// </summary>
        public bool? Busy => GetBusyFunc();

        /// <summary>
        /// Returns Loaded state shared by all indicator instances or null if the state is not the same for all indicators.
        /// </summary>
        public bool? Loaded => GetLoadedFunc();

        #endregion
    }
}