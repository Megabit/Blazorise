#region Using directives
using System;
using System.Globalization;
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class RangeSlider<TValue> : Blazorise.RangeSlider<TValue>
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

    private static double CalculatePercentage( double value, double minimum, double maximum )
    {
        if ( maximum <= minimum )
            return 0d;

        return Math.Clamp( ( value - minimum ) / ( maximum - minimum ) * 100d, 0d, 100d );
    }

    private static string ToPercentageString( double value )
        => $"{value.ToString( "0.####", CultureInfo.InvariantCulture )}%";

    private static string RemoveZIndex( string style )
    {
        if ( string.IsNullOrWhiteSpace( style ) )
            return style;

        string[] segments = style.Split( ';', StringSplitOptions.RemoveEmptyEntries );

        return string.Join( "; ", Array.FindAll( segments, x => !x.TrimStart().StartsWith( "z-index", StringComparison.OrdinalIgnoreCase ) ) );
    }

    private void GetDisplayBounds( out double minimum, out double maximum )
    {
        minimum = TryConvertToDouble( Min, out double convertedMin ) ? convertedMin : 0d;
        maximum = TryConvertToDouble( Max, out double convertedMax ) ? convertedMax : 100d;

        if ( maximum < minimum )
            (minimum, maximum) = (maximum, minimum);
    }

    private double GetValueAsDouble( TValue value, double fallback )
        => TryConvertToDouble( value, out double convertedValue ) ? convertedValue : fallback;

    #endregion

    #region Properties

    private double StartHandlePercentage
    {
        get
        {
            GetDisplayBounds( out double minimum, out double maximum );

            return CalculatePercentage( GetValueAsDouble( Value.Start, minimum ), minimum, maximum );
        }
    }

    private double EndHandlePercentage
    {
        get
        {
            GetDisplayBounds( out double minimum, out double maximum );

            return CalculatePercentage( GetValueAsDouble( Value.End, maximum ), minimum, maximum );
        }
    }

    protected string RootClassNames
        => $"{ClassNames} ant-slider-horizontal";

    protected string StartHandleStyle
        => $"left: {ToPercentageString( StartHandlePercentage )}; transform: translateX(-50%); z-index: 5;";

    protected string EndHandleStyle
        => $"left: {ToPercentageString( EndHandlePercentage )}; transform: translateX(-50%); z-index: 6;";

    protected string VisibleStartInputStyleNames
        => RemoveZIndex( StartInputStyleNames );

    protected string VisibleEndInputStyleNames
        => RemoveZIndex( EndInputStyleNames );

    protected string VisibleStartTooltipStyleNames
        => $"{StartTooltipStyleNames}; transform: translate(-50%, calc(-100% - var(--ant-margin-xs)));";

    protected string VisibleEndTooltipStyleNames
        => $"{EndTooltipStyleNames}; transform: translate(-50%, calc(-100% - var(--ant-margin-xs)));";

    #endregion
}