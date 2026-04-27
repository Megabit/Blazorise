#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Localization;
using Blazorise.PivotGrid.Extensions;
using Blazorise.PivotGrid.Utilities;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Represents a read-only pivot table component for multi-dimensional data analysis.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public partial class PivotGrid<TItem> : BaseComponent
{
    #region Members

    private readonly List<BasePivotGridField<TItem>> fields = new();
    private readonly Dictionary<BasePivotGridField<TItem>, int> fieldStateHashes = new();
    private PivotGridResult<TItem> pivotResult = PivotGridResult<TItem>.Empty;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if ( LocalizerService is not null )
        {
            LocalizerService.LocalizationChanged += OnLocalizationChanged;
        }

        base.OnInitialized();
    }

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        await base.SetParametersAsync( parameters );

        RebuildPivot();
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-pivot-grid" );

        base.BuildClasses( builder );
    }

    /// <inheritdoc />
    protected override void Dispose( bool disposing )
    {
        if ( disposing && LocalizerService is not null )
        {
            LocalizerService.LocalizationChanged -= OnLocalizationChanged;
        }

        base.Dispose( disposing );
    }

    private async void OnLocalizationChanged( object sender, EventArgs eventArgs )
    {
        await InvokeAsync( StateHasChanged );
    }

    internal void RegisterField( BasePivotGridField<TItem> field )
    {
        if ( field is null )
            return;

        var fieldStateHash = field.GetFieldStateHash();
        var fieldExists = fieldStateHashes.TryGetValue( field, out var previousFieldStateHash );

        if ( !fields.Contains( field ) )
            fields.Add( field );

        if ( !fieldExists || previousFieldStateHash != fieldStateHash )
        {
            fieldStateHashes[field] = fieldStateHash;
            RebuildPivot();

            _ = InvokeAsync( StateHasChanged );
        }
    }

    internal bool RemoveField( BasePivotGridField<TItem> field )
    {
        if ( field is null )
            return false;

        fieldStateHashes.Remove( field );

        var removed = fields.Remove( field );

        if ( removed )
        {
            RebuildPivot();
            _ = InvokeAsync( StateHasChanged );
        }

        return removed;
    }

    internal Task NotifyCellClicked( PivotGridCell<TItem> cell, PivotGridAxisItem<TItem> row )
    {
        if ( !CellClicked.HasDelegate || cell is null || row is null )
            return Task.CompletedTask;

        return CellClicked.InvokeAsync( new PivotGridCellClickedEventArgs<TItem>(
            cell.DataColumn.Aggregate,
            cell.Value,
            row.Values,
            cell.DataColumn.Column.Values,
            cell.Items ) );
    }

    internal async Task SelectPage( string page )
    {
        var newPage = page switch
        {
            "first" => 1,
            "prev" => CurrentPage - 1,
            "next" => CurrentPage + 1,
            "last" => LastPage,
            _ => int.TryParse( page, out var pageNumber ) ? pageNumber : CurrentPage,
        };

        await SetPage( newPage );
    }

    internal async Task SetPage( int page )
    {
        var newPage = Math.Clamp( page, 1, LastPage );

        if ( Page == newPage )
            return;

        Page = newPage;

        await PageChanged.InvokeAsync( newPage );
        await InvokeAsync( StateHasChanged );
    }

    internal async Task SetPageSize( int pageSize )
    {
        var newPageSize = Math.Max( 1, pageSize );

        if ( PageSize != newPageSize )
        {
            PageSize = newPageSize;
            await PageSizeChanged.InvokeAsync( newPageSize );
        }

        await SetPage( 1 );
        await InvokeAsync( StateHasChanged );
    }

    private void RebuildPivot()
    {
        var rowFields = fields.Where( x => x.Visible && x.FieldArea == PivotGridFieldArea.Row ).ToList();
        var columnFields = fields.Where( x => x.Visible && x.FieldArea == PivotGridFieldArea.Column ).ToList();
        var aggregates = fields.OfType<PivotGridAggregate<TItem>>().Where( x => x.Visible && x.FieldArea == PivotGridFieldArea.Aggregate ).ToList();

        if ( aggregates.Count == 0 )
        {
            pivotResult = new( rowFields, columnFields, aggregates, [], [] );
            return;
        }

        var sourceItems = Data?.ToList() ?? [];

        if ( sourceItems.Count == 0 )
        {
            pivotResult = new( rowFields, columnFields, aggregates, [], [] );
            return;
        }

        var rowAxisItems = BuildAxisItems( sourceItems, rowFields, ShowRowSubtotals, ShowRowTotals, RowTotalPosition );
        var columnAxisItems = BuildAxisItems( sourceItems, columnFields, ShowColumnSubtotals, ShowColumnTotals, ColumnTotalPosition );
        var dataColumns = columnAxisItems
            .SelectMany( column => aggregates.Select( aggregate => new PivotGridDataColumn<TItem>( column, aggregate ) ) )
            .ToList();

        var rows = rowAxisItems
            .Select( row => new PivotGridResultRow<TItem>( row, BuildCells( row, columnFields, dataColumns ) ) )
            .ToList();

        pivotResult = new( rowFields, columnFields, aggregates, dataColumns, rows );
    }

    private IReadOnlyList<PivotGridCell<TItem>> BuildCells( PivotGridAxisItem<TItem> row, IReadOnlyList<BasePivotGridField<TItem>> columnFields, IReadOnlyList<PivotGridDataColumn<TItem>> dataColumns )
    {
        return dataColumns.Select( dataColumn =>
        {
            var cellItems = GetMatchingItems( row.Items, columnFields, dataColumn.Column.Values );
            var value = dataColumn.Aggregate.Aggregate( cellItems );
            var formattedValue = dataColumn.Aggregate.FormatValue( value );

            return new PivotGridCell<TItem>(
                dataColumn,
                value,
                formattedValue,
                cellItems,
                row.IsTotal || row.IsGrandTotal,
                dataColumn.Column.IsTotal || dataColumn.Column.IsGrandTotal,
                row.IsGrandTotal && dataColumn.Column.IsGrandTotal );
        } ).ToList();
    }

    private IReadOnlyList<TItem> GetMatchingItems( IReadOnlyList<TItem> items, IReadOnlyList<BasePivotGridField<TItem>> columnFields, IReadOnlyList<object> values )
    {
        if ( values.Count == 0 )
            return items;

        return items.Where( item => MatchesPath( item, columnFields, values ) ).ToList();
    }

    private IReadOnlyList<PivotGridAxisItem<TItem>> BuildAxisItems( IReadOnlyList<TItem> items, IReadOnlyList<BasePivotGridField<TItem>> axisFields, bool showSubtotals, bool showGrandTotal, PivotGridTotalPosition totalPosition )
    {
        var result = new List<PivotGridAxisItem<TItem>>();

        if ( axisFields.Count == 0 )
        {
            result.Add( new PivotGridAxisItem<TItem>( [], items, 0, true, true ) );
            return result;
        }

        BuildAxisItems( result, items, axisFields, 0, [] , showSubtotals, totalPosition );

        if ( showGrandTotal )
            result.Add( new PivotGridAxisItem<TItem>( [], items, 0, true, true ) );

        return result;
    }

    private void BuildAxisItems( List<PivotGridAxisItem<TItem>> result, IReadOnlyList<TItem> items, IReadOnlyList<BasePivotGridField<TItem>> axisFields, int level, IReadOnlyList<object> parentValues, bool showSubtotals, PivotGridTotalPosition totalPosition )
    {
        var field = axisFields[level];
        var groups = items
            .GroupBy( field.GetValue, PivotGridObjectEqualityComparer.Instance )
            .OrderBy( group => field.FormatValue( group.Key ), StringComparer.CurrentCultureIgnoreCase )
            .ToList();

        foreach ( var group in groups )
        {
            var values = parentValues.Concat( [group.Key] ).ToList();
            var groupItems = group.ToList();
            var isLeafLevel = level == axisFields.Count - 1;
            var subtotal = new PivotGridAxisItem<TItem>( values, groupItems, level, true, false );

            if ( showSubtotals && !isLeafLevel && totalPosition == PivotGridTotalPosition.Before )
                result.Add( subtotal );

            if ( isLeafLevel )
            {
                result.Add( new PivotGridAxisItem<TItem>( values, groupItems, level, false, false ) );
            }
            else
            {
                BuildAxisItems( result, groupItems, axisFields, level + 1, values, showSubtotals, totalPosition );
            }

            if ( showSubtotals && !isLeafLevel && totalPosition == PivotGridTotalPosition.After )
                result.Add( subtotal );
        }
    }

    private bool MatchesPath( TItem item, IReadOnlyList<BasePivotGridField<TItem>> axisFields, IReadOnlyList<object> values )
    {
        for ( var i = 0; i < values.Count; i++ )
        {
            if ( !object.Equals( axisFields[i].GetValue( item ), values[i] ) )
                return false;
        }

        return true;
    }

    internal string LocalizedEmptyText
        => Localizer.Localize( Localizers?.EmptyLocalizer, LocalizationConstants.Empty );

    internal string LocalizedNoDataText
        => Localizer.Localize( Localizers?.NoDataLocalizer, LocalizationConstants.NoData );

    internal string LocalizedMissingValuesText
        => Localizer.Localize( Localizers?.MissingValuesLocalizer, LocalizationConstants.MissingValues );

    internal string LocalizedGrandTotalText
        => Localizer.Localize( Localizers?.GrandTotalLocalizer, LocalizationConstants.GrandTotal );

    internal string LocalizedTotalText
        => Localizer.Localize( Localizers?.TotalLocalizer, LocalizationConstants.Total );

    internal string LocalizedValuesText
        => Localizer.Localize( Localizers?.ValuesLocalizer, LocalizationConstants.Values );

    internal string LocalizedPaginationText
        => Localizer.Localize( Localizers?.PaginationLocalizer, LocalizationConstants.Pagination );

    internal string LocalizedFirstText
        => Localizer.Localize( Localizers?.FirstPageButtonLocalizer, LocalizationConstants.First );

    internal string LocalizedPreviousText
        => Localizer.Localize( Localizers?.PreviousPageButtonLocalizer, LocalizationConstants.Previous );

    internal string LocalizedNextText
        => Localizer.Localize( Localizers?.NextPageButtonLocalizer, LocalizationConstants.Next );

    internal string LocalizedLastText
        => Localizer.Localize( Localizers?.LastPageButtonLocalizer, LocalizationConstants.Last );

    internal string LocalizedItemsPerPageText
        => Localizer.Localize( Localizers?.ItemsPerPageLocalizer, LocalizationConstants.ItemsPerPage );

    internal string LocalizedItemsRangeText
    {
        get
        {
            if ( TotalRows == 0 )
                return Localizer.Localize( Localizers?.NumbersOfItemsLocalizer, LocalizationConstants.NumbersOfItems, 0, 0, 0 );

            var firstItem = ( CurrentPage - 1 ) * EffectivePageSize + 1;
            var lastItem = Math.Min( CurrentPage * EffectivePageSize, TotalRows );

            return Localizer.Localize( Localizers?.NumbersOfItemsLocalizer, LocalizationConstants.NumbersOfItems, firstItem, lastItem, TotalRows );
        }
    }

    internal PivotGridResult<TItem> VisiblePivotResult
    {
        get
        {
            if ( !ShowPager || pivotResult is null || !pivotResult.HasValues )
                return pivotResult;

            var rows = pivotResult.Rows
                .Skip( ( CurrentPage - 1 ) * EffectivePageSize )
                .Take( EffectivePageSize )
                .ToList();

            return new( pivotResult.RowFields, pivotResult.ColumnFields, pivotResult.Aggregates, pivotResult.DataColumns, rows );
        }
    }

    internal bool IsPagerVisible
        => ShowPager && pivotResult is not null && pivotResult.HasValues && pivotResult.Rows.Count > 0;

    internal int TotalRows
        => pivotResult?.Rows.Count ?? 0;

    internal int EffectivePageSize
        => Math.Max( 1, PageSize );

    internal int CurrentPage
        => Math.Clamp( Page <= 0 ? 1 : Page, 1, LastPage );

    internal int LastPage
        => Math.Max( 1, (int)Math.Ceiling( TotalRows / (double)EffectivePageSize ) );

    internal Size PaginationSize
        => PagerSize ?? Size.Default;

    internal IReadOnlyList<int> EffectivePageSizes
    {
        get
        {
            var pageSizes = ( PageSizes ?? Array.Empty<int>() )
                .Append( EffectivePageSize )
                .Where( x => x > 0 )
                .Distinct()
                .OrderBy( x => x )
                .ToList();

            return pageSizes.Count == 0 ? [EffectivePageSize] : pageSizes;
        }
    }

    internal IReadOnlyList<int> VisiblePageNumbers
    {
        get
        {
            var maxPaginationLinks = Math.Max( 1, MaxPaginationLinks );
            var half = maxPaginationLinks / 2;
            var firstPage = Math.Max( 1, CurrentPage - half );
            var lastPage = Math.Min( LastPage, firstPage + maxPaginationLinks - 1 );

            firstPage = Math.Max( 1, lastPage - maxPaginationLinks + 1 );

            return Enumerable.Range( firstPage, lastPage - firstPage + 1 ).ToList();
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets text localizer used by this component.
    /// </summary>
    [Inject] protected ITextLocalizer<PivotGrid<TItem>> Localizer { get; set; }

    /// <summary>
    /// Gets text localizer service.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Defines the source data to be analyzed.
    /// </summary>
    [Parameter] public IEnumerable<TItem> Data { get; set; }

    /// <summary>
    /// Defines child content. Field components can be declared directly here.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Shows subtotal rows for row dimensions.
    /// </summary>
    [Parameter] public bool ShowRowSubtotals { get; set; } = true;

    /// <summary>
    /// Shows subtotal columns for column dimensions.
    /// </summary>
    [Parameter] public bool ShowColumnSubtotals { get; set; } = true;

    /// <summary>
    /// Shows the row totals.
    /// </summary>
    [Parameter] public bool ShowRowTotals { get; set; } = true;

    /// <summary>
    /// Shows the column totals.
    /// </summary>
    [Parameter] public bool ShowColumnTotals { get; set; } = true;

    /// <summary>
    /// Defines row subtotal position.
    /// </summary>
    [Parameter] public PivotGridTotalPosition RowTotalPosition { get; set; } = PivotGridTotalPosition.After;

    /// <summary>
    /// Defines column subtotal position.
    /// </summary>
    [Parameter] public PivotGridTotalPosition ColumnTotalPosition { get; set; } = PivotGridTotalPosition.After;

    /// <summary>
    /// Defines whether table cells have borders.
    /// </summary>
    [Parameter] public bool Bordered { get; set; } = true;

    /// <summary>
    /// Defines whether rows are striped.
    /// </summary>
    [Parameter] public bool Striped { get; set; }

    /// <summary>
    /// Defines whether rows show hover styling.
    /// </summary>
    [Parameter] public bool Hoverable { get; set; } = true;

    /// <summary>
    /// Defines whether cells use narrow spacing.
    /// </summary>
    [Parameter] public bool Narrow { get; set; }

    /// <summary>
    /// Defines whether table is responsive.
    /// </summary>
    [Parameter] public bool Responsive { get; set; } = true;

    /// <summary>
    /// Defines whether header uses theme contrast.
    /// </summary>
    [Parameter] public ThemeContrast HeaderThemeContrast { get; set; } = ThemeContrast.Light;

    /// <summary>
    /// Shows pager controls for rendered pivot rows.
    /// </summary>
    [Parameter] public bool ShowPager { get; set; }

    /// <summary>
    /// Shows page size selector in the pager.
    /// </summary>
    [Parameter] public bool ShowPageSizes { get; set; }

    /// <summary>
    /// Currently selected page.
    /// </summary>
    [Parameter] public int Page { get; set; } = 1;

    /// <summary>
    /// Occurs after the page has changed.
    /// </summary>
    [Parameter] public EventCallback<int> PageChanged { get; set; }

    /// <summary>
    /// Number of rendered pivot rows per page.
    /// </summary>
    [Parameter] public int PageSize { get; set; } = 10;

    /// <summary>
    /// Occurs after the page size has changed.
    /// </summary>
    [Parameter] public EventCallback<int> PageSizeChanged { get; set; }

    /// <summary>
    /// Available page size options.
    /// </summary>
    [Parameter] public IEnumerable<int> PageSizes { get; set; } = [5, 10, 25, 50, 100];

    /// <summary>
    /// Maximum number of page links visible at once.
    /// </summary>
    [Parameter] public int MaxPaginationLinks { get; set; } = 5;

    /// <summary>
    /// Pager control size.
    /// </summary>
    [Parameter] public Size? PagerSize { get; set; }

    /// <summary>
    /// Custom content shown when the pivot result is empty.
    /// </summary>
    [Parameter] public RenderFragment EmptyTemplate { get; set; }

    /// <summary>
    /// Custom content shown when there are no data rows.
    /// </summary>
    [Parameter] public RenderFragment NoDataTemplate { get; set; }

    /// <summary>
    /// Custom content shown when no value fields are declared.
    /// </summary>
    [Parameter] public RenderFragment MissingValuesTemplate { get; set; }

    /// <summary>
    /// Custom localizer handlers to override default <see cref="PivotGrid{TItem}"/> localization.
    /// </summary>
    [Parameter] public PivotGridLocalizers Localizers { get; set; }

    /// <summary>
    /// Occurs when an aggregate cell is clicked.
    /// </summary>
    [Parameter] public EventCallback<PivotGridCellClickedEventArgs<TItem>> CellClicked { get; set; }

    #endregion
}