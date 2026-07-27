#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDetailHeaderSynchronizer
{
    #region Members

    private const double DefaultHeaderElementHeight = 18;

    private const double DefaultHeaderElementY = 10;

    private const double HeaderElementMatchTolerance = 0.1;

    #endregion

    #region Methods

    internal static void AddPageHeaderForDetailField( ReportDefinition definition, int detailSectionIndex, ReportBandDefinition detailSection, string fieldName, double x, double width )
    {
        if ( detailSection?.Type != ReportBandType.Detail || string.IsNullOrWhiteSpace( fieldName ) )
            return;

        var pageHeader = FindPageHeaderForDetail( definition, detailSectionIndex );

        if ( pageHeader is null || ReportValueResolver.ResolveStaticSuppress( pageHeader ) )
            return;

        var headerText = GetFieldHeaderText( fieldName );
        var headerY = GetPageHeaderElementY( pageHeader );

        if ( HasPageHeaderElement( pageHeader, headerText, x ) )
            return;

        pageHeader.Elements.Add( new ReportTextElementDefinition
        {
            Name = headerText,
            Text = headerText,
            X = x,
            Y = headerY,
            Width = width,
            Height = DefaultHeaderElementHeight,
            Font = new()
            {
                Bold = true,
                Alignment = TextAlignment.End,
            },
        } );
    }

    internal static void SyncMatchingPageHeaderForDetailElement(
        ReportDefinition definition,
        int sourceSectionIndex,
        int targetSectionIndex,
        ReportElementDefinition detailElement,
        double originalX,
        double originalWidth,
        double newX,
        double newWidth,
        IEnumerable<string> ignoredElementKeys = null )
    {
        if ( definition is null
            || detailElement is not ReportFieldElementDefinition detailFieldElement
            || string.IsNullOrWhiteSpace( detailFieldElement.Field )
            || ( Math.Abs( newX - originalX ) < HeaderElementMatchTolerance && Math.Abs( newWidth - originalWidth ) < HeaderElementMatchTolerance )
            || sourceSectionIndex < 0
            || sourceSectionIndex >= definition.Bands.Count
            || targetSectionIndex < 0
            || targetSectionIndex >= definition.Bands.Count
            || definition.Bands[sourceSectionIndex].Type != ReportBandType.Detail
            || definition.Bands[targetSectionIndex].Type != ReportBandType.Detail )
        {
            return;
        }

        var pageHeader = FindPageHeaderForDetail( definition, sourceSectionIndex );

        if ( pageHeader is null || ReportValueResolver.ResolveStaticSuppress( pageHeader ) )
            return;

        HashSet<string> ignoredKeys = ignoredElementKeys is null
            ? null
            : new( ignoredElementKeys.Where( key => !string.IsNullOrWhiteSpace( key ) ), StringComparer.Ordinal );

        ReportElementDefinition headerElement = pageHeader.Elements.FirstOrDefault( element =>
            element is ReportTextElementDefinition
            && Math.Abs( element.X - originalX ) < HeaderElementMatchTolerance
            && Math.Abs( element.Width - originalWidth ) < HeaderElementMatchTolerance
            && ( ignoredKeys is null || !ignoredKeys.Contains( element.Id ) ) );

        if ( headerElement is not null )
        {
            headerElement.X = newX;
            headerElement.Width = newWidth;
        }
    }

    private static ReportBandDefinition FindPageHeaderForDetail( ReportDefinition definition, int detailSectionIndex )
    {
        if ( definition is null )
            return null;

        for ( var i = detailSectionIndex - 1; i >= 0; i-- )
        {
            if ( definition.Bands[i].Type == ReportBandType.PageHeader )
                return definition.Bands[i];
        }

        return definition.Bands.FirstOrDefault( section => section.Type == ReportBandType.PageHeader );
    }

    private static double GetPageHeaderElementY( ReportBandDefinition pageHeader )
    {
        ReportElementDefinition firstElement = pageHeader.Elements
            .Where( element => element is ReportTextElementDefinition or ReportFieldElementDefinition )
            .OrderBy( element => element.Y )
            .ThenBy( element => element.X )
            .FirstOrDefault();

        return firstElement?.Y ?? DefaultHeaderElementY;
    }

    private static bool HasPageHeaderElement( ReportBandDefinition pageHeader, string headerText, double x )
    {
        return pageHeader.Elements.Any( element =>
            element is ReportTextElementDefinition textElement
            && string.Equals( textElement.Text, headerText, StringComparison.OrdinalIgnoreCase )
            && Math.Abs( element.X - x ) < HeaderElementMatchTolerance );
    }

    private static string GetFieldHeaderText( string fieldName )
    {
        var segment = fieldName.Split( '.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries ).LastOrDefault() ?? fieldName;

        if ( string.IsNullOrWhiteSpace( segment ) )
            return fieldName;

        segment = segment.Replace( '_', ' ' );
        var characters = new List<char>();

        for ( var i = 0; i < segment.Length; i++ )
        {
            var character = segment[i];

            if ( i > 0
                && character != ' '
                && char.IsUpper( character )
                && segment[i - 1] != ' '
                && ( char.IsLower( segment[i - 1] ) || ( i + 1 < segment.Length && char.IsLower( segment[i + 1] ) ) ) )
            {
                characters.Add( ' ' );
            }

            characters.Add( character );
        }

        return new string( characters.ToArray() );
    }

    #endregion
}