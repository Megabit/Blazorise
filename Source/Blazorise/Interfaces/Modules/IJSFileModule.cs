﻿#region Using directives
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Contracts for the reading a file JS module.
    /// </summary>
    public interface IJSFileModule : IBaseJSModule
    {
        /// <summary>
        /// Reads the batch of data for the specified position and offset.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="fileEntryId">File id.</param>
        /// <param name="position">Byte position starting from zero.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask<string> ReadDataAsync( ElementReference elementRef, int fileEntryId, long position, long length, CancellationToken cancellationToken = default );
    }
}
