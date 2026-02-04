#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.FluentUI2.Components;

public partial class Radio<TValue>
{
    #region Constructors

    public Radio()
    {
        InputClassBuilder = new ClassBuilder( BuildInputClasses, builder => builder.Append( Classes?.Wrapper ) );
        LabelButonClassBuilder = new ClassBuilder( BuildLabelButonClasses, builder => builder.Append( Classes?.LabelButton ) );
        AddonClassBuilder = new ClassBuilder( BuildAddonClasses );
        WrapperStyleBuilder = new StyleBuilder( BuildWrapperStyles, builder => builder.Append( Styles?.Wrapper ) );
    }

    #endregion

    #region Methods

    protected internal override void DirtyClasses()
    {
        if ( AsButton )
        {
            LabelButonClassBuilder.Dirty();
        }
        else
        {
            InputClassBuilder.Dirty();
        }

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
        builder.Append( "fui-Radio" );

        if ( ParentValidation?.Status == ValidationStatus.Error )
        {
            builder.Append( "fui-Radio-error" );
        }
        else if ( ParentValidation?.Status == ValidationStatus.Success )
        {
            builder.Append( "fui-Radio-success" );
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

    private void BuildLabelButonClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Button( false ) );
        builder.Append( ClassProvider.ButtonColor( ButtonColor, false ) );
        builder.Append( ClassProvider.ButtonActive( false, IsActive ) );
        builder.Append( ClassProvider.ButtonDisabled( false, Disabled ) );
    }

    private void BuildWrapperStyles( StyleBuilder builder )
    {
        AppendWrapperUtilities( builder );
    }

    #endregion

    #region Properties

    protected ClassBuilder InputClassBuilder { get; private set; }

    protected ClassBuilder LabelButonClassBuilder { get; private set; }

    protected ClassBuilder AddonClassBuilder { get; private set; }

    protected StyleBuilder WrapperStyleBuilder { get; private set; }

    protected string InputClassNames => InputClassBuilder.Class;

    protected string LabelButonClassNames => LabelButonClassBuilder.Class;

    protected string AddonClassNames => AddonClassBuilder.Class;

    protected string WrapperStyleNames => WrapperStyleBuilder.Styles;

    #endregion
}