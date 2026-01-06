#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
#endregion

namespace Blazorise.FluentUI2.Components;

public partial class Slider<TValue>
{
    #region Constructors

    public Slider()
    {
        InputClassBuilder = new ClassBuilder( BuildInputClasses, builder => builder.Append( Classes?.Wrapper ) );
        InputStyleBuilder = new StyleBuilder( BuildInputStyles, builder => builder.Append( Styles?.Wrapper ) );
    }

    #endregion

    #region Methods

    protected internal override void DirtyClasses()
    {
        InputClassBuilder.Dirty();

        base.DirtyClasses();
    }

    protected override void DirtyStyles()
    {
        InputStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    protected override Task OnInternalValueChanged( TValue value )
    {
        DirtyStyles();

        return base.OnInternalValueChanged( value );
    }

    private void BuildInputClasses( ClassBuilder builder )
    {
        builder.Append( "fui-Slider" );

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
    }

    private void BuildInputStyles( StyleBuilder builder )
    {
        TValue min = Converters.ChangeType<TValue>( Min );
        TValue max = Converters.ChangeType<TValue>( Max );

        var currentValue = MathUtils.Clamp( CurrentValue, min, max );
        var valuePercent = MathUtils.GetPercent( currentValue, min, max );

        builder.Append( $"--fui-Slider--direction: 90deg;--fui-Slider--progress: {valuePercent}%;" );

        if ( Disabled )
        {
            builder.Append( "--fui-Slider__thumb--color: var(--colorNeutralForegroundDisabled);" );
            builder.Append( "--fui-Slider__progress--color: var(--colorNeutralForegroundDisabled);" );
            builder.Append( "--fui-Slider__rail--color: var(--colorNeutralBackgroundDisabled);" );
        }
        else
        {
            builder.Append( "--fui-Slider__thumb--color: var(--colorCompoundBrandBackground);" );
            builder.Append( "--fui-Slider__progress--color: var(--colorCompoundBrandBackground);" );
            builder.Append( "--fui-Slider__rail--color: var(--colorNeutralStrokeAccessible);" );
        }
    }

    #endregion

    #region Properties

    protected ClassBuilder InputClassBuilder { get; private set; }

    protected StyleBuilder InputStyleBuilder { get; private set; }

    protected string InputClassNames => InputClassBuilder.Class;

    protected string InputStyleNames => InputStyleBuilder.Styles;

    protected string AddonClassNames => "fui-Input__content";

    #endregion
}