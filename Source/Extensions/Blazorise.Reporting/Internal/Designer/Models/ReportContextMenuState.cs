#region Using directives
using Blazorise.Reporting;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportContextMenuState
{
    #region Properties

    internal bool Visible { get; set; }

    internal ReportContextMenuTarget Target { get; set; }

    internal int SectionIndex { get; set; } = -1;

    internal string ElementKey { get; set; }

    internal string CellKey { get; set; }

    internal int SelectedElementCount { get; set; }

    internal bool SectionSuppressed { get; set; }

    internal bool CanPasteElement { get; set; }

    internal bool CanSelectAllSectionElements { get; set; }

    internal bool CanInsertSection { get; set; }

    internal bool CanInsertGroup { get; set; }

    internal bool CanDeleteSection { get; set; }

    internal bool CanAlignOrSizeSelectedElements { get; set; }

    internal bool CanInsertAggregate { get; set; }

    internal bool CanEditText { get; set; }

    internal bool CanEditFormula { get; set; }

    internal bool CanEditRunningTotal { get; set; }

    internal bool ElementCanGrow { get; set; }

    internal bool ElementSuppressed { get; set; }

    internal bool SectionKeepTogether { get; set; }

    internal bool SectionNewPageBefore { get; set; }

    internal bool SectionNewPageAfter { get; set; }

    internal bool CanMergeCellRight { get; set; }

    internal bool CanMergeCellDown { get; set; }

    internal bool CanUnmergeCell { get; set; }

    internal bool CanShowTableCellMergeOperations => CanMergeCellRight || CanMergeCellDown || CanUnmergeCell;

    internal bool CanInsertTableRowAbove { get; set; }

    internal bool CanInsertTableRowBelow { get; set; }

    internal bool CanInsertTableColumnLeft { get; set; }

    internal bool CanInsertTableColumnRight { get; set; }

    internal bool CanInsertTableCell { get; set; }

    internal bool CanDeleteTableRow { get; set; }

    internal bool CanDeleteTableColumn { get; set; }

    internal bool CanDeleteTableCell { get; set; }

    internal bool HasPastePosition { get; set; }

    internal double PasteX { get; set; }

    internal double PasteY { get; set; }

    internal double ClientX { get; set; }

    internal double ClientY { get; set; }

    #endregion
}