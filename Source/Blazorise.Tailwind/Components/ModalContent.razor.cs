#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Tailwind.Components;

public partial class ModalContent : Blazorise.ModalContent
{
    #region Members

    #endregion

    #region Constructors

    public ModalContent()
    {
        DialogClassBuilder = new( BuildDialogClasses );
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
        builder.Append( "relative w-full h-full" );

        builder.Append( ClassProvider.ToModalSize( Size ) );

        if ( Size == ModalSize.Fullscreen )
            builder.Append( "modal-fullscreen h-screen w-screen max-w-none h-full m-0" );

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