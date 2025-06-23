#region Using directives
using System;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Utility class for throttling the execution of asynchronous tasks.
/// </summary>
public class ThrottleDispatcher
{
    #region Members

    private readonly int interval;
    private readonly bool delayAfterExecution;
    private readonly bool resetIntervalOnException;

    private readonly object locker = new();

    private bool isBusy;
    private Task lastTask;
    private DateTime? lastInvokeTime;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrottleDispatcher"/> class.
    /// </summary>
    /// <param name="interval">Minimum interval in milliseconds between invocations.</param>
    /// <param name="delayAfterExecution">If true, interval starts after task completion; otherwise, at task start.</param>
    /// <param name="resetIntervalOnException">If true, resets interval on task failure.</param>
    public ThrottleDispatcher( int interval, bool delayAfterExecution = false, bool resetIntervalOnException = false )
    {
        this.interval = interval;
        this.delayAfterExecution = delayAfterExecution;
        this.resetIntervalOnException = resetIntervalOnException;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Throttles the execution of the provided asynchronous function.
    /// </summary>
    /// <param name="action">The function to execute.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The task representing the throttled execution.</returns>
    public Task ThrottleAsync( Func<Task> action, CancellationToken cancellationToken = default )
    {
        lock ( locker )
        {
            if ( lastTask is not null && ( isBusy || ShouldThrottle() ) )
                return lastTask;

            isBusy = true;
            lastInvokeTime = DateTime.UtcNow;

            lastTask = ExecuteAsync( action, cancellationToken );

            return lastTask;
        }
    }

    /// <summary>
    /// Determines whether the operation should be throttled based on the time elapsed since the last invocation.
    /// </summary>
    /// <returns><see langword="true"/> if the time elapsed since the last invocation is less than the specified interval; otherwise, <see langword="false"/>.</returns>
    private bool ShouldThrottle() =>
        lastInvokeTime.HasValue && ( DateTime.UtcNow - lastInvokeTime.Value ).TotalMilliseconds < interval;

    private async Task ExecuteAsync( Func<Task> action, CancellationToken cancellationToken )
    {
        try
        {
            await action.Invoke();

            if ( delayAfterExecution )
                lastInvokeTime = DateTime.UtcNow;
        }
        catch
        {
            if ( resetIntervalOnException )
            {
                lock ( locker )
                {
                    lastInvokeTime = null;
                    lastTask = null;
                }
            }

            throw;
        }
        finally
        {
            lock ( locker )
            {
                isBusy = false;
            }
        }
    }

    #endregion
}