#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDetailHeaderSynchronizer
{
    #region Methods

    internal static void AddPageHeaderForDetailField( ReportDefinition definition, int detailSectionIndex, ReportSectionDefinition detailSection, string fieldName, double x, double width )
    {
        if ( detailSection?.Type != ReportSectionType.Detail || string.IsNullOrWhiteSpace( fieldName ) )
            return;

        var pageHeader = FindPageHeaderForDetail( definition, detailSectionIndex );

        if ( pageHeader is null || pageHeader.Suppressed )
            return;

        var headerText = GetFieldHeaderText( fieldName );
        var headerY = GetPageHeaderElementY( pageHeader );

        if ( HasPageHeaderElement( pageHeader, headerText, x ) )
            return;

        pageHeader.Elements.Add( new()
        {
            Name = headerText,
            Type = ReportElementType.Text,
            Text = headerText,
            X = x,
            Y = headerY,
            Width = width,
            Height = 18,
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
            || detailElement?.Type != ReportElementType.Field
            || string.IsNullOrWhiteSpace( detailElement.Field )
            || ( Math.Abs( newX - originalX ) < 0.1 && Math.Abs( newWidth - originalWidth ) < 0.1 )
            || sourceSectionIndex < 0
            || sourceSectionIndex >= definition.Sections.Count
            || targetSectionIndex < 0
            || targetSectionIndex >= definition.Sections.Count
            || definition.Sections[sourceSectionIndex].Type != ReportSectionType.Detail
            || definition.Sections[targetSectionIndex].Type != ReportSectionType.Detail )
        {
            return;
        }

        var pageHeader = FindPageHeaderForDetail( definition, sourceSectionIndex );

        if ( pageHeader is null || pageHeader.Suppressed )
            return;

        HashSet<string> ignoredKeys = ignoredElementKeys is null
            ? null
            : new( ignoredElementKeys.Where( key => !string.IsNullOrWhiteSpace( key ) ), StringComparer.Ordinal );

        var headerElement = pageHeader.Elements.FirstOrDefault( element =>
            element.Type == ReportElementType.Text
            && Math.Abs( element.X - originalX ) < 0.1
            && Math.Abs( element.Width - originalWidth ) < 0.1
            && ( ignoredKeys is null || !ignoredKeys.Contains( element.Id ) ) );

        if ( headerElement is not null )
        {
            headerElement.X = newX;
            headerElement.Width = newWidth;
        }
    }

    private static ReportSectionDefinition FindPageHeaderForDetail( ReportDefinition definition, int detailSectionIndex )
    {
        if ( definition is null )
            return null;

        for ( var i = detailSectionIndex - 1; i >= 0; i-- )
        {
            if ( definition.Sections[i].Type == ReportSectionType.PageHeader )
                return definition.Sections[i];
        }

        return definition.Sections.FirstOrDefault( section => section.Type == ReportSectionType.PageHeader );
    }

    private static double GetPageHeaderElementY( ReportSectionDefinition pageHeader )
    {
        var firstElement = pageHeader.Elements
            .Where( element => element.Type is ReportElementType.Text or ReportElementType.Field )
            .OrderBy( element => element.Y )
            .ThenBy( element => element.X )
            .FirstOrDefault();

        return firstElement?.Y ?? 10;
    }

    private static bool HasPageHeaderElement( ReportSectionDefinition pageHeader, string headerText, double x )
    {
        return pageHeader.Elements.Any( element =>
            element.Type == ReportElementType.Text
            && string.Equals( element.Text, headerText, StringComparison.OrdinalIgnoreCase )
            && Math.Abs( element.X - x ) < 0.1 );
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