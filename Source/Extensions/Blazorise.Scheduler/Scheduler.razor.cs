#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.DeepCloner;
using Blazorise.Extensions;
using Blazorise.Infrastructure;
using Blazorise.Localization;
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

    private SchedulerPropertyMapper<TItem> propertyMapper;

    private Lazy<Func<TItem>> newItemCreator;

    protected _SchedulerIItemModal<TItem> schedulerItemModalRef;

    protected _SchedulerIItemOccurrenceModal<TItem> schedulerItemOccurrenceModalRef;

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
        propertyMapper = new SchedulerPropertyMapper<TItem>( this );
        newItemCreator = new( () => SchedulerFunctionCompiler.CreateNewItem<TItem>() );

        prevDaySubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, NavigatePrevious ) );
        nextDaySubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, NavigateNext ) );
        todaySubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, NavigateToday ) );
        dayViewSubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, ShowDayView ) );
        weekViewSubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, ShowWeekView ) );
        workWeekViewSubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, ShowWorkWeekView ) );
        monthViewSubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, ShowMonthView ) );
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

    internal TItem GetParentItem( TItem item )
    {
        if ( IsRecurrenceItem( item ) && Data is ICollection<TItem> data && propertyMapper.GetRecurrenceId( item ) is not null )
        {
            return data.FirstOrDefault( x => PropertyMapper.GetId( x ).Equals( propertyMapper.GetRecurrenceId( item ) ) );
        }

        return item;
    }

    /// <summary>
    /// Gets the id of the specified appointment.
    /// </summary>
    /// <param name="item">The appointment to get the id from.</param>
    /// <returns>The id of the appointment.</returns>
    internal object GetItemId( TItem item )
    {
        return propertyMapper.GetId( item );
    }

    /// <summary>
    /// Gets the title of the specified appointment.
    /// </summary>
    /// <param name="item">The appointment to get the title from.</param>
    /// <returns>The title of the appointment.</returns>
    internal string GetItemTitle( TItem item )
    {
        return propertyMapper.GetTitle( item );
    }

    /// <summary>
    /// Calculates the duration of an appointment based on its start and end times.
    /// </summary>
    /// <param name="item">Represents an appointment from which the start and end times are derived.</param>
    /// <returns>Returns the duration as a TimeSpan, or zero if the times are not valid DateTime values.</returns>
    internal TimeSpan GetItemDuration( TItem item )
    {
        var start = propertyMapper.GetStart( item );
        var end = propertyMapper.GetEnd( item );

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
        return propertyMapper.GetStart( item ) as DateTime? ?? DateTime.MinValue;
    }

    /// <summary>
    /// Gets the appointment end time.
    /// </summary>
    /// <param name="item">An item from which the end time is derived.</param>
    /// <returns>Returns the end time of the item.</returns>
    internal DateTime GetItemEndTime( TItem item )
    {
        return propertyMapper.GetEnd( item ) as DateTime? ?? DateTime.MaxValue;
    }

    /// <summary>
    /// Gets the appointment all day value.
    /// </summary>
    /// <param name="item">An item from which the all-day value is derived.</param>
    /// <returns>Returns the all-day value of the item.</returns>
    internal bool GetItemAllDay( TItem item )
    {
        return propertyMapper.GetAllDay( item );
    }

    internal string GetItemRecurrenceRule( TItem item )
    {
        return propertyMapper.GetRecurrenceRule( item );
    }

    /// <summary>
    /// Gets the description of the specified appointment.
    /// </summary>
    /// <param name="item">The appointment to get the description from.</param>
    /// <returns>The description of the appointment.</returns>
    internal string GetItemDescription( TItem item )
    {
        return propertyMapper.GetDescription( item );
    }

    /// <summary>
    /// Sets the starting value for a specified item using a provided value.
    /// </summary>
    /// <param name="item">Specifies the item for which the starting value is being set.</param>
    /// <param name="value">Provides the value to be assigned as the starting point for the specified item.</param>
    internal void SetItemStart( TItem item, DateTime value )
    {
        propertyMapper.SetStart( item, value );
    }

    /// <summary>
    /// Sets the end value for a specified item using a provided function.
    /// </summary>
    /// <param name="item">Specifies the item for which the end value is being set.</param>
    /// <param name="value">Provides the value to be assigned to the end of the specified item.</param>
    internal void SetItemEnd( TItem item, DateTime value )
    {
        propertyMapper.SetEnd( item, value );
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

    internal async Task NotifyEditItemClicked( SchedulerItemViewInfo<TItem> viewItem )
    {
        var isPartOfSeries = PropertyMapper.GetRecurrenceId( viewItem.Item ) is not null;

        if ( isPartOfSeries )
        {
            var result = await MessageService.Choose(
                        message: "What do you want to do?",
                        title: Localizer.Localize( Localizers?.EditLocalizer, "Edit" ), options =>
                        {
                            options.ShowCloseButton = false;
                            options.ShowMessageIcon = false;
                            options.Choices = new List<MessageOptionsChoice>
                            {
                            new MessageOptionsChoice
                            {
                                Key = "EditSeries",
                                Text = Localizer.Localize( Localizers?.EditSeriesLocalizer, "Edit Series" ),
                                Color = Color.Primary,
                            },
                            new MessageOptionsChoice
                            {
                                Key = "EditOccurrence",
                                Text = Localizer.Localize( Localizers?.EditOccurrenceLocalizer, "Edit Occurrence" ),
                                Color = Color.Primary,
                            },
                            new MessageOptionsChoice
                            {
                                Key = "Cancel",
                                Text = Localizer.Localize( Localizers?.CancelLocalizer, "Cancel" ),
                                Color = Color.Secondary,
                            }
                            };
                        } ) as string;

            if ( result is null || result == "Cancel" )
                return;

            if ( result == "EditSeries" )
            {
                await Edit( GetParentItem( viewItem.Item ) );
            }
            else if ( result == "EditOccurrence" )
            {
                await EditOccurrence( viewItem );
            }
        }
        else
        {
            await Edit( viewItem.Item );
        }

        await ItemClicked.InvokeAsync( new( viewItem.Item ) );
    }

    internal async Task NotifyDeleteItemClicked( SchedulerItemViewInfo<TItem> viewItem )
    {
        if ( viewItem.IsRecurring )
        {
            var result = await MessageService.Choose(
                    message: "Item is a recurring series. What do you want to do?",
                    title: Localizer.Localize( Localizers?.DeleteLocalizer, "Delete" ), options =>
                    {
                        options.ShowCloseButton = false;
                        options.ShowMessageIcon = false;
                        options.Choices = new List<MessageOptionsChoice>
                        {
                            new MessageOptionsChoice
                            {
                                Key = "DeleteSeries",
                                Text = Localizer.Localize( Localizers?.DeleteSeriesLocalizer, "Delete Series" ),
                                Color = Color.Danger,
                            },
                            new MessageOptionsChoice
                            {
                                Key = "DeleteOccurrence",
                                Text = Localizer.Localize( Localizers?.DeleteOccurrenceLocalizer, "Delete Occurrence" ),
                                Color = Color.Warning,
                            },
                            new MessageOptionsChoice
                            {
                                Key = "Cancel",
                                Text = Localizer.Localize( Localizers?.CancelLocalizer, "Cancel" ),
                                Color = Color.Secondary,
                            }
                        };
                    } ) as string;

            if ( result is null || result == "Cancel" )
                return;

            if ( result == "DeleteSeries" )
            {
                await DeleteItemImpl( viewItem.Item );
            }
            else if ( result == "DeleteOccurrence" )
            {
                await DeleteOccurrenceImpl( viewItem );
            }
        }
        else
        {
            var hasRecurrenceRule = !string.IsNullOrEmpty( GetItemRecurrenceRule( viewItem.Item ) );

            var deleteMessage = hasRecurrenceRule
                ? Localizer.Localize( Localizers?.SeriesDeleteConfirmationTextLocalizer, "Item is a recurring series. Are you sure you want to delete all occurrences?" )
                : Localizer.Localize( Localizers?.ItemDeleteConfirmationLocalizer, "Item will be deleted permanently. Are you sure?" );

            if ( await MessageService.Confirm( deleteMessage, Localizer.Localize( Localizers?.DeleteLocalizer, "Delete" ), options =>
            {
                options.ShowCloseButton = false;
                options.ShowMessageIcon = false;
                options.CancelButtonText = Localizer.Localize( Localizers?.CancelLocalizer, "Cancel" );
                options.ConfirmButtonText = Localizer.Localize( Localizers?.DeleteLocalizer, "Delete" );
                options.ConfirmButtonColor = Color.Danger;
            } ) == false )
                return;

            await DeleteItemImpl( viewItem.Item );
        }

        await ItemClicked.InvokeAsync( new( viewItem.Item ) );
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
    /// <param name="item">The item to be created and potentially edited in a modal.</param>
    /// <returns>Returns a Task representing the asynchronous operation.</returns>
    public async Task New( TItem item )
    {
        if ( item is null )
            throw new ArgumentNullException( nameof( item ), "New item is not assigned." );

        editItem = item;
        editState = SchedulerEditState.New;

        if ( Editable && UseInternalEditing && schedulerItemModalRef is not null )
        {
            await schedulerItemModalRef.ShowModal( item.DeepClone(), editState );
        }
    }

    /// <summary>
    /// Sets the <see cref="Scheduler{TItem}"/> into the Edit state mode for the specified item. 
    /// </summary>
    /// <param name="item">Item for which to set the edit mode.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Edit( TItem item )
    {
        if ( item is null )
            throw new ArgumentNullException( nameof( item ), "Edit item is not assigned." );

        editItem = item;
        editState = SchedulerEditState.Edit;

        if ( Editable && UseInternalEditing && schedulerItemModalRef is not null )
        {
            await schedulerItemModalRef.ShowModal( item.DeepClone(), editState );
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
            await DeleteItemImpl( item );
        }
    }

    protected internal async Task<bool> DeleteOccurrence( SchedulerItemViewInfo<TItem> viewItem )
    {
        if ( Editable && UseInternalEditing && Data is ICollection<TItem> data )
        {
            var result = await MessageService.Choose(
                    message: "Item is a recurring series. What do you want to do?",
                    title: Localizer.Localize( Localizers?.DeleteLocalizer, "Delete" ), options =>
                    {
                        options.ShowCloseButton = false;
                        options.ShowMessageIcon = false;
                        options.Choices = new List<MessageOptionsChoice>
                        {
                            new MessageOptionsChoice
                            {
                                Key = "DeleteSeries",
                                Text = Localizer.Localize( Localizers?.DeleteSeriesLocalizer, "Delete Series" ),
                                Color = Color.Danger,
                            },
                            new MessageOptionsChoice
                            {
                                Key = "DeleteOccurrence",
                                Text = Localizer.Localize( Localizers?.DeleteOccurrenceLocalizer, "Delete Occurrence" ),
                                Color = Color.Warning,
                            },
                            new MessageOptionsChoice
                            {
                                Key = "Cancel",
                                Text = Localizer.Localize( Localizers?.CancelLocalizer, "Cancel" ),
                                Color = Color.Secondary,
                            }
                        };
                    } ) as string;

            if ( result is null || result == "Cancel" )
                return false;

            if ( result == "DeleteSeries" )
            {
                await DeleteItemImpl( viewItem.Item );
            }
            else if ( result == "DeleteOccurrence" )
            {
                await DeleteOccurrenceImpl( viewItem );
            }

            await Refresh();

            return true;
        }

        return false;
    }

    public async Task EditOccurrence( SchedulerItemViewInfo<TItem> viewItem )
    {
        if ( Editable && UseInternalEditing && Data is ICollection<TItem> data && schedulerItemOccurrenceModalRef is not null )
        {
            // we are actually editing the original item, but with exception rules
            editItem = GetParentItem( viewItem.Item );
            editState = SchedulerEditState.Edit;

            await schedulerItemOccurrenceModalRef.ShowModal( viewItem.Item.DeepClone() );
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

    protected internal async Task<bool> DeleteItemImpl( TItem itemToDelete )
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

    protected internal async Task<bool> SaveOccurrenceImpl( TItem recurringItem )
    {
        var editItemClone = editItem.DeepClone();

        HandleRecurrenceException( editItemClone, recurringItem );

        return await SaveImpl( editItemClone );
    }

    protected internal void HandleRecurrenceException( TItem item, TItem recurringItem )
    {
        var recurrenceExceptions = propertyMapper.GetRecurrenceExceptions( item ) ?? new List<TItem>();

        var recurrenceException = recurrenceExceptions.FirstOrDefault( x => propertyMapper.GetOriginalStart( x ) == propertyMapper.GetOriginalStart( recurringItem ) );

        if ( recurrenceException is not null )
        {
            CopyOccurrenceValues( recurringItem, recurrenceException );
        }
        else
        {
            recurrenceExceptions.Add( recurringItem );
        }

        propertyMapper.SetRecurrenceExceptions( item, recurrenceExceptions );
    }


    protected internal async Task<bool> DeleteOccurrenceImpl( SchedulerItemViewInfo<TItem> viewItem )
    {
        if ( viewItem is null )
            return false;

        editItem = GetParentItem( viewItem.Item );

        if ( editItem is null )
            return false;

        editState = SchedulerEditState.Edit;

        var editItemClone = editItem.DeepClone();

        var deletedOccurrences = propertyMapper.GetDeletedOccurrences( editItemClone ) ?? new List<DateTime>();

        if ( !deletedOccurrences.Contains( viewItem.ViewStart ) )
        {
            deletedOccurrences.Add( viewItem.ViewStart );
            propertyMapper.SetDeletedOccurrences( editItemClone, deletedOccurrences );
        }

        return await SaveImpl( editItemClone );
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
    protected internal TItem CreateNewItem()
       => NewItemCreator is not null ? NewItemCreator.Invoke() : newItemCreator.Value();

    /// <summary>
    /// Generates a new identifier using a specified creator if available; otherwise, it generates a new GUID as a string.
    /// </summary>
    /// <returns>Returns a string representation of a new identifier.</returns>
    protected internal object CreateNewId()
        => NewIdCreator is not null ? NewIdCreator.Invoke() : Guid.NewGuid().ToString();

    /// <summary>
    /// Copies values from one appointment to another.
    /// </summary>
    /// <param name="source">The source appointment to copy values from.</param>
    /// <param name="destination">The destination appointment to copy values to.</param>
    internal void CopyItemValues( TItem source, TItem destination )
    {
        propertyMapper.SetId( destination, propertyMapper.GetId( source ) );
        propertyMapper.SetTitle( destination, propertyMapper.GetTitle( source ) );
        propertyMapper.SetDescription( destination, propertyMapper.GetDescription( source ) );
        propertyMapper.SetStart( destination, propertyMapper.GetStart( source ) );
        propertyMapper.SetEnd( destination, propertyMapper.GetEnd( source ) );
        propertyMapper.SetAllDay( destination, propertyMapper.GetAllDay( source ) );
        propertyMapper.SetRecurrenceRule( destination, propertyMapper.GetRecurrenceRule( source ) );
        propertyMapper.SetRecurrenceId( destination, propertyMapper.GetRecurrenceId( source ) );
        propertyMapper.SetDeletedOccurrences( destination, propertyMapper.GetDeletedOccurrences( source ).DeepClone() );
        propertyMapper.SetOriginalStart( destination, propertyMapper.GetOriginalStart( source ) );
        propertyMapper.SetRecurrenceExceptions( destination, propertyMapper.GetRecurrenceExceptions( source ).DeepClone() );
    }

    /// <summary>
    /// Copies various properties from one item to another, including title, description, start and end times, and all-day status.
    /// </summary>
    /// <param name="source">The item from which properties are copied.</param>
    /// <param name="destination">The item to which properties are assigned.</param>
    internal void CopyOccurrenceValues( TItem source, TItem destination )
    {
        propertyMapper.SetId( destination, propertyMapper.GetId( source ) );
        propertyMapper.SetTitle( destination, propertyMapper.GetTitle( source ) );
        propertyMapper.SetDescription( destination, propertyMapper.GetDescription( source ) );
        propertyMapper.SetStart( destination, propertyMapper.GetStart( source ) );
        propertyMapper.SetEnd( destination, propertyMapper.GetEnd( source ) );
        propertyMapper.SetAllDay( destination, propertyMapper.GetAllDay( source ) );
        propertyMapper.SetRecurrenceId( destination, propertyMapper.GetRecurrenceId( source ) );
        propertyMapper.SetOriginalStart( destination, propertyMapper.GetOriginalStart( source ) );
    }

    private TItem CreateOccurrence( TItem item, DateTime start, DateTime end )
    {
        var occurrenceItem = item.DeepClone();

        PropertyMapper.SetId( occurrenceItem, CreateNewId() );
        propertyMapper.SetRecurrenceId( occurrenceItem, GetItemId( item ) );
        propertyMapper.SetRecurrenceRule( occurrenceItem, null );
        propertyMapper.SetStart( occurrenceItem, start );
        propertyMapper.SetEnd( occurrenceItem, end );
        propertyMapper.SetOriginalStart( occurrenceItem, start );

        return occurrenceItem;
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

        // items that are created prior to the viewStart and have a recurrence rule
        var itemsWithRecurrenceRule = from item in Data
                                      let recurrenceRule = GetItemRecurrenceRule( item )
                                      where !string.IsNullOrEmpty( recurrenceRule )
                                      let allDay = GetItemAllDay( item )
                                      let start = GetItemStartTime( item )
                                      let end = GetItemEndTime( item )
                                      where start < viewStart
                                      select new SchedulerItemInfo<TItem>( item, start, end, allDay, recurrenceRule, false );

        var recurringItems = new List<SchedulerItemInfo<TItem>>();

        foreach ( var item in itemsWithRecurrenceRule )
        {
            // Generate virtual items based on recurrence rule
            var occurrences = GenerateOccurrences( item.Start, item.End, item.RecurrenceRule, viewStart, viewEnd );

            var deletedOccurrences = propertyMapper.GetDeletedOccurrences( item.Item );

            foreach ( var occurrenceStart in occurrences )
            {
                if ( deletedOccurrences is not null && deletedOccurrences.Contains( occurrenceStart ) )
                    continue;

                var occurrenceEnd = occurrenceStart.Add( item.Duration );

                if ( HandleRecurrenceExceptions( item.Item, occurrenceStart, recurringItems ) )
                    continue;

                var occurrenceItem = CreateOccurrence( item.Item, occurrenceStart, occurrenceEnd );

                recurringItems.Add( new SchedulerItemInfo<TItem>( occurrenceItem, occurrenceStart, occurrenceEnd, false, item.RecurrenceRule, true ) );
            }
        }

        foreach ( var item in itemsInView )
        {
            if ( item.RecurrenceRule is null )
                continue;

            // Generate virtual items based on recurrence rule
            var occurrences = GenerateOccurrences( item.Start, item.End, item.RecurrenceRule, viewStart, viewEnd );

            var deletedOccurrences = propertyMapper.GetDeletedOccurrences( item.Item );

            foreach ( var occurrenceStart in occurrences )
            {
                if ( deletedOccurrences is not null && deletedOccurrences.Contains( occurrenceStart ) )
                    continue;

                var occurrenceEnd = occurrenceStart.Add( item.Duration );

                if ( HandleRecurrenceExceptions( item.Item, occurrenceStart, recurringItems ) )
                    continue;

                var occurrenceItem = CreateOccurrence( item.Item, occurrenceStart, occurrenceEnd );

                recurringItems.Add( new SchedulerItemInfo<TItem>( occurrenceItem, occurrenceStart, occurrenceEnd, false, item.RecurrenceRule, true ) );
            }
        }

        // If the item is having a recurring rule that means we want to show a series and we want to hide original item.
        return itemsInView
            .Where( x => x.RecurrenceRule is null )
            .Concat( recurringItems );
    }

    /// <summary>
    /// Handles exceptions for recurring items by checking for specific occurrences and adding them to a list if found.
    /// </summary>
    /// <param name="item">Represents the item for which recurrence exceptions are being handled.</param>
    /// <param name="occurrenceStart">Indicates the start time of the occurrence being checked for exceptions.</param>
    /// <param name="recurringItems">Holds the list of recurring items to which exceptions will be added if found.</param>
    /// <returns>Returns a boolean indicating whether an exception was found and added to the list.</returns>
    private bool HandleRecurrenceExceptions( TItem item, DateTime occurrenceStart, List<SchedulerItemInfo<TItem>> recurringItems )
    {
        var recurrenceExceptions = propertyMapper.GetRecurrenceExceptions( item );

        if ( recurrenceExceptions is not null )
        {
            var recurrenceException = recurrenceExceptions.FirstOrDefault( x => propertyMapper.GetOriginalStart( x ) == occurrenceStart );

            if ( recurrenceException is not null )
            {
                recurringItems.Add( new SchedulerItemInfo<TItem>( recurrenceException, GetItemStartTime( recurrenceException ), GetItemEndTime( recurrenceException ), false, true ) );

                return true;
            }
        }

        return false;
    }

    protected internal bool IsRecurrenceItem( TItem item )
    {
        var originalStart = propertyMapper.GetOriginalStart( item );
        var recurrenceId = propertyMapper.GetRecurrenceId( item );
        var recurrenceRule = propertyMapper.GetRecurrenceRule( item );
        return originalStart != null && recurrenceId != null && recurrenceRule == null;
    }

    private IEnumerable<DateTime> GenerateOccurrences( DateTime start, DateTime end, SchedulerRecurrenceRule recurrenceRule, DateTime viewStart, DateTime viewEnd )
    {
        if ( recurrenceRule.Pattern == SchedulerRecurrencePattern.Daily )
        {
            return RecurringRuleCalculators.GetDailyRecurringDates( start, viewStart, viewEnd, recurrenceRule );
        }
        else if ( recurrenceRule.Pattern == SchedulerRecurrencePattern.Weekly )
        {
            return RecurringRuleCalculators.GetWeeklyRecurringDates( start, viewStart, viewEnd, FirstDayOfWeek, recurrenceRule );
        }
        else if ( recurrenceRule.Pattern == SchedulerRecurrencePattern.Monthly )
        {
            return RecurringRuleCalculators.GetMonthlyRecurringDates( start, viewStart, viewEnd, FirstDayOfWeek, recurrenceRule );
        }
        else if ( recurrenceRule.Pattern == SchedulerRecurrencePattern.Yearly )
        {
            return RecurringRuleCalculators.GetYearlyRecurringDates( start, viewStart, viewEnd, FirstDayOfWeek, recurrenceRule );
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

        // Set the edit item and state
        editItem = GetParentItem( item );
        editState = SchedulerEditState.Edit;

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

        editItem = default;
        editState = SchedulerEditState.None;

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
    /// Provides access to the <see cref="SchedulerPropertyMapper{TItem}"/> instance, which is responsible for mapping properties of the item type.
    /// </summary>
    protected internal SchedulerPropertyMapper<TItem> PropertyMapper => propertyMapper;

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

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the recurrence rule string in <see href="https://icalendar.org/RFC-Specifications/iCalendar-RFC-5545/">iCalendar (RFC 5545)</see> compliance rules.
    /// </summary>
    [Parameter] public string RecurrenceRuleField { get; set; } = "RecurrenceRule";

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the recurrence ID of the appointment. This is used to identify the master appointment in a recurring series.
    /// </summary>
    [Parameter] public string RecurrenceIdField { get; set; } = "RecurrenceId";

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the original start date of the appointment. This is used to store the original start date of a recurring series.
    /// </summary>
    [Parameter] public string OriginalStartField { get; set; } = "OriginalStart";

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the recurrence exceptions. This is used to store the dates of the exceptions in a recurring series.
    /// </summary>
    [Parameter] public string RecurrenceExceptionsField { get; set; } = "RecurrenceExceptions";

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the deleted occurrences. This is used to store the dates of the deleted occurrences in a recurring series.
    /// </summary>
    [Parameter] public string DeletedOccurrencesField { get; set; } = "DeletedOccurrences";

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
    /// Defines a function that creates a new ID for the item. It allows for custom ID generation logic.
    /// </summary>
    [Parameter] public Func<object> NewIdCreator { get; set; }

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