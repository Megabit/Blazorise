#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.DeepCloner;
using Blazorise.Gantt.Components;
using Blazorise.Gantt.Extensions;
using Blazorise.Gantt.Utilities;
using Blazorise.Localization;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Represents a native Blazorise Gantt chart component with tree and timeline views.
/// </summary>
/// <typeparam name="TItem">The item type rendered by the Gantt chart.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public partial class Gantt<TItem> : BaseComponent
{
    #region Members

    private const double DefaultRowHeight = 44d;
    private const double DefaultTimelineCellWidth = 72d;

    private readonly HashSet<string> collapsedNodeKeys = new( StringComparer.Ordinal );

    private GanttToolbar<TItem> ganttToolbar;
    private GanttDayView<TItem> ganttDayView;
    private GanttWeekView<TItem> ganttWeekView;
    private GanttMonthView<TItem> ganttMonthView;
    private GanttYearView<TItem> ganttYearView;

    private GanttPropertyMapper<TItem> propertyMapper;
    private Lazy<Func<TItem>> newItemCreator;
    private GanttEditingTransaction<TItem> currentEditingTransaction;
    private CancellationTokenSource readDataCancellationTokenSource;

    /// <summary>
    /// Reference to the internal item modal used for create/edit operations.
    /// </summary>
    protected _GanttItemModal<TItem> ganttItemModalRef;

    /// <summary>
    /// Currently edited item instance.
    /// </summary>
    protected TItem editItem;

    /// <summary>
    /// Parent item used when creating a new child item.
    /// </summary>
    protected TItem editParentItem;

    /// <summary>
    /// Current edit state.
    /// </summary>
    protected GanttEditState editState = GanttEditState.None;

    private DateOnly currentDate = DateOnly.FromDateTime( DateTime.Today );
    private string searchText = string.Empty;
    private bool showTitleColumn = true;
    private bool showStartColumn = true;
    private bool showEndColumn = true;
    private bool readDataInitialized;
    private bool readDataRequested;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new <see cref="Gantt{TItem}"/>.
    /// </summary>
    public Gantt()
    {
        propertyMapper = new GanttPropertyMapper<TItem>( this );
        newItemCreator = new( () => GanttFunctionCompiler.CreateNewItem<TItem>() );
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var previousSelectedView = SelectedView;
        var previousDate = currentDate;
        var previousSearchText = searchText;

        var mappingChanged =
            parameters.TryGetValue<string>( nameof( IdField ), out var paramIdField ) && !string.Equals( IdField, paramIdField, StringComparison.Ordinal ) ||
            parameters.TryGetValue<string>( nameof( ParentIdField ), out var paramParentIdField ) && !string.Equals( ParentIdField, paramParentIdField, StringComparison.Ordinal ) ||
            parameters.TryGetValue<string>( nameof( TitleField ), out var paramTitleField ) && !string.Equals( TitleField, paramTitleField, StringComparison.Ordinal ) ||
            parameters.TryGetValue<string>( nameof( DescriptionField ), out var paramDescriptionField ) && !string.Equals( DescriptionField, paramDescriptionField, StringComparison.Ordinal ) ||
            parameters.TryGetValue<string>( nameof( StartField ), out var paramStartField ) && !string.Equals( StartField, paramStartField, StringComparison.Ordinal ) ||
            parameters.TryGetValue<string>( nameof( EndField ), out var paramEndField ) && !string.Equals( EndField, paramEndField, StringComparison.Ordinal ) ||
            parameters.TryGetValue<string>( nameof( DurationField ), out var paramDurationField ) && !string.Equals( DurationField, paramDurationField, StringComparison.Ordinal ) ||
            parameters.TryGetValue<string>( nameof( ProgressField ), out var paramProgressField ) && !string.Equals( ProgressField, paramProgressField, StringComparison.Ordinal );

        if ( parameters.TryGetValue<DateOnly>( nameof( Date ), out var paramDate ) )
            currentDate = paramDate;

        if ( parameters.TryGetValue<string>( nameof( SearchText ), out var paramSearchText ) )
            searchText = paramSearchText ?? string.Empty;

        if ( parameters.TryGetValue<bool>( nameof( ShowTitleColumn ), out var paramShowTitleColumn ) )
            showTitleColumn = paramShowTitleColumn;

        if ( parameters.TryGetValue<bool>( nameof( ShowStartColumn ), out var paramShowStartColumn ) )
            showStartColumn = paramShowStartColumn;

        if ( parameters.TryGetValue<bool>( nameof( ShowEndColumn ), out var paramShowEndColumn ) )
            showEndColumn = paramShowEndColumn;

        await base.SetParametersAsync( parameters );

        EnsureAtLeastOneColumnVisible();

        if ( mappingChanged )
        {
            propertyMapper = new GanttPropertyMapper<TItem>( this );
        }

        if ( ReadData.HasDelegate && !readDataInitialized )
        {
            readDataInitialized = true;
            readDataRequested = true;
        }

        if ( ReadData.HasDelegate
             && ( previousSelectedView != SelectedView
                  || previousDate != currentDate
                  || !string.Equals( previousSearchText, searchText, StringComparison.Ordinal ) ) )
        {
            readDataRequested = true;
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        currentDate = Date;
        searchText = SearchText ?? string.Empty;
        showTitleColumn = ShowTitleColumn;
        showStartColumn = ShowStartColumn;
        showEndColumn = ShowEndColumn;

        EnsureAtLeastOneColumnVisible();

        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( readDataRequested && ReadData.HasDelegate )
        {
            readDataRequested = false;

            await HandleReadData( CancellationToken.None );
            await InvokeAsync( StateHasChanged );
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-gantt" );

        base.BuildClasses( builder );
    }

    /// <inheritdoc />
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( readDataCancellationTokenSource is not null )
            {
                readDataCancellationTokenSource.Cancel();
                readDataCancellationTokenSource.Dispose();
                readDataCancellationTokenSource = null;
            }
        }

        base.Dispose( disposing );
    }

    internal void NotifyGanttToolbar( GanttToolbar<TItem> ganttToolbar )
    {
        var changed = !ReferenceEquals( this.ganttToolbar, ganttToolbar );
        this.ganttToolbar = ganttToolbar;

        if ( changed )
            _ = InvokeAsync( StateHasChanged );
    }

    internal void NotifyGanttDayView( GanttDayView<TItem> ganttDayView )
    {
        var changed = !ReferenceEquals( this.ganttDayView, ganttDayView );
        this.ganttDayView = ganttDayView;

        if ( changed )
            _ = InvokeAsync( StateHasChanged );
    }

    internal void NotifyGanttWeekView( GanttWeekView<TItem> ganttWeekView )
    {
        var changed = !ReferenceEquals( this.ganttWeekView, ganttWeekView );
        this.ganttWeekView = ganttWeekView;

        if ( changed )
            _ = InvokeAsync( StateHasChanged );
    }

    internal void NotifyGanttMonthView( GanttMonthView<TItem> ganttMonthView )
    {
        var changed = !ReferenceEquals( this.ganttMonthView, ganttMonthView );
        this.ganttMonthView = ganttMonthView;

        if ( changed )
            _ = InvokeAsync( StateHasChanged );
    }

    internal void NotifyGanttYearView( GanttYearView<TItem> ganttYearView )
    {
        var changed = !ReferenceEquals( this.ganttYearView, ganttYearView );
        this.ganttYearView = ganttYearView;

        if ( changed )
            _ = InvokeAsync( StateHasChanged );
    }

    internal Task Refresh()
    {
        return RefreshInternalAsync();
    }

    /// <summary>
    /// Navigates to the previous period based on the selected view.
    /// </summary>
    public async Task NavigatePreviousPeriod()
    {
        if ( SelectedView == GanttView.Week )
        {
            currentDate = currentDate.StartOfWeek( FirstDayOfWeek ).AddDays( -7 );
        }
        else if ( SelectedView == GanttView.Month )
        {
            currentDate = currentDate.AddMonths( -1 );
        }
        else if ( SelectedView == GanttView.Year )
        {
            currentDate = currentDate.AddYears( -1 );
        }
        else
        {
            currentDate = currentDate.AddDays( -1 );
        }

        await DateChanged.InvokeAsync( currentDate );
        await RefreshInternalAsync();
    }

    /// <summary>
    /// Navigates to the next period based on the selected view.
    /// </summary>
    public async Task NavigateNextPeriod()
    {
        if ( SelectedView == GanttView.Week )
        {
            currentDate = currentDate.StartOfWeek( FirstDayOfWeek ).AddDays( 7 );
        }
        else if ( SelectedView == GanttView.Month )
        {
            currentDate = currentDate.AddMonths( 1 );
        }
        else if ( SelectedView == GanttView.Year )
        {
            currentDate = currentDate.AddYears( 1 );
        }
        else
        {
            currentDate = currentDate.AddDays( 1 );
        }

        await DateChanged.InvokeAsync( currentDate );
        await RefreshInternalAsync();
    }

    /// <summary>
    /// Navigates the chart to current date.
    /// </summary>
    public async Task NavigateToday()
    {
        currentDate = DateOnly.FromDateTime( DateTime.Today );
        await DateChanged.InvokeAsync( currentDate );
        await RefreshInternalAsync();
    }

    /// <summary>
    /// Switches to Day view.
    /// </summary>
    public async Task ShowDayView()
    {
        if ( SelectedView == GanttView.Day )
            return;

        SelectedView = GanttView.Day;
        await SelectedViewChanged.InvokeAsync( SelectedView );
        await RefreshInternalAsync();
    }

    /// <summary>
    /// Switches to Week view.
    /// </summary>
    public async Task ShowWeekView()
    {
        if ( SelectedView == GanttView.Week )
            return;

        SelectedView = GanttView.Week;
        await SelectedViewChanged.InvokeAsync( SelectedView );
        await RefreshInternalAsync();
    }

    /// <summary>
    /// Switches to Month view.
    /// </summary>
    public async Task ShowMonthView()
    {
        if ( SelectedView == GanttView.Month )
            return;

        SelectedView = GanttView.Month;
        await SelectedViewChanged.InvokeAsync( SelectedView );
        await RefreshInternalAsync();
    }

    /// <summary>
    /// Switches to Year view.
    /// </summary>
    public async Task ShowYearView()
    {
        if ( SelectedView == GanttView.Year )
            return;

        SelectedView = GanttView.Year;
        await SelectedViewChanged.InvokeAsync( SelectedView );
        await RefreshInternalAsync();
    }

    /// <summary>
    /// Creates a new item using configured new-item factory.
    /// </summary>
    public Task New()
    {
        return New( CreateNewItem() );
    }

    /// <summary>
    /// Creates a new top-level item.
    /// </summary>
    public Task New( TItem item )
    {
        return New( item, default );
    }

    /// <summary>
    /// Creates a new child item for specified parent.
    /// </summary>
    public Task AddChild( TItem parentItem )
    {
        if ( parentItem is null )
            throw new ArgumentNullException( nameof( parentItem ), "Parent item is not assigned." );

        return New( CreateNewItem(), parentItem );
    }

    /// <summary>
    /// Puts chart in new-item editing state.
    /// </summary>
    public async Task New( TItem item, TItem parentItem )
    {
        if ( item is null )
            throw new ArgumentNullException( nameof( item ), "New item is not assigned." );

        ApplyNewItemDateDefaults( item, parentItem );

        editItem = item;
        editParentItem = parentItem;
        editState = GanttEditState.New;

        if ( Editable && UseInternalEditing && ganttItemModalRef is not null )
        {
            await ganttItemModalRef.ShowModal( item.DeepClone(), editState, parentItem );
        }
    }

    /// <summary>
    /// Puts chart in edit-item state for specified item.
    /// </summary>
    public async Task Edit( TItem item )
    {
        if ( item is null )
            throw new ArgumentNullException( nameof( item ), "Edit item is not assigned." );

        if ( EditItemClicked.HasDelegate )
        {
            await EditItemClicked.InvokeAsync( new GanttItemClickedEventArgs<TItem>( item ) );
        }

        editItem = item;
        editParentItem = default;
        editState = GanttEditState.Edit;

        if ( Editable && UseInternalEditing && ganttItemModalRef is not null )
        {
            await ganttItemModalRef.ShowModal( item.DeepClone(), editState );
        }
    }

    /// <summary>
    /// Deletes specified item.
    /// </summary>
    public async Task Delete( TItem item )
    {
        if ( item is null )
            throw new ArgumentNullException( nameof( item ), "Delete item is not assigned." );

        if ( Editable && UseInternalEditing )
        {
            await DeleteItemImpl( item );
        }
    }

    /// <summary>
    /// Persists submitted item changes through editing transaction.
    /// </summary>
    /// <param name="submittedItem">Submitted item values.</param>
    /// <returns>True if operation succeeds; otherwise false.</returns>
    protected internal async Task<bool> SaveImpl( TItem submittedItem )
    {
        if ( submittedItem is null || editItem is null )
            return false;

        if ( currentEditingTransaction is not null )
        {
            await currentEditingTransaction.Rollback();
            currentEditingTransaction = null;
        }

        currentEditingTransaction = new GanttEditingTransaction<TItem>( this, editItem, submittedItem, editState )
        {
            Committed = ResetEditingTransaction,
            Canceled = ResetEditingTransaction,
        };

        return await currentEditingTransaction.Commit();
    }

    /// <summary>
    /// Deletes an item and all of its children when hierarchical mapping is available.
    /// </summary>
    /// <param name="itemToDelete">Item to remove.</param>
    /// <returns>True if delete operation succeeds; otherwise false.</returns>
    protected internal async Task<bool> DeleteItemImpl( TItem itemToDelete )
    {
        if ( itemToDelete is null )
            return false;

        if ( DeleteItemClicked.HasDelegate )
        {
            await DeleteItemClicked.InvokeAsync( new GanttItemClickedEventArgs<TItem>( itemToDelete ) );
        }

        if ( !await IsSafeToProceed( ItemRemoving, itemToDelete, itemToDelete ) )
            return false;

        if ( CanEditData && Data is ICollection<TItem> data )
        {
            RemoveItemAndChildren( data, itemToDelete );
        }

        await ItemRemoved.InvokeAsync( new GanttUpdatedItem<TItem>( itemToDelete, itemToDelete ) );

        editState = GanttEditState.None;
        editItem = default;
        editParentItem = default;

        await RefreshInternalAsync();

        return true;
    }

    internal async Task<bool> CommitEditInternalAsync( TItem targetItem, TItem submittedItem, GanttEditState submittedEditState )
    {
        if ( targetItem is null || submittedItem is null )
            return false;

        var handler = submittedEditState == GanttEditState.New ? ItemInserting : ItemUpdating;
        var oldItem = submittedEditState == GanttEditState.New ? targetItem : targetItem.DeepClone();
        var newItem = submittedItem.DeepClone();

        if ( !await IsSafeToProceed( handler, targetItem, newItem ) )
            return false;

        if ( submittedEditState == GanttEditState.New && UseInternalEditing && CanInsertNewItem && Data is ICollection<TItem> data )
        {
            data.Add( targetItem );
        }

        if ( UseInternalEditing || submittedEditState == GanttEditState.New )
        {
            CopyItemValues( newItem, targetItem );
        }

        if ( submittedEditState == GanttEditState.New )
        {
            await ItemInserted.InvokeAsync( new GanttInsertedItem<TItem>( newItem ) );
        }
        else
        {
            await ItemUpdated.InvokeAsync( new GanttUpdatedItem<TItem>( oldItem, newItem ) );
        }

        editState = GanttEditState.None;
        editItem = default;
        editParentItem = default;

        await RefreshInternalAsync();

        return true;
    }

    internal async Task<bool> IsSafeToProceed( EventCallback<GanttCancellableItemChange<TItem>> handler, TItem oldItem, TItem newItem )
    {
        if ( handler.HasDelegate )
        {
            var args = new GanttCancellableItemChange<TItem>( oldItem, newItem );
            await handler.InvokeAsync( args );

            if ( args.Cancel )
                return false;
        }

        return true;
    }

    /// <summary>
    /// Creates a new item using custom or auto-generated item factory.
    /// </summary>
    /// <returns>New item instance.</returns>
    protected internal TItem CreateNewItem()
        => NewItemCreator is not null ? NewItemCreator.Invoke() : newItemCreator.Value();

    /// <summary>
    /// Creates a new item identifier using custom or default generator.
    /// </summary>
    /// <returns>Generated identifier.</returns>
    protected internal object CreateNewId()
        => NewIdCreator is not null ? NewIdCreator.Invoke() : Guid.NewGuid().ToString();

    internal void CopyItemValues( TItem source, TItem destination )
    {
        if ( source is null || destination is null )
            return;

        propertyMapper.SetId( destination, propertyMapper.GetId( source ) );
        propertyMapper.SetParentId( destination, propertyMapper.GetParentId( source ) );
        propertyMapper.SetTitle( destination, propertyMapper.GetTitle( source ) );
        propertyMapper.SetDescription( destination, propertyMapper.GetDescription( source ) );
        propertyMapper.SetStart( destination, propertyMapper.GetStart( source ) );
        propertyMapper.SetEnd( destination, propertyMapper.GetEnd( source ) );
        propertyMapper.SetDuration( destination, propertyMapper.GetDuration( source ) );
        propertyMapper.SetProgress( destination, propertyMapper.GetProgress( source ) );
    }

    private async Task RefreshInternalAsync()
    {
        if ( ReadData.HasDelegate )
        {
            await HandleReadData( CancellationToken.None );
        }

        await InvokeAsync( StateHasChanged );
    }

    private async Task HandleReadData( CancellationToken cancellationToken )
    {
        if ( !ReadData.HasDelegate )
            return;

        readDataCancellationTokenSource?.Cancel();
        readDataCancellationTokenSource?.Dispose();
        readDataCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );

        var token = readDataCancellationTokenSource.Token;
        var viewRange = GetCurrentViewRange();

        try
        {
            await ReadData.InvokeAsync( new GanttReadDataEventArgs<TItem>(
                SelectedView,
                currentDate,
                viewRange.Start,
                viewRange.End,
                searchText,
                token ) );
        }
        catch ( OperationCanceledException ) when ( token.IsCancellationRequested )
        {
        }
    }

    private Task ResetEditingTransaction()
    {
        currentEditingTransaction = null;
        return Task.CompletedTask;
    }

    private async Task SetSearchText( string value )
    {
        var normalizedValue = value ?? string.Empty;

        if ( string.Equals( searchText, normalizedValue, StringComparison.Ordinal ) )
            return;

        searchText = normalizedValue;

        await SearchTextChanged.InvokeAsync( searchText );
        await RefreshInternalAsync();
    }

    private async Task SetShowTitleColumn( bool value )
    {
        showTitleColumn = value;
        EnsureAtLeastOneColumnVisible();

        await NotifyColumnVisibilityChanged();
        await InvokeAsync( StateHasChanged );
    }

    private async Task SetShowStartColumn( bool value )
    {
        showStartColumn = value;
        EnsureAtLeastOneColumnVisible();

        await NotifyColumnVisibilityChanged();
        await InvokeAsync( StateHasChanged );
    }

    private async Task SetShowEndColumn( bool value )
    {
        showEndColumn = value;
        EnsureAtLeastOneColumnVisible();

        await NotifyColumnVisibilityChanged();
        await InvokeAsync( StateHasChanged );
    }

    private async Task NotifyColumnVisibilityChanged()
    {
        await ShowTitleColumnChanged.InvokeAsync( showTitleColumn );
        await ShowStartColumnChanged.InvokeAsync( showStartColumn );
        await ShowEndColumnChanged.InvokeAsync( showEndColumn );
    }

    private void EnsureAtLeastOneColumnVisible()
    {
        if ( !showTitleColumn && !showStartColumn && !showEndColumn )
        {
            showTitleColumn = true;
        }
    }

    private DateTime GetItemStart( TItem item )
    {
        if ( item is null || !propertyMapper.HasStart )
            return DateTime.MinValue;

        return propertyMapper.GetStart( item );
    }

    private DateTime GetItemEnd( TItem item )
    {
        if ( item is null || !propertyMapper.HasEnd )
            return DateTime.MinValue;

        return propertyMapper.GetEnd( item );
    }

    private string GetItemTitle( TItem item )
    {
        if ( item is null || !propertyMapper.HasTitle )
            return string.Empty;

        return propertyMapper.GetTitle( item ) ?? string.Empty;
    }

    private string GetItemDescription( TItem item )
    {
        if ( item is null || !propertyMapper.HasDescription )
            return string.Empty;

        return propertyMapper.GetDescription( item ) ?? string.Empty;
    }

    private double? GetItemProgress( TItem item )
    {
        if ( !propertyMapper.HasProgress || item is null )
            return null;

        if ( !TryConvertToDouble( propertyMapper.GetProgress( item ), out var progressValue ) )
            return null;

        if ( progressValue <= 1d )
            progressValue *= 100d;

        return Math.Max( 0d, Math.Min( 100d, progressValue ) );
    }

    private string FormatDate( DateTime date )
    {
        if ( date == DateTime.MinValue || date == DateTime.MaxValue )
            return string.Empty;

        return SelectedView == GanttView.Day
            ? date.ToString( "MMM dd, yyyy HH:mm", CultureInfo.InvariantCulture )
            : date.ToString( "MMM dd, yyyy", CultureInfo.InvariantCulture );
    }

    private void ApplyNewItemDateDefaults( TItem item, TItem parentItem )
    {
        if ( item is null || ( !propertyMapper.HasStart && !propertyMapper.HasEnd ) )
            return;

        var (defaultStart, defaultEnd) = GetDefaultNewItemRange( parentItem );

        if ( propertyMapper.HasStart )
        {
            var itemStart = propertyMapper.GetStart( item );

            if ( IsUnassignedDate( itemStart ) )
            {
                propertyMapper.SetStart( item, defaultStart );
            }
            else
            {
                defaultStart = itemStart;
            }
        }

        if ( propertyMapper.HasEnd )
        {
            var itemEnd = propertyMapper.GetEnd( item );

            if ( IsUnassignedDate( itemEnd ) || itemEnd <= defaultStart )
            {
                if ( defaultEnd <= defaultStart )
                {
                    defaultEnd = defaultStart.Add( GetDefaultNewItemDuration() );
                }

                propertyMapper.SetEnd( item, defaultEnd );
            }
        }

        if ( propertyMapper.HasDuration && propertyMapper.HasStart )
        {
            var itemStart = propertyMapper.GetStart( item );
            var itemDuration = propertyMapper.GetDuration( item );

            if ( itemDuration <= 0 )
            {
                var itemEnd = propertyMapper.HasEnd
                    ? propertyMapper.GetEnd( item )
                    : itemStart.Add( GetDefaultNewItemDuration() );

                if ( IsUnassignedDate( itemEnd ) || itemEnd <= itemStart )
                    itemEnd = itemStart.Add( GetDefaultNewItemDuration() );

                itemDuration = GetDurationInDays( itemStart, itemEnd );
                propertyMapper.SetDuration( item, itemDuration );
            }

            if ( propertyMapper.HasEnd )
            {
                propertyMapper.SetEnd( item, itemStart.AddDays( itemDuration ) );
            }
        }
    }

    private (DateTime Start, DateTime End) GetDefaultNewItemRange( TItem parentItem )
    {
        var viewRange = GetCurrentViewRange();
        var duration = GetDefaultNewItemDuration();
        var start = viewRange.Start;
        var end = start.Add( duration );

        if ( parentItem is not null )
        {
            var parentStart = propertyMapper.HasStart ? GetItemStart( parentItem ) : DateTime.MinValue;
            var parentEnd = propertyMapper.HasEnd ? GetItemEnd( parentItem ) : DateTime.MinValue;

            if ( !IsUnassignedDate( parentStart ) )
            {
                start = parentStart;
            }

            end = start.Add( duration );

            if ( !IsUnassignedDate( parentEnd ) && parentEnd > start && end > parentEnd )
            {
                end = parentEnd;
            }
        }

        if ( end <= start )
        {
            end = start.Add( duration );
        }

        return (start, end);
    }

    private TimeSpan GetDefaultNewItemDuration()
    {
        return SelectedView switch
        {
            GanttView.Day => TimeSpan.FromHours( 1 ),
            GanttView.Week => TimeSpan.FromDays( 1 ),
            GanttView.Month => TimeSpan.FromDays( 3 ),
            GanttView.Year => TimeSpan.FromDays( 30 ),
            _ => TimeSpan.FromDays( 1 ),
        };
    }

    private static bool IsUnassignedDate( DateTime value )
    {
        return value == DateTime.MinValue || value == DateTime.MaxValue;
    }

    private static int GetDurationInDays( DateTime start, DateTime end )
    {
        var totalDays = ( end - start ).TotalDays;

        if ( totalDays <= 0d )
            return 1;

        return Math.Max( 1, (int)Math.Ceiling( totalDays ) );
    }

    private async Task OnBarClicked( TItem item )
    {
        if ( ItemClicked.HasDelegate )
        {
            await ItemClicked.InvokeAsync( new GanttItemClickedEventArgs<TItem>( item ) );
        }

        if ( Editable )
        {
            await Edit( item );
        }
    }

    private Task ToggleNode( GanttTreeRow node )
    {
        if ( IsSearchMode || node is null )
            return Task.CompletedTask;

        if ( !collapsedNodeKeys.Add( node.Key ) )
        {
            collapsedNodeKeys.Remove( node.Key );
        }

        return InvokeAsync( StateHasChanged );
    }

    private bool IsExpanded( GanttTreeRow node )
    {
        if ( node is null )
            return false;

        if ( IsSearchMode )
            return true;

        return !collapsedNodeKeys.Contains( node.Key );
    }

    private List<GanttTreeRow> GetVisibleRows()
    {
        var roots = BuildTree();

        if ( roots.Count == 0 )
            return new List<GanttTreeRow>();

        var includeLookup = IsSearchMode
            ? BuildSearchIncludeLookup( roots )
            : null;

        var rows = new List<GanttTreeRow>();

        foreach ( var root in SortNodes( roots ) )
        {
            AppendVisibleRows( root, rows, includeLookup );
        }

        return rows;
    }

    private Dictionary<string, bool> BuildSearchIncludeLookup( IReadOnlyCollection<GanttTreeNode> roots )
    {
        var includeLookup = new Dictionary<string, bool>( StringComparer.Ordinal );

        foreach ( var root in roots )
        {
            EvaluateSearchMatch( root, includeLookup );
        }

        return includeLookup;
    }

    private bool EvaluateSearchMatch( GanttTreeNode node, Dictionary<string, bool> includeLookup )
    {
        var includeSelf = MatchesSearch( node.Item );
        var includeChild = false;

        foreach ( var child in node.Children )
        {
            if ( EvaluateSearchMatch( child, includeLookup ) )
            {
                includeChild = true;
            }
        }

        var includeNode = includeSelf || includeChild;
        includeLookup[node.Key] = includeNode;

        return includeNode;
    }

    private bool MatchesSearch( TItem item )
    {
        if ( !IsSearchMode )
            return true;

        var term = searchText?.Trim();

        if ( string.IsNullOrWhiteSpace( term ) )
            return true;

        return ( GetItemTitle( item )?.Contains( term, StringComparison.OrdinalIgnoreCase ) ?? false )
               || ( GetItemDescription( item )?.Contains( term, StringComparison.OrdinalIgnoreCase ) ?? false );
    }

    private void AppendVisibleRows( GanttTreeNode node, ICollection<GanttTreeRow> rows, Dictionary<string, bool> includeLookup )
    {
        if ( includeLookup is not null
             && includeLookup.TryGetValue( node.Key, out var includeNode )
             && !includeNode )
            return;

        var visibleChildren = GetVisibleChildren( node, includeLookup );

        rows.Add( new GanttTreeRow( node.Key, node.Item, node.Level, visibleChildren.Count > 0 ) );

        if ( IsSearchMode || !collapsedNodeKeys.Contains( node.Key ) )
        {
            foreach ( var child in SortNodes( visibleChildren ) )
            {
                AppendVisibleRows( child, rows, includeLookup );
            }
        }
    }

    private IReadOnlyCollection<GanttTreeNode> GetVisibleChildren( GanttTreeNode node, Dictionary<string, bool> includeLookup )
    {
        if ( includeLookup is null )
            return node.Children;

        return node.Children
            .Where( child => includeLookup.TryGetValue( child.Key, out var includeNode ) && includeNode )
            .ToList();
    }

    private List<GanttTreeNode> BuildTree()
    {
        var nodeList = new List<GanttTreeNode>();
        var nodesById = new Dictionary<string, GanttTreeNode>( StringComparer.Ordinal );
        var roots = new List<GanttTreeNode>();
        var usedKeys = new HashSet<string>( StringComparer.Ordinal );

        var items = Data ?? Array.Empty<TItem>();
        var index = 0;

        foreach ( var item in items )
        {
            if ( item is null )
            {
                index++;
                continue;
            }

            var idKey = NormalizeIdentifier( propertyMapper.HasId ? propertyMapper.GetId( item ) : null );
            var stableKey = !string.IsNullOrEmpty( idKey )
                ? idKey
                : $"idx-{index.ToString( CultureInfo.InvariantCulture )}";

            var uniqueKey = stableKey;

            while ( !usedKeys.Add( uniqueKey ) )
            {
                uniqueKey = $"{stableKey}-{index.ToString( CultureInfo.InvariantCulture )}";
            }

            var node = new GanttTreeNode( uniqueKey, item );

            nodeList.Add( node );

            if ( !string.IsNullOrEmpty( idKey ) && !nodesById.ContainsKey( idKey ) )
            {
                nodesById.Add( idKey, node );
            }

            index++;
        }

        foreach ( var node in nodeList )
        {
            var parentIdentifier = propertyMapper.HasParentId
                ? NormalizeIdentifier( propertyMapper.GetParentId( node.Item ) )
                : null;

            if ( !string.IsNullOrEmpty( parentIdentifier )
                 && nodesById.TryGetValue( parentIdentifier, out var parentNode )
                 && !ReferenceEquals( parentNode, node ) )
            {
                parentNode.Children.Add( node );
            }
            else
            {
                roots.Add( node );
            }
        }

        if ( roots.Count == 0 && nodeList.Count > 0 )
        {
            roots.AddRange( nodeList );
        }

        foreach ( var root in roots )
        {
            AssignLevels( root, 0, new HashSet<string>( StringComparer.Ordinal ) );
        }

        return roots;
    }

    private IReadOnlyCollection<GanttTreeNode> SortNodes( IReadOnlyCollection<GanttTreeNode> nodes )
    {
        return nodes
            .OrderBy( x => GetItemStart( x.Item ) )
            .ThenBy( x => GetItemTitle( x.Item ), StringComparer.OrdinalIgnoreCase )
            .ToList();
    }

    private void AssignLevels( GanttTreeNode node, int level, ISet<string> recursionGuard )
    {
        if ( !recursionGuard.Add( node.Key ) )
            return;

        node.Level = level;

        foreach ( var child in node.Children )
        {
            AssignLevels( child, level + 1, recursionGuard );
        }

        recursionGuard.Remove( node.Key );
    }

    private string GetTreePaneStyle()
    {
        var width = GetTreePaneWidth();
        var widthText = width.ToString( "0.###", CultureInfo.InvariantCulture );

        return $"display: flex; flex-direction: column; width: {widthText}px; min-width: {widthText}px; max-width: {widthText}px; overflow: hidden;";
    }

    private string GetBodyStyle()
    {
        return "min-height: 360px; max-height: 100%; min-width: 0; overflow: hidden;";
    }

    private string GetTreeRowsStyle()
    {
        return "height: calc(100% - 44px); max-height: 100%; overflow-x: hidden; overflow-y: hidden;";
    }

    private string GetTimelinePaneStyle()
    {
        return "min-width: 0; min-height: 0; overflow: hidden;";
    }

    private string GetTimelineViewportStyle()
    {
        return "height: 100%; width: 100%; min-width: 0; min-height: 0; overflow-x: scroll; overflow-y: auto;";
    }

    private string GetTreeRowStyle( double rowHeight )
    {
        var rowHeightText = rowHeight.ToString( "0.###", CultureInfo.InvariantCulture );

        return $"height: {rowHeightText}px; min-height: {rowHeightText}px; border-bottom: 1px solid rgba(0,0,0,0.08);";
    }

    private string GetTreeTitleContentStyle( int level )
    {
        var paddingLeft = Math.Max( 0d, level * TreeIndentSize ).ToString( "0.###", CultureInfo.InvariantCulture );
        return $"display: flex; align-items: center; min-width: 0; width: 100%; padding-left: {paddingLeft}px;";
    }

    private string GetTitleColumnStyle()
    {
        var width = TitleColumnWidth.ToString( "0.###", CultureInfo.InvariantCulture );
        return $"width: {width}px; min-width: {width}px; max-width: {width}px; overflow: hidden;";
    }

    private string GetDateColumnStyle()
    {
        var width = DateColumnWidth.ToString( "0.###", CultureInfo.InvariantCulture );
        return $"width: {width}px; min-width: {width}px; max-width: {width}px; overflow: hidden;";
    }

    private string GetActionColumnStyle()
    {
        var width = ActionColumnWidth.ToString( "0.###", CultureInfo.InvariantCulture );
        return $"display: flex; align-items: center; justify-content: center; width: {width}px; min-width: {width}px; max-width: {width}px; overflow: hidden; border-left: 1px solid rgba(0,0,0,0.08);";
    }

    private string GetTimelineInnerStyle( int slotsCount, double cellWidth )
    {
        var width = Math.Max( 1, slotsCount ) * cellWidth;
        var widthText = width.ToString( "0.###", CultureInfo.InvariantCulture );

        return $"position: relative; width: {widthText}px; min-width: {widthText}px;";
    }

    private string GetTimelineHeaderCellStyle( double cellWidth )
    {
        var width = cellWidth.ToString( "0.###", CultureInfo.InvariantCulture );
        return $"display: flex; align-items: center; border-right: 1px solid rgba(0,0,0,0.08); width: {width}px; min-width: {width}px; max-width: {width}px;";
    }

    private string GetTimelineRowStyle( double rowHeight, double cellWidth )
    {
        var rowHeightText = rowHeight.ToString( "0.###", CultureInfo.InvariantCulture );
        var cellWidthText = cellWidth.ToString( "0.###", CultureInfo.InvariantCulture );

        return $"position: relative; width: 100%; min-width: 100%; height: {rowHeightText}px; min-height: {rowHeightText}px; border-bottom: 1px solid rgba(0,0,0,0.08); background-image: repeating-linear-gradient(to right, rgba(0,0,0,0.07), rgba(0,0,0,0.07) 1px, transparent 1px, transparent {cellWidthText}px);";
    }

    private bool TryGetVisibleBar( TItem item, DateTime viewStart, DateTime viewEnd, int slotsCount, double cellWidth, out GanttBarInfo barInfo )
    {
        barInfo = default;

        var itemStart = GetItemStart( item );
        var itemEnd = GetItemEnd( item );

        if ( itemStart == DateTime.MinValue || itemEnd == DateTime.MinValue )
            return false;

        if ( itemEnd <= itemStart )
            itemEnd = itemStart.AddHours( 1 );

        if ( itemEnd <= viewStart || itemStart >= viewEnd )
            return false;

        var visibleStart = itemStart < viewStart ? viewStart : itemStart;
        var visibleEnd = itemEnd > viewEnd ? viewEnd : itemEnd;

        var totalMinutes = ( viewEnd - viewStart ).TotalMinutes;

        if ( totalMinutes <= 0d )
            return false;

        var totalWidth = Math.Max( 1, slotsCount ) * cellWidth;

        var left = ( ( visibleStart - viewStart ).TotalMinutes / totalMinutes ) * totalWidth;
        var width = ( ( visibleEnd - visibleStart ).TotalMinutes / totalMinutes ) * totalWidth;

        if ( width < MinBarWidth )
            width = MinBarWidth;

        barInfo = new GanttBarInfo( left, width, GetItemProgress( item ) );

        return true;
    }

    private string GetBarStyle( GanttBarInfo barInfo, double rowHeight, string itemCustomStyle )
    {
        var barHeight = Math.Max( 16d, rowHeight - 12d );
        var top = Math.Max( 1d, ( rowHeight - barHeight ) / 2d );

        var style = $"position: absolute; left: {barInfo.Left.ToString( "0.###", CultureInfo.InvariantCulture )}px; width: {barInfo.Width.ToString( "0.###", CultureInfo.InvariantCulture )}px; top: {top.ToString( "0.###", CultureInfo.InvariantCulture )}px; height: {barHeight.ToString( "0.###", CultureInfo.InvariantCulture )}px; display: flex; align-items: center; cursor: pointer;";

        if ( !string.IsNullOrWhiteSpace( itemCustomStyle ) )
            style = $"{style} {itemCustomStyle}";

        return style;
    }

    private GanttItemStyling GetItemStyling( TItem item )
    {
        var itemStyling = new GanttItemStyling();

        ItemStyling?.Invoke( item, itemStyling );

        return itemStyling;
    }

    private GanttViewRange GetCurrentViewRange()
    {
        if ( SelectedView == GanttView.Week )
        {
            var start = currentDate.StartOfWeek( FirstDayOfWeek ).ToDateTime( TimeOnly.MinValue );
            return new GanttViewRange( start, start.AddDays( 7 ) );
        }

        if ( SelectedView == GanttView.Month )
        {
            var start = currentDate.StartOfMonth().ToDateTime( TimeOnly.MinValue );
            return new GanttViewRange( start, start.AddMonths( 1 ) );
        }

        if ( SelectedView == GanttView.Year )
        {
            var start = new DateOnly( currentDate.Year, 1, 1 ).ToDateTime( TimeOnly.MinValue );
            return new GanttViewRange( start, start.AddYears( 1 ) );
        }

        var dayStart = currentDate.ToDateTime( TimeOnly.MinValue );
        return new GanttViewRange( dayStart, dayStart.AddDays( 1 ) );
    }

    private List<GanttTimeSlot> GetTimeSlots( DateTime viewStart, DateTime viewEnd )
    {
        var slots = new List<GanttTimeSlot>();

        if ( SelectedView == GanttView.Week )
        {
            var current = viewStart;
            var index = 0;

            while ( current < viewEnd )
            {
                var next = current.AddDays( 1 );
                slots.Add( new GanttTimeSlot(
                    key: $"week-{index.ToString( CultureInfo.InvariantCulture )}",
                    label: current.ToString( "ddd dd", CultureInfo.InvariantCulture ),
                    start: current,
                    end: next ) );

                current = next;
                index++;
            }

            return slots;
        }

        if ( SelectedView == GanttView.Month )
        {
            var current = viewStart;
            var index = 0;

            while ( current < viewEnd )
            {
                var next = current.AddDays( 1 );
                slots.Add( new GanttTimeSlot(
                    key: $"month-{index.ToString( CultureInfo.InvariantCulture )}",
                    label: current.ToString( "dd", CultureInfo.InvariantCulture ),
                    start: current,
                    end: next ) );

                current = next;
                index++;
            }

            return slots;
        }

        if ( SelectedView == GanttView.Year )
        {
            var current = viewStart;
            var index = 0;

            while ( current < viewEnd )
            {
                var next = current.AddMonths( 1 );
                slots.Add( new GanttTimeSlot(
                    key: $"year-{index.ToString( CultureInfo.InvariantCulture )}",
                    label: current.ToString( "MMM", CultureInfo.InvariantCulture ),
                    start: current,
                    end: next ) );

                current = next;
                index++;
            }

            return slots;
        }

        {
            var current = viewStart;
            var index = 0;

            while ( current < viewEnd )
            {
                var next = current.AddHours( 1 );
                slots.Add( new GanttTimeSlot(
                    key: $"day-{index.ToString( CultureInfo.InvariantCulture )}",
                    label: current.ToString( "HH:mm", CultureInfo.InvariantCulture ),
                    start: current,
                    end: next ) );

                current = next;
                index++;
            }
        }

        return slots;
    }

    private double GetTreePaneWidth()
    {
        var width = 0d;

        if ( showTitleColumn )
            width += TitleColumnWidth;

        if ( showStartColumn )
            width += DateColumnWidth;

        if ( showEndColumn )
            width += DateColumnWidth;

        if ( ShowNewColumn )
            width += ActionColumnWidth;

        return Math.Max( 1d, width );
    }

    private double GetRowHeight()
    {
        var view = ActiveView;

        if ( view is not null && view.RowHeight > 0d )
            return view.RowHeight;

        return DefaultRowHeight;
    }

    private double GetTimelineCellWidth()
    {
        var view = ActiveView;

        if ( view is not null && view.TimelineCellWidth > 0d )
            return view.TimelineCellWidth;

        return DefaultTimelineCellWidth;
    }

    private void RemoveItemAndChildren( ICollection<TItem> data, TItem itemToDelete )
    {
        if ( data is null || itemToDelete is null )
            return;

        if ( !propertyMapper.HasId )
        {
            data.Remove( itemToDelete );
            return;
        }

        var dataSnapshot = data.ToList();
        var itemId = NormalizeIdentifier( propertyMapper.GetId( itemToDelete ) );

        if ( string.IsNullOrEmpty( itemId ) )
        {
            data.Remove( itemToDelete );
            return;
        }

        var idsToDelete = new HashSet<string>( StringComparer.Ordinal ) { itemId };
        var changed = true;

        while ( changed )
        {
            changed = false;

            foreach ( var dataItem in dataSnapshot )
            {
                var dataItemId = NormalizeIdentifier( propertyMapper.GetId( dataItem ) );
                var dataItemParentId = propertyMapper.HasParentId
                    ? NormalizeIdentifier( propertyMapper.GetParentId( dataItem ) )
                    : null;

                if ( string.IsNullOrEmpty( dataItemId ) || string.IsNullOrEmpty( dataItemParentId ) )
                    continue;

                if ( idsToDelete.Contains( dataItemParentId ) && idsToDelete.Add( dataItemId ) )
                {
                    changed = true;
                }
            }
        }

        foreach ( var dataItem in dataSnapshot )
        {
            var dataItemId = NormalizeIdentifier( propertyMapper.GetId( dataItem ) );

            if ( !string.IsNullOrEmpty( dataItemId ) && idsToDelete.Contains( dataItemId ) )
            {
                data.Remove( dataItem );
            }
        }
    }

    private static bool TryConvertToDouble( object value, out double parsedValue )
    {
        switch ( value )
        {
            case byte byteValue:
                parsedValue = byteValue;
                return true;
            case short shortValue:
                parsedValue = shortValue;
                return true;
            case int intValue:
                parsedValue = intValue;
                return true;
            case long longValue:
                parsedValue = longValue;
                return true;
            case float floatValue:
                parsedValue = floatValue;
                return true;
            case double doubleValue:
                parsedValue = doubleValue;
                return true;
            case decimal decimalValue:
                parsedValue = (double)decimalValue;
                return true;
            case string stringValue when double.TryParse( stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedValue ):
                return true;
            case IConvertible convertible:
                try
                {
                    parsedValue = convertible.ToDouble( CultureInfo.InvariantCulture );
                    return true;
                }
                catch
                {
                    break;
                }
        }

        parsedValue = 0d;
        return false;
    }

    private static string NormalizeIdentifier( object value )
    {
        if ( value is null )
            return null;

        return Convert.ToString( value, CultureInfo.InvariantCulture );
    }

    #endregion

    #region Properties

    private bool IsSearchMode
        => !string.IsNullOrWhiteSpace( searchText );

    private bool ShowNewColumn
        => Editable && UseInternalEditing;

    private string TaskColumnHeaderText
        => Localizer.Localize( Localizers?.TaskLocalizer, LocalizationConstants.Task );

    private string AddTaskText
        => Localizer.Localize( Localizers?.AddTaskLocalizer, LocalizationConstants.AddTask );

    private string AddChildText
        => Localizer.Localize( Localizers?.AddChildLocalizer, LocalizationConstants.AddChild );

    private string NoTasksToDisplayText
        => Localizer.Localize( Localizers?.NoTasksToDisplayLocalizer, LocalizationConstants.NoTasksToDisplay );

    private string StartColumnHeaderText
        => Localizer.Localize( Localizers?.StartLocalizer, LocalizationConstants.Start );

    private string EndColumnHeaderText
        => Localizer.Localize( Localizers?.EndLocalizer, LocalizationConstants.End );

    private BaseGanttView<TItem> ActiveView
        => SelectedView switch
        {
            GanttView.Week => ganttWeekView,
            GanttView.Month => ganttMonthView,
            GanttView.Year => ganttYearView,
            _ => ganttDayView,
        };

    /// <summary>
    /// Gets property mapper used to map model fields to Gantt item values.
    /// </summary>
    protected internal GanttPropertyMapper<TItem> PropertyMapper
        => propertyMapper;

    /// <summary>
    /// Gets whether a new item can be inserted into <see cref="Data"/>.
    /// </summary>
    protected bool CanInsertNewItem
        => Editable && UseInternalEditing && Data is ICollection<TItem>;

    /// <summary>
    /// Gets whether in-memory edit operations can update <see cref="Data"/>.
    /// </summary>
    protected bool CanEditData
        => Editable && UseInternalEditing && Data is ICollection<TItem>;

    /// <summary>
    /// Gets row template for currently selected view.
    /// </summary>
    protected RenderFragment<GanttRowContext<TItem>> RowTemplateForCurrentView
        => ActiveView?.RowTemplate;

    /// <summary>
    /// Gets item-bar template for currently selected view.
    /// </summary>
    protected RenderFragment<GanttItemContext<TItem>> ItemTemplateForCurrentView
        => ActiveView?.ItemTemplate;

    /// <inheritdoc />
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets text localizer used by this component.
    /// </summary>
    [Inject] protected ITextLocalizer<Gantt<TItem>> Localizer { get; set; }

    /// <summary>
    /// Gets or sets data source displayed by the Gantt.
    /// </summary>
    [Parameter] public IEnumerable<TItem> Data { get; set; }

    /// <summary>
    /// Gets or sets current anchor date used by selected view.
    /// </summary>
    [Parameter] public DateOnly Date { get; set; } = DateOnly.FromDateTime( DateTime.Today );

    /// <summary>
    /// Gets or sets callback raised when <see cref="Date"/> changes.
    /// </summary>
    [Parameter] public EventCallback<DateOnly> DateChanged { get; set; }

    /// <summary>
    /// Gets or sets currently selected view mode.
    /// </summary>
    [Parameter] public GanttView SelectedView { get; set; } = GanttView.Week;

    /// <summary>
    /// Gets or sets callback raised when <see cref="SelectedView"/> changes.
    /// </summary>
    [Parameter] public EventCallback<GanttView> SelectedViewChanged { get; set; }

    /// <summary>
    /// Gets or sets first day of week used by weekly calculations.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Monday;

    /// <summary>
    /// Gets or sets whether toolbar is rendered.
    /// </summary>
    [Parameter] public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// Gets or sets search text used to filter tree rows.
    /// </summary>
    [Parameter] public string SearchText { get; set; }

    /// <summary>
    /// Gets or sets callback raised when <see cref="SearchText"/> changes.
    /// </summary>
    [Parameter] public EventCallback<string> SearchTextChanged { get; set; }

    /// <summary>
    /// Gets or sets whether title column is visible.
    /// </summary>
    [Parameter] public bool ShowTitleColumn { get; set; } = true;

    /// <summary>
    /// Gets or sets callback raised when <see cref="ShowTitleColumn"/> changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowTitleColumnChanged { get; set; }

    /// <summary>
    /// Gets or sets whether start date column is visible.
    /// </summary>
    [Parameter] public bool ShowStartColumn { get; set; } = true;

    /// <summary>
    /// Gets or sets callback raised when <see cref="ShowStartColumn"/> changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowStartColumnChanged { get; set; }

    /// <summary>
    /// Gets or sets whether end date column is visible.
    /// </summary>
    [Parameter] public bool ShowEndColumn { get; set; } = true;

    /// <summary>
    /// Gets or sets callback raised when <see cref="ShowEndColumn"/> changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowEndColumnChanged { get; set; }

    /// <summary>
    /// Gets or sets item identifier field name.
    /// </summary>
    [Parameter] public string IdField { get; set; } = "Id";

    /// <summary>
    /// Gets or sets parent identifier field name.
    /// </summary>
    [Parameter] public string ParentIdField { get; set; } = "ParentId";

    /// <summary>
    /// Gets or sets title field name.
    /// </summary>
    [Parameter] public string TitleField { get; set; } = "Title";

    /// <summary>
    /// Gets or sets description field name.
    /// </summary>
    [Parameter] public string DescriptionField { get; set; } = "Description";

    /// <summary>
    /// Gets or sets start date field name.
    /// </summary>
    [Parameter] public string StartField { get; set; } = "Start";

    /// <summary>
    /// Gets or sets end date field name.
    /// </summary>
    [Parameter] public string EndField { get; set; } = "End";

    /// <summary>
    /// Gets or sets duration (days) field name.
    /// </summary>
    [Parameter] public string DurationField { get; set; } = "Duration";

    /// <summary>
    /// Gets or sets progress field name.
    /// </summary>
    [Parameter] public string ProgressField { get; set; } = "Progress";

    /// <summary>
    /// Gets or sets whether item editing is enabled.
    /// </summary>
    [Parameter] public bool Editable { get; set; }

    /// <summary>
    /// Gets or sets whether built-in modal editing is used.
    /// </summary>
    [Parameter] public bool UseInternalEditing { get; set; } = true;

    /// <summary>
    /// Gets or sets custom factory used to create new item instances.
    /// </summary>
    [Parameter] public Func<TItem> NewItemCreator { get; set; }

    /// <summary>
    /// Gets or sets custom factory used to create new item identifiers.
    /// </summary>
    [Parameter] public Func<object> NewIdCreator { get; set; }

    /// <summary>
    /// Gets or sets callback invoked before an item is inserted.
    /// </summary>
    [Parameter] public EventCallback<GanttCancellableItemChange<TItem>> ItemInserting { get; set; }

    /// <summary>
    /// Gets or sets callback invoked before an item is updated.
    /// </summary>
    [Parameter] public EventCallback<GanttCancellableItemChange<TItem>> ItemUpdating { get; set; }

    /// <summary>
    /// Gets or sets callback invoked before an item is removed.
    /// </summary>
    [Parameter] public EventCallback<GanttCancellableItemChange<TItem>> ItemRemoving { get; set; }

    /// <summary>
    /// Gets or sets callback invoked after item insertion.
    /// </summary>
    [Parameter] public EventCallback<GanttInsertedItem<TItem>> ItemInserted { get; set; }

    /// <summary>
    /// Gets or sets callback invoked after item update.
    /// </summary>
    [Parameter] public EventCallback<GanttUpdatedItem<TItem>> ItemUpdated { get; set; }

    /// <summary>
    /// Gets or sets callback invoked after item removal.
    /// </summary>
    [Parameter] public EventCallback<GanttUpdatedItem<TItem>> ItemRemoved { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when a timeline item is clicked.
    /// </summary>
    [Parameter] public EventCallback<GanttItemClickedEventArgs<TItem>> ItemClicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when edit action is triggered for item.
    /// </summary>
    [Parameter] public EventCallback<GanttItemClickedEventArgs<TItem>> EditItemClicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when delete action is triggered for item.
    /// </summary>
    [Parameter] public EventCallback<GanttItemClickedEventArgs<TItem>> DeleteItemClicked { get; set; }

    /// <summary>
    /// Gets or sets callback used for API-style data reads.
    /// </summary>
    [Parameter] public EventCallback<GanttReadDataEventArgs<TItem>> ReadData { get; set; }

    /// <summary>
    /// Gets or sets custom localizers for Gantt texts.
    /// </summary>
    [Parameter] public GanttLocalizers Localizers { get; set; }

    /// <summary>
    /// Gets or sets item styling callback for timeline bars.
    /// </summary>
    [Parameter] public Action<TItem, GanttItemStyling> ItemStyling { get; set; }

    /// <summary>
    /// Gets or sets title column width in pixels.
    /// </summary>
    [Parameter] public double TitleColumnWidth { get; set; } = 320d;

    /// <summary>
    /// Gets or sets date column width in pixels.
    /// </summary>
    [Parameter] public double DateColumnWidth { get; set; } = 140d;

    /// <summary>
    /// Gets or sets action column width in pixels.
    /// </summary>
    [Parameter] public double ActionColumnWidth { get; set; } = 42d;

    /// <summary>
    /// Gets or sets tree indentation size per level in pixels.
    /// </summary>
    [Parameter] public double TreeIndentSize { get; set; } = 18d;

    /// <summary>
    /// Gets or sets minimum item bar width in pixels.
    /// </summary>
    [Parameter] public double MinBarWidth { get; set; } = 14d;

    /// <summary>
    /// Gets or sets child content used to declare toolbar and views.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion

    #region Data structures

    private sealed class GanttTreeNode
    {
        public GanttTreeNode( string key, TItem item )
        {
            Key = key;
            Item = item;
        }

        public string Key { get; }

        public TItem Item { get; }

        public int Level { get; set; }

        public List<GanttTreeNode> Children { get; } = new();
    }

    private sealed class GanttTreeRow
    {
        public GanttTreeRow( string key, TItem item, int level, bool hasChildren )
        {
            Key = key;
            Item = item;
            Level = level;
            HasChildren = hasChildren;
        }

        public string Key { get; }

        public TItem Item { get; }

        public int Level { get; }

        public bool HasChildren { get; }
    }

    private readonly struct GanttViewRange
    {
        public GanttViewRange( DateTime start, DateTime end )
        {
            Start = start;
            End = end;
        }

        public DateTime Start { get; }

        public DateTime End { get; }
    }

    private readonly struct GanttTimeSlot
    {
        public GanttTimeSlot( string key, string label, DateTime start, DateTime end )
        {
            Key = key;
            Label = label;
            Start = start;
            End = end;
        }

        public string Key { get; }

        public string Label { get; }

        public DateTime Start { get; }

        public DateTime End { get; }
    }

    private readonly struct GanttBarInfo
    {
        public GanttBarInfo( double left, double width, double? progressPercentage )
        {
            Left = left;
            Width = width;
            ProgressPercentage = progressPercentage;
        }

        public double Left { get; }

        public double Width { get; }

        public double? ProgressPercentage { get; }
    }

    #endregion
}