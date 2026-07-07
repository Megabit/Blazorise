#region Using directives
using System;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportContextMenuService
{
    #region Methods

    private readonly ReportTableEditor tableEditor = new();

    internal void PopulateSectionCapabilities(
        ReportDefinition definition,
        ReportContextMenuState state,
        bool hasClipboardElements,
        Func<ReportDefinition, int, bool> canInsertSection,
        Func<ReportSectionDefinition, bool> canInsertGroup )
    {
        if ( definition is null
            || state is null
            || state.SectionIndex < 0
            || state.SectionIndex >= definition.Sections.Count )
        {
            return;
        }

        ReportSectionDefinition section = definition.Sections[state.SectionIndex];

        state.SectionSuppressed = section.Suppressed;
        state.CanPasteElement = CanPasteElement( definition, state, hasClipboardElements );
        state.CanSelectAllSectionElements = section.Elements?.Count > 0;
        state.CanInsertSection = canInsertSection?.Invoke( definition, state.SectionIndex ) == true;
        state.CanInsertGroup = canInsertGroup?.Invoke( section ) == true;
        state.CanDeleteSection = ReportDefinitionHelper.CanDeleteSection( section );
        state.SectionKeepTogether = section.KeepTogether?.Value == true;
        state.SectionNewPageBefore = section.NewPageBefore?.Value == true;
        state.SectionNewPageAfter = section.NewPageAfter?.Value == true;
    }

    internal void PopulateElementCapabilities( ReportDefinition definition, ReportContextMenuState state, bool hasClipboardElements )
    {
        if ( definition is null
            || state is null
            || !ReportDefinitionHelper.TryFindElementLocation( definition, state.ElementKey, out int sectionIndex, out _, out ReportElementDefinition element ) )
        {
            return;
        }

        state.CanEditText = CanEditElementText( element );
        state.CanEditFormula = CanEditFormulaFieldElement( definition, element );
        state.CanEditRunningTotal = CanEditRunningTotalElement( definition, element );
        state.CanPasteElement = hasClipboardElements;
        state.ElementCanGrow = element.CanGrow?.Value == true;
        state.ElementSuppressed = element.Suppress?.Value == true;
        state.CanOrderSelectedElements = state.SelectedElementCount > 0;
        state.CanAlignOrSizeSelectedElements = state.SelectedElementCount >= ReportDesignerConstants.MinimumBatchElementCount;
        state.CanInsertAggregate = sectionIndex >= 0
            && sectionIndex < definition.Sections.Count
            && definition.Sections[sectionIndex].Type == ReportSectionType.Detail
            && element is ReportFieldElementDefinition;
    }

    internal void PopulateTableCellCapabilities( ReportDefinition definition, ReportContextMenuState state, bool hasClipboardElements )
    {
        if ( definition is null
            || state is null
            || !ReportDefinitionHelper.TryFindTableCellLocation( definition, state.CellKey, out _, out _, out ReportTableElementDefinition table, out ReportTableCellDefinition cell ) )
        {
            return;
        }

        state.CanPasteElement = hasClipboardElements;
        state.CanMergeCellRight = tableEditor.CanMergeCellRight( table, cell );
        state.CanMergeCellDown = tableEditor.CanMergeCellDown( table, cell );
        state.CanUnmergeCell = cell.RowSpan > 1 || cell.ColumnSpan > 1;
        state.CanInsertTableRowAbove = true;
        state.CanInsertTableRowBelow = true;
        state.CanInsertTableColumnLeft = true;
        state.CanInsertTableColumnRight = true;
        state.CanInsertTableCell = tableEditor.CanInsertCell( cell );
        state.CanDeleteTableRow = tableEditor.CanDeleteRow( table );
        state.CanDeleteTableColumn = tableEditor.CanDeleteColumn( table );
        state.CanDeleteTableCell = tableEditor.CanDeleteCell( table, cell );
    }

    internal bool CanPasteElement( ReportDefinition definition, ReportContextMenuState state, bool hasClipboardElements )
    {
        return hasClipboardElements
            && definition?.Sections is not null
            && state?.Target is ReportContextMenuTarget.Section or ReportContextMenuTarget.Cell
            && state.HasPastePosition
            && state.SectionIndex >= 0
            && state.SectionIndex < definition.Sections.Count;
    }

    internal bool CanEditElementText( ReportElementDefinition element )
    {
        return element is ReportTextElementDefinition;
    }

    internal bool TryGetElementFormulaFieldName( ReportDefinition definition, string elementKey, out string formulaFieldName )
    {
        formulaFieldName = null;

        if ( !ReportDefinitionHelper.TryFindElementLocation( definition, elementKey, out _, out _, out ReportElementDefinition element )
            || element is not ReportFieldElementDefinition fieldElement
            || string.IsNullOrWhiteSpace( fieldElement.Field ) )
        {
            return false;
        }

        string normalizedFieldName = ReportFormulaFieldResolver.NormalizeFieldName( fieldElement.Field );

        if ( !ReportFormulaFieldResolver.IsFormulaField( definition, normalizedFieldName ) )
            return false;

        formulaFieldName = normalizedFieldName;

        return true;
    }

    internal bool TryGetElementRunningTotalName( ReportDefinition definition, string elementKey, out string runningTotalName )
    {
        runningTotalName = null;

        if ( !ReportDefinitionHelper.TryFindElementLocation( definition, elementKey, out _, out _, out ReportElementDefinition element )
            || element is not ReportFieldElementDefinition fieldElement
            || string.IsNullOrWhiteSpace( fieldElement.Field ) )
        {
            return false;
        }

        string normalizedFieldName = ReportRunningTotalResolver.NormalizeFieldName( fieldElement.Field );

        if ( !ReportRunningTotalResolver.IsRunningTotalField( definition, normalizedFieldName ) )
            return false;

        runningTotalName = normalizedFieldName;

        return true;
    }

    private static bool CanEditFormulaFieldElement( ReportDefinition definition, ReportElementDefinition element )
    {
        return element is ReportFieldElementDefinition fieldElement
            && !string.IsNullOrWhiteSpace( fieldElement.Field )
            && ReportFormulaFieldResolver.IsFormulaField( definition, ReportFormulaFieldResolver.NormalizeFieldName( fieldElement.Field ) );
    }

    private static bool CanEditRunningTotalElement( ReportDefinition definition, ReportElementDefinition element )
    {
        return element is ReportFieldElementDefinition fieldElement
            && !string.IsNullOrWhiteSpace( fieldElement.Field )
            && ReportRunningTotalResolver.IsRunningTotalField( definition, ReportRunningTotalResolver.NormalizeFieldName( fieldElement.Field ) );
    }

    #endregion
}