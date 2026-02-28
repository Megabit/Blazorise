#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.DeepCloner;
using Blazorise.Extensions;
using Blazorise.Gantt.Components;
using Blazorise.Gantt.Extensions;
using Blazorise.Gantt.Utilities;
using Blazorise.Localization;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Represents a native Blazorise Gantt chart component with tree and timeline views.
/// </summary>
/// <typeparam name="TItem">The item type rendered by the Gantt chart.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public partial class Gantt<TItem> : BaseComponent, IDisposable, IAsyncDisposable
{
    #region Members

    private const double DefaultRowHeight = 44d;
    private const double DefaultHeaderRowHeight = 44d;
    private const double DefaultTimelineCellWidth = 72d;
    private const double DefaultSearchInputWidth = 220d;
    private const double DefaultTreeToggleWidth = 28d;
    private const double DragStartThreshold = 3d;
    private const double MinAutoSizedTreeColumnWidth = 64d;
    private const double AutoSizedTreeColumnCharacterWidth = 8d;
    private const double AutoSizedTreeColumnHorizontalPadding = 24d;
    private const double AutoSizedTreeSortableIndicatorWidth = 20d;

    private readonly HashSet<string> collapsedNodeKeys = new( StringComparer.Ordinal );

    private GanttToolbar<TItem> ganttToolbar;
    private GanttDayView<TItem> ganttDayView;
    private GanttWeekView<TItem> ganttWeekView;
    private GanttMonthView<TItem> ganttMonthView;
    private GanttYearView<TItem> ganttYearView;
    private _GanttTreeRows ganttTreeRowsRef;
    private Div ganttDivRef;

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
    private bool showWbsColumn = false;
    private bool showStartColumn = true;
    private bool showEndColumn = false;
    private bool showDurationColumn = true;
    private bool showCommandColumn = true;
    private string focusedRowKey;
    private bool shouldFocusTreeRows;
    private GanttSortColumn ganttSortColumn = GanttSortColumn.Start;
    private SortDirection ganttSortDirection = SortDirection.Default;
    private bool readDataInitialized;
    private bool readDataRequested;
    private bool barDragPending;
    private bool barDragging;
    private bool suppressBarClick;
    private string barDragRowKey;
    private TItem barDragItem;
    private double barDragCellWidth;
    private double barDragStartClientX;
    private int barDragSlotOffset;

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

        if ( parameters.TryGetValue<bool>( nameof( ShowWbsColumn ), out var paramShowWbsColumn ) )
            showWbsColumn = paramShowWbsColumn;

        if ( parameters.TryGetValue<bool>( nameof( ShowStartColumn ), out var paramShowStartColumn ) )
            showStartColumn = paramShowStartColumn;

        if ( parameters.TryGetValue<bool>( nameof( ShowEndColumn ), out var paramShowEndColumn ) )
            showEndColumn = paramShowEndColumn;

        if ( parameters.TryGetValue<bool>( nameof( ShowDurationColumn ), out var paramShowDurationColumn ) )
            showDurationColumn = paramShowDurationColumn;

        if ( parameters.TryGetValue<bool>( nameof( ShowCommandColumn ), out var paramShowCommandColumn ) )
            showCommandColumn = paramShowCommandColumn;

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
        if ( JSModule is null )
        {
            DotNetObjectRef ??= DotNetObjectReference.Create( this );
            JSModule ??= new JSGanttModule( JSRuntime, VersionProvider, BlazoriseOptions, () => ganttDivRef.ElementRef, () => ElementId );
        }

        currentDate = Date;
        searchText = SearchText ?? string.Empty;
        showTitleColumn = ShowTitleColumn;
        showWbsColumn = ShowWbsColumn;
        showStartColumn = ShowStartColumn;
        showEndColumn = ShowEndColumn;
        showDurationColumn = ShowDurationColumn;
        showCommandColumn = ShowCommandColumn;

        EnsureAtLeastOneColumnVisible();

        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender && JSModule is not null )
            await JSModule.Initialize( DotNetObjectRef );

        if ( readDataRequested && ReadData.HasDelegate )
        {
            readDataRequested = false;

            await HandleReadData( CancellationToken.None );
            await InvokeAsync( StateHasChanged );
        }

        if ( shouldFocusTreeRows && KeyboardNavigation && ganttTreeRowsRef is not null )
        {
            shouldFocusTreeRows = false;
            await ganttTreeRowsRef.FocusAsync();
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

    /// <inheritdoc />
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            if ( Rendered && JSModule is not null )
            {
                await JSModule.SafeDestroy( ganttDivRef.ElementRef, ElementId );
                await JSModule.SafeDisposeAsync();
                JSModule = null;
            }

            if ( DotNetObjectRef is not null )
            {
                DotNetObjectRef.Dispose();
                DotNetObjectRef = null;
            }
        }

        await base.DisposeAsync( disposing );
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
    /// Expands all nodes in the tree, including all nested child nodes.
    /// </summary>
    public Task ExpandAll()
    {
        if ( collapsedNodeKeys.Count == 0 )
            return Task.CompletedTask;

        collapsedNodeKeys.Clear();

        return InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Collapses all nodes in the tree, including all nested child nodes.
    /// </summary>
    public Task CollapseAll()
    {
        collapsedNodeKeys.Clear();

        foreach ( var root in BuildTree() )
        {
            CollapseNodeAndChildren( root );
        }

        return InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Determines whether specified edit command is currently allowed.
    /// </summary>
    /// <param name="commandType">Command to evaluate.</param>
    /// <param name="item">Target item for edit/delete operations.</param>
    /// <param name="parentItem">Parent item for add-child operations.</param>
    /// <returns>True when command is allowed; otherwise false.</returns>
    protected internal bool IsCommandAllowed( GanttCommandType commandType, TItem item = default, TItem parentItem = default )
    {
        if ( !Editable )
            return false;

        if ( commandType == GanttCommandType.New && !NewCommandAllowed )
            return false;

        if ( commandType == GanttCommandType.AddChild && !AddChildCommandAllowed )
            return false;

        if ( commandType == GanttCommandType.Edit && !EditCommandAllowed )
            return false;

        if ( commandType == GanttCommandType.Delete && !DeleteCommandAllowed )
            return false;

        if ( CommandAllowed is not null )
        {
            var commandContext = new GanttCommandContext<TItem>( commandType, item, parentItem );

            if ( !CommandAllowed.Invoke( commandContext ) )
                return false;
        }

        return true;
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

        var commandType = parentItem is null
            ? GanttCommandType.New
            : GanttCommandType.AddChild;

        if ( !IsCommandAllowed( commandType, item, parentItem ) )
            return;

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

        if ( !IsCommandAllowed( GanttCommandType.Edit, item ) )
            return;

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

        if ( !IsCommandAllowed( GanttCommandType.Delete, item ) )
            return;

        if ( Editable && UseInternalEditing )
        {
            if ( await ConfirmDelete() == false )
                return;

            await DeleteItemImpl( item );
        }
    }

    private Task<bool> ConfirmDelete()
    {
        if ( MessageService is null )
            return Task.FromResult( true );

        return MessageService.Confirm(
            Localizer.Localize( Localizers?.DeleteTaskConfirmationLocalizer, LocalizationConstants.DeleteTaskConfirmation ),
            Localizer.Localize( Localizers?.DeleteLocalizer, LocalizationConstants.Delete ),
            options =>
            {
                options.ShowCloseButton = false;
                options.ShowMessageIcon = false;
                options.CancelButtonText = Localizer.Localize( Localizers?.CancelLocalizer, LocalizationConstants.Cancel );
                options.ConfirmButtonText = Localizer.Localize( Localizers?.DeleteLocalizer, LocalizationConstants.Delete );
                options.ConfirmButtonColor = Color.Danger;
            } );
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

        if ( !IsCommandAllowed( GanttCommandType.Delete, itemToDelete ) )
            return false;

        if ( DeleteItemClicked.HasDelegate )
        {
            await DeleteItemClicked.InvokeAsync( new GanttItemClickedEventArgs<TItem>( itemToDelete ) );
        }

        if ( !await IsSafeToProceed( ItemRemoving, itemToDelete, itemToDelete ) )
            return false;

        var selectedRowDeleted = IsSelectedRowDeleted( itemToDelete );

        if ( CanEditData && Data is ICollection<TItem> data )
        {
            RemoveItemAndChildren( data, itemToDelete );
        }

        await ItemRemoved.InvokeAsync( new GanttUpdatedItem<TItem>( itemToDelete, itemToDelete ) );

        editState = GanttEditState.None;
        editItem = default;
        editParentItem = default;

        if ( selectedRowDeleted )
        {
            SelectedRow = default;
            await SelectedRowChanged.InvokeAsync( SelectedRow );
        }

        await RefreshInternalAsync();

        return true;
    }

    internal async Task<bool> CommitEditInternalAsync( TItem targetItem, TItem submittedItem, GanttEditState submittedEditState )
    {
        if ( targetItem is null || submittedItem is null )
            return false;

        if ( submittedEditState == GanttEditState.New )
        {
            var commandType = editParentItem is null
                ? GanttCommandType.New
                : GanttCommandType.AddChild;

            if ( !IsCommandAllowed( commandType, targetItem, editParentItem ) )
                return false;
        }
        else if ( submittedEditState == GanttEditState.Edit && !IsCommandAllowed( GanttCommandType.Edit, targetItem ) )
        {
            return false;
        }

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
        var hasActiveSort = Sortable && ganttSortDirection != SortDirection.Default;
        GanttSortColumn? sortColumn = hasActiveSort ? ganttSortColumn : null;
        var sortDirection = hasActiveSort ? ganttSortDirection : SortDirection.Default;

        try
        {
            await ReadData.InvokeAsync( new GanttReadDataEventArgs<TItem>(
                SelectedView,
                currentDate,
                viewRange.Start,
                viewRange.End,
                searchText,
                sortColumn,
                sortDirection,
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

    private async Task SetShowWbsColumn( bool value )
    {
        showWbsColumn = value;
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

    private async Task SetShowDurationColumn( bool value )
    {
        showDurationColumn = value;
        EnsureAtLeastOneColumnVisible();

        await NotifyColumnVisibilityChanged();
        await InvokeAsync( StateHasChanged );
    }

    private async Task SetShowCommandColumn( bool value )
    {
        showCommandColumn = value;

        await NotifyColumnVisibilityChanged();
        await InvokeAsync( StateHasChanged );
    }

    private async Task NotifyColumnVisibilityChanged()
    {
        await ShowTitleColumnChanged.InvokeAsync( showTitleColumn );
        await ShowWbsColumnChanged.InvokeAsync( showWbsColumn );
        await ShowStartColumnChanged.InvokeAsync( showStartColumn );
        await ShowEndColumnChanged.InvokeAsync( showEndColumn );
        await ShowDurationColumnChanged.InvokeAsync( showDurationColumn );
        await ShowCommandColumnChanged.InvokeAsync( showCommandColumn );
    }

    private void EnsureAtLeastOneColumnVisible()
    {
        if ( !showTitleColumn && !showWbsColumn && !showStartColumn && !showEndColumn && !showDurationColumn )
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

    private int? GetItemDuration( TItem item )
    {
        if ( item is null )
            return null;

        if ( propertyMapper.HasDuration )
        {
            var duration = propertyMapper.GetDuration( item );

            if ( duration > 0 )
                return duration;
        }

        var start = GetItemStart( item );
        var end = GetItemEnd( item );

        if ( IsUnassignedDate( start ) || IsUnassignedDate( end ) )
            return null;

        return GetDurationInDays( start, end );
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

    private static string FormatDuration( int? duration )
    {
        if ( duration is null )
            return string.Empty;

        return duration.Value.ToString( CultureInfo.InvariantCulture );
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
        if ( suppressBarClick )
        {
            suppressBarClick = false;
            return;
        }

        if ( ItemClicked.HasDelegate )
        {
            await ItemClicked.InvokeAsync( new GanttItemClickedEventArgs<TItem>( item ) );
        }

        if ( IsCommandAllowed( GanttCommandType.Edit, item ) )
        {
            await Edit( item );
        }
    }

    private async Task OnBarMouseDown( MouseEventArgs eventArgs, GanttTreeRow row, double cellWidth )
    {
        if ( eventArgs is null || row is null || eventArgs.Button != 0 || !CanDragItem( row.Item ) || cellWidth <= 0d )
            return;

        if ( barDragPending )
            await FinalizeBarDrag( false );

        barDragPending = true;
        barDragging = false;
        barDragItem = row.Item;
        barDragRowKey = row.Key;
        barDragCellWidth = cellWidth;
        barDragStartClientX = eventArgs.ClientX;
        barDragSlotOffset = 0;

        if ( JSModule is not null )
            await JSModule.BarDragStarted( eventArgs.ClientX );
    }

    [JSInvokable]
    public Task NotifyBarDragMouseMove( double clientX )
    {
        if ( !TryUpdateBarDragFromClientX( clientX ) )
            return Task.CompletedTask;

        return InvokeAsync( StateHasChanged );
    }

    [JSInvokable]
    public Task NotifyBarDragMouseUp( double clientX )
    {
        TryUpdateBarDragFromClientX( clientX );

        return FinalizeBarDrag();
    }

    private async Task FinalizeBarDrag( bool commitChanges = true )
    {
        if ( !barDragPending )
            return;

        if ( JSModule is not null )
            await JSModule.BarDragEnded();

        var dragged = barDragging;
        var slotOffset = barDragSlotOffset;
        var item = barDragItem;

        ResetBarDragState();

        if ( dragged && commitChanges )
            suppressBarClick = true;

        if ( !commitChanges || !dragged || slotOffset == 0 || item is null )
        {
            await InvokeAsync( StateHasChanged );
            return;
        }

        if ( !CanDragItem( item ) )
            return;

        await MoveItemBySlotOffset( item, slotOffset );
    }

    private void ResetBarDragState()
    {
        barDragPending = false;
        barDragging = false;
        barDragItem = default;
        barDragRowKey = null;
        barDragCellWidth = 0d;
        barDragStartClientX = 0d;
        barDragSlotOffset = 0;
    }

    private bool TryUpdateBarDragFromClientX( double clientX )
    {
        if ( !barDragPending )
            return false;

        var deltaX = clientX - barDragStartClientX;

        if ( !barDragging && Math.Abs( deltaX ) < DragStartThreshold )
            return false;

        barDragging = true;

        var nextSlotOffset = GetSlotOffsetFromDeltaX( deltaX, barDragCellWidth );

        if ( nextSlotOffset == barDragSlotOffset )
            return false;

        barDragSlotOffset = nextSlotOffset;

        return true;
    }

    private bool CanDragItem( TItem item )
    {
        if ( item is null )
            return false;

        if ( !IsCommandAllowed( GanttCommandType.Edit, item ) )
            return false;

        if ( !propertyMapper.HasStart || !propertyMapper.HasEnd )
            return false;

        var itemStart = GetItemStart( item );
        var itemEnd = GetItemEnd( item );

        if ( IsUnassignedDate( itemStart ) || IsUnassignedDate( itemEnd ) )
            return false;

        return itemEnd > itemStart;
    }

    private static int GetSlotOffsetFromDeltaX( double deltaX, double cellWidth )
    {
        if ( cellWidth <= 0d )
            return 0;

        return (int)Math.Round( deltaX / cellWidth, MidpointRounding.AwayFromZero );
    }

    private DateTime ShiftDateBySlotOffset( DateTime value, int slotOffset )
    {
        if ( slotOffset == 0 )
            return value;

        return SelectedView switch
        {
            GanttView.Day => value.AddHours( slotOffset ),
            GanttView.Year => value.AddMonths( slotOffset ),
            _ => value.AddDays( slotOffset ),
        };
    }

    private async Task MoveItemBySlotOffset( TItem item, int slotOffset )
    {
        if ( item is null || slotOffset == 0 || !CanDragItem( item ) )
            return;

        var sourceStart = GetItemStart( item );
        var sourceEnd = GetItemEnd( item );

        if ( IsUnassignedDate( sourceStart ) || IsUnassignedDate( sourceEnd ) )
            return;

        var movedStart = ShiftDateBySlotOffset( sourceStart, slotOffset );
        var movedEnd = ShiftDateBySlotOffset( sourceEnd, slotOffset );

        if ( movedEnd <= movedStart )
            return;

        var movedItem = item.DeepClone();

        if ( propertyMapper.HasStart )
            propertyMapper.SetStart( movedItem, movedStart );

        if ( propertyMapper.HasEnd )
            propertyMapper.SetEnd( movedItem, movedEnd );

        if ( propertyMapper.HasDuration )
            propertyMapper.SetDuration( movedItem, GetDurationInDays( movedStart, movedEnd ) );

        editItem = item;
        editParentItem = default;
        editState = GanttEditState.Edit;

        await SaveImpl( movedItem );
    }

    private Task OnItemModalClosed()
    {
        if ( !KeyboardNavigation )
            return Task.CompletedTask;

        shouldFocusTreeRows = true;
        return InvokeAsync( StateHasChanged );
    }

    private async Task OnTreeRowDoubleClicked( TItem item, string rowKey )
    {
        await OnTreeRowClicked( item, rowKey );

        if ( IsCommandAllowed( GanttCommandType.Edit, item ) )
            await Edit( item );
    }

    private async Task OnTreeRowClicked( TItem item, string rowKey )
    {
        await FocusRow( rowKey );
        await SelectRow( item );
    }

    private Task OnTreeRowsKeyDown( KeyboardEventArgs eventArgs )
    {
        return HandleTreeRowsKeyDown( eventArgs );
    }

    private async Task HandleTreeRowsKeyDown( KeyboardEventArgs eventArgs )
    {
        if ( !KeyboardNavigation || eventArgs is null )
            return;

        var visibleRows = GetVisibleRows();

        if ( visibleRows.Count == 0 )
            return;

        EnsureFocusedRow( visibleRows );

        var focusedRow = GetFocusedRow( visibleRows );

        if ( focusedRow is null )
            return;

        switch ( eventArgs.Key )
        {
            case "ArrowDown":
                MoveFocusedRow( visibleRows, 1 );
                break;
            case "ArrowUp":
                MoveFocusedRow( visibleRows, -1 );
                break;
            case "ArrowRight":
                if ( focusedRow.HasChildren && !IsExpanded( focusedRow ) )
                    await ToggleNode( focusedRow );
                break;
            case "ArrowLeft":
                if ( focusedRow.HasChildren && IsExpanded( focusedRow ) )
                    await ToggleNode( focusedRow );
                break;
            case "Enter":
            case "F2":
                if ( IsCommandAllowed( GanttCommandType.Edit, focusedRow.Item ) )
                    await Edit( focusedRow.Item );
                break;
            case "Delete":
                if ( IsCommandAllowed( GanttCommandType.Delete, focusedRow.Item ) )
                    await Delete( focusedRow.Item );
                break;
            case " ":
            case "Space":
            case "Spacebar":
                await SelectRow( focusedRow.Item );
                break;
            default:
                break;
        }
    }

    private void EnsureFocusedRow( IReadOnlyList<GanttTreeRow> visibleRows )
    {
        if ( visibleRows is null || visibleRows.Count == 0 )
        {
            focusedRowKey = null;
            return;
        }

        foreach ( var row in visibleRows )
        {
            if ( string.Equals( row.Key, focusedRowKey, StringComparison.Ordinal ) )
                return;
        }

        focusedRowKey = visibleRows[0].Key;
    }

    private GanttTreeRow GetFocusedRow( IReadOnlyList<GanttTreeRow> visibleRows )
    {
        if ( visibleRows is null || visibleRows.Count == 0 )
            return null;

        foreach ( var row in visibleRows )
        {
            if ( string.Equals( row.Key, focusedRowKey, StringComparison.Ordinal ) )
                return row;
        }

        return visibleRows[0];
    }

    private void MoveFocusedRow( IReadOnlyList<GanttTreeRow> visibleRows, int offset )
    {
        if ( visibleRows is null || visibleRows.Count == 0 )
        {
            focusedRowKey = null;
            return;
        }

        var currentIndex = 0;

        for ( int i = 0; i < visibleRows.Count; i++ )
        {
            if ( string.Equals( visibleRows[i].Key, focusedRowKey, StringComparison.Ordinal ) )
            {
                currentIndex = i;
                break;
            }
        }

        var targetIndex = Math.Max( 0, Math.Min( visibleRows.Count - 1, currentIndex + offset ) );
        focusedRowKey = visibleRows[targetIndex].Key;
    }

    private Task FocusRow( string rowKey )
    {
        focusedRowKey = rowKey;
        return Task.CompletedTask;
    }

    private Task SelectRow( TItem item )
    {
        if ( AreRowsEqual( SelectedRow, item ) )
            return Task.CompletedTask;

        SelectedRow = item;

        return SelectedRowChanged.InvokeAsync( SelectedRow );
    }

    private bool IsFocusedRow( string rowKey )
        => string.Equals( focusedRowKey, rowKey, StringComparison.Ordinal );

    private bool IsSelectedRow( TItem item )
        => AreRowsEqual( SelectedRow, item );

    private Background GetTreeRowBackground( TItem item, string rowKey )
    {
        var isFocused = KeyboardNavigation && IsFocusedRow( rowKey );
        var isSelected = IsSelectedRow( item );

        if ( isFocused && isSelected )
            return Blazorise.Background.Primary.Subtle;

        if ( isSelected )
            return Blazorise.Background.Info.Subtle;

        if ( isFocused )
            return Blazorise.Background.Light;

        return Blazorise.Background.Default;
    }

    private Background GetFocusedRowBackground( string rowKey )
        => KeyboardNavigation && IsFocusedRow( rowKey )
            ? Blazorise.Background.Light
            : Blazorise.Background.Default;

    private Task SortByColumn( GanttSortColumn column )
    {
        if ( !Sortable )
            return Task.CompletedTask;

        if ( ganttSortColumn == column )
        {
            ganttSortDirection = GetNextSortDirection( ganttSortDirection );
        }
        else
        {
            ganttSortColumn = column;
            ganttSortDirection = SortDirection.Ascending;
        }

        return InvokeAsync( StateHasChanged );
    }

    private bool ShowSortIcon( GanttSortColumn column )
        => Sortable
           && ganttSortColumn == column
           && ganttSortDirection != SortDirection.Default;

    private IconName GetSortIconName( GanttSortColumn column )
        => !ShowSortIcon( column ) || ganttSortDirection == SortDirection.Ascending
            ? IconName.SortUp
            : IconName.SortDown;

    private GanttTreeHeaderCellContext<TItem> GetTreeHeaderCellContext( GanttTreeColumn column, string text, GanttSortColumn? sortColumn = null )
    {
        var sortable = Sortable && sortColumn.HasValue;
        var showSortIcon = sortColumn.HasValue && ShowSortIcon( sortColumn.Value );
        var sortDirection = sortColumn.HasValue && ganttSortColumn == sortColumn.Value
            ? ganttSortDirection
            : SortDirection.Default;

        return new GanttTreeHeaderCellContext<TItem>( this, column, text, sortable, showSortIcon, sortDirection );
    }

    private GanttTreeCellContext<TItem> GetTreeCellContext( GanttTreeRow row, GanttTreeColumn column, string text, double treeToggleWidth )
    {
        Func<Task> toggleNode = row.HasChildren
            ? () => ToggleNode( row )
            : NoopAsync;

        var selected = IsSelectedRow( row.Item );
        var focused = string.Equals( focusedRowKey, row.Key, StringComparison.Ordinal );

        return new GanttTreeCellContext<TItem>(
            this,
            row.Item,
            row.Key,
            column,
            text,
            row.Level,
            row.HasChildren,
            IsExpanded( row ),
            selected,
            focused,
            toggleNode,
            treeToggleWidth );
    }

    private GanttTreeCommandHeaderContext<TItem> GetTreeCommandHeaderContext( bool showHeaderNewButton )
    {
        Func<Task> addTask = showHeaderNewButton
            ? New
            : NoopAsync;

        return new GanttTreeCommandHeaderContext<TItem>( this, showHeaderNewButton, addTask, AddTaskText );
    }

    private GanttTreeCommandCellContext<TItem> GetTreeCommandCellContext( GanttTreeRow row )
    {
        var canAddChild = CanShowAddChildButton( row.Item );
        Func<Task> addChild = canAddChild
            ? () => AddChild( row.Item )
            : NoopAsync;

        var selected = IsSelectedRow( row.Item );
        var focused = string.Equals( focusedRowKey, row.Key, StringComparison.Ordinal );

        return new GanttTreeCommandCellContext<TItem>(
            this,
            row.Item,
            row.Key,
            row.Level,
            row.HasChildren,
            IsExpanded( row ),
            selected,
            focused,
            canAddChild,
            addChild,
            AddChildText );
    }

    private static GanttTimelineHeaderCellContext GetTimelineHeaderCellContext( GanttTimeSlot slot, int index )
        => new( slot.Key, slot.Label, slot.Start, slot.End, index );

    private static Task NoopAsync()
        => Task.CompletedTask;

    private static SortDirection GetNextSortDirection( SortDirection sortDirection )
    {
        return sortDirection switch
        {
            SortDirection.Default => SortDirection.Ascending,
            SortDirection.Ascending => SortDirection.Descending,
            SortDirection.Descending => SortDirection.Default,
            _ => SortDirection.Default,
        };
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

    private Dictionary<string, string> BuildWbsLookup( IReadOnlyList<GanttTreeRow> visibleRows )
    {
        var wbsLookup = new Dictionary<string, string>( StringComparer.Ordinal );

        if ( visibleRows is null || visibleRows.Count == 0 )
            return wbsLookup;

        var levelCounters = new List<int>();

        foreach ( var row in visibleRows )
        {
            var level = Math.Max( 0, row.Level );

            while ( levelCounters.Count <= level )
            {
                levelCounters.Add( 0 );
            }

            levelCounters[level]++;

            if ( level + 1 < levelCounters.Count )
            {
                levelCounters.RemoveRange( level + 1, levelCounters.Count - level - 1 );
            }

            var segments = new string[level + 1];

            for ( int i = 0; i <= level; i++ )
            {
                segments[i] = levelCounters[i].ToString( CultureInfo.InvariantCulture );
            }

            wbsLookup[row.Key] = string.Join( '.', segments );
        }

        return wbsLookup;
    }

    private static string GetWbsValue( IReadOnlyDictionary<string, string> wbsLookup, string rowKey )
    {
        if ( wbsLookup is null || string.IsNullOrEmpty( rowKey ) )
            return string.Empty;

        return wbsLookup.TryGetValue( rowKey, out var wbsValue )
            ? wbsValue
            : string.Empty;
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

    private void CollapseNodeAndChildren( GanttTreeNode node )
    {
        if ( node is null )
            return;

        if ( node.Children.Count > 0 )
        {
            collapsedNodeKeys.Add( node.Key );
        }

        foreach ( var child in node.Children )
        {
            CollapseNodeAndChildren( child );
        }
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
            .OrderBy( x => x, Comparer<GanttTreeNode>.Create( CompareNodes ) )
            .ToList();
    }

    private int CompareNodes( GanttTreeNode x, GanttTreeNode y )
    {
        if ( ReferenceEquals( x, y ) )
            return 0;

        if ( x is null )
            return -1;

        if ( y is null )
            return 1;

        var comparison = Sortable && ganttSortDirection != SortDirection.Default
            ? CompareByCurrentSort( x, y )
            : CompareByDefaultSort( x, y );

        if ( comparison != 0 )
            return comparison;

        return StringComparer.Ordinal.Compare( x.Key, y.Key );
    }

    private int CompareByCurrentSort( GanttTreeNode x, GanttTreeNode y )
    {
        var comparison = ganttSortColumn switch
        {
            GanttSortColumn.Title => StringComparer.OrdinalIgnoreCase.Compare( GetItemTitle( x.Item ), GetItemTitle( y.Item ) ),
            GanttSortColumn.End => DateTime.Compare( GetItemEnd( x.Item ), GetItemEnd( y.Item ) ),
            GanttSortColumn.Duration => Nullable.Compare( GetItemDuration( x.Item ), GetItemDuration( y.Item ) ),
            _ => DateTime.Compare( GetItemStart( x.Item ), GetItemStart( y.Item ) ),
        };

        if ( ganttSortDirection == SortDirection.Descending )
            comparison = -comparison;

        if ( comparison != 0 )
            return comparison;

        return CompareByDefaultSort( x, y );
    }

    private int CompareByDefaultSort( GanttTreeNode x, GanttTreeNode y )
    {
        var startComparison = DateTime.Compare( GetItemStart( x.Item ), GetItemStart( y.Item ) );

        if ( startComparison != 0 )
            return startComparison;

        var titleComparison = StringComparer.OrdinalIgnoreCase.Compare( GetItemTitle( x.Item ), GetItemTitle( y.Item ) );

        if ( titleComparison != 0 )
            return titleComparison;

        return DateTime.Compare( GetItemEnd( x.Item ), GetItemEnd( y.Item ) );
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

    private string GetTreePaneStyle( bool showActionColumn, double wbsColumnWidth, double startColumnWidth, double endColumnWidth, double durationColumnWidth )
    {
        var width = GetTreePaneWidth( showActionColumn, wbsColumnWidth, startColumnWidth, endColumnWidth, durationColumnWidth );
        var widthText = width.ToString( "0.###", CultureInfo.InvariantCulture );

        return $"display: flex; flex-direction: column; width: {widthText}px; min-width: {widthText}px; max-width: {widthText}px; overflow: hidden;";
    }

    private string GetBodyStyle()
    {
        return "min-height: 360px; max-height: 100%; min-width: 0; overflow: hidden;";
    }

    private string GetTreeRowsStyle()
    {
        var headerHeightText = GetHeaderRowHeight().ToString( "0.###", CultureInfo.InvariantCulture );
        return $"height: calc(100% - {headerHeightText}px); max-height: 100%; overflow-x: hidden; overflow-y: hidden; outline: none;";
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

    private string GetTitleColumnStyle( bool sortable = false )
    {
        var width = TitleColumnWidth.ToString( "0.###", CultureInfo.InvariantCulture );
        return $"width: {width}px; min-width: {width}px; max-width: {width}px; overflow: hidden; cursor: {(sortable ? "pointer" : "default")};";
    }

    private string GetDateColumnStyle( double width, bool sortable = false )
    {
        var columnWidth = width > 0d ? width : DateColumnWidth;
        var widthText = columnWidth.ToString( "0.###", CultureInfo.InvariantCulture );
        return $"width: {widthText}px; min-width: {widthText}px; max-width: {widthText}px; overflow: hidden; cursor: {(sortable ? "pointer" : "default")};";
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
        return $"display: flex; align-items: center; justify-content: center; text-align: center; border-right: 1px solid rgba(0,0,0,0.08); width: {width}px; min-width: {width}px; max-width: {width}px;";
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

    private string GetBarStyle( GanttBarInfo barInfo, double rowHeight, string itemCustomStyle, string rowKey, TItem item )
    {
        var barHeight = Math.Max( 16d, rowHeight - 12d );
        var top = Math.Max( 1d, ( rowHeight - barHeight ) / 2d );

        var style = $"position: absolute; left: {barInfo.Left.ToString( "0.###", CultureInfo.InvariantCulture )}px; width: {barInfo.Width.ToString( "0.###", CultureInfo.InvariantCulture )}px; top: {top.ToString( "0.###", CultureInfo.InvariantCulture )}px; height: {barHeight.ToString( "0.###", CultureInfo.InvariantCulture )}px; display: flex; align-items: center; cursor: {( CanDragItem( item ) ? "grab" : "pointer" )};";

        if ( barDragging && string.Equals( barDragRowKey, rowKey, StringComparison.Ordinal ) )
        {
            var translateX = ( barDragSlotOffset * barDragCellWidth ).ToString( "0.###", CultureInfo.InvariantCulture );
            style = $"{style} transform: translateX({translateX}px); z-index: 2; cursor: grabbing;";
        }

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

    private double GetTreePaneWidth( bool showActionColumn, double wbsColumnWidth, double startColumnWidth, double endColumnWidth, double durationColumnWidth )
    {
        var width = 0d;

        if ( showTitleColumn )
            width += TitleColumnWidth;

        if ( showWbsColumn )
            width += wbsColumnWidth;

        if ( showStartColumn )
            width += startColumnWidth;

        if ( showEndColumn )
            width += endColumnWidth;

        if ( showDurationColumn )
            width += durationColumnWidth;

        if ( showActionColumn )
            width += ActionColumnWidth;

        return Math.Max( 1d, width );
    }

    private double GetAutoSizedTreeColumnWidth( IReadOnlyList<GanttTreeRow> visibleRows, string headerText, Func<GanttTreeRow, string> valueSelector, bool sortable = false )
    {
        var maxTextLength = GetTextLength( headerText );

        if ( visibleRows is not null && valueSelector is not null )
        {
            foreach ( var row in visibleRows )
            {
                maxTextLength = Math.Max( maxTextLength, GetTextLength( valueSelector( row ) ) );
            }
        }

        var width = maxTextLength * AutoSizedTreeColumnCharacterWidth + AutoSizedTreeColumnHorizontalPadding;

        if ( sortable )
            width += AutoSizedTreeSortableIndicatorWidth;

        width = Math.Max( MinAutoSizedTreeColumnWidth, width );

        if ( width <= 0d )
            return DateColumnWidth > 0d ? DateColumnWidth : MinAutoSizedTreeColumnWidth;

        return width;
    }

    private static int GetTextLength( string text )
        => string.IsNullOrEmpty( text ) ? 0 : text.Length;

    private double GetRowHeight()
    {
        var view = ActiveView;

        if ( view is not null && view.RowHeight > 0d )
            return view.RowHeight;

        return DefaultRowHeight;
    }

    private double GetHeaderRowHeight()
    {
        if ( HeaderRowHeight > 0d )
            return HeaderRowHeight;

        return DefaultHeaderRowHeight;
    }

    private double GetSearchInputWidth()
    {
        if ( SearchInputWidth > 0d )
            return SearchInputWidth;

        return DefaultSearchInputWidth;
    }

    private double GetTreeToggleWidth()
    {
        if ( TreeToggleWidth > 0d )
            return TreeToggleWidth;

        return DefaultTreeToggleWidth;
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

    private bool AreRowsEqual( TItem first, TItem second )
    {
        if ( ReferenceEquals( first, second ) )
            return true;

        if ( first is null || second is null )
            return false;

        if ( propertyMapper.HasId )
        {
            var firstId = NormalizeIdentifier( propertyMapper.GetId( first ) );
            var secondId = NormalizeIdentifier( propertyMapper.GetId( second ) );

            if ( !string.IsNullOrEmpty( firstId ) || !string.IsNullOrEmpty( secondId ) )
                return string.Equals( firstId, secondId, StringComparison.Ordinal );
        }

        return EqualityComparer<TItem>.Default.Equals( first, second );
    }

    private bool IsSelectedRowDeleted( TItem itemToDelete )
    {
        if ( SelectedRow is null || itemToDelete is null )
            return false;

        if ( AreRowsEqual( SelectedRow, itemToDelete ) )
            return true;

        if ( !propertyMapper.HasId || !propertyMapper.HasParentId )
            return false;

        var selectedRowId = NormalizeIdentifier( propertyMapper.GetId( SelectedRow ) );
        var deleteRowId = NormalizeIdentifier( propertyMapper.GetId( itemToDelete ) );

        if ( string.IsNullOrEmpty( selectedRowId ) || string.IsNullOrEmpty( deleteRowId ) )
            return false;

        if ( string.Equals( selectedRowId, deleteRowId, StringComparison.Ordinal ) )
            return true;

        var parentById = new Dictionary<string, string>( StringComparer.Ordinal );

        foreach ( var dataItem in Data ?? Array.Empty<TItem>() )
        {
            if ( dataItem is null )
                continue;

            var dataId = NormalizeIdentifier( propertyMapper.GetId( dataItem ) );

            if ( string.IsNullOrEmpty( dataId ) || parentById.ContainsKey( dataId ) )
                continue;

            var parentId = NormalizeIdentifier( propertyMapper.GetParentId( dataItem ) );
            parentById.Add( dataId, parentId );
        }

        var currentId = selectedRowId;
        var loopGuard = new HashSet<string>( StringComparer.Ordinal );

        while ( !string.IsNullOrEmpty( currentId ) && loopGuard.Add( currentId ) )
        {
            if ( string.Equals( currentId, deleteRowId, StringComparison.Ordinal ) )
                return true;

            if ( !parentById.TryGetValue( currentId, out currentId ) )
                break;
        }

        return false;
    }

    #endregion

    #region Properties

    private bool IsSearchMode
        => !string.IsNullOrWhiteSpace( searchText );

    private bool ShowToolbarAddTaskButton
        => UseInternalEditing && IsCommandAllowed( GanttCommandType.New );

    private bool ShowHeaderNewButton
        => UseInternalEditing && IsCommandAllowed( GanttCommandType.New );

    private bool ShowAddChildColumn
        => UseInternalEditing && Editable && AddChildCommandAllowed;

    private bool CanShowActionColumn( IReadOnlyCollection<GanttTreeRow> visibleRows )
        => showCommandColumn
           && ( ShowHeaderNewButton
           || ( ShowAddChildColumn && visibleRows.Any( x => IsCommandAllowed( GanttCommandType.AddChild, parentItem: x.Item ) ) ) );

    private bool CanShowAddChildButton( TItem parentItem )
        => ShowAddChildColumn && IsCommandAllowed( GanttCommandType.AddChild, parentItem: parentItem );

    private string TaskColumnHeaderText
        => Localizer.Localize( Localizers?.TaskLocalizer, LocalizationConstants.Task );

    private string WbsColumnHeaderText
        => Localizer.Localize( Localizers?.WbsLocalizer, LocalizationConstants.Wbs );

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

    private string DurationColumnHeaderText
        => Localizer.Localize( Localizers?.DurationLocalizer, LocalizationConstants.Duration );

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

    /// <summary>
    /// Gets current dotnet object reference used by JavaScript module callbacks.
    /// </summary>
    protected DotNetObjectReference<Gantt<TItem>> DotNetObjectRef { get; private set; }

    /// <summary>
    /// Gets Gantt JavaScript module wrapper.
    /// </summary>
    internal protected JSGanttModule JSModule { get; private set; }

    /// <summary>
    /// Gets or sets the <see cref="IJSRuntime"/>.
    /// </summary>
    [Inject] private IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IVersionProvider"/> for the JS module.
    /// </summary>
    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    /// <inheritdoc />
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets text localizer used by this component.
    /// </summary>
    [Inject] protected ITextLocalizer<Gantt<TItem>> Localizer { get; set; }

    /// <summary>
    /// Gets message service used for delete confirmations.
    /// </summary>
    [Inject] protected IMessageService MessageService { get; set; }

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
    /// Gets or sets currently selected tree row item.
    /// </summary>
    [Parameter] public TItem SelectedRow { get; set; }

    /// <summary>
    /// Gets or sets callback raised when <see cref="SelectedRow"/> changes.
    /// </summary>
    [Parameter] public EventCallback<TItem> SelectedRowChanged { get; set; }

    /// <summary>
    /// Gets or sets whether title column is visible.
    /// </summary>
    [Parameter] public bool ShowTitleColumn { get; set; } = true;

    /// <summary>
    /// Gets or sets callback raised when <see cref="ShowTitleColumn"/> changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowTitleColumnChanged { get; set; }

    /// <summary>
    /// Gets or sets whether WBS (work breakdown structure) column is visible.
    /// </summary>
    [Parameter] public bool ShowWbsColumn { get; set; } = false;

    /// <summary>
    /// Gets or sets callback raised when <see cref="ShowWbsColumn"/> changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowWbsColumnChanged { get; set; }

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
    [Parameter] public bool ShowEndColumn { get; set; } = false;

    /// <summary>
    /// Gets or sets callback raised when <see cref="ShowEndColumn"/> changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowEndColumnChanged { get; set; }

    /// <summary>
    /// Gets or sets whether duration column is visible.
    /// </summary>
    [Parameter] public bool ShowDurationColumn { get; set; } = true;

    /// <summary>
    /// Gets or sets callback raised when <see cref="ShowDurationColumn"/> changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowDurationColumnChanged { get; set; }

    /// <summary>
    /// Gets or sets whether command column (+ actions) is visible.
    /// </summary>
    [Parameter] public bool ShowCommandColumn { get; set; } = true;

    /// <summary>
    /// Gets or sets callback raised when <see cref="ShowCommandColumn"/> changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowCommandColumnChanged { get; set; }

    /// <summary>
    /// Gets or sets whether toggle-all commands (expand/collapse all) are visible in the toolbar.
    /// </summary>
    [Parameter] public bool ShowToggleAllCommands { get; set; } = true;

    /// <summary>
    /// Gets or sets whether tree column sorting is enabled.
    /// </summary>
    [Parameter] public bool Sortable { get; set; } = true;

    /// <summary>
    /// Gets or sets whether basic keyboard navigation is enabled for tree rows.
    /// </summary>
    [Parameter] public bool KeyboardNavigation { get; set; } = true;

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
    /// Gets or sets whether creating new top-level items is allowed.
    /// </summary>
    [Parameter] public bool NewCommandAllowed { get; set; } = true;

    /// <summary>
    /// Gets or sets whether creating child items is allowed.
    /// </summary>
    [Parameter] public bool AddChildCommandAllowed { get; set; } = true;

    /// <summary>
    /// Gets or sets whether editing existing items is allowed.
    /// </summary>
    [Parameter] public bool EditCommandAllowed { get; set; } = true;

    /// <summary>
    /// Gets or sets whether deleting existing items is allowed.
    /// </summary>
    [Parameter] public bool DeleteCommandAllowed { get; set; } = true;

    /// <summary>
    /// Gets or sets callback used to allow or deny command execution per item.
    /// </summary>
    [Parameter] public Func<GanttCommandContext<TItem>, bool> CommandAllowed { get; set; }

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
    /// Gets or sets template used to render tree header cells.
    /// </summary>
    [Parameter] public RenderFragment<GanttTreeHeaderCellContext<TItem>> TreeHeaderCellTemplate { get; set; }

    /// <summary>
    /// Gets or sets template used to render tree data cells.
    /// </summary>
    [Parameter] public RenderFragment<GanttTreeCellContext<TItem>> TreeCellTemplate { get; set; }

    /// <summary>
    /// Gets or sets template used to render command header content.
    /// </summary>
    [Parameter] public RenderFragment<GanttTreeCommandHeaderContext<TItem>> TreeCommandHeaderTemplate { get; set; }

    /// <summary>
    /// Gets or sets template used to render command cell content.
    /// </summary>
    [Parameter] public RenderFragment<GanttTreeCommandCellContext<TItem>> TreeCommandCellTemplate { get; set; }

    /// <summary>
    /// Gets or sets template used to render timeline header cells.
    /// </summary>
    [Parameter] public RenderFragment<GanttTimelineHeaderCellContext> TimelineHeaderCellTemplate { get; set; }

    /// <summary>
    /// Gets or sets template used to render timeline task content.
    /// </summary>
    [Parameter] public RenderFragment<GanttItemContext<TItem>> TaskItemTemplate { get; set; }

    /// <summary>
    /// Gets or sets title column width in pixels.
    /// </summary>
    [Parameter] public double TitleColumnWidth { get; set; } = 320d;

    /// <summary>
    /// Gets or sets fallback date column width in pixels used when an auto-sized value cannot be calculated.
    /// </summary>
    [Parameter] public double DateColumnWidth { get; set; } = 140d;

    /// <summary>
    /// Gets or sets action column width in pixels.
    /// </summary>
    [Parameter] public double ActionColumnWidth { get; set; } = 42d;

    /// <summary>
    /// Gets or sets header row height in pixels.
    /// </summary>
    [Parameter] public double HeaderRowHeight { get; set; } = DefaultHeaderRowHeight;

    /// <summary>
    /// Gets or sets search input width in pixels.
    /// </summary>
    [Parameter] public double SearchInputWidth { get; set; } = DefaultSearchInputWidth;

    /// <summary>
    /// Gets or sets tree toggle placeholder width in pixels.
    /// </summary>
    [Parameter] public double TreeToggleWidth { get; set; } = DefaultTreeToggleWidth;

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