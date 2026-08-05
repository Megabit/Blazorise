#region Using directives
using System;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Defines HTTP PDF resource policies.
/// </summary>
public sealed class PdfHttpResourceOptions
{
    #region Methods

    internal void Validate()
    {
        if ( MaxResourceSize <= 0 || MaxResourceSize > int.MaxValue )
            throw new ArgumentOutOfRangeException( nameof( MaxResourceSize ), $"The maximum PDF HTTP resource size must be between 1 and {int.MaxValue} bytes." );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Maximum number of bytes allowed for one resource. Defaults to 20 MB.
    /// </summary>
    public long MaxResourceSize { get; set; } = PdfGenerationOptions.DefaultMaxResourceSize;

    /// <summary>
    /// Determines whether an absolute HTTP resource URI is allowed. A null value allows every HTTP and HTTPS URI.
    /// </summary>
    public Func<Uri, bool> ResourceAllowed { get; set; }

    #endregion
}