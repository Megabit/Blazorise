#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.FluentUI2.Components;

public partial class ColorPicker
{
    #region Constructors

    public ColorPicker()
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
        builder.Append( "fui-Input" );

        if ( ParentValidation?.Status == ValidationStatus.Error )
        {
            builder.Append( "fui-Input-error" );
        }
        else if ( ParentValidation?.Status == ValidationStatus.Success )
        {
            builder.Append( "fui-Input-success" );
        }

        if ( ThemeSize != Blazorise.Size.Default )
        {
            builder.Append( $"fui-Input-{ClassProvider.ToSize( ThemeSize )}" );
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

    protected override string ColorPreviewElementSelector => ":scope > .fui-Input__colorPreview";

    protected override string ColorValueElementSelector => ":scope > .fui-Input__colorValue";

    protected ClassBuilder InputClassBuilder { get; private set; }

    protected ClassBuilder AddonClassBuilder { get; private set; }

    protected StyleBuilder WrapperStyleBuilder { get; private set; }

    protected string InputClassNames => InputClassBuilder.Class;

    protected string AddonClassNames => AddonClassBuilder.Class;

    protected string WrapperStyleNames => WrapperStyleBuilder.Styles;

    protected string ColorPreviewClassNames => "fui-Input__colorPreview";

    protected string ColorValueClassNames => "fui-Input__colorValue";

    #endregion
}