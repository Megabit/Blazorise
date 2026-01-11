#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Bootstrap5.Components;

public partial class ModalContent : Blazorise.ModalContent
{
    #region Members

    #endregion

    #region Constructors

    public ModalContent()
    {
        DialogClassBuilder = new( BuildDialogClasses, builder => builder.Append( Classes?.Dialog ) );
    }

    #endregion

    #region Methods

    protected internal override void DirtyClasses()
    {
        DialogClassBuilder.Dirty();

        base.DirtyClasses();
    }

    private void BuildDialogClasses( ClassBuilder builder )
    {
        builder.Append( "modal-dialog" );

        if ( Size == ModalSize.Fullscreen )
            builder.Append( "modal-fullscreen" );
        else if ( Size != ModalSize.Default )
            builder.Append( $"modal-{ClassProvider.ToModalSize( Size )}" );

        builder.Append( ClassProvider.ModalContentCentered( Centered ) );

        if ( Centered )
            builder.Append( "modal-dialog-centered" );

        if ( Scrollable )
            builder.Append( "modal-dialog-scrollable" );
    }

    #endregion

    #region Properties

    protected ClassBuilder DialogClassBuilder { get; private set; }

    /// <summary>
    /// Gets dialog container class-names.
    /// </summary>
    protected string DialogClassNames => DialogClassBuilder.Class;

    #endregion
}