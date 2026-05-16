#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Localization;
using Blazorise.PivotGrid.Components;
using Blazorise.PivotGrid.Extensions;
using Blazorise.PivotGrid.Utilities;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
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
    private readonly List<PivotGridFieldState> runtimeRows = new();
    private readonly List<PivotGridFieldState> runtimeColumns = new();
    private readonly List<PivotGridFieldState> runtimeAggregates = new();
    private readonly List<PivotGridFieldState> runtimeFilters = new();
    private readonly HashSet<string> collapsedRowGroupKeys = new();
    private readonly HashSet<string> collapsedColumnGroupKeys = new();
    private readonly HashSet<string> expandedRowGroupKeys = new();
    private readonly HashSet<string> expandedColumnGroupKeys = new();
    private bool runtimeStateInitialized;
    private bool runtimeStateUserModified;
    private bool? previousInitiallyExpanded;
    private _PivotGridFieldChooser<TItem> fieldChooserRef;
    private PivotGridResult<TItem> pivotResult = PivotGridResult<TItem>.Empty;
    private IReadOnlyList<TItem> externalData = [];
    private PivotGridResult<TItem> externalPivotResult;
    private int? externalTotalItems;
    private bool externalDataIsPaged;
    private string lastExternalDataRequestKey;
    private CancellationTokenSource readDataCancellationTokenSource;
    private IPivotGridDataSource<TItem> previousDataSource;
    private bool previousReadDataHasDelegate;
    private bool externalDataReadQueued;
    private bool externalVirtualizedResultInitialized;

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

        if ( previousInitiallyExpanded != InitiallyExpanded )
        {
            collapsedRowGroupKeys.Clear();
            collapsedColumnGroupKeys.Clear();
            expandedRowGroupKeys.Clear();
            expandedColumnGroupKeys.Clear();
            previousInitiallyExpanded = InitiallyExpanded;
        }

        if ( !ReferenceEquals( previousDataSource, DataSource ) )
        {
            previousDataSource = DataSource;
            InvalidateExternalDataRead();
        }

        if ( previousReadDataHasDelegate != ReadData.HasDelegate )
        {
            previousReadDataHasDelegate = ReadData.HasDelegate;
            InvalidateExternalDataRead();
        }

        if ( UsesExternalData )
        {
            if ( IsExternalVirtualizeActive )
                PrepareExternalVirtualizedData();
            else if ( fields.Count > 0 || ChildContent is null )
                await ReadExternalDataAsync();
            else
                RebuildPivot();
        }
        else
        {
            RebuildPivot();
        }
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
        if ( disposing )
        {
            if ( LocalizerService is not null )
            {
                LocalizerService.LocalizationChanged -= OnLocalizationChanged;
            }

            readDataCancellationTokenSource?.Cancel();
            readDataCancellationTokenSource?.Dispose();
            readDataCancellationTokenSource = null;
        }

        base.Dispose( disposing );
    }

    private async void OnLocalizationChanged( object sender, EventArgs eventArgs )
    {
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Reloads the PivotGrid data and rebuilds the pivot result.
    /// </summary>
    /// <returns>Returns the awaitable task.</returns>
    public async Task Reload()
    {
        InvalidateExternalDataRead();

        if ( UsesExternalData )
        {
            await ReadExternalDataAsync( true );
        }
        else
        {
            RebuildPivot();

            await InvokeAsync( StateHasChanged );
        }
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
            InvalidateExternalDataRead();
            RequestPivotRefresh();

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
            InvalidateExternalDataRead();
            RequestPivotRefresh();
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

        if ( UsesExternalData )
            await ReadExternalDataAsync();
        else
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

        if ( Page != 1 )
        {
            await SetPage( 1 );
        }
        else if ( UsesExternalData )
        {
            await ReadExternalDataAsync();
        }
        else
        {
            await InvokeAsync( StateHasChanged );
        }
    }

    internal Task OpenFieldChooser()
    {
        EnsureRuntimeState();

        return fieldChooserRef?.Show() ?? Task.CompletedTask;
    }

    internal PivotGridToolbarContext<TItem> CreateToolbarContext()
        => new(
            this,
            OpenFieldChooser,
            () => SetPage( 1 ),
            () => SetPage( CurrentPage - 1 ),
            () => SetPage( CurrentPage + 1 ),
            () => SetPage( LastPage ) );

    internal async Task ApplyFieldChooserState( IReadOnlyList<PivotGridFieldState> rows, IReadOnlyList<PivotGridFieldState> columns, IReadOnlyList<PivotGridFieldState> aggregates, IReadOnlyList<PivotGridFieldState> filters )
    {
        runtimeRows.Clear();
        runtimeRows.AddRange( rows.Select( PivotGridFieldStateUtilities.Clone ) );

        runtimeColumns.Clear();
        runtimeColumns.AddRange( columns.Select( PivotGridFieldStateUtilities.Clone ) );

        runtimeAggregates.Clear();
        runtimeAggregates.AddRange( aggregates.Select( PivotGridFieldStateUtilities.Clone ) );

        runtimeFilters.Clear();
        runtimeFilters.AddRange( filters.Select( PivotGridFieldStateUtilities.Clone ) );

        runtimeStateInitialized = true;
        runtimeStateUserModified = true;
        InvalidateExternalDataRead();
        collapsedRowGroupKeys.Clear();
        collapsedColumnGroupKeys.Clear();
        expandedRowGroupKeys.Clear();
        expandedColumnGroupKeys.Clear();

        if ( Page != 1 )
        {
            Page = 1;
            await PageChanged.InvokeAsync( 1 );
        }

        RequestPivotRefresh();

        await InvokeAsync( StateHasChanged );
    }

    private void RequestPivotRefresh()
    {
        if ( UsesExternalData )
        {
            if ( IsExternalVirtualizeActive )
                PrepareExternalVirtualizedData();
            else
                QueueExternalDataRead();
        }
        else
        {
            RebuildPivot();
        }
    }

    private void PrepareExternalVirtualizedData()
    {
        if ( externalVirtualizedResultInitialized )
            return;

        externalData = [];
        externalPivotResult = null;
        externalTotalItems = null;
        externalDataIsPaged = false;
        pivotResult = BuildPivotResult( [] );
    }

    private void InvalidateExternalDataRead()
    {
        lastExternalDataRequestKey = null;
        externalVirtualizedResultInitialized = false;
    }

    private void QueueExternalDataRead()
    {
        if ( externalDataReadQueued )
            return;

        externalDataReadQueued = true;

        _ = InvokeAsync( async () =>
        {
            await Task.Yield();

            externalDataReadQueued = false;

            await ReadExternalDataAsync();
        } );
    }

    private async Task ReadExternalDataAsync( bool force = false )
    {
        EnsureRuntimeState();

        var request = CreateDataRequest();
        var requestKey = PivotGridKeyGenerator.CreateDataRequestKey( request );

        if ( !force && string.Equals( lastExternalDataRequestKey, requestKey, StringComparison.Ordinal ) )
        {
            if ( externalPivotResult is not null )
                pivotResult = externalPivotResult;
            else
                RebuildPivot();

            await InvokeAsync( StateHasChanged );
            return;
        }

        var cancellationTokenSource = BeginExternalDataRead();

        try
        {
            var dataResult = await ReadExternalDataResultAsync( request, cancellationTokenSource.Token );

            if ( cancellationTokenSource.IsCancellationRequested )
                return;

            lastExternalDataRequestKey = requestKey;
            ApplyExternalDataResult( dataResult );

            await InvokeAsync( StateHasChanged );
        }
        catch ( OperationCanceledException )
        {
        }
        finally
        {
            EndExternalDataRead( cancellationTokenSource );
        }
    }

    private void ApplyExternalDataResult( PivotGridDataResult<TItem> dataResult )
    {
        externalData = dataResult?.Data?.ToList() ?? [];
        ApplyExternalDataResultMetadata( dataResult );
        externalPivotResult = PivotGridResultNormalizer.Normalize( dataResult?.Result );

        if ( externalPivotResult is not null )
            pivotResult = externalPivotResult;
        else
            RebuildPivot();
    }

    private void ApplyExternalDataResultMetadata( PivotGridDataResult<TItem> dataResult )
    {
        externalTotalItems = dataResult?.TotalItems;
        externalDataIsPaged = dataResult?.IsPaged == true;
    }

    private CancellationTokenSource BeginExternalDataRead( CancellationToken cancellationToken = default )
    {
        var previousCancellationTokenSource = readDataCancellationTokenSource;
        previousCancellationTokenSource?.Cancel();

        var cancellationTokenSource = cancellationToken.CanBeCanceled
            ? CancellationTokenSource.CreateLinkedTokenSource( cancellationToken )
            : new CancellationTokenSource();

        readDataCancellationTokenSource = cancellationTokenSource;

        return cancellationTokenSource;
    }

    private void EndExternalDataRead( CancellationTokenSource cancellationTokenSource )
    {
        if ( ReferenceEquals( readDataCancellationTokenSource, cancellationTokenSource ) )
        {
            readDataCancellationTokenSource = null;
        }

        cancellationTokenSource.Dispose();
    }

    private Task<PivotGridDataResult<TItem>> ReadExternalDataResultAsync( PivotGridDataRequest request, CancellationToken cancellationToken )
    {
        if ( DataSource is not null )
            return DataSource.ReadDataAsync( request, cancellationToken );

        if ( ReadData.HasDelegate )
            return ReadExternalCallbackDataResultAsync( request, cancellationToken );

        return Task.FromResult<PivotGridDataResult<TItem>>( null );
    }

    private async Task<PivotGridDataResult<TItem>> ReadExternalCallbackDataResultAsync( PivotGridDataRequest request, CancellationToken cancellationToken )
    {
        var eventArgs = new PivotGridReadDataEventArgs<TItem>( request, cancellationToken );

        await ReadData.InvokeAsync( eventArgs );

        return new()
        {
            Data = eventArgs.Data,
            TotalItems = eventArgs.TotalItems,
            IsPaged = eventArgs.IsPaged,
            Result = eventArgs.Result,
        };
    }

    private void RebuildPivot()
    {
        EnsureRuntimeState();

        pivotResult = BuildPivotResult( GetCurrentSourceData() );
    }

    private PivotGridResult<TItem> BuildPivotResult( IReadOnlyList<TItem> sourceData )
    {
        var rowFields = GetEffectiveRowFields();
        var columnFields = GetEffectiveColumnFields();
        var aggregates = GetEffectiveAggregates();
        var rowFieldInfos = rowFields.Select( CreateFieldInfo ).ToList();
        var columnFieldInfos = columnFields.Select( CreateFieldInfo ).ToList();
        var aggregateInfos = aggregates.Select( CreateAggregateInfo ).ToList();

        if ( aggregates.Count == 0 )
        {
            return new( rowFieldInfos, columnFieldInfos, aggregateInfos, [], [] );
        }

        var sourceItems = ApplyFilters( sourceData ?? [] );

        if ( sourceItems.Count == 0 )
        {
            return new( rowFieldInfos, columnFieldInfos, aggregateInfos, [], [] );
        }

        var showColumnTotalsRow = ShowColumnTotals;
        var showRowTotalsColumn = ShowRowTotals;

        var rowAxisItems = BuildAxisItems( sourceItems, rowFields, ShowRowSubtotals, showColumnTotalsRow, ExpandableRows ? PivotGridTotalPosition.Before : RowTotalPosition );
        var columnAxisItems = BuildAxisItems( sourceItems, columnFields, ShowColumnSubtotals, showRowTotalsColumn, ExpandableColumns ? PivotGridTotalPosition.Before : ColumnTotalPosition );
        var dataColumns = columnAxisItems
            .SelectMany( column => aggregateInfos.Select( aggregate => new PivotGridDataColumn<TItem>( column, aggregate ) ) )
            .ToList();

        var rows = rowAxisItems
            .Select( row => new PivotGridResultRow<TItem>( row, BuildCells( row, columnFields, dataColumns ) ) )
            .ToList();

        return new( rowFieldInfos, columnFieldInfos, aggregateInfos, dataColumns, rows );
    }

    private void EnsureRuntimeState()
    {
        if ( !ShowFieldChooser || ( runtimeStateInitialized && runtimeStateUserModified ) )
            return;

        runtimeRows.Clear();
        runtimeRows.AddRange( fields.Where( x => x.Visible && x.FieldArea == PivotGridFieldArea.Row ).Select( x => CreateFieldState( x, PivotGridFieldArea.Row ) ) );

        runtimeColumns.Clear();
        runtimeColumns.AddRange( fields.Where( x => x.Visible && x.FieldArea == PivotGridFieldArea.Column ).Select( x => CreateFieldState( x, PivotGridFieldArea.Column ) ) );

        runtimeAggregates.Clear();
        runtimeAggregates.AddRange( fields.OfType<PivotGridAggregate<TItem>>().Where( x => x.Visible && x.FieldArea == PivotGridFieldArea.Aggregate ).Select( x => CreateFieldState( x, PivotGridFieldArea.Aggregate ) ) );

        runtimeFilters.Clear();
        runtimeStateInitialized = true;
    }

    private IReadOnlyList<BasePivotGridField<TItem>> GetEffectiveRowFields()
        => GetEffectiveFields( PivotGridFieldArea.Row );

    private IReadOnlyList<BasePivotGridField<TItem>> GetEffectiveColumnFields()
        => GetEffectiveFields( PivotGridFieldArea.Column );

    private IReadOnlyList<PivotGridAggregate<TItem>> GetEffectiveAggregates()
        => ShowFieldChooser
            ? GetActiveRuntimeFieldStates( GetRuntimeFieldStates( PivotGridFieldArea.Aggregate ), PivotGridFieldArea.Aggregate )
                .Select( CreateRuntimeAggregate )
                .ToList()
            : GetDeclaredAggregates();

    private IReadOnlyList<BasePivotGridField<TItem>> GetEffectiveFields( PivotGridFieldArea area )
        => ShowFieldChooser
            ? GetActiveRuntimeFieldStates( GetRuntimeFieldStates( area ), area ).Select( x => CreateRuntimeField( x, area ) ).ToList()
            : GetDeclaredFields( area );

    private IReadOnlyList<TItem> GetCurrentSourceData()
        => UsesExternalData ? externalData : Data?.ToList() ?? [];

    internal async ValueTask<ItemsProviderResult<PivotGridResultRow<TItem>>> VirtualizeRowsProvider( ItemsProviderRequest request )
    {
        if ( request.CancellationToken.IsCancellationRequested )
            return default;

        if ( UsesExternalData )
            return await ReadExternalVirtualizedRowsAsync( request );

        var expandedPivotResult = GetExpandedPivotResult();
        var rows = expandedPivotResult?.Rows ?? [];
        var totalRows = rows.Count;
        var requestCount = Math.Min( request.Count, Math.Max( 0, totalRows - request.StartIndex ) );

        return new( rows.Skip( request.StartIndex ).Take( requestCount ).ToList(), totalRows );
    }

    private async ValueTask<ItemsProviderResult<PivotGridResultRow<TItem>>> ReadExternalVirtualizedRowsAsync( ItemsProviderRequest providerRequest )
    {
        EnsureRuntimeState();

        var requestCount = externalTotalItems.HasValue
            ? Math.Min( providerRequest.Count, Math.Max( 0, externalTotalItems.Value - providerRequest.StartIndex ) )
            : providerRequest.Count;

        var request = CreateDataRequest( PivotGridReadDataMode.Virtualize, providerRequest.StartIndex, requestCount );
        var requestKey = PivotGridKeyGenerator.CreateDataRequestKey( request );

        if ( readDataCancellationTokenSource is null && string.Equals( lastExternalDataRequestKey, requestKey, StringComparison.Ordinal ) && pivotResult is not null )
        {
            return new( pivotResult.Rows, externalTotalItems ?? pivotResult.Rows.Count );
        }

        var cancellationTokenSource = BeginExternalDataRead( providerRequest.CancellationToken );

        try
        {
            var dataResult = await ReadExternalDataResultAsync( request, cancellationTokenSource.Token );

            if ( cancellationTokenSource.IsCancellationRequested )
                return default;

            ApplyExternalDataResultMetadata( dataResult );

            if ( dataResult?.Result is not null )
            {
                var virtualizedResult = PivotGridResultNormalizer.NormalizeVirtualized(
                    dataResult.Result,
                    externalVirtualizedResultInitialized ? pivotResult : null );

                ApplyInitialExternalVirtualizedResult( requestKey, virtualizedResult );

                return new( virtualizedResult.Rows, externalTotalItems ?? virtualizedResult.Rows.Count );
            }

            var virtualizedData = dataResult?.Data?.ToList() ?? [];
            var virtualizedPivotResult = BuildPivotResult( virtualizedData );
            var rows = virtualizedPivotResult.Rows;

            ApplyInitialExternalVirtualizedResult( requestKey, virtualizedPivotResult );

            return new( rows, externalTotalItems ?? rows.Count );
        }
        catch ( OperationCanceledException )
        {
            return default;
        }
        finally
        {
            EndExternalDataRead( cancellationTokenSource );
        }
    }

    private void ApplyInitialExternalVirtualizedResult( string requestKey, PivotGridResult<TItem> result )
    {
        if ( externalVirtualizedResultInitialized )
            return;

        lastExternalDataRequestKey = requestKey;
        externalPivotResult = result;
        pivotResult = result;
        externalVirtualizedResultInitialized = true;

        _ = InvokeAsync( StateHasChanged );
    }

    private PivotGridDataRequest CreateDataRequest( PivotGridReadDataMode? readDataMode = null, int virtualizeOffset = 0, int virtualizeCount = 0 )
    {
        var effectiveReadDataMode = readDataMode
            ?? ( IsVirtualizeActive ? PivotGridReadDataMode.Virtualize : ShowPager ? PivotGridReadDataMode.Paging : PivotGridReadDataMode.All );

        if ( effectiveReadDataMode == PivotGridReadDataMode.Virtualize && virtualizeCount <= 0 )
        {
            virtualizeCount = EffectivePageSize;
        }

        return new()
        {
            ReadDataMode = effectiveReadDataMode,
            Rows = GetDataRequestRows(),
            Columns = GetDataRequestColumns(),
            Aggregates = GetDataRequestAggregates(),
            Filters = GetDataRequestFilters(),
            Page = CurrentPage,
            PageSize = EffectivePageSize,
            VirtualizeOffset = effectiveReadDataMode == PivotGridReadDataMode.Virtualize ? Math.Max( 0, virtualizeOffset ) : 0,
            VirtualizeCount = effectiveReadDataMode == PivotGridReadDataMode.Virtualize ? Math.Max( 0, virtualizeCount ) : 0,
            PageByGroups = PageByGroups,
            ShowPager = ShowPager,
            ShowRowSubtotals = ShowRowSubtotals,
            ShowColumnSubtotals = ShowColumnSubtotals,
            ShowRowTotals = ShowRowTotals,
            ShowColumnTotals = ShowColumnTotals,
            RowTotalPosition = RowTotalPosition,
            ColumnTotalPosition = ColumnTotalPosition,
            ExpandableRows = ExpandableRows,
            ExpandableColumns = ExpandableColumns,
            InitiallyExpanded = InitiallyExpanded,
        };
    }

    private IReadOnlyList<PivotGridFieldState> GetDataRequestRows()
        => GetDataRequestFields( PivotGridFieldArea.Row );

    private IReadOnlyList<PivotGridFieldState> GetDataRequestColumns()
        => GetDataRequestFields( PivotGridFieldArea.Column );

    private IReadOnlyList<PivotGridFieldState> GetDataRequestAggregates()
        => GetDataRequestFields( PivotGridFieldArea.Aggregate );

    private IReadOnlyList<PivotGridFieldState> GetDataRequestFilters()
        => ShowFieldChooser
            ? GetRuntimeFilters()
            : [];

    private IReadOnlyList<PivotGridFieldState> GetDataRequestFields( PivotGridFieldArea area )
        => ShowFieldChooser
            ? GetActiveRuntimeFieldStates( GetRuntimeFieldStates( area ), area )
            : GetDeclaredFieldStates( area );

    private IReadOnlyList<BasePivotGridField<TItem>> GetDeclaredFields( PivotGridFieldArea area )
        => fields.Where( x => x.Visible && x.FieldArea == area ).ToList();

    private IReadOnlyList<PivotGridAggregate<TItem>> GetDeclaredAggregates()
        => fields.OfType<PivotGridAggregate<TItem>>().Where( x => x.Visible && x.FieldArea == PivotGridFieldArea.Aggregate ).ToList();

    private IReadOnlyList<PivotGridFieldState> GetDeclaredFieldStates( PivotGridFieldArea area )
        => area == PivotGridFieldArea.Aggregate
            ? GetDeclaredAggregates().Select( x => CreateFieldState( x, area ) ).ToList()
            : GetDeclaredFields( area ).Select( x => CreateFieldState( x, area ) ).ToList();

    private IReadOnlyList<PivotGridFieldState> GetRuntimeFieldStates( PivotGridFieldArea area )
        => area switch
        {
            PivotGridFieldArea.Row => runtimeRows,
            PivotGridFieldArea.Column => runtimeColumns,
            PivotGridFieldArea.Aggregate => runtimeAggregates,
            PivotGridFieldArea.Filter => runtimeFilters,
            _ => [],
        };

    private IReadOnlyList<PivotGridFieldState> GetActiveRuntimeFieldStates( IReadOnlyList<PivotGridFieldState> states, PivotGridFieldArea area )
    {
        var activeStates = new List<PivotGridFieldState>();

        foreach ( var state in states )
        {
            var source = FindRuntimeFieldMetadata( state, area );

            if ( source?.Visible != true )
                continue;

            activeStates.Add( CreateCurrentRuntimeFieldState( state, source ) );
        }

        return activeStates;
    }

    private static PivotGridFieldInfo<TItem> CreateFieldInfo( BasePivotGridField<TItem> field )
        => new ComponentPivotGridFieldInfo( field );

    private static PivotGridAggregateInfo<TItem> CreateAggregateInfo( PivotGridAggregate<TItem> aggregate )
        => new ComponentPivotGridAggregateInfo( aggregate );

    private sealed class ComponentPivotGridFieldInfo : PivotGridFieldInfo<TItem>
    {
        private readonly BasePivotGridField<TItem> field;

        internal ComponentPivotGridFieldInfo( BasePivotGridField<TItem> field )
        {
            this.field = field;

            Field = field.Field;
            Caption = field.Caption;
            DisplayFormat = field.DisplayFormat;
            DisplayFormatProvider = field.DisplayFormatProvider;
            EmptyText = field.EmptyText;
            HeaderTemplate = field.HeaderTemplate;
            DisplayTemplate = field.DisplayTemplate;
        }

        public override object GetValue( TItem item )
            => field.GetValue( item );

        public override string FormatValue( object value )
            => field.FormatValue( value );
    }

    private sealed class ComponentPivotGridAggregateInfo : PivotGridAggregateInfo<TItem>
    {
        private readonly PivotGridAggregate<TItem> aggregate;

        internal ComponentPivotGridAggregateInfo( PivotGridAggregate<TItem> aggregate )
        {
            this.aggregate = aggregate;

            Field = aggregate.Field;
            Caption = aggregate.Caption;
            DisplayFormat = aggregate.DisplayFormat;
            DisplayFormatProvider = aggregate.DisplayFormatProvider;
            EmptyText = aggregate.EmptyText;
            HeaderTemplate = aggregate.HeaderTemplate;
            DisplayTemplate = aggregate.DisplayTemplate;
            Aggregate = aggregate.Aggregate;
            Aggregator = aggregate.Aggregator;
            CellTemplate = aggregate.CellTemplate;
        }

        public override object GetValue( TItem item )
            => aggregate.GetValue( item );

        public override string FormatValue( object value )
            => aggregate.FormatValue( value );

        public override object Calculate( IReadOnlyList<TItem> items )
            => aggregate.Calculate( items );
    }

    private PivotGridFieldState CreateCurrentRuntimeFieldState( PivotGridFieldState state, BasePivotGridField<TItem> source )
    {
        var clone = PivotGridFieldStateUtilities.Clone( state );
        clone.Caption = source.GetCaption();
        clone.FieldType = PivotGridFieldUtilities.GetFieldValueType<TItem>( source.Field );

        return clone;
    }

    private IReadOnlyList<TItem> ApplyFilters( IReadOnlyList<TItem> sourceItems )
    {
        if ( !ShowFieldChooser || runtimeFilters.Count == 0 )
            return sourceItems;

        var activeFilters = GetActiveRuntimeFieldStates( runtimeFilters, PivotGridFieldArea.Filter )
            .Where( x => !string.IsNullOrEmpty( x.FilterValueKey ) )
            .ToList();

        if ( activeFilters.Count == 0 )
            return sourceItems;

        return sourceItems
            .Where( item => activeFilters.All( filter =>
            {
                var field = CreateRuntimeField( filter, PivotGridFieldArea.Filter );
                return string.Equals( PivotGridKeyGenerator.CreateFilterValueKey( field.GetValue( item ) ), filter.FilterValueKey, StringComparison.Ordinal );
            } ) )
            .ToList();
    }

    private PivotGridFieldState CreateFieldState( BasePivotGridField<TItem> field, PivotGridFieldArea area )
        => new()
        {
            Field = field.Field,
            Caption = field.GetCaption(),
            FieldType = PivotGridFieldUtilities.GetFieldValueType<TItem>( field.Field ),
            Area = area,
            Aggregate = field is PivotGridAggregate<TItem> aggregate ? aggregate.Aggregate : PivotGridAggregateFunction.Sum
        };

    private BasePivotGridField<TItem> CreateRuntimeField( PivotGridFieldState state, PivotGridFieldArea area )
    {
        var source = FindFieldMetadata( state.Field, area );
        var field = new PivotGridRuntimeField<TItem>( area );

        CopyFieldMetadata( source, field, state );

        return field;
    }

    private PivotGridAggregate<TItem> CreateRuntimeAggregate( PivotGridFieldState state )
    {
        var source = FindFieldMetadata( state.Field, PivotGridFieldArea.Aggregate );
        var aggregate = new PivotGridAggregate<TItem>();

        CopyFieldMetadata( source, aggregate, state );

        if ( source is PivotGridAggregate<TItem> sourceAggregate )
        {
            aggregate.Aggregator = sourceAggregate.Aggregator;
            aggregate.CellTemplate = sourceAggregate.CellTemplate;
        }

        aggregate.Aggregate = state.Aggregate;

        return aggregate;
    }

    private void CopyFieldMetadata( BasePivotGridField<TItem> source, BasePivotGridField<TItem> target, PivotGridFieldState state )
    {
        target.Field = state.Field;
        target.Caption = string.IsNullOrWhiteSpace( state.Caption ) ? state.Field : state.Caption;

        if ( source is null )
            return;

        if ( !string.IsNullOrWhiteSpace( source.Caption ) )
            target.Caption = source.Caption;

        target.DisplayFormat = source.DisplayFormat;
        target.DisplayFormatProvider = source.DisplayFormatProvider;
        target.EmptyText = source.EmptyText;
        target.HeaderTemplate = source.HeaderTemplate;
        target.DisplayTemplate = source.DisplayTemplate;
        target.Visible = source.Visible;
    }

    private BasePivotGridField<TItem> FindFieldMetadata( string fieldName, PivotGridFieldArea area )
        => fields.FirstOrDefault( x => string.Equals( x.Field, fieldName, StringComparison.Ordinal ) && x.FieldArea == area )
            ?? fields.FirstOrDefault( x => string.Equals( x.Field, fieldName, StringComparison.Ordinal ) && x.FieldArea == PivotGridFieldArea.Available )
            ?? fields.FirstOrDefault( x => string.Equals( x.Field, fieldName, StringComparison.Ordinal ) );

    private BasePivotGridField<TItem> FindRuntimeFieldMetadata( PivotGridFieldState state, PivotGridFieldArea area )
        => state is null || string.IsNullOrWhiteSpace( state.Field )
            ? null
            : FindFieldMetadata( state.Field, area );

    internal IReadOnlyList<PivotGridFieldState> GetFieldChooserCatalog()
    {
        var availableFields = fields
            .Where( x => !string.IsNullOrWhiteSpace( x.Field ) && x.FieldArea == PivotGridFieldArea.Available )
            .ToList();
        var visibleAvailableFields = availableFields
            .Where( x => x.Visible )
            .ToList();
        var declaredFields = fields
            .Where( x => x.Visible && !string.IsNullOrWhiteSpace( x.Field ) && x.FieldArea is PivotGridFieldArea.Row or PivotGridFieldArea.Column or PivotGridFieldArea.Aggregate )
            .ToList();
        var catalog = new Dictionary<string, PivotGridFieldState>( StringComparer.Ordinal );

        if ( availableFields.Count == 0 )
        {
            foreach ( var property in typeof( TItem ).GetProperties().Where( x => x.GetMethod is not null && x.GetMethod.IsPublic && x.GetIndexParameters().Length == 0 ) )
            {
                catalog[property.Name] = new()
                {
                    Field = property.Name,
                    Caption = property.Name,
                    FieldType = Nullable.GetUnderlyingType( property.PropertyType ) ?? property.PropertyType,
                    Area = PivotGridFieldArea.Available,
                };
            }
        }

        foreach ( var field in visibleAvailableFields )
        {
            catalog[field.Field] = CreateFieldState( field, PivotGridFieldArea.Available );
        }

        foreach ( var field in declaredFields )
        {
            if ( !catalog.ContainsKey( field.Field ) )
                catalog[field.Field] = CreateFieldState( field, PivotGridFieldArea.Available );
        }

        return catalog.Values.OrderBy( x => x.Caption, StringComparer.CurrentCultureIgnoreCase ).ToList();
    }

    internal IReadOnlyList<PivotGridFieldState> GetRuntimeRows()
        => GetActiveRuntimeFieldStates( runtimeRows, PivotGridFieldArea.Row );

    internal IReadOnlyList<PivotGridFieldState> GetRuntimeColumns()
        => GetActiveRuntimeFieldStates( runtimeColumns, PivotGridFieldArea.Column );

    internal IReadOnlyList<PivotGridFieldState> GetRuntimeAggregates()
        => GetActiveRuntimeFieldStates( runtimeAggregates, PivotGridFieldArea.Aggregate );

    internal IReadOnlyList<PivotGridFieldState> GetRuntimeFilters()
        => GetActiveRuntimeFieldStates( runtimeFilters, PivotGridFieldArea.Filter );

    internal IReadOnlyList<PivotGridFilterOption> GetFilterOptions( PivotGridFieldState state )
    {
        var field = CreateRuntimeField( state, PivotGridFieldArea.Filter );

        return GetCurrentSourceData()
            .Select( item => field.GetValue( item ) )
            .Distinct( PivotGridObjectEqualityComparer.Instance )
            .OrderBy( value => field.FormatValue( value ), StringComparer.CurrentCultureIgnoreCase )
            .Select( value => new PivotGridFilterOption( PivotGridKeyGenerator.CreateFilterValueKey( value ), field.FormatValue( value ) ) )
            .ToList();
    }

    private IReadOnlyList<PivotGridCell<TItem>> BuildCells( PivotGridAxisItem<TItem> row, IReadOnlyList<BasePivotGridField<TItem>> columnFields, IReadOnlyList<PivotGridDataColumn<TItem>> dataColumns )
    {
        return dataColumns.Select( dataColumn =>
        {
            var cellItems = GetMatchingItems( row.Items, columnFields, dataColumn.Column.Values );
            var value = dataColumn.Aggregate.Calculate( cellItems );
            var formattedValue = dataColumn.Aggregate.FormatValue( value );
            var isColumnTotalsRow = row.IsTotal || row.IsGrandTotal;
            var isRowTotalsColumn = dataColumn.Column.IsTotal || dataColumn.Column.IsGrandTotal;

            return new PivotGridCell<TItem>(
                dataColumn,
                value,
                formattedValue,
                cellItems,
                isRowTotalsColumn,
                isColumnTotalsRow,
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

        BuildAxisItems( result, items, axisFields, 0, [], showSubtotals, totalPosition );

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

    internal bool CanToggleRowExpansion( PivotGridAxisItem<TItem> row )
        => ExpandableRows && PivotGridAxisItemUtilities.CanToggleExpansion( row, pivotResult?.RowFields );

    internal bool CanToggleColumnExpansion( PivotGridAxisItem<TItem> column )
        => ExpandableColumns && PivotGridAxisItemUtilities.CanToggleExpansion( column, pivotResult?.ColumnFields );

    internal bool IsRowExpanded( PivotGridAxisItem<TItem> row )
        => IsAxisItemExpanded( row, collapsedRowGroupKeys, expandedRowGroupKeys );

    internal bool IsColumnExpanded( PivotGridAxisItem<TItem> column )
        => IsAxisItemExpanded( column, collapsedColumnGroupKeys, expandedColumnGroupKeys );

    internal Task ToggleRowExpanded( PivotGridAxisItem<TItem> row )
    {
        ToggleAxisItemExpanded( row, collapsedRowGroupKeys, expandedRowGroupKeys );

        return InvokeAsync( StateHasChanged );
    }

    internal Task ToggleColumnExpanded( PivotGridAxisItem<TItem> column )
    {
        ToggleAxisItemExpanded( column, collapsedColumnGroupKeys, expandedColumnGroupKeys );

        return InvokeAsync( StateHasChanged );
    }

    private bool IsAxisItemExpanded( PivotGridAxisItem<TItem> axisItem, HashSet<string> collapsedGroupKeys, HashSet<string> expandedGroupKeys )
    {
        var key = PivotGridKeyGenerator.CreateGroupKey( axisItem.Values );

        return InitiallyExpanded
            ? !collapsedGroupKeys.Contains( key )
            : expandedGroupKeys.Contains( key );
    }

    private void ToggleAxisItemExpanded( PivotGridAxisItem<TItem> axisItem, HashSet<string> collapsedGroupKeys, HashSet<string> expandedGroupKeys )
    {
        if ( axisItem is null )
            return;

        var key = PivotGridKeyGenerator.CreateGroupKey( axisItem.Values );

        if ( InitiallyExpanded )
        {
            if ( !collapsedGroupKeys.Remove( key ) )
                collapsedGroupKeys.Add( key );
        }
        else
        {
            if ( !expandedGroupKeys.Remove( key ) )
                expandedGroupKeys.Add( key );
        }
    }

    private IReadOnlyList<PivotGridResultRow<TItem>> GetExpandedRows()
    {
        if ( pivotResult is null || pivotResult.Rows.Count == 0 )
            return [];

        return GetVisibleAxisValues(
            pivotResult.Rows,
            row => row.Row,
            pivotResult.RowFields,
            ExpandableRows,
            collapsedRowGroupKeys,
            expandedRowGroupKeys );
    }

    private IReadOnlyList<int> GetExpandedDataColumnIndexes()
    {
        if ( pivotResult is null || pivotResult.DataColumns.Count == 0 )
            return [];

        var indexes = Enumerable.Range( 0, pivotResult.DataColumns.Count ).ToList();

        return GetVisibleAxisValues(
            indexes,
            index => pivotResult.DataColumns[index].Column,
            pivotResult.ColumnFields,
            ExpandableColumns,
            collapsedColumnGroupKeys,
            expandedColumnGroupKeys );
    }

    private IReadOnlyList<TValue> GetVisibleAxisValues<TValue>( IReadOnlyList<TValue> values, Func<TValue, PivotGridAxisItem<TItem>> axisItemSelector, IReadOnlyList<PivotGridFieldInfo<TItem>> axisFields, bool expandable, HashSet<string> collapsedGroupKeys, HashSet<string> expandedGroupKeys )
    {
        if ( !expandable )
            return values;

        var expandableGroupKeys = values
            .Select( axisItemSelector )
            .Distinct( PivotGridAxisItemEqualityComparer<TItem>.Instance )
            .Where( axisItem => PivotGridAxisItemUtilities.CanToggleExpansion( axisItem, axisFields ) )
            .Select( axisItem => PivotGridKeyGenerator.CreateGroupKey( axisItem.Values ) )
            .ToHashSet( StringComparer.Ordinal );

        return values
            .Where( value => IsAxisItemVisible( axisItemSelector( value ), axisFields, expandableGroupKeys, collapsedGroupKeys, expandedGroupKeys ) )
            .ToList();
    }

    private bool IsAxisItemVisible( PivotGridAxisItem<TItem> axisItem, IReadOnlyList<PivotGridFieldInfo<TItem>> axisFields, HashSet<string> expandableGroupKeys, HashSet<string> collapsedGroupKeys, HashSet<string> expandedGroupKeys )
    {
        if ( axisItem is null || axisItem.IsGrandTotal || axisFields.Count == 0 )
            return true;

        for ( var i = 1; i < axisItem.Values.Count; i++ )
        {
            var key = PivotGridKeyGenerator.CreateGroupKey( axisItem.Values.Take( i ) );

            if ( !expandableGroupKeys.Contains( key ) )
                continue;

            var isExpanded = InitiallyExpanded
                ? !collapsedGroupKeys.Contains( key )
                : expandedGroupKeys.Contains( key );

            if ( !isExpanded )
                return false;
        }

        return true;
    }

    private PivotGridResult<TItem> GetExpandedPivotResult()
    {
        if ( pivotResult is null || !pivotResult.HasValues )
            return pivotResult;

        var expandedRows = GetExpandedRows();
        var expandedDataColumnIndexes = GetExpandedDataColumnIndexes();
        var dataColumns = expandedDataColumnIndexes
            .Select( index => pivotResult.DataColumns[index] )
            .ToList();
        var rows = expandedRows
            .Select( row => new PivotGridResultRow<TItem>(
                row.Row,
                expandedDataColumnIndexes.Select( index => row.Cells[index] ).ToList() ) )
            .ToList();

        return new( pivotResult.RowFields, pivotResult.ColumnFields, pivotResult.Aggregates, dataColumns, rows );
    }

    private IReadOnlyList<string> GetRootGroupKeys()
    {
        if ( pivotResult is null || pivotResult.Rows.Count == 0 || pivotResult.RowFields.Count == 0 )
            return [];

        return pivotResult.Rows
            .Where( row => !row.Row.IsGrandTotal && row.Row.Values.Count > 0 )
            .Select( row => PivotGridKeyGenerator.CreateGroupKey( row.Row.Values.Take( 1 ) ) )
            .Distinct()
            .ToList();
    }

    private IReadOnlyList<string> GetCurrentPageRootGroupKeys()
        => GetRootGroupKeys()
            .Skip( ( CurrentPage - 1 ) * EffectivePageSize )
            .Take( EffectivePageSize )
            .ToList();

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

    internal string LocalizedFieldsText
        => Localizer.Localize( Localizers?.FieldsLocalizer, LocalizationConstants.Fields );

    internal string LocalizedAvailableFieldsText
        => Localizer.Localize( Localizers?.AvailableFieldsLocalizer, LocalizationConstants.AvailableFields );

    internal string LocalizedDragFieldsText
        => Localizer.Localize( Localizers?.DragFieldsLocalizer, LocalizationConstants.DragFields );

    internal string LocalizedDropFieldText
        => Localizer.Localize( Localizers?.DropFieldLocalizer, LocalizationConstants.DropField );

    internal string LocalizedRowsText
        => Localizer.Localize( Localizers?.RowsLocalizer, LocalizationConstants.Rows );

    internal string LocalizedColumnsText
        => Localizer.Localize( Localizers?.ColumnsLocalizer, LocalizationConstants.Columns );

    internal string LocalizedExpandRowText
        => Localizer.Localize( Localizers?.ExpandRowLocalizer, LocalizationConstants.ExpandRow );

    internal string LocalizedCollapseRowText
        => Localizer.Localize( Localizers?.CollapseRowLocalizer, LocalizationConstants.CollapseRow );

    internal string LocalizedExpandColumnText
        => Localizer.Localize( Localizers?.ExpandColumnLocalizer, LocalizationConstants.ExpandColumn );

    internal string LocalizedCollapseColumnText
        => Localizer.Localize( Localizers?.CollapseColumnLocalizer, LocalizationConstants.CollapseColumn );

    internal string LocalizedFiltersText
        => Localizer.Localize( Localizers?.FiltersLocalizer, LocalizationConstants.Filters );

    internal string LocalizedApplyText
        => Localizer.Localize( Localizers?.ApplyLocalizer, LocalizationConstants.Apply );

    internal string LocalizedCancelText
        => Localizer.Localize( Localizers?.CancelLocalizer, LocalizationConstants.Cancel );

    internal string LocalizedAllText
        => Localizer.Localize( Localizers?.AllLocalizer, LocalizationConstants.All );

    internal string LocalizedAggregateText
        => Localizer.Localize( Localizers?.AggregateLocalizer, LocalizationConstants.Aggregate );

    internal string LocalizeAggregateFunction( PivotGridAggregateFunction aggregateFunction )
        => Localizer.Localize( null, aggregateFunction.ToString() );

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
            var expandedPivotResult = GetExpandedPivotResult();

            if ( !ShowPager || expandedPivotResult is null || !expandedPivotResult.HasValues )
                return expandedPivotResult;

            if ( UsesExternalData && externalDataIsPaged )
                return expandedPivotResult;

            if ( IsGroupPagingActive )
            {
                var pageRootGroupKeys = GetCurrentPageRootGroupKeys().ToHashSet( StringComparer.Ordinal );
                var groupRows = expandedPivotResult.Rows
                    .Where( row => PivotGridAxisItemUtilities.IsInGroupPage( row.Row, pageRootGroupKeys ) )
                    .ToList();

                return new( expandedPivotResult.RowFields, expandedPivotResult.ColumnFields, expandedPivotResult.Aggregates, expandedPivotResult.DataColumns, groupRows );
            }

            var rows = expandedPivotResult.Rows
                .Skip( ( CurrentPage - 1 ) * EffectivePageSize )
                .Take( EffectivePageSize )
                .ToList();

            return new( expandedPivotResult.RowFields, expandedPivotResult.ColumnFields, expandedPivotResult.Aggregates, expandedPivotResult.DataColumns, rows );
        }
    }

    internal bool IsPagerVisible
        => ShowPager && pivotResult is not null && pivotResult.HasValues && TotalRows > 0;

    internal bool IsVirtualizeActive
        => Virtualize && !ShowPager;

    internal bool IsExternalVirtualizeActive
        => UsesExternalData && IsVirtualizeActive;

    internal int VirtualizeOverscanCount
        => VirtualizeOptions?.OverscanCount ?? 10;

    internal float VirtualizeItemSize
        => VirtualizeOptions?.ItemSize ?? 50f;

    internal string VirtualizeTableHeight
        => VirtualizeOptions?.Height ?? "500px";

    internal string VirtualizeTableMaxHeight
        => VirtualizeOptions?.MaxHeight ?? VirtualizeTableHeight;

    internal bool IsGroupPagingActive
        => PageByGroups && pivotResult is not null && pivotResult.HasValues && pivotResult.RowFields.Count > 0;

    internal int TotalRows
        => UsesExternalData && externalDataIsPaged && externalTotalItems.HasValue
            ? externalTotalItems.Value
            : IsGroupPagingActive ? GetRootGroupKeys().Count : GetExpandedRows().Count;

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

    internal bool IsToolbarVisible
        => ToolbarTemplate is not null || ShowToolbar || ShowFieldChooser;

    private bool UsesExternalData
        => DataSource is not null || ReadData.HasDelegate;

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
    /// Defines an external data source used to read pivot grid data. When assigned, it has priority over <see cref="ReadData"/> and <see cref="Data"/>.
    /// </summary>
    [Parameter] public IPivotGridDataSource<TItem> DataSource { get; set; }

    /// <summary>
    /// Occurs when the pivot grid requests data from an external source. Ignored when <see cref="DataSource"/> is assigned.
    /// </summary>
    [Parameter] public EventCallback<PivotGridReadDataEventArgs<TItem>> ReadData { get; set; }

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
    /// Shows totals for each row as total columns.
    /// </summary>
    [Parameter] public bool ShowRowTotals { get; set; } = true;

    /// <summary>
    /// Shows totals for each column as total rows.
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
    /// Enables virtualized rendering of pivot rows. Ignored when <see cref="ShowPager"/> is enabled.
    /// </summary>
    /// <remarks>
    /// When local <see cref="Data"/> is used, virtualization reduces rendered rows only; the full pivot result is still computed before rendering.
    /// For large remote datasets, use <see cref="ReadData"/> or <see cref="DataSource"/> with <see cref="PivotGridReadDataMode.Virtualize"/> and return prepared <see cref="PivotGridDataResult{TItem}.Result"/> rows for the requested range.
    /// </remarks>
    [Parameter] public bool Virtualize { get; set; }

    /// <summary>
    /// Defines virtualized row rendering options.
    /// </summary>
    /// <remarks>
    /// These options tune rendered row virtualization only. They do not change local pivot grouping or aggregate calculation behavior.
    /// </remarks>
    [Parameter] public PivotGridVirtualizeOptions VirtualizeOptions { get; set; }

    /// <summary>
    /// Shows the PivotGrid toolbar.
    /// </summary>
    [Parameter] public bool ShowToolbar { get; set; }

    /// <summary>
    /// Shows the runtime field chooser.
    /// </summary>
    [Parameter] public bool ShowFieldChooser { get; set; }

    /// <summary>
    /// Enables expanding and collapsing row groups.
    /// </summary>
    [Parameter] public bool ExpandableRows { get; set; } = true;

    /// <summary>
    /// Enables expanding and collapsing column groups.
    /// </summary>
    [Parameter] public bool ExpandableColumns { get; set; }

    /// <summary>
    /// Defines whether expandable groups are expanded on first render.
    /// </summary>
    [Parameter] public bool InitiallyExpanded { get; set; } = true;

    /// <summary>
    /// Defines whether paging is applied to top-level row groups instead of rendered pivot rows.
    /// </summary>
    [Parameter] public bool PageByGroups { get; set; }

    /// <summary>
    /// Defines how row group captions are displayed.
    /// </summary>
    [Parameter] public PivotGridGroupCaptionMode RowGroupCaptionMode { get; set; } = PivotGridGroupCaptionMode.FullPath;

    /// <summary>
    /// Defines how column group captions are displayed.
    /// </summary>
    [Parameter] public PivotGridGroupCaptionMode ColumnGroupCaptionMode { get; set; } = PivotGridGroupCaptionMode.Leaf;

    /// <summary>
    /// Defines the separator used when group captions show the full path.
    /// </summary>
    [Parameter] public string GroupCaptionSeparator { get; set; } = " / ";

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
    /// Custom toolbar content.
    /// </summary>
    [Parameter] public RenderFragment<PivotGridToolbarContext<TItem>> ToolbarTemplate { get; set; }

    /// <summary>
    /// Custom content for row field header cells.
    /// </summary>
    [Parameter] public RenderFragment<PivotGridHeaderContext<TItem>> HeaderTemplate { get; set; }

    /// <summary>
    /// Custom content for column header cells.
    /// </summary>
    [Parameter] public RenderFragment<PivotGridColumnHeaderContext<TItem>> ColumnHeaderTemplate { get; set; }

    /// <summary>
    /// Custom content for row header cells.
    /// </summary>
    [Parameter] public RenderFragment<PivotGridRowHeaderContext<TItem>> RowHeaderTemplate { get; set; }

    /// <summary>
    /// Custom content for aggregate header cells.
    /// </summary>
    [Parameter] public RenderFragment<PivotGridAggregateHeaderContext<TItem>> AggregateHeaderTemplate { get; set; }

    /// <summary>
    /// Custom content for aggregate value cells.
    /// </summary>
    [Parameter] public RenderFragment<PivotGridCellContext<TItem>> CellTemplate { get; set; }

    /// <summary>
    /// Custom styling for aggregate value cells.
    /// </summary>
    [Parameter] public Action<PivotGridCellContext<TItem>, PivotGridCellStyling> CellStyling { get; set; }

    /// <summary>
    /// Custom styling for row header cells.
    /// </summary>
    [Parameter] public Action<PivotGridRowHeaderContext<TItem>, PivotGridCellStyling> RowHeaderStyling { get; set; }

    /// <summary>
    /// Custom styling for column header cells.
    /// </summary>
    [Parameter] public Action<PivotGridColumnHeaderContext<TItem>, PivotGridCellStyling> ColumnHeaderStyling { get; set; }

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