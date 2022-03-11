#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// The focus trap component allows to restrict TAB key navigation inside the component.
    /// </summary>
    public partial class FocusTrap : BaseComponent
    {
        #region Members

        private ElementReference containerRef;

        private ElementReference startFirstRef;

        private ElementReference startSecondRef;

        private ElementReference endFirstRef;

        private ElementReference endSecondRef;

        private bool shiftTabPressed;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                await startFirstRef.FocusAsync();
            }
        }

        /// <summary>
        /// Sets the focus trap.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetFocus()
            => await startFirstRef.FocusAsync();

        /// <summary>
        /// Handles the focus start event.
        /// </summary>
        /// <param name="args">Supplies information about a focus event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual async Task OnFocusStartHandler( FocusEventArgs args )
        {
            if ( !shiftTabPressed )
            {
                await startFirstRef.FocusAsync();
            }
        }

        /// <summary>
        /// Handles the focus end event.
        /// </summary>
        /// <param name="args">Supplies information about a focus event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual async Task OnFocusEndHandler( FocusEventArgs args )
        {
            if ( shiftTabPressed )
            {
                await endFirstRef.FocusAsync();
            }
        }

        /// <summary>
        /// Handles the focus start event.
        /// </summary>
        /// <param name="args">Supplies information about a keyboard event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual void OnKeyPressesHandler( KeyboardEventArgs args )
        {
            if ( args.Key == "Tab" )
            {
                shiftTabPressed = args.ShiftKey;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the focusable element tab index.
        /// </summary>
        protected int FocusableTabIndex => Active ? 0 : -1;

        /// <summary>
        /// If true the TAB focus will be activated.
        /// </summary>
        [Parameter] public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="FocusTrap"/> component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
