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
            ClipboardSectionId = ResolveClipboardSectionId( definition, selectedElements ),
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
        string clipboardSectionId,
        ReportContextMenuState contextMenu,
        int targetSectionIndex,
        Func<ReportElementDefinition, bool> isSnapToGridEnabled,
        Func<double, bool, double> applyGrid,
        ReportElementLayoutService layoutService,
        ReportTableEditor tableEditor )
    {
        if ( definition is null || clipboardElements is null || clipboardElements.Count == 0 || targetSectionIndex < 0 || targetSectionIndex >= definition.Sections.Count )
            return new();

        ReportSectionDefinition targetSection = definition.Sections[targetSectionIndex];
        bool sameSection = clipboardSectionId == ReportDefinitionHelper.EnsureSectionId( targetSection );
        bool pasteIntoCell = TryResolveContextPasteCell( definition, contextMenu, out ReportTableElementDefinition pasteTable, out ReportTableCellDefinition pasteCell );
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
        element.Id = ReportDefinitionHelper.CreateDefinitionId();
        bool useSnapToGrid = isSnapToGridEnabled( element );

        element.X = layoutService.ClampX( definition, element, applyGrid( pasteBaseX + sourceElement.X - minimumX, useSnapToGrid ) );
        element.Y = applyGrid( Math.Max( 0, pasteBaseY + sourceElement.Y - minimumY ), useSnapToGrid );

        return element;
    }

    private static string ResolveClipboardSectionId( ReportDefinition definition, IReadOnlyList<ReportSelectedElementContext> selectedElements )
    {
        List<int> sectionIndexes = selectedElements
            .Select( item => item.SectionIndex )
            .Distinct()
            .ToList();

        if ( sectionIndexes.Count != 1 )
            return null;

        int sectionIndex = sectionIndexes[0];

        return definition is not null && sectionIndex >= 0 && sectionIndex < definition.Sections.Count
            ? ReportDefinitionHelper.EnsureSectionId( definition.Sections[sectionIndex] )
            : null;
    }

    internal int ResolvePasteSectionIndex( ReportDefinition definition, ReportContextMenuState contextMenu, Func<ReportDefinition, int> resolveSelectionPasteSectionIndex )
    {
        if ( contextMenu?.Target == ReportContextMenuTarget.Section
            && contextMenu.HasPastePosition
            && contextMenu.SectionIndex >= 0
            && contextMenu.SectionIndex < definition.Sections.Count )
        {
            return contextMenu.SectionIndex;
        }

        if ( contextMenu?.Target == ReportContextMenuTarget.Cell
            && contextMenu.SectionIndex >= 0
            && contextMenu.SectionIndex < definition.Sections.Count )
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

    private static bool TryResolveContextPasteCell( ReportDefinition definition, ReportContextMenuState contextMenu, out ReportTableElementDefinition table, out ReportTableCellDefinition cell )
    {
        table = null;
        cell = null;

        return contextMenu?.Target == ReportContextMenuTarget.Cell
            && ReportDefinitionHelper.TryFindTableCellLocation( definition, contextMenu.CellKey, out _, out _, out table, out cell );
    }

    #endregion
}