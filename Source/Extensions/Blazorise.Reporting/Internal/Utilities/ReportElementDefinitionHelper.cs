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
        BuildStyle( builder, element, null, null, null, null, false );
    }

    internal static void BuildStyle( StyleBuilder builder, ReportElementDefinition element, ReportDefinition definition, object defaultData, ReportSectionDefinition section )
    {
        BuildStyle( builder, element, definition, defaultData, null, section, false );
    }

    internal static void BuildStyle( StyleBuilder builder, ReportElementDefinition element, ReportDefinition definition, object defaultData, object item, ReportSectionDefinition section, bool designMode )
    {
        ReportFontDefinition font = SupportsTextFormatting( element.Type ) ? element.Font : null;
        ReportAppearanceDefinition appearance = element.Appearance;
        ReportBorderDefinition border = element.Border;

        builder.Append( $"left:{ReportMeasurementConverter.ToCssPixelString( element.X )}" );
        builder.Append( $"top:{ReportMeasurementConverter.ToCssPixelString( element.Y )}" );
        builder.Append( $"width:{ReportMeasurementConverter.ToCssPixelString( element.Width )}" );

        if ( ReportValueResolver.ResolveCanGrow( element, section, definition, defaultData, item, designMode ) && !designMode && element.Type != ReportElementType.Line )
            builder.Append( $"min-height:{ReportMeasurementConverter.ToCssPixelString( element.Height )}" );
        else
            builder.Append( $"height:{ReportMeasurementConverter.ToCssPixelString( element.Height )}" );

        if ( !string.IsNullOrWhiteSpace( font?.Family ) )
            builder.Append( $"font-family:{font.Family}" );

        if ( font?.Size is > 0 )
            builder.Append( $"font-size:{ReportMeasurementConverter.ToCssPixelString( font.Size.Value )}" );

        string fontColor = ToCssColor( font?.Color ?? ReportColor.Default );

        if ( fontColor is not null )
            builder.Append( $"color:{fontColor}!important" );

        string backgroundColor = ToCssColor( appearance?.BackgroundColor ?? ReportColor.Default );

        if ( backgroundColor is not null && element.Type != ReportElementType.Line )
            builder.Append( $"background-color:{backgroundColor}!important" );

        builder.Append( "font-weight:700", font?.Bold == true );
        builder.Append( "font-style:italic", font?.Italic == true );
        builder.Append( "text-decoration:underline", font?.Underline == true );

        string textAlignment = ToCssTextAlignment( ResolveTextAlignment( element, definition, defaultData, section ) );

        if ( textAlignment is not null )
            builder.Append( $"text-align:{textAlignment}" );

        string verticalAlignment = ToCssContentVerticalAlignment( ResolveVerticalAlignment( element ) );

        if ( verticalAlignment is not null )
            builder.Append( $"--b-report-element-content-justify:{verticalAlignment}" );

        if ( element is ReportLineElementDefinition )
        {
            string lineColor = ToCssColor( border?.Color ?? ReportColor.Default );

            if ( lineColor is not null )
                builder.Append( $"--b-report-line-color:{lineColor}" );

            builder.Append( $"--b-report-line-thickness:{ReportMeasurementConverter.ToCssPixelString( ReportLayoutGeometry.GetLineThickness( element ) )}" );
        }
        else
        {
            string borderColor = ToCssColor( border?.Color ?? ReportColor.Default );

            if ( borderColor is not null )
                builder.Append( $"border-color:{borderColor}!important" );

            if ( border?.Width is >= 0 )
            {
                builder.Append( $"border-width:{ReportMeasurementConverter.ToCssPixelString( border.Width.Value )}" );
                builder.Append( "border-style:solid" );
            }

            if ( border?.Radius is >= 0 )
                builder.Append( $"border-radius:{ReportMeasurementConverter.ToCssPixelString( border.Radius.Value )}" );
        }

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
        return elementType is ReportElementType.Text or ReportElementType.Field;
    }

    internal static ReportAppearanceDefinition EnsureAppearance( ReportElementDefinition element )
    {
        return element.Appearance ??= new();
    }

    internal static ReportBorderDefinition EnsureBorder( ReportElementDefinition element )
    {
        return element.Border ??= new();
    }

    internal static string GetText( ReportElementDefinition element )
    {
        return element switch
        {
            ReportTextElementDefinition textElement => textElement.Text,
            ReportImageElementDefinition imageElement => imageElement.Text,
            _ => null,
        };
    }

    internal static string GetDisplayText( ReportElementDefinition element )
    {
        if ( element is null )
            return string.Empty;

        return element is ReportFieldElementDefinition
            ? ReportExpressionFormatter.FormatFieldExpression( element )
            : GetText( element ) ?? element.Name ?? element.Type.ToString();
    }

    internal static string GetDisplayText( ReportDefinition definition, ReportElementDefinition element )
    {
        if ( element is null )
            return string.Empty;

        return element is ReportFieldElementDefinition
            ? ReportExpressionFormatter.FormatFieldExpression( definition, element )
            : GetText( element ) ?? element.Name ?? element.Type.ToString();
    }

    internal static string NormalizeColorValue( ReportColor value )
    {
        return ToCssColor( value ) ?? "#000000";
    }

    internal static string ToCssColor( ReportColor color )
    {
        return color.ToCssString();
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

    private static string ToCssContentVerticalAlignment( VerticalAlignment alignment )
    {
        return alignment switch
        {
            VerticalAlignment.Top => "flex-start",
            VerticalAlignment.Middle => "center",
            VerticalAlignment.Bottom => "flex-end",
            _ => null,
        };
    }

    internal static TextAlignment ResolveTextAlignment( ReportElementDefinition element, ReportDefinition definition, object defaultData, ReportSectionDefinition section )
    {
        TextAlignment alignment = element.Font?.Alignment ?? TextAlignment.Default;

        if ( alignment != TextAlignment.Default || element is not ReportFieldElementDefinition fieldElement )
            return alignment;

        string dataSourceName = !string.IsNullOrWhiteSpace( fieldElement.DataSource )
            ? fieldElement.DataSource
            : section?.DataSource;

        return ReportDataSourceExplorer.TryResolveFieldType( definition, defaultData, dataSourceName, fieldElement.Field, out Type dataType ) && IsNumericType( dataType )
            ? TextAlignment.End
            : TextAlignment.Default;
    }

    internal static VerticalAlignment ResolveVerticalAlignment( ReportElementDefinition element )
    {
        return SupportsTextFormatting( element.Type )
            ? element.Font?.VerticalAlignment ?? VerticalAlignment.Default
            : VerticalAlignment.Default;
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