#region Using directives
using System;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Represents a countdown timer that can be extended, paused, and resumed, and allows for actions to be taken on each tick and upon completion.
/// </summary>
public class AsyncCountdownTimer : IDisposable
{
    #region Members

    private PeriodicTimer periodicTimer;

    private readonly int ticksToTimeout;

    private readonly CancellationToken cancellationToken;
    private double percentComplete;
    private bool paused;
    private Func<double, Task> tickDelegate;
    private Func<Task> elapsedDelegate;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CountdownTimer"/> class.
    /// </summary>
    /// <param name="timeout">The initial timeout duration in milliseconds.</param>    
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the timer. Defaults to an empty token.</param>
    public AsyncCountdownTimer( double timeout, CancellationToken cancellationToken = default )
    {
        ticksToTimeout = 100;
        periodicTimer = new PeriodicTimer( TimeSpan.FromMilliseconds( timeout / ticksToTimeout ) );
        this.cancellationToken = cancellationToken;
    }

    /// <summary>
    /// <see cref="AsyncCountdownTimer"/> finalizer.
    /// </summary>
    ~AsyncCountdownTimer()
    {
        Dispose( false );
    }

    /// <summary>
    /// Releases all resources used by the <see cref="CountdownTimer"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose( true );
        GC.SuppressFinalize( this );
    }

    /// <summary>
    /// Releases all resources used by the <see cref="CountdownTimer"/>.
    /// </summary>
    protected virtual void Dispose( bool disposing )
    {
        if ( disposing )
        {
            periodicTimer?.Dispose();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sets the delegate to be invoked on each tick of the timer.
    /// </summary>
    /// <param name="updateProgressDelegate">The delegate to invoke on each tick, which receives the percent complete as an argument.</param>
    /// <returns>The <see cref="CountdownTimer"/> instance for chaining.</returns>
    public AsyncCountdownTimer Ticked( Func<double, Task> updateProgressDelegate )
    {
        tickDelegate = updateProgressDelegate;
        return this;
    }

    /// <summary>
    /// Sets the delegate to be invoked when the timer elapses.
    /// </summary>
    /// <param name="elapsedDelegate">The action to perform when the timer elapses.</param>
    /// <returns>The <see cref="CountdownTimer"/> instance for chaining.</returns>
    public AsyncCountdownTimer Elapsed( Func<Task> elapsedDelegate )
    {
        this.elapsedDelegate = elapsedDelegate;
        return this;
    }

    /// <summary>
    /// Starts the countdown timer.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    public async Task Start()
    {
        percentComplete = 0;
        await DoWorkAsync();
    }

    /// <summary>
    /// Pauses the countdown timer.
    /// </summary>
    public void Pause()
    {
        paused = true;
    }

    /// <summary>
    /// Resumes the countdown timer, optionally using an extended timeout.
    /// </summary>
    /// <param name="extendedTimeout">The extended timeout duration in milliseconds. Defaults to 0.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation of resuming the timer.</returns>
    public async Task Resume( double extendedTimeout = 0 )
    {
        paused = false;

        if ( extendedTimeout > 0 )
        {
            periodicTimer?.Dispose();
            periodicTimer = new PeriodicTimer( TimeSpan.FromMilliseconds( extendedTimeout / ticksToTimeout ) );
        }

        await Start();
    }

    /// <summary>
    /// Performs the timer's work, waiting for ticks, updating progress, and invoking delegates appropriately.
    /// </summary>
    private async Task DoWorkAsync()
    {
        while ( await periodicTimer.WaitForNextTickAsync( cancellationToken ) && !cancellationToken.IsCancellationRequested )
        {
            if ( !paused )
            {
                percentComplete++;
            }

            tickDelegate?.Invoke( percentComplete );

            if ( percentComplete >= ticksToTimeout )
            {
                elapsedDelegate?.Invoke();
                break;
            }
        }
    }

    #endregion
}
