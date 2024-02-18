#region Using directives
using System.ComponentModel;
#endregion

namespace Blazorise;

/// <summary>
/// Provides data for the <see cref="Toast.Opening"/> event.
/// </summary>
public class ToastOpeningEventArgs : CancelEventArgs
{
    /// <summary>
    /// A default <see cref="ToastOpeningEventArgs"/> constructor.
    /// </summary>
    /// <param name="cancel">True if close event should be cancelled.</param>
    public ToastOpeningEventArgs( bool cancel )
        : base( cancel )
    {
    }
}