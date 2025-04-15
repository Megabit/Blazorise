#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Blazorise.DeepCloner;
using Blazorise.Extensions;
using Blazorise.Infrastructure;
using Blazorise.Localization;
using Blazorise.Scheduler.Components;
using Blazorise.Scheduler.Extensions;
using Blazorise.Scheduler.Utilities;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;
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

    private Func<TItem, object> getIdFunc;

    private Func<TItem, string> getTitleFunc;
    private Action<TItem, string> setTitleFunc;

    private Func<TItem, string> getDescriptionFunc;
    private Action<TItem, string> setDescriptionFunc;

    private Func<TItem, DateTime> getStartFunc;
    private Action<TItem, DateTime> setStartFunc;

    private Func<TItem, DateTime> getEndFunc;
    private Action<TItem, DateTime> setEndFunc;

    private Func<TItem, bool> getAllDayFunc;
    private Action<TItem, bool> setAllDayFunc;

    private Func<TItem, string> getRecurrenceRuleFunc;
    private Action<TItem, string> setRecurrenceRuleFunc;

    private Lazy<Func<TItem>> newItemCreator;

    protected _SchedulerModal<TItem> schedulerModalRef;

    protected TItem editItem;

    protected SchedulerEditState editState = SchedulerEditState.None;

    protected SchedulerTransaction<TItem> currentTransaction;

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

        if ( !string.IsNullOrEmpty( IdField ) && typeof( TItem ).GetProperty( IdField )?.PropertyType is not null )
        {
            getIdFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem>( IdField );
        }

        if ( !string.IsNullOrEmpty( TitleField ) && typeof( TItem ).GetProperty( TitleField )?.PropertyType is not null )
        {
            getTitleFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, string>( TitleField );
            setTitleFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, string>( TitleField );
        }

        if ( !string.IsNullOrEmpty( DescriptionField ) && typeof( TItem ).GetProperty( DescriptionField )?.PropertyType is not null )
        {
            getDescriptionFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, string>( DescriptionField );
            setDescriptionFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, string>( DescriptionField );
        }

        if ( !string.IsNullOrEmpty( StartField ) && typeof( TItem ).GetProperty( StartField )?.PropertyType is not null )
        {
            getStartFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, DateTime>( StartField );
            setStartFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, DateTime>( StartField );
        }

        if ( !string.IsNullOrEmpty( EndField ) && typeof( TItem ).GetProperty( EndField )?.PropertyType is not null )
        {
            getEndFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, DateTime>( EndField );
            setEndFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, DateTime>( EndField );
        }

        if ( !string.IsNullOrEmpty( AllDayField ) && typeof( TItem ).GetProperty( AllDayField )?.PropertyType is not null )
        {
            getAllDayFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, bool>( AllDayField );
            setAllDayFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, bool>( AllDayField );
        }

        if ( !string.IsNullOrEmpty( RecurrenceRuleField ) && typeof( TItem ).GetProperty( RecurrenceRuleField )?.PropertyType is not null )
        {
            getRecurrenceRuleFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, string>( RecurrenceRuleField );
            setRecurrenceRuleFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, string>( RecurrenceRuleField );
        }

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
        await Refresh();
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
        await Refresh();
    }

    /// <summary>
    /// Navigates to the current date.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task NavigateToday()
    {
        state.SelectedDate = DateOnly.FromDateTime( DateTime.Today );
        await DateChanged.InvokeAsync( state.SelectedDate );
        await Refresh();
    }

    /// <summary>
    /// Sets the selected view to 'Day' and triggers an update to reflect this change.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ShowDayView()
    {
        SelectedView = SchedulerView.Day;
        await SelectedViewChanged.InvokeAsync( SelectedView );
        await Refresh();
    }

    /// <summary>
    /// Sets the selected view to 'Week' and triggers an update to reflect this change.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ShowWeekView()
    {
        SelectedView = SchedulerView.Week;
        await SelectedViewChanged.InvokeAsync( SelectedView );
        await Refresh();
    }

    /// <summary>
    /// Sets the selected view to 'WorkWeek' and triggers an update to reflect this change.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ShowWorkWeekView()
    {
        SelectedView = SchedulerView.WorkWeek;
        await SelectedViewChanged.InvokeAsync( SelectedView );
        await Refresh();
    }

    /// <summary>
    /// Sets the selected view to 'Month' and triggers an update to reflect this change.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ShowMonthView()
    {
        SelectedView = SchedulerView.Month;
        await SelectedViewChanged.InvokeAsync( SelectedView );
        await Refresh();
    }

    /// <summary>
    /// Gets the id of the specified appointment.
    /// </summary>
    /// <param name="item">The appointment to get the id from.</param>
    /// <returns>The id of the appointment.</returns>
    internal object GetItemId( TItem item )
    {
        return getIdFunc?.Invoke( item );
    }

    /// <summary>
    /// Gets the title of the specified appointment.
    /// </summary>
    /// <param name="item">The appointment to get the title from.</param>
    /// <returns>The title of the appointment.</returns>
    internal string GetItemTitle( TItem item )
    {
        return getTitleFunc?.Invoke( item );
    }

    /// <summary>
    /// Calculates the duration of an appointment based on its start and end times.
    /// </summary>
    /// <param name="item">Represents an appointment from which the start and end times are derived.</param>
    /// <returns>Returns the duration as a TimeSpan, or zero if the times are not valid DateTime values.</returns>
    internal TimeSpan GetItemDuration( TItem item )
    {
        var start = getStartFunc?.Invoke( item );
        var end = getEndFunc?.Invoke( item );

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
        return getStartFunc?.Invoke( item ) as DateTime? ?? DateTime.MinValue;
    }

    /// <summary>
    /// Gets the appointment end time.
    /// </summary>
    /// <param name="item">An item from which the end time is derived.</param>
    /// <returns>Returns the end time of the item.</returns>
    internal DateTime GetItemEndTime( TItem item )
    {
        return getEndFunc?.Invoke( item ) as DateTime? ?? DateTime.MaxValue;
    }

    /// <summary>
    /// Gets the appointment all day value.
    /// </summary>
    /// <param name="item">An item from which the all-day value is derived.</param>
    /// <returns>Returns the all-day value of the item.</returns>
    internal bool GetItemAllDay( TItem item )
    {
        return getAllDayFunc?.Invoke( item ) ?? false;
    }

    internal string GetItemRecurrenceRule( TItem item )
    {
        return getRecurrenceRuleFunc?.Invoke( item );
    }

    /// <summary>
    /// Gets the description of the specified appointment.
    /// </summary>
    /// <param name="item">The appointment to get the description from.</param>
    /// <returns>The description of the appointment.</returns>
    internal string GetItemDescription( TItem item )
    {
        return getDescriptionFunc?.Invoke( item );
    }

    /// <summary>
    /// Sets the starting value for a specified item using a provided value.
    /// </summary>
    /// <param name="item">Specifies the item for which the starting value is being set.</param>
    /// <param name="value">Provides the value to be assigned as the starting point for the specified item.</param>
    internal void SetItemStart( TItem item, DateTime value )
    {
        setStartFunc?.Invoke( item, value );
    }

    /// <summary>
    /// Sets the end value for a specified item using a provided function.
    /// </summary>
    /// <param name="item">Specifies the item for which the end value is being set.</param>
    /// <param name="value">Provides the value to be assigned to the end of the specified item.</param>
    internal void SetItemEnd( TItem item, DateTime value )
    {
        setEndFunc?.Invoke( item, value );
    }

    /// <summary>
    /// Sets the start and end dates for a specified item.
    /// </summary>
    /// <param name="item">Specifies the item for which the dates are being set.</param>
    /// <param name="start">Indicates the starting date or time for the item.</param>
    /// <param name="end">Indicates the ending date or time for the item.</param>
    internal void SetItemDates( TItem item, DateTime start, DateTime end )
    {
        SetItemStart( item, start );
        SetItemEnd( item, end );
    }

    internal async Task NotifyEditItemClicked( TItem item )
    {
        await Edit( item );

        await ItemClicked.InvokeAsync( new( item ) );
    }

    internal async Task NotifyDeleteItemClicked( TItem item )
    {
        await Delete( item );

        await ItemClicked.InvokeAsync( new( item ) );
    }

    internal async Task NotifySlotClicked( DateTime start, DateTime end )
    {
        editItem = CreateNewItem();
        SetItemDates( editItem, start, end );

        await New( editItem );

        await SlotClicked.InvokeAsync( new SchedulerSlotClickedEventArgs( start, end ) );
    }

    /// <summary>
    /// Triggers a re-evaluation of the component's state and UI.
    /// </summary>
    /// <returns>Returns a Task representing the asynchronous operation.</returns>
    internal Task Refresh()
    {
        return InvokeAsync( StateHasChanged );
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

    /// <summary>
    /// Sets the <see cref="Scheduler{TItem}"/> into the Edit state mode for the specified item. 
    /// </summary>
    /// <param name="item">Item for which to set the edit mode.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Edit( TItem item )
    {
        editItem = item;
        editState = SchedulerEditState.Edit;

        if ( Editable && UseInternalEditing && schedulerModalRef is not null )
        {
            await schedulerModalRef.ShowModal( item.DeepClone(), false );
        }
    }

    /// <summary>
    /// Deletes the specified item from the <see cref="Data"/> source.
    /// </summary>
    /// <param name="item">Item to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Delete( TItem item )
    {
        if ( Editable && UseInternalEditing && Data is ICollection<TItem> data )
        {
            var isSeries = !string.IsNullOrEmpty( GetItemRecurrenceRule( item ) );

            var deleteMessage = isSeries
                ? Localizer.Localize( Localizers?.SeriesDeleteConfirmationTextLocalizer, "Item is a recurring series. Are you sure you want to delete all occurrences?" )
                : Localizer.Localize( Localizers?.ItemDeleteConfirmationLocalizer, "Item will be deleted permanently, are you sure?" );

            if ( await MessageService.Confirm( deleteMessage, "Delete", options =>
            {
                options.ShowCloseButton = false;
                options.ShowMessageIcon = false;
                options.CancelButtonText = "Cancel";
                options.ConfirmButtonText = "Delete";
                options.ConfirmButtonColor = Color.Danger;
            } ) == false )
                return;

            await DeleteImpl( item );
        }
    }

    protected internal async Task<bool> SaveImpl( TItem submitedItem )
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

            await Refresh();

            return true;
        }

        return false;
    }

    protected internal async Task<bool> DeleteImpl( TItem itemToDelete )
    {
        if ( await IsSafeToProceed( ItemRemoving, itemToDelete, itemToDelete ) )
        {
            if ( UseInternalEditing && CanInsertNewItem && Data is ICollection<TItem> data )
            {
                var itemRef = data.FirstOrDefault( x => GetItemId( x ).IsEqual( GetItemId( itemToDelete ) ) );

                if ( itemRef is not null )
                {
                    data.Remove( itemRef );
                }
            }

            editState = SchedulerEditState.None;

            await Refresh();

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
    internal void CopyItemValues( TItem source, TItem destination )
    {
        setTitleFunc?.Invoke( destination, getTitleFunc?.Invoke( source ) );
        setDescriptionFunc?.Invoke( destination, getDescriptionFunc?.Invoke( source ) );
        setStartFunc?.Invoke( destination, getStartFunc?.Invoke( source ) ?? DateTime.MinValue );
        setEndFunc?.Invoke( destination, getEndFunc?.Invoke( source ) ?? DateTime.MinValue );
        setAllDayFunc?.Invoke( destination, getAllDayFunc?.Invoke( source ) ?? false );
        setRecurrenceRuleFunc?.Invoke( destination, getRecurrenceRuleFunc?.Invoke( source ) );
    }

    /// <summary>
    /// Retrieves items that fall within a specified time range, excluding all-day events and those lasting a full day.
    /// </summary>
    /// <param name="viewStart">Indicates the beginning of the time range for filtering items.</param>
    /// <param name="viewEnd">Indicates the end of the time range for filtering items.</param>
    /// <returns>A collection of items that meet the specified time criteria.</returns>
    internal IEnumerable<SchedulerItemInfo<TItem>> GetItemsInView( DateTime viewStart, DateTime viewEnd )
    {
        var itemsInView = from item in Data
                          let allDay = GetItemAllDay( item )
                          let start = GetItemStartTime( item )
                          let end = GetItemEndTime( item )
                          let duration = GetItemDuration( item )
                          let recurrenceRule = GetItemRecurrenceRule( item )
                          let allDayByDuration = duration.Days >= 1
                          where !allDay && !allDayByDuration &&
                          ( ( start >= viewStart && start <= viewEnd )
                           || ( start < viewStart && end >= viewStart && end <= viewEnd )
                           || ( start < viewStart && end > viewStart ) )
                          select new SchedulerItemInfo<TItem>( item, start, end, allDay, recurrenceRule, false );

        var recurringItems = from item in Data
                             let recurrenceRule = GetItemRecurrenceRule( item )
                             where !string.IsNullOrEmpty( recurrenceRule )
                             let start = GetItemStartTime( item )
                             where start < viewStart
                             select item;

        var virtualItems = new List<SchedulerItemInfo<TItem>>();

        foreach ( var item in recurringItems )
        {
            var recurrenceRule = GetItemRecurrenceRule( item );
            var start = GetItemStartTime( item );
            var end = GetItemEndTime( item );
            var duration = GetItemDuration( item );

            // Generate virtual items based on recurrence rule
            var occurrences = GenerateOccurrences( start, end, recurrenceRule, viewStart, viewEnd );

            foreach ( var occurrence in occurrences )
            {
                var virtualStart = occurrence;
                var virtualEnd = virtualStart.Add( duration );
                virtualItems.Add( new SchedulerItemInfo<TItem>( item, virtualStart, virtualEnd, false, recurrenceRule, true ) );
            }
        }

        foreach ( var item in itemsInView )
        {
            var recurrenceRule = GetItemRecurrenceRule( item.Item );
            if ( !string.IsNullOrEmpty( recurrenceRule ) )
            {
                var start = GetItemStartTime( item.Item );
                var end = GetItemEndTime( item.Item );
                var duration = GetItemDuration( item.Item );

                // Generate virtual items based on recurrence rule
                var occurrences = GenerateOccurrences( start, end, recurrenceRule, viewStart, viewEnd );

                foreach ( var occurrence in occurrences )
                {
                    var virtualStart = occurrence;
                    var virtualEnd = virtualStart.Add( duration );
                    virtualItems.Add( new SchedulerItemInfo<TItem>( item.Item, virtualStart, virtualEnd, false, recurrenceRule, true ) );
                }
            }
        }

        // If the item is having a recurring rule that means we want to show a series and we want to hide original item.
        return itemsInView
            .Where( x => x.RecurrenceRule is null )
            .Concat( virtualItems );
    }

    private IEnumerable<DateTime> GenerateOccurrences( DateTime start, DateTime end, string recurrenceRule, DateTime viewStart, DateTime viewEnd )
    {
        var rule = RecurringRuleParser.Parse( recurrenceRule );

        if ( rule.Pattern == SchedulerRecurrencePattern.Daily )
        {
            return RecurringRuleCalculators.GetDailyRecurringDates( start, viewStart, viewEnd, rule );
        }
        else if ( rule.Pattern == SchedulerRecurrencePattern.Weekly )
        {
            return RecurringRuleCalculators.GetWeeklyRecurringDates( start, viewStart, viewEnd, FirstDayOfWeek, rule );
        }
        else if ( rule.Pattern == SchedulerRecurrencePattern.Monthly )
        {
            return RecurringRuleCalculators.GetMonthlyRecurringDates( start, viewStart, viewEnd, FirstDayOfWeek, rule );
        }
        else if ( rule.Pattern == SchedulerRecurrencePattern.Yearly )
        {
            return RecurringRuleCalculators.GetYearlyRecurringDates( start, viewStart, viewEnd, FirstDayOfWeek, rule );
        }

        return Enumerable.Empty<DateTime>();
    }

    /// <summary>
    /// Retrieves all-day items within a specified date range, filtering based on start and end times.
    /// </summary>
    /// <param name="viewStart">Specifies the beginning of the date range for filtering all-day items.</param>
    /// <param name="viewEnd">Specifies the end of the date range for filtering all-day items.</param>
    /// <returns>A collection of all-day item information that falls within the specified date range.</returns>
    internal IEnumerable<SchedulerAllDayItemInfo<TItem>> GetAllDayItemsInRange( DateTime viewStart, DateTime viewEnd )
    {
        return from d in Data
               let allDay = GetItemAllDay( d )
               let start = GetItemStartTime( d ).Date   // we are only interested in date part
               let end = GetItemEndTime( d ).Date       // we are only interested in date part
               let duration = GetItemDuration( d )      // for duration we want all parts, date and time
               let allDayByDuration = duration.Days >= 1
               where ( allDay || allDayByDuration ) &&
               ( ( start >= viewStart && start <= viewEnd )
                || ( start < viewStart && end >= viewStart && end <= viewEnd )
                || ( start < viewStart && end > viewStart ) )
               orderby GetItemDuration( d ) descending
               select new SchedulerAllDayItemInfo<TItem>( item: d,
                   viewStart: start < viewStart ? viewStart : start,
                   viewEnd: end > viewEnd ? viewEnd : end,
                   overflowingFromStart: start < viewStart,
                   overflowingOnEnd: end > viewEnd );
    }

    /// <summary>
    /// Retrieves scheduled items occurring on a specific date within a defined time range.
    /// </summary>
    /// <param name="items">A collection of items to be filtered based on their scheduled times.</param>
    /// <param name="viewDate">The date for which the scheduled items are being retrieved.</param>
    /// <param name="viewStartTime">The start time of the time range for filtering the scheduled items.</param>
    /// <param name="viewEndTime">The end time of the time range for filtering the scheduled items.</param>
    /// <returns>A collection of filtered scheduled item information based on the specified date and time range.</returns>
    internal IEnumerable<SchedulerItemViewInfo<TItem>> GetViewItemsOnDate( IEnumerable<SchedulerItemInfo<TItem>> items, DateOnly viewDate, TimeOnly viewStartTime, TimeOnly viewEndTime )
    {
        var minDateTime = viewDate.ToDateTime( viewStartTime );
        var maxDateTime = viewDate.ToDateTime( viewEndTime );

        return from item in items
               where !item.AllDay && !item.AllDayByDuration &&
               ( ( item.Start >= minDateTime && item.Start <= maxDateTime )
                || ( item.Start < minDateTime && item.End >= minDateTime && item.End <= maxDateTime )
                || ( item.Start < minDateTime && item.End > minDateTime ) )
               select new SchedulerItemViewInfo<TItem>( item: item.Item,
                   viewStart: item.Start < minDateTime ? minDateTime : item.Start,
                   viewEnd: item.End > maxDateTime ? maxDateTime : item.End,
                   overflowingFromStart: item.Start < minDateTime,
                   overflowingOnEnd: item.End > maxDateTime,
                   recurrenceRule: item.RecurrenceRule,
                   isRecurring: item.IsRecurring );
    }

    /// <summary>
    /// Retrieves the first item from a collection that starts within a specified date range.
    /// </summary>
    /// <param name="items">A collection of scheduled items to search for an item that starts within the specified range.</param>
    /// <param name="viewStart">The beginning of the date range used to filter the scheduled items.</param>
    /// <param name="viewEnd">The end of the date range used to filter the scheduled items.</param>
    /// <returns>The first scheduled item that starts within the specified date range or null if none is found.</returns>
    internal IEnumerable<SchedulerItemViewInfo<TItem>> GetViewItemInRange( IEnumerable<SchedulerItemViewInfo<TItem>> items, DateTime viewStart, DateTime viewEnd )
    {
        return ( from itemInfo in items
                 let start = itemInfo.ViewStart
                 let end = itemInfo.ViewEnd
                 let duration = end - start
                 where start >= viewStart && start < viewEnd
                 select itemInfo );
    }

    /// <summary>
    /// Retrieves all-day items that start on a specific date from a collection of items.
    /// </summary>
    /// <param name="items">A collection of all-day item information to filter based on the specified date.</param>
    /// <param name="date">The specific date used to find items that start on that day.</param>
    /// <returns>A sorted collection of all-day items that start on the specified date, ordered by their duration.</returns>
    internal IEnumerable<SchedulerAllDayItemInfo<TItem>> GetAllDayItemOnDate( IEnumerable<SchedulerAllDayItemInfo<TItem>> items, DateTime date )
    {
        return from itemInfo in items
               let start = itemInfo.ViewStart
               let end = itemInfo.ViewEnd
               let duration = end - start
               where start >= date && start <= date
               orderby duration descending
               select itemInfo;
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
    /// <param name="items">A collection of all-day items to evaluate for overlaps.</param>
    /// <param name="viewStart">The start date of the range to check for overlapping items.</param>
    /// <param name="viewEnd">The end date of the range to check for overlapping items.</param>
    /// <returns>The maximum count of overlapping items found during the specified date range.</returns>
    internal int GetMaxStackedAllDayItems( IEnumerable<SchedulerAllDayItemInfo<TItem>> items, DateOnly viewStart, DateOnly viewEnd )
    {
        int maxItems = 0;

        for ( var date = viewStart; date <= viewEnd; date = date.AddDays( 1 ) )
        {
            var currentDate = date.ToDateTime( TimeOnly.MinValue );
            var nextDate = date.AddDays( 1 ).ToDateTime( TimeOnly.MinValue );

            var overlappingItems = items.Where( x => x.ViewStart < nextDate && x.ViewEnd >= currentDate ).Count();

            if ( overlappingItems > maxItems )
            {
                maxItems = overlappingItems;
            }
        }

        return maxItems;
    }

    /// <summary>
    /// Counts the number of overlapping items before a specified item in a collection.
    /// </summary>
    /// <param name="items">A collection of all-day items to check for overlaps.</param>
    /// <param name="item">The specific item to compare against for overlap detection.</param>
    /// <returns>The total count of overlapping items that occur before the specified item.</returns>
    internal int CountOverlappingItemsBefore( IEnumerable<SchedulerAllDayItemInfo<TItem>> items, SchedulerAllDayItemInfo<TItem> item )
    {
        return items.Where( x => x.ViewStart <= item.ViewEnd && x.ViewEnd >= item.ViewStart )
            .OrderByDescending( x => x.ViewEnd - x.ViewStart )
            .ThenBy( x => x.ViewStart )
            .TakeWhile( x => !x.Item.Equals( item.Item ) )
            .Count();
    }

    private bool isDragging = false;

    // Events for drag and drop operations
    public event EventHandler<TItem> DragStarted;
    public event EventHandler<TItem> DragCancelled;
    public event EventHandler<(TItem Item, DateTime Start, DateTime End)> ItemDropped;

    public bool IsDragging => isDragging;

    internal async Task StartDrag( TItem item )
    {
        // Cancel any existing transaction
        if ( currentTransaction != null )
        {
            await currentTransaction.Rollback();
            currentTransaction = null;
        }

        // Create a new transaction
        currentTransaction = new SchedulerTransaction<TItem>( this, item );
        isDragging = true;

        DragStarted?.Invoke( this, item );
    }

    internal async Task CancelDrag()
    {
        if ( currentTransaction == null )
            return;

        TItem item = currentTransaction.Item;

        await currentTransaction.Rollback();
        currentTransaction = null;
        isDragging = false;

        DragCancelled?.Invoke( this, item );
    }

    internal async Task<bool> DropSlotItem( DateTime newStart, DateTime newEnd )
    {
        if ( currentTransaction == null )
            return false;

        try
        {
            var duration = GetItemDuration( currentTransaction.Item );
            SetItemDates( currentTransaction.Item, newStart, newStart.Add( duration ) );

            await currentTransaction.Commit();

            ItemDropped?.Invoke( this, (currentTransaction.Item, newStart, newEnd) );

            isDragging = false;
            currentTransaction = null;

            return true;
        }
        catch
        {
            await CancelDrag();

            return false;
        }
        finally
        {
            await Refresh();
        }
    }

    internal async Task<bool> DropDateItem( DateOnly date )
    {
        if ( currentTransaction == null )
            return false;

        try
        {
            var start = GetItemStartTime( currentTransaction.Item );
            var end = GetItemEndTime( currentTransaction.Item );

            var newStart = new DateTime( date.Year, date.Month, date.Day, start.Hour, start.Minute, start.Second );
            var newEnd = new DateTime( date.Year, date.Month, date.Day, end.Hour, end.Minute, end.Second );

            SetItemDates( currentTransaction.Item, newStart, newEnd );

            await currentTransaction.Commit();

            ItemDropped?.Invoke( this, (currentTransaction.Item, newStart, newEnd) );

            isDragging = false;
            currentTransaction = null;

            return true;
        }
        catch
        {
            await CancelDrag();

            return false;
        }
        finally
        {
            await Refresh();
        }
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
    /// Returns the first day of the week based on the current view.
    /// </summary>
    protected DayOfWeek FirstDayOfWeek => ShowingWorkWeekView
        ? schedulerWorkWeekView.FirstDayOfWorkWeek
        : ShowingWeekView
            ? schedulerWeekView.FirstDayOfWeek
            : ShowingDayView
                ? schedulerDayView.FirstDayOfWeek
                : DayOfWeek.Sunday;

    /// <summary>
    /// Gets the scheduler state.
    /// </summary>
    protected SchedulerState State => state;

    /// <summary>
    /// Returns true if <see cref="Data"/> is safe to modify.
    /// </summary>
    protected bool CanInsertNewItem => Editable && Data is ICollection<TItem>;

    /// <summary>
    /// Injects an instance of <see cref="IMessageService"/> for handling message-related operations. It is a private property.
    /// </summary>
    [Inject] private IMessageService MessageService { get; set; }

    /// <summary>
    /// Injects an instance of <see cref="ITextLocalizerService"/> for localization.
    /// </summary>
    [Inject] protected ITextLocalizer<Scheduler<TItem>> Localizer { get; set; }

    /// <summary>
    /// Injects an instance of <see cref="ITextLocalizerService"/> for localization.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

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

    // <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the recurrence rule string in <see href="https://icalendar.org/RFC-Specifications/iCalendar-RFC-5545/">iCalendar (RFC 5545)</see> compliance rules.
    /// </summary>
    [Parameter] public string RecurrenceRuleField { get; set; } = "RecurrenceRule";

    /// <summary>
    /// Indicates whether the component is editable. Defaults to true.
    /// </summary>
    [Parameter] public bool Editable { get; set; } = true;

    /// <summary>
    /// Indicates whether the items in the scheduler can be dragged and dropped. Defaults to false.
    /// </summary>
    [Parameter] public bool Draggable { get; set; }

    /// <summary>
    /// Indicates whether internal editing is enabled. Defaults to true.
    /// </summary>
    [Parameter] public bool UseInternalEditing { get; set; } = true;

    /// <summary>
    /// Defines a function that creates a new item of type TItem. It allows for custom item creation logic.
    /// </summary>
    [Parameter] public Func<TItem> NewItemCreator { get; set; }

    /// <summary>
    /// Triggers an event when a new item is being inserted into the scheduler. It allows handling and cancellation of the item insert process.
    /// </summary>
    [Parameter] public EventCallback<SchedulerCancellableItemChange<TItem>> ItemInserting { get; set; }

    /// <summary>
    /// An event callback triggered when an item is being updated in the scheduler. It allows handling and cancellation of the item update process.
    /// </summary>
    [Parameter] public EventCallback<SchedulerCancellableItemChange<TItem>> ItemUpdating { get; set; }

    /// <summary>
    /// An event callback triggered when an item is being removed from the scheduler. It allows handling and cancellation of the removal process.
    /// </summary>
    [Parameter] public EventCallback<SchedulerCancellableItemChange<TItem>> ItemRemoving { get; set; }

    /// <summary>
    /// Triggers an event when a new item is inserted into the scheduler.
    /// </summary>
    [Parameter] public EventCallback<SchedulerInsertedItem<TItem>> ItemInserted { get; set; }

    /// <summary>
    /// Triggers an event when an item in the scheduler is updated.
    /// </summary>
    [Parameter] public EventCallback<SchedulerUpdatedItem<TItem>> ItemUpdated { get; set; }

    /// <summary>
    /// Triggers an event when an item in the scheduler is deleted.
    /// </summary>
    [Parameter] public EventCallback<SchedulerUpdatedItem<TItem>> ItemRemoved { get; set; }

    /// <summary>
    /// Occurs when an empty slot is clicked.
    /// </summary>
    [Parameter] public EventCallback<SchedulerSlotClickedEventArgs> SlotClicked { get; set; }

    /// <summary>
    /// Occurs when an appointment is clicked.
    /// </summary>
    [Parameter] public EventCallback<SchedulerItemClickedEventArgs<TItem>> ItemClicked { get; set; }

    /// <summary>
    /// Custom localizer handlers to override default <see cref="Scheduler{TItem}"/> localization.
    /// </summary>
    [Parameter] public SchedulerLocalizers Localizers { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the component.
    /// </summary>
    /// <remarks>
    /// This property allows developers to define custom content within the <see cref="Scheduler"/> component.
    /// </remarks>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}