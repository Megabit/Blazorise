#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Provides state and commands for PivotGrid toolbar templates.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridToolbarContext<TItem>
{
    /// <summary>
    /// Initializes a new instance of <see cref="PivotGridToolbarContext{TItem}"/>.
    /// </summary>
    public PivotGridToolbarContext(
        PivotGrid<TItem> pivotGrid,
        Func<Task> openFieldChooserCommand,
        Func<Task> firstPageCommand,
        Func<Task> previousPageCommand,
        Func<Task> nextPageCommand,
        Func<Task> lastPageCommand )
        : this(
            pivotGrid,
            openFieldChooserCommand,
            pivotGrid.Reload,
            pivotGrid.ExpandAllGroups,
            pivotGrid.CollapseAllGroups,
            pivotGrid.ResetLayout,
            firstPageCommand,
            previousPageCommand,
            nextPageCommand,
            lastPageCommand )
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PivotGridToolbarContext{TItem}"/>.
    /// </summary>
    public PivotGridToolbarContext(
        PivotGrid<TItem> pivotGrid,
        Func<Task> openFieldChooserCommand,
        Func<Task> refreshCommand,
        Func<Task> expandAllCommand,
        Func<Task> collapseAllCommand,
        Func<Task> resetLayoutCommand,
        Func<Task> firstPageCommand,
        Func<Task> previousPageCommand,
        Func<Task> nextPageCommand,
        Func<Task> lastPageCommand )
    {
        OpenFieldChooserCommand = openFieldChooserCommand;
        RefreshCommand = refreshCommand;
        ExpandAllCommand = expandAllCommand;
        CollapseAllCommand = collapseAllCommand;
        ResetLayoutCommand = resetLayoutCommand;
        FirstPageCommand = firstPageCommand;
        PreviousPageCommand = previousPageCommand;
        NextPageCommand = nextPageCommand;
        LastPageCommand = lastPageCommand;
        ShowFieldChooser = pivotGrid.ShowFieldChooser;
        CanExpandCollapseGroups = pivotGrid.CanExpandCollapseGroups;
        CanResetLayout = pivotGrid.CanResetLayout;
        FieldsText = pivotGrid.LocalizedFieldsText;
        RefreshText = pivotGrid.LocalizedRefreshText;
        ExpandAllText = pivotGrid.LocalizedExpandAllText;
        CollapseAllText = pivotGrid.LocalizedCollapseAllText;
        ResetLayoutText = pivotGrid.LocalizedResetLayoutText;
        CurrentPage = pivotGrid.CurrentPage;
        LastPage = pivotGrid.LastPage;
        TotalRows = pivotGrid.TotalRows;
        PageSize = pivotGrid.EffectivePageSize;
        PageByGroups = pivotGrid.PageByGroups;
    }

    /// <summary>
    /// Gets whether the field chooser command is available.
    /// </summary>
    public bool ShowFieldChooser { get; }

    /// <summary>
    /// Gets the current page.
    /// </summary>
    public int CurrentPage { get; }

    /// <summary>
    /// Gets the last page.
    /// </summary>
    public int LastPage { get; }

    /// <summary>
    /// Gets the total row count, or the total top-level group count when group paging is enabled.
    /// </summary>
    public int TotalRows { get; }

    /// <summary>
    /// Gets the effective page size.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets whether paging is applied to top-level row groups instead of rendered pivot rows.
    /// </summary>
    public bool PageByGroups { get; }

    /// <summary>
    /// Gets localized text for the field chooser command.
    /// </summary>
    public string FieldsText { get; }

    /// <summary>
    /// Gets localized text for the refresh command.
    /// </summary>
    public string RefreshText { get; }

    /// <summary>
    /// Gets localized text for the expand all command.
    /// </summary>
    public string ExpandAllText { get; }

    /// <summary>
    /// Gets localized text for the collapse all command.
    /// </summary>
    public string CollapseAllText { get; }

    /// <summary>
    /// Gets localized text for the reset layout command.
    /// </summary>
    public string ResetLayoutText { get; }

    /// <summary>
    /// Opens the field chooser dialog.
    /// </summary>
    public Func<Task> OpenFieldChooserCommand { get; }

    /// <summary>
    /// Reloads or recalculates the pivot data.
    /// </summary>
    public Func<Task> RefreshCommand { get; }

    /// <summary>
    /// Expands all expandable row and column groups.
    /// </summary>
    public Func<Task> ExpandAllCommand { get; }

    /// <summary>
    /// Collapses all expandable row and column groups.
    /// </summary>
    public Func<Task> CollapseAllCommand { get; }

    /// <summary>
    /// Resets runtime field layout and filters to the declared field configuration.
    /// </summary>
    public Func<Task> ResetLayoutCommand { get; }

    /// <summary>
    /// Navigates to the first page.
    /// </summary>
    public Func<Task> FirstPageCommand { get; }

    /// <summary>
    /// Navigates to the previous page.
    /// </summary>
    public Func<Task> PreviousPageCommand { get; }

    /// <summary>
    /// Navigates to the next page.
    /// </summary>
    public Func<Task> NextPageCommand { get; }

    /// <summary>
    /// Navigates to the last page.
    /// </summary>
    public Func<Task> LastPageCommand { get; }

    /// <summary>
    /// Gets whether the field chooser command is available.
    /// </summary>
    public bool CanOpenFieldChooser => ShowFieldChooser;

    /// <summary>
    /// Gets whether the grid has expandable row or column groups.
    /// </summary>
    public bool CanExpandCollapseGroups { get; }

    /// <summary>
    /// Gets whether the runtime field layout can be reset.
    /// </summary>
    public bool CanResetLayout { get; }

    /// <summary>
    /// Gets whether the grid can navigate to the previous page.
    /// </summary>
    public bool CanGoToPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Gets whether the grid can navigate to the next page.
    /// </summary>
    public bool CanGoToNextPage => CurrentPage < LastPage;
}