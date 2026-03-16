#region Using directives
using System;
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class ModalContent : Blazorise.ModalContent, IDisposable
{
    #region Members

    private bool wrapperCentered;

    private bool wrapperScrollable;

    private ModalSize wrapperModalSize = ModalSize.Default;

    #endregion

    #region Constructors

    public ModalContent()
    {
        DialogClassBuilder = new( BuildDialogClasses, builder => builder.Append( Classes?.Dialog ) );
        DialogStyleBuilder = new( BuildDialogStyles, builder => builder.Append( Styles?.Dialog ) );
        WrapperClassBuilder = new( BuildWrapperClasses );
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
        WrapperClassBuilder.Dirty();

        base.DirtyClasses();
    }

    protected internal override void DirtyStyles()
    {
        DialogStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    protected override void OnParametersSet()
    {
        var parentCentered = ParentModal?.Centered ?? false;
        var parentScrollable = ParentModal?.Scrollable ?? false;
        var parentSize = ParentModal?.Size ?? ModalSize.Default;

        if ( wrapperCentered != parentCentered || wrapperScrollable != parentScrollable || wrapperModalSize != parentSize )
        {
            wrapperCentered = parentCentered;
            wrapperScrollable = parentScrollable;
            wrapperModalSize = parentSize;

            DirtyClasses();
            DirtyStyles();
        }

        base.OnParametersSet();
    }

    private void BuildDialogClasses( ClassBuilder builder )
    {
        builder.Append( "ant-modal" );

        var sizeClass = ClassProvider.ToModalSize( wrapperModalSize );

        if ( !string.IsNullOrEmpty( sizeClass ) )
        {
            builder.Append( $"ant-modal-{sizeClass}" );
        }

        if ( wrapperModalSize == ModalSize.Fullscreen )
        {
            builder.Append( "ant-modal-fullscreen" );
        }
    }

    private void BuildWrapperClasses( ClassBuilder builder )
    {
        builder.Append( "ant-modal-wrap" );

        if ( wrapperCentered )
        {
            builder.Append( "ant-modal-centered" );
        }
    }

    private void BuildDialogStyles( StyleBuilder builder )
    {
        if ( wrapperModalSize == ModalSize.Fullscreen )
        {
            return;
        }

        var width = wrapperModalSize switch
        {
            ModalSize.Small => "var(--ant-modal-sm-width)",
            ModalSize.Large => "var(--ant-modal-lg-width)",
            ModalSize.ExtraLarge => "var(--ant-modal-xl-width)",
            _ => "var(--ant-modal-md-width)",
        };

        builder.Append( $"width: {width}" );
        builder.Append( "max-width: calc(100vw - calc(var(--ant-margin) * 2))" );
    }

    #endregion

    #region Properties

    protected string WrapperElementId { get; set; }

    protected ClassBuilder WrapperClassBuilder { get; private set; }

    protected string WrapperClassNames => WrapperClassBuilder.Class;

    protected ClassBuilder DialogClassBuilder { get; private set; }

    /// <summary>
    /// Gets dialog container class-names.
    /// </summary>
    protected string DialogClassNames => DialogClassBuilder.Class;

    protected StyleBuilder DialogStyleBuilder { get; private set; }

    protected string DialogStyleNames => DialogStyleBuilder.Styles;

    #endregion
}