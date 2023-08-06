#region Using directives
using System.ComponentModel;
#endregion

namespace Blazorise;

/// <summary>
/// Provides data for the <see cref="Offcanvas.Opening"/> event.
/// </summary>
public class OffcanvasOpeningEventArgs : CancelEventArgs
{
    /// <summary>
    /// A default <see cref="OffcanvasOpeningEventArgs"/> constructor.
    /// </summary>
    /// <param name="cancel">True if close event should be cancelled.</param>
    public OffcanvasOpeningEventArgs( bool cancel )
        : base( cancel )
    {
    }
}