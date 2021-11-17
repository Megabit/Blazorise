#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <inheritdoc/>
    class PageProgressService : IPageProgressService
    {
        /// <inheritdoc/>
        public event EventHandler<PageProgressEventArgs> ProgressChanged;

        /// <inheritdoc/>
        public Task Go( int? percentage, Action<PageProgressOptions> options = null )
        {
            var pageProgressOptions = PageProgressOptions.Default;
            options?.Invoke( pageProgressOptions );

            ProgressChanged?.Invoke( this, new( percentage, pageProgressOptions ) );

            return Task.CompletedTask;
        }
    }
}
