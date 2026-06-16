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

    private bool shouldRender;

    #endregion

    #region Methods

    /// <summary>
    /// Shows the context menu with the supplied state.
    /// </summary>
    /// <param name="state">Context menu state to render.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal Task ShowAsync( ReportContextMenuState state )
    {
        State = state;
        shouldRender = true;

        return InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Hides the context menu.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal Task CloseAsync()
    {
        State = null;
        shouldRender = true;

        return InvokeAsync( StateHasChanged );
    }

    /// <inheritdoc />
    protected override bool ShouldRender()
    {
        if ( shouldRender )
        {
            shouldRender = false;
            return true;
        }

        return false;
    }

    #endregion

    #region Properties

    private bool IsVisible => State?.Visible == true;

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
    /// Raised when the context menu should be closed.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Close { get; set; }

    #endregion
}