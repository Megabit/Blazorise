#region Using directives
using System;
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

    private static Task InvokeMenuCommand( Func<MouseEventArgs, Task> callback )
    {
        if ( callback is not null )
            return callback.Invoke( default );

        return Task.CompletedTask;
    }

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
    [Parameter] public Func<MouseEventArgs, Task> CutElement { get; set; }

    /// <summary>
    /// Raised when the selected element should be copied.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> CopyElement { get; set; }

    /// <summary>
    /// Raised when a copied element should be pasted.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> PasteElement { get; set; }

    /// <summary>
    /// Raised when all elements in the selected section should be selected.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> SelectAllSectionElements { get; set; }

    /// <summary>
    /// Raised when the properties panel should be shown.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> ShowProperties { get; set; }

    /// <summary>
    /// Raised when the selected section should insert a section before it.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> InsertSectionBefore { get; set; }

    /// <summary>
    /// Raised when the selected section should insert a section after it.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> InsertSectionAfter { get; set; }

    /// <summary>
    /// Raised when a group should be inserted.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> InsertGroup { get; set; }

    /// <summary>
    /// Raised when section suppression should be toggled.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> ToggleSectionSuppression { get; set; }

    /// <summary>
    /// Raised when section keep-together should be toggled.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> ToggleSectionKeepTogether { get; set; }

    /// <summary>
    /// Raised when section new-page-before should be toggled.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> ToggleSectionNewPageBefore { get; set; }

    /// <summary>
    /// Raised when section new-page-after should be toggled.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> ToggleSectionNewPageAfter { get; set; }

    /// <summary>
    /// Raised when the selected section should be deleted.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> DeleteSection { get; set; }

    /// <summary>
    /// Raised when selected elements should be aligned to top.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> AlignTops { get; set; }

    /// <summary>
    /// Raised when selected elements should be vertically centered.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> AlignMiddles { get; set; }

    /// <summary>
    /// Raised when selected elements should be aligned to bottom.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> AlignBottoms { get; set; }

    /// <summary>
    /// Raised when selected elements should be baseline aligned.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> AlignBaseline { get; set; }

    /// <summary>
    /// Raised when selected elements should be aligned to left.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> AlignLefts { get; set; }

    /// <summary>
    /// Raised when selected elements should be horizontally centered.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> AlignCenters { get; set; }

    /// <summary>
    /// Raised when selected elements should be aligned to right.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> AlignRights { get; set; }

    /// <summary>
    /// Raised when selected elements should be aligned to the report grid.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> AlignToGrid { get; set; }

    /// <summary>
    /// Raised when selected elements should use the same width.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> SizeSameWidth { get; set; }

    /// <summary>
    /// Raised when selected elements should use the same height.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> SizeSameHeight { get; set; }

    /// <summary>
    /// Raised when selected elements should use the same size.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> SizeSameSize { get; set; }

    /// <summary>
    /// Raised when selected elements should be brought to the front.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> BringToFront { get; set; }

    /// <summary>
    /// Raised when selected elements should be sent to the back.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> SendToBack { get; set; }

    /// <summary>
    /// Raised when selected elements should move one layer forward.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> MoveForward { get; set; }

    /// <summary>
    /// Raised when selected elements should move one layer backward.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> MoveBackward { get; set; }

    /// <summary>
    /// Raised when an aggregate should be inserted.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> InsertAggregate { get; set; }

    /// <summary>
    /// Raised when a text element should be edited inline.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> EditText { get; set; }

    /// <summary>
    /// Raised when a formula field should be edited.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> EditFormula { get; set; }

    /// <summary>
    /// Raised when a running total should be edited.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> EditRunningTotal { get; set; }

    /// <summary>
    /// Raised when the selected element should be deleted.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> DeleteElement { get; set; }

    /// <summary>
    /// Raised when selected element can-grow should be toggled.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> ToggleElementCanGrow { get; set; }

    /// <summary>
    /// Raised when selected element suppression should be toggled.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> ToggleElementSuppression { get; set; }

    /// <summary>
    /// Raised when the selected table cell should merge with the cell to its right.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> MergeCellRight { get; set; }

    /// <summary>
    /// Raised when the selected table cell should merge with the cell below it.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> MergeCellDown { get; set; }

    /// <summary>
    /// Raised when the selected table cell should be split back to a single cell.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> UnmergeCell { get; set; }

    /// <summary>
    /// Raised when a table row should be inserted above the selected cell.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> InsertTableRowAbove { get; set; }

    /// <summary>
    /// Raised when a table row should be inserted below the selected cell.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> InsertTableRowBelow { get; set; }

    /// <summary>
    /// Raised when a table column should be inserted to the left of the selected cell.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> InsertTableColumnLeft { get; set; }

    /// <summary>
    /// Raised when a table column should be inserted to the right of the selected cell.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> InsertTableColumnRight { get; set; }

    /// <summary>
    /// Raised when a table cell should be inserted relative to the selected cell.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> InsertTableCell { get; set; }

    /// <summary>
    /// Raised when the selected table row should be deleted.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> DeleteTableRow { get; set; }

    /// <summary>
    /// Raised when the selected table column should be deleted.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> DeleteTableColumn { get; set; }

    /// <summary>
    /// Raised when the selected table cell should be deleted.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> DeleteTableCell { get; set; }

    /// <summary>
    /// Raised when the context menu should be closed.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> Close { get; set; }

    #endregion
}