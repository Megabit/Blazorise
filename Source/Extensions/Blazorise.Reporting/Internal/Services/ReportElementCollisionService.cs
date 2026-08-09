#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportElementCollisionService
{
    #region Methods

    internal IReadOnlyList<ReportDesignerWarning> FindWarnings( ReportDefinition definition )
    {
        List<ReportDesignerWarning> warnings = [];
        HashSet<ReportDefinition> visitedDefinitions = [];

        FindWarnings( definition, warnings, visitedDefinitions );

        return warnings;
    }

    internal bool HasCollision(
        IList<ReportElementDefinition> elements,
        ReportElementDefinition element,
        ReportDesignerDragPreview preview,
        double x,
        double y,
        Func<string, bool> ignoreElement = null )
    {
        if ( elements is null || preview is null || !CanCollide( element, preview.ElementType ) )
            return false;

        (double Left, double Top, double Width, double Height) bounds = GetBounds( element, preview.ElementType, x, y, preview.Width, preview.Height );

        foreach ( ReportElementDefinition sibling in elements )
        {
            if ( ReferenceEquals( sibling, element )
                || !CanCollide( sibling, sibling?.Type )
                || ignoreElement?.Invoke( ReportDefinitionHelper.EnsureElementId( sibling ) ) == true )
            {
                continue;
            }

            (double Left, double Top, double Width, double Height) siblingBounds = GetBounds( sibling, sibling.Type, sibling.X, sibling.Y, sibling.Width, sibling.Height );

            if ( Intersects( bounds, siblingBounds ) )
                return true;
        }

        return false;
    }

    private static void FindWarnings(
        ReportDefinition definition,
        ICollection<ReportDesignerWarning> warnings,
        ISet<ReportDefinition> visitedDefinitions )
    {
        if ( definition is null || !visitedDefinitions.Add( definition ) )
            return;

        IReadOnlyList<ReportPageDefinition> pages = definition.Pages ?? [];

        for ( int pageIndex = 0; pageIndex < pages.Count; pageIndex++ )
        {
            ReportPageDefinition page = pages[pageIndex];
            string pageName = string.IsNullOrWhiteSpace( page?.Name ) ? $"Page {pageIndex + 1}" : page.Name;

            foreach ( ReportBandDefinition band in page?.Bands ?? [] )
            {
                if ( band is null )
                    continue;

                FindWarnings( band.Elements, pageName, ReportDefinitionHelper.GetSectionDisplayName( band ), warnings );
                FindSubreportWarnings( band.Elements, warnings, visitedDefinitions );
            }
        }
    }

    private static void FindWarnings(
        IList<ReportElementDefinition> elements,
        string pageName,
        string bandName,
        ICollection<ReportDesignerWarning> warnings )
    {
        if ( elements is null )
            return;

        for ( int elementIndex = 0; elementIndex < elements.Count; elementIndex++ )
        {
            ReportElementDefinition element = elements[elementIndex];

            if ( CanCollide( element, element?.Type ) )
            {
                (double Left, double Top, double Width, double Height) bounds = GetBounds( element, element.Type, element.X, element.Y, element.Width, element.Height );

                for ( int siblingIndex = elementIndex + 1; siblingIndex < elements.Count; siblingIndex++ )
                {
                    ReportElementDefinition sibling = elements[siblingIndex];

                    if ( !CanCollide( sibling, sibling?.Type ) )
                        continue;

                    (double Left, double Top, double Width, double Height) siblingBounds = GetBounds( sibling, sibling.Type, sibling.X, sibling.Y, sibling.Width, sibling.Height );

                    if ( !Intersects( bounds, siblingBounds ) )
                        continue;

                    string elementKey = ReportDefinitionHelper.EnsureElementId( element );
                    string siblingKey = ReportDefinitionHelper.EnsureElementId( sibling );
                    string elementName = GetElementDisplayName( element );
                    string siblingName = GetElementDisplayName( sibling );

                    warnings.Add( new(
                        $"{elementName} overlaps {siblingName} in {bandName} on {pageName}.",
                        [elementKey, siblingKey] ) );
                }
            }

            foreach ( IList<ReportElementDefinition> childElements in ReportDefinitionHelper.GetChildElementCollections( element ) )
                FindWarnings( childElements, pageName, bandName, warnings );
        }
    }

    private static void FindSubreportWarnings(
        IEnumerable<ReportElementDefinition> elements,
        ICollection<ReportDesignerWarning> warnings,
        ISet<ReportDefinition> visitedDefinitions )
    {
        foreach ( ReportElementDefinition element in elements ?? [] )
        {
            if ( element is null )
                continue;

            if ( element is ReportSubreportElementDefinition subreport )
                FindWarnings( subreport.Report, warnings, visitedDefinitions );

            foreach ( IList<ReportElementDefinition> childElements in ReportDefinitionHelper.GetChildElementCollections( element ) )
                FindSubreportWarnings( childElements, warnings, visitedDefinitions );
        }
    }

    private static string GetElementDisplayName( ReportElementDefinition element )
    {
        string name = element?.Name;

        if ( string.IsNullOrWhiteSpace( name ) )
            name = ReportElementDefinitionHelper.GetDisplayText( element );

        if ( string.IsNullOrWhiteSpace( name ) )
            name = ReportDefinitionHelper.GetElementTypeDisplayName( element.Type );

        return $"“{name}”";
    }

    private static bool CanCollide( ReportElementDefinition element, ReportElementType? elementType )
    {
        return elementType is not null
            && elementType != ReportElementType.PageBreak
            && element?.Suppress?.Value != true
            && element?.ShowCollisionWarnings != false;
    }

    private static (double Left, double Top, double Width, double Height) GetBounds(
        ReportElementDefinition element,
        ReportElementType elementType,
        double x,
        double y,
        double width,
        double height )
    {
        width = Math.Max( 0, width );
        height = Math.Max( 0, height );

        if ( elementType != ReportElementType.Line )
            return ( x, y, width, height );

        double thickness = ReportLayoutGeometry.GetLineThickness( element );
        Orientation orientation = ( element as ReportLineElementDefinition )?.Orientation ?? Orientation.Horizontal;

        if ( orientation == Orientation.Vertical )
        {
            double visibleWidth = Math.Min( width, thickness );
            return ( x + ( width - visibleWidth ) / 2, y, visibleWidth, height );
        }

        double visibleHeight = Math.Min( height, thickness );
        return ( x, y + ( height - visibleHeight ) / 2, width, visibleHeight );
    }

    private static bool Intersects(
        (double Left, double Top, double Width, double Height) bounds,
        (double Left, double Top, double Width, double Height) otherBounds )
    {
        return ReportLayoutGeometry.Intersects(
            bounds.Left,
            bounds.Top,
            bounds.Width,
            bounds.Height,
            otherBounds.Left,
            otherBounds.Top,
            otherBounds.Width,
            otherBounds.Height );
    }

    #endregion
}