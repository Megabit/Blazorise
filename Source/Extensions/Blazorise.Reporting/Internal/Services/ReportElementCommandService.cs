#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportElementCommandService
{
    #region Members

    private readonly ReportElementLayoutService layoutService;

    #endregion

    #region Constructors

    internal ReportElementCommandService( ReportElementLayoutService layoutService )
    {
        this.layoutService = layoutService;
    }

    #endregion

    #region Methods

    internal ReportElementCommandResult MoveElement( ReportDefinition definition, ReportElementDefinition element, double x, double y, double width, double height, Func<ReportElementDefinition, bool> isSnapToGridEnabled, Func<double, bool, double> applyGrid )
    {
        if ( definition is null || element is null || element.Suppress?.Value == true )
            return new();

        ReportDefinitionHelper.TryFindElementLocation( definition, ReportDefinitionHelper.EnsureElementId( element ), out int sectionIndex, out _, out _ );
        double originalX = element.X;
        double originalWidth = element.Width;
        double originalHeight = element.Height;
        bool useSnapToGrid = isSnapToGridEnabled( element );

        element.X = applyGrid( element.X + x, useSnapToGrid );
        element.Y = applyGrid( element.Y + y, useSnapToGrid );
        element.Width = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, width == 0 ? element.Width : applyGrid( element.Width + width, useSnapToGrid ) );
        element.Height = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, height == 0 ? element.Height : applyGrid( element.Height + height, useSnapToGrid ) );

        if ( element is ReportTableElementDefinition table )
            ReportDefinitionHelper.ScaleTableLayout( table, originalWidth, originalHeight );

        ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement( definition, sectionIndex, sectionIndex, element, originalX, originalWidth, element.X, element.Width );

        return new()
        {
            Changed = true,
            PrimaryElementKey = ReportDefinitionHelper.EnsureElementId( element ),
        };
    }

    internal ReportElementCommandResult MoveElements( ReportDefinition definition, IReadOnlyList<ReportElementPointerItemState> selectedElements, string primaryElementKey, double x, double y, bool useSnapToGrid, Func<double, bool, double> applyGrid )
    {
        if ( definition is null || selectedElements is null || selectedElements.Count == 0 )
            return new();

        List<string> selectedElementKeys = selectedElements.Select( item => item.ElementKey ).ToList();
        HashSet<int> affectedSectionIndexes = [];

        foreach ( ReportElementPointerItemState item in selectedElements )
        {
            if ( !ReportDefinitionHelper.TryFindElementLocation( definition, item.ElementKey, out int sectionIndex, out _, out ReportElementDefinition element ) )
                continue;

            if ( element.Suppress?.Value == true )
                continue;

            element.X = ReportLayoutGeometry.Clamp( applyGrid( item.OriginalX + x, useSnapToGrid ), 0, Math.Max( 0, definition.Page.Width - element.Width ) );
            element.Y = applyGrid( item.OriginalY + y, useSnapToGrid );

            ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement(
                definition,
                sectionIndex,
                sectionIndex,
                element,
                item.OriginalX,
                item.OriginalWidth,
                element.X,
                element.Width,
                selectedElementKeys );

            affectedSectionIndexes.Add( sectionIndex );
        }

        GrowSections( definition, affectedSectionIndexes );

        return new()
        {
            Changed = true,
            SelectedElementKeys = selectedElementKeys,
            PrimaryElementKey = primaryElementKey,
        };
    }

    internal ReportElementCommandResult AlignElements( ReportDefinition definition, List<ReportSelectedElementContext> selectedElements, ReportElementAlignment alignment, Func<double, bool, double> applyGrid )
    {
        if ( selectedElements is null || selectedElements.Count < ReportDesignerConstants.MinimumBatchElementCount )
            return new();

        ReportSelectedElementContext anchor = selectedElements[0];
        List<string> selectedElementKeys = selectedElements.Select( item => item.ElementKey ).ToList();
        HashSet<int> affectedSectionIndexes = [];
        IEnumerable<ReportSelectedElementContext> elementsToAlign = alignment == ReportElementAlignment.ToGrid
            ? selectedElements
            : selectedElements.Skip( 1 );

        foreach ( ReportSelectedElementContext item in elementsToAlign )
        {
            ReportElementDefinition element = item.Element;

            if ( element.Suppress?.Value == true )
                continue;

            double originalX = element.X;
            double originalWidth = element.Width;

            layoutService.ApplyAlignment( definition, anchor.Element, element, alignment, applyGrid );

            ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement(
                definition,
                item.SectionIndex,
                item.SectionIndex,
                element,
                originalX,
                originalWidth,
                element.X,
                element.Width,
                selectedElementKeys );

            affectedSectionIndexes.Add( item.SectionIndex );
        }

        GrowSections( definition, affectedSectionIndexes );

        return new()
        {
            Changed = true,
            SelectedElementKeys = selectedElementKeys,
            PrimaryElementKey = anchor.ElementKey,
        };
    }

    internal ReportElementCommandResult SizeElements( ReportDefinition definition, List<ReportSelectedElementContext> selectedElements, ReportElementSizeMode sizeMode )
    {
        if ( selectedElements is null || selectedElements.Count < ReportDesignerConstants.MinimumBatchElementCount )
            return new();

        ReportSelectedElementContext anchor = selectedElements[0];
        List<string> selectedElementKeys = selectedElements.Select( item => item.ElementKey ).ToList();
        HashSet<int> affectedSectionIndexes = [];

        foreach ( ReportSelectedElementContext item in selectedElements.Skip( 1 ) )
        {
            ReportElementDefinition element = item.Element;

            if ( element.Suppress?.Value == true )
                continue;

            double originalX = element.X;
            double originalWidth = element.Width;
            double originalHeight = element.Height;

            layoutService.ApplySize( definition, anchor.Element, element, sizeMode );

            if ( element is ReportTableElementDefinition table )
                ReportDefinitionHelper.ScaleTableLayout( table, originalWidth, originalHeight );

            ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement(
                definition,
                item.SectionIndex,
                item.SectionIndex,
                element,
                originalX,
                originalWidth,
                element.X,
                element.Width,
                selectedElementKeys );

            affectedSectionIndexes.Add( item.SectionIndex );
        }

        GrowSections( definition, affectedSectionIndexes );

        return new()
        {
            Changed = true,
            SelectedElementKeys = selectedElementKeys,
            PrimaryElementKey = anchor.ElementKey,
        };
    }

    internal ReportElementCommandResult OrderElements( List<ReportSelectedElementContext> selectedElements, ReportElementOrderMode orderMode )
    {
        if ( selectedElements is null || selectedElements.Count == 0 )
            return new();

        foreach ( IGrouping<IList<ReportElementDefinition>, ReportSelectedElementContext> group in selectedElements.GroupBy( item => item.OwnerElements ) )
        {
            layoutService.ReorderElements( group.Key, group.Select( item => item.Element ), orderMode );
        }

        return new()
        {
            Changed = true,
            SelectedElementKeys = selectedElements.Select( item => item.ElementKey ).ToList(),
            PrimaryElementKey = selectedElements[0].ElementKey,
        };
    }

    internal ReportElementCommandResult UpdateElements( ReportDefinition definition, List<ReportSelectedElementContext> selectedElements, Action<ReportElementDefinition> update )
    {
        if ( selectedElements is null || selectedElements.Count == 0 )
            return new();

        List<string> selectedElementKeys = selectedElements.Select( item => item.ElementKey ).ToList();
        HashSet<int> affectedSectionIndexes = [];

        foreach ( ReportSelectedElementContext item in selectedElements )
        {
            ReportElementDefinition element = item.Element;

            if ( element is null )
                continue;

            double originalX = element.X;
            double originalWidth = element.Width;
            double originalHeight = element.Height;

            update?.Invoke( element );

            if ( element is ReportTableElementDefinition table )
                ReportDefinitionHelper.ScaleTableLayout( table, originalWidth, originalHeight );

            ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement( definition, item.SectionIndex, item.SectionIndex, element, originalX, originalWidth, element.X, element.Width, selectedElementKeys );
            affectedSectionIndexes.Add( item.SectionIndex );
        }

        GrowSections( definition, affectedSectionIndexes );

        return new()
        {
            Changed = true,
            SelectedElementKeys = selectedElementKeys,
            PrimaryElementKey = selectedElements[0].ElementKey,
        };
    }

    internal ReportElementCommandResult DeleteElements( ReportDefinition definition, List<ReportSelectedElementContext> selectedElements )
    {
        if ( definition is null || selectedElements is null || selectedElements.Count == 0 )
            return new();

        int? selectedSectionIndex = selectedElements[0].SectionIndex;

        foreach ( IGrouping<IList<ReportElementDefinition>, ReportSelectedElementContext> group in selectedElements.GroupBy( item => item.OwnerElements ) )
        {
            foreach ( ReportSelectedElementContext item in group.OrderByDescending( selectedElement => group.Key.IndexOf( selectedElement.Element ) ) )
            {
                group.Key.Remove( item.Element );
            }
        }

        return new()
        {
            Changed = true,
            SelectedSectionIndex = selectedSectionIndex,
        };
    }

    private static void GrowSections( ReportDefinition definition, IEnumerable<int> sectionIndexes )
    {
        foreach ( int sectionIndex in sectionIndexes )
        {
            ReportLayoutGeometry.GrowSectionToFitElements( definition.Sections[sectionIndex] );
        }
    }

    #endregion
}