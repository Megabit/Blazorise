#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Localization;
using Blazorise.Scheduler.Extensions;
using Blazorise.Scheduler.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler.Components;

/// <summary>
/// Represents a modal dialog for creating or editing scheduler items, including support for validation and recurrence.
/// </summary>
/// <typeparam name="TItem">
/// The type of the item used in the scheduler. This allows the modal to be bound to custom data models.
/// </typeparam>
public partial class _SchedulerItemModal<TItem> : BaseComponent, IDisposable
{
    #region Members

    private Modal modalRef;

    private Validations validationsRef;

    private List<string> customValidationErrors = new();

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            LocalizerService.LocalizationChanged -= OnLocalizationChanged;
        }
    }

    /// <summary>
    /// Handles changes in localization by triggering a UI refresh.
    /// </summary>
    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Validates that the title is not empty.
    /// </summary>
    /// <param name="e">Validation arguments containing the input value.</param>
    private void OnValidateTitle( ValidatorEventArgs e )
    {
        var value = e.Value as string;

        e.Status = string.IsNullOrEmpty( value )
            ? ValidationStatus.Error
            : ValidationStatus.None;

        e.ErrorText = e.Status == ValidationStatus.Error
            ? Localizer.Localize( Scheduler.Localizers?.TitleRequiredValidationLocalizer, LocalizationConstants.TitleRequired )
            : null;
    }

    /// <summary>
    /// Validates that the start date is not after (or equal to, if not all-day) the end date.
    /// </summary>
    /// <param name="e">Validation arguments containing the input value.</param>
    private void OnValidateStartDate( ValidatorEventArgs e )
    {
        var startValue = e.Value as DateOnly?;

        var start = AllDay
            ? new DateTime( startValue.Value.Year, startValue.Value.Month, startValue.Value.Day )
            : new DateTime( startValue.Value.Year, startValue.Value.Month, startValue.Value.Day, StartTime.Hour, StartTime.Minute, 0 );

        var end = AllDay
            ? new DateTime( EndDate.Year, EndDate.Month, EndDate.Day )
            : new DateTime( EndDate.Year, EndDate.Month, EndDate.Day, EndTime.Hour, EndTime.Minute, 0 );

        e.Status = ( AllDay ? start > end : start >= end )
            ? ValidationStatus.Error
            : ValidationStatus.None;

        e.ErrorText = e.Status == ValidationStatus.Error
            ? AllDay
                ? Localizer.Localize( Scheduler.Localizers?.StartDateHigherValidationLocalizer, LocalizationConstants.StartDateHigherValidation )
                : Localizer.Localize( Scheduler.Localizers?.StartDateHigherOrEqualValidationLocalizer, LocalizationConstants.StartDateHigherOrEqualValidation )
            : null;
    }

    /// <summary>
    /// Validates that the end date is not before (or equal to, if not all-day) the start date.
    /// </summary>
    /// <param name="e">Validation arguments containing the input value.</param>
    private void OnValidateEndDate( ValidatorEventArgs e )
    {
        var endValue = e.Value as DateOnly?;

        var start = AllDay
            ? new DateTime( StartDate.Year, StartDate.Month, StartDate.Day )
            : new DateTime( StartDate.Year, StartDate.Month, StartDate.Day, StartTime.Hour, StartTime.Minute, 0 );

        var end = AllDay
            ? new DateTime( endValue.Value.Year, endValue.Value.Month, endValue.Value.Day )
            : new DateTime( endValue.Value.Year, endValue.Value.Month, endValue.Value.Day, EndTime.Hour, EndTime.Minute, 0 );

        e.Status = ( AllDay ? end < start : end <= start )
            ? ValidationStatus.Error
            : ValidationStatus.None;

        e.ErrorText = e.Status == ValidationStatus.Error
            ? AllDay
                ? Localizer.Localize( Scheduler.Localizers?.EndDateLowerValidationLocalizer, LocalizationConstants.EndDateLowerValidation )
                : Localizer.Localize( Scheduler.Localizers?.EndDateLowerOrEqualValidationLocalizer, LocalizationConstants.EndDateLowerOrEqualValidation )
            : null;
    }

    /// <summary>
    /// Validates that the start time is before the end time (when not all-day).
    /// </summary>
    /// <param name="e">Validation arguments containing the input value.</param>
    private void OnValidateStartTime( ValidatorEventArgs e )
    {
        var startValue = e.Value as TimeOnly?;

        var start = AllDay
            ? new DateTime( StartDate.Year, StartDate.Month, StartDate.Day )
            : new DateTime( StartDate.Year, StartDate.Month, StartDate.Day, startValue.Value.Hour, startValue.Value.Minute, 0 );

        var end = AllDay
            ? new DateTime( EndDate.Year, EndDate.Month, EndDate.Day )
            : new DateTime( EndDate.Year, EndDate.Month, EndDate.Day, EndTime.Hour, EndTime.Minute, 0 );

        e.Status = ( AllDay ? start > end : start >= end )
            ? ValidationStatus.Error
            : ValidationStatus.None;
    }

    /// <summary>
    /// Validates that the end time is after the start time (when not all-day).
    /// </summary>
    /// <param name="e">Validation arguments containing the input value.</param>
    private void OnValidateEndTime( ValidatorEventArgs e )
    {
        var endValue = e.Value as TimeOnly?;

        var start = AllDay
            ? new DateTime( StartDate.Year, StartDate.Month, StartDate.Day )
            : new DateTime( StartDate.Year, StartDate.Month, StartDate.Day, StartTime.Hour, StartTime.Minute, 0 );

        var end = AllDay
            ? new DateTime( EndDate.Year, EndDate.Month, EndDate.Day )
            : new DateTime( EndDate.Year, EndDate.Month, EndDate.Day, endValue.Value.Hour, endValue.Value.Minute, 0 );

        e.Status = ( AllDay ? end < start : end <= start )
            ? ValidationStatus.Error
            : ValidationStatus.None;
    }

    /// <summary>
    /// Handles changes to the start date, ensuring the end date is not before it.
    /// </summary>
    /// <param name="value">The new start date value.</param>
    private Task OnStartDateChanged( DateOnly value )
    {
        StartDate = value;

        if ( StartDate > EndDate )
        {
            EndDate = StartDate;
        }

        return Task.CompletedTask;
    }

    private Task OnStartTimeChanged( TimeOnly value )
    {
        var diff = value - StartTime;

        StartTime = value;

        EndTime = EndTime.Add( diff );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Displays the modal and initializes the fields based on the given item and edit state.
    /// </summary>
    /// <param name="item">The item to edit or create.</param>
    /// <param name="editState">The edit state to apply.</param>
    /// <returns>A task that completes when the modal is shown.</returns>
    public Task ShowModal( TItem item, SchedulerEditState editState )
    {
        customValidationErrors.Clear();

        EditItem = item;
        EditState = editState;

        if ( TitleAvailable )
        {
            Title = Scheduler.PropertyMapper.GetTitle( EditItem );
        }

        if ( DescriptionAvailable )
        {
            Description = Scheduler.PropertyMapper.GetDescription( EditItem );
        }

        if ( StartAvailable )
        {
            var start = Scheduler.PropertyMapper.GetStart( EditItem );
            StartDate = DateOnly.FromDateTime( start );
            StartTime = TimeOnly.FromDateTime( start );
        }

        if ( EndAvailable )
        {
            var end = Scheduler.PropertyMapper.GetEnd( EditItem );
            EndDate = DateOnly.FromDateTime( end );
            EndTime = TimeOnly.FromDateTime( end );
        }

        if ( AllDayAvailable )
        {
            AllDay = Scheduler.PropertyMapper.GetAllDay( EditItem );
        }

        if ( RecurrenceRuleAvailable )
        {
            RecurrenceRule = Scheduler.PropertyMapper.GetRecurrenceRule( EditItem );
        }

        return modalRef.Show();
    }

    /// <summary>
    /// Cancels the operation and hides the modal dialog.
    /// </summary>
    protected async Task Cancel()
    {
        customValidationErrors.Clear();

        await modalRef.Hide();
    }

    /// <summary>
    /// Validates the form inputs and attempts to save the item.
    /// </summary>
    protected async Task Submit()
    {
        customValidationErrors.Clear();

        if ( await validationsRef.ValidateAll() )
        {
            if ( EditItem is null )
                return;

            var start = AllDay
                ? new DateTime( StartDate.Year, StartDate.Month, StartDate.Day )
                : new DateTime( StartDate.Year, StartDate.Month, StartDate.Day, StartTime.Hour, StartTime.Minute, 0 );

            var end = AllDay
                ? new DateTime( EndDate.Year, EndDate.Month, EndDate.Day )
                : new DateTime( EndDate.Year, EndDate.Month, EndDate.Day, EndTime.Hour, EndTime.Minute, 0 );


            if ( StartAvailable && EndAvailable )
            {
                if ( AllDay && end < start )
                    customValidationErrors.Add( Localizer.Localize( Scheduler.Localizers?.EndDateLowerValidationLocalizer, LocalizationConstants.EndDateLowerValidation ) );
                else if ( !AllDay && end <= start )
                    customValidationErrors.Add( Localizer.Localize( Scheduler.Localizers?.EndTimeLowerValidationLocalizer, LocalizationConstants.EndTimeLowerValidation ) );
            }

            if ( customValidationErrors.Any() )
                return;

            if ( IdAvailable )
            {
                var id = Scheduler.PropertyMapper.GetId( EditItem );

                if ( id is null )
                    Scheduler.PropertyMapper.SetId( EditItem, Scheduler.CreateNewId() );
            }

            if ( TitleAvailable )
            {
                Scheduler.PropertyMapper.SetTitle( EditItem, Title );
            }

            if ( DescriptionAvailable )
            {
                Scheduler.PropertyMapper.SetDescription( EditItem, Description );
            }

            if ( StartAvailable )
            {
                Scheduler.PropertyMapper.SetStart( EditItem, start );
            }

            if ( EndAvailable )
            {
                Scheduler.PropertyMapper.SetEnd( EditItem, end );
            }

            if ( AllDayAvailable )
            {
                Scheduler.PropertyMapper.SetAllDay( EditItem, AllDay );
            }

            if ( RecurrenceRuleAvailable )
            {
                // If the recurrence rule is set to "never", clear it as we don't want to store invalid rules.
                // We do it here so that UI can be updated before the save operation.
                if ( RecurrenceRule?.Contains( "FREQ=NEVER", StringComparison.InvariantCulture ) ?? false )
                {
                    RecurrenceRule = null;
                }

                Scheduler.PropertyMapper.SetRecurrenceRule( EditItem, RecurrenceRule );
            }

            var result = await SaveRequested.Invoke( EditItem );

            if ( result )
            {
                await modalRef.Hide();
            }
        }
    }

    /// <summary>
    /// Prompts for confirmation and deletes the current item, if confirmed.
    /// </summary>
    protected async Task Delete()
    {
        var hasRecurrenceRule = !string.IsNullOrEmpty( Scheduler.GetItemRecurrenceRule( EditItem ) );

        var deleteMessage = hasRecurrenceRule
            ? Localizer.Localize( Scheduler.Localizers?.SeriesDeleteConfirmationTextLocalizer, LocalizationConstants.DeleteSeriesConfirmation )
            : Localizer.Localize( Scheduler.Localizers?.ItemDeleteConfirmationLocalizer, LocalizationConstants.DeleteSeriesConfirmation );

        if ( await MessageService.Confirm( deleteMessage, Localizer.Localize( Scheduler.Localizers?.DeleteLocalizer, LocalizationConstants.Delete ), options =>
        {
            options.ShowCloseButton = false;
            options.ShowMessageIcon = false;
            options.CancelButtonText = Localizer.Localize( Scheduler.Localizers?.CancelLocalizer, LocalizationConstants.Cancel );
            options.ConfirmButtonText = Localizer.Localize( Scheduler.Localizers?.DeleteLocalizer, LocalizationConstants.Delete );
            options.ConfirmButtonColor = Color.Danger;
        } ) == false )
            return;

        var result = await DeleteRequested.Invoke( EditItem );

        if ( result )
        {
            await modalRef.Hide();
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the localized title displayed in the modal, based on the edit state.
    /// </summary>
    string ModalTitle
        => $"{Localizer.Localize( Scheduler.Localizers?.TitleLocalizer, EditState == SchedulerEditState.New ? LocalizationConstants.New : LocalizationConstants.Edit )} {Localizer.Localize( Scheduler.Localizers?.AppointmentLocalizer, LocalizationConstants.Appointment )}";

    /// <summary>
    /// Indicates whether the item ID is available for mapping.
    /// </summary>
    protected bool IdAvailable => Scheduler.PropertyMapper.HasId;

    /// <summary>
    /// Indicates whether the item title is available for mapping.
    /// </summary>
    protected bool TitleAvailable => Scheduler.PropertyMapper.HasTitle;

    /// <summary>
    /// Indicates whether the item description is available for mapping.
    /// </summary>
    protected bool DescriptionAvailable => Scheduler.PropertyMapper.HasDescription;

    /// <summary>
    /// Indicates whether the item start date/time is available for mapping.
    /// </summary>
    protected bool StartAvailable => Scheduler.PropertyMapper.HasStart;

    /// <summary>
    /// Indicates whether the item end date/time is available for mapping.
    /// </summary>
    protected bool EndAvailable => Scheduler.PropertyMapper.HasEnd;

    /// <summary>
    /// Indicates whether the AllDay flag is available for mapping.
    /// </summary>
    protected bool AllDayAvailable => Scheduler.PropertyMapper.HasAllDay;

    /// <summary>
    /// Indicates whether the recurrence rule is available for mapping.
    /// </summary>
    protected bool RecurrenceRuleAvailable => Scheduler.PropertyMapper.HasRecurrenceRule;

    /// <summary>
    /// Gets or sets the title of the appointment.
    /// </summary>
    protected string Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the appointment.
    /// </summary>
    protected string Description { get; set; }

    /// <summary>
    /// Gets or sets the start date of the appointment.
    /// </summary>
    protected DateOnly StartDate { get; set; }

    /// <summary>
    /// Gets or sets the start time of the appointment.
    /// </summary>
    protected TimeOnly StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end date of the appointment.
    /// </summary>
    protected DateOnly EndDate { get; set; }

    /// <summary>
    /// Gets or sets the end time of the appointment.
    /// </summary>
    protected TimeOnly EndTime { get; set; }

    /// <summary>
    /// Gets or sets whether the appointment spans the entire day.
    /// </summary>
    protected bool AllDay { get; set; }

    /// <summary>
    /// Gets or sets the recurrence rule string for the appointment.
    /// </summary>
    protected string RecurrenceRule { get; set; }

    /// <summary>
    /// Gets or sets the appointment item being edited or created.
    /// </summary>
    protected TItem EditItem { get; set; }

    /// <summary>
    /// Gets or sets the current edit state (new or existing).
    /// </summary>
    protected SchedulerEditState EditState { get; set; }

    /// <summary>
    /// Gets the minimum recurring end date based on the start date and end date.
    /// </summary>
    protected DateTime? MinRecurringEndDate
    {
        get
        {
            var start = new DateTime( StartDate.Year, StartDate.Month, StartDate.Day );
            var end = new DateTime( EndDate.Year, EndDate.Month, EndDate.Day );

            return start < end ? start : end;
        }
    }

    /// <summary>
    /// Injects an instance of <see cref="IMessageService"/> for handling message-related operations.
    /// </summary>
    [Inject] private IMessageService MessageService { get; set; }

    /// <summary>
    /// Injects an instance of <see cref="ITextLocalizer{TScope}"/> for localizing strings in the scheduler.
    /// </summary>
    [Inject] protected ITextLocalizer<Scheduler<TItem>> Localizer { get; set; }

    /// <summary>
    /// Injects the global localization service used to track language changes.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Gets or sets the cascading instance of the parent <see cref="Scheduler{TItem}"/> component.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Occurs when the user submits the form to save the appointment.
    /// </summary>
    [Parameter] public Func<TItem, Task<bool>> SaveRequested { get; set; }

    /// <summary>
    /// Occurs when the user requests deletion of the appointment.
    /// </summary>
    [Parameter] public Func<TItem, Task<bool>> DeleteRequested { get; set; }

    /// <summary>
    /// Gets or sets the first day of the week, used for recurrence options and date pickers.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }

    #endregion
}
