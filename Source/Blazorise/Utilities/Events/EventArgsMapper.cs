#region Using directives
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Utilities;

internal static class EventArgsMapper
{
    public static BLMouseEventArgs ToMouseEventArgs( MouseEventArgs e )
    {
        if ( e is null )
            return null;

        return new( ToMouseButton( e.Button ),
            e.Detail,
            new( (int)e.ScreenX, (int)e.ScreenY ),
            new( (int)e.ClientX, (int)e.ClientY ),
            e.CtrlKey, e.ShiftKey, e.AltKey, e.MetaKey );
    }

    private static MouseButton ToMouseButton( long button )
    {
        return button switch
        {
            1 => MouseButton.Middle,
            2 => MouseButton.Right,
            _ => MouseButton.Left,
        };
    }
}