#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.LoadingIndicator
{
    /// <inheritdoc />
    public class LoadingIndicatorService: ILoadingIndicatorService
    {
        #region Members

        private object hashLock = new();
        private HashSet<LoadingIndicator> indicators = new();

        // avoid locking in single indicator (app busy) scenario
        Func<bool, Task> SetVisibleFunc;
        Func<bool, Task> SetLoadedFunc;
        Func<bool?> GetVisibleFunc;
        Func<bool?> GetLoadedFunc;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public LoadingIndicatorService()
        {
            // default to foreach implementation
            MultiMode();
        }

        // use locking implementation
        private void MultiMode()
        {
            SetVisibleFunc = SetVisibleMulti;
            SetLoadedFunc = SetLoadedMulti;
            GetVisibleFunc = GetVisibleMulti;
            GetLoadedFunc = GetLoadedMulti;
        }

        // no lock implementation
        private void SingleMode( LoadingIndicator indicator )
        {
            SetVisibleFunc = indicator.SetVisible;
            SetLoadedFunc = indicator.SetLoaded;
            GetVisibleFunc = () => indicator.Visible;
            GetLoadedFunc = () => indicator.Loaded;
        }

        /// <inheritdoc/>
        public Task Show() => SetVisibleFunc( true );

        /// <inheritdoc/>
        public Task Hide() => SetVisibleFunc( false );

        /// <inheritdoc/>
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

        private Task SetVisibleMulti( bool value )
        {
            List<Task> tasks;
            lock ( hashLock )
            {
                tasks = new(indicators.Count);
                foreach ( var indicator in indicators )
                {
                    tasks.Add( indicator.SetVisible( value ) );
                }
            }
            return Task.WhenAll( tasks );
        }

        private Task SetLoadedMulti( bool value )
        {
            List<Task> tasks;
            lock ( hashLock )
            {
                tasks = new( indicators.Count );
                foreach ( var indicator in indicators )
                {
                    tasks.Add( indicator.SetLoaded( value ) );
                }
            }
            return Task.WhenAll( tasks );
        }

        private bool? GetVisibleMulti()
        {
            bool? val = null;
            lock ( hashLock )
            {
                foreach ( var indicator in indicators )
                {
                    if ( val == null )
                    {
                        val = indicator.Visible;
                    }
                    else
                    {
                        if ( val != indicator.Visible )
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

        /// <inheritdoc/>
        public bool? Visible => GetVisibleFunc();

        /// <inheritdoc/>
        public bool? Loaded => GetLoadedFunc();

        #endregion
    }
}