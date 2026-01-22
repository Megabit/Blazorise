#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.LoadingIndicator;

/// <inheritdoc />
public class LoadingIndicatorService : ILoadingIndicatorService
{
    #region Members

    private object hashLock = new();
    private HashSet<LoadingIndicator> indicators = new();

    // avoid locking in single indicator (app busy) scenario
    Func<bool, Task> SetVisibleFunc;
    Func<bool, Task> SetInitializingFunc;
    Func<LoadingIndicatorStatus, Task> SetStatusFunc;
    Func<bool?> GetVisibleFunc;
    Func<bool?> GetInitializingFunc;
    Func<LoadingIndicatorStatus> GetStatusFunc;

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
        SetInitializingFunc = SetInitializingMulti;
        SetStatusFunc = SetStatusMulti;
        GetVisibleFunc = GetVisibleMulti;
        GetInitializingFunc = GetInitializingMulti;
        GetStatusFunc = GetStatusMulti;
    }

    // no lock implementation
    private void SingleMode( LoadingIndicator indicator )
    {
        SetVisibleFunc = indicator.SetVisible;
        SetInitializingFunc = indicator.SetInitializing;
        SetStatusFunc = indicator.SetStatus;
        GetVisibleFunc = () => indicator.Visible;
        GetInitializingFunc = () => indicator.Initializing;
        GetStatusFunc = () => indicator.Status;
    }

    /// <inheritdoc/>
    public Task Show() => SetVisibleFunc( true );

    /// <inheritdoc/>
    public Task Hide() => SetVisibleFunc( false );

    /// <inheritdoc/>
    public Task SetInitializing( bool value ) => SetInitializingFunc( value );

    /// <inheritdoc/>
    public Task SetStatus( LoadingIndicatorStatus status ) => SetStatusFunc( status );

    /// <inheritdoc/>
    public Task SetStatus( string text = null, int? progress = null )
    {
        LoadingIndicatorStatus status = new LoadingIndicatorStatus( text, progress );

        return SetStatusFunc( status );
    }

    /// <summary>
    /// Subscribes an indicator to shared state updates and tracks it internally.
    /// </summary>
    /// <param name="indicator">The indicator to subscribe.</param>
    void ILoadingIndicatorService.Subscribe( LoadingIndicator indicator )
    {
        lock ( hashLock )
        {
            indicators.Add( indicator );

            if ( indicators.Count == 1 )
            {
                SingleMode( indicator );
            }
            else
            {
                MultiMode();
            }
        }
    }

    /// <summary>
    /// Unsubscribes an indicator from shared state updates and removes it from tracking.
    /// </summary>
    /// <param name="indicator">The indicator to unsubscribe.</param>
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
            tasks = new( indicators.Count );
            foreach ( var indicator in indicators )
            {
                tasks.Add( indicator.SetVisible( value ) );
            }
        }
        return Task.WhenAll( tasks );
    }

    private Task SetInitializingMulti( bool value )
    {
        List<Task> tasks;
        lock ( hashLock )
        {
            tasks = new( indicators.Count );
            foreach ( var indicator in indicators )
            {
                tasks.Add( indicator.SetInitializing( value ) );
            }
        }
        return Task.WhenAll( tasks );
    }

    private Task SetStatusMulti( LoadingIndicatorStatus status )
    {
        List<Task> tasks;
        LoadingIndicatorStatus nextStatus = status ?? LoadingIndicatorStatus.Empty;

        lock ( hashLock )
        {
            tasks = new( indicators.Count );

            foreach ( var indicator in indicators )
            {
                tasks.Add( indicator.SetStatus( nextStatus ) );
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

    private bool? GetInitializingMulti()
    {
        bool? val = null;
        lock ( hashLock )
        {
            foreach ( var indicator in indicators )
            {
                if ( val == null )
                {
                    val = indicator.Initializing;
                }
                else
                {
                    if ( val != indicator.Initializing )
                    {
                        return null;
                    }
                }
            }
        }
        return val;
    }

    private LoadingIndicatorStatus GetStatusMulti()
    {
        LoadingIndicatorStatus val = null;

        lock ( hashLock )
        {
            foreach ( var indicator in indicators )
            {
                if ( val == null )
                {
                    val = indicator.Status;
                }
                else
                {
                    if ( val != indicator.Status )
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
    public bool? Initializing => GetInitializingFunc();

    /// <inheritdoc/>
    public LoadingIndicatorStatus Status => GetStatusFunc();

    #endregion
}