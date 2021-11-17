#region Using directives
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

        /// <summary>
        /// Default constructor for <see cref="CloseActivatorAdapter"/>.
        /// </summary>
        /// <param name="component">Reference to the close activator.</param>
        public CloseActivatorAdapter( ICloseActivator component )
        {
            this.component = component;
        }

        /// <summary>
        /// Gets the closable element id.
        /// </summary>
        /// <returns>A closable element id.</returns>
        [JSInvokable()]
        public string GetElementId()
        {
            return component.ElementId;
        }

        /// <summary>
        /// Checks if the closable component is safe to close.
        /// </summary>
        /// <param name="elementId">Element id that has trigger close.</param>
        /// <param name="reason">Reason for closing.</param>
        /// <param name="isChildClicked">True if child element was clicked.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [JSInvokable()]
        public Task<bool> SafeToClose( string elementId, string reason, bool isChildClicked )
        {
            return component.IsSafeToClose( elementId, GetCloseReason( reason ), isChildClicked );
        }

        /// <summary>
        /// Closes the closable component.
        /// </summary>
        /// <param name="reason">Reason for closing.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [JSInvokable()]
        public Task Close( string reason )
        {
            return component.Close( GetCloseReason( reason ) );
        }

        /// <summary>
        /// Gets the <see cref="CloseReason"/> based on a name.
        /// </summary>
        /// <param name="reason">Close reason name.</param>
        /// <returns>Returns a close reason.</returns>
        private static CloseReason GetCloseReason( string reason )
        {
            return reason switch
            {
                "leave" => CloseReason.FocusLostClosing,
                "escape" => CloseReason.EscapeClosing,
                _ => CloseReason.None,
            };
        }
    }
}
