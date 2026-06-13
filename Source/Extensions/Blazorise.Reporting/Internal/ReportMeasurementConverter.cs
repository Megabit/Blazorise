#region Using directives
using System;
using System.Globalization;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportMeasurementConverter
{
    #region Members

    internal const double PointsPerInch = 72d;

    private const double CssPixelsPerInch = 96d;

    private const double MillimetersPerInch = 25.4d;

    private const double CentimetersPerInch = 2.54d;

    #endregion

    #region Methods

    internal static double ToPoints( double value, ReportMeasurementUnit unit )
    {
        return unit switch
        {
            ReportMeasurementUnit.Inch => value * PointsPerInch,
            ReportMeasurementUnit.Centimeter => value * PointsPerInch / CentimetersPerInch,
            ReportMeasurementUnit.Millimeter => value * PointsPerInch / MillimetersPerInch,
            _ => value,
        };
    }

    internal static double ToPoints( double? value, ReportMeasurementUnit unit, double defaultPoints = 0d )
    {
        return value.HasValue
            ? ToPoints( value.Value, unit )
            : defaultPoints;
    }

    internal static double FromPoints( double value, ReportMeasurementUnit unit )
    {
        return unit switch
        {
            ReportMeasurementUnit.Inch => value / PointsPerInch,
            ReportMeasurementUnit.Centimeter => value * CentimetersPerInch / PointsPerInch,
            ReportMeasurementUnit.Millimeter => value * MillimetersPerInch / PointsPerInch,
            _ => value,
        };
    }

    internal static double FromPoints( double? value, ReportMeasurementUnit unit, double defaultValue = 0d )
    {
        return value.HasValue
            ? FromPoints( value.Value, unit )
            : defaultValue;
    }

    internal static double ToCssPixelValue( double points )
    {
        return points * CssPixelsPerInch / PointsPerInch;
    }

    internal static double FromCssPixelValue( double cssPixels )
    {
        return cssPixels * PointsPerInch / CssPixelsPerInch;
    }

    internal static string ToCssPixelString( double points )
    {
        return FormatCssPixelValue( ToCssPixelValue( points ) );
    }

    internal static string FormatCssPixelValue( double cssPixels )
    {
        return $"{cssPixels.ToString( "0.###", CultureInfo.InvariantCulture )}px";
    }

    internal static decimal? GetEditorStep( ReportMeasurementUnit unit )
    {
        return unit switch
        {
            ReportMeasurementUnit.Inch => 0.1m,
            ReportMeasurementUnit.Centimeter => 0.1m,
            ReportMeasurementUnit.Millimeter => 1m,
            _ => 1m,
        };
    }

    internal static double RoundForDisplay( double value, ReportMeasurementUnit unit )
    {
        return unit switch
        {
            ReportMeasurementUnit.Inch => Math.Round( value, 3 ),
            ReportMeasurementUnit.Centimeter => Math.Round( value, 2 ),
            ReportMeasurementUnit.Millimeter => Math.Round( value, 1 ),
            _ => Math.Round( value, 1 ),
        };
    }

    #endregion
}