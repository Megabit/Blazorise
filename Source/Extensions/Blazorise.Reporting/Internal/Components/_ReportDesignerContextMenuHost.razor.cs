#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Hosts the report designer context menu in its own render boundary.
/// </summary>
public partial class _ReportDesignerContextMenuHost
{
    #region Members

    private ContextMenu contextMenuRef;

    #endregion

    #region Methods

    /// <summary>
    /// Shows the context menu with the supplied state.
    /// </summary>
    /// <param name="state">Context menu state to render.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal async Task Show( ReportContextMenuState state )
    {
        State = state;

        await InvokeAsync( StateHasChanged );

        if ( contextMenuRef is not null )
            await contextMenuRef.Show( state.ClientX, state.ClientY );
    }

    /// <summary>
    /// Hides the context menu.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal async Task CloseMenu()
    {
        State = null;

        if ( contextMenuRef is not null )
            await contextMenuRef.Hide();

        await InvokeAsync( StateHasChanged );
    }

    private static Task InvokeMenuCommand( EventCallback<MouseEventArgs> callback )
        => callback.InvokeAsync( default );

    private Task OnPasteElementClicked( object value )
        => InvokeMenuCommand( PasteElement );

    private Task OnSelectAllSectionElementsClicked( object value )
        => InvokeMenuCommand( SelectAllSectionElements );

    private Task OnDeleteSectionClicked( object value )
        => InvokeMenuCommand( DeleteSection );

    private Task OnShowPropertiesClicked( object value )
        => InvokeMenuCommand( ShowProperties );

    private Task OnToggleSectionSuppressionClicked( object value )
        => InvokeMenuCommand( ToggleSectionSuppression );

    private Task OnInsertSectionBeforeClicked( object value )
        => InvokeMenuCommand( InsertSectionBefore );

    private Task OnInsertSectionAfterClicked( object value )
        => InvokeMenuCommand( InsertSectionAfter );

    private Task OnInsertGroupClicked( object value )
        => InvokeMenuCommand( InsertGroup );

    private Task OnToggleSectionKeepTogetherClicked( object value )
        => InvokeMenuCommand( ToggleSectionKeepTogether );

    private Task OnToggleSectionNewPageBeforeClicked( object value )
        => InvokeMenuCommand( ToggleSectionNewPageBefore );

    private Task OnToggleSectionNewPageAfterClicked( object value )
        => InvokeMenuCommand( ToggleSectionNewPageAfter );

    private Task OnCutElementClicked( object value )
        => InvokeMenuCommand( CutElement );

    private Task OnCopyElementClicked( object value )
        => InvokeMenuCommand( CopyElement );

    private Task OnDeleteElementClicked( object value )
        => InvokeMenuCommand( DeleteElement );

    private Task OnBringToFrontClicked( object value )
        => InvokeMenuCommand( BringToFront );

    private Task OnSendToBackClicked( object value )
        => InvokeMenuCommand( SendToBack );

    private Task OnMoveForwardClicked( object value )
        => InvokeMenuCommand( MoveForward );

    private Task OnMoveBackwardClicked( object value )
        => InvokeMenuCommand( MoveBackward );

    private Task OnAlignTopsClicked( object value )
        => InvokeMenuCommand( AlignTops );

    private Task OnAlignMiddlesClicked( object value )
        => InvokeMenuCommand( AlignMiddles );

    private Task OnAlignBottomsClicked( object value )
        => InvokeMenuCommand( AlignBottoms );

    private Task OnAlignBaselineClicked( object value )
        => InvokeMenuCommand( AlignBaseline );

    private Task OnAlignLeftsClicked( object value )
        => InvokeMenuCommand( AlignLefts );

    private Task OnAlignCentersClicked( object value )
        => InvokeMenuCommand( AlignCenters );

    private Task OnAlignRightsClicked( object value )
        => InvokeMenuCommand( AlignRights );

    private Task OnAlignToGridClicked( object value )
        => InvokeMenuCommand( AlignToGrid );

    private Task OnSizeSameWidthClicked( object value )
        => InvokeMenuCommand( SizeSameWidth );

    private Task OnSizeSameHeightClicked( object value )
        => InvokeMenuCommand( SizeSameHeight );

    private Task OnSizeSameSizeClicked( object value )
        => InvokeMenuCommand( SizeSameSize );

    private Task OnInsertAggregateClicked( object value )
        => InvokeMenuCommand( InsertAggregate );

    private Task OnEditTextClicked( object value )
        => InvokeMenuCommand( EditText );

    private Task OnEditFormulaClicked( object value )
        => InvokeMenuCommand( EditFormula );

    private Task OnEditRunningTotalClicked( object value )
        => InvokeMenuCommand( EditRunningTotal );

    private Task OnToggleElementCanGrowClicked( object value )
        => InvokeMenuCommand( ToggleElementCanGrow );

    private Task OnToggleElementSuppressionClicked( object value )
        => InvokeMenuCommand( ToggleElementSuppression );

    private Task OnMergeCellRightClicked( object value )
        => InvokeMenuCommand( MergeCellRight );

    private Task OnMergeCellDownClicked( object value )
        => InvokeMenuCommand( MergeCellDown );

    private Task OnUnmergeCellClicked( object value )
        => InvokeMenuCommand( UnmergeCell );

    private Task OnInsertTableRowAboveClicked( object value )
        => InvokeMenuCommand( InsertTableRowAbove );

    private Task OnInsertTableRowBelowClicked( object value )
        => InvokeMenuCommand( InsertTableRowBelow );

    private Task OnInsertTableColumnLeftClicked( object value )
        => InvokeMenuCommand( InsertTableColumnLeft );

    private Task OnInsertTableColumnRightClicked( object value )
        => InvokeMenuCommand( InsertTableColumnRight );

    private Task OnInsertTableCellClicked( object value )
        => InvokeMenuCommand( InsertTableCell );

    private Task OnDeleteTableRowClicked( object value )
        => InvokeMenuCommand( DeleteTableRow );

    private Task OnDeleteTableColumnClicked( object value )
        => InvokeMenuCommand( DeleteTableColumn );

    private Task OnDeleteTableCellClicked( object value )
        => InvokeMenuCommand( DeleteTableCell );

    private Task OnCloseClicked( object value )
        => InvokeMenuCommand( Close );

    #endregion

    #region Properties

    private bool IsSectionMenu => State?.Target == ReportContextMenuTarget.Section;

    private bool IsElementMenu => State?.Target == ReportContextMenuTarget.Element;

    private bool IsCellMenu => State?.Target == ReportContextMenuTarget.Cell;

    /// <summary>
    /// Current context menu state.
    /// </summary>
    internal ReportContextMenuState State { get; private set; }

    /// <summary>
    /// Raised when the selected element should be cut.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> CutElement { get; set; }

    /// <summary>
    /// Raised when the selected element should be copied.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> CopyElement { get; set; }

    /// <summary>
    /// Raised when a copied element should be pasted.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> PasteElement { get; set; }

    /// <summary>
    /// Raised when all elements in the selected section should be selected.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> SelectAllSectionElements { get; set; }

    /// <summary>
    /// Raised when the properties panel should be shown.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> ShowProperties { get; set; }

    /// <summary>
    /// Raised when the selected section should insert a section before it.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> InsertSectionBefore { get; set; }

    /// <summary>
    /// Raised when the selected section should insert a section after it.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> InsertSectionAfter { get; set; }

    /// <summary>
    /// Raised when a group should be inserted.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> InsertGroup { get; set; }

    /// <summary>
    /// Raised when section suppression should be toggled.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> ToggleSectionSuppression { get; set; }

    /// <summary>
    /// Raised when section keep-together should be toggled.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> ToggleSectionKeepTogether { get; set; }

    /// <summary>
    /// Raised when section new-page-before should be toggled.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> ToggleSectionNewPageBefore { get; set; }

    /// <summary>
    /// Raised when section new-page-after should be toggled.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> ToggleSectionNewPageAfter { get; set; }

    /// <summary>
    /// Raised when the selected section should be deleted.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> DeleteSection { get; set; }

    /// <summary>
    /// Raised when selected elements should be aligned to top.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> AlignTops { get; set; }

    /// <summary>
    /// Raised when selected elements should be vertically centered.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> AlignMiddles { get; set; }

    /// <summary>
    /// Raised when selected elements should be aligned to bottom.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> AlignBottoms { get; set; }

    /// <summary>
    /// Raised when selected elements should be baseline aligned.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> AlignBaseline { get; set; }

    /// <summary>
    /// Raised when selected elements should be aligned to left.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> AlignLefts { get; set; }

    /// <summary>
    /// Raised when selected elements should be horizontally centered.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> AlignCenters { get; set; }

    /// <summary>
    /// Raised when selected elements should be aligned to right.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> AlignRights { get; set; }

    /// <summary>
    /// Raised when selected elements should be aligned to the report grid.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> AlignToGrid { get; set; }

    /// <summary>
    /// Raised when selected elements should use the same width.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> SizeSameWidth { get; set; }

    /// <summary>
    /// Raised when selected elements should use the same height.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> SizeSameHeight { get; set; }

    /// <summary>
    /// Raised when selected elements should use the same size.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> SizeSameSize { get; set; }

    /// <summary>
    /// Raised when selected elements should be brought to the front.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> BringToFront { get; set; }

    /// <summary>
    /// Raised when selected elements should be sent to the back.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> SendToBack { get; set; }

    /// <summary>
    /// Raised when selected elements should move one layer forward.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> MoveForward { get; set; }

    /// <summary>
    /// Raised when selected elements should move one layer backward.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> MoveBackward { get; set; }

    /// <summary>
    /// Raised when an aggregate should be inserted.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> InsertAggregate { get; set; }

    /// <summary>
    /// Raised when a text element should be edited inline.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> EditText { get; set; }

    /// <summary>
    /// Raised when a formula field should be edited.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> EditFormula { get; set; }

    /// <summary>
    /// Raised when a running total should be edited.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> EditRunningTotal { get; set; }

    /// <summary>
    /// Raised when the selected element should be deleted.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> DeleteElement { get; set; }

    /// <summary>
    /// Raised when selected element can-grow should be toggled.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> ToggleElementCanGrow { get; set; }

    /// <summary>
    /// Raised when selected element suppression should be toggled.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> ToggleElementSuppression { get; set; }

    /// <summary>
    /// Raised when the selected table cell should merge with the cell to its right.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> MergeCellRight { get; set; }

    /// <summary>
    /// Raised when the selected table cell should merge with the cell below it.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> MergeCellDown { get; set; }

    /// <summary>
    /// Raised when the selected table cell should be split back to a single cell.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> UnmergeCell { get; set; }

    /// <summary>
    /// Raised when a table row should be inserted above the selected cell.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> InsertTableRowAbove { get; set; }

    /// <summary>
    /// Raised when a table row should be inserted below the selected cell.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> InsertTableRowBelow { get; set; }

    /// <summary>
    /// Raised when a table column should be inserted to the left of the selected cell.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> InsertTableColumnLeft { get; set; }

    /// <summary>
    /// Raised when a table column should be inserted to the right of the selected cell.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> InsertTableColumnRight { get; set; }

    /// <summary>
    /// Raised when a table cell should be inserted relative to the selected cell.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> InsertTableCell { get; set; }

    /// <summary>
    /// Raised when the selected table row should be deleted.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> DeleteTableRow { get; set; }

    /// <summary>
    /// Raised when the selected table column should be deleted.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> DeleteTableColumn { get; set; }

    /// <summary>
    /// Raised when the selected table cell should be deleted.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> DeleteTableCell { get; set; }

    /// <summary>
    /// Raised when the context menu should be closed.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Close { get; set; }

    #endregion
}