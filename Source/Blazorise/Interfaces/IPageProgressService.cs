#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Service to show a page progress.
    /// </summary>
    public interface IPageProgressService
    {
        /// <summary>
        /// An event raised after the notification is received.
        /// </summary>
        public event EventHandler<PageProgressEventArgs> ProgressChanged;

        /// <summary>
        /// Sets the progress percentage.
        /// </summary>
        /// <param name="percentage">Value of the progress from 0 to 100, or null for indeterminate progress.</param>
        /// <param name="options">Additional options.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Go( int? percentage, Action<PageProgressOptions> options = null );
    }
}
