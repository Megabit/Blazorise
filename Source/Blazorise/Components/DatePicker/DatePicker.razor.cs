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

    private bool inputMaskInitialized;

    private bool inputFocused;

    private bool? mobileDevice;

    private IAsyncDisposable outsidePointerSubscription;

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

        if ( Rendered && paramInputFormat.Defined && paramInputFormat.Changed )
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

            if ( inputMaskInitialized )
            {
                ExecuteAfterRender( DestroyInputMaskAsync );
            }
        }

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
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            if ( inputMaskInitialized )
            {
                await InputMaskJSModule.SafeDestroy( ElementRef, ElementId );
                inputMaskInitialized = false;
            }

            await DisposeOutsidePointerSubscriptionAsync();

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
    protected override async Task OnChangeHandler( ChangeEventArgs eventArgs )
    {
        string value = eventArgs?.Value?.ToString();

        inputText = value;

        if ( string.IsNullOrWhiteSpace( inputText ) )
        {
            await CurrentValueHandler( null );
            inputText = null;
            await FinishMaskedEditingAsync();
            pendingRangeStart = null;
            hoveredRangeEnd = null;
            return;
        }

        if ( TryNormalizeInputValue( inputText, out string normalizedValue ) )
        {
            await CurrentValueHandler( normalizedValue );
            SynchronizeStateFromValue( resetVisibleMonth: true );
            await FinishMaskedEditingAsync();
        }
        else if ( ParentValidation is not null )
        {
            await ParentValidation.NotifyInputChanged<TValue>( default );
        }
    }

    /// <summary>
    /// Opens the calendar when the visible input is clicked.
    /// </summary>
    [JSInvokable]
    protected async Task OnClickHandler( MouseEventArgs eventArgs )
    {
        if ( IsDisabled || ReadOnly || Plaintext )
            return;

        if ( UseNativeMobilePicker )
            return;

        await BeginMaskedEditingAsync();
        await OpenAsync();
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
                    results.Add( Formaters.FormatDateValueAsString( item, format ) );
                }
            }

            string delimiter = SelectionMode == DateInputSelectionMode.Multiple ? MULTIPLE_DELIMITER : CurrentRangeSeparator;

            return string.Join( delimiter, results );
        }

        return Formaters.FormatDateValueAsString( value, format );
    }

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
        else if ( eventArgs.Key is "ArrowDown" || eventArgs.Key is "F4" )
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
        inputFocused = true;

        await OnFocus.InvokeAsync( eventArgs );
        await BeginMaskedEditingAsync();
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

        if ( inputMaskInitialized )
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
        inputText = value;
        await OnChangeHandler( new ChangeEventArgs { Value = inputText } );
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

        await DisposeOutsidePointerSubscriptionAsync();
        await InvokeAsync( StateHasChanged );

        if ( focusInput )
        {
            ExecuteAfterRender( () => Focus() );
        }
    }

    private void SynchronizeStateFromValue( bool resetVisibleMonth )
    {
        inputText = FormatValueAsString( Value );

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
        DateTime initial = DateTime.Today
            .AddHours( Math.Clamp( DefaultHour, 0, 23 ) )
            .AddMinutes( Math.Clamp( DefaultMinute, 0, 59 ) );

        if ( Min.HasValue && initial.Date < Min.Value.Date )
        {
            initial = Min.Value.DateTime;
        }

        if ( Max.HasValue && initial.Date > Max.Value.Date )
        {
            initial = Max.Value.DateTime;
        }

        return initial;
    }

    private IReadOnlyList<DateTime> GetSelectedDates()
    {
        List<DateTime> result = new();

        if ( Value is null )
            return result;

        if ( SelectionMode != DateInputSelectionMode.Single && Value is IEnumerable values )
        {
            foreach ( object value in values )
            {
                if ( TryConvertToDateTime( value, out DateTime date ) )
                {
                    result.Add( date );
                }
            }
        }
        else if ( TryConvertToDateTime( Value, out DateTime date ) )
        {
            result.Add( date );
        }

        return result;
    }

    private static bool TryConvertToDateTime( object value, out DateTime result )
    {
        switch ( value )
        {
            case DateTime dateTime:
                result = dateTime;
                return true;
            case DateTimeOffset dateTimeOffset:
                result = dateTimeOffset.DateTime;
                return true;
            case DateOnly dateOnly:
                result = dateOnly.ToDateTime( TimeOnly.MinValue );
                return true;
            default:
                result = default;
                return false;
        }
    }

    private bool TryNormalizeInputValue( string value, out string normalizedValue )
    {
        normalizedValue = null;

        if ( string.IsNullOrWhiteSpace( value ) )
            return false;

        if ( SelectionMode == DateInputSelectionMode.Single )
        {
            if ( TryParseInputDate( value, out DateTime date ) )
            {
                normalizedValue = date.ToString( DateFormat, CultureInfo.InvariantCulture );
                return true;
            }

            return false;
        }

        string delimiter = SelectionMode == DateInputSelectionMode.Multiple ? MULTIPLE_DELIMITER : CurrentRangeSeparator;
        string[] parts = value.Split( delimiter, StringSplitOptions.None );
        List<string> normalizedDates = new();

        foreach ( string part in parts )
        {
            if ( !TryParseInputDate( part, out DateTime date ) )
                return false;

            normalizedDates.Add( date.ToString( DateFormat, CultureInfo.InvariantCulture ) );
        }

        if ( SelectionMode == DateInputSelectionMode.Range && normalizedDates.Count is < 1 or > 2 )
            return false;

        normalizedValue = string.Join( delimiter, normalizedDates );
        return true;
    }

    private bool TryParseInputDate( string value, out DateTime result )
    {
        result = default;

        string trimmedValue = value?.Trim();
        List<string> formats = new();

        AddFormat( formats, PickerDateTimeFormat.Normalize( InputFormat ) );
        AddFormat( formats, PickerDateTimeFormat.Normalize( DisplayFormat ) );
        AddFormat( formats, DateFormat );

        foreach ( string format in formats )
        {
            if ( DateTime.TryParseExact( trimmedValue, format, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out result )
                 || DateTime.TryParseExact( trimmedValue, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result ) )
            {
                return true;
            }
        }

        if ( !string.IsNullOrWhiteSpace( InputFormat ) || !string.IsNullOrWhiteSpace( DisplayFormat ) )
            return false;

        return DateTime.TryParse( trimmedValue, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out result )
            || DateTime.TryParse( trimmedValue, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result );
    }

    private static void AddFormat( ICollection<string> formats, string format )
    {
        if ( !string.IsNullOrWhiteSpace( format ) && !formats.Contains( format ) )
        {
            formats.Add( format );
        }
    }

    private async Task RefreshInputMaskAsync()
    {
        await DestroyInputMaskAsync();

        if ( string.IsNullOrWhiteSpace( InputFormat ) )
            return;

        string convertedInputFormat = InputFormatConverter.Convert( InputFormat );

        await InputMaskJSModule.Initialize( null, ElementRef, ElementId, new InputMaskJSOptions
        {
            Alias = "datetime",
            InputFormat = convertedInputFormat,
            MaskPlaceholder = "_",
            ShowMaskOnFocus = true,
            ShowMaskOnHover = true,
            ClearMaskOnLostFocus = true,
            DispatchChangeOnComplete = true,
        } );

        inputMaskInitialized = true;
    }

    private async Task DestroyInputMaskAsync()
    {
        if ( !inputMaskInitialized )
            return;

        await InputMaskJSModule.SafeDestroy( ElementRef, ElementId );
        inputMaskInitialized = false;
    }

    private async Task BeginMaskedEditingAsync()
    {
        if ( inputMaskInitialized
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

            await InvokeAsync( StateHasChanged );
        }
    }

    private async Task FinishMaskedEditingAsync()
    {
        if ( !inputMaskInitialized )
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
            return;
        }

        string delimiter = SelectionMode == DateInputSelectionMode.Multiple ? MULTIPLE_DELIMITER : CurrentRangeSeparator;
        string normalizedValue = string.Join(
            delimiter,
            dates.Select( date => date.ToString( DateFormat, CultureInfo.InvariantCulture ) ) );

        await CurrentValueHandler( normalizedValue );
        inputText = FormatValueAsString( Value );
        await FinishMaskedEditingAsync();
    }

    internal async Task SelectDateAsync( DateTime selectedDate )
    {
        if ( CalendarInteractionDisabled || IsDateDisabled( selectedDate ) )
            return;

        DateTime date = ApplyCurrentTime( selectedDate );
        focusedDate = date;
        visibleMonth = new DateTime( date.Year, date.Month, 1 );

        if ( SelectionMode == DateInputSelectionMode.Single )
        {
            await CommitDatesAsync( new[] { date } );
            await CloseCalendarAsync( focusInput: true );
        }
        else if ( SelectionMode == DateInputSelectionMode.Range )
        {
            if ( !pendingRangeStart.HasValue )
            {
                pendingRangeStart = date;
                hoveredRangeEnd = null;
            }
            else
            {
                DateTime start = pendingRangeStart.Value;
                DateTime end = date;

                if ( end < start )
                {
                    ( start, end ) = ( end, start );
                }

                pendingRangeStart = null;
                hoveredRangeEnd = null;

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
        if ( SelectionMode == DateInputSelectionMode.Range && pendingRangeStart.HasValue )
        {
            hoveredRangeEnd = date;
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

        await SelectDateAsync( today );
    }

    internal async Task ClearAsync()
    {
        if ( CalendarInteractionDisabled )
            return;

        pendingRangeStart = null;
        hoveredRangeEnd = null;
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

    internal void ChangeVisibleMonth( ChangeEventArgs eventArgs )
    {
        if ( CalendarInteractionDisabled )
            return;

        if ( int.TryParse( eventArgs?.Value?.ToString(), out int month ) && month is >= 1 and <= 12 )
        {
            visibleMonth = new DateTime( visibleMonth.Year, month, 1 );
            focusedDate = MoveIntoMonth( focusedDate, visibleMonth );
        }
    }

    internal void ChangeVisibleYear( ChangeEventArgs eventArgs )
    {
        if ( CalendarInteractionDisabled )
            return;

        if ( int.TryParse( eventArgs?.Value?.ToString(), out int year ) && year is >= 1 and <= 9999 )
        {
            visibleMonth = new DateTime( year, visibleMonth.Month, 1 );
            focusedDate = MoveIntoMonth( focusedDate, visibleMonth );
        }
    }

    internal async Task OnCalendarKeyDownAsync( KeyboardEventArgs eventArgs )
    {
        if ( eventArgs is null || CalendarInteractionDisabled )
            return;

        bool monthMode = InputMode == DateInputMode.Month;

        switch ( eventArgs.Key )
        {
            case "ArrowLeft":
                MoveFocus( -1, monthMode );
                break;
            case "ArrowRight":
                MoveFocus( 1, monthMode );
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
        if ( !TryMoveDate( focusedDate, amount, byMonth, out DateTime candidate ) )
            return;

        int attempts = 0;

        while ( ( byMonth ? IsMonthDisabled( candidate ) : IsDateDisabled( candidate ) ) && attempts++ < 3660 )
        {
            if ( !TryMoveDate( candidate, Math.Sign( amount ), byMonth, out candidate ) )
                return;
        }

        focusedDate = candidate;
        visibleMonth = new DateTime( candidate.Year, candidate.Month, 1 );
    }

    private void MoveFocusedMonth( int months )
    {
        if ( !TryMoveDate( visibleMonth, months, byMonth: true, out DateTime targetMonth ) )
            return;

        visibleMonth = new DateTime( targetMonth.Year, targetMonth.Month, 1 );
        focusedDate = MoveIntoMonth( focusedDate, visibleMonth );
    }

    private static bool TryMoveDate( DateTime date, int amount, bool byMonth, out DateTime result )
    {
        try
        {
            result = byMonth ? date.AddMonths( amount ) : date.AddDays( amount );
            return true;
        }
        catch ( ArgumentOutOfRangeException )
        {
            result = date;
            return false;
        }
    }

    private static DateTime MoveIntoMonth( DateTime date, DateTime month )
    {
        int day = Math.Min( Math.Max( date.Day, 1 ), DateTime.DaysInMonth( month.Year, month.Month ) );
        return new DateTime( month.Year, month.Month, day, date.Hour, date.Minute, date.Second, date.Kind );
    }

    private void MoveFocusToWeekBoundary( bool beginning )
    {
        int offset = ( 7 + (int)focusedDate.DayOfWeek - (int)FirstDayOfWeek ) % 7;
        focusedDate = beginning
            ? focusedDate.AddDays( -offset )
            : focusedDate.AddDays( 6 - offset );
        visibleMonth = new DateTime( focusedDate.Year, focusedDate.Month, 1 );
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

        if ( ContainsDate( DisabledDates, day ) )
            return true;

        if ( DisabledDays?.Contains( day.DayOfWeek ) == true )
            return true;

        if ( EnabledDates is not null && !ContainsDate( EnabledDates, day ) )
            return true;

        return false;
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

        if ( EnumerateDates( DisabledDates ).Any( date => date.Year == month.Year && date.Month == month.Month ) )
            return true;

        if ( EnabledDates is not null )
        {
            return !EnumerateDates( EnabledDates ).Any( date => date.Year == month.Year && date.Month == month.Month );
        }

        return false;
    }

    private static bool ContainsDate( IEnumerable values, DateTime date )
    {
        return EnumerateDates( values ).Any( item => item.Date == date.Date );
    }

    private static IEnumerable<DateTime> EnumerateDates( IEnumerable values )
    {
        if ( values is null )
            yield break;

        foreach ( object value in values )
        {
            if ( TryConvertToDateTime( value, out DateTime date ) )
            {
                yield return date;
            }
        }
    }

    internal IReadOnlyList<DatePickerCalendarWeek> BuildCalendarWeeks()
    {
        List<DatePickerCalendarWeek> weeks = new();
        IReadOnlyList<DateTime> selectedDates = GetSelectedDates();
        DateTime firstOfMonth = new( visibleMonth.Year, visibleMonth.Month, 1 );
        int leadingDays = ( 7 + (int)firstOfMonth.DayOfWeek - (int)FirstDayOfWeek ) % 7;
        DateTime gridStart = firstOfMonth.AddDays( -leadingDays );
        ( DateTime? rangeStart, DateTime? rangeEnd ) = GetDisplayRange( selectedDates );

        for ( int weekIndex = 0; weekIndex < 6; weekIndex++ )
        {
            List<DatePickerCalendarDay> days = new();
            DateTime weekStart = gridStart.AddDays( weekIndex * 7 );

            for ( int dayIndex = 0; dayIndex < 7; dayIndex++ )
            {
                DateTime date = weekStart.AddDays( dayIndex );
                bool rangeStartDay = rangeStart.HasValue && date.Date == rangeStart.Value.Date;
                bool rangeEndDay = rangeEnd.HasValue && date.Date == rangeEnd.Value.Date;
                bool inRange = rangeStart.HasValue
                    && rangeEnd.HasValue
                    && date.Date >= rangeStart.Value.Date
                    && date.Date <= rangeEnd.Value.Date;
                bool selected = SelectionMode == DateInputSelectionMode.Multiple
                    ? selectedDates.Any( item => item.Date == date.Date )
                    : rangeStartDay || rangeEndDay || SelectionMode == DateInputSelectionMode.Single
                        && selectedDates.Any( item => item.Date == date.Date );

                days.Add( new DatePickerCalendarDay(
                    date,
                    date.Month != visibleMonth.Month,
                    date.Date == DateTime.Today,
                    selected,
                    rangeStartDay,
                    inRange,
                    rangeEndDay,
                    IsDateDisabled( date ),
                    date.Date == focusedDate.Date ) );
            }

            weeks.Add( new DatePickerCalendarWeek( ISOWeek.GetWeekOfYear( weekStart ), days ) );
        }

        return weeks;
    }

    internal IReadOnlyList<DatePickerCalendarMonth> BuildCalendarMonths()
    {
        IReadOnlyList<DateTime> selectedDates = GetSelectedDates();
        List<DatePickerCalendarMonth> months = new();

        for ( int monthIndex = 1; monthIndex <= 12; monthIndex++ )
        {
            DateTime month = new( visibleMonth.Year, monthIndex, 1 );

            months.Add( new DatePickerCalendarMonth(
                month,
                MonthNames[monthIndex - 1],
                selectedDates.Any( item => item.Year == month.Year && item.Month == month.Month ),
                IsMonthDisabled( month ),
                focusedDate.Year == month.Year && focusedDate.Month == month.Month ) );
        }

        return months;
    }

    private ( DateTime? Start, DateTime? End ) GetDisplayRange( IReadOnlyList<DateTime> selectedDates )
    {
        if ( SelectionMode == DateInputSelectionMode.Range )
        {
            if ( pendingRangeStart.HasValue )
            {
                DateTime end = hoveredRangeEnd ?? focusedDate;
                return end < pendingRangeStart.Value
                    ? ( end, pendingRangeStart.Value )
                    : ( pendingRangeStart.Value, end );
            }

            if ( selectedDates.Count > 0 )
            {
                return ( selectedDates[0], selectedDates.Count > 1 ? selectedDates[1] : selectedDates[0] );
            }
        }

        return ( null, null );
    }

    private void OnLocalizationChanged( object sender, EventArgs eventArgs )
    {
        inputText = FormatValueAsString( Value );
        _ = InvokeAsync( StateHasChanged );
    }

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

    #region Calendar rendering helpers

    internal bool CalendarVisible => !UseNativeMobilePicker && ( Inline || calendarOpen );

    internal bool FocusCalendarOnOpen => focusCalendarOnOpen;

    internal int CalendarControlTabIndex => FocusCalendarOnOpen ? 0 : -1;

    internal bool CalendarInteractionDisabled => IsDisabled || ReadOnly || Plaintext;

    internal string CalendarId => $"{ElementId}-calendar";

    internal string PickerContainerId => $"{ElementId}-container";

    internal string CalendarLabel => InputMode == DateInputMode.Month
        ? $"{visibleMonth.Year}"
        : $"{MonthNames[visibleMonth.Month - 1]} {visibleMonth.Year}";

    internal DateTime FocusedDate => focusedDate;

    internal int VisibleMonthNumber => visibleMonth.Month;

    internal int VisibleMonthYear => visibleMonth.Year;

    internal string CalendarContainerClassNames => ClassProvider.DatePickerContainer( Inline, CalendarVisible );

    internal string CalendarClassNames => ClassProvider.DatePickerCalendar( Inline, StaticPicker );

    internal string CalendarBackdropClassNames => ClassProvider.DatePickerCalendarBackdrop();

    internal string CalendarHeaderClassNames => ClassProvider.DatePickerCalendarHeader();

    internal string CalendarNavigationClassNames => ClassProvider.DatePickerCalendarNavigation();

    internal string CalendarTitleClassNames => ClassProvider.DatePickerCalendarTitle();

    internal string CalendarGridClassNames => ClassProvider.DatePickerCalendarGrid();

    internal string CalendarWeekdaysClassNames => ClassProvider.DatePickerCalendarWeekdays();

    internal string CalendarWeekdayClassNames => ClassProvider.DatePickerCalendarWeekday();

    internal string CalendarWeekClassNames => ClassProvider.DatePickerCalendarWeek();

    internal string CalendarWeekNumberClassNames => ClassProvider.DatePickerCalendarWeekNumber();

    internal string CalendarMonthsClassNames => ClassProvider.DatePickerCalendarMonths();

    internal string CalendarTimeClassNames => ClassProvider.DatePickerCalendarTime();

    internal string CalendarTimeInputClassNames => ClassProvider.DatePickerCalendarTimeInput();

    internal string CalendarActionsClassNames => ClassProvider.DatePickerCalendarActions();

    internal string CalendarButtonClassNames => ClassProvider.DatePickerCalendarButton();

    internal string GetCalendarDayClassNames( DatePickerCalendarDay day )
    {
        return ClassProvider.DatePickerCalendarDay(
            day.Outside,
            day.Today,
            day.Selected,
            day.RangeStart,
            day.InRange,
            day.RangeEnd,
            day.Disabled,
            day.Focused && FocusCalendarOnOpen );
    }

    internal string GetCalendarMonthClassNames( DatePickerCalendarMonth month )
    {
        return ClassProvider.DatePickerCalendarMonth( month.Selected, month.Disabled, month.Focused && FocusCalendarOnOpen );
    }

    internal string GetDayId( DateTime date )
    {
        return $"{ElementId}-day-{date:yyyyMMdd}";
    }

    internal string GetMonthId( DateTime date )
    {
        return $"{ElementId}-month-{date:yyyyMM}";
    }

    internal string GetDateAriaLabel( DateTime date )
    {
        return date.ToString( "D", CultureInfo.CurrentCulture );
    }

    internal string[] WeekdayNames
    {
        get
        {
            string[] names =
            {
                Localizer["Sun"],
                Localizer["Mon"],
                Localizer["Tue"],
                Localizer["Wed"],
                Localizer["Thu"],
                Localizer["Fri"],
                Localizer["Sat"],
            };

            return Enumerable.Range( 0, 7 )
                .Select( index => names[( index + (int)FirstDayOfWeek ) % 7] )
                .ToArray();
        }
    }

    internal string[] MonthNames =>
    [
        Localizer["January"],
        Localizer["February"],
        Localizer["March"],
        Localizer["April"],
        Localizer["May!"],
        Localizer["June"],
        Localizer["July"],
        Localizer["August"],
        Localizer["September"],
        Localizer["October"],
        Localizer["November"],
        Localizer["December"],
    ];

    internal string TodayText => Localizer["Today"];

    internal string ClearText => Localizer["Clear"];

    internal string MonthText => "Month";

    internal string YearText => "Year";

    internal string WeekText => "Wk";

    internal string TimeText => "Time";

    internal string HourText => "Hour";

    internal string MinuteText => "Minute";

    internal string PreviousPeriodAriaLabel => InputMode == DateInputMode.Month ? "Previous year" : "Previous month";

    internal string NextPeriodAriaLabel => InputMode == DateInputMode.Month ? "Next year" : "Next month";

    internal string PreviousText => CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? "\u203A" : "\u2039";

    internal string NextText => CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? "\u2039" : "\u203A";

    internal int CurrentHour => GetCurrentTimeSource().Hour;

    internal int CurrentMinute => GetCurrentTimeSource().Minute;

    internal int DisplayHour => TimeAs24hr ? CurrentHour : CurrentHour % 12 == 0 ? 12 : CurrentHour % 12;

    internal string DisplayHourText => DisplayHour.ToString( "D2", CultureInfo.InvariantCulture );

    internal string CurrentMinuteText => CurrentMinute.ToString( "D2", CultureInfo.InvariantCulture );

    internal bool IsPostMeridiem => CurrentHour >= 12;

    internal string MeridiemText => Localizer[IsPostMeridiem ? "PM" : "AM"];

    private Task HandleOutsidePointerAsync( DocumentEventArgs eventArgs )
    {
        return CloseCalendarAsync( focusInput: false ).AsTask();
    }

    private async ValueTask SynchronizeOutsidePointerSubscriptionAsync()
    {
        if ( calendarOpen && !Inline )
        {
            outsidePointerSubscription ??= await DocumentObserver.Subscribe( new()
            {
                OwnerId = ElementId,
                EventTypes = DocumentEventTypes.PointerDown,
                ExcludeSelector = $"{CssSelectorUtilities.BuildElementIdSelector( PickerContainerId )}, {CssSelectorUtilities.BuildElementIdSelector( CalendarId )}",
                Priority = -100,
                Handler = HandleOutsidePointerAsync,
            } );
        }
        else
        {
            await DisposeOutsidePointerSubscriptionAsync();
        }
    }

    private async ValueTask DisposeOutsidePointerSubscriptionAsync()
    {
        if ( outsidePointerSubscription is null )
            return;

        await outsidePointerSubscription.DisposeAsync();
        outsidePointerSubscription = null;
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

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
    protected string EffectiveDisplayFormat => PickerDateTimeFormat.Normalize( DisplayFormat ?? ( InputMode == DateInputMode.DateTime ? DEFAULT_DATETIME_DISPLAY_FORMAT : DateFormat ) );

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
        ? FormatValueWithFormat( Value, DateFormat )
        : InputText;

    /// <summary>
    /// Gets the minimum value rendered by the input.
    /// </summary>
    protected string InputMin => Min?.ToString( DateFormat, CultureInfo.InvariantCulture );

    /// <summary>
    /// Gets the maximum value rendered by the input.
    /// </summary>
    protected string InputMax => Max?.ToString( DateFormat, CultureInfo.InvariantCulture );

    /// <summary>
    /// Gets the step rendered by a native mobile input.
    /// </summary>
    protected string InputStep => UseNativeMobilePicker ? "any" : null;

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
    /// Gets whether the browser's native mobile picker should be used.
    /// </summary>
    internal bool UseNativeMobilePicker => !DisableMobile
        && mobileDevice == true
        && !Plaintext
        && !Inline
        && SelectionMode == DateInputSelectionMode.Single
        && !ShowWeekNumbers
        && !HasItems( DisabledDates )
        && !HasItems( EnabledDates )
        && !HasItems( DisabledDays );

    private static bool HasItems( IEnumerable items )
    {
        if ( items is null )
            return false;

        IEnumerator enumerator = items.GetEnumerator();

        try
        {
            return enumerator.MoveNext();
        }
        finally
        {
            ( enumerator as IDisposable )?.Dispose();
        }
    }

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
                new[] { CalendarContainerClassNames, Classes?.Wrapper }
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
    protected string ProviderPickerContainerClassNames => CalendarContainerClassNames;

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
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Monday;

    /// <summary>
    /// Specifies the display format of the date input using the picker format syntax supported by earlier versions.
    /// </summary>
    [Parameter] public string DisplayFormat { get; set; }

    /// <summary>
    /// Specifies the input format mask of the date input using Blazorise's InputMask integration.
    /// </summary>
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