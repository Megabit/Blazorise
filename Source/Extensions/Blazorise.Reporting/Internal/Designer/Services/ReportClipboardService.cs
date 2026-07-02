#region Using directives
using System;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportClipboardService
{
    #region Methods

    internal ReportClipboardResult CopyElement( ReportDefinition definition, string elementKey )
    {
        if ( !ReportDefinitionHelper.TryFindElementLocation( definition, elementKey, out int sectionIndex, out _, out ReportElementDefinition element ) )
            return new();

        return new()
        {
            ClipboardElement = ReportContext.CloneElement( element ),
            ClipboardSectionId = ReportDefinitionHelper.EnsureSectionId( definition.Sections[sectionIndex] ),
        };
    }

    internal ReportClipboardResult CutElement( ReportDefinition definition, string elementKey )
    {
        if ( !ReportDefinitionHelper.TryFindElementLocation( definition, elementKey, out ReportElementLocation location ) )
            return new();

        ReportElementDefinition element = location.Element;
        ReportClipboardResult result = new()
        {
            Changed = true,
            ClipboardElement = ReportContext.CloneElement( element ),
            ClipboardSectionId = ReportDefinitionHelper.EnsureSectionId( definition.Sections[location.SectionIndex] ),
            SelectedSectionIndex = location.SectionIndex,
        };

        location.OwnerElements.RemoveAt( location.ElementIndex );

        return result;
    }

    internal ReportClipboardResult PasteElement(
        ReportDefinition definition,
        ReportElementDefinition clipboardElement,
        string clipboardSectionId,
        ReportContextMenuState contextMenu,
        int targetSectionIndex,
        Func<ReportElementDefinition, bool> isSnapToGridEnabled,
        Func<double, bool, double> applyGrid,
        ReportElementLayoutService layoutService,
        ReportTableEditor tableEditor )
    {
        if ( definition is null || clipboardElement is null || targetSectionIndex < 0 || targetSectionIndex >= definition.Sections.Count )
            return new();

        ReportSectionDefinition targetSection = definition.Sections[targetSectionIndex];
        bool sameSection = clipboardSectionId == ReportDefinitionHelper.EnsureSectionId( targetSection );
        bool pasteIntoCell = TryResolveContextPasteCell( definition, contextMenu, out ReportTableElementDefinition pasteTable, out ReportTableCellDefinition pasteCell );

        ReportElementDefinition element = ReportContext.CloneElement( clipboardElement );
        element.Id = ReportDefinitionHelper.CreateDefinitionId();
        bool useSnapToGrid = isSnapToGridEnabled( element );

        if ( TryResolveContextPastePosition( contextMenu, out double pasteX, out double pasteY ) )
        {
            element.X = layoutService.ClampX( definition, element, applyGrid( pasteX, useSnapToGrid ) );
            element.Y = applyGrid( pasteY, useSnapToGrid );
        }
        else
        {
            element.X = sameSection ? applyGrid( element.X + ReportDesignerConstants.PasteElementOffset, useSnapToGrid ) : 0;
            element.Y = sameSection ? applyGrid( element.Y + ReportDesignerConstants.PasteElementOffset, useSnapToGrid ) : 0;
        }

        if ( pasteIntoCell )
        {
            tableEditor.ReplaceCellElement( pasteTable, pasteCell, element );

            return new()
            {
                Changed = true,
                SelectedCellKey = pasteCell.Id,
            };
        }

        targetSection.Elements.Add( element );
        ReportLayoutGeometry.GrowSectionToFitElement( targetSection, element );

        return new()
        {
            Changed = true,
            SelectedElementKey = ReportDefinitionHelper.EnsureElementId( element ),
        };
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