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
/// Represents a modal dialog for editing or deleting a single occurrence of a recurring scheduler item.
/// </summary>
/// <typeparam name="TItem">
/// The type of the item used in the scheduler. This supports binding to user-defined data models for occurrence-specific edits.
/// </typeparam>
public partial class _SchedulerItemOccurrenceModal<TItem> : BaseComponent, IDisposable
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
    /// Handles localization updates and triggers UI refresh.
    /// </summary>
    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Validates that the title is not empty.
    /// </summary>
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
    /// Validates that the start date is before the end date.
    /// </summary>
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
    /// Validates that the end date is after the start date.
    /// </summary>
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
    /// Validates that the start time is before the end time.
    /// </summary>
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
    /// Validates that the end time is after the start time.
    /// </summary>
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
    /// Updates the start date and adjusts the end date if it is earlier than the new start date.
    /// </summary>
    /// <param name="value">Represents the new start date to be set.</param>
    /// <returns>Completes a task indicating the operation has finished.</returns>
    private Task OnStartDateChanged( DateOnly value )
    {
        StartDate = value;

        if ( StartDate > EndDate )
        {
            EndDate = StartDate;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates the start time and adjusts the end time accordingly.
    /// </summary>
    /// <param name="value">Represents the new start time to be set.</param>
    /// <returns>Completes a task indicating the operation has finished.</returns>
    private Task OnStartTimeChanged( TimeOnly value )
    {
        var diff = value - StartTime;

        StartTime = value;

        EndTime = EndTime.Add( diff );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Displays the modal with item details filled from the current occurrence.
    /// </summary>
    /// <param name="item">The item occurrence to edit.</param>
    public Task ShowModal( TItem item )
    {
        customValidationErrors.Clear();

        EditItem = item;

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

        return modalRef.Show();
    }

    /// <summary>
    /// Cancels editing and hides the modal.
    /// </summary>
    protected async Task Cancel()
    {
        customValidationErrors.Clear();

        await modalRef.Hide();
    }

    /// <summary>
    /// Validates the input and applies changes to the edited occurrence.
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

            var result = await SaveOccurrenceRequested.Invoke( EditItem );

            if ( result )
            {
                await modalRef.Hide();
            }
        }
    }

    /// <summary>
    /// Confirms and deletes the current item occurrence.
    /// </summary>
    protected async Task Delete()
    {
        if ( await MessageService.Confirm( Localizer.Localize( Scheduler.Localizers?.OccurrenceDeleteConfirmationLocalizer, LocalizationConstants.DeleteOccurrenceConfirmation ),
            Localizer.Localize( Scheduler.Localizers?.DeleteLocalizer, LocalizationConstants.Delete ), options =>
        {
            options.ShowCloseButton = false;
            options.ShowMessageIcon = false;
            options.CancelButtonText = Localizer.Localize( Scheduler.Localizers?.CancelLocalizer, LocalizationConstants.Cancel );
            options.ConfirmButtonText = Localizer.Localize( Scheduler.Localizers?.DeleteLocalizer, LocalizationConstants.Delete );
            options.ConfirmButtonColor = Color.Danger;
        } ) == false )
            return;

        var result = await DeleteOccurrenceRequested.Invoke( EditItem );

        if ( result )
        {
            await modalRef.Hide();
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the localized modal title for editing an occurrence.
    /// </summary>
    string ModalTitle
        => Localizer.Localize( Scheduler.Localizers?.EditOccurrenceLocalizer, LocalizationConstants.EditOccurrence );

    /// <summary>
    /// Indicates whether the item ID is available for mapping.
    /// </summary>
    protected bool IdAvailable => Scheduler.PropertyMapper.HasId;

    /// <summary>
    /// Indicates whether the title is available for mapping.
    /// </summary>
    protected bool TitleAvailable => Scheduler.PropertyMapper.HasTitle;

    /// <summary>
    /// Indicates whether the description is available for mapping.
    /// </summary>
    protected bool DescriptionAvailable => Scheduler.PropertyMapper.HasDescription;

    /// <summary>
    /// Indicates whether the start datetime is available for mapping.
    /// </summary>
    protected bool StartAvailable => Scheduler.PropertyMapper.HasStart;

    /// <summary>
    /// Indicates whether the end datetime is available for mapping.
    /// </summary>
    protected bool EndAvailable => Scheduler.PropertyMapper.HasEnd;

    /// <summary>
    /// Indicates whether the all-day flag is available for mapping.
    /// </summary>
    protected bool AllDayAvailable => Scheduler.PropertyMapper.HasAllDay;

    /// <summary>
    /// Gets or sets the appointment title.
    /// </summary>
    protected string Title { get; set; }

    /// <summary>
    /// Gets or sets the appointment description.
    /// </summary>
    protected string Description { get; set; }

    /// <summary>
    /// Gets or sets the start date of the occurrence.
    /// </summary>
    protected DateOnly StartDate { get; set; }

    /// <summary>
    /// Gets or sets the start time of the occurrence.
    /// </summary>
    protected TimeOnly StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end date of the occurrence.
    /// </summary>
    protected DateOnly EndDate { get; set; }

    /// <summary>
    /// Gets or sets the end time of the occurrence.
    /// </summary>
    protected TimeOnly EndTime { get; set; }

    /// <summary>
    /// Gets or sets whether the occurrence is all-day.
    /// </summary>
    protected bool AllDay { get; set; }

    /// <summary>
    /// Gets or sets the occurrence item being edited.
    /// </summary>
    protected TItem EditItem { get; set; }

    /// <summary>
    /// Injects a message service used to show confirmation dialogs.
    /// </summary>
    [Inject] private IMessageService MessageService { get; set; }

    /// <summary>
    /// Injects a scheduler-specific text localizer.
    /// </summary>
    [Inject] protected ITextLocalizer<Scheduler<TItem>> Localizer { get; set; }

    /// <summary>
    /// Injects a global localization change tracker service.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Gets or sets the scheduler instance cascading to this modal.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Called when the user saves the edited occurrence.
    /// </summary>
    [Parameter] public Func<TItem, Task<bool>> SaveOccurrenceRequested { get; set; }

    /// <summary>
    /// Called when the user requests deletion of the occurrence.
    /// </summary>
    [Parameter] public Func<TItem, Task<bool>> DeleteOccurrenceRequested { get; set; }

    /// <summary>
    /// Gets or sets the first day of the week, for recurrence/calendar logic.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }

    #endregion
}
