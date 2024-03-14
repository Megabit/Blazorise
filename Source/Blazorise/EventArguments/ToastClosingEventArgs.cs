#region Using directives
using System.ComponentModel;
#endregion

namespace Blazorise;

/// <summary>
/// Provides data for the <see cref="Toast.Closing"/> event.
/// </summary>
public class ToastClosingEventArgs : CancelEventArgs
{
    /// <summary>
    /// A default <see cref="ToastClosingEventArgs"/> constructor.
    /// </summary>
    /// <param name="cancel">True if close event should be cancelled.</param>
    /// <param name="closeReason">Reason for closing.</param>
    public ToastClosingEventArgs( bool cancel, CloseReason closeReason )
        : base( cancel )
    {
        CloseReason = closeReason;
    }

    /// <summary>
    /// Gets a value that indicates why the toast is being closed.
    /// </summary>
    public CloseReason CloseReason { get; }
}