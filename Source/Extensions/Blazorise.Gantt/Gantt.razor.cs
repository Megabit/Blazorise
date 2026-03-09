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
    private const string TimelinePaddedSlotBackgroundColor = "rgba(0,0,0,0.04)";
    private const string TimelineAnchorBoundaryColor = "rgba(0,0,0,0.16)";
    private const string WbsPseudoField = "__wbs";
    private const string CommandPseudoField = "__commands";

    private readonly HashSet<string> collapsedNodeKeys = new( StringComparer.Ordinal );
    private readonly List<BaseGanttColumn<TItem>> columns = new();
    private readonly Dictionary<BaseGanttColumn<TItem>, bool> columnVisibility = new();

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
    private bool showWbsColumn;
    private bool showStartColumn = true;
    private bool showEndColumn;
    private bool showDurationColumn = true;
    private bool showProgressColumn = true;
    private bool showCommandColumn = true;
    private string focusedRowKey;
    private bool shouldFocusTreeRows;
    private string ganttSortField;
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
    private int barDragMaxSlotOffset;
    private int nextColumnId;
    private readonly Dictionary<BaseGanttColumn<TItem>, string> columnKeys = new();
    private readonly Dictionary<string, int> columnDisplayOrders = new( StringComparer.Ordinal );
    private readonly List<string> legacyColumnOrder = new();
    private FluentUnitValue treeListWidthOverride;
    private bool applyingState;

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
            parameters.TryGetValue<string>( nameof( ItemsField ), out var paramItemsField ) && !string.Equals( ItemsField, paramItemsField, StringComparison.Ordinal ) ||
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

    internal void AddColumn( BaseGanttColumn<TItem> column )
    {
        if ( column is null || columns.Contains( column ) )
            return;

        if ( column is GanttCommandColumn<TItem> && columns.Any( x => x is GanttCommandColumn<TItem> ) )
            return;

        columns.Add( column );
        columnVisibility[column] = column.Visible;
        columnKeys[column] = $"column-{( ++nextColumnId ).ToString( CultureInfo.InvariantCulture )}";
        EnsureAtLeastOneDeclarativeColumnVisible();

        _ = InvokeAsync( StateHasChanged );
    }

    internal bool RemoveColumn( BaseGanttColumn<TItem> column )
    {
        if ( column is null )
            return false;

        if ( columnKeys.TryGetValue( column, out var key ) && !string.IsNullOrWhiteSpace( key ) )
            columnDisplayOrders.Remove( key );

        columnVisibility.Remove( column );
        columnKeys.Remove( column );

        var removed = columns.Remove( column );

        if ( removed && string.Equals( ganttSortField, column?.GetSortField(), StringComparison.OrdinalIgnoreCase ) )
        {
            ganttSortField = null;
            ganttSortDirection = SortDirection.Default;
        }

        EnsureAtLeastOneDeclarativeColumnVisible();

        if ( removed )
            _ = InvokeAsync( StateHasChanged );

        return removed;
    }

    /// <summary>
    /// Resolves whether a mapped field is editable based on declarative column configuration.
    /// </summary>
    /// <param name="field">Field name.</param>
    /// <param name="editState">Current edit state.</param>
    /// <returns>True when field is editable; otherwise false.</returns>
    protected internal bool IsFieldEditable( string field, GanttEditState editState = GanttEditState.Edit )
    {
        if ( string.IsNullOrWhiteSpace( field ) || columns.Count == 0 )
            return true;

        var mappedColumn = GetOrderedColumns()
            .OfType<GanttColumn<TItem>>()
            .FirstOrDefault( x => StringUtils.IsMatch( x.Field, field ) );

        if ( mappedColumn is null )
            return true;

        return mappedColumn.CellValueIsEditable( editState );
    }

    /// <summary>
    /// Gets current Gantt state.
    /// </summary>
    /// <returns>Current Gantt state.</returns>
    public Task<GanttState<TItem>> GetState()
    {
        var state = new GanttState<TItem>
        {
            Date = currentDate,
            SelectedView = SelectedView,
            SearchText = searchText,
            SortField = ganttSortDirection == SortDirection.Default ? null : ganttSortField,
            SortDirection = ganttSortDirection,
            SelectedRow = SelectedRow,
            FocusedRowKey = focusedRowKey,
            TreeListWidth = NormalizeTreeListWidth( treeListWidthOverride ),
            EditState = editState,
            EditItem = editState == GanttEditState.None ? default : editItem,
            EditParentItem = editState == GanttEditState.New ? editParentItem : default,
            CollapsedRowKeys = collapsedNodeKeys.ToList(),
            ColumnStates = GetColumnStatesForCurrentState(),
        };

        return Task.FromResult( state );
    }

    /// <summary>
    /// Loads and applies Gantt state.
    /// </summary>
    /// <param name="ganttState">State to apply.</param>
    public async Task LoadState( GanttState<TItem> ganttState )
    {
        if ( ganttState is null || applyingState )
            return;

        applyingState = true;

        try
        {
            var normalizedSearchText = ganttState.SearchText ?? string.Empty;
            var targetDate = ganttState.Date == default ? currentDate : ganttState.Date;
            var targetView = Enum.IsDefined( typeof( GanttView ), ganttState.SelectedView )
                ? ganttState.SelectedView
                : SelectedView;
            var dateChanged = currentDate != targetDate;
            var viewChanged = SelectedView != targetView;
            var searchChanged = !string.Equals( searchText, normalizedSearchText, StringComparison.Ordinal );
            var previousSelectedRow = SelectedRow;

            currentDate = targetDate;
            SelectedView = targetView;
            searchText = normalizedSearchText;
            focusedRowKey = ganttState.FocusedRowKey;
            treeListWidthOverride = NormalizeTreeListWidth( ganttState.TreeListWidth );

            if ( Sortable
                 && !string.IsNullOrWhiteSpace( ganttState.SortField )
                 && ganttState.SortDirection != SortDirection.Default )
            {
                ganttSortField = ganttState.SortField;
                ganttSortDirection = ganttState.SortDirection;
            }
            else
            {
                ganttSortField = null;
                ganttSortDirection = SortDirection.Default;
            }

            collapsedNodeKeys.Clear();

            if ( !ganttState.CollapsedRowKeys.IsNullOrEmpty() )
            {
                foreach ( var key in ganttState.CollapsedRowKeys.Where( x => !string.IsNullOrWhiteSpace( x ) ) )
                {
                    collapsedNodeKeys.Add( key );
                }
            }

            ApplyColumnStates( ganttState.ColumnStates );

            editState = ganttState.EditState;

            var resolvedSelectedRow = ResolveStateItemReference( ganttState.SelectedRow );
            var resolvedEditItem = ganttState.EditState == GanttEditState.None
                ? default
                : ResolveStateItemReference( ganttState.EditItem );
            var resolvedEditParentItem = ganttState.EditState == GanttEditState.New
                ? ResolveStateItemReference( ganttState.EditParentItem )
                : default;

            SelectedRow = resolvedSelectedRow;
            editItem = resolvedEditItem;
            editParentItem = resolvedEditParentItem;

            var selectedRowChanged = !AreRowsEqual( previousSelectedRow, SelectedRow );

            if ( dateChanged )
                await DateChanged.InvokeAsync( currentDate );

            if ( viewChanged )
                await SelectedViewChanged.InvokeAsync( SelectedView );

            if ( searchChanged )
                await SearchTextChanged.InvokeAsync( searchText );

            if ( selectedRowChanged )
                await SelectedRowChanged.InvokeAsync( SelectedRow );

            if ( UseInternalEditing
                 && ganttItemModalRef is not null
                 && editItem is not null )
            {
                if ( editState == GanttEditState.New )
                    await ganttItemModalRef.ShowModal( editItem.DeepClone(), editState, editParentItem );
                else if ( editState == GanttEditState.Edit )
                    await ganttItemModalRef.ShowModal( editItem.DeepClone(), editState );
            }

            await RefreshInternalAsync();
        }
        finally
        {
            applyingState = false;
            await InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Navigates to the previous period based on the selected view.
    /// </summary>
    public async Task NavigatePreviousPeriod()
    {
        if ( SelectedView == GanttView.Week )
        {
            currentDate = currentDate.StartOfWeek( EffectiveFirstDayOfWeek ).AddDays( -7 );
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
            currentDate = currentDate.StartOfWeek( EffectiveFirstDayOfWeek ).AddDays( 7 );
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

    private async Task NotifyNewItemClicked()
    {
        if ( !IsCommandAllowed( GanttCommandType.New ) )
            return;

        var item = CreateNewItem();

        await New( item );

        if ( NewItemClicked.HasDelegate )
        {
            await NewItemClicked.InvokeAsync( new GanttCommandContext<TItem>( GanttCommandType.New, item ) );
        }
    }

    private async Task NotifyAddChildItemClicked( TItem parentItem )
    {
        if ( parentItem is null || !IsCommandAllowed( GanttCommandType.AddChild, parentItem: parentItem ) )
            return;

        var item = CreateNewItem();

        await New( item, parentItem );

        if ( AddChildItemClicked.HasDelegate )
        {
            await AddChildItemClicked.InvokeAsync( new GanttCommandContext<TItem>( GanttCommandType.AddChild, item, parentItem ) );
        }
    }

    private async Task NotifyEditItemClicked( TItem item )
    {
        if ( item is null || !IsCommandAllowed( GanttCommandType.Edit, item ) )
            return;

        await Edit( item );

        if ( EditItemClicked.HasDelegate )
        {
            await EditItemClicked.InvokeAsync( new GanttItemClickedEventArgs<TItem>( item ) );
        }
    }

    private async Task NotifyDeleteItemClicked( TItem item )
    {
        if ( item is null || !IsCommandAllowed( GanttCommandType.Delete, item ) )
            return;

        if ( Editable && UseInternalEditing )
        {
            if ( await ConfirmDelete() == false )
                return;

            await DeleteItemImpl( item );
        }

        if ( DeleteItemClicked.HasDelegate )
        {
            await DeleteItemClicked.InvokeAsync( new GanttItemClickedEventArgs<TItem>( item ) );
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

        if ( submittedEditState == GanttEditState.New && UseInternalEditing && CanInsertNewItem )
        {
            if ( !TryInsertNewItem( targetItem, editParentItem ) )
                return false;
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

    private bool TryInsertNewItem( TItem itemToInsert, TItem parentItem )
    {
        if ( itemToInsert is null )
            return false;

        if ( UseHierarchicalData && parentItem is not null )
        {
            var resolvedParentItem = ResolveStateItemReference( parentItem );

            if ( resolvedParentItem is not null )
            {
                var childItems = propertyMapper.GetItemsCollection( resolvedParentItem, createIfMissing: true );

                if ( childItems is not null )
                {
                    if ( propertyMapper.HasParentId )
                        propertyMapper.SetParentId( itemToInsert, propertyMapper.GetId( resolvedParentItem ) );

                    childItems.Add( itemToInsert );
                    return true;
                }
            }

            return false;
        }

        if ( Data is ICollection<TItem> data )
        {
            if ( propertyMapper.HasParentId && parentItem is not null )
                propertyMapper.SetParentId( itemToInsert, propertyMapper.GetId( parentItem ) );

            data.Add( itemToInsert );
            return true;
        }

        return false;
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
        var hasActiveSort = Sortable && !string.IsNullOrWhiteSpace( ganttSortField ) && ganttSortDirection != SortDirection.Default;
        var sortField = hasActiveSort ? ganttSortField : null;
        var sortColumnField = hasActiveSort ? ResolveColumnFieldFromSortField( ganttSortField ) : null;
        var sortDirection = hasActiveSort ? ganttSortDirection : SortDirection.Default;

        try
        {
            await ReadData.InvokeAsync( new GanttReadDataEventArgs<TItem>(
                SelectedView,
                currentDate,
                viewRange.Start,
                viewRange.End,
                searchText,
                sortField,
                sortColumnField,
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

    private IReadOnlyList<GanttColumnPickerItem> GetColumnPickerItems()
    {
        if ( columns.Count > 0 )
        {
            var items = new List<GanttColumnPickerItem>();

            foreach ( var column in GetOrderedColumns() )
            {
                if ( column is null || !column.Displayable )
                    continue;

                items.Add( new GanttColumnPickerItem
                {
                    Key = GetColumnKey( column ),
                    Text = GetColumnHeaderText( column ),
                    Visible = IsColumnVisible( column ),
                } );
            }

            return items;
        }

        var legacyItems = new List<GanttColumnPickerItem>
        {
            new() { Key = WbsPseudoField, Text = WbsColumnHeaderText, Visible = showWbsColumn },
            new() { Key = TitleField, Text = TaskColumnHeaderText, Visible = showTitleColumn },
            new() { Key = StartField, Text = StartColumnHeaderText, Visible = showStartColumn },
            new() { Key = EndField, Text = EndColumnHeaderText, Visible = showEndColumn },
            new() { Key = DurationField, Text = DurationColumnHeaderText, Visible = showDurationColumn },
        };

        if ( ProgressColumnAvailable )
        {
            legacyItems.Add( new GanttColumnPickerItem
            {
                Key = ProgressField,
                Text = ProgressColumnHeaderText,
                Visible = showProgressColumn,
            } );
        }

        legacyItems.Add( new GanttColumnPickerItem
        {
            Key = CommandPseudoField,
            Text = AddChildText,
            Visible = showCommandColumn,
        } );

        return legacyItems;
    }

    private async Task OnColumnVisibilityChanged( GanttColumnVisibilityChangedEventArgs args )
    {
        if ( args is null || string.IsNullOrWhiteSpace( args.Key ) )
            return;

        if ( columns.Count > 0 )
        {
            var targetColumn = columns.FirstOrDefault( x => string.Equals( GetColumnKey( x ), args.Key, StringComparison.Ordinal ) );

            if ( targetColumn is null )
                return;

            columnVisibility[targetColumn] = args.Visible;
            EnsureAtLeastOneDeclarativeColumnVisible();

            await InvokeAsync( StateHasChanged );
            return;
        }

        if ( string.Equals( args.Key, WbsPseudoField, StringComparison.Ordinal ) )
            showWbsColumn = args.Visible;
        else if ( string.Equals( args.Key, TitleField, StringComparison.Ordinal ) )
            showTitleColumn = args.Visible;
        else if ( string.Equals( args.Key, StartField, StringComparison.Ordinal ) )
            showStartColumn = args.Visible;
        else if ( string.Equals( args.Key, EndField, StringComparison.Ordinal ) )
            showEndColumn = args.Visible;
        else if ( string.Equals( args.Key, DurationField, StringComparison.Ordinal ) )
            showDurationColumn = args.Visible;
        else if ( string.Equals( args.Key, ProgressField, StringComparison.Ordinal ) )
            showProgressColumn = args.Visible;
        else if ( string.Equals( args.Key, CommandPseudoField, StringComparison.Ordinal ) )
            showCommandColumn = args.Visible;

        EnsureAtLeastOneColumnVisible();

        await InvokeAsync( StateHasChanged );
    }

    private void EnsureAtLeastOneColumnVisible()
    {
        if ( columns.Count > 0 )
        {
            EnsureAtLeastOneDeclarativeColumnVisible();
            return;
        }

        var hasVisibleRegularColumn = showTitleColumn
            || showWbsColumn
            || showStartColumn
            || showEndColumn
            || showDurationColumn
            || ( ProgressColumnAvailable && showProgressColumn );

        if ( !hasVisibleRegularColumn )
        {
            showTitleColumn = true;
        }
    }

    private List<GanttColumnState> GetColumnStatesForCurrentState()
    {
        var columnStates = new List<GanttColumnState>();

        if ( columns.Count > 0 )
        {
            var orderedColumns = GetOrderedColumns().ToList();

            for ( int i = 0; i < orderedColumns.Count; i++ )
            {
                var column = orderedColumns[i];
                var key = GetColumnKey( column );
                var width = GetColumnStateWidth( column.Width );

                if ( string.IsNullOrWhiteSpace( key ) )
                    continue;

                columnStates.Add( new GanttColumnState
                {
                    Key = key,
                    Field = column.Field,
                    Visible = IsColumnVisible( column ),
                    DisplayOrder = GetColumnDisplayOrder( column, i ),
                    Width = width,
                } );
            }

            return columnStates;
        }

        var defaultLegacyOrder = GetLegacyColumnDefaultOrder();

        for ( int i = 0; i < defaultLegacyOrder.Count; i++ )
        {
            var key = defaultLegacyOrder[i];
            var visible = key switch
            {
                var x when StringUtils.IsMatch( x, WbsPseudoField ) => showWbsColumn,
                var x when StringUtils.IsMatch( x, TitleField ) => showTitleColumn,
                var x when StringUtils.IsMatch( x, StartField ) => showStartColumn,
                var x when StringUtils.IsMatch( x, EndField ) => showEndColumn,
                var x when StringUtils.IsMatch( x, DurationField ) => showDurationColumn,
                var x when StringUtils.IsMatch( x, ProgressField ) => showProgressColumn,
                var x when StringUtils.IsMatch( x, CommandPseudoField ) => showCommandColumn,
                _ => true,
            };

            columnStates.Add( new GanttColumnState
            {
                Key = key,
                Field = key,
                Visible = visible,
                DisplayOrder = GetLegacyDisplayOrder( key, i ),
                Width = null,
            } );
        }

        return columnStates.OrderBy( x => x.DisplayOrder ).ToList();
    }

    private void ApplyColumnStates( IReadOnlyList<GanttColumnState> columnStates )
    {
        columnDisplayOrders.Clear();
        legacyColumnOrder.Clear();

        if ( columns.Count > 0 )
        {
            foreach ( var column in columns )
            {
                if ( !columnVisibility.ContainsKey( column ) )
                    columnVisibility[column] = column.Visible;
            }

            if ( columnStates.IsNullOrEmpty() )
            {
                EnsureAtLeastOneDeclarativeColumnVisible();
                return;
            }

            var matchedColumnsCount = 0;

            foreach ( var columnState in columnStates.OrderBy( x => x.DisplayOrder ) )
            {
                var column = FindDeclarativeColumnFromState( columnState );

                if ( column is null )
                    continue;

                var key = GetColumnKey( column );

                if ( string.IsNullOrWhiteSpace( key ) )
                    continue;

                columnVisibility[column] = columnState.Visible;
                columnDisplayOrders[key] = columnState.DisplayOrder;

                if ( TryBuildColumnWidth( columnState?.Width, out var restoredWidth ) )
                    column.Width = restoredWidth;

                matchedColumnsCount++;
            }

            if ( matchedColumnsCount == 0 )
            {
                EnsureAtLeastOneDeclarativeColumnVisible();
                return;
            }

            EnsureAtLeastOneDeclarativeColumnVisible();
            return;
        }

        ApplyLegacyColumnStates( columnStates );
    }

    private void ApplyLegacyColumnStates( IReadOnlyList<GanttColumnState> columnStates )
    {
        if ( columnStates.IsNullOrEmpty() )
        {
            EnsureAtLeastOneColumnVisible();
            return;
        }

        foreach ( var columnState in columnStates.OrderBy( x => x.DisplayOrder ) )
        {
            var key = ResolveLegacyColumnKey( columnState );

            if ( string.IsNullOrWhiteSpace( key ) )
                continue;

            if ( !legacyColumnOrder.Any( x => StringUtils.IsMatch( x, key ) ) )
                legacyColumnOrder.Add( key );

            if ( StringUtils.IsMatch( key, WbsPseudoField ) )
                showWbsColumn = columnState.Visible;
            else if ( StringUtils.IsMatch( key, TitleField ) )
                showTitleColumn = columnState.Visible;
            else if ( StringUtils.IsMatch( key, StartField ) )
                showStartColumn = columnState.Visible;
            else if ( StringUtils.IsMatch( key, EndField ) )
                showEndColumn = columnState.Visible;
            else if ( StringUtils.IsMatch( key, DurationField ) )
                showDurationColumn = columnState.Visible;
            else if ( StringUtils.IsMatch( key, ProgressField ) )
                showProgressColumn = columnState.Visible;
            else if ( StringUtils.IsMatch( key, CommandPseudoField ) )
                showCommandColumn = columnState.Visible;
        }

        foreach ( var key in GetLegacyColumnDefaultOrder() )
        {
            if ( !legacyColumnOrder.Any( x => StringUtils.IsMatch( x, key ) ) )
                legacyColumnOrder.Add( key );
        }

        EnsureAtLeastOneColumnVisible();
    }

    private BaseGanttColumn<TItem> FindDeclarativeColumnFromState( GanttColumnState columnState )
    {
        if ( columnState is null )
            return null;

        if ( !string.IsNullOrWhiteSpace( columnState.Field ) )
        {
            if ( !string.IsNullOrWhiteSpace( columnState.Key ) )
            {
                var columnByKeyAndField = columns.FirstOrDefault( x =>
                    StringUtils.IsMatch( GetColumnKey( x ), columnState.Key )
                    && IsColumnFieldMatchState( x, columnState.Field ) );

                if ( columnByKeyAndField is not null )
                    return columnByKeyAndField;
            }

            var columnByField = columns.FirstOrDefault( x => IsColumnFieldMatchState( x, columnState.Field ) );

            if ( columnByField is not null )
                return columnByField;
        }

        if ( !string.IsNullOrWhiteSpace( columnState.Key ) )
        {
            var columnByKey = columns.FirstOrDefault( x => StringUtils.IsMatch( GetColumnKey( x ), columnState.Key ) );

            if ( columnByKey is not null )
                return columnByKey;
        }

        return null;
    }

    private static bool IsColumnFieldMatchState( BaseGanttColumn<TItem> column, string field )
    {
        if ( column is null || string.IsNullOrWhiteSpace( field ) )
            return false;

        if ( IsWbsField( field ) )
            return IsWbsField( column.Field );

        return string.Equals( column.Field, field, StringComparison.OrdinalIgnoreCase );
    }

    private FluentUnitValue GetColumnStateWidth( IFluentSizing width )
    {
        if ( !CssValueUtils.TryGetNumericStyleValue( width, StyleProvider, "width", out var parsedUnit, out var parsedValue ) )
            return null;

        if ( parsedValue <= 0d )
            return null;

        return new FluentUnitValue( parsedUnit, parsedValue );
    }

    private static bool TryBuildColumnWidth( FluentUnitValue widthState, out IFluentSizing width )
    {
        width = null;

        if ( widthState is null
             || !widthState.HasValue
             || widthState.Value is null
             || widthState.Value.Value <= 0d )
            return false;

        width = widthState.ToFluentSizing( SizingType.Width );

        return width is not null;
    }

    private static FluentUnitValue NormalizeTreeListWidth( FluentUnitValue treeListWidth )
    {
        if ( treeListWidth is null
             || !treeListWidth.HasValue
             || treeListWidth.Value is null
             || treeListWidth.Value.Value <= 0d )
            return null;

        return treeListWidth.Clone();
    }

    private List<string> GetLegacyColumnDefaultOrder()
    {
        var order = new List<string>
        {
            WbsPseudoField,
            TitleField,
            StartField,
            EndField,
            DurationField,
            CommandPseudoField,
        };

        if ( ProgressColumnAvailable )
            order.Insert( order.Count - 1, ProgressField );

        return order;
    }

    private int GetColumnDisplayOrder( BaseGanttColumn<TItem> column, int fallbackOrder )
    {
        var key = GetColumnKey( column );

        if ( string.IsNullOrWhiteSpace( key ) )
            return fallbackOrder;

        return columnDisplayOrders.TryGetValue( key, out var order )
            ? order
            : fallbackOrder;
    }

    private int GetLegacyDisplayOrder( string key, int fallbackOrder )
    {
        var index = legacyColumnOrder.FindIndex( x => StringUtils.IsMatch( x, key ) );
        return index >= 0 ? index : fallbackOrder;
    }

    private string ResolveLegacyColumnKey( GanttColumnState columnState )
    {
        if ( columnState is null )
            return null;

        var candidates = new[] { columnState.Key, columnState.Field };

        foreach ( var candidate in candidates )
        {
            if ( IsWbsField( candidate ) )
                return WbsPseudoField;

            if ( StringUtils.IsMatch( candidate, TitleField ) )
                return TitleField;

            if ( StringUtils.IsMatch( candidate, StartField ) )
                return StartField;

            if ( StringUtils.IsMatch( candidate, EndField ) )
                return EndField;

            if ( StringUtils.IsMatch( candidate, DurationField ) )
                return DurationField;

            if ( StringUtils.IsMatch( candidate, ProgressField ) )
                return ProgressField;

            if ( StringUtils.IsMatch( candidate, CommandPseudoField ) )
                return CommandPseudoField;
        }

        return null;
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

        if ( DateTimeUtils.IsUnassigned( start ) || DateTimeUtils.IsUnassigned( end ) )
            return null;

        return DateTimeUtils.GetDurationInDays( start, end );
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
        if ( item is null || !propertyMapper.HasProgress )
            return null;

        return propertyMapper.GetProgressPercentage( item );
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

    private static string FormatProgress( double? progress )
    {
        if ( progress is null )
            return string.Empty;

        return $"{progress.Value.ToString( "0.##", CultureInfo.InvariantCulture )}%";
    }

    private void ApplyNewItemDateDefaults( TItem item, TItem parentItem )
    {
        if ( item is null || ( !propertyMapper.HasStart && !propertyMapper.HasEnd ) )
            return;

        var (defaultStart, defaultEnd) = GetDefaultNewItemRange( parentItem );

        if ( propertyMapper.HasStart )
        {
            var itemStart = propertyMapper.GetStart( item );

            if ( DateTimeUtils.IsUnassigned( itemStart ) )
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

            if ( DateTimeUtils.IsUnassigned( itemEnd ) || itemEnd <= defaultStart )
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

                if ( DateTimeUtils.IsUnassigned( itemEnd ) || itemEnd <= itemStart )
                    itemEnd = itemStart.Add( GetDefaultNewItemDuration() );

                itemDuration = DateTimeUtils.GetDurationInDays( itemStart, itemEnd );
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
        var viewRange = GetCurrentAnchorRange();
        var duration = GetDefaultNewItemDuration();
        var start = viewRange.Start;
        var end = start.Add( duration );

        if ( parentItem is not null )
        {
            var parentStart = propertyMapper.HasStart ? GetItemStart( parentItem ) : DateTime.MinValue;
            var parentEnd = propertyMapper.HasEnd ? GetItemEnd( parentItem ) : DateTime.MinValue;

            if ( !DateTimeUtils.IsUnassigned( parentStart ) )
            {
                start = parentStart;
            }

            end = start.Add( duration );

            if ( !DateTimeUtils.IsUnassigned( parentEnd ) && parentEnd > start && end > parentEnd )
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
            await NotifyEditItemClicked( item );
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
        barDragMaxSlotOffset = Math.Max( 1, GetTimeSlots( GetCurrentViewRange().Start, GetCurrentViewRange().End ).Count );

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
    public Task NotifyBarDragMouseUp( double clientX, bool dragged )
    {
        if ( dragged && !barDragging )
        {
            TryUpdateBarDragFromClientX( clientX );
        }

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
        barDragMaxSlotOffset = 0;
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

        if ( !IsSlotOffsetWithinBounds( nextSlotOffset, barDragMaxSlotOffset ) )
            return false;

        if ( nextSlotOffset == barDragSlotOffset )
            return false;

        barDragSlotOffset = nextSlotOffset;

        return true;
    }

    private bool CanDragItem( TItem item )
    {
        if ( item is null )
            return false;

        if ( !UseInternalEditing )
            return false;

        if ( !IsCommandAllowed( GanttCommandType.Edit, item ) )
            return false;

        if ( !IsFieldEditable( StartField, GanttEditState.Edit ) )
            return false;

        if ( !propertyMapper.HasStart || !propertyMapper.HasEnd )
            return false;

        var itemStart = GetItemStart( item );
        var itemEnd = GetItemEnd( item );

        if ( DateTimeUtils.IsUnassigned( itemStart ) || DateTimeUtils.IsUnassigned( itemEnd ) )
            return false;

        return itemEnd > itemStart;
    }

    private static int GetSlotOffsetFromDeltaX( double deltaX, double cellWidth )
    {
        if ( cellWidth <= 0d )
            return 0;

        return (int)Math.Round( deltaX / cellWidth, MidpointRounding.AwayFromZero );
    }

    private static bool IsSlotOffsetWithinBounds( int slotOffset, int maxAbsoluteSlotOffset )
    {
        if ( maxAbsoluteSlotOffset <= 0 )
            return true;

        return slotOffset <= maxAbsoluteSlotOffset && slotOffset >= -maxAbsoluteSlotOffset;
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

        var targetItem = ResolveStateItemReference( item );

        if ( targetItem is null || !CanDragItem( targetItem ) )
            return;

        var sourceStart = GetItemStart( targetItem );
        var sourceEnd = GetItemEnd( targetItem );

        if ( DateTimeUtils.IsUnassigned( sourceStart ) || DateTimeUtils.IsUnassigned( sourceEnd ) )
            return;

        var movedStart = ShiftDateBySlotOffset( sourceStart, slotOffset );
        var movedEnd = ShiftDateBySlotOffset( sourceEnd, slotOffset );

        if ( movedEnd <= movedStart )
            return;

        var movedItem = targetItem.DeepClone();

        if ( propertyMapper.HasId )
            propertyMapper.SetId( movedItem, propertyMapper.GetId( targetItem ) );

        if ( propertyMapper.HasParentId )
            propertyMapper.SetParentId( movedItem, propertyMapper.GetParentId( targetItem ) );

        if ( propertyMapper.HasStart )
            propertyMapper.SetStart( movedItem, movedStart );

        if ( propertyMapper.HasEnd )
            propertyMapper.SetEnd( movedItem, movedEnd );

        if ( propertyMapper.HasDuration )
            propertyMapper.SetDuration( movedItem, DateTimeUtils.GetDurationInDays( movedStart, movedEnd ) );

        editItem = targetItem;
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
            await NotifyEditItemClicked( item );
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
                    await NotifyEditItemClicked( focusedRow.Item );
                break;
            case "Delete":
                if ( IsCommandAllowed( GanttCommandType.Delete, focusedRow.Item ) )
                    await NotifyDeleteItemClicked( focusedRow.Item );
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

    private Task SortByColumn( GanttRenderColumn column )
    {
        if ( !Sortable || column is null || !column.CanSort || string.IsNullOrWhiteSpace( column.SortField ) )
            return Task.CompletedTask;

        if ( string.Equals( ganttSortField, column.SortField, StringComparison.OrdinalIgnoreCase ) )
        {
            ganttSortDirection = GetNextSortDirection( ganttSortDirection );
        }
        else
        {
            ganttSortField = column.SortField;
            ganttSortDirection = SortDirection.Ascending;
        }

        return InvokeAsync( StateHasChanged );
    }

    private bool ShowSortIcon( GanttRenderColumn column )
        => Sortable
           && column is not null
           && column.CanSort
           && !string.IsNullOrWhiteSpace( column.SortField )
           && string.Equals( ganttSortField, column.SortField, StringComparison.OrdinalIgnoreCase )
           && ganttSortDirection != SortDirection.Default;

    private IconName GetSortIconName( GanttRenderColumn column )
        => !ShowSortIcon( column ) || ganttSortDirection == SortDirection.Ascending
            ? IconName.SortUp
            : IconName.SortDown;

    private GanttColumnHeaderContext<TItem> GetColumnHeaderContext( GanttRenderColumn column, bool showHeaderNewButton = false )
    {
        var showSortIcon = ShowSortIcon( column );
        var sortDirection = showSortIcon
            ? ganttSortDirection
            : SortDirection.Default;
        var isCommandColumn = column?.IsCommand ?? false;
        var canAddTask = isCommandColumn && showHeaderNewButton && IsCommandAllowed( GanttCommandType.New );
        Func<Task> addTask = canAddTask
            ? NotifyNewItemClicked
            : NoopAsync;

        return new GanttColumnHeaderContext<TItem>( this, column.Column, column.HeaderText, column.CanSort, showSortIcon, sortDirection, isCommandColumn, canAddTask, addTask, AddTaskText );
    }

    private GanttTreeCommandHeaderContext<TItem> GetTreeCommandHeaderContext( bool showHeaderNewButton )
    {
        Func<Task> addTask = showHeaderNewButton
            ? NotifyNewItemClicked
            : NoopAsync;

        return new GanttTreeCommandHeaderContext<TItem>( this, showHeaderNewButton, addTask, AddTaskText );
    }

    private GanttTreeCommandCellContext<TItem> GetTreeCommandCellContext( GanttTreeRow row )
    {
        var canAddChild = CanShowAddChildButton( row.Item );
        Func<Task> addChild = canAddChild
            ? () => NotifyAddChildItemClicked( row.Item )
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

    private GanttColumnDisplayContext<TItem> GetColumnDisplayContext( GanttTreeRow row, GanttRenderColumn column, IReadOnlyDictionary<string, string> wbsLookup, double treeToggleWidth )
    {
        Func<Task> toggleNode = row.HasChildren
            ? () => ToggleNode( row )
            : NoopAsync;

        var selected = IsSelectedRow( row.Item );
        var focused = string.Equals( focusedRowKey, row.Key, StringComparison.Ordinal );
        var value = GetColumnDisplayValue( column, row, wbsLookup );
        var text = GetColumnDisplayText( column, row, wbsLookup );

        return new GanttColumnDisplayContext<TItem>(
            this,
            column.Column,
            row.Item,
            row.Key,
            value,
            text,
            row.Level,
            row.HasChildren,
            IsExpanded( row ),
            selected,
            focused,
            toggleNode,
            treeToggleWidth );
    }

    private GanttCommandColumnDisplayContext<TItem> GetCommandColumnDisplayContext( GanttTreeRow row, GanttRenderColumn column, IReadOnlyDictionary<string, string> wbsLookup, double treeToggleWidth )
    {
        Func<Task> toggleNode = row.HasChildren
            ? () => ToggleNode( row )
            : NoopAsync;
        var canAddChild = CanShowAddChildButton( row.Item );
        Func<Task> addChild = canAddChild
            ? () => NotifyAddChildItemClicked( row.Item )
            : NoopAsync;
        var canEdit = IsCommandAllowed( GanttCommandType.Edit, row.Item );
        Func<Task> edit = canEdit
            ? () => NotifyEditItemClicked( row.Item )
            : NoopAsync;
        var canDelete = IsCommandAllowed( GanttCommandType.Delete, row.Item );
        Func<Task> delete = canDelete
            ? () => NotifyDeleteItemClicked( row.Item )
            : NoopAsync;

        var selected = IsSelectedRow( row.Item );
        var focused = string.Equals( focusedRowKey, row.Key, StringComparison.Ordinal );
        var value = GetColumnDisplayValue( column, row, wbsLookup );
        var text = GetColumnDisplayText( column, row, wbsLookup );

        return new GanttCommandColumnDisplayContext<TItem>(
            this,
            column.Column,
            row.Item,
            row.Key,
            value,
            text,
            row.Level,
            row.HasChildren,
            IsExpanded( row ),
            selected,
            focused,
            toggleNode,
            treeToggleWidth,
            canAddChild,
            addChild,
            canEdit,
            edit,
            canDelete,
            delete );
    }

    private object GetColumnDisplayValue( GanttRenderColumn column, GanttTreeRow row, IReadOnlyDictionary<string, string> wbsLookup )
    {
        if ( column is null || row is null )
            return null;

        if ( column.IsWbs )
            return GetWbsValue( wbsLookup, row.Key );

        if ( column.IsStart )
            return GetItemStart( row.Item );

        if ( column.IsEnd )
            return GetItemEnd( row.Item );

        if ( column.IsDuration )
            return GetItemDuration( row.Item );

        if ( column.IsProgress )
        {
            if ( column.Column is not null )
                return column.Column.GetValue( row.Item );

            return GetItemProgress( row.Item );
        }

        if ( column.Expandable )
            return GetItemTitle( row.Item );

        return column.Column?.GetValue( row.Item );
    }

    private string GetColumnDisplayText( GanttRenderColumn column, GanttTreeRow row, IReadOnlyDictionary<string, string> wbsLookup )
    {
        var value = GetColumnDisplayValue( column, row, wbsLookup );

        if ( value is null )
            return string.Empty;

        if ( column.IsStart || column.IsEnd )
            return FormatDate( (DateTime)value );

        if ( column.IsDuration )
        {
            var duration = value switch
            {
                int intValue => intValue,
                _ => GetItemDuration( row.Item ),
            };

            return FormatDuration( duration );
        }

        if ( column.IsProgress )
        {
            if ( column.Column?.DisplayFormat is not null )
                return column.Column.FormatDisplayValue( value );

            if ( !ValueUtils.TryConvertToDouble( value, out var progressValue ) )
                return Convert.ToString( value, CultureInfo.InvariantCulture ) ?? string.Empty;

            if ( progressValue <= 1d )
                progressValue *= 100d;

            var progress = Math.Max( 0d, Math.Min( 100d, progressValue ) );

            return FormatProgress( progress );
        }

        if ( column.Expandable || column.IsWbs )
            return Convert.ToString( value, CultureInfo.InvariantCulture ) ?? string.Empty;

        return column.Column?.FormatDisplayValue( value ) ?? Convert.ToString( value, CultureInfo.InvariantCulture ) ?? string.Empty;
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

    private List<GanttRenderColumn> GetTreeRenderColumns( IReadOnlyList<GanttTreeRow> visibleRows, IReadOnlyDictionary<string, string> wbsLookup, bool showHeaderNewButton )
    {
        var renderColumns = new List<GanttRenderColumn>();

        if ( columns.Count > 0 )
        {
            foreach ( var column in GetOrderedColumns() )
            {
                if ( column is null || !IsColumnVisible( column ) )
                    continue;

                if ( column is GanttCommandColumn<TItem> commandColumn )
                {
                    var showCommandColumn = showHeaderNewButton
                        || ( ShowAddChildColumn && visibleRows.Any( x => IsCommandAllowed( GanttCommandType.AddChild, parentItem: x.Item ) ) )
                        || commandColumn.HeaderTemplate is not null
                        || commandColumn.DisplayTemplate is not null;

                    if ( !showCommandColumn )
                        continue;

                    renderColumns.Add( new GanttRenderColumn(
                        key: GetColumnKey( column ),
                        column: column,
                        commandColumn: commandColumn,
                        headerText: GetColumnHeaderText( column ),
                        sortField: null,
                        canSort: false,
                        width: ResolveColumnWidth( column, visibleRows, wbsLookup ),
                        textAlignment: TextAlignment.Center,
                        isCommand: true,
                        isWbs: false,
                        isStart: false,
                        isEnd: false,
                        isDuration: false,
                        isProgress: false,
                        isExpandable: false ) );

                    continue;
                }

                var isWbs = IsWbsField( column.Field );
                var isStart = StringUtils.IsMatch( column.Field, StartField );
                var isEnd = StringUtils.IsMatch( column.Field, EndField );
                var isDuration = StringUtils.IsMatch( column.Field, DurationField );
                var isProgress = StringUtils.IsMatch( column.Field, ProgressField );
                var sortField = column.GetSortField();
                var canSort = Sortable && column.CanSort();
                var textAlignment = ResolveColumnTextAlignment( column, isStart, isEnd, isDuration, isProgress );

                renderColumns.Add( new GanttRenderColumn(
                    key: GetColumnKey( column ),
                    column: column,
                    commandColumn: null,
                    headerText: GetColumnHeaderText( column ),
                    sortField: sortField,
                    canSort: canSort,
                    width: ResolveColumnWidth( column, visibleRows, wbsLookup ),
                    textAlignment: textAlignment,
                    isCommand: false,
                    isWbs: isWbs,
                    isStart: isStart,
                    isEnd: isEnd,
                    isDuration: isDuration,
                    isProgress: isProgress,
                    isExpandable: column.Expandable ) );
            }

            return renderColumns;
        }

        if ( showWbsColumn )
        {
            var width = GetAutoSizedTreeColumnWidth( visibleRows, WbsColumnHeaderText, row => GetWbsValue( wbsLookup, row.Key ) );

            renderColumns.Add( new GanttRenderColumn(
                key: WbsPseudoField,
                column: null,
                commandColumn: null,
                headerText: WbsColumnHeaderText,
                sortField: null,
                canSort: false,
                width: width,
                textAlignment: TextAlignment.Default,
                isCommand: false,
                isWbs: true,
                isStart: false,
                isEnd: false,
                isDuration: false,
                isProgress: false,
                isExpandable: false ) );
        }

        if ( showTitleColumn )
        {
            renderColumns.Add( new GanttRenderColumn(
                key: TitleField,
                column: null,
                commandColumn: null,
                headerText: TaskColumnHeaderText,
                sortField: TitleField,
                canSort: Sortable,
                width: TitleColumnWidth,
                textAlignment: TextAlignment.Default,
                isCommand: false,
                isWbs: false,
                isStart: false,
                isEnd: false,
                isDuration: false,
                isProgress: false,
                isExpandable: true ) );
        }

        if ( showStartColumn )
        {
            var width = GetAutoSizedTreeColumnWidth( visibleRows, StartColumnHeaderText, row => FormatDate( GetItemStart( row.Item ) ), Sortable );
            renderColumns.Add( new GanttRenderColumn(
                key: StartField,
                column: null,
                commandColumn: null,
                headerText: StartColumnHeaderText,
                sortField: StartField,
                canSort: Sortable,
                width: width,
                textAlignment: TextAlignment.Center,
                isCommand: false,
                isWbs: false,
                isStart: true,
                isEnd: false,
                isDuration: false,
                isProgress: false,
                isExpandable: false ) );
        }

        if ( showEndColumn )
        {
            var width = GetAutoSizedTreeColumnWidth( visibleRows, EndColumnHeaderText, row => FormatDate( GetItemEnd( row.Item ) ), Sortable );
            renderColumns.Add( new GanttRenderColumn(
                key: EndField,
                column: null,
                commandColumn: null,
                headerText: EndColumnHeaderText,
                sortField: EndField,
                canSort: Sortable,
                width: width,
                textAlignment: TextAlignment.Center,
                isCommand: false,
                isWbs: false,
                isStart: false,
                isEnd: true,
                isDuration: false,
                isProgress: false,
                isExpandable: false ) );
        }

        if ( showDurationColumn )
        {
            var width = GetAutoSizedTreeColumnWidth( visibleRows, DurationColumnHeaderText, row => FormatDuration( GetItemDuration( row.Item ) ), Sortable );
            renderColumns.Add( new GanttRenderColumn(
                key: DurationField,
                column: null,
                commandColumn: null,
                headerText: DurationColumnHeaderText,
                sortField: DurationField,
                canSort: Sortable,
                width: width,
                textAlignment: TextAlignment.Center,
                isCommand: false,
                isWbs: false,
                isStart: false,
                isEnd: false,
                isDuration: true,
                isProgress: false,
                isExpandable: false ) );
        }

        if ( ProgressColumnAvailable && showProgressColumn )
        {
            var width = GetAutoSizedTreeColumnWidth( visibleRows, ProgressColumnHeaderText, row => FormatProgress( GetItemProgress( row.Item ) ), Sortable );
            renderColumns.Add( new GanttRenderColumn(
                key: ProgressField,
                column: null,
                commandColumn: null,
                headerText: ProgressColumnHeaderText,
                sortField: ProgressField,
                canSort: Sortable,
                width: width,
                textAlignment: TextAlignment.Center,
                isCommand: false,
                isWbs: false,
                isStart: false,
                isEnd: false,
                isDuration: false,
                isProgress: true,
                isExpandable: false ) );
        }

        if ( showCommandColumn && CanShowActionColumn( visibleRows ) )
        {
            renderColumns.Add( new GanttRenderColumn(
                key: CommandPseudoField,
                column: null,
                commandColumn: null,
                headerText: AddChildText,
                sortField: null,
                canSort: false,
                width: ActionColumnWidth,
                textAlignment: TextAlignment.Center,
                isCommand: true,
                isWbs: false,
                isStart: false,
                isEnd: false,
                isDuration: false,
                isProgress: false,
                isExpandable: false ) );
        }

        if ( legacyColumnOrder.Count > 0 )
        {
            var indexedColumns = renderColumns
                .Select( ( column, index ) => new { Column = column, Index = index } )
                .ToList();

            return indexedColumns
                .Where( x => x.Column.IsWbs )
                .OrderBy( x => GetLegacyDisplayOrder( x.Column.Key, x.Index ) )
                .ThenBy( x => x.Index )
                .Concat( indexedColumns
                    .Where( x => !x.Column.IsWbs && !x.Column.IsCommand )
                    .OrderBy( x => GetLegacyDisplayOrder( x.Column.Key, x.Index ) )
                    .ThenBy( x => x.Index ) )
                .Concat( indexedColumns
                    .Where( x => x.Column.IsCommand )
                    .OrderBy( x => GetLegacyDisplayOrder( x.Column.Key, x.Index ) )
                    .ThenBy( x => x.Index ) )
                .Select( x => x.Column )
                .ToList();
        }

        return renderColumns;
    }

    private IReadOnlyList<BaseGanttColumn<TItem>> GetOrderedColumns()
    {
        if ( columns.Count == 0 )
            return Array.Empty<BaseGanttColumn<TItem>>();

        var regularColumns = columns
            .Select( ( column, index ) => new { Column = column, Index = index } )
            .Where( x => x.Column is not GanttCommandColumn<TItem> )
            .ToList();

        var commandColumns = columns
            .Select( ( column, index ) => new { Column = column, Index = index } )
            .Where( x => x.Column is GanttCommandColumn<TItem> )
            .ToList();

        var orderedColumns = regularColumns
            .Where( x => IsWbsField( x.Column.Field ) )
            .OrderBy( x => GetColumnDisplayOrder( x.Column, x.Index ) )
            .ThenBy( x => x.Index )
            .Concat( regularColumns
                .Where( x => !IsWbsField( x.Column.Field ) )
                .OrderBy( x => GetColumnDisplayOrder( x.Column, x.Index ) )
                .ThenBy( x => x.Index ) )
            .Concat( commandColumns
                .OrderBy( x => GetColumnDisplayOrder( x.Column, x.Index ) )
                .ThenBy( x => x.Index ) )
            .Select( x => x.Column )
            .ToList();

        return orderedColumns;
    }

    private string GetColumnKey( BaseGanttColumn<TItem> column )
    {
        if ( column is null )
            return string.Empty;

        return columnKeys.TryGetValue( column, out var key )
            ? key
            : string.Empty;
    }

    private bool IsColumnVisible( BaseGanttColumn<TItem> column )
    {
        if ( column is null )
            return false;

        return columnVisibility.TryGetValue( column, out var visible )
            ? visible
            : column.Visible;
    }

    private void EnsureAtLeastOneDeclarativeColumnVisible()
    {
        if ( columns.Count == 0 )
            return;

        var regularColumns = GetOrderedColumns().Where( x => x is not GanttCommandColumn<TItem> ).ToList();

        if ( regularColumns.Count == 0 )
            return;

        if ( regularColumns.Any( IsColumnVisible ) )
            return;

        columnVisibility[regularColumns[0]] = true;
    }

    private string GetColumnHeaderText( BaseGanttColumn<TItem> column )
    {
        if ( column is null )
            return string.Empty;

        if ( !string.IsNullOrWhiteSpace( column.Title ) )
            return column.Title;

        if ( column is GanttCommandColumn<TItem> )
            return AddChildText;

        if ( IsWbsField( column.Field ) )
            return WbsColumnHeaderText;

        if ( StringUtils.IsMatch( column.Field, TitleField ) )
            return TaskColumnHeaderText;

        if ( StringUtils.IsMatch( column.Field, StartField ) )
            return StartColumnHeaderText;

        if ( StringUtils.IsMatch( column.Field, EndField ) )
            return EndColumnHeaderText;

        if ( StringUtils.IsMatch( column.Field, DurationField ) )
            return DurationColumnHeaderText;

        if ( StringUtils.IsMatch( column.Field, ProgressField ) )
            return ProgressColumnHeaderText;

        return column.Field ?? string.Empty;
    }

    private static bool IsWbsField( string field )
        => string.Equals( field, WbsPseudoField, StringComparison.OrdinalIgnoreCase )
            || string.Equals( field, "Wbs", StringComparison.OrdinalIgnoreCase );

    private TextAlignment ResolveColumnTextAlignment( BaseGanttColumn<TItem> column, bool isStart, bool isEnd, bool isDuration, bool isProgress )
    {
        if ( column.TextAlignment != TextAlignment.Default )
            return column.TextAlignment;

        if ( isStart || isEnd || isDuration || isProgress )
            return TextAlignment.Center;

        return TextAlignment.Default;
    }

    private double ResolveColumnWidth( BaseGanttColumn<TItem> column, IReadOnlyList<GanttTreeRow> visibleRows, IReadOnlyDictionary<string, string> wbsLookup )
    {
        if ( column is null )
            return DateColumnWidth;

        if ( TryGetPixelWidth( column.Width, out var fixedWidth ) )
            return fixedWidth;

        if ( column is GanttCommandColumn<TItem> )
            return ActionColumnWidth;

        if ( column.Expandable )
            return TitleColumnWidth;

        if ( IsWbsField( column.Field ) )
            return GetAutoSizedTreeColumnWidth( visibleRows, GetColumnHeaderText( column ), row => GetWbsValue( wbsLookup, row.Key ) );

        if ( StringUtils.IsMatch( column.Field, StartField ) )
            return GetAutoSizedTreeColumnWidth( visibleRows, GetColumnHeaderText( column ), row => FormatDate( GetItemStart( row.Item ) ), column.CanSort() && Sortable );

        if ( StringUtils.IsMatch( column.Field, EndField ) )
            return GetAutoSizedTreeColumnWidth( visibleRows, GetColumnHeaderText( column ), row => FormatDate( GetItemEnd( row.Item ) ), column.CanSort() && Sortable );

        if ( StringUtils.IsMatch( column.Field, DurationField ) )
            return GetAutoSizedTreeColumnWidth( visibleRows, GetColumnHeaderText( column ), row => FormatDuration( GetItemDuration( row.Item ) ), column.CanSort() && Sortable );

        if ( StringUtils.IsMatch( column.Field, ProgressField ) )
            return GetAutoSizedTreeColumnWidth( visibleRows, GetColumnHeaderText( column ), row => FormatProgress( GetItemProgress( row.Item ) ), column.CanSort() && Sortable );

        return GetAutoSizedTreeColumnWidth( visibleRows, GetColumnHeaderText( column ), row =>
        {
            var value = column.GetValue( row.Item );
            return column.FormatDisplayValue( value );
        }, column.CanSort() && Sortable );
    }

    private bool TryGetPixelWidth( IFluentSizing width, out double pixelWidth )
    {
        pixelWidth = 0d;

        if ( !CssValueUtils.TryGetNumericStyleValue( width, StyleProvider, "width", out var widthUnit, out var widthValue ) )
            return false;

        if ( !string.Equals( widthUnit, "px", StringComparison.OrdinalIgnoreCase ) )
            return false;

        pixelWidth = widthValue;

        return pixelWidth > 0d;
    }

    private string ResolveColumnFieldFromSortField( string sortField )
    {
        if ( string.IsNullOrWhiteSpace( sortField ) )
            return null;

        var mappedColumn = columns.FirstOrDefault( x => string.Equals( x.GetSortField(), sortField, StringComparison.OrdinalIgnoreCase ) );

        if ( mappedColumn is not null && !string.IsNullOrWhiteSpace( mappedColumn.Field ) )
            return mappedColumn.Field;

        return sortField;
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
        return UseHierarchicalData
            ? BuildHierarchicalTree()
            : BuildFlatTree();
    }

    private List<GanttTreeNode> BuildFlatTree()
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

            var idKey = ValueUtils.NormalizeIdentifier( propertyMapper.HasId ? propertyMapper.GetId( item ) : null );
            var uniqueKey = CreateUniqueNodeKey( item, index, usedKeys );

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
                ? ValueUtils.NormalizeIdentifier( propertyMapper.GetParentId( node.Item ) )
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

    private List<GanttTreeNode> BuildHierarchicalTree()
    {
        var roots = new List<GanttTreeNode>();
        var usedKeys = new HashSet<string>( StringComparer.Ordinal );
        var recursionGuard = new HashSet<object>( ReferenceEqualityComparer.Instance );
        var items = Data ?? Array.Empty<TItem>();
        var index = 0;

        foreach ( var item in items )
        {
            var node = BuildHierarchicalNode( item, ref index, usedKeys, recursionGuard );

            if ( node is not null )
                roots.Add( node );
        }

        foreach ( var root in roots )
        {
            AssignLevels( root, 0, new HashSet<string>( StringComparer.Ordinal ) );
        }

        return roots;
    }

    private GanttTreeNode BuildHierarchicalNode( TItem item, ref int index, HashSet<string> usedKeys, ISet<object> recursionGuard )
    {
        if ( item is null )
        {
            index++;
            return null;
        }

        if ( !recursionGuard.Add( item ) )
            return null;

        var node = new GanttTreeNode( CreateUniqueNodeKey( item, index, usedKeys ), item );
        index++;

        foreach ( var childItem in propertyMapper.GetItems( item ) )
        {
            var childNode = BuildHierarchicalNode( childItem, ref index, usedKeys, recursionGuard );

            if ( childNode is not null )
                node.Children.Add( childNode );
        }

        recursionGuard.Remove( item );
        return node;
    }

    private string CreateUniqueNodeKey( TItem item, int index, HashSet<string> usedKeys )
    {
        var idKey = ValueUtils.NormalizeIdentifier( propertyMapper.HasId ? propertyMapper.GetId( item ) : null );
        var stableKey = !string.IsNullOrEmpty( idKey )
            ? idKey
            : $"idx-{index.ToString( CultureInfo.InvariantCulture )}";

        var uniqueKey = stableKey;
        var suffix = 1;

        while ( !usedKeys.Add( uniqueKey ) )
        {
            uniqueKey = $"{stableKey}-{suffix.ToString( CultureInfo.InvariantCulture )}";
            suffix++;
        }

        return uniqueKey;
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
        var comparison = CompareBySortField( x.Item, y.Item, ganttSortField );

        if ( ganttSortDirection == SortDirection.Descending )
            comparison = -comparison;

        if ( comparison != 0 )
            return comparison;

        return CompareByDefaultSort( x, y );
    }

    private int CompareBySortField( TItem x, TItem y, string sortField )
    {
        if ( string.IsNullOrWhiteSpace( sortField ) )
            return 0;

        if ( StringUtils.IsMatch( sortField, TitleField ) )
            return StringComparer.OrdinalIgnoreCase.Compare( GetItemTitle( x ), GetItemTitle( y ) );

        if ( StringUtils.IsMatch( sortField, StartField ) )
            return DateTime.Compare( GetItemStart( x ), GetItemStart( y ) );

        if ( StringUtils.IsMatch( sortField, EndField ) )
            return DateTime.Compare( GetItemEnd( x ), GetItemEnd( y ) );

        if ( StringUtils.IsMatch( sortField, DurationField ) )
            return Nullable.Compare( GetItemDuration( x ), GetItemDuration( y ) );

        if ( StringUtils.IsMatch( sortField, ProgressField ) )
            return Nullable.Compare( GetItemProgress( x ), GetItemProgress( y ) );

        var mappedColumn = columns.FirstOrDefault( column => string.Equals( column.GetSortField(), sortField, StringComparison.OrdinalIgnoreCase ) );

        if ( mappedColumn is null )
            return 0;

        var xValue = mappedColumn.GetSortValue( x );
        var yValue = mappedColumn.GetSortValue( y );

        return ValueUtils.CompareValues( xValue, yValue );
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

    private string GetTreePaneStyle( IReadOnlyList<GanttRenderColumn> treeColumns )
    {
        var width = NormalizeTreeListWidth( treeListWidthOverride )
            ?? new FluentUnitValue( "px", GetTreePaneWidth( treeColumns ) );
        var widthText = width.ToCssValue();

        if ( string.IsNullOrWhiteSpace( widthText ) )
            widthText = new FluentUnitValue( "px", GetTreePaneWidth( treeColumns ) ).ToCssValue();

        return $"display: flex; flex-direction: column; width: {widthText}; min-width: {widthText}; max-width: {widthText}; overflow: hidden;";
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

    private string GetTreeColumnStyle( double width, bool sortable = false, TextAlignment textAlignment = TextAlignment.Default )
    {
        var widthText = width.ToString( "0.###", CultureInfo.InvariantCulture );
        var textAlign = textAlignment switch
        {
            TextAlignment.Center => "center",
            TextAlignment.End => "right",
            TextAlignment.Justified => "justify",
            _ => "left",
        };

        return $"width: {widthText}px; min-width: {widthText}px; max-width: {widthText}px; overflow: hidden; cursor: {( sortable ? "pointer" : "default" )}; text-align: {textAlign};";
    }

    private string GetActionColumnStyle( double width )
    {
        var widthText = width.ToString( "0.###", CultureInfo.InvariantCulture );
        return $"display: flex; align-items: center; justify-content: center; width: {widthText}px; min-width: {widthText}px; max-width: {widthText}px; overflow: hidden; border-left: 1px solid rgba(0,0,0,0.08);";
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

    private string GetTimelineHeaderStyle( int slotsCount, double cellWidth )
    {
        var style = "position: relative;";
        var paddedZoneBackground = GetTimelinePaddedZoneBackground( slotsCount, cellWidth );

        if ( !string.IsNullOrWhiteSpace( paddedZoneBackground ) )
            style = $"{style} background-image: {paddedZoneBackground};";

        return style;
    }

    private string GetTimelineRowStyle( double rowHeight, int slotsCount, double cellWidth )
    {
        var rowHeightText = rowHeight.ToString( "0.###", CultureInfo.InvariantCulture );
        var cellWidthText = cellWidth.ToString( "0.###", CultureInfo.InvariantCulture );
        var paddedZoneBackground = GetTimelinePaddedZoneBackground( slotsCount, cellWidth );
        var rowGridBackground = $"repeating-linear-gradient(to right, rgba(0,0,0,0.07), rgba(0,0,0,0.07) 1px, transparent 1px, transparent {cellWidthText}px)";

        if ( !string.IsNullOrWhiteSpace( paddedZoneBackground ) )
            rowGridBackground = $"{paddedZoneBackground}, {rowGridBackground}";

        return $"position: relative; width: 100%; min-width: 100%; height: {rowHeightText}px; min-height: {rowHeightText}px; border-bottom: 1px solid rgba(0,0,0,0.08); background-image: {rowGridBackground};";
    }

    private string GetTimelinePaddedZoneBackground( int slotsCount, double cellWidth )
    {
        if ( slotsCount <= 0 || cellWidth <= 0d )
            return null;

        var (leadingSlots, trailingSlots) = GetTimelinePaddedZoneSlots( slotsCount );

        if ( leadingSlots <= 0 && trailingSlots <= 0 )
            return null;

        var totalWidth = Math.Max( 1, slotsCount ) * cellWidth;
        var leadingWidth = leadingSlots * cellWidth;
        var trailingWidth = trailingSlots * cellWidth;
        var trailingStart = totalWidth - trailingWidth;

        var layers = new List<string>();

        if ( leadingSlots > 0 )
        {
            var leadingWidthText = leadingWidth.ToString( "0.###", CultureInfo.InvariantCulture );
            layers.Add( $"linear-gradient(to right, {TimelinePaddedSlotBackgroundColor} 0px, {TimelinePaddedSlotBackgroundColor} {leadingWidthText}px, transparent {leadingWidthText}px, transparent 100%)" );

            if ( leadingWidth > 0d && leadingWidth < totalWidth )
                layers.Insert( 0, BuildTimelineBoundaryLayer( leadingWidth ) );
        }

        if ( trailingSlots > 0 )
        {
            var trailingStartText = trailingStart.ToString( "0.###", CultureInfo.InvariantCulture );
            layers.Add( $"linear-gradient(to right, transparent 0px, transparent {trailingStartText}px, {TimelinePaddedSlotBackgroundColor} {trailingStartText}px, {TimelinePaddedSlotBackgroundColor} 100%)" );

            if ( trailingStart > 0d
                 && trailingStart < totalWidth
                 && ( layers.Count == 1 || Math.Abs( trailingStart - leadingWidth ) > 0.5d ) )
                layers.Insert( 0, BuildTimelineBoundaryLayer( trailingStart ) );
        }

        return string.Join( ", ", layers );
    }

    private (int LeadingSlots, int TrailingSlots) GetTimelinePaddedZoneSlots( int slotsCount )
    {
        if ( slotsCount <= 0 )
            return (0, 0);

        var leadingSlots = GetCurrentViewLeadingSlots();
        var trailingSlots = GetCurrentViewTrailingSlots();

        leadingSlots = Math.Min( Math.Max( 0, leadingSlots ), slotsCount );
        trailingSlots = Math.Min( Math.Max( 0, trailingSlots ), Math.Max( 0, slotsCount - leadingSlots ) );

        return (leadingSlots, trailingSlots);
    }

    private static string BuildTimelineBoundaryLayer( double offset )
    {
        var offsetText = offset.ToString( "0.###", CultureInfo.InvariantCulture );
        var nextOffsetText = ( offset + 1d ).ToString( "0.###", CultureInfo.InvariantCulture );

        return $"linear-gradient(to right, transparent 0px, transparent {offsetText}px, {TimelineAnchorBoundaryColor} {offsetText}px, {TimelineAnchorBoundaryColor} {nextOffsetText}px, transparent {nextOffsetText}px, transparent 100%)";
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

        var style = $"position: absolute; left: {barInfo.Left.ToString( "0.###", CultureInfo.InvariantCulture )}px; width: {barInfo.Width.ToString( "0.###", CultureInfo.InvariantCulture )}px; top: {top.ToString( "0.###", CultureInfo.InvariantCulture )}px; height: {barHeight.ToString( "0.###", CultureInfo.InvariantCulture )}px; display: flex; align-items: center; cursor: {( CanDragItem( item ) ? "grab" : "pointer" )}; user-select: none; -webkit-user-select: none;";

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

    private GanttViewRange GetCurrentAnchorRange()
    {
        if ( SelectedView == GanttView.Week )
        {
            var start = currentDate.StartOfWeek( EffectiveFirstDayOfWeek ).ToDateTime( TimeOnly.MinValue );
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

    private GanttViewRange GetCurrentViewRange()
    {
        var anchorRange = GetCurrentAnchorRange();
        var leadingSlots = GetCurrentViewLeadingSlots();
        var trailingSlots = GetCurrentViewTrailingSlots();

        if ( leadingSlots == 0 && trailingSlots == 0 )
            return anchorRange;

        var start = ShiftDateBySlotOffset( anchorRange.Start, -leadingSlots );
        var end = ShiftDateBySlotOffset( anchorRange.End, trailingSlots );

        if ( end <= start )
            end = anchorRange.End;

        return new GanttViewRange( start, end );
    }

    private int GetCurrentViewLeadingSlots()
    {
        var view = ActiveView;

        if ( view is null || view.LeadingSlots <= 0 )
            return 0;

        return view.LeadingSlots;
    }

    private int GetCurrentViewTrailingSlots()
    {
        var view = ActiveView;

        if ( view is null || view.TrailingSlots <= 0 )
            return 0;

        return view.TrailingSlots;
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

    private static double GetTreePaneWidth( IReadOnlyList<GanttRenderColumn> treeColumns )
    {
        if ( treeColumns is null || treeColumns.Count == 0 )
            return 1d;

        var width = treeColumns.Sum( x => x.Width );

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

        if ( UseHierarchicalData )
        {
            TryRemoveHierarchicalItem( data, itemToDelete, new HashSet<object>( ReferenceEqualityComparer.Instance ) );
            return;
        }

        if ( !propertyMapper.HasId )
        {
            data.Remove( itemToDelete );
            return;
        }

        var dataSnapshot = data.ToList();
        var itemId = ValueUtils.NormalizeIdentifier( propertyMapper.GetId( itemToDelete ) );

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
                var dataItemId = ValueUtils.NormalizeIdentifier( propertyMapper.GetId( dataItem ) );
                var dataItemParentId = propertyMapper.HasParentId
                    ? ValueUtils.NormalizeIdentifier( propertyMapper.GetParentId( dataItem ) )
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
            var dataItemId = ValueUtils.NormalizeIdentifier( propertyMapper.GetId( dataItem ) );

            if ( !string.IsNullOrEmpty( dataItemId ) && idsToDelete.Contains( dataItemId ) )
            {
                data.Remove( dataItem );
            }
        }
    }

    private bool TryRemoveHierarchicalItem( ICollection<TItem> items, TItem itemToDelete, ISet<object> recursionGuard )
    {
        if ( items is null || itemToDelete is null )
            return false;

        var snapshot = items.ToList();

        foreach ( var item in snapshot )
        {
            if ( item is null )
                continue;

            if ( AreRowsEqual( item, itemToDelete ) )
            {
                items.Remove( item );
                return true;
            }

            if ( !recursionGuard.Add( item ) )
                continue;

            try
            {
                var childItems = propertyMapper.GetItemsCollection( item );

                if ( childItems is not null
                     && TryRemoveHierarchicalItem( childItems, itemToDelete, recursionGuard ) )
                {
                    return true;
                }
            }
            finally
            {
                recursionGuard.Remove( item );
            }
        }

        return false;
    }

    private TItem ResolveStateItemReference( TItem stateItem )
    {
        if ( stateItem is null )
            return default;

        if ( !propertyMapper.HasId )
            return stateItem;

        var stateItemId = ValueUtils.NormalizeIdentifier( propertyMapper.GetId( stateItem ) );

        if ( string.IsNullOrWhiteSpace( stateItemId ) )
            return stateItem;

        foreach ( var dataItem in EnumerateAllDataItems() )
        {
            if ( dataItem is null )
                continue;

            var dataItemId = ValueUtils.NormalizeIdentifier( propertyMapper.GetId( dataItem ) );

            if ( string.Equals( dataItemId, stateItemId, StringComparison.Ordinal ) )
                return dataItem;
        }

        return stateItem;
    }

    private IEnumerable<TItem> EnumerateAllDataItems()
    {
        if ( !UseHierarchicalData )
            return Data ?? Array.Empty<TItem>();

        return EnumerateHierarchicalDataItems( Data ?? Array.Empty<TItem>() );
    }

    private IEnumerable<TItem> EnumerateHierarchicalDataItems( IEnumerable<TItem> rootItems )
    {
        if ( rootItems is null )
            yield break;

        var recursionGuard = new HashSet<object>( ReferenceEqualityComparer.Instance );

        foreach ( var item in rootItems )
        {
            foreach ( var nestedItem in EnumerateHierarchicalDataItems( item, recursionGuard ) )
            {
                yield return nestedItem;
            }
        }
    }

    private IEnumerable<TItem> EnumerateHierarchicalDataItems( TItem item, ISet<object> recursionGuard )
    {
        if ( item is null || !recursionGuard.Add( item ) )
            yield break;

        yield return item;

        foreach ( var childItem in propertyMapper.GetItems( item ) )
        {
            foreach ( var nestedItem in EnumerateHierarchicalDataItems( childItem, recursionGuard ) )
            {
                yield return nestedItem;
            }
        }

        recursionGuard.Remove( item );
    }

    private bool AreRowsEqual( TItem first, TItem second )
    {
        if ( ReferenceEquals( first, second ) )
            return true;

        if ( first is null || second is null )
            return false;

        if ( propertyMapper.HasId )
        {
            var firstId = ValueUtils.NormalizeIdentifier( propertyMapper.GetId( first ) );
            var secondId = ValueUtils.NormalizeIdentifier( propertyMapper.GetId( second ) );

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

        if ( !propertyMapper.HasId )
            return false;

        var selectedRowId = ValueUtils.NormalizeIdentifier( propertyMapper.GetId( SelectedRow ) );
        var deleteRowId = ValueUtils.NormalizeIdentifier( propertyMapper.GetId( itemToDelete ) );

        if ( string.IsNullOrEmpty( selectedRowId ) || string.IsNullOrEmpty( deleteRowId ) )
            return false;

        if ( string.Equals( selectedRowId, deleteRowId, StringComparison.Ordinal ) )
            return true;

        var parentById = BuildParentLookup();

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

    private Dictionary<string, string> BuildParentLookup()
    {
        var parentById = new Dictionary<string, string>( StringComparer.Ordinal );

        if ( !propertyMapper.HasId )
            return parentById;

        foreach ( var root in BuildTree() )
        {
            AppendParentLookup( root, null, parentById, new HashSet<string>( StringComparer.Ordinal ) );
        }

        return parentById;
    }

    private void AppendParentLookup( GanttTreeNode node, string parentId, IDictionary<string, string> parentById, ISet<string> recursionGuard )
    {
        if ( node is null || !recursionGuard.Add( node.Key ) )
            return;

        var nodeId = ValueUtils.NormalizeIdentifier( propertyMapper.GetId( node.Item ) );

        if ( !string.IsNullOrEmpty( nodeId ) && !parentById.ContainsKey( nodeId ) )
        {
            parentById.Add( nodeId, parentId );
            parentId = nodeId;
        }

        foreach ( var childNode in node.Children )
        {
            AppendParentLookup( childNode, parentId, parentById, recursionGuard );
        }

        recursionGuard.Remove( node.Key );
    }

    #endregion

    #region Properties

    private bool IsSearchMode
        => !string.IsNullOrWhiteSpace( searchText );

    private bool UseHierarchicalData
        => propertyMapper?.HasItems == true
           && ( HierarchicalData || !propertyMapper.HasParentId );

    private DayOfWeek EffectiveFirstDayOfWeek
        => SelectedView == GanttView.Week
           && ganttWeekView?.FirstDayOfWeek is DayOfWeek weekViewFirstDayOfWeek
            ? weekViewFirstDayOfWeek
            : FirstDayOfWeek;

    private bool ShowToolbarAddTaskButton
        => IsCommandAllowed( GanttCommandType.New );

    private bool ShowHeaderNewButton
        => IsCommandAllowed( GanttCommandType.New );

    private bool ShowAddChildColumn
        => Editable && AddChildCommandAllowed;

    private bool ProgressColumnAvailable
        => propertyMapper?.HasProgress == true
           && !string.IsNullOrWhiteSpace( ProgressField );

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

    private string ProgressColumnHeaderText
        => Localizer.Localize( Localizers?.ProgressLocalizer, LocalizationConstants.Progress );

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
    /// Gets whether a new item can be inserted into <see cref="Data"/> or mapped child collection.
    /// </summary>
    protected bool CanInsertNewItem
        => Editable
           && UseInternalEditing
           && ( UseHierarchicalData && editParentItem is not null
               ? propertyMapper.CanSetItems
                 || propertyMapper.GetItemsCollection( ResolveStateItemReference( editParentItem ) ) is not null
               : Data is ICollection<TItem> );

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
    /// Gets or sets child item collection field name.
    /// </summary>
    [Parameter] public string ItemsField { get; set; } = "Items";

    /// <summary>
    /// Gets or sets whether hierarchical data mode is forced when both parent-id and items mapping exist.
    /// </summary>
    [Parameter] public bool HierarchicalData { get; set; }

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
    /// Gets or sets whether built-in modal editing and internal data mutation are used.
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
    /// Gets or sets callback invoked when the new-item command is triggered.
    /// </summary>
    [Parameter] public EventCallback<GanttCommandContext<TItem>> NewItemClicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the add-child command is triggered.
    /// </summary>
    [Parameter] public EventCallback<GanttCommandContext<TItem>> AddChildItemClicked { get; set; }

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

    private sealed class GanttRenderColumn
    {
        public GanttRenderColumn( string key, BaseGanttColumn<TItem> column, GanttCommandColumn<TItem> commandColumn, string headerText, string sortField, bool canSort, double width, TextAlignment textAlignment, bool isCommand, bool isWbs, bool isStart, bool isEnd, bool isDuration, bool isProgress, bool isExpandable )
        {
            Key = key;
            Column = column;
            CommandColumn = commandColumn;
            HeaderText = headerText;
            SortField = sortField;
            CanSort = canSort;
            Width = width;
            TextAlignment = textAlignment;
            IsCommand = isCommand;
            IsWbs = isWbs;
            IsStart = isStart;
            IsEnd = isEnd;
            IsDuration = isDuration;
            IsProgress = isProgress;
            Expandable = isExpandable;
        }

        public string Key { get; }

        public BaseGanttColumn<TItem> Column { get; }

        public GanttCommandColumn<TItem> CommandColumn { get; }

        public string HeaderText { get; }

        public string SortField { get; }

        public bool CanSort { get; }

        public double Width { get; }

        public TextAlignment TextAlignment { get; }

        public bool IsCommand { get; }

        public bool IsWbs { get; }

        public bool IsStart { get; }

        public bool IsEnd { get; }

        public bool IsDuration { get; }

        public bool IsProgress { get; }

        public bool Expandable { get; }
    }

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