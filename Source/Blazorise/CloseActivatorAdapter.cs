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
        public bool SafeToClose( string elementId )
        {
            return component.SafeToClose( elementId );
        }

        [JSInvokable()]
        public void Close()
        {
            component.Close();
        }
    }
}
