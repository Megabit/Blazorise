#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Localization;
using Blazorise.Gantt.Utilities;
using Microsoft.AspNetCore.Components;
using Blazorise.Gantt.Extensions;
#endregion

namespace Blazorise.Gantt.Components;

/// <summary>
/// Internal modal used for item edit and insert operations.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public partial class _GanttItemModal<TItem> : BaseComponent, IDisposable
{
    #region Members

    private Modal modalRef;

    private Validations validationsRef;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <inheritdoc />
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

        e.Status = string.IsNullOrWhiteSpace( value )
            ? ValidationStatus.Error
            : ValidationStatus.None;

        e.ErrorText = e.Status == ValidationStatus.Error
            ? Localizer.Localize( Gantt.Localizers?.TitleRequiredLocalizer, LocalizationConstants.TitleRequired )
            : null;
    }

    private void OnValidateStartDate( ValidatorEventArgs e )
    {
        if ( !TryGetDateOnlyValue( e.Value, out var startDate ) || IsUnassignedDate( startDate ) )
        {
            e.Status = ValidationStatus.Error;
            e.ErrorText = Localizer.Localize( Gantt.Localizers?.StartRequiredLocalizer, LocalizationConstants.StartRequired );
            return;
        }

        if ( IsUnassignedDate( EndDate ) )
        {
            e.Status = ValidationStatus.None;
            e.ErrorText = null;
            return;
        }

        var start = new DateTime( startDate.Year, startDate.Month, startDate.Day, StartTime.Hour, StartTime.Minute, 0 );
        var end = GetEndValue();

        SetRangeValidationResult( e, start, end );
    }

    private void OnValidateStartTime( ValidatorEventArgs e )
    {
        var startTime = e.Value as TimeOnly?;

        if ( startTime is null || IsUnassignedDate( StartDate ) || IsUnassignedDate( EndDate ) )
        {
            e.Status = ValidationStatus.None;
            e.ErrorText = null;
            return;
        }

        var start = new DateTime( StartDate.Year, StartDate.Month, StartDate.Day, startTime.Value.Hour, startTime.Value.Minute, 0 );
        var end = GetEndValue();

        SetRangeValidationResult( e, start, end );
    }

    private void OnValidateEndDate( ValidatorEventArgs e )
    {
        if ( !TryGetDateOnlyValue( e.Value, out var endDate ) || IsUnassignedDate( endDate ) )
        {
            e.Status = ValidationStatus.Error;
            e.ErrorText = Localizer.Localize( Gantt.Localizers?.EndRequiredLocalizer, LocalizationConstants.EndRequired );
            return;
        }

        if ( IsUnassignedDate( StartDate ) )
        {
            e.Status = ValidationStatus.None;
            e.ErrorText = null;
            return;
        }

        var start = GetStartValue();
        var end = new DateTime( endDate.Year, endDate.Month, endDate.Day, EndTime.Hour, EndTime.Minute, 0 );

        SetRangeValidationResult( e, start, end );
    }

    private void OnValidateEndTime( ValidatorEventArgs e )
    {
        var endTime = e.Value as TimeOnly?;

        if ( endTime is null || IsUnassignedDate( StartDate ) || IsUnassignedDate( EndDate ) )
        {
            e.Status = ValidationStatus.None;
            e.ErrorText = null;
            return;
        }

        var start = GetStartValue();
        var end = new DateTime( EndDate.Year, EndDate.Month, EndDate.Day, endTime.Value.Hour, endTime.Value.Minute, 0 );

        SetRangeValidationResult( e, start, end );
    }

    private void SetRangeValidationResult( ValidatorEventArgs e, DateTime start, DateTime end )
    {
        e.Status = end <= start
            ? ValidationStatus.Error
            : ValidationStatus.None;

        e.ErrorText = e.Status == ValidationStatus.Error
            ? Localizer.Localize( Gantt.Localizers?.EndBeforeStartLocalizer, LocalizationConstants.EndBeforeStart )
            : null;
    }

    private Task OnStartDateChanged( DateOnly value )
    {
        var previousStart = GetStartValue();
        StartDate = value;

        if ( IsUnassignedDate( StartDate ) )
            return Task.CompletedTask;

        if ( DurationAvailable )
        {
            SyncEndFromDuration();
            return Task.CompletedTask;
        }

        ShiftEndByStartDelta( previousStart );

        return Task.CompletedTask;
    }

    private Task OnStartTimeChanged( TimeOnly value )
    {
        var previousStart = GetStartValue();
        StartTime = value;

        if ( IsUnassignedDate( StartDate ) )
            return Task.CompletedTask;

        if ( DurationAvailable )
        {
            SyncEndFromDuration();
            return Task.CompletedTask;
        }

        ShiftEndByStartDelta( previousStart );

        return Task.CompletedTask;
    }

    private Task OnEndDateChanged( DateOnly value )
    {
        EndDate = value;

        if ( IsUnassignedDate( EndDate ) )
            return Task.CompletedTask;

        if ( DurationAvailable )
        {
            SyncDurationFromEnd();
            SyncEndFromDuration();
        }

        return Task.CompletedTask;
    }

    private Task OnEndTimeChanged( TimeOnly value )
    {
        EndTime = value;

        if ( IsUnassignedDate( EndDate ) || IsUnassignedDate( StartDate ) )
            return Task.CompletedTask;

        if ( DurationAvailable )
        {
            SyncDurationFromEnd();
            SyncEndFromDuration();
        }

        return Task.CompletedTask;
    }

    private Task OnDurationChanged( int value )
    {
        DurationDays = NormalizeDurationDays( value );

        if ( DurationAvailable )
        {
            SyncEndFromDuration();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Shows modal for provided item.
    /// </summary>
    public async Task ShowModal( TItem item, GanttEditState editState, TItem parentItem = default )
    {
        EditItem = item;
        EditState = editState;
        ParentItem = parentItem;

        Title = TitleAvailable ? Gantt.PropertyMapper.GetTitle( EditItem ) : null;
        Description = DescriptionAvailable ? Gantt.PropertyMapper.GetDescription( EditItem ) : null;

        var start = StartAvailable ? Gantt.PropertyMapper.GetStart( EditItem ) : DateTime.Now;
        var end = EndAvailable ? Gantt.PropertyMapper.GetEnd( EditItem ) : start.AddHours( 1 );

        if ( end <= start )
            end = start.AddHours( 1 );

        if ( DurationAvailable )
        {
            var mappedDuration = Gantt.PropertyMapper.GetDuration( EditItem );
            DurationDays = mappedDuration > 0
                ? mappedDuration
                : NormalizeDurationDays( CalculateDurationInDays( start, end ) );
            end = start.AddDays( DurationDays );
        }
        else
        {
            DurationDays = NormalizeDurationDays( CalculateDurationInDays( start, end ) );
        }

        StartDate = DateOnly.FromDateTime( start );
        StartTime = TimeOnly.FromDateTime( start );

        EndDate = DateOnly.FromDateTime( end );
        EndTime = TimeOnly.FromDateTime( end );

        if ( validationsRef is not null )
            await validationsRef.ClearAll();

        await modalRef.Show();
    }

    private async Task Cancel()
    {
        if ( validationsRef is not null )
            await validationsRef.ClearAll();

        await modalRef.Hide();
    }

    private async Task Submit()
    {
        if ( validationsRef is not null && !await validationsRef.ValidateAll() )
            return;

        var start = new DateTime( StartDate.Year, StartDate.Month, StartDate.Day, StartTime.Hour, StartTime.Minute, 0 );
        var end = new DateTime( EndDate.Year, EndDate.Month, EndDate.Day, EndTime.Hour, EndTime.Minute, 0 );
        var duration = NormalizeDurationDays( DurationDays );

        if ( DurationAvailable )
            end = start.AddDays( duration );

        if ( end <= start )
            return;

        if ( EditItem is null )
            return;

        if ( IdAvailable )
        {
            var id = Gantt.PropertyMapper.GetId( EditItem );

            if ( id is null )
                Gantt.PropertyMapper.SetId( EditItem, Gantt.CreateNewId() );
        }

        if ( ParentIdAvailable && ParentItem is not null )
        {
            Gantt.PropertyMapper.SetParentId( EditItem, Gantt.PropertyMapper.GetId( ParentItem ) );
        }

        if ( TitleAvailable )
            Gantt.PropertyMapper.SetTitle( EditItem, Title );

        if ( DescriptionAvailable )
            Gantt.PropertyMapper.SetDescription( EditItem, Description );

        if ( StartAvailable )
            Gantt.PropertyMapper.SetStart( EditItem, start );

        if ( EndAvailable )
            Gantt.PropertyMapper.SetEnd( EditItem, end );

        if ( DurationAvailable )
            Gantt.PropertyMapper.SetDuration( EditItem, duration );

        var result = await SaveRequested.Invoke( EditItem );

        if ( result )
            await modalRef.Hide();
    }

    private async Task Delete()
    {
        if ( await MessageService.Confirm( Localizer.Localize( Gantt.Localizers?.DeleteTaskConfirmationLocalizer, LocalizationConstants.DeleteTaskConfirmation ),
             Localizer.Localize( Gantt.Localizers?.DeleteLocalizer, LocalizationConstants.Delete ), options =>
             {
                 options.ShowCloseButton = false;
                 options.ShowMessageIcon = false;
                 options.CancelButtonText = Localizer.Localize( Gantt.Localizers?.CancelLocalizer, LocalizationConstants.Cancel );
                 options.ConfirmButtonText = Localizer.Localize( Gantt.Localizers?.DeleteLocalizer, LocalizationConstants.Delete );
                 options.ConfirmButtonColor = Color.Danger;
             } ) == false )
            return;

        var result = await DeleteRequested.Invoke( EditItem );

        if ( result )
            await modalRef.Hide();
    }

    private DateTime GetStartValue()
        => new( StartDate.Year, StartDate.Month, StartDate.Day, StartTime.Hour, StartTime.Minute, 0 );

    private DateTime GetEndValue()
        => new( EndDate.Year, EndDate.Month, EndDate.Day, EndTime.Hour, EndTime.Minute, 0 );

    private void SetEndValue( DateTime value )
    {
        EndDate = DateOnly.FromDateTime( value );
        EndTime = TimeOnly.FromDateTime( value );
    }

    private void ShiftEndByStartDelta( DateTime previousStart )
    {
        var currentStart = GetStartValue();
        var previousEnd = GetEndValue();
        var shiftedEnd = previousEnd.Add( currentStart - previousStart );
        SetEndValue( shiftedEnd );
    }

    private void SyncDurationFromEnd()
    {
        var start = GetStartValue();
        var end = GetEndValue();
        DurationDays = NormalizeDurationDays( CalculateDurationInDays( start, end ) );
    }

    private void SyncEndFromDuration()
    {
        var start = GetStartValue();
        var end = start.AddDays( NormalizeDurationDays( DurationDays ) );
        SetEndValue( end );
    }

    private static int CalculateDurationInDays( DateTime start, DateTime end )
    {
        var totalDays = ( end - start ).TotalDays;

        if ( totalDays <= 0d )
            return 1;

        return Math.Max( 1, (int)Math.Ceiling( totalDays ) );
    }

    private static int NormalizeDurationDays( int value )
        => Math.Max( 1, value );

    private static bool TryGetDateOnlyValue( object value, out DateOnly date )
    {
        if ( value is DateOnly dateOnlyValue )
        {
            date = dateOnlyValue;
            return true;
        }

        if ( value is DateTime dateTimeValue )
        {
            date = DateOnly.FromDateTime( dateTimeValue );
            return true;
        }

        date = default;
        return false;
    }

    private static bool IsUnassignedDate( DateOnly value )
        => value == DateOnly.MinValue || value == DateOnly.MaxValue;

    private static bool IsUnassignedDate( DateTime value )
        => value == DateTime.MinValue || value == DateTime.MaxValue;

    private string GetParentTitleText()
    {
        if ( ParentItem is null || !TitleAvailable )
            return "-";

        var title = Gantt.PropertyMapper.GetTitle( ParentItem );

        if ( string.IsNullOrWhiteSpace( title ) )
            return "-";

        return title;
    }

    private DateTime? GetParentStart()
    {
        if ( ParentItem is null || !StartAvailable )
            return null;

        var start = Gantt.PropertyMapper.GetStart( ParentItem );

        return IsUnassignedDate( start )
            ? null
            : start;
    }

    private DateTime? GetParentEnd()
    {
        if ( ParentItem is null || !EndAvailable )
            return null;

        var end = Gantt.PropertyMapper.GetEnd( ParentItem );

        return IsUnassignedDate( end )
            ? null
            : end;
    }

    private int? GetParentDurationDays()
    {
        if ( ParentItem is null )
            return null;

        if ( DurationAvailable )
        {
            var mappedDuration = Gantt.PropertyMapper.GetDuration( ParentItem );

            if ( mappedDuration > 0 )
                return mappedDuration;
        }

        var parentStart = GetParentStart();
        var parentEnd = GetParentEnd();

        if ( parentStart is null || parentEnd is null )
            return null;

        return NormalizeDurationDays( CalculateDurationInDays( parentStart.Value, parentEnd.Value ) );
    }

    private static string FormatDateTime( DateTime? value )
    {
        if ( value is null )
            return null;

        return value.Value.ToString( "g" );
    }

    #endregion

    #region Properties

    private string ModalTitle
        => $"{Localizer.Localize( EditState == GanttEditState.New ? Gantt.Localizers?.NewLocalizer : Gantt.Localizers?.EditLocalizer, EditState == GanttEditState.New ? LocalizationConstants.New : LocalizationConstants.Edit )} {Localizer.Localize( Gantt.Localizers?.TaskLocalizer, LocalizationConstants.Task )}";

    private bool IdAvailable => Gantt.PropertyMapper.HasId;

    private bool ParentIdAvailable => Gantt.PropertyMapper.HasParentId;

    private bool TitleAvailable => Gantt.PropertyMapper.HasTitle;

    private bool DescriptionAvailable => Gantt.PropertyMapper.HasDescription;

    private bool StartAvailable => Gantt.PropertyMapper.HasStart;

    private bool EndAvailable => Gantt.PropertyMapper.HasEnd;

    private bool DurationAvailable => Gantt.PropertyMapper.HasDuration;

    private bool DeleteCommandVisible
        => EditItem is not null
           && Gantt?.IsCommandAllowed( GanttCommandType.Delete, EditItem ) == true;

    private bool ShowParentDetails
        => EditState == GanttEditState.New && ParentItem is not null;

    private string ParentTitleText
        => GetParentTitleText();

    private string ParentStartText
        => FormatDateTime( GetParentStart() );

    private string ParentEndText
        => FormatDateTime( GetParentEnd() );

    private int? ParentDurationDays
        => GetParentDurationDays();

    private string Title { get; set; }

    private string Description { get; set; }

    private DateOnly StartDate { get; set; }

    private TimeOnly StartTime { get; set; }

    private DateOnly EndDate { get; set; }

    private TimeOnly EndTime { get; set; }

    private int DurationDays { get; set; } = 1;

    private TItem EditItem { get; set; }

    private TItem ParentItem { get; set; }

    private GanttEditState EditState { get; set; }

    [Inject] private IMessageService MessageService { get; set; }

    /// <summary>
    /// Gets localized texts used by the Gantt item modal UI.
    /// </summary>
    [Inject] protected ITextLocalizer<Gantt<TItem>> Localizer { get; set; }

    /// <summary>
    /// Gets localization service used to react to culture changes.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Gets parent <see cref="Gantt{TItem}"/> instance.
    /// </summary>
    [CascadingParameter] public Gantt<TItem> Gantt { get; set; }

    /// <summary>
    /// Gets callback invoked when modal requests save.
    /// </summary>
    [Parameter] public Func<TItem, Task<bool>> SaveRequested { get; set; }

    /// <summary>
    /// Gets callback invoked when modal requests delete.
    /// </summary>
    [Parameter] public Func<TItem, Task<bool>> DeleteRequested { get; set; }

    /// <summary>
    /// Gets or sets the first day of week used by date pickers.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }

    #endregion
}