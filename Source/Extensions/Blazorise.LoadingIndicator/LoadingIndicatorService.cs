#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.LoadingIndicator
{
    /// <summary>
    /// A service to control LoadingIndicator components
    /// </summary>
    public class LoadingIndicatorService: ILoadingIndicatorService
    {
        #region Members

        private object hashLock = new();
        private HashSet<LoadingIndicator> indicators = new();

        // avoid locking in single indicator (app busy) scenario
        Func<bool, Task> SetBusyFunc;
        Func<bool, Task> SetLoadedFunc;
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
            SetBusyFunc = SetBusyMulti;
            SetLoadedFunc = SetLoadedMulti;
            GetBusyFunc = GetBusyMulti;
            GetLoadedFunc = GetLoadedMulti;
        }

        // no lock implementation
        private void SingleMode( LoadingIndicator indicator )
        {
            SetBusyFunc = indicator.SetBusy;
            SetLoadedFunc = indicator.SetLoaded;
            GetBusyFunc = () => indicator.Busy;
            GetLoadedFunc = () => indicator.Loaded;
        }

        /// <summary>
        /// Show loading indicator
        /// </summary>
        public Task Show() => SetBusyFunc( true );

        /// <summary>
        /// Hide loading indicator
        /// </summary>
        public Task Hide() => SetBusyFunc( false );

        /// <summary>
        /// Set Loaded state
        /// </summary>
        /// <param name="value">true or false</param>
        public Task SetLoaded( bool value ) => SetLoadedFunc( value );

        /// <summary>
        /// Subscribe indicator to change events and save reference
        /// </summary>
        /// <param name="indicator"></param>
        void ILoadingIndicatorService.Subscribe( LoadingIndicator indicator )
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
        void ILoadingIndicatorService.Unsubscribe( LoadingIndicator indicator )
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

        private Task SetBusyMulti( bool value )
        {
            lock ( hashLock )
            {
                foreach ( var indicator in indicators )
                {
                    // CS1996: cannot await in the body of a lock statement
                    _ = indicator.SetBusy( value );
                }                
                return Task.CompletedTask;
            }
        }

        private Task SetLoadedMulti( bool value )
        {
            lock ( hashLock )
            {
                foreach ( var indicator in indicators )
                {
                    // CS1996: cannot await in the body of a lock statement
                    _ = indicator.SetLoaded( value );
                }
                return Task.CompletedTask;
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