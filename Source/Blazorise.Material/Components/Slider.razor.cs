#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Material.Components;

public partial class Slider<TValue> : Blazorise.Slider<TValue>
{
    #region Members

    private readonly ClassBuilder sliderContainerClassBuilder;

    private readonly StyleBuilder sliderContainerStyleBuilder;

    #endregion

    #region Constructors

    public Slider()
    {
        sliderContainerClassBuilder = new( BuildSliderContainerClasses, builder => builder.Append( Classes?.Wrapper ) );
        sliderContainerStyleBuilder = new( BuildSliderContainerStyles, builder => builder.Append( Styles?.Wrapper ) );
    }

    #endregion

    #region Methods

    private void BuildSliderContainerClasses( ClassBuilder builder )
    {
        builder.Append( "mui-slider-container" );
        AppendWrapperUtilities( builder );
    }

    private void BuildSliderContainerStyles( StyleBuilder builder )
    {
        builder.Append( $"--mui-slider-percent: {SliderPercent}" );
        AppendWrapperUtilities( builder );
    }

    protected override void BuildClasses( ClassBuilder builder )
    {
        base.BuildClasses( builder );

        if ( ThemeSize != Blazorise.Size.Default )
            builder.Append( $"mui-slider-{ClassProvider.ToSize( ThemeSize )}" );
    }

    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"--mui-slider-percent: {SliderPercent}" );

        base.BuildStyles( builder );
    }

    protected internal override void DirtyClasses()
    {
        sliderContainerClassBuilder.Dirty();

        base.DirtyClasses();
    }

    protected internal override void DirtyStyles()
    {
        sliderContainerStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    protected override async Task OnAfterSetParametersAsync( ParameterView parameters )
    {
        await base.OnAfterSetParametersAsync( parameters );

        if ( paramValue.Changed || paramMin.Changed || paramMax.Changed )
        {
            DirtyStyles();
        }
    }

    protected override async Task OnInternalValueChanged( TValue value )
    {
        DirtyStyles();

        await base.OnInternalValueChanged( value );
    }

    private string SliderPercent
    {
        get
        {
            double minValue = TryConvertToDouble( Min, out double convertedMin )
                ? convertedMin
                : 0d;
            double maxValue = TryConvertToDouble( Max, out double convertedMax )
                ? convertedMax
                : 100d;
            double currentValue = TryConvertToDouble( Value, out double convertedValue )
                ? convertedValue
                : minValue;

            if ( maxValue <= minValue )
                return "0%";

            double range = maxValue - minValue;
            double percent = ( currentValue - minValue ) / range * 100d;
            double clampedPercent = Math.Clamp( percent, 0d, 100d );

            return $"{clampedPercent.ToString( "0.###", CultureInfo.InvariantCulture )}%";
        }
    }

    private static bool TryConvertToDouble( TValue value, out double result )
    {
        if ( value is null )
        {
            result = 0d;
            return false;
        }

        return Converters.TryChangeType<double>( value, out result, CultureInfo.InvariantCulture );
    }

    #endregion

    #region Properties

    protected string SliderContainerClassNames => sliderContainerClassBuilder.Class;

    protected string SliderContainerStyleNames => sliderContainerStyleBuilder.Styles;

    protected string SliderDisplayValue => CurrentValueAsString;

    #endregion
}