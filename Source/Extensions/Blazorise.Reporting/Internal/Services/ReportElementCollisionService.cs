#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportElementCollisionService
{
    #region Methods

    internal HashSet<string> FindCollidingElementKeys( ReportDefinition definition )
    {
        HashSet<string> collidingElementKeys = [];

        foreach ( ReportBandDefinition band in definition?.Bands ?? [] )
            FindCollisions( band.Elements, collidingElementKeys );

        return collidingElementKeys;
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

    private static void FindCollisions( IList<ReportElementDefinition> elements, ISet<string> collidingElementKeys )
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

                    collidingElementKeys.Add( ReportDefinitionHelper.EnsureElementId( element ) );
                    collidingElementKeys.Add( ReportDefinitionHelper.EnsureElementId( sibling ) );
                }
            }

            foreach ( IList<ReportElementDefinition> childElements in ReportDefinitionHelper.GetChildElementCollections( element ) )
                FindCollisions( childElements, collidingElementKeys );
        }
    }

    private static bool CanCollide( ReportElementDefinition element, ReportElementType? elementType )
    {
        return elementType is not null
            && elementType != ReportElementType.PageBreak
            && element?.Suppress?.Value != true;
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