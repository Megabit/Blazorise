#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise.Bulma
{
    public class ModalContent : Blazorise.ModalContent
    {
        #region Methods

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender && AsDialog )
            {
                await InvokeAsync( StateHasChanged );
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        #endregion

        #region Properties

        protected override bool AsDialog
            => ( HasModalHeader || HasModalBody || HasModalFooter ) && Scrollable;

        #endregion
    }
}
