#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the UI page progress.
    /// </summary>
    public class PageProgressEventArgs : EventArgs
    {
        /// <summary>
        /// A default <see cref="PageProgressEventArgs"/> constructor.
        /// </summary>
        /// <param name="percentage">Total percentage of the progress.</param>
        /// <param name="options">Additional options to override default progress settings.</param>
        public PageProgressEventArgs( int? percentage, PageProgressOptions options )
        {
            Percentage = percentage;
            Options = options;
        }

        /// <summary>
        /// Gets the value in range 0-100 that tells us how much the progress has finished.
        /// </summary>
        public int? Percentage { get; }

        /// <summary>
        /// Gets the options that will override default progress settings.
        /// </summary>
        public PageProgressOptions Options { get; }
    }
}
