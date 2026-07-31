#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Localization;
using Blazorise.Modules;
using Blazorise.Utilities;
using Blazorise.Vendors;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// An editor that displays a date value and allows a user to edit the value.
/// </summary>
/// <typeparam name="TValue">Data-type to be binded by the <see cref="DatePicker{TValue}"/> property.</typeparam>
public partial class DatePicker<TValue> : BaseTextInput<TValue, DatePickerClasses, DatePickerStyles>, IAsyncDisposable, IDatePicker
{
    #region Events

    internal event Action CalendarStateChanged;

    #endregion

    #region Members

    /// <summary>
    /// Captured Min parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<DateTimeOffset?> paramMin;

    /// <summary>
    /// Captured Max parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<DateTimeOffset?> paramMax;

    /// <summary>
    /// Captured FirstDayOfWeek parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<DayOfWeek> paramFirstDayOfWeek;

    /// <summary>
    /// Captured DisplayFormat parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<string> paramDisplayFormat;

    /// <summary>
    /// Captured InputFormat parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<string> paramInputFormat;

    /// <summary>
    /// Captured TimeAs24hr parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramTimeAs24hr;

    /// <summary>
    /// Captured Disabled parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramDisabled;

    /// <summary>
    /// Captured ReadOnly parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramReadOnly;

    /// <summary>
    /// Captured DisabledDates parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<IEnumerable> paramDisabledDates;

    /// <summary>
    /// Captured EnabledDates parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<IEnumerable> paramEnabledDates;

    /// <summary>
    /// Captured DisabledDays parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<IEnumerable<DayOfWeek>> paramDisabledDays;

    /// <summary>
    /// Captured SelectionMode parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<DateInputSelectionMode> paramSelectionMode;

    /// <summary>
    /// Captured InputMode parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<DateInputMode> paramInputMode;

    /// <summary>
    /// Captured RangeSeparator parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<string> paramRangeSeparator;

    /// <summary>
    /// Captured Inline parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramInline;

    /// <summary>
    /// Captured DisableMobile parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramDisableMobile;

    /// <summary>
    /// Captured Placeholder parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<string> paramPlaceholder;

    /// <summary>
    /// Captured StaticPicker parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramStaticPicker;

    /// <summary>
    /// Captured ShowWeekNumbers parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramShowWeekNumbers;

    /// <summary>
    /// Captured ShowTodayButton parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramShowTodayButton;

    /// <summary>
    /// Captured ShowClearButton parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramShowClearButton;

    /// <summary>
    /// Captured DefaultHour parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<int> paramDefaultHour;

    /// <summary>
    /// Captured DefaultMinute parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<int> paramDefaultMinute;

    /// <summary>
    /// The internal value used to separate dates.
    /// </summary>
    protected const string MULTIPLE_DELIMITER = ", ";

    /// <summary>
    /// The default format presented by date-time inputs.
    /// </summary>
    protected const string DEFAULT_DATETIME_DISPLAY_FORMAT = "yyyy-MM-dd HH:mm";

    private bool stateInitialized;

    private bool calendarOpen;

    private bool focusCalendarOnOpen;

    private string inputText;

    private DateTime visibleMonth;

    private DateTime focusedDate;

    private DateTime? pendingRangeStart;

    private DateTime? hoveredRangeEnd;

    private DateTime? hoveredWeekStart;

    private DatePickerInputMask inputMask;

    private bool inputFocused;

    private bool pointerInteraction;

    private bool? mobileDevice;

    private PickerObserverCoordinator observerCoordinator;

    private DatePickerCalendarContext<TValue> calendarContext;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void CaptureParameters( ParameterView parameters )
    {
        base.CaptureParameters( parameters );

        parameters.TryGetParameter( Min, out paramMin );
        parameters.TryGetParameter( Max, out paramMax );
        parameters.TryGetParameter( FirstDayOfWeek, out paramFirstDayOfWeek );
        parameters.TryGetParameter( DisplayFormat, out paramDisplayFormat );
        parameters.TryGetParameter( InputFormat, out paramInputFormat );
        parameters.TryGetParameter( TimeAs24hr, out paramTimeAs24hr );
        parameters.TryGetParameter( Disabled, out paramDisabled );
        parameters.TryGetParameter( ReadOnly, out paramReadOnly );
        parameters.TryGetParameter( DisabledDates, out paramDisabledDates );
        parameters.TryGetParameter( EnabledDates, out paramEnabledDates );
        parameters.TryGetParameter( DisabledDays, out paramDisabledDays );
        parameters.TryGetParameter( SelectionMode, out paramSelectionMode );
        parameters.TryGetParameter( InputMode, out paramInputMode );
        parameters.TryGetParameter( RangeSeparator, out paramRangeSeparator );
        parameters.TryGetParameter( Inline, out paramInline );
        parameters.TryGetParameter( DisableMobile, out paramDisableMobile );
        parameters.TryGetParameter( Placeholder, out paramPlaceholder );
        parameters.TryGetParameter( StaticPicker, out paramStaticPicker );
        parameters.TryGetParameter( ShowWeekNumbers, out paramShowWeekNumbers );
        parameters.TryGetParameter( ShowTodayButton, out paramShowTodayButton );
        parameters.TryGetParameter( ShowClearButton, out paramShowClearButton );
        parameters.TryGetParameter( DefaultHour, out paramDefaultHour );
        parameters.TryGetParameter( DefaultMinute, out paramDefaultMinute );
    }

    /// <inheritdoc/>
    protected override Task OnBeforeSetParametersAsync( ParameterView parameters )
    {
        return base.OnBeforeSetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override async Task OnAfterSetParametersAsync( ParameterView parameters )
    {
        await base.OnAfterSetParametersAsync( parameters );

        bool formatChanged = ( paramDisplayFormat.Defined && paramDisplayFormat.Changed )
            || ( paramInputFormat.Defined && paramInputFormat.Changed )
            || ( paramSelectionMode.Defined && paramSelectionMode.Changed )
            || ( paramInputMode.Defined && paramInputMode.Changed )
            || ( paramRangeSeparator.Defined && paramRangeSeparator.Changed );
        bool navigationDefaultsChanged = ( paramMin.Defined && paramMin.Changed )
            || ( paramMax.Defined && paramMax.Changed )
            || ( paramDefaultHour.Defined && paramDefaultHour.Changed )
            || ( paramDefaultMinute.Defined && paramDefaultMinute.Changed );
        bool resetEmptyNavigation = navigationDefaultsChanged && GetSelectedDates().Count == 0;

        if ( !stateInitialized || paramValue.Changed || formatChanged || resetEmptyNavigation )
        {
            SynchronizeStateFromValue( resetVisibleMonth: !stateInitialized || paramValue.Changed || resetEmptyNavigation );
        }

        if ( paramInline.Defined && paramInline.Changed )
        {
            calendarOpen = Inline;
            focusCalendarOnOpen = false;
            await SynchronizeOutsidePointerSubscriptionAsync();
        }

        bool inputMaskConfigurationChanged = ( paramInputFormat.Defined && paramInputFormat.Changed )
            || ( paramInputMode.Defined && paramInputMode.Changed );

        if ( Rendered && inputMaskConfigurationChanged )
        {
            if ( inputFocused && !UseNativeMobilePicker && !string.IsNullOrWhiteSpace( InputFormat ) )
            {
                inputText = FormatValueWithFormat( Value, PickerDateTimeFormat.Normalize( InputFormat ) );
                ExecuteAfterRender( RefreshInputMaskAsync );
            }
            else
            {
                ExecuteAfterRender( DestroyInputMaskAsync );
            }
        }

        if ( Rendered && paramDisableMobile.Defined && paramDisableMobile.Changed && !DisableMobile )
        {
            ExecuteAfterRender( DetectMobileDeviceAsync );
        }

        if ( Rendered && UseNativeMobilePicker )
        {
            calendarOpen = false;
            focusCalendarOnOpen = false;
            await DisposeOutsidePointerSubscriptionAsync();

            if ( InputMaskInitialized )
            {
                ExecuteAfterRender( DestroyInputMaskAsync );
            }
        }

        NotifyCalendarStateChanged();
        stateInitialized = true;
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        await base.OnFirstAfterRenderAsync();
        await DetectMobileDeviceAsync();
        await InitializeInputKeyDownSubscriptionAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            if ( inputMask is not null )
            {
                await inputMask.DestroyAsync( ElementRef, ElementId );
            }

            if ( observerCoordinator is not null )
            {
                await observerCoordinator.DisposeAsync();
            }

            LocalizerService.LocalizationChanged -= OnLocalizationChanged;
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DatePicker( Plaintext ) );
        builder.Append( ClassProvider.DatePickerSize( ThemeSize ) );
        builder.Append( ClassProvider.DatePickerColor( Color ) );
        builder.Append( ClassProvider.DatePickerValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override Task OnChangeHandler( ChangeEventArgs eventArgs )
    {
        return ProcessInputTextAsync( eventArgs?.Value?.ToString(), formatParsedValue: true );
    }

    private async Task ProcessInputTextAsync( string value, bool formatParsedValue )
    {
        inputText = value;

        if ( string.IsNullOrWhiteSpace( inputText ) )
        {
            await CurrentValueHandler( null );
            inputText = null;
            await FinishMaskedEditingAsync();
            pendingRangeStart = null;
            hoveredRangeEnd = null;
            hoveredWeekStart = null;
            NotifyCalendarStateChanged();
            return;
        }

        if ( TryNormalizeInputValue( inputText, out string normalizedValue ) )
        {
            await CurrentValueHandler( normalizedValue );
            SynchronizeStateFromValue( resetVisibleMonth: true, updateInputText: formatParsedValue );
            await FinishMaskedEditingAsync();
        }
        else if ( ParentValidation is not null )
        {
            await ParentValidation.NotifyInputChanged<TValue>( default );
        }

        NotifyCalendarStateChanged();
    }

    /// <summary>
    /// Opens the calendar when the visible input is clicked.
    /// </summary>
    [JSInvokable]
    protected async Task OnClickHandler( MouseEventArgs eventArgs )
    {
        if ( !OpenTrigger.HasFlag( PickerOpenTrigger.Click ) )
            return;

        if ( IsDisabled || ReadOnly || Plaintext )
            return;

        if ( UseNativeMobilePicker )
            return;

        await BeginMaskedEditingAsync();
        await OpenAsync();
    }

    /// <summary>
    /// Records that the input is receiving focus through a pointer interaction.
    /// </summary>
    /// <param name="eventArgs">Information about the pointer event.</param>
    /// <returns>A completed task.</returns>
    protected Task OnPointerDownHandler( PointerEventArgs eventArgs )
    {
        pointerInteraction = true;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Clears the active pointer interaction when it finishes without focusing the input.
    /// </summary>
    /// <param name="eventArgs">Information about the pointer event.</param>
    /// <returns>A completed task.</returns>
    protected Task OnPointerInteractionEndedHandler( PointerEventArgs eventArgs )
    {
        pointerInteraction = false;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override string FormatValueAsString( TValue value )
    {
        return FormatValueWithFormat( value, EffectiveDisplayFormat );
    }

    private string FormatValueWithFormat( TValue value, string format )
    {
        if ( value is null )
            return null;

        if ( SelectionMode != DateInputSelectionMode.Single )
        {
            List<string> results = new();

            if ( value is IEnumerable values )
            {
                foreach ( object item in values )
                {
                    results.Add( FormatDateValue( item, format ) );
                }
            }

            if ( InputMode == DateInputMode.Week )
            {
                if ( SelectionMode == DateInputSelectionMode.Multiple )
                {
                    results = results.Distinct().ToList();
                }
                else if ( SelectionMode == DateInputSelectionMode.Range
                          && results.Count == 2
                          && results[0] == results[1] )
                {
                    results.RemoveAt( 1 );
                }
            }

            string delimiter = SelectionMode == DateInputSelectionMode.Multiple ? MULTIPLE_DELIMITER : CurrentRangeSeparator;

            return string.Join( delimiter, results );
        }

        return FormatDateValue( value, format );
    }

    private string FormatDateValue( object value, string format )
        => InputMode == DateInputMode.Week
            ? WeekDateFormat.FormatValue( value, format, CultureInfo.CurrentCulture )
            : Formaters.FormatDateValueAsString( value, format );

    /// <inheritdoc/>
    protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
    {
        if ( !TryNormalizeInputValue( value, out string normalizedValue ) )
        {
            return Task.FromResult( new ParseValue<TValue>( false, default, null ) );
        }

        if ( SelectionMode != DateInputSelectionMode.Single )
        {
            string delimiter = SelectionMode == DateInputSelectionMode.Multiple ? MULTIPLE_DELIMITER : CurrentRangeSeparator;

            try
            {
                TValue readOnlyList = Parsers.ParseCsvDatesToReadOnlyList<TValue>( normalizedValue, delimiter, InputMode );

                return Task.FromResult( new ParseValue<TValue>( true, readOnlyList, null ) );
            }
            catch ( ArgumentException )
            {
                return Task.FromResult( new ParseValue<TValue>( false, default, null ) );
            }
        }

        if ( Parsers.TryParseDate( normalizedValue, InputMode, out TValue result ) )
        {
            return Task.FromResult( new ParseValue<TValue>( true, result, null ) );
        }

        return Task.FromResult( new ParseValue<TValue>( false, default, null ) );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public new virtual async Task OnKeyDownHandler( KeyboardEventArgs eventArgs )
    {
        await KeyDown.InvokeAsync( eventArgs );

        if ( IsDisabled || ReadOnly || eventArgs is null )
            return;

        if ( UseNativeMobilePicker )
            return;

        if ( CalendarVisible )
        {
            if ( focusCalendarOnOpen )
            {
                await OnCalendarKeyDownAsync( eventArgs );
            }
            else if ( eventArgs.Key is "ArrowDown" or "F4" )
            {
                await OpenCalendarAsync( focusCalendar: true );
            }
            else if ( eventArgs.Key == "Escape" )
            {
                await CloseCalendarAsync( focusInput: false );
            }
            else if ( eventArgs.Key == "Tab" )
            {
                await CloseCalendarAsync( focusInput: false );
            }
        }
        else if ( OpenTrigger.HasFlag( PickerOpenTrigger.OpenKeys )
                  && eventArgs.Key is "ArrowDown" or "F4" )
        {
            await OpenCalendarAsync( focusCalendar: true );
        }
    }

    /// <inheritdoc/>
    [JSInvokable]
    public new virtual Task OnKeyUpHandler( KeyboardEventArgs eventArgs )
    {
        return KeyUp.InvokeAsync( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public new virtual async Task OnFocusHandler( FocusEventArgs eventArgs )
    {
        bool pointerInitiatedFocus = pointerInteraction;
        pointerInteraction = false;
        inputFocused = true;

        await OnFocus.InvokeAsync( eventArgs );
        await BeginMaskedEditingAsync();

        if ( !pointerInitiatedFocus
             && OpenTrigger.HasFlag( PickerOpenTrigger.Focus )
             && !UseNativeMobilePicker )
        {
            await OpenCalendarAsync( focusCalendar: false );
        }
    }

    /// <inheritdoc/>
    [JSInvokable]
    public new virtual async Task OnFocusInHandler( FocusEventArgs eventArgs )
    {
        await FocusIn.InvokeAsync( eventArgs );

        if ( ShouldShowOnScreenKeyboardOnFocus )
        {
            await ShowOnScreenKeyboard( false );
        }
    }

    /// <inheritdoc/>
    [JSInvokable]
    public new virtual Task OnFocusOutHandler( FocusEventArgs eventArgs )
    {
        return FocusOut.InvokeAsync( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public new virtual Task OnKeyPressHandler( KeyboardEventArgs eventArgs )
    {
        return KeyPress.InvokeAsync( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public new virtual async Task OnBlurHandler( FocusEventArgs eventArgs )
    {
        inputFocused = false;

        if ( InputMaskInitialized )
        {
            inputText = FormatValueAsString( Value );

            await DestroyInputMaskAsync();
            await JSUtilitiesModule.SetTextValue( ElementRef, inputText );
            await InvokeAsync( StateHasChanged );
        }

        await base.OnBlurHandler( eventArgs );
    }

    /// <inheritdoc/>
    protected override async Task OnScreenKeyboardValueChanged( string value )
    {
        await ProcessInputTextAsync( value, formatParsedValue: false );
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Opens the calendar dropdown.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask OpenAsync()
    {
        if ( IsDisabled || ReadOnly || Plaintext )
            return ValueTask.CompletedTask;

        if ( UseNativeMobilePicker )
            return JSUtilitiesModule.ShowPicker( ElementRef, ElementId );

        return OpenCalendarAsync( focusCalendar: false );
    }

    private async ValueTask OpenCalendarAsync( bool focusCalendar )
    {
        if ( IsDisabled || ReadOnly || Plaintext )
            return;

        if ( UseNativeMobilePicker )
        {
            await JSUtilitiesModule.ShowPicker( ElementRef, ElementId );
            return;
        }

        bool renderRequired = !CalendarVisible || focusCalendarOnOpen != focusCalendar;

        InitializeNavigationTarget();
        focusCalendarOnOpen = focusCalendar;
        calendarOpen = true;

        if ( renderRequired )
        {
            NotifyCalendarStateChanged();
            await InvokeAsync( StateHasChanged );
        }

        await SynchronizeOutsidePointerSubscriptionAsync();
    }

    /// <summary>
    /// Closes the calendar dropdown.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask CloseAsync()
    {
        return CloseCalendarAsync( focusInput: false );
    }

    /// <summary>
    /// Shows/opens the calendar if its closed, hides/closes it otherwise.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask ToggleAsync()
    {
        if ( CalendarVisible && !Inline )
        {
            await CloseCalendarAsync( focusInput: false );
        }
        else
        {
            await OpenAsync();
        }
    }

    /// <inheritdoc/>
    public override Task Focus( bool scrollToElement = true )
    {
        return base.Focus( scrollToElement );
    }

    /// <inheritdoc/>
    public override Task Select( bool focus = true )
    {
        return base.Select( focus );
    }

    private async ValueTask CloseCalendarAsync( bool focusInput )
    {
        if ( Inline )
            return;

        calendarOpen = false;
        focusCalendarOnOpen = false;
        pendingRangeStart = null;
        hoveredRangeEnd = null;
        hoveredWeekStart = null;

        NotifyCalendarStateChanged();
        await DisposeOutsidePointerSubscriptionAsync();

        if ( focusInput )
        {
            ExecuteAfterRender( () => Focus() );
        }

        await InvokeAsync( StateHasChanged );
    }

    private void SynchronizeStateFromValue( bool resetVisibleMonth, bool updateInputText = true )
    {
        if ( updateInputText )
        {
            inputText = FormatValueAsString( Value );
        }

        IReadOnlyList<DateTime> selectedDates = GetSelectedDates();
        DateTime navigationDate = selectedDates.FirstOrDefault();

        if ( navigationDate == default )
        {
            navigationDate = GetInitialDate();
        }

        focusedDate = navigationDate;

        if ( resetVisibleMonth || visibleMonth == default )
        {
            visibleMonth = new DateTime( navigationDate.Year, navigationDate.Month, 1 );
        }
    }

    private void InitializeNavigationTarget()
    {
        if ( focusedDate == default )
        {
            focusedDate = GetSelectedDates().FirstOrDefault();
        }

        if ( focusedDate == default )
        {
            focusedDate = GetInitialDate();
        }

        if ( visibleMonth == default )
        {
            visibleMonth = new DateTime( focusedDate.Year, focusedDate.Month, 1 );
        }
    }

    private DateTime GetInitialDate()
    {
        DateTime initialDate = DatePickerDateUtilities.GetInitialDate( DefaultHour, DefaultMinute, Min, Max );

        return InputMode == DateInputMode.Week
            ? WeekDateFormat.GetWeekStart( initialDate )
            : initialDate;
    }

    private IReadOnlyList<DateTime> GetSelectedDates()
    {
        IReadOnlyList<DateTime> selectedDates = DatePickerDateUtilities.GetSelectedDates( Value, SelectionMode );

        if ( InputMode != DateInputMode.Week )
            return selectedDates;

        IEnumerable<DateTime> normalizedDates = selectedDates.Select( WeekDateFormat.GetWeekStart );

        if ( SelectionMode == DateInputSelectionMode.Multiple )
        {
            normalizedDates = normalizedDates.Distinct();
        }

        return normalizedDates.ToArray();
    }

    private bool TryNormalizeInputValue( string value, out string normalizedValue )
        => DatePickerInputParser.TryNormalize(
            value,
            SelectionMode,
            SelectionMode == DateInputSelectionMode.Multiple ? MULTIPLE_DELIMITER : CurrentRangeSeparator,
            InputFormat,
            DisplayFormat,
            DateFormat,
            InputMode,
            out normalizedValue );

    private Task RefreshInputMaskAsync()
        => InputMask.RefreshAsync( ElementRef, ElementId, InputFormat, InputMode );

    private Task DestroyInputMaskAsync()
        => inputMask?.DestroyAsync( ElementRef, ElementId ) ?? Task.CompletedTask;

    private async Task BeginMaskedEditingAsync()
    {
        if ( InputMaskInitialized
             || !inputFocused
             || UseNativeMobilePicker
             || string.IsNullOrWhiteSpace( InputFormat )
             || IsDisabled
             || ReadOnly
             || Plaintext )
        {
            return;
        }

        inputText = FormatValueWithFormat( Value, PickerDateTimeFormat.Normalize( InputFormat ) );

        ExecuteAfterRender( RefreshInputMaskAsync );
        await InvokeAsync( StateHasChanged );
    }

    private async Task DetectMobileDeviceAsync()
    {
        if ( DisableMobile || mobileDevice.HasValue )
            return;

        string userAgent = await JSUtilitiesModule.GetUserAgent();
        bool detectedMobileDevice = MobileDeviceDetector.IsMobile( userAgent );

        if ( mobileDevice != detectedMobileDevice )
        {
            mobileDevice = detectedMobileDevice;

            if ( UseNativeMobilePicker )
            {
                calendarOpen = false;
                focusCalendarOnOpen = false;
                await DisposeOutsidePointerSubscriptionAsync();
                await DestroyInputMaskAsync();
            }

            NotifyCalendarStateChanged();
            await InvokeAsync( StateHasChanged );
        }
    }

    private async Task FinishMaskedEditingAsync()
    {
        if ( !InputMaskInitialized )
            return;

        await DestroyInputMaskAsync();
        await JSUtilitiesModule.SetTextValue( ElementRef, inputText );
    }

    private async Task CommitDatesAsync( IReadOnlyList<DateTime> dates )
    {
        if ( dates.Count == 0 )
        {
            await CurrentValueHandler( null );
            inputText = null;
            await FinishMaskedEditingAsync();
            NotifyCalendarStateChanged();
            await InvokeAsync( StateHasChanged );
            return;
        }

        if ( InputMode == DateInputMode.Week )
        {
            dates = dates
                .Select( WeekDateFormat.GetWeekStart )
                .ToArray();
        }

        if ( SelectionMode == DateInputSelectionMode.Range
             && InputMode != DateInputMode.DateTime
             && dates.Count == 2
             && dates[0].Date == dates[1].Date )
        {
            dates = new[] { dates[0] };
        }

        string delimiter = SelectionMode == DateInputSelectionMode.Multiple ? MULTIPLE_DELIMITER : CurrentRangeSeparator;
        string normalizedValue = string.Join(
            delimiter,
            dates.Select( date => date.ToString( DateFormat, CultureInfo.InvariantCulture ) ) );

        await CurrentValueHandler( normalizedValue );
        inputText = FormatValueAsString( Value );
        await FinishMaskedEditingAsync();
        NotifyCalendarStateChanged();
        await InvokeAsync( StateHasChanged );
    }

    internal async Task SelectDateAsync( DateTime selectedDate, DatePickerSelectionSource selectionSource = DatePickerSelectionSource.Calendar )
    {
        if ( CalendarInteractionDisabled
             || ( InputMode == DateInputMode.Week ? IsWeekDisabled( selectedDate ) : IsDateDisabled( selectedDate ) ) )
            return;

        DateTime date = ApplyCurrentTime( InputMode == DateInputMode.Week
            ? WeekDateFormat.GetWeekStart( selectedDate )
            : selectedDate );
        focusedDate = date;
        visibleMonth = new DateTime( selectedDate.Year, selectedDate.Month, 1 );

        if ( SelectionMode == DateInputSelectionMode.Single )
        {
            await CommitDatesAsync( new[] { date } );
            await CloseCalendarAsync( focusInput: true );
        }
        else if ( SelectionMode == DateInputSelectionMode.Range )
        {
            if ( selectionSource == DatePickerSelectionSource.TodayButton )
            {
                pendingRangeStart = null;
                hoveredRangeEnd = null;
                hoveredWeekStart = null;

                await CommitDatesAsync( new[] { date, date } );
                await CloseCalendarAsync( focusInput: true );
            }
            else if ( !pendingRangeStart.HasValue )
            {
                pendingRangeStart = date;
                hoveredRangeEnd = null;
                hoveredWeekStart = null;
                NotifyCalendarStateChanged();
            }
            else
            {
                DateTime start = pendingRangeStart.Value;
                DateTime end = date;

                if ( end < start )
                {
                    (start, end) = (end, start);
                }

                pendingRangeStart = null;
                hoveredRangeEnd = null;
                hoveredWeekStart = null;

                await CommitDatesAsync( new[] { start, end } );
                await CloseCalendarAsync( focusInput: true );
            }
        }
        else
        {
            List<DateTime> selectedDates = GetSelectedDates().ToList();
            int existingIndex = selectedDates.FindIndex( item => item.Date == date.Date );

            if ( existingIndex >= 0 )
            {
                selectedDates.RemoveAt( existingIndex );
            }
            else
            {
                selectedDates.Add( date );
            }

            await CommitDatesAsync( selectedDates );
        }
    }

    internal Task SelectMonthAsync( DateTime month )
    {
        if ( CalendarInteractionDisabled || IsMonthDisabled( month ) )
            return Task.CompletedTask;

        DateTime selectedMonth = new(
            month.Year,
            month.Month,
            1,
            CurrentHour,
            CurrentMinute,
            0,
            DateTimeKind.Unspecified );

        return SelectDateAsync( selectedMonth );
    }

    internal void PreviewRange( DateTime? date )
    {
        bool stateChanged = false;

        if ( InputMode == DateInputMode.Week && date.HasValue )
        {
            date = WeekDateFormat.GetWeekStart( date.Value );
        }

        if ( InputMode == DateInputMode.Week && hoveredWeekStart != date )
        {
            hoveredWeekStart = date;
            stateChanged = true;
        }

        if ( SelectionMode == DateInputSelectionMode.Range
             && pendingRangeStart.HasValue
             && hoveredRangeEnd != date )
        {
            hoveredRangeEnd = date;
            stateChanged = true;
        }

        if ( stateChanged )
        {
            NotifyCalendarStateChanged();
        }
    }

    internal async Task SelectTodayAsync()
    {
        if ( CalendarInteractionDisabled )
            return;

        DateTime today = DateTime.Today
            .AddHours( CurrentHour )
            .AddMinutes( CurrentMinute );

        if ( InputMode == DateInputMode.Month )
        {
            today = new DateTime( today.Year, today.Month, 1, today.Hour, today.Minute, 0 );
        }

        await SelectDateAsync( today, DatePickerSelectionSource.TodayButton );
    }

    internal async Task ClearAsync()
    {
        if ( CalendarInteractionDisabled )
            return;

        pendingRangeStart = null;
        hoveredRangeEnd = null;
        hoveredWeekStart = null;
        await CommitDatesAsync( Array.Empty<DateTime>() );

        if ( !Inline )
        {
            await CloseCalendarAsync( focusInput: true );
        }
    }

    internal void ShowPreviousPeriod()
    {
        if ( CalendarInteractionDisabled )
            return;

        if ( InputMode == DateInputMode.Month )
        {
            MoveFocusedMonth( -12 );
        }
        else
        {
            MoveFocusedMonth( -1 );
        }
    }

    internal void ShowNextPeriod()
    {
        if ( CalendarInteractionDisabled )
            return;

        if ( InputMode == DateInputMode.Month )
        {
            MoveFocusedMonth( 12 );
        }
        else
        {
            MoveFocusedMonth( 1 );
        }
    }

    internal void ShowPreviousYear()
    {
        if ( CalendarInteractionDisabled )
            return;

        MoveFocusedMonth( -12 );
    }

    internal void ShowNextYear()
    {
        if ( CalendarInteractionDisabled )
            return;

        MoveFocusedMonth( 12 );
    }

    internal void ChangeVisibleMonth( ChangeEventArgs eventArgs )
    {
        if ( CalendarInteractionDisabled )
            return;

        if ( int.TryParse( eventArgs?.Value?.ToString(), out int month ) && month is >= 1 and <= 12 )
        {
            visibleMonth = new DateTime( visibleMonth.Year, month, 1 );
            focusedDate = DatePickerDateUtilities.MoveIntoMonth( focusedDate, visibleMonth );

            if ( InputMode == DateInputMode.Week )
            {
                focusedDate = WeekDateFormat.GetWeekStart( focusedDate );
            }

            NotifyCalendarStateChanged();
        }
    }

    internal void ChangeVisibleYear( ChangeEventArgs eventArgs )
    {
        if ( CalendarInteractionDisabled )
            return;

        if ( int.TryParse( eventArgs?.Value?.ToString(), out int year ) && year is >= 1 and <= 9999 )
        {
            visibleMonth = new DateTime( year, visibleMonth.Month, 1 );
            focusedDate = DatePickerDateUtilities.MoveIntoMonth( focusedDate, visibleMonth );

            if ( InputMode == DateInputMode.Week )
            {
                focusedDate = WeekDateFormat.GetWeekStart( focusedDate );
            }

            NotifyCalendarStateChanged();
        }
    }

    internal async Task OnCalendarKeyDownAsync( KeyboardEventArgs eventArgs )
    {
        if ( eventArgs is null || CalendarInteractionDisabled )
            return;

        bool monthMode = InputMode == DateInputMode.Month;
        bool weekMode = InputMode == DateInputMode.Week;

        switch ( eventArgs.Key )
        {
            case "ArrowLeft":
                MoveFocus( weekMode ? -7 : -1, monthMode );
                break;
            case "ArrowRight":
                MoveFocus( weekMode ? 7 : 1, monthMode );
                break;
            case "ArrowUp":
                MoveFocus( monthMode ? -4 : -7, monthMode );
                break;
            case "ArrowDown":
                MoveFocus( monthMode ? 4 : 7, monthMode );
                break;
            case "PageUp":
                MoveFocusedMonth( eventArgs.ShiftKey ? -12 : monthMode ? -12 : -1 );
                break;
            case "PageDown":
                MoveFocusedMonth( eventArgs.ShiftKey ? 12 : monthMode ? 12 : 1 );
                break;
            case "Home":
                if ( monthMode )
                {
                    focusedDate = new DateTime( visibleMonth.Year, 1, 1 );
                    NotifyCalendarStateChanged();
                }
                else if ( weekMode )
                {
                    focusedDate = WeekDateFormat.GetWeekStart( visibleMonth );
                    NotifyCalendarStateChanged();
                }
                else
                {
                    MoveFocusToWeekBoundary( beginning: true );
                }
                break;
            case "End":
                if ( monthMode )
                {
                    focusedDate = new DateTime( visibleMonth.Year, 12, 1 );
                    NotifyCalendarStateChanged();
                }
                else if ( weekMode )
                {
                    DateTime monthEnd = new(
                        visibleMonth.Year,
                        visibleMonth.Month,
                        DateTime.DaysInMonth( visibleMonth.Year, visibleMonth.Month ) );
                    focusedDate = WeekDateFormat.GetWeekStart( monthEnd );
                    NotifyCalendarStateChanged();
                }
                else
                {
                    MoveFocusToWeekBoundary( beginning: false );
                }
                break;
            case "Enter":
            case " ":
                if ( monthMode )
                {
                    await SelectMonthAsync( focusedDate );
                }
                else
                {
                    await SelectDateAsync( focusedDate );
                }
                break;
            case "Escape":
                await CloseCalendarAsync( focusInput: true );
                break;
        }
    }

    internal Task OnCalendarControlKeyDownAsync( KeyboardEventArgs eventArgs )
    {
        return eventArgs?.Key is "Escape"
            ? CloseCalendarAsync( focusInput: true ).AsTask()
            : Task.CompletedTask;
    }

    private void MoveFocus( int amount, bool byMonth )
    {
        if ( !DatePickerDateUtilities.TryMoveDate( focusedDate, amount, byMonth, out DateTime candidate ) )
            return;

        int attempts = 0;

        while ( ( byMonth
            ? IsMonthDisabled( candidate )
            : InputMode == DateInputMode.Week
                ? IsWeekDisabled( candidate )
                : IsDateDisabled( candidate ) ) && attempts++ < 3660 )
        {
            if ( !DatePickerDateUtilities.TryMoveDate( candidate, Math.Sign( amount ), byMonth, out candidate ) )
                return;
        }

        focusedDate = candidate;
        visibleMonth = new DateTime( candidate.Year, candidate.Month, 1 );
        NotifyCalendarStateChanged();
    }

    private void MoveFocusedMonth( int months )
    {
        if ( !DatePickerDateUtilities.TryMoveDate( visibleMonth, months, byMonth: true, out DateTime targetMonth ) )
            return;

        visibleMonth = new DateTime( targetMonth.Year, targetMonth.Month, 1 );
        focusedDate = DatePickerDateUtilities.MoveIntoMonth( focusedDate, visibleMonth );

        if ( InputMode == DateInputMode.Week )
        {
            focusedDate = WeekDateFormat.GetWeekStart( focusedDate );
        }

        NotifyCalendarStateChanged();
    }

    private void MoveFocusToWeekBoundary( bool beginning )
    {
        int offset = ( 7 + (int)focusedDate.DayOfWeek - (int)CalendarFirstDayOfWeek ) % 7;
        focusedDate = beginning
            ? focusedDate.AddDays( -offset )
            : focusedDate.AddDays( 6 - offset );
        visibleMonth = new DateTime( focusedDate.Year, focusedDate.Month, 1 );
        NotifyCalendarStateChanged();
    }

    internal async Task ChangeHourAsync( ChangeEventArgs eventArgs )
    {
        if ( CalendarInteractionDisabled )
            return;

        if ( !int.TryParse( eventArgs?.Value?.ToString(), out int hour ) )
            return;

        if ( TimeAs24hr )
        {
            hour = Math.Clamp( hour, 0, 23 );
        }
        else
        {
            hour = Math.Clamp( hour, 1, 12 ) % 12;

            if ( IsPostMeridiem )
            {
                hour += 12;
            }
        }

        await CommitTimeAsync( hour, CurrentMinute );
    }

    internal async Task ChangeMinuteAsync( ChangeEventArgs eventArgs )
    {
        if ( CalendarInteractionDisabled )
            return;

        if ( int.TryParse( eventArgs?.Value?.ToString(), out int minute ) )
        {
            await CommitTimeAsync( CurrentHour, Math.Clamp( minute, 0, 59 ) );
        }
    }

    internal Task ToggleMeridiemAsync()
    {
        if ( CalendarInteractionDisabled )
            return Task.CompletedTask;

        int hour = CurrentHour >= 12 ? CurrentHour - 12 : CurrentHour + 12;
        return CommitTimeAsync( hour, CurrentMinute );
    }

    private async Task CommitTimeAsync( int hour, int minute )
    {
        IReadOnlyList<DateTime> selectedDates = GetSelectedDates();

        if ( selectedDates.Count == 0 )
        {
            focusedDate = new DateTime(
                focusedDate.Year,
                focusedDate.Month,
                focusedDate.Day,
                hour,
                minute,
                0,
                DateTimeKind.Unspecified );

            if ( pendingRangeStart.HasValue )
            {
                pendingRangeStart = new DateTime(
                    pendingRangeStart.Value.Year,
                    pendingRangeStart.Value.Month,
                    pendingRangeStart.Value.Day,
                    hour,
                    minute,
                    0,
                    DateTimeKind.Unspecified );
            }

            NotifyCalendarStateChanged();
            return;
        }

        List<DateTime> updatedDates = selectedDates
            .Select( date => new DateTime( date.Year, date.Month, date.Day, hour, minute, 0, date.Kind ) )
            .ToList();

        focusedDate = updatedDates[0];
        await CommitDatesAsync( updatedDates );
    }

    private DateTime ApplyCurrentTime( DateTime date )
    {
        if ( InputMode != DateInputMode.DateTime )
            return date.Date;

        return new DateTime( date.Year, date.Month, date.Day, CurrentHour, CurrentMinute, 0, date.Kind );
    }

    private DateTime GetCurrentTimeSource()
    {
        DateTime selectedDate = GetSelectedDates().FirstOrDefault();

        if ( selectedDate != default )
            return selectedDate;

        if ( pendingRangeStart.HasValue )
            return pendingRangeStart.Value;

        if ( focusedDate != default )
            return focusedDate;

        return DateTime.Today
            .AddHours( Math.Clamp( DefaultHour, 0, 23 ) )
            .AddMinutes( Math.Clamp( DefaultMinute, 0, 59 ) );
    }

    private int CurrentHour => GetCurrentTimeSource().Hour;

    private int CurrentMinute => GetCurrentTimeSource().Minute;

    private bool IsPostMeridiem => CurrentHour >= 12;

    private bool IsDateDisabled( DateTime date )
    {
        if ( CalendarInteractionDisabled )
            return true;

        if ( InputMode == DateInputMode.Month )
            return IsMonthDisabled( date );

        DateTime day = date.Date;

        if ( Min.HasValue && day < Min.Value.Date )
            return true;

        if ( Max.HasValue && day > Max.Value.Date )
            return true;

        if ( DatePickerDateUtilities.ContainsDate( DisabledDates, day ) )
            return true;

        if ( DisabledDays?.Contains( day.DayOfWeek ) == true )
            return true;

        if ( EnabledDates is not null && !DatePickerDateUtilities.ContainsDate( EnabledDates, day ) )
            return true;

        return false;
    }

    private bool IsWeekDisabled( DateTime date )
    {
        DateTime weekStart = WeekDateFormat.GetWeekStart( date );

        for ( int dayOffset = 0; dayOffset < 7; dayOffset++ )
        {
            if ( !IsDateDisabled( weekStart.AddDays( dayOffset ) ) )
                return false;
        }

        return true;
    }

    private bool IsMonthDisabled( DateTime month )
    {
        if ( CalendarInteractionDisabled )
            return true;

        DateTime monthStart = new( month.Year, month.Month, 1 );
        DateTime monthEnd = new( month.Year, month.Month, DateTime.DaysInMonth( month.Year, month.Month ) );

        if ( Min.HasValue && monthEnd < Min.Value.Date )
            return true;

        if ( Max.HasValue && monthStart > Max.Value.Date )
            return true;

        if ( DatePickerDateUtilities.EnumerateDates( DisabledDates ).Any( date => date.Year == month.Year && date.Month == month.Month ) )
            return true;

        if ( EnabledDates is not null )
        {
            return !DatePickerDateUtilities.EnumerateDates( EnabledDates ).Any( date => date.Year == month.Year && date.Month == month.Month );
        }

        return false;
    }

    internal IReadOnlyList<DatePickerCalendarWeek> BuildCalendarWeeks()
        => DatePickerCalendarBuilder.BuildWeeks(
            visibleMonth,
            focusedDate,
            CalendarFirstDayOfWeek,
            InputMode,
            SelectionMode,
            GetSelectedDates(),
            pendingRangeStart,
            hoveredRangeEnd,
            hoveredWeekStart,
            IsDateDisabled );

    internal IReadOnlyList<DatePickerCalendarMonth> BuildCalendarMonths()
        => DatePickerCalendarBuilder.BuildMonths(
            visibleMonth,
            focusedDate,
            GetSelectedDates(),
            CalendarContext.MonthNames,
            IsMonthDisabled );

    private void OnLocalizationChanged( object sender, EventArgs eventArgs )
    {
        inputText = FormatValueAsString( Value );
        NotifyCalendarStateChanged();
        _ = InvokeAsync( StateHasChanged );
    }

    private void NotifyCalendarStateChanged()
        => CalendarStateChanged?.Invoke();

    private Task HandleOutsidePointerAsync( DocumentEventArgs eventArgs )
    {
        return CloseCalendarAsync( focusInput: false ).AsTask();
    }

    private ValueTask SynchronizeOutsidePointerSubscriptionAsync()
        => ObserverCoordinator.SynchronizeOutsideSubscriptionAsync(
            calendarOpen,
            Inline,
            ElementId,
            PickerContainerId,
            CalendarId,
            HandleOutsidePointerAsync );

    private ValueTask DisposeOutsidePointerSubscriptionAsync()
        => ObserverCoordinator.DisposeOutsideSubscriptionAsync();

    private ValueTask InitializeInputKeyDownSubscriptionAsync()
        => ObserverCoordinator.InitializeInputKeyDownAsync( ElementId, ElementId );

    /// <inheritdoc/>
    protected override bool IsSameAsInternalValue( TValue value )
    {
        if ( value is IEnumerable<TValue> values1 && Value is IEnumerable<TValue> values2 )
        {
            return values1.AreEqual( values2 );
        }

        if ( value is IEnumerable objects1 && Value is IEnumerable objects2 )
        {
            return objects1.AreEqual( objects2 );
        }

        return value.IsEqual( Value );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    internal bool CalendarVisible => !UseNativeMobilePicker && ( Inline || calendarOpen );

    internal bool FocusCalendarOnOpen => focusCalendarOnOpen;

    internal bool CalendarInteractionDisabled => IsDisabled || ReadOnly || Plaintext;

    internal string CalendarId => $"{ElementId}-calendar";

    internal string PickerContainerId => $"{ElementId}-container";

    internal DatePickerCalendarContext<TValue> CalendarContext
        => calendarContext ??= new( this );

    internal DateTime CalendarVisibleMonth => visibleMonth;

    internal DateTime CalendarFocusedDate => focusedDate;

    internal DateTime CalendarTimeSource => GetCurrentTimeSource();

    internal DayOfWeek CalendarFirstDayOfWeek => FirstDayOfWeek;

    internal bool CalendarShowsWeekNumbers => ShowWeekNumbers || InputMode == DateInputMode.Week;

    internal IClassProvider PickerClassProvider => ClassProvider;

    internal ITextLocalizer PickerLocalizer => Localizer;

    private PickerObserverCoordinator ObserverCoordinator
        => observerCoordinator ??= new( DocumentObserver );

    private DatePickerInputMask InputMask
        => inputMask ??= new( InputFormatConverter, InputMaskJSModule );

    private bool InputMaskInitialized
        => inputMask?.IsInitialized == true;

    /// <inheritdoc/>
    protected override OnScreenKeyboardInputType OnScreenKeyboardInputType => InputMode == DateInputMode.DateTime
        ? OnScreenKeyboardInputType.Date | OnScreenKeyboardInputType.Time | OnScreenKeyboardInputType.Pickers
        : OnScreenKeyboardInputType.Date | OnScreenKeyboardInputType.Pickers;

    /// <summary>
    /// Gets the range separator based on the current locale settings.
    /// </summary>
    protected string CurrentRangeSeparator => RangeSeparator ?? Localizer.GetString( "RangeSeparator" ) ?? " to ";

    /// <summary>
    /// Gets the string representation of the input mode.
    /// </summary>
    protected string Mode => InputMode.ToDateInputMode();

    /// <summary>
    /// Gets the date format based on the current <see cref="InputMode"/> settings.
    /// </summary>
    protected string DateFormat => Parsers.GetInternalDateFormat( InputMode );

    /// <summary>
    /// Gets the format presented in the visible input.
    /// </summary>
    protected string EffectiveDisplayFormat => PickerDateTimeFormat.Normalize(
        DisplayFormat ?? ( InputMode switch
        {
            DateInputMode.DateTime => DEFAULT_DATETIME_DISPLAY_FORMAT,
            DateInputMode.Week => WeekDateFormat.DefaultDisplayFormat,
            _ => DateFormat,
        } ) );

    /// <summary>
    /// Gets the text presented in the visible input.
    /// </summary>
    protected string InputText => inputText;

    /// <summary>
    /// Gets the input type rendered for the active picker mode.
    /// </summary>
    protected string InputType => UseNativeMobilePicker ? Mode : "text";

    /// <summary>
    /// Gets the value rendered by the visible input.
    /// </summary>
    protected string VisibleInputText => UseNativeMobilePicker
        ? InputMode == DateInputMode.Week
            ? WeekDateFormat.FormatNativeValue( Value )
            : FormatValueWithFormat( Value, DateFormat )
        : InputText;

    /// <summary>
    /// Gets the minimum value rendered by the input.
    /// </summary>
    protected string InputMin => InputMode == DateInputMode.Week
        ? WeekDateFormat.FormatNativeValue( Min )
        : Min?.ToString( DateFormat, CultureInfo.InvariantCulture );

    /// <summary>
    /// Gets the maximum value rendered by the input.
    /// </summary>
    protected string InputMax => InputMode == DateInputMode.Week
        ? WeekDateFormat.FormatNativeValue( Max )
        : Max?.ToString( DateFormat, CultureInfo.InvariantCulture );

    /// <summary>
    /// Gets the step rendered by a native mobile input.
    /// </summary>
    protected string InputStep => UseNativeMobilePicker
        ? InputMode == DateInputMode.Week ? "1" : "any"
        : null;

    /// <summary>
    /// Gets the ARIA role used by the custom picker input.
    /// </summary>
    protected string InputRole => UseNativeMobilePicker ? null : "combobox";

    /// <summary>
    /// Gets the ARIA popup type used by the custom picker input.
    /// </summary>
    protected string InputAriaHasPopup => UseNativeMobilePicker ? null : "dialog";

    /// <summary>
    /// Gets the ARIA expanded state used by the custom picker input.
    /// </summary>
    protected string InputAriaExpanded => UseNativeMobilePicker ? null : CalendarVisible.ToString().ToLowerInvariant();

    /// <summary>
    /// Gets the ARIA control target used by the custom picker input.
    /// </summary>
    protected string InputAriaControls => UseNativeMobilePicker ? null : CalendarId;

    /// <summary>
    /// Gets whether keyboard opening keys are enabled for the visible input.
    /// </summary>
    protected bool InputKeyboardNavigationEnabled => !UseNativeMobilePicker
        && !IsDisabled
        && !ReadOnly
        && !Plaintext
        && OpenTrigger.HasFlag( PickerOpenTrigger.OpenKeys );

    /// <summary>
    /// Gets whether the browser's native mobile picker should be used.
    /// </summary>
    internal bool UseNativeMobilePicker => !DisableMobile
        && mobileDevice == true
        && !Plaintext
        && !Inline
        && SelectionMode == DateInputSelectionMode.Single
        && !ShowWeekNumbers
        && !DatePickerDateUtilities.HasItems( DisabledDates )
        && EnabledDates is null
        && DisabledDays is null;

    private Func<MouseEventArgs, Task> NonRenderingClickHandler
        => EventUtil.AsNonRenderingEventHandler<MouseEventArgs>( OnClickHandler );

    private Func<KeyboardEventArgs, Task> NonRenderingKeyDownHandler
        => EventUtil.AsNonRenderingEventHandler<KeyboardEventArgs>( OnKeyDownHandler );

    private Func<KeyboardEventArgs, Task> NonRenderingKeyPressHandler
        => EventUtil.AsNonRenderingEventHandler<KeyboardEventArgs>( OnKeyPressHandler );

    private Func<KeyboardEventArgs, Task> NonRenderingKeyUpHandler
        => EventUtil.AsNonRenderingEventHandler<KeyboardEventArgs>( OnKeyUpHandler );

    private Func<FocusEventArgs, Task> NonRenderingBlurHandler
        => EventUtil.AsNonRenderingEventHandler<FocusEventArgs>( OnBlurHandler );

    private Func<FocusEventArgs, Task> NonRenderingFocusHandler
        => EventUtil.AsNonRenderingEventHandler<FocusEventArgs>( OnFocusHandler );

    private Func<FocusEventArgs, Task> NonRenderingFocusInHandler
        => EventUtil.AsNonRenderingEventHandler<FocusEventArgs>( OnFocusInHandler );

    private Func<FocusEventArgs, Task> NonRenderingFocusOutHandler
        => EventUtil.AsNonRenderingEventHandler<FocusEventArgs>( OnFocusOutHandler );

    /// <summary>
    /// Gets the wrapper classes supplied by the active provider.
    /// </summary>
    protected string PickerContainerClassNames
    {
        get
        {
            return string.Join(
                " ",
                new[] { CalendarContext.ContainerClassNames, Classes?.Wrapper }
                    .Where( value => !string.IsNullOrWhiteSpace( value ) ) );
        }
    }

    /// <summary>
    /// Gets the wrapper styles supplied through <see cref="DatePickerStyles"/>.
    /// </summary>
    protected string PickerContainerStyleNames => Styles?.Wrapper;

    /// <summary>
    /// Gets only the active provider's DatePicker container classes.
    /// </summary>
    protected string ProviderPickerContainerClassNames => CalendarContext.ContainerClassNames;

    /// <summary>
    /// Gets or sets the legacy DatePicker JavaScript module.
    /// </summary>
    /// <remarks>
    /// Retained for source compatibility. The native DatePicker implementation does not use this module.
    /// </remarks>
    [Inject] public IJSDatePickerModule JSModule { get; set; }

    /// <summary>
    /// Gets or sets the DI registered <see cref="ITextLocalizerService"/>.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Specifies the DI registered <see cref="ITextLocalizer{T}"/>.
    /// </summary>
    [Inject] protected ITextLocalizer<DatePicker<TValue>> Localizer { get; set; }

    /// <summary>
    /// Gets or sets the date input-mask format converter.
    /// </summary>
    [Inject] protected IInputMaskDateTimeInputFormatConverter InputFormatConverter { get; set; }

    /// <summary>
    /// Gets or sets the existing Blazorise input-mask module used when <see cref="InputFormat"/> is defined.
    /// </summary>
    [Inject] protected IJSInputMaskModule InputMaskJSModule { get; set; }

    /// <summary>
    /// Gets or sets the document observer used to detect pointer interactions outside of the picker.
    /// </summary>
    [Inject] protected IDocumentObserver DocumentObserver { get; set; }

    /// <summary>
    /// Hints at the type of data that might be entered by the user while editing the element or its contents.
    /// </summary>
    [Parameter] public DateInputMode InputMode { get; set; } = DateInputMode.Date;

    /// <summary>
    /// Specifies the mode in which the dates can be selected.
    /// </summary>
    [Parameter] public DateInputSelectionMode SelectionMode { get; set; } = DateInputSelectionMode.Single;

    /// <summary>
    /// Overrides the range separator that is used to separate date values when <see cref="SelectionMode"/> is set to <see cref="DateInputSelectionMode.Range"/>.
    /// </summary>
    [Parameter] public string RangeSeparator { get; set; }

    /// <summary>
    /// The earliest date to accept. Updating this value does not change the selected date, even if it falls below the new minimum.
    /// </summary>
    [Parameter] public DateTimeOffset? Min { get; set; }

    /// <summary>
    /// The latest date to accept. Updating this value does not change the selected date, even if it exceeds the new maximum.
    /// </summary>
    [Parameter] public DateTimeOffset? Max { get; set; }

    /// <summary>
    /// Specifies the first day of the week used for date calculations.
    /// </summary>
    /// <remarks>
    /// In week mode this controls the visual calendar layout. Selected values continue to represent ISO Monday-to-Sunday weeks.
    /// </remarks>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Monday;

    /// <summary>
    /// Specifies the display format of the date input using the picker format syntax supported by earlier versions.
    /// </summary>
    /// <remarks>
    /// Week mode additionally supports <c>w</c>, <c>ww</c>, and <c>wo</c> for the week number and its English ordinal form.
    /// </remarks>
    [Parameter] public string DisplayFormat { get; set; }

    /// <summary>
    /// Specifies the input format mask of the date input using Blazorise's InputMask integration.
    /// </summary>
    /// <remarks>
    /// Week mode supports the <c>w</c> and <c>ww</c> week-number tokens.
    /// </remarks>
    [Parameter] public string InputFormat { get; set; }

    /// <summary>
    /// Displays time picker in 24 hour mode without AM/PM selection when enabled.
    /// </summary>
    [Parameter] public bool TimeAs24hr { get; set; }

    /// <summary>
    /// List of disabled dates that the user should not be able to pick.
    /// </summary>
    [Parameter] public IEnumerable DisabledDates { get; set; }

    /// <summary>
    /// List of enabled dates that the user should be able to pick.
    /// </summary>
    [Parameter] public IEnumerable EnabledDates { get; set; }

    /// <summary>
    /// List of disabled days in a week that the user should not be able to pick.
    /// </summary>
    [Parameter] public IEnumerable<DayOfWeek> DisabledDays { get; set; }

    /// <summary>
    /// Display the calendar in an always-open state with the inline option.
    /// </summary>
    [Parameter] public bool Inline { get; set; }

    /// <summary>
    /// Defines which interactions can open the calendar menu.
    /// </summary>
    [Parameter] public PickerOpenTrigger OpenTrigger { get; set; } = PickerOpenTrigger.All;

    /// <summary>
    /// Prevents the browser's native picker from being used on mobile devices.
    /// </summary>
    [Parameter] public bool DisableMobile { get; set; } = true;

    /// <summary>
    /// If enabled, the calendar menu will be positioned as static.
    /// </summary>
    [Parameter] public bool StaticPicker { get; set; } = true;

    /// <summary>
    /// Determines whether the calendar menu will show week numbers.
    /// </summary>
    /// <remarks>
    /// Week numbers are always shown when <see cref="InputMode"/> is <see cref="DateInputMode.Week"/>.
    /// </remarks>
    [Parameter] public bool ShowWeekNumbers { get; set; }

    /// <summary>
    /// Determines whether to show the today button in the calendar menu.
    /// </summary>
    [Parameter] public bool ShowTodayButton { get; set; }

    /// <summary>
    /// Determines whether to show the clear button in the calendar menu.
    /// </summary>
    [Parameter] public bool ShowClearButton { get; set; }

    /// <summary>
    /// Specifies the initial value of the hour element.
    /// </summary>
    [Parameter] public int DefaultHour { get; set; } = 12;

    /// <summary>
    /// Specifies the initial value of the minute element.
    /// </summary>
    [Parameter] public int DefaultMinute { get; set; } = 0;

    #endregion
}