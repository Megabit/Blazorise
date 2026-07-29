#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Localization;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Identifies the active field in the native time selection menu.
/// </summary>
internal enum TimePickerPart
{
    Hour,
    Minute,
    Second,
    Meridiem,
}

/// <summary>
/// An editor that displays a time value and allows a user to edit the value.
/// </summary>
/// <typeparam name="TValue">Data-type to be binded by the <see cref="TimePicker{TValue}"/> property.</typeparam>
public partial class TimePicker<TValue> : BaseTextInput<TValue, TimePickerClasses, TimePickerStyles>, IAsyncDisposable, ITimePicker
{
    #region Members

    /// <summary>
    /// Captured Min parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<TimeSpan?> paramMin;

    /// <summary>
    /// Captured Max parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<TimeSpan?> paramMax;

    /// <summary>
    /// Captured DisplayFormat parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<string> paramDisplayFormat;

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
    /// Captured Seconds parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramSeconds;

    /// <summary>
    /// Captured HourIncrement parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<int> paramHourIncrement;

    /// <summary>
    /// Captured MinuteIncrement parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<int> paramMinuteIncrement;

    /// <summary>
    /// Captured DefaultHour parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<int> paramDefaultHour;

    /// <summary>
    /// Captured DefaultMinute parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<int> paramDefaultMinute;

    private bool stateInitialized;

    private bool menuOpen;

    private bool focusMenuOnOpen;

    private string inputText;

    private TimeSpan selectedTime;

    private TimePickerPart focusedPart;

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
        parameters.TryGetParameter( DisplayFormat, out paramDisplayFormat );
        parameters.TryGetParameter( TimeAs24hr, out paramTimeAs24hr );
        parameters.TryGetParameter( Disabled, out paramDisabled );
        parameters.TryGetParameter( ReadOnly, out paramReadOnly );
        parameters.TryGetParameter( Inline, out paramInline );
        parameters.TryGetParameter( DisableMobile, out paramDisableMobile );
        parameters.TryGetParameter( Placeholder, out paramPlaceholder );
        parameters.TryGetParameter( StaticPicker, out paramStaticPicker );
        parameters.TryGetParameter( Seconds, out paramSeconds );
        parameters.TryGetParameter( HourIncrement, out paramHourIncrement );
        parameters.TryGetParameter( MinuteIncrement, out paramMinuteIncrement );
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
            || ( paramTimeAs24hr.Defined && paramTimeAs24hr.Changed )
            || ( paramSeconds.Defined && paramSeconds.Changed );
        bool defaultChanged = ( paramDefaultHour.Defined && paramDefaultHour.Changed )
            || ( paramDefaultMinute.Defined && paramDefaultMinute.Changed );
        bool limitsChanged = ( paramMin.Defined && paramMin.Changed )
            || ( paramMax.Defined && paramMax.Changed );

        if ( !stateInitialized || paramValue.Changed || formatChanged || ( defaultChanged && Value is null ) || limitsChanged )
        {
            SynchronizeStateFromValue();
        }

        if ( paramInline.Defined && paramInline.Changed )
        {
            menuOpen = Inline;
            focusMenuOnOpen = false;
            await SynchronizeOutsidePointerSubscriptionAsync();
        }

        if ( ( !Seconds && focusedPart == TimePickerPart.Second )
             || ( TimeAs24hr && focusedPart == TimePickerPart.Meridiem ) )
        {
            focusedPart = TimePickerPart.Hour;
        }

        if ( Rendered && paramDisableMobile.Defined && paramDisableMobile.Changed && !DisableMobile )
        {
            ExecuteAfterRender( DetectMobileDeviceAsync );
        }

        if ( Rendered && UseNativeMobilePicker )
        {
            menuOpen = false;
            focusMenuOnOpen = false;
            await DisposeOutsidePointerSubscriptionAsync();
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
            await DisposeOutsidePointerSubscriptionAsync();
            LocalizerService.LocalizationChanged -= OnLocalizationChanged;
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TimePicker( Plaintext ) );
        builder.Append( ClassProvider.TimePickerSize( ThemeSize ) );
        builder.Append( ClassProvider.TimePickerColor( Color ) );
        builder.Append( ClassProvider.TimePickerValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override async Task OnChangeHandler( ChangeEventArgs eventArgs )
    {
        inputText = eventArgs?.Value?.ToString();

        if ( string.IsNullOrWhiteSpace( inputText ) )
        {
            await CurrentValueHandler( null );
            selectedTime = GetDefaultTime();
            return;
        }

        if ( TryNormalizeInputValue( inputText, out string normalizedValue, out TimeSpan parsedTime ) )
        {
            selectedTime = ClampTime( parsedTime );
            normalizedValue = FormatInternalTime( selectedTime );

            await CurrentValueHandler( normalizedValue );

            inputText = FormatTime( selectedTime );
        }
        else if ( ParentValidation is not null )
        {
            await ParentValidation.NotifyInputChanged<TValue>( default );
        }
    }

    /// <summary>
    /// Handles text input from the editable TimePicker field.
    /// </summary>
    protected Task OnInputHandler( ChangeEventArgs eventArgs )
    {
        inputText = eventArgs?.Value?.ToString();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Opens the time menu when the visible input is clicked.
    /// </summary>
    [JSInvokable]
    protected async Task OnClickHandler( MouseEventArgs eventArgs )
    {
        if ( MenuInteractionDisabled )
            return;

        if ( UseNativeMobilePicker )
            return;

        await OpenAsync();
    }

    /// <inheritdoc/>
    protected override string FormatValueAsString( TValue value )
    {
        if ( value is null )
            return null;

        if ( TryGetTime( value, out TimeSpan time ) )
            return FormatInternalTime( time );

        throw new InvalidOperationException( $"Unsupported type {value.GetType()}" );
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
    {
        if ( Parsers.TryParseTime<TValue>( value, out TValue result ) )
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

        if ( MenuInteractionDisabled || eventArgs is null )
            return;

        if ( UseNativeMobilePicker )
            return;

        if ( MenuVisible )
        {
            if ( !focusMenuOnOpen && eventArgs.Key is "ArrowDown" or "F4" )
            {
                await OpenMenuAsync( focusMenu: true );
            }
            else if ( eventArgs.Key is "Escape" or "Tab" )
            {
                await CloseMenuAsync( focusInput: false );
            }
        }
        else if ( eventArgs.Key is "ArrowDown" or "F4" )
        {
            await OpenMenuAsync( focusMenu: true );
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
    public new virtual Task OnFocusHandler( FocusEventArgs eventArgs )
    {
        return OnFocus.InvokeAsync( eventArgs );
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
    public new virtual Task OnBlurHandler( FocusEventArgs eventArgs )
    {
        return base.OnBlurHandler( eventArgs );
    }

    /// <inheritdoc/>
    protected override async Task OnScreenKeyboardValueChanged( string value )
    {
        inputText = value;
        await OnChangeHandler( new ChangeEventArgs { Value = inputText } );
    }

    /// <summary>
    /// Opens the time dropdown.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask OpenAsync()
    {
        if ( MenuInteractionDisabled )
            return ValueTask.CompletedTask;

        if ( UseNativeMobilePicker )
            return JSUtilitiesModule.ShowPicker( ElementRef, ElementId );

        return OpenMenuAsync( focusMenu: false );
    }

    private async ValueTask OpenMenuAsync( bool focusMenu )
    {
        if ( MenuInteractionDisabled )
            return;

        if ( UseNativeMobilePicker )
        {
            await JSUtilitiesModule.ShowPicker( ElementRef, ElementId );
            return;
        }

        SynchronizeSelectionForOpen();
        focusedPart = TimePickerPart.Hour;
        focusMenuOnOpen = focusMenu;
        menuOpen = true;

        await InvokeAsync( StateHasChanged );
        await SynchronizeOutsidePointerSubscriptionAsync();
    }

    /// <summary>
    /// Closes the time dropdown.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask CloseAsync()
    {
        return CloseMenuAsync( focusInput: false );
    }

    /// <summary>
    /// Shows/opens the time dropdown if its closed, hides/closes it otherwise.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask ToggleAsync()
    {
        if ( MenuVisible && !Inline )
        {
            await CloseMenuAsync( focusInput: false );
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

    internal async Task OnMenuKeyDownAsync( KeyboardEventArgs eventArgs )
    {
        if ( eventArgs is null || MenuInteractionDisabled )
            return;

        switch ( eventArgs.Key )
        {
            case "ArrowLeft":
                MoveFocusedPart( -1 );
                break;
            case "ArrowRight":
                MoveFocusedPart( 1 );
                break;
            case "ArrowUp":
                await AdjustFocusedPartAsync( 1 );
                break;
            case "ArrowDown":
                await AdjustFocusedPartAsync( -1 );
                break;
            case "Home":
                await SetBoundaryAsync( useMaximum: false );
                break;
            case "End":
                await SetBoundaryAsync( useMaximum: true );
                break;
            case "Enter":
            case " ":
                if ( focusedPart == TimePickerPart.Meridiem )
                {
                    await ToggleMeridiemAsync();
                }
                else if ( !Inline )
                {
                    await CloseMenuAsync( focusInput: true );
                }
                break;
            case "Escape":
                await CloseMenuAsync( focusInput: true );
                break;
        }
    }

    internal async Task OnControlKeyDownAsync( TimePickerPart part, KeyboardEventArgs eventArgs )
    {
        FocusPart( part );

        if ( eventArgs is null )
            return;

        switch ( eventArgs.Key )
        {
            case "Escape":
                await CloseMenuAsync( focusInput: true );
                break;
            case "Enter":
                if ( part != TimePickerPart.Meridiem && !Inline )
                {
                    await CloseMenuAsync( focusInput: true );
                }
                break;
        }
    }

    internal async Task ChangeHourAsync( ChangeEventArgs eventArgs )
    {
        if ( !int.TryParse( eventArgs?.Value?.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int hour ) )
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

        await CommitTimeAsync( new TimeSpan( hour, CurrentMinute, CurrentSecond ) );
    }

    internal async Task ChangeMinuteAsync( ChangeEventArgs eventArgs )
    {
        if ( int.TryParse( eventArgs?.Value?.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int minute ) )
        {
            await CommitTimeAsync( new TimeSpan( CurrentHour, Math.Clamp( minute, 0, 59 ), CurrentSecond ) );
        }
    }

    internal async Task ChangeSecondAsync( ChangeEventArgs eventArgs )
    {
        if ( int.TryParse( eventArgs?.Value?.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int second ) )
        {
            await CommitTimeAsync( new TimeSpan( CurrentHour, CurrentMinute, Math.Clamp( second, 0, 59 ) ) );
        }
    }

    internal Task ToggleMeridiemAsync()
    {
        int hour = CurrentHour >= 12 ? CurrentHour - 12 : CurrentHour + 12;
        return CommitTimeAsync( new TimeSpan( hour, CurrentMinute, CurrentSecond ) );
    }

    internal void FocusPart( TimePickerPart part )
    {
        focusedPart = part;
    }

    internal string GetMenuControlClassNames( TimePickerPart part )
    {
        return ClassProvider.TimePickerControl( focusedPart == part );
    }

    internal string GetPartId( TimePickerPart part )
    {
        return $"{ElementId}-{part.ToString().ToLowerInvariant()}";
    }

    private async ValueTask CloseMenuAsync( bool focusInput )
    {
        if ( Inline )
            return;

        menuOpen = false;
        focusMenuOnOpen = false;

        await DisposeOutsidePointerSubscriptionAsync();
        await InvokeAsync( StateHasChanged );

        if ( focusInput )
        {
            ExecuteAfterRender( () => Focus() );
        }
    }

    private Task HandleOutsidePointerAsync( DocumentEventArgs eventArgs )
    {
        return CloseMenuAsync( focusInput: false ).AsTask();
    }

    private async ValueTask SynchronizeOutsidePointerSubscriptionAsync()
    {
        if ( menuOpen && !Inline )
        {
            outsidePointerSubscription ??= await DocumentObserver.Subscribe( new()
            {
                OwnerId = ElementId,
                EventTypes = DocumentEventTypes.PointerDown,
                ExcludeSelector = CssSelectorUtilities.BuildElementIdSelector( PickerContainerId ),
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

    private void SynchronizeStateFromValue()
    {
        if ( TryGetTime( Value, out TimeSpan time ) )
        {
            selectedTime = ClampTime( time );
            inputText = FormatTime( selectedTime );
        }
        else if ( Value is not null )
        {
            throw new InvalidOperationException( $"Unsupported type {Value.GetType()}" );
        }
        else
        {
            selectedTime = GetDefaultTime();
            inputText = null;
        }
    }

    private void SynchronizeSelectionForOpen()
    {
        if ( TryGetTime( Value, out TimeSpan time ) )
        {
            selectedTime = ClampTime( time );
        }
        else if ( TryNormalizeInputValue( inputText, out _, out TimeSpan parsedTime ) )
        {
            selectedTime = ClampTime( parsedTime );
        }
        else
        {
            selectedTime = GetDefaultTime();
        }
    }

    private async Task CommitTimeAsync( TimeSpan time )
    {
        selectedTime = NormalizeTime( time );

        if ( !Seconds )
        {
            selectedTime = new TimeSpan( selectedTime.Hours, selectedTime.Minutes, 0 );
        }

        selectedTime = ClampTime( selectedTime );

        string normalizedValue = FormatInternalTime( selectedTime );

        await CurrentValueHandler( normalizedValue );

        inputText = FormatTime( selectedTime );
    }

    private async Task AdjustFocusedPartAsync( int direction )
    {
        switch ( focusedPart )
        {
            case TimePickerPart.Hour:
                await CommitTimeAsync( selectedTime.Add( TimeSpan.FromHours( direction * SafeHourIncrement ) ) );
                break;
            case TimePickerPart.Minute:
                await CommitTimeAsync( selectedTime.Add( TimeSpan.FromMinutes( direction * SafeMinuteIncrement ) ) );
                break;
            case TimePickerPart.Second:
                await CommitTimeAsync( selectedTime.Add( TimeSpan.FromSeconds( direction ) ) );
                break;
            case TimePickerPart.Meridiem:
                await ToggleMeridiemAsync();
                break;
        }
    }

    private async Task SetBoundaryAsync( bool useMaximum )
    {
        TimeSpan boundary = useMaximum
            ? Max.HasValue ? NormalizeTime( Max.Value ) : new TimeSpan( 23, 59, Seconds ? 59 : 0 )
            : Min.HasValue ? NormalizeTime( Min.Value ) : TimeSpan.Zero;

        await CommitTimeAsync( boundary );
    }

    private void MoveFocusedPart( int direction )
    {
        List<TimePickerPart> parts = new()
        {
            TimePickerPart.Hour,
            TimePickerPart.Minute,
        };

        if ( Seconds )
        {
            parts.Add( TimePickerPart.Second );
        }

        if ( !TimeAs24hr )
        {
            parts.Add( TimePickerPart.Meridiem );
        }

        int currentIndex = parts.IndexOf( focusedPart );
        int nextIndex = ( currentIndex + direction + parts.Count ) % parts.Count;

        focusedPart = parts[nextIndex];
    }

    private bool TryNormalizeInputValue( string value, out string normalizedValue, out TimeSpan result )
    {
        normalizedValue = null;
        result = default;

        if ( string.IsNullOrWhiteSpace( value ) || !TryParseInputTime( value, out result ) )
            return false;

        result = ClampTime( NormalizeTime( result ) );
        normalizedValue = FormatInternalTime( result );

        return true;
    }

    private bool TryParseInputTime( string value, out TimeSpan result )
    {
        result = default;

        string trimmedValue = value?.Trim();
        List<string> formats = new();

        AddFormat( formats, PickerDateTimeFormat.Normalize( DisplayFormat ) );
        AddFormat( formats, EffectiveDisplayFormat );
        AddFormat( formats, "HH:mm:ss" );
        AddFormat( formats, "HH:mm" );
        AddFormat( formats, "H:mm" );
        AddFormat( formats, "hh:mm:ss tt" );
        AddFormat( formats, "hh:mm tt" );
        AddFormat( formats, "h:mm tt" );

        foreach ( string format in formats )
        {
            if ( DateTime.TryParseExact( trimmedValue, format, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime parsedDateTime )
                 || DateTime.TryParseExact( trimmedValue, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out parsedDateTime ) )
            {
                result = parsedDateTime.TimeOfDay;
                return true;
            }
        }

        if ( !string.IsNullOrWhiteSpace( DisplayFormat ) )
            return false;

        string cultureSeparator = CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator;
        bool hasTimeSyntax = trimmedValue.Contains( ":", StringComparison.Ordinal )
            || ( !string.IsNullOrEmpty( cultureSeparator ) && trimmedValue.Contains( cultureSeparator, StringComparison.Ordinal ) )
            || ( !string.IsNullOrEmpty( CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator ) && trimmedValue.Contains( CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator, StringComparison.OrdinalIgnoreCase ) )
            || ( !string.IsNullOrEmpty( CultureInfo.CurrentCulture.DateTimeFormat.PMDesignator ) && trimmedValue.Contains( CultureInfo.CurrentCulture.DateTimeFormat.PMDesignator, StringComparison.OrdinalIgnoreCase ) );

        if ( !hasTimeSyntax )
            return false;

        if ( TimeSpan.TryParse( trimmedValue, CultureInfo.CurrentCulture, out result )
             || TimeSpan.TryParse( trimmedValue, CultureInfo.InvariantCulture, out result ) )
        {
            return true;
        }

        if ( DateTime.TryParse( trimmedValue, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime parsed )
             || DateTime.TryParse( trimmedValue, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out parsed ) )
        {
            result = parsed.TimeOfDay;
            return true;
        }

        return false;
    }

    private static void AddFormat( ICollection<string> formats, string format )
    {
        if ( !string.IsNullOrWhiteSpace( format ) && !formats.Contains( format ) )
        {
            formats.Add( format );
        }
    }

    private static bool TryGetTime( object value, out TimeSpan result )
    {
        switch ( value )
        {
            case TimeSpan timeSpan:
                result = NormalizeTime( timeSpan );
                return true;
            case TimeOnly timeOnly:
                result = timeOnly.ToTimeSpan();
                return true;
            case DateTime dateTime:
                result = dateTime.TimeOfDay;
                return true;
            default:
                result = default;
                return false;
        }
    }

    private TimeSpan GetDefaultTime()
    {
        TimeSpan result = new(
            Math.Clamp( DefaultHour, 0, 23 ),
            Math.Clamp( DefaultMinute, 0, 59 ),
            0 );

        return ClampTime( result );
    }

    private TimeSpan ClampTime( TimeSpan time )
    {
        time = NormalizeTime( time );

        if ( Min.HasValue && time < NormalizeTime( Min.Value ) )
        {
            time = NormalizeTime( Min.Value );
        }

        if ( Max.HasValue && time > NormalizeTime( Max.Value ) )
        {
            time = NormalizeTime( Max.Value );
        }

        return time;
    }

    private static TimeSpan NormalizeTime( TimeSpan time )
    {
        long ticks = time.Ticks % TimeSpan.TicksPerDay;

        if ( ticks < 0 )
        {
            ticks += TimeSpan.TicksPerDay;
        }

        return TimeSpan.FromTicks( ticks );
    }

    private string FormatTime( TimeSpan time )
    {
        return DateTime.Today.Add( NormalizeTime( time ) ).ToString( EffectiveDisplayFormat, CultureInfo.CurrentCulture );
    }

    private static string FormatInternalTime( TimeSpan time )
    {
        return NormalizeTime( time ).ToString( Parsers.InternalTimeFormat.ToLowerInvariant(), CultureInfo.InvariantCulture );
    }

    private static string FormatNativeTime( TimeSpan time, bool includeSeconds )
    {
        return NormalizeTime( time ).ToString( includeSeconds ? @"hh\:mm\:ss" : @"hh\:mm", CultureInfo.InvariantCulture );
    }

    private async Task DetectMobileDeviceAsync()
    {
        if ( DisableMobile || mobileDevice.HasValue )
            return;

        bool detectedMobileDevice;

        if ( JSUtilitiesModule is Blazorise.Modules.JSUtilitiesModule utilitiesModule )
        {
            detectedMobileDevice = await utilitiesModule.IsMobileDevice();
        }
        else
        {
            string userAgent = await JSUtilitiesModule.GetUserAgent();
            detectedMobileDevice = MobileDeviceDetector.IsMobile( userAgent );
        }

        if ( mobileDevice != detectedMobileDevice )
        {
            mobileDevice = detectedMobileDevice;

            if ( UseNativeMobilePicker )
            {
                menuOpen = false;
                focusMenuOnOpen = false;
                await DisposeOutsidePointerSubscriptionAsync();
            }

            await InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Handles the localization changed event.
    /// </summary>
    private async void OnLocalizationChanged( object sender, EventArgs eventArgs )
    {
        inputText = Value is null ? inputText : FormatTime( selectedTime );

        await InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <inheritdoc/>
    protected override OnScreenKeyboardInputType OnScreenKeyboardInputType => OnScreenKeyboardInputType.Time | OnScreenKeyboardInputType.Pickers;

    /// <summary>
    /// Gets the text presented in the visible input.
    /// </summary>
    protected string InputText => inputText;

    /// <summary>
    /// Gets the input type rendered for the active picker mode.
    /// </summary>
    protected string InputType => UseNativeMobilePicker ? "time" : "text";

    /// <summary>
    /// Gets the value rendered by the visible input.
    /// </summary>
    protected string VisibleInputText => UseNativeMobilePicker
        ? ( Value is null ? null : FormatNativeTime( selectedTime, Seconds ) )
        : InputText;

    /// <summary>
    /// Gets the minimum value rendered by the input.
    /// </summary>
    protected string InputMin => UseNativeMobilePicker && Min.HasValue
        ? FormatNativeTime( Min.Value, includeSeconds: true )
        : null;

    /// <summary>
    /// Gets the maximum value rendered by the input.
    /// </summary>
    protected string InputMax => UseNativeMobilePicker && Max.HasValue
        ? FormatNativeTime( Max.Value, includeSeconds: true )
        : null;

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
    protected string InputAriaExpanded => UseNativeMobilePicker ? null : MenuVisible.ToString().ToLowerInvariant();

    /// <summary>
    /// Gets the ARIA control target used by the custom picker input.
    /// </summary>
    protected string InputAriaControls => UseNativeMobilePicker ? null : MenuId;

    /// <summary>
    /// Gets the format presented in the visible input.
    /// </summary>
    protected string EffectiveDisplayFormat => PickerDateTimeFormat.Normalize( DisplayFormat ?? ( Seconds ? "HH:mm:ss" : "HH:mm" ) );

    /// <summary>
    /// Gets the wrapper classes supplied by the active provider.
    /// </summary>
    protected string PickerContainerClassNames
    {
        get
        {
            return string.Join(
                " ",
                new[] { ProviderPickerContainerClassNames, Classes?.Wrapper }
                    .Where( value => !string.IsNullOrWhiteSpace( value ) ) );
        }
    }

    /// <summary>
    /// Gets the wrapper styles supplied through <see cref="TimePickerStyles"/>.
    /// </summary>
    protected string PickerContainerStyleNames => Styles?.Wrapper;

    /// <summary>
    /// Gets only the active provider's TimePicker container classes.
    /// </summary>
    protected string ProviderPickerContainerClassNames => ClassProvider.TimePickerContainer( Inline, MenuVisible );

    protected internal bool MenuVisible => !UseNativeMobilePicker && !Plaintext && ( Inline || menuOpen );

    /// <summary>
    /// Gets whether the browser's native mobile picker should be used.
    /// </summary>
    internal bool UseNativeMobilePicker => !DisableMobile
        && mobileDevice == true
        && !Plaintext
        && !Inline;

    internal bool FocusMenuOnOpen => focusMenuOnOpen;

    internal int MenuControlTabIndex => Inline || FocusMenuOnOpen ? 0 : -1;

    internal bool MenuInteractionDisabled => IsDisabled || ReadOnly || Plaintext;

    protected internal string MenuId => $"{ElementId}-menu";

    internal string PickerContainerId => $"{ElementId}-container";

    internal string MenuClassNames => ClassProvider.TimePickerMenu( Inline, StaticPicker );

    internal string MenuBackdropClassNames => ClassProvider.TimePickerBackdrop();

    internal string MenuControlsClassNames => ClassProvider.TimePickerControls();

    internal string MenuInputClassNames => ClassProvider.TimePickerInput();

    internal string MenuSeparatorClassNames => ClassProvider.TimePickerSeparator();

    internal string MenuMeridiemClassNames => ClassProvider.TimePickerMeridiem( IsPostMeridiem, focusedPart == TimePickerPart.Meridiem );

    internal string FocusedPartId => GetPartId( focusedPart );

    internal int SafeHourIncrement => Math.Max( 1, HourIncrement );

    internal int SafeMinuteIncrement => Math.Max( 1, MinuteIncrement );

    internal int CurrentHour => selectedTime.Hours;

    internal int CurrentMinute => selectedTime.Minutes;

    internal int CurrentSecond => selectedTime.Seconds;

    internal int DisplayHour => TimeAs24hr ? CurrentHour : CurrentHour % 12 == 0 ? 12 : CurrentHour % 12;

    internal bool IsPostMeridiem => CurrentHour >= 12;

    internal string MeridiemText => Localizer[IsPostMeridiem ? "PM" : "AM"];

    internal string MeridiemLabel => Localizer[IsPostMeridiem ? "PM" : "AM"];

    internal string TimeText => "Time";

    internal string HourText => "Hour";

    internal string MinuteText => "Minute";

    internal string SecondText => "Second";

    /// <summary>
    /// Gets or sets the legacy TimePicker JavaScript module.
    /// </summary>
    /// <remarks>
    /// Retained for source compatibility. The native TimePicker implementation does not use this module.
    /// </remarks>
    [Inject] public IJSTimePickerModule JSModule { get; set; }

    /// <summary>
    /// Specifies the DI registered <see cref="ITextLocalizerService"/>.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Specifies the DI registered <see cref="ITextLocalizer{T}"/>.
    /// </summary>
    [Inject] protected ITextLocalizer<TimePicker<TValue>> Localizer { get; set; }

    /// <summary>
    /// Gets or sets the document observer used to detect pointer interactions outside of the picker.
    /// </summary>
    [Inject] protected IDocumentObserver DocumentObserver { get; set; }

    /// <summary>
    /// The earliest time to accept.
    /// </summary>
    [Parameter] public TimeSpan? Min { get; set; }

    /// <summary>
    /// The latest time to accept.
    /// </summary>
    [Parameter] public TimeSpan? Max { get; set; }

    /// <summary>
    /// Specifies the display format of the time input using the picker format syntax supported by earlier versions.
    /// </summary>
    [Parameter] public string DisplayFormat { get; set; }

    /// <summary>
    /// Displays time picker in 24 hour mode without AM/PM selection when enabled.
    /// </summary>
    [Parameter] public bool TimeAs24hr { get; set; }

    /// <summary>
    /// Display the time menu in an always-open state with the inline option.
    /// </summary>
    [Parameter] public bool Inline { get; set; }

    /// <summary>
    /// Prevents the browser's native picker from being used on mobile devices.
    /// </summary>
    [Parameter] public bool DisableMobile { get; set; } = true;

    /// <summary>
    /// If enabled, the time menu will be positioned as static.
    /// </summary>
    [Parameter] public bool StaticPicker { get; set; } = true;

    /// <summary>
    /// If enabled, the time picker will include seconds in the selection.
    /// </summary>
    [Parameter] public bool Seconds { get; set; }

    /// <summary>
    /// Adjusts the step for the hour input.
    /// </summary>
    [Parameter] public int HourIncrement { get; set; } = 1;

    /// <summary>
    /// Adjusts the step for the minute input.
    /// </summary>
    [Parameter] public int MinuteIncrement { get; set; } = 1;

    /// <summary>
    /// Specifies the initial value of the hour element.
    /// </summary>
    [Parameter] public int DefaultHour { get; set; } = 12;

    /// <summary>
    /// Specifies the initial value of the minute element.
    /// </summary>
    [Parameter] public int DefaultMinute { get; set; }

    #endregion
}