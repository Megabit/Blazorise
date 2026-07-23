#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportClipboardService
{
    #region Methods

    internal ReportClipboardResult CopyElements( ReportDefinition definition, IReadOnlyList<ReportSelectedElementContext> selectedElements )
    {
        if ( selectedElements is null || selectedElements.Count == 0 )
            return new();

        List<ReportElementDefinition> clipboardElements = selectedElements
            .Where( item => item.Element is not null )
            .Select( item => ReportContext.CloneElement( item.Element ) )
            .ToList();

        if ( clipboardElements.Count == 0 )
            return new();

        return new()
        {
            ClipboardElements = clipboardElements,
            ClipboardBandId = ResolveClipboardBandId( definition, selectedElements ),
        };
    }

    internal ReportClipboardResult CutElements( ReportDefinition definition, IReadOnlyList<ReportSelectedElementContext> selectedElements )
    {
        if ( selectedElements is null || selectedElements.Count == 0 )
            return new();

        ReportClipboardResult result = CopyElements( definition, selectedElements );

        if ( result.ClipboardElements.Count == 0 )
            return new();

        foreach ( IGrouping<IList<ReportElementDefinition>, ReportSelectedElementContext> group in selectedElements.GroupBy( item => item.OwnerElements ) )
        {
            foreach ( ReportSelectedElementContext item in group.OrderByDescending( selectedElement => group.Key.IndexOf( selectedElement.Element ) ) )
            {
                group.Key.Remove( item.Element );
            }
        }

        result.Changed = true;
        result.SelectedSectionIndex = selectedElements[0].SectionIndex;

        return result;
    }

    internal ReportClipboardResult PasteElements(
        ReportDefinition definition,
        IReadOnlyList<ReportElementDefinition> clipboardElements,
        string clipboardBandId,
        ReportContextMenuState contextMenu,
        string selectedCellKey,
        int targetSectionIndex,
        Func<ReportElementDefinition, bool> isSnapToGridEnabled,
        Func<double, bool, double> applyGrid,
        ReportElementLayoutService layoutService,
        ReportTableEditor tableEditor )
    {
        if ( definition is null || clipboardElements is null || clipboardElements.Count == 0 || targetSectionIndex < 0 || targetSectionIndex >= definition.Bands.Count )
            return new();

        ReportBandDefinition targetSection = definition.Bands[targetSectionIndex];
        bool sameSection = clipboardBandId == ReportDefinitionHelper.EnsureBandId( targetSection );
        bool pasteIntoCell = TryResolvePasteCell( definition, contextMenu, selectedCellKey, out ReportTableElementDefinition pasteTable, out ReportTableCellDefinition pasteCell );
        List<ReportElementDefinition> sourceElements = clipboardElements.Where( element => element is not null ).ToList();

        if ( sourceElements.Count == 0 )
            return new();

        double minimumX = sourceElements.Min( element => element.X );
        double minimumY = sourceElements.Min( element => element.Y );
        double pasteBaseX;
        double pasteBaseY;

        if ( TryResolveContextPastePosition( contextMenu, out double pasteX, out double pasteY ) )
        {
            pasteBaseX = pasteX;
            pasteBaseY = pasteY;
        }
        else if ( sameSection )
        {
            pasteBaseX = minimumX + ReportDesignerConstants.PasteElementOffset;
            pasteBaseY = minimumY + ReportDesignerConstants.PasteElementOffset;
        }
        else
        {
            pasteBaseX = 0;
            pasteBaseY = 0;
        }

        if ( pasteIntoCell )
        {
            ReportElementDefinition cellElement = CreatePastedElement( definition, sourceElements[0], minimumX, minimumY, pasteBaseX, pasteBaseY, isSnapToGridEnabled, applyGrid, layoutService );
            tableEditor.ReplaceCellElement( pasteTable, pasteCell, cellElement );

            return new()
            {
                Changed = true,
                SelectedCellKey = pasteCell.Id,
            };
        }

        List<string> selectedElementKeys = [];

        foreach ( ReportElementDefinition sourceElement in sourceElements )
        {
            ReportElementDefinition element = CreatePastedElement( definition, sourceElement, minimumX, minimumY, pasteBaseX, pasteBaseY, isSnapToGridEnabled, applyGrid, layoutService );
            targetSection.Elements.Add( element );
            selectedElementKeys.Add( ReportDefinitionHelper.EnsureElementId( element ) );
        }

        ReportLayoutGeometry.GrowSectionToFitElements( targetSection );

        return new()
        {
            Changed = true,
            PrimaryElementKey = selectedElementKeys.FirstOrDefault(),
            SelectedElementKeys = selectedElementKeys,
        };
    }

    private static ReportElementDefinition CreatePastedElement(
        ReportDefinition definition,
        ReportElementDefinition sourceElement,
        double minimumX,
        double minimumY,
        double pasteBaseX,
        double pasteBaseY,
        Func<ReportElementDefinition, bool> isSnapToGridEnabled,
        Func<double, bool, double> applyGrid,
        ReportElementLayoutService layoutService )
    {
        ReportElementDefinition element = ReportContext.CloneElement( sourceElement );
        ReportDefinitionHelper.RegenerateElementIds( element );
        bool useSnapToGrid = isSnapToGridEnabled( element );

        element.X = layoutService.ClampX( definition, element, applyGrid( pasteBaseX + sourceElement.X - minimumX, useSnapToGrid ) );
        element.Y = applyGrid( Math.Max( 0, pasteBaseY + sourceElement.Y - minimumY ), useSnapToGrid );

        return element;
    }

    private static string ResolveClipboardBandId( ReportDefinition definition, IReadOnlyList<ReportSelectedElementContext> selectedElements )
    {
        List<int> sectionIndexes = selectedElements
            .Select( item => item.SectionIndex )
            .Distinct()
            .ToList();

        if ( sectionIndexes.Count != 1 )
            return null;

        int sectionIndex = sectionIndexes[0];

        return definition is not null && sectionIndex >= 0 && sectionIndex < definition.Bands.Count
            ? ReportDefinitionHelper.EnsureBandId( definition.Bands[sectionIndex] )
            : null;
    }

    internal int ResolvePasteSectionIndex( ReportDefinition definition, ReportContextMenuState contextMenu, Func<ReportDefinition, int> resolveSelectionPasteSectionIndex )
    {
        if ( contextMenu?.Target == ReportContextMenuTarget.Section
            && contextMenu.HasPastePosition
            && contextMenu.SectionIndex >= 0
            && contextMenu.SectionIndex < definition.Bands.Count )
        {
            return contextMenu.SectionIndex;
        }

        if ( contextMenu?.Target == ReportContextMenuTarget.Cell
            && contextMenu.SectionIndex >= 0
            && contextMenu.SectionIndex < definition.Bands.Count )
        {
            return contextMenu.SectionIndex;
        }

        return resolveSelectionPasteSectionIndex( definition );
    }

    private static bool TryResolveContextPastePosition( ReportContextMenuState contextMenu, out double x, out double y )
    {
        x = 0;
        y = 0;

        if ( contextMenu?.Target is not ( ReportContextMenuTarget.Section or ReportContextMenuTarget.Cell ) || !contextMenu.HasPastePosition )
            return false;

        x = contextMenu.PasteX;
        y = Math.Max( 0, contextMenu.PasteY );

        return true;
    }

    private static bool TryResolvePasteCell( ReportDefinition definition, ReportContextMenuState contextMenu, string selectedCellKey, out ReportTableElementDefinition table, out ReportTableCellDefinition cell )
    {
        table = null;
        cell = null;

        string cellKey = contextMenu?.Target == ReportContextMenuTarget.Cell
            ? contextMenu.CellKey
            : selectedCellKey;

        return ReportDefinitionHelper.TryFindTableCellLocation( definition, cellKey, out _, out _, out table, out cell );
    }

    #endregion
}