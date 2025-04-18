#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Localization;
using Blazorise.Scheduler.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler.Components;

public partial class _SchedulerIItemOccurrenceModal<TItem> : BaseComponent, IDisposable
{
    #region Members

    private Modal modalRef;

    private Validations validationsRef;

    private List<string> customValidationErrors = new();

    #endregion

    #region Methods

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
        var startValue = ( e.Value as DateOnly[] )?[0];

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

    protected async Task Delete()
    {
        if ( await MessageService.Confirm( Localizer.Localize( Scheduler.Localizers?.OccurrenceDeleteConfirmationLocalizer, "Are you sure you want to delete occurrence?" ),
            Localizer.Localize( Scheduler.Localizers?.DeleteLocalizer, "Delete" ), options =>
        {
            options.ShowCloseButton = false;
            options.ShowMessageIcon = false;
            options.CancelButtonText = Localizer.Localize( Scheduler.Localizers?.CancelLocalizer, "Cancel" );
            options.ConfirmButtonText = Localizer.Localize( Scheduler.Localizers?.DeleteLocalizer, "Delete" );
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

    string ModalTitle
        => Localizer.Localize( Scheduler.Localizers?.EditOccurrenceLocalizer, "Edit Occurrence" );

    protected bool IdAvailable => Scheduler.PropertyMapper.HasId;

    protected bool TitleAvailable => Scheduler.PropertyMapper.HasTitle;

    protected bool DescriptionAvailable => Scheduler.PropertyMapper.HasDescription;

    protected bool StartAvailable => Scheduler.PropertyMapper.HasStart;

    protected bool EndAvailable => Scheduler.PropertyMapper.HasEnd;

    protected bool AllDayAvailable => Scheduler.PropertyMapper.HasAllDay;

    protected string Title { get; set; }

    protected string Description { get; set; }

    protected DateOnly StartDate { get; set; }

    protected TimeOnly StartTime { get; set; }

    protected DateOnly EndDate { get; set; }

    protected TimeOnly EndTime { get; set; }

    protected bool AllDay { get; set; }

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
    /// Occurs when the user clicks the save button.
    /// </summary>
    [Parameter] public Func<TItem, Task<bool>> SaveOccurrenceRequested { get; set; }

    /// <summary>
    /// Occurs when the user clicks the delete button.
    /// </summary>
    [Parameter] public Func<TItem, Task<bool>> DeleteOccurrenceRequested { get; set; }

    /// <summary>
    /// Defines the first day of the week.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }

    #endregion
}
