#region Using directives
using System;
using System.Timers;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Timer that will execute an event after it elapsed.
/// </summary>
public class CountdownTimer : IDisposable
{
    #region Members

    /// <summary>
    /// Saved the timer interval when it was first initialized.
    /// </summary>
    private readonly double interval;

    /// <summary>
    /// Internal timer used to delay the value.
    /// </summary>
    private readonly System.Timers.Timer timer;

    /// <summary>
    /// Event raised after the interval has passed.
    /// </summary>
    public event EventHandler Elapsed;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="interval">Interval by which the timer will countdown.</param>
    public CountdownTimer( double interval )
    {
        this.interval = interval;

        timer = new( interval );
        timer.Elapsed += OnElapsed;
        timer.AutoReset = false;
    }

    #endregion

    #region Events

    private void OnElapsed( object source, ElapsedEventArgs e )
    {
        Elapsed?.Invoke( this, EventArgs.Empty );

        // reset timer in case we modified it by Delay
        timer.Interval = interval;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Starts the countdown.
    /// </summary>
    public void Start()
    {
        timer.Interval = interval;

        timer.Start();
    }

    /// <summary>
    /// Delays the timer.
    /// </summary>
    /// <param name="interval">Interval by which the timer will be delayed.</param>
    public void Delay( double interval )
    {
        timer.Stop();

        timer.Interval += interval;

        timer.Start();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose( true );
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="CountdownTimer"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True if the component is in the disposing process.</param>
    protected virtual void Dispose( bool disposing )
    {
        if ( !Disposed )
        {
            if ( timer != null )
            {
                timer.Stop();
                timer.Elapsed -= OnElapsed;
            }

            Disposed = true;
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Flag that indicates if the component is already fully disposed.
    /// </summary>
    protected bool Disposed { get; private set; }
    public double Duration { get; set; }

    #endregion
}