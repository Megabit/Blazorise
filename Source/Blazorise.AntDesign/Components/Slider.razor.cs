#region Using directives
using System;
using System.Globalization;
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class Slider<TValue> : Blazorise.Slider<TValue>
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( Orientation == Orientation.Vertical ? "ant-slider-vertical" : "ant-slider-horizontal" );

        base.BuildClasses( builder );
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

    private double SliderPercentage
    {
        get
        {
            double minimum = TryConvertToDouble( Min, out double convertedMin ) ? convertedMin : 0d;
            double maximum = TryConvertToDouble( Max, out double convertedMax ) ? convertedMax : 100d;
            double current = TryConvertToDouble( Value, out double convertedValue ) ? convertedValue : minimum;

            if ( maximum <= minimum )
                return 0d;

            double percentage = ( current - minimum ) / ( maximum - minimum ) * 100d;

            return Math.Clamp( percentage, 0d, 100d );
        }
    }

    /// <summary>
    /// Gets the filled track styles for the current value and orientation.
    /// </summary>
    protected string TrackStyle
        => Orientation == Orientation.Vertical
            ? $"bottom: 0%; top: auto; height: {SliderPercentage.ToString( "0.####", CultureInfo.InvariantCulture )}%;"
            : $"left: 0%; right: auto; width: {SliderPercentage.ToString( "0.####", CultureInfo.InvariantCulture )}%;";

    /// <summary>
    /// Gets the handle position styles for the current value and orientation.
    /// </summary>
    protected string HandleStyle
        => Orientation == Orientation.Vertical
            ? $"left: 50%; bottom: {SliderPercentage.ToString( "0.####", CultureInfo.InvariantCulture )}%; transform: translate(-50%, 50%);"
            : $"left: {SliderPercentage.ToString( "0.####", CultureInfo.InvariantCulture )}%; transform: translateX(-50%);";

    /// <summary>
    /// Gets the tooltip position styles for the current value and orientation.
    /// </summary>
    protected string TooltipStyle
        => Orientation == Orientation.Vertical
            ? $"left: calc(100% + var(--ant-margin-xs)); top: auto; bottom: {SliderPercentage.ToString( "0.####", CultureInfo.InvariantCulture )}%; transform: translateY(50%);"
            : $"left: {SliderPercentage.ToString( "0.####", CultureInfo.InvariantCulture )}%; transform: translate(-50%, calc(-100% - var(--ant-margin-xs)));";

    /// <summary>
    /// Gets the classes for the slider root.
    /// </summary>
    protected string RootClassNames => ClassNames;

    #endregion
}