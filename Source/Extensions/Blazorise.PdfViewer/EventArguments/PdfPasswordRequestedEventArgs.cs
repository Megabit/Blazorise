#region Using directives
using System;
#endregion

namespace Blazorise.PdfViewer;

/// <summary>
/// Provides data when a password is requested for opening a protected PDF document.
/// </summary>
public class PdfPasswordRequestedEventArgs : EventArgs
{
    #region Constructors

    /// <summary>
    /// A default <see cref="PdfPasswordRequestedEventArgs"/> constructor.
    /// </summary>
    /// <param name="reason">The reason why the password is being requested.</param>
    /// <param name="attempt">The number of attempts made so far.</param>
    /// <param name="source">The source of the PDF currently being loaded.</param>
    public PdfPasswordRequestedEventArgs( PdfPasswordRequestReason reason, int attempt, string source )
    {
        Reason = reason;
        Attempt = attempt;
        Source = source;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the reason why the password is being requested.
    /// </summary>
    public PdfPasswordRequestReason Reason { get; }

    /// <summary>
    /// Gets the number of password attempts made so far.
    /// </summary>
    public int Attempt { get; }

    /// <summary>
    /// Gets the source of the protected PDF document.
    /// </summary>
    public string Source { get; }

    #endregion
}