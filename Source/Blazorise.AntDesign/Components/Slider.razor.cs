#region Using directives
using System;
using System.Globalization;
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class Slider<TValue> : Blazorise.Slider<TValue>
{
    #region Methods

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

    protected string TrackStyle
        => $"left: 0%; right: auto; width: {SliderPercentage.ToString( "0.####", CultureInfo.InvariantCulture )}%;";

    protected string HandleStyle
        => $"left: {SliderPercentage.ToString( "0.####", CultureInfo.InvariantCulture )}%; transform: translateX(-50%);";

    protected string TooltipStyle
        => $"left: {SliderPercentage.ToString( "0.####", CultureInfo.InvariantCulture )}%; transform: translate(-50%, calc(-100% - var(--ant-margin-xs)));";

    protected string RootClassNames
        => $"{ClassNames} ant-slider-horizontal";

    #endregion
}