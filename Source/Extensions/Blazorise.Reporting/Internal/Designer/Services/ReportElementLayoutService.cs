#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportElementLayoutService
{
    #region Methods

    internal void ApplyAlignment(
        ReportDefinition definition,
        ReportElementDefinition anchor,
        ReportElementDefinition element,
        ReportElementAlignment alignment,
        Func<double, bool, double> applyGrid )
    {
        switch ( alignment )
        {
            case ReportElementAlignment.Tops:
                element.Y = Math.Max( 0, anchor.Y );
                break;
            case ReportElementAlignment.Middles:
                element.Y = Math.Max( 0, anchor.Y + ( anchor.Height - element.Height ) / 2 );
                break;
            case ReportElementAlignment.Bottoms:
                element.Y = Math.Max( 0, anchor.Y + anchor.Height - element.Height );
                break;
            case ReportElementAlignment.Baseline:
                element.Y = Math.Max( 0, anchor.Y + GetBaselineOffset( anchor ) - GetBaselineOffset( element ) );
                break;
            case ReportElementAlignment.Lefts:
                element.X = ClampX( definition, element, anchor.X );
                break;
            case ReportElementAlignment.Centers:
                element.X = ClampX( definition, element, anchor.X + ( anchor.Width - element.Width ) / 2 );
                break;
            case ReportElementAlignment.Rights:
                element.X = ClampX( definition, element, anchor.X + anchor.Width - element.Width );
                break;
            case ReportElementAlignment.ToGrid:
                element.Width = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, applyGrid( element.Width, true ) );
                element.Height = Math.Max( ReportLayoutGeometry.GetMinimumElementHeight( element ), applyGrid( element.Height, true ) );
                element.X = ClampX( definition, element, applyGrid( element.X, true ) );
                element.Y = applyGrid( element.Y, true );
                break;
        }
    }

    internal void ApplySize( ReportDefinition definition, ReportElementDefinition anchor, ReportElementDefinition element, ReportElementSizeMode sizeMode )
    {
        switch ( sizeMode )
        {
            case ReportElementSizeMode.SameWidth:
                element.Width = ClampWidth( definition, element, anchor.Width );
                break;
            case ReportElementSizeMode.SameHeight:
                element.Height = Math.Max( ReportLayoutGeometry.GetMinimumElementHeight( element ), anchor.Height );
                break;
            case ReportElementSizeMode.SameSize:
                element.Width = ClampWidth( definition, element, anchor.Width );
                element.Height = Math.Max( ReportLayoutGeometry.GetMinimumElementHeight( element ), anchor.Height );
                break;
        }
    }

    internal void ReorderElements( IList<ReportElementDefinition> ownerElements, IEnumerable<ReportElementDefinition> elements, ReportElementOrderMode orderMode )
    {
        if ( ownerElements is null || elements is null )
            return;

        HashSet<ReportElementDefinition> selectedElements = elements.ToHashSet();

        if ( selectedElements.Count == 0 )
            return;

        switch ( orderMode )
        {
            case ReportElementOrderMode.BringToFront:
                BringToFront( ownerElements, selectedElements );
                break;
            case ReportElementOrderMode.SendToBack:
                SendToBack( ownerElements, selectedElements );
                break;
            case ReportElementOrderMode.MoveForward:
                MoveForward( ownerElements, selectedElements );
                break;
            case ReportElementOrderMode.MoveBackward:
                MoveBackward( ownerElements, selectedElements );
                break;
        }
    }

    internal List<ReportSelectedElementContext> GetSelectedElementContexts( ReportDefinition definition, IEnumerable<string> selectedElementKeys, string primaryElementKey )
    {
        if ( definition is null )
            return [];

        List<string> elementKeys = selectedElementKeys?.ToList() ?? [];

        if ( elementKeys.Count == 0 && !string.IsNullOrWhiteSpace( primaryElementKey ) )
            elementKeys.Add( primaryElementKey );

        if ( !string.IsNullOrWhiteSpace( primaryElementKey ) && elementKeys.Remove( primaryElementKey ) )
            elementKeys.Insert( 0, primaryElementKey );

        List<ReportSelectedElementContext> selectedElements = [];

        foreach ( string elementKey in elementKeys.Distinct( StringComparer.Ordinal ) )
        {
            if ( ReportDefinitionHelper.TryFindElementLocation( definition, elementKey, out ReportElementLocation location ) )
            {
                selectedElements.Add( new()
                {
                    ElementKey = elementKey,
                    SectionIndex = location.SectionIndex,
                    Element = location.Element,
                    OwnerElements = location.OwnerElements,
                } );
            }
        }

        return selectedElements;
    }

    internal double ClampX( ReportDefinition definition, ReportElementDefinition element, double x )
    {
        double pageWidth = definition?.Page?.Width ?? ReportDesignerConstants.DefaultPageWidthFallback;
        double maximum = Math.Max( 0, pageWidth - element.Width );

        return ReportLayoutGeometry.Clamp( x, 0, maximum );
    }

    internal double ClampWidth( ReportDefinition definition, ReportElementDefinition element, double width )
    {
        double pageWidth = definition?.Page?.Width ?? ReportDesignerConstants.DefaultPageWidthFallback;
        double maximum = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, pageWidth - element.X );

        return ReportLayoutGeometry.Clamp( width, ReportLayoutGeometry.DefaultMinimumElementSize, maximum );
    }

    internal string GetAlignmentDisplayName( ReportElementAlignment alignment )
    {
        return alignment switch
        {
            ReportElementAlignment.Tops => "tops",
            ReportElementAlignment.Middles => "middles",
            ReportElementAlignment.Bottoms => "bottoms",
            ReportElementAlignment.Baseline => "baseline",
            ReportElementAlignment.Lefts => "lefts",
            ReportElementAlignment.Centers => "centers",
            ReportElementAlignment.Rights => "rights",
            ReportElementAlignment.ToGrid => "to grid",
            _ => alignment.ToString(),
        };
    }

    internal string GetSizeDisplayName( ReportElementSizeMode sizeMode )
    {
        return sizeMode switch
        {
            ReportElementSizeMode.SameWidth => "same width",
            ReportElementSizeMode.SameHeight => "same height",
            ReportElementSizeMode.SameSize => "same size",
            _ => sizeMode.ToString(),
        };
    }

    internal string GetOrderDisplayName( ReportElementOrderMode orderMode )
    {
        return orderMode switch
        {
            ReportElementOrderMode.BringToFront => "Bring to Front",
            ReportElementOrderMode.SendToBack => "Send to Back",
            ReportElementOrderMode.MoveForward => "Move Forward",
            ReportElementOrderMode.MoveBackward => "Move Backward",
            _ => orderMode.ToString(),
        };
    }

    private static void BringToFront( IList<ReportElementDefinition> ownerElements, ISet<ReportElementDefinition> selectedElements )
    {
        List<ReportElementDefinition> movingElements = ownerElements.Where( selectedElements.Contains ).ToList();

        if ( movingElements.Count == 0 )
            return;

        RemoveElements( ownerElements, selectedElements );

        foreach ( ReportElementDefinition element in movingElements )
        {
            ownerElements.Add( element );
        }
    }

    private static void SendToBack( IList<ReportElementDefinition> ownerElements, ISet<ReportElementDefinition> selectedElements )
    {
        List<ReportElementDefinition> movingElements = ownerElements.Where( selectedElements.Contains ).ToList();

        if ( movingElements.Count == 0 )
            return;

        RemoveElements( ownerElements, selectedElements );

        for ( int index = 0; index < movingElements.Count; index++ )
        {
            ownerElements.Insert( index, movingElements[index] );
        }
    }

    private static void MoveForward( IList<ReportElementDefinition> ownerElements, ISet<ReportElementDefinition> selectedElements )
    {
        for ( int index = ownerElements.Count - 2; index >= 0; index-- )
        {
            if ( selectedElements.Contains( ownerElements[index] ) && !selectedElements.Contains( ownerElements[index + 1] ) )
                ( ownerElements[index], ownerElements[index + 1] ) = ( ownerElements[index + 1], ownerElements[index] );
        }
    }

    private static void MoveBackward( IList<ReportElementDefinition> ownerElements, ISet<ReportElementDefinition> selectedElements )
    {
        for ( int index = 1; index < ownerElements.Count; index++ )
        {
            if ( selectedElements.Contains( ownerElements[index] ) && !selectedElements.Contains( ownerElements[index - 1] ) )
                ( ownerElements[index - 1], ownerElements[index] ) = ( ownerElements[index], ownerElements[index - 1] );
        }
    }

    private static void RemoveElements( IList<ReportElementDefinition> ownerElements, ISet<ReportElementDefinition> selectedElements )
    {
        for ( int index = ownerElements.Count - 1; index >= 0; index-- )
        {
            if ( selectedElements.Contains( ownerElements[index] ) )
                ownerElements.RemoveAt( index );
        }
    }

    private static double GetBaselineOffset( ReportElementDefinition element )
    {
        if ( element?.Type is ReportElementType.Text or ReportElementType.Field )
        {
            double fontSize = element.Font?.Size ?? Math.Min( ReportDesignerConstants.DefaultDroppedFieldHeight, element.Height );

            return Math.Min( element.Height, fontSize * ReportDesignerConstants.ElementBaselineFontRatio );
        }

        return element?.Height ?? 0;
    }

    #endregion
}