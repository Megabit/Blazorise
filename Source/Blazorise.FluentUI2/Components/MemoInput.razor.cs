#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.FluentUI2.Components;

public partial class MemoInput
{
    #region Constructors

    public MemoInput()
    {
        InputClassBuilder = new ClassBuilder( BuildInputClasses, builder => builder.Append( Classes?.Wrapper ) );
        AddonClassBuilder = new ClassBuilder( BuildAddonClasses );
        WrapperStyleBuilder = new StyleBuilder( BuildWrapperStyles, builder => builder.Append( Styles?.Wrapper ) );
    }

    #endregion

    #region Methods

    protected internal override void DirtyClasses()
    {
        InputClassBuilder.Dirty();
        AddonClassBuilder.Dirty();

        base.DirtyClasses();
    }

    protected internal override void DirtyStyles()
    {
        WrapperStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    private void BuildInputClasses( ClassBuilder builder )
    {
        builder.Append( "fui-Textarea" );

        if ( Plaintext )
        {
            builder.Append( "fui-Textarea-plaintext" );
        }

        if ( ParentValidation?.Status == ValidationStatus.Error )
        {
            builder.Append( "fui-Textarea-error" );
        }
        else if ( ParentValidation?.Status == ValidationStatus.Success )
        {
            builder.Append( "fui-Textarea-success" );
        }

        if ( ThemeSize != Blazorise.Size.Default )
        {
            builder.Append( $"fui-Textarea-{ClassProvider.ToSize( ThemeSize )}" );
        }

        if ( Disabled )
        {
            builder.Append( "disabled" );
        }

        AppendWrapperUtilities( builder );
    }

    private void BuildAddonClasses( ClassBuilder builder )
    {
        builder.Append( "fui-Input__content" );
        builder.Append( Classes?.Wrapper );
        AppendWrapperUtilities( builder );
    }

    private void BuildWrapperStyles( StyleBuilder builder )
    {
        AppendWrapperUtilities( builder );
    }

    #endregion

    #region Properties

    protected ClassBuilder InputClassBuilder { get; private set; }

    protected ClassBuilder AddonClassBuilder { get; private set; }

    protected StyleBuilder WrapperStyleBuilder { get; private set; }

    protected string InputClassNames => InputClassBuilder.Class;

    protected string AddonClassNames => AddonClassBuilder.Class;

    protected string WrapperStyleNames => WrapperStyleBuilder.Styles;

    #endregion
}