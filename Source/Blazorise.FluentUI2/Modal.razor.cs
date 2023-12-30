using Blazorise.Utilities;

namespace Blazorise.FluentUI2;

public partial class Modal
{
    #region Members

    private ModalSize modalSize;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ModalSize( modalSize ) );

        base.BuildClasses( builder );
    }

    protected internal void NotifyModalSizeChanged( ModalSize modalSize )
    {
        this.modalSize = modalSize;

        ExecuteAfterRender( async () =>
        {
            DirtyClasses();
            await InvokeAsync( StateHasChanged );
        } );
    }

    #endregion
}
