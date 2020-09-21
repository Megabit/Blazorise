#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Middleman between the closable component and javascript.
    /// </summary>
    public class CloseActivatorAdapter
    {
        private readonly ICloseActivator component;

        public CloseActivatorAdapter( ICloseActivator component )
        {
            this.component = component;
        }

        [JSInvokable()]
        public string GetElementId()
        {
            return component.ElementId;
        }

        [JSInvokable()]
        public Task<bool> SafeToClose( string elementId, string reason, bool isChildClicked )
        {
            return component.IsSafeToClose( elementId, GetCloseReason( reason ), isChildClicked );
        }

        [JSInvokable()]
        public Task Close( string reason )
        {
            return component.Close( GetCloseReason( reason ) );
        }

        private static CloseReason GetCloseReason( string reason )
        {
            switch ( reason )
            {
                case "leave":
                    return CloseReason.FocusLostClosing;
                case "escape":
                    return CloseReason.EscapeClosing;
                default:
                    return CloseReason.None;
            }
        }
    }
}
