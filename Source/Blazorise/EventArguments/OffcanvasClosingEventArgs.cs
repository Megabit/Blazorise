#region Using directives
using System.ComponentModel;
#endregion

namespace Blazorise;

/// <summary>
/// Provides data for the <see cref="Offcanvas.Closing"/> event.
/// </summary>
public class OffcanvasClosingEventArgs : CancelEventArgs
{
    /// <summary>
    /// A default <see cref="OffcanvasClosingEventArgs"/> constructor.
    /// </summary>
    /// <param name="cancel">True if close event should be cancelled.</param>
    /// <param name="closeReason">Reason for closing.</param>
    public OffcanvasClosingEventArgs( bool cancel, CloseReason closeReason )
        : base( cancel )
    {
        CloseReason = closeReason;
    }

    /// <summary>
    /// Gets a value that indicates why the offcanvas is being closed.
    /// </summary>
    public CloseReason CloseReason { get; }
}