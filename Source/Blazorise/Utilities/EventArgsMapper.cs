#region Using directives
using System.Drawing;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Utilities
{
    internal static class EventArgsMapper
    {
        public static BLMouseEventArgs ToMouseEventArgs( MouseEventArgs e )
        {
            if ( e == null )
                return null;

            return new BLMouseEventArgs( ToMouseButton( e.Button ),
                e.Detail,
                new Point( (int)e.ScreenX, (int)e.ScreenY ),
                new Point( (int)e.ClientX, (int)e.ClientY ),
                e.CtrlKey, e.ShiftKey, e.AltKey );
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
}
