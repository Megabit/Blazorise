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

public partial class _SchedulerModal<TItem> : BaseComponent, IDisposable
{
    #region Members

    private Modal modalRef;

    private Func<TItem, object> getIdValue;
    private Action<TItem, object> setIdValue;

    private Func<TItem, string> getTitleValue;
    private Action<TItem, string> setTitleValue;

    private Func<TItem, string> getDescriptionValue;
    private Action<TItem, string> setDescriptionValue;

    private Func<TItem, DateTime> getStartValue;
    private Action<TItem, DateTime> setStartValue;

    private Func<TItem, DateTime> getEndValue;
    private Action<TItem, DateTime> setEndValue;

    private Func<TItem, bool> getAllDayFunc;
    private Action<TItem, bool> setAllDayFunc;

    private Func<TItem, string> getRecurrenceRuleFunc;
    private Action<TItem, string> setRecurrenceRuleFunc;

    private Validations validationsRef;

    private List<string> customValidationErrors = new();

    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        if ( typeof( TItem ).GetProperty( IdField )?.PropertyType is not null )
        {
            getIdValue = SchedulerFunctionCompiler.CreateValueGetter<TItem>( IdField );
            setIdValue = SchedulerFunctionCompiler.CreateValueSetter<TItem>( IdField );
        }

        if ( typeof( TItem ).GetProperty( TitleField )?.PropertyType is not null )
        {
            getTitleValue = SchedulerFunctionCompiler.CreateValueGetter<TItem, string>( TitleField );
            setTitleValue = SchedulerFunctionCompiler.CreateValueSetter<TItem, string>( TitleField );
        }

        if ( typeof( TItem ).GetProperty( DescriptionField )?.PropertyType is not null )
        {
            getDescriptionValue = SchedulerFunctionCompiler.CreateValueGetter<TItem, string>( DescriptionField );
            setDescriptionValue = SchedulerFunctionCompiler.CreateValueSetter<TItem, string>( DescriptionField );
        }

        if ( typeof( TItem ).GetProperty( StartField )?.PropertyType is not null )
        {
            getStartValue = SchedulerFunctionCompiler.CreateValueGetter<TItem, DateTime>( StartField );
            setStartValue = SchedulerFunctionCompiler.CreateValueSetter<TItem, DateTime>( StartField );
        }

        if ( typeof( TItem ).GetProperty( EndField )?.PropertyType is not null )
        {
            getEndValue = SchedulerFunctionCompiler.CreateValueGetter<TItem, DateTime>( EndField );
            setEndValue = SchedulerFunctionCompiler.CreateValueSetter<TItem, DateTime>( EndField );
        }

        if ( typeof( TItem ).GetProperty( AllDayField )?.PropertyType is not null )
        {
            getAllDayFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, bool>( AllDayField );
            setAllDayFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, bool>( AllDayField );
        }

        if ( typeof( TItem ).GetProperty( RecurrenceRuleField )?.PropertyType is not null )
        {
            getRecurrenceRuleFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, string>( RecurrenceRuleField );
            setRecurrenceRuleFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, string>( RecurrenceRuleField );
        }

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

    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    private void OnValidateTitle( ValidatorEventArgs e )
    {
        var value = e.Value as string;

        e.Status = string.IsNullOrEmpty( value )
            ? ValidationStatus.Error
            : ValidationStatus.None;

        e.ErrorText = e.Status == ValidationStatus.Error
            ? "Title is required."
            : null;
    }

    private void OnValidateStartDate( ValidatorEventArgs e )
    {
        var startValue = (e.Value as DateOnly[] )?[0];

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
            ? $"Start date cannot be higher {( AllDay ? null : "or equal " )}than the end date."
            : null;
    }

    private void OnValidateEndDate( ValidatorEventArgs e )
    {
        var endValue = ( e.Value as DateOnly[] )?[0];

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
            ? $"End date cannot be lower {( AllDay ? null : "or equal " )}than the start date."
            : null;
    }

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

    public Task ShowModal( TItem item, bool isNewItem )
    {
        customValidationErrors.Clear();

        EditItem = item;
        IsNewItem = isNewItem;

        if ( TitleAvailable )
        {
            Title = getTitleValue?.Invoke( EditItem );
        }

        if ( DescriptionAvailable )
        {
            Description = getDescriptionValue?.Invoke( EditItem );
        }

        if ( StartAvailable )
        {
            var start = (DateTime?)getStartValue?.Invoke( EditItem );
            StartDate = DateOnly.FromDateTime( start.Value );
            StartTime = TimeOnly.FromDateTime( start.Value );
        }

        if ( EndAvailable )
        {
            var end = (DateTime?)getEndValue?.Invoke( EditItem );
            EndDate = DateOnly.FromDateTime( end.Value );
            EndTime = TimeOnly.FromDateTime( end.Value );
        }

        if ( AllDayAvailable )
        {
            AllDay = getAllDayFunc?.Invoke( EditItem ) ?? false;
        }

        if ( RecurrenceRuleAvailable )
        {
            RecurrenceRule = getRecurrenceRuleFunc?.Invoke( EditItem );
        }

        return modalRef.Show();
    }

    protected async Task Cancel()
    {
        customValidationErrors.Clear();

        await modalRef.Hide();
    }

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
                    customValidationErrors.Add( "End date cannot be lower than the start date" );
                else if ( !AllDay && end <= start )
                    customValidationErrors.Add( "End time cannot be lower than the start time" );
            }

            if ( customValidationErrors.Any() )
                return;

            if ( IdAvailable )
            {
                var id = getIdValue?.Invoke( EditItem );

                if ( id is null )
                    setIdValue?.Invoke( EditItem, Guid.NewGuid().ToString() );
            }

            if ( TitleAvailable )
            {
                setTitleValue?.Invoke( EditItem, Title );
            }

            if ( DescriptionAvailable )
            {
                setDescriptionValue?.Invoke( EditItem, Description );
            }

            if ( StartAvailable )
            {
                setStartValue?.Invoke( EditItem, start );
            }

            if ( EndAvailable )
            {
                setEndValue?.Invoke( EditItem, end );
            }

            if ( AllDayAvailable )
            {
                setAllDayFunc?.Invoke( EditItem, AllDay );
            }

            if ( RecurrenceRuleAvailable )
            {
                setRecurrenceRuleFunc?.Invoke( EditItem, RecurrenceRule );
            }

            var result = await SaveRequested.Invoke( EditItem );

            if ( result )
            {
                await modalRef.Hide();
            }
        }
    }

    protected async Task Delete()
    {
        var isSeries = !string.IsNullOrEmpty( Scheduler.GetItemRecurrenceRule( EditItem ) );

        var deleteMessage = isSeries
            ? Localizer.Localize( Scheduler.Localizers?.SeriesDeleteConfirmationTextLocalizer, "Item is a recurring series. Are you sure you want to delete all occurrences?" )
            : Localizer.Localize( Scheduler.Localizers?.ItemDeleteConfirmationLocalizer, "Item will be deleted permanently, are you sure?" );

        if ( await MessageService.Confirm( deleteMessage, "Delete", options =>
        {
            options.ShowCloseButton = false;
            options.ShowMessageIcon = false;
            options.CancelButtonText = "Cancel";
            options.ConfirmButtonText = "Delete";
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

    string ModalTitle
        => $"{Localizer.Localize( Scheduler.Localizers?.TitleLocalizer, IsNewItem ? "New" : "Edit" )} {Localizer.Localize( Scheduler.Localizers?.AppointmentLocalizer, "Appointment" )}";

    protected bool IsNewItem { get; set; }

    protected bool IdAvailable => getIdValue is not null;

    protected bool TitleAvailable => getTitleValue is not null;

    protected bool DescriptionAvailable => getDescriptionValue is not null;

    protected bool StartAvailable => getStartValue is not null;

    protected bool EndAvailable => getEndValue is not null;

    protected bool AllDayAvailable => getAllDayFunc is not null;

    protected bool RecurrenceRuleAvailable => getRecurrenceRuleFunc is not null;

    protected string Title { get; set; }

    protected string Description { get; set; }

    protected DateOnly StartDate { get; set; }

    protected TimeOnly StartTime { get; set; }

    protected DateOnly EndDate { get; set; }

    protected TimeOnly EndTime { get; set; }

    protected bool AllDay { get; set; }

    protected string RecurrenceRule { get; set; }

    protected TItem EditItem { get; set; }

    /// <summary>
    /// Injects an instance of <see cref="IMessageService"/> for handling message-related operations.
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
    /// Cascades the <see cref="Scheduler{TItem}"/> instance to the modal.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the unique identifier of the appointment. Defaults to "Id".
    /// </summary>
    [Parameter] public string IdField { get; set; }

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the start date of the appointment. Defaults to "Start".
    /// </summary>
    [Parameter] public string StartField { get; set; }

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the end date of the appointment. Defaults to "End".
    /// </summary>
    [Parameter] public string EndField { get; set; }

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the title of the appointment. Defaults to "Title".
    /// </summary>
    [Parameter] public string TitleField { get; set; }

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the description of the appointment. Defaults to "Description".
    /// </summary>
    [Parameter] public string DescriptionField { get; set; }

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the all day flag of the appointment. Defaults to "AllDay".
    /// </summary>
    [Parameter] public string AllDayField { get; set; }

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the recurrence rule of the appointment. Defaults to "RecurrenceRule".
    /// </summary>
    [Parameter] public string RecurrenceRuleField { get; set; }

    /// <summary>
    /// Occurs when the user clicks the save button.
    /// </summary>
    [Parameter] public Func<TItem, Task<bool>> SaveRequested { get; set; }

    /// <summary>
    /// Occurs when the user clicks the delete button.
    /// </summary>
    [Parameter] public Func<TItem, Task<bool>> DeleteRequested { get; set; }

    /// <summary>
    /// Defines the first day of the week.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }

    #endregion
}
