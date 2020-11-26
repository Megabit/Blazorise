#region Using directives
using System;
using System.Timers;
#endregion

namespace Blazorise.Utils
{
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
        private Timer timer;

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

            timer = new Timer( interval );
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
        /// Deleys the timer.
        /// </summary>
        /// <param name="interval">Interval by which the timer will be delayed.</param>
        public void Delay( double interval )
        {
            timer.Stop();

            timer.Interval += interval;

            timer.Start();
        }

        /// <summary>
        /// Releases all subscribed events.
        /// </summary>
        public void Dispose()
        {
            if ( timer != null )
            {
                timer.Stop();
                timer.Elapsed -= OnElapsed;
                timer = null;
            }
        }

        #endregion
    }
}
