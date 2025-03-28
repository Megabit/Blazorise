#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.DeepCloner;
using Blazorise.Extensions;
using Blazorise.Infrastructure;
using Blazorise.Scheduler.Components;
using Blazorise.Scheduler.Extensions;
using Blazorise.Scheduler.Utilities;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents a scheduler component that manages and displays appointments, allowing navigation and editing of scheduled items.
/// </summary>
/// <typeparam name="TItem">Represents the type of items that the scheduler will manage, such as appointments or events.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public partial class Scheduler<TItem> : BaseComponent, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Provides the state of the <see cref="Scheduler"/> component.
    /// </summary>
    private SchedulerState state = new();

    private SchedulerToolbar<TItem> schedulerToolbar;
    private SchedulerDayView<TItem> schedulerDayView;
    private SchedulerWeekView<TItem> schedulerWeekView;
    private SchedulerWorkWeekView<TItem> schedulerWorkWeekView;
    private SchedulerMonthView<TItem> schedulerMonthView;

    private readonly EventCallbackSubscriber prevDaySubscriber;
    private readonly EventCallbackSubscriber nextDaySubscriber;
    private readonly EventCallbackSubscriber todaySubscriber;
    private readonly EventCallbackSubscriber dayViewSubscriber;
    private readonly EventCallbackSubscriber weekViewSubscriber;
    private readonly EventCallbackSubscriber workWeekViewSubscriber;
    private readonly EventCallbackSubscriber monthViewSubscriber;

    private Func<TItem, DateTime, DateTime, bool> searchPredicate;

    private Func<TItem, object> getIdFunc;

    private Func<TItem, string> getTitleFunc;
    private Action<TItem, object> setTitleFunc;

    private Func<TItem, string> getDescriptionFunc;
    private Action<TItem, object> setDescriptionFunc;

    private Func<TItem, object> getStartFunc;
    private Action<TItem, object> setStartFunc;

    private Func<TItem, object> getEndFunc;
    private Action<TItem, object> setEndFunc;

    private Func<TItem, bool> getAllDayFunc;
    private Action<TItem, object> setAllDayFunc;

    private Lazy<Func<TItem>> newItemCreator;

    protected _SchedulerModal<TItem> schedulerModalRef;

    protected TItem editItem;

    protected SchedulerEditState editState = SchedulerEditState.None;

    #endregion

    #region Constructors

    /// <summary>
    /// Default <see cref="Scheduler"/> constructor.
    /// </summary>
    public Scheduler()
    {
        prevDaySubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, NavigatePrevious ) );
        nextDaySubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, NavigateNext ) );
        todaySubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, NavigateToday ) );
        dayViewSubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, ShowDayView ) );
        weekViewSubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, ShowWeekView ) );
        workWeekViewSubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, ShowWorkWeekView ) );
        monthViewSubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, ShowMonthView ) );

        searchPredicate = SchedulerExpressionCompiler.BuildSearchPredicate<TItem>( StartField, EndField );
        getIdFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem>( IdField );
        getTitleFunc = SchedulerExpressionCompiler.BuildGetStringFunc<TItem>( TitleField );
        setTitleFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem>( TitleField );
        getDescriptionFunc = SchedulerExpressionCompiler.BuildGetStringFunc<TItem>( DescriptionField );
        setDescriptionFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem>( DescriptionField );
        getStartFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem>( StartField );
        setStartFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem>( StartField );
        getEndFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem>( EndField );
        setEndFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem>( EndField );
        getAllDayFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, bool>( AllDayField );
        setAllDayFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem>( AllDayField );
        newItemCreator = new( () => SchedulerFunctionCompiler.CreateNewItem<TItem>() );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnParametersSetAsync()
    {
        prevDaySubscriber.SubscribeOrReplace( State?.PrevDayRequested );
        nextDaySubscriber.SubscribeOrReplace( State?.NextDayRequested );
        todaySubscriber.SubscribeOrReplace( State?.TodayRequested );
        dayViewSubscriber.SubscribeOrReplace( State?.DayViewRequested );
        weekViewSubscriber.SubscribeOrReplace( State?.WeekViewRequested );
        workWeekViewSubscriber.SubscribeOrReplace( State?.WorkWeekViewRequested );
        monthViewSubscriber.SubscribeOrReplace( State?.MonthViewRequested );

        return base.OnParametersSetAsync();
    }

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var dateChanged = parameters.TryGetValue<DateOnly>( nameof( Date ), out var paramDate ) && !state.SelectedDate.IsEqual( paramDate );

        if ( dateChanged )
        {
            state.SelectedDate = paramDate;
        }

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    override protected void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-scheduler" );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            prevDaySubscriber?.Dispose();
            nextDaySubscriber?.Dispose();
            todaySubscriber?.Dispose();
            dayViewSubscriber?.Dispose();
            weekViewSubscriber?.Dispose();
            workWeekViewSubscriber?.Dispose();
            monthViewSubscriber?.Dispose();
        }

        await base.DisposeAsync( disposing );
    }

    internal void NotifySchedulerToolbar( SchedulerToolbar<TItem> schedulerToolbar )
    {
        this.schedulerToolbar = schedulerToolbar;
    }

    internal void NotifySchedulerDayView( SchedulerDayView<TItem> schedulerDayView )
    {
        this.schedulerDayView = schedulerDayView;
    }

    internal void NotifySchedulerWeekView( SchedulerWeekView<TItem> schedulerWeekView )
    {
        this.schedulerWeekView = schedulerWeekView;
    }

    internal void NotifySchedulerWorkWeekView( SchedulerWorkWeekView<TItem> schedulerWorkWeekView )
    {
        this.schedulerWorkWeekView = schedulerWorkWeekView;
    }

    internal void NotifySchedulerMonthView( SchedulerMonthView<TItem> schedulerMonthView )
    {
        this.schedulerMonthView = schedulerMonthView;
    }

    /// <summary>
    /// Navigates to the previous date.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task NavigatePrevious()
    {
        if ( SelectedView == SchedulerView.Week && schedulerWeekView is not null )
        {
            state.SelectedDate = WeekNavigationMode == SchedulerWeekNavigationMode.FirstDayOfWeek
                ? state.SelectedDate.StartOfPreviousWeek( schedulerWeekView.FirstDayOfWeek )
                : state.SelectedDate.AddDays( -7 );
        }
        else if ( SelectedView == SchedulerView.WorkWeek && schedulerWorkWeekView is not null )
        {
            state.SelectedDate = WeekNavigationMode == SchedulerWeekNavigationMode.FirstDayOfWeek
                ? state.SelectedDate.StartOfPreviousWeek( schedulerWorkWeekView.FirstDayOfWorkWeek )
                : state.SelectedDate.AddDays( -7 );
        }
        else if ( SelectedView == SchedulerView.Month && schedulerMonthView is not null )
        {
            state.SelectedDate = state.SelectedDate.AddMonths( -1 );
        }
        else
        {
            state.SelectedDate = state.SelectedDate.AddDays( -1 );
        }

        await DateChanged.InvokeAsync( state.SelectedDate );
        await InvokeAsync( StateHasChanged );
    }


    /// <summary>
    /// Navigates to the next date.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task NavigateNext()
    {
        if ( SelectedView == SchedulerView.Week && schedulerWeekView is not null )
        {
            state.SelectedDate = WeekNavigationMode == SchedulerWeekNavigationMode.FirstDayOfWeek
                ? state.SelectedDate.StartOfNextWeek( schedulerWeekView.FirstDayOfWeek )
                : state.SelectedDate.AddDays( 7 );
        }
        else if ( SelectedView == SchedulerView.WorkWeek && schedulerWorkWeekView is not null )
        {
            state.SelectedDate = WeekNavigationMode == SchedulerWeekNavigationMode.FirstDayOfWeek
                ? state.SelectedDate.StartOfNextWeek( schedulerWorkWeekView.FirstDayOfWorkWeek )
                : state.SelectedDate.AddDays( 7 );
        }
        else if ( SelectedView == SchedulerView.Month && schedulerMonthView is not null )
        {
            state.SelectedDate = state.SelectedDate.AddMonths( 1 );
        }
        else
        {
            state.SelectedDate = state.SelectedDate.AddDays( 1 );
        }

        await DateChanged.InvokeAsync( state.SelectedDate );
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Navigates to the current date.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task NavigateToday()
    {
        state.SelectedDate = DateOnly.FromDateTime( DateTime.Today );
        await DateChanged.InvokeAsync( state.SelectedDate );
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Sets the selected view to 'Day' and triggers an update to reflect this change.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ShowDayView()
    {
        SelectedView = SchedulerView.Day;
        await SelectedViewChanged.InvokeAsync( SelectedView );
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Sets the selected view to 'Week' and triggers an update to reflect this change.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ShowWeekView()
    {
        SelectedView = SchedulerView.Week;
        await SelectedViewChanged.InvokeAsync( SelectedView );
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Sets the selected view to 'WorkWeek' and triggers an update to reflect this change.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ShowWorkWeekView()
    {
        SelectedView = SchedulerView.WorkWeek;
        await SelectedViewChanged.InvokeAsync( SelectedView );
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Sets the selected view to 'Month' and triggers an update to reflect this change.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ShowMonthView()
    {
        SelectedView = SchedulerView.Month;
        await SelectedViewChanged.InvokeAsync( SelectedView );
        await InvokeAsync( StateHasChanged );
    }

    internal IEnumerable<TItem> ItemsInRange( DateTime start, DateTime end )
    {
        return Data?.Where( x => searchPredicate( x, start, end ) );
    }

    /// <summary>
    /// Gets the title of the specified appointment.
    /// </summary>
    /// <param name="item">The appointment to get the title from.</param>
    /// <returns>The title of the appointment.</returns>
    internal string GetItemTitle( TItem item )
    {
        return getTitleFunc( item );
    }

    /// <summary>
    /// Calculates the duration of an appointment based on its start and end times.
    /// </summary>
    /// <param name="item">Represents an appointment from which the start and end times are derived.</param>
    /// <returns>Returns the duration as a TimeSpan, or zero if the times are not valid DateTime values.</returns>
    internal TimeSpan GetItemDuration( TItem item )
    {
        var start = getStartFunc( item );
        var end = getEndFunc( item );

        if ( start is DateTime startDateTime && end is DateTime endDateTime )
        {
            return endDateTime - startDateTime;
        }

        return TimeSpan.Zero;
    }

    /// <summary>
    /// Gets the appointment start time.
    /// </summary>
    /// <param name="item">An item from which the start time is derived.</param>
    /// <returns>Returns the start time of the item.</returns>
    internal DateTime GetItemStartTime( TItem item )
    {
        return getStartFunc( item ) as DateTime? ?? DateTime.MinValue;
    }

    /// <summary>
    /// Gets the appointment end time.
    /// </summary>
    /// <param name="item">An item from which the end time is derived.</param>
    /// <returns>Returns the end time of the item.</returns>
    internal DateTime GetItemEndTime( TItem item )
    {
        return getEndFunc( item ) as DateTime? ?? DateTime.MaxValue;
    }

    /// <summary>
    /// Gets the appointment all day value.
    /// </summary>
    /// <param name="item">An item from which the all-day value is derived.</param>
    /// <returns>Returns the all-day value of the item.</returns>
    internal bool GetItemAllDay( TItem item )
    {
        return getAllDayFunc( item );
    }

    /// <summary>
    /// Gets the description of the specified appointment.
    /// </summary>
    /// <param name="item">The appointment to get the description from.</param>
    /// <returns>The description of the appointment.</returns>
    internal string GetItemDescription( TItem item )
    {
        return getDescriptionFunc( item );
    }

    /// <summary>
    /// Sets the starting value for a specified item using a provided value.
    /// </summary>
    /// <param name="item">Specifies the item for which the starting value is being set.</param>
    /// <param name="value">Provides the value to be assigned as the starting point for the specified item.</param>
    internal void SetItemStart( TItem item, object value )
    {
        setStartFunc( item, value );
    }

    /// <summary>
    /// Sets the end value for a specified item using a provided function.
    /// </summary>
    /// <param name="item">Specifies the item for which the end value is being set.</param>
    /// <param name="value">Provides the value to be assigned to the end of the specified item.</param>
    internal void SetItemEnd( TItem item, object value )
    {
        setEndFunc( item, value );
    }

    /// <summary>
    /// Sets the start and end dates for a specified item.
    /// </summary>
    /// <param name="item">Specifies the item for which the dates are being set.</param>
    /// <param name="start">Indicates the starting date or time for the item.</param>
    /// <param name="end">Indicates the ending date or time for the item.</param>
    internal void SetItemDates( TItem item, object start, object end )
    {
        SetItemStart( item, start );
        SetItemEnd( item, end );
    }

    internal async Task NotifyItemClicked( TItem item )
    {
        await Edit( item );

        await ItemClicked.InvokeAsync( new( item ) );
    }

    internal async Task NotifySlotClicked( DateTime start, DateTime end )
    {
        editItem = Data.FirstOrDefault( x => searchPredicate( x, start, end ) );

        if ( editItem is null )
        {
            editItem = CreateNewItem();

            SetItemDates( editItem, start, end );

            await New( editItem );
        }
        else
        {
            await Edit( editItem );
        }

        await SlotClicked.InvokeAsync( new SchedulerSlotClickedEventArgs( start, end ) );
    }

    /// <summary>
    /// Creates a new appointment by invoking the <see cref="New(TItem)"/> method with a newly created item.
    /// </summary>
    /// <returns>Returns a Task representing the asynchronous operation.</returns>
    public Task New()
    {
        return New( CreateNewItem() );
    }

    /// <summary>
    /// Creates a new item and sets the editing state to 'New'. If conditions are met, it displays a modal for editing the item.
    /// </summary>
    /// <param name="newItem">The item to be created and potentially edited in a modal.</param>
    /// <returns>Returns a Task representing the asynchronous operation.</returns>
    public async Task New( TItem newItem )
    {
        editItem = newItem;
        editState = SchedulerEditState.New;

        if ( Editable && UseInternalEditing && schedulerModalRef is not null )
        {
            await schedulerModalRef.ShowModal( newItem.DeepClone(), true );
        }
    }

    public async Task Edit( TItem item )
    {
        editItem = item;
        editState = SchedulerEditState.Edit;

        if ( Editable && UseInternalEditing && schedulerModalRef is not null )
        {
            await schedulerModalRef.ShowModal( item.DeepClone(), false );
        }
    }

    public Task Delete( TItem item )
    {
        if ( Editable && UseInternalEditing && Data is ICollection<TItem> data )
        {
            data.Remove( item );
        }

        return Task.CompletedTask;
    }

    protected internal async Task<bool> ModalSubmitedItem( TItem submitedItem )
    {
        var handler = editState == SchedulerEditState.New ? ItemInserting : ItemUpdating;

        var editItemClone = submitedItem.DeepClone();

        if ( await IsSafeToProceed( handler, editItem, editItemClone ) )
        {
            if ( UseInternalEditing && editState == SchedulerEditState.New && CanInsertNewItem && Data is ICollection<TItem> data )
            {
                data.Add( editItem );
            }

            if ( UseInternalEditing || editState == SchedulerEditState.New )
            {
                CopyItemValues( editItemClone, editItem );
            }

            if ( editState == SchedulerEditState.New )
            {
                await ItemInserted.InvokeAsync( new SchedulerInsertedItem<TItem>( editItemClone ) );
            }
            else
            {
                await ItemUpdated.InvokeAsync( new SchedulerUpdatedItem<TItem>( editItem, editItemClone ) );
            }

            editState = SchedulerEditState.None;

            await InvokeAsync( StateHasChanged );

            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if it is safe to proceed with an operation based on a callback's response.
    /// </summary>
    /// <param name="handler">Handles the event callback to determine if the operation should be canceled.</param>
    /// <param name="oldItem">Represents the item before the change occurs.</param>
    /// <param name="newItem">Represents the item after the change is proposed.</param>
    /// <returns>Returns true if the operation can proceed, false if it should be canceled.</returns>
    internal async Task<bool> IsSafeToProceed( EventCallback<SchedulerCancellableItemChange<TItem>> handler, TItem oldItem, TItem newItem )
    {
        if ( handler.HasDelegate )
        {
            var args = new SchedulerCancellableItemChange<TItem>( oldItem, newItem );

            await handler.InvokeAsync( args );

            if ( args.Cancel )
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Creates a new item using a specified item creator if available; otherwise, it uses a default item creator.
    /// </summary>
    /// <returns>Returns a new instance of TItem.</returns>
    private TItem CreateNewItem()
       => NewItemCreator is not null ? NewItemCreator.Invoke() : newItemCreator.Value();

    /// <summary>
    /// Copies values from one appointment to another.
    /// </summary>
    /// <param name="source">The source appointment to copy values from.</param>
    /// <param name="destination">The destination appointment to copy values to.</param>
    private void CopyItemValues( TItem source, TItem destination )
    {
        setTitleFunc( destination, getTitleFunc( source ) );
        setDescriptionFunc( destination, getDescriptionFunc( source ) );
        setStartFunc( destination, getStartFunc( source ) );
        setEndFunc( destination, getEndFunc( source ) );
    }

    /// <summary>
    /// Retrieves all-day items within a specified date range. It filters items based on their start date.
    /// </summary>
    /// <param name="from">Specifies the start date of the range for filtering items.</param>
    /// <param name="to">Specifies the end date of the range for filtering items.</param>
    /// <returns>An enumerable collection of items that are all-day events within the given date range.</returns>
    internal IEnumerable<TItem> GetAllDayItemsInRange( DateOnly from, DateOnly to )
    {
        return Data?.Where( x => GetItemAllDay( x )
            && GetItemStartTime( x ).Date >= from.ToDateTime( TimeOnly.MinValue )
            && GetItemStartTime( x ).Date <= to.ToDateTime( TimeOnly.MaxValue ) );
    }

    /// <summary>
    /// Calculates the duration of an all-day item in days based on its start and end times.
    /// </summary>
    /// <param name="item">Represents the item for which the duration is being calculated.</param>
    /// <returns>Returns the total number of days the item spans, including both start and end dates.</returns>
    internal int GetAllDayItemDurationInDays( TItem item )
    {
        var start = GetItemStartTime( item );
        var end = GetItemEndTime( item );

        if ( start == DateTime.MinValue || end == DateTime.MaxValue )
        {
            return 0;
        }

        return ( end.Date - start.Date ).Days + 1;
    }

    /// <summary>
    /// Calculates the maximum number of overlapping all-day items within a specified date range.
    /// </summary>
    /// <param name="from">Specifies the start date of the range for which to calculate overlapping items.</param>
    /// <param name="to">Specifies the end date of the range for which to calculate overlapping items.</param>
    /// <returns>Returns the maximum count of overlapping all-day items found within the given date range.</returns>
    internal int GetMaxOverlappingAllDayItems( DateOnly from, DateOnly to )
    {
        int maxItems = 0;

        for ( var date = from; date <= to; date = date.AddDays( 1 ) )
        {
            var items = Data?.Where( x => GetItemAllDay( x ) &&
                GetItemStartTime( x ).Date <= date.ToDateTime( TimeOnly.MaxValue ) &&
                GetItemEndTime( x ).Date >= date.ToDateTime( TimeOnly.MinValue ) ).ToList();

            if ( items != null )
            {
                int overlappingCount = 0;
                foreach ( var item in items )
                {
                    int currentOverlap = items.Count( x =>
                        GetItemStartTime( x ).Date <= GetItemEndTime( item ).Date &&
                        GetItemEndTime( x ).Date >= GetItemStartTime( item ).Date );

                    if ( currentOverlap > overlappingCount )
                    {
                        overlappingCount = currentOverlap;
                    }
                }

                if ( overlappingCount > maxItems )
                {
                    maxItems = overlappingCount;
                }
            }
        }

        return maxItems;
    }

    internal int CountOverlappingItemsBefore( TItem item )
    {
        var items = Data?.Where( x =>
            GetItemStartTime( x ).Date <= GetItemEndTime( item ).Date &&
            GetItemEndTime( x ).Date >= GetItemStartTime( item ) ).ToList();

        if ( items is null )
        {
            return 0;
        }

        return items
            .OrderBy( x => GetItemStartTime( x ) )
            .ThenBy( x => GetItemEndTime( x ) )
            .TakeWhile( x => !x.Equals( item ) )
            .Count();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates whether the day view is currently displayed.
    /// </summary>
    protected bool ShowingDayView => schedulerDayView is not null && SelectedView == SchedulerView.Day;

    /// <summary>
    /// Indicates whether the week view is currently displayed.
    /// </summary>
    protected bool ShowingWeekView => schedulerWeekView is not null && SelectedView == SchedulerView.Week;

    /// <summary>
    /// Indicates whether the work week view is currently displayed.
    /// </summary>
    protected bool ShowingWorkWeekView => schedulerWorkWeekView is not null && SelectedView == SchedulerView.WorkWeek;

    /// <summary>
    /// Indicates whether the month view is currently displayed.
    /// </summary>
    protected bool ShowingMonthView => schedulerMonthView is not null && SelectedView == SchedulerView.Month;

    /// <summary>
    /// Gets the scheduler state.
    /// </summary>
    protected SchedulerState State => state;

    /// <summary>
    /// Returns true if <see cref="Data"/> is safe to modify.
    /// </summary>
    protected bool CanInsertNewItem => Editable && Data is ICollection<TItem>;

    /// <summary>
    /// Holds a collection of items of type <typeparamref name="TItem"/>. Used to manage and display a list of appointments.
    /// </summary>
    [Parameter] public IEnumerable<TItem> Data { get; set; }

    /// <summary>
    /// The currently selected date. Determines the date that is displayed in the scheduler. Defaults to the current date.
    /// </summary>
    [Parameter] public DateOnly Date { get; set; }

    /// <summary>
    /// Occurs when the selected date changes.
    /// </summary>
    [Parameter] public EventCallback<DateOnly> DateChanged { get; set; }

    /// <summary>
    /// The mode of the week navigation. Determines how the week navigation is handled.
    /// </summary>
    [Parameter] public SchedulerWeekNavigationMode WeekNavigationMode { get; set; } = SchedulerWeekNavigationMode.FirstDayOfWeek;

    /// <summary>
    /// The currently selected view. Determines the view that is displayed in the scheduler.
    /// </summary>
    [Parameter] public SchedulerView SelectedView { get; set; }

    /// <summary>
    /// Occurs when the selected view changes.
    /// </summary>
    [Parameter] public EventCallback<SchedulerView> SelectedViewChanged { get; set; }

    /// <summary>
    /// Indicates if the toolbar should be displayed.
    /// </summary>
    [Parameter] public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the unique identifier of the appointment. Defaults to "Id".
    /// </summary>
    [Parameter] public string IdField { get; set; } = "Id";

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the start date of the appointment. Defaults to "Start".
    /// </summary>
    [Parameter] public string StartField { get; set; } = "Start";

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the end date of the appointment. Defaults to "End".
    /// </summary>
    [Parameter] public string EndField { get; set; } = "End";

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the title of the appointment. Defaults to "Title".
    /// </summary>
    [Parameter] public string TitleField { get; set; } = "Title";

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the description of the appointment. Defaults to "Description".
    /// </summary>
    [Parameter] public string DescriptionField { get; set; } = "Description";

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the all-day status of the appointment. Defaults to "AllDay".
    /// </summary>
    [Parameter] public string AllDayField { get; set; } = "AllDay";

    /// <summary>
    /// Indicates whether the component is editable. Defaults to true.
    /// </summary>
    [Parameter] public bool Editable { get; set; } = true;

    /// <summary>
    /// Indicates whether internal editing is enabled. Defaults to true.
    /// </summary>
    [Parameter] public bool UseInternalEditing { get; set; } = true;

    /// <summary>
    /// Defines a function that creates a new item of type TItem. It allows for custom item creation logic.
    /// </summary>
    [Parameter] public Func<TItem> NewItemCreator { get; set; }

    /// <summary>
    /// Triggers an event when a new item is being inserted into the scheduler. It provides a callback with details about the item being inserted.
    /// </summary>
    [Parameter] public EventCallback<SchedulerCancellableItemChange<TItem>> ItemInserting { get; set; }

    /// <summary>
    /// An event callback triggered when an item is being updated in the scheduler. It allows handling of the item update process.
    /// </summary>
    [Parameter] public EventCallback<SchedulerCancellableItemChange<TItem>> ItemUpdating { get; set; }

    /// <summary>
    /// Triggers an event when a new item is inserted into the scheduler.
    /// </summary>
    [Parameter] public EventCallback<SchedulerInsertedItem<TItem>> ItemInserted { get; set; }

    /// <summary>
    /// Triggers an event when an item in the scheduler is updated.
    /// </summary>
    [Parameter] public EventCallback<SchedulerUpdatedItem<TItem>> ItemUpdated { get; set; }

    /// <summary>
    /// Occurs when an empty slot is clicked.
    /// </summary>
    [Parameter] public EventCallback<SchedulerSlotClickedEventArgs> SlotClicked { get; set; }

    /// <summary>
    /// Occurs when an appointment is clicked.
    /// </summary>
    [Parameter] public EventCallback<SchedulerItemClickedEventArgs<TItem>> ItemClicked { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the component.
    /// </summary>
    /// <remarks>
    /// This property allows developers to define custom content within the <see cref="Scheduler"/> component.
    /// </remarks>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
