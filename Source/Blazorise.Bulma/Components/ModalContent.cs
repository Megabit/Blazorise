#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
#endregion

namespace Blazorise.Bulma.Components;

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

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( Size == ModalSize.Fullscreen && !AsDialog )
        {
            builder.Append( "modal-card" );
        }

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    protected override bool AsDialog
        => ( HasModalHeader || HasModalBody || HasModalFooter ) && Scrollable;

    #endregion
}