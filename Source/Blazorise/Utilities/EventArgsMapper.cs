#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Microsoft.AspNetCore.Components;
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
            switch ( button )
            {
                case 1:
                    return MouseButton.Middle;
                case 2:
                    return MouseButton.Right;
                default:
                    return MouseButton.Left;
            }
        }
    }
}
