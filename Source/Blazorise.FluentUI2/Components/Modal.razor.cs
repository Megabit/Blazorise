using Blazorise.Utilities;

namespace Blazorise.FluentUI2.Components;

public partial class Modal
{
    #region Members

    private ModalSize modalSize;

    private bool centered;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ModalSize( modalSize ) );
        builder.Append( ClassProvider.ModalCentered( centered ) );

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

    protected internal void NotifyModalCenteredChanged( bool centered )
    {
        this.centered = centered;

        ExecuteAfterRender( async () =>
        {
            DirtyClasses();
            await InvokeAsync( StateHasChanged );
        } );
    }

    #endregion
}
