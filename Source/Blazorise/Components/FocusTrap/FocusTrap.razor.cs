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

        private bool _shiftPressed;

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

        internal async Task SetFocus()
            => await startFirstRef.FocusAsync();

        private async Task FocusStartAsync( FocusEventArgs args )
        {
            if ( !_shiftPressed )
            {
                await startFirstRef.FocusAsync();
            }
        }

        private async Task FocusEndAsync( FocusEventArgs args )
        {
            if ( _shiftPressed )
            {
                await endFirstRef.FocusAsync();
            }
        }

        private void HandleKeyPresses( KeyboardEventArgs args )
        {
            if ( args.Key == "Tab" )
            {
                _shiftPressed = args.ShiftKey;
            }
        }

        #endregion

        #region Properties

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
