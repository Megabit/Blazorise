#region Using directives
using System;
using System.Globalization;
using Blazorise.Utilities;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportElementDefinitionHelper
{
    #region Methods

    internal static void BuildStyle( StyleBuilder builder, ReportElementDefinition element )
    {
        BuildStyle( builder, element, null, null, null );
    }

    internal static void BuildStyle( StyleBuilder builder, ReportElementDefinition element, ReportDefinition definition, object defaultData, ReportSectionDefinition section )
    {
        var font = SupportsTextFormatting( element.Type ) ? element.Font : null;
        var appearance = element.Appearance;
        var border = element.Border;

        builder.Append( $"left:{ReportMeasurementConverter.ToCssPixelString( element.X )}" );
        builder.Append( $"top:{ReportMeasurementConverter.ToCssPixelString( element.Y )}" );
        builder.Append( $"width:{ReportMeasurementConverter.ToCssPixelString( element.Width )}" );
        builder.Append( $"height:{ReportMeasurementConverter.ToCssPixelString( element.Height )}" );

        if ( !string.IsNullOrWhiteSpace( font?.Family ) )
            builder.Append( $"font-family:{font.Family}" );

        if ( font?.Size is > 0 )
            builder.Append( $"font-size:{ReportMeasurementConverter.ToCssPixelString( font.Size.Value )}" );

        if ( !string.IsNullOrWhiteSpace( font?.Color ) )
            builder.Append( $"color:{font.Color}!important" );

        if ( !string.IsNullOrWhiteSpace( appearance?.BackgroundColor ) )
            builder.Append( $"background-color:{appearance.BackgroundColor}!important" );

        builder.Append( "font-weight:700", font?.Bold == true );
        builder.Append( "font-style:italic", font?.Italic == true );
        builder.Append( "text-decoration:underline", font?.Underline == true );

        string textAlignment = ToCssTextAlignment( ResolveTextAlignment( element, definition, defaultData, section ) );

        if ( textAlignment is not null )
            builder.Append( $"text-align:{textAlignment}" );

        if ( !string.IsNullOrWhiteSpace( border?.Color ) )
            builder.Append( $"border-color:{border.Color}!important" );

        if ( border?.Width is >= 0 )
        {
            builder.Append( $"border-width:{ReportMeasurementConverter.ToCssPixelString( border.Width.Value )}" );
            builder.Append( "border-style:solid" );
        }

        if ( border?.Radius is >= 0 )
            builder.Append( $"border-radius:{ReportMeasurementConverter.ToCssPixelString( border.Radius.Value )}" );

        if ( appearance?.Opacity is >= 0 and <= 1 )
            builder.Append( $"opacity:{appearance.Opacity.Value.ToString( CultureInfo.InvariantCulture )}" );

        if ( !string.IsNullOrWhiteSpace( element.Style ) )
            builder.Append( element.Style.Trim().TrimEnd( ';' ) );
    }

    internal static double? NormalizeNullablePositiveNumber( double? value )
    {
        return value is > 0 ? value : null;
    }

    internal static double? NormalizeOpacity( double? value )
    {
        if ( value is null )
            return null;

        return Math.Clamp( value.Value, 0, 1 );
    }

    internal static ReportFontDefinition EnsureFont( ReportElementDefinition element )
    {
        return element.Font ??= new();
    }

    internal static bool SupportsTextFormatting( ReportElementType elementType )
    {
        return elementType is ReportElementType.Text or ReportElementType.Field or ReportElementType.Table;
    }

    internal static ReportAppearanceDefinition EnsureAppearance( ReportElementDefinition element )
    {
        return element.Appearance ??= new();
    }

    internal static ReportBorderDefinition EnsureBorder( ReportElementDefinition element )
    {
        return element.Border ??= new();
    }

    internal static string GetDisplayText( ReportElementDefinition element )
    {
        if ( element is null )
            return string.Empty;

        return element.Type == ReportElementType.Field
            ? ReportExpressionFormatter.FormatFieldExpression( element )
            : element.Text ?? element.Name ?? element.Type.ToString();
    }

    internal static string GetDisplayText( ReportDefinition definition, ReportElementDefinition element )
    {
        if ( element is null )
            return string.Empty;

        return element.Type == ReportElementType.Field
            ? ReportExpressionFormatter.FormatFieldExpression( definition, element )
            : element.Text ?? element.Name ?? element.Type.ToString();
    }

    internal static string NormalizeColorValue( string value )
    {
        return !string.IsNullOrWhiteSpace( value ) && value.StartsWith( "#", StringComparison.Ordinal ) && value.Length == 7
            ? value
            : "#000000";
    }

    private static string ToCssTextAlignment( TextAlignment alignment )
    {
        return alignment switch
        {
            TextAlignment.Start => "left",
            TextAlignment.End => "right",
            TextAlignment.Center => "center",
            TextAlignment.Justified => "justify",
            _ => null,
        };
    }

    private static TextAlignment ResolveTextAlignment( ReportElementDefinition element, ReportDefinition definition, object defaultData, ReportSectionDefinition section )
    {
        TextAlignment alignment = element.Font?.Alignment ?? TextAlignment.Default;

        if ( alignment != TextAlignment.Default || element.Type != ReportElementType.Field )
            return alignment;

        string dataSourceName = !string.IsNullOrWhiteSpace( element.DataSource )
            ? element.DataSource
            : section?.DataSource;

        return ReportDataSourceExplorer.TryResolveFieldType( definition, defaultData, dataSourceName, element.Field, out Type dataType ) && IsNumericType( dataType )
            ? TextAlignment.End
            : TextAlignment.Default;
    }

    private static bool IsNumericType( Type dataType )
    {
        if ( dataType is null )
            return false;

        Type normalizedType = Nullable.GetUnderlyingType( dataType ) ?? dataType;

        return normalizedType == typeof( byte )
            || normalizedType == typeof( sbyte )
            || normalizedType == typeof( short )
            || normalizedType == typeof( ushort )
            || normalizedType == typeof( int )
            || normalizedType == typeof( uint )
            || normalizedType == typeof( long )
            || normalizedType == typeof( ulong )
            || normalizedType == typeof( Int128 )
            || normalizedType == typeof( UInt128 )
            || normalizedType == typeof( Half )
            || normalizedType == typeof( float )
            || normalizedType == typeof( double )
            || normalizedType == typeof( decimal );
    }

    #endregion
}