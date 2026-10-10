#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.FluentUI2.Components;

/// <summary>
/// A color picker styled for Fluent UI 2.
/// </summary>
public partial class ColorPicker
{
    #region Constructors

    /// <summary>
    /// Initializes the Fluent UI input and addon builders.
    /// </summary>
    public ColorPicker()
    {
        InputClassBuilder = new ClassBuilder( BuildInputClasses, builder => builder.Append( Classes?.Wrapper ) );
        AddonClassBuilder = new ClassBuilder( BuildAddonClasses );
        WrapperStyleBuilder = new StyleBuilder( BuildWrapperStyles, builder => builder.Append( Styles?.Wrapper ) );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        InputClassBuilder.Dirty();
        AddonClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <inheritdoc/>
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
        else if ( ParentValidation?.Status == ValidationStatus.Warning )
        {
            builder.Append( "fui-Input-warning" );
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

    /// <summary>
    /// Gets the class builder for the input wrapper.
    /// </summary>
    protected ClassBuilder InputClassBuilder { get; private set; }

    /// <summary>
    /// Gets the class builder for the addon content wrapper.
    /// </summary>
    protected ClassBuilder AddonClassBuilder { get; private set; }

    /// <summary>
    /// Gets the style builder for the input wrapper.
    /// </summary>
    protected StyleBuilder WrapperStyleBuilder { get; private set; }

    /// <summary>
    /// Gets the classes for the input wrapper.
    /// </summary>
    protected string InputClassNames => InputClassBuilder.Class;

    /// <summary>
    /// Gets the classes for the addon content wrapper.
    /// </summary>
    protected string AddonClassNames => AddonClassBuilder.Class;

    /// <summary>
    /// Gets the styles for the input wrapper.
    /// </summary>
    protected string WrapperStyleNames => WrapperStyleBuilder.Styles;

    /// <summary>
    /// Gets the classes for the displayed color value.
    /// </summary>
    protected string ColorValueClassNames => "fui-Input__colorValue";

    #endregion
}