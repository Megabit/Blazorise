#region Using directives
using System;
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class ModalContent : Blazorise.ModalContent, IDisposable
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

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ParentModal.NotifyCloseActivatorIdInitialized( WrapperElementId ??= IdGenerator.Generate );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentModal?.NotifyCloseActivatorIdRemoved( WrapperElementId );
        }

        base.Dispose( disposing );
    }

    protected internal override void DirtyClasses()
    {
        DialogClassBuilder.Dirty();

        base.DirtyClasses();
    }

    private void BuildDialogClasses( ClassBuilder builder )
    {
        builder.Append( "ant-modal" );
        builder.Append( $"ant-modal-{ClassProvider.ToModalSize( Size )}" );

        if ( Size == ModalSize.Fullscreen )
        {
            builder.Append( "ant-modal-fullscreen" );
        }
    }

    #endregion

    #region Properties

    protected string WrapperElementId { get; set; }

    protected ClassBuilder DialogClassBuilder { get; private set; }

    /// <summary>
    /// Gets dialog container class-names.
    /// </summary>
    protected string DialogClassNames => DialogClassBuilder.Class;

    #endregion
}