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
    {
        OpenFieldChooserCommand = openFieldChooserCommand;
        FirstPageCommand = firstPageCommand;
        PreviousPageCommand = previousPageCommand;
        NextPageCommand = nextPageCommand;
        LastPageCommand = lastPageCommand;
        ShowFieldChooser = pivotGrid.ShowFieldChooser;
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
    /// Opens the field chooser dialog.
    /// </summary>
    public Func<Task> OpenFieldChooserCommand { get; }

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
    /// Gets whether the grid can navigate to the previous page.
    /// </summary>
    public bool CanGoToPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Gets whether the grid can navigate to the next page.
    /// </summary>
    public bool CanGoToNextPage => CurrentPage < LastPage;
}