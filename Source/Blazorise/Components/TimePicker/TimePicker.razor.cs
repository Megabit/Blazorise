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
/// An editor that displays a time value and allows a user to edit the value.
/// </summary>
/// <typeparam name="TValue">Data-type to be binded by the <see cref="TimePicker{TValue}"/> property.</typeparam>
public partial class TimePicker<TValue> : BaseTextInput<TValue, TimePickerClasses, TimePickerStyles>, IAsyncDisposable, ITimePicker
{
    #region Events

    internal event Action MenuStateChanged;

    #endregion

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

    private bool pointerInteraction;

    private bool? mobileDevice;

    private PickerObserverCoordinator observerCoordinator;

    private TimePickerMenuContext<TValue> menuContext;

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

        NotifyMenuStateChanged();
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
        builder.Append( ClassProvider.TimePicker( Plaintext ) );
        builder.Append( ClassProvider.TimePickerSize( ThemeSize ) );
        builder.Append( ClassProvider.TimePickerColor( Color ) );
        builder.Append( ClassProvider.TimePickerValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

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
            selectedTime = GetDefaultTime();
            NotifyMenuStateChanged();
            return;
        }

        if ( TryNormalizeInputValue( inputText, out string normalizedValue, out TimeSpan parsedTime ) )
        {
            selectedTime = ClampTime( parsedTime );
            normalizedValue = FormatInternalTime( selectedTime );

            await CurrentValueHandler( normalizedValue );

            if ( formatParsedValue )
            {
                inputText = FormatTime( selectedTime );
            }
        }
        else if ( ParentValidation is not null )
        {
            await ParentValidation.NotifyInputChanged<TValue>( default );
        }

        NotifyMenuStateChanged();
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
        if ( !OpenTrigger.HasFlag( PickerOpenTrigger.Click ) )
            return;

        if ( MenuInteractionDisabled )
            return;

        if ( UseNativeMobilePicker )
            return;

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
        if ( value is null )
            return null;

        if ( TimePickerTimeUtilities.TryGetTime( value, out TimeSpan time ) )
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
        else if ( OpenTrigger.HasFlag( PickerOpenTrigger.OpenKeys )
                  && eventArgs.Key is "ArrowDown" or "F4" )
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
    public new virtual async Task OnFocusHandler( FocusEventArgs eventArgs )
    {
        bool pointerInitiatedFocus = pointerInteraction;
        pointerInteraction = false;

        await OnFocus.InvokeAsync( eventArgs );

        if ( !pointerInitiatedFocus
             && OpenTrigger.HasFlag( PickerOpenTrigger.Focus )
             && !UseNativeMobilePicker )
        {
            await OpenMenuAsync( focusMenu: false );
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
    public new virtual Task OnBlurHandler( FocusEventArgs eventArgs )
    {
        return base.OnBlurHandler( eventArgs );
    }

    /// <inheritdoc/>
    protected override async Task OnScreenKeyboardValueChanged( string value )
    {
        await ProcessInputTextAsync( value, formatParsedValue: false );
        await InvokeAsync( StateHasChanged );
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

        NotifyMenuStateChanged();
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

        hour = Math.Clamp( hour, 0, 23 );

        if ( !TimeAs24hr && hour is >= 1 and <= 12 )
        {
            hour %= 12;

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
        if ( focusedPart == part )
            return;

        focusedPart = part;
        NotifyMenuStateChanged();
    }

    private async ValueTask CloseMenuAsync( bool focusInput )
    {
        if ( Inline )
            return;

        menuOpen = false;
        focusMenuOnOpen = false;

        NotifyMenuStateChanged();
        await DisposeOutsidePointerSubscriptionAsync();

        if ( focusInput )
        {
            ExecuteAfterRender( () => Focus() );
        }

        await InvokeAsync( StateHasChanged );
    }

    private Task HandleOutsidePointerAsync( DocumentEventArgs eventArgs )
    {
        return CloseMenuAsync( focusInput: false ).AsTask();
    }

    private ValueTask SynchronizeOutsidePointerSubscriptionAsync()
        => ObserverCoordinator.SynchronizeOutsideSubscriptionAsync(
            menuOpen,
            Inline,
            ElementId,
            PickerContainerId,
            MenuId,
            HandleOutsidePointerAsync );

    private ValueTask DisposeOutsidePointerSubscriptionAsync()
        => ObserverCoordinator.DisposeOutsideSubscriptionAsync();

    private ValueTask InitializeInputKeyDownSubscriptionAsync()
        => ObserverCoordinator.InitializeInputKeyDownAsync( ElementId, ElementId );

    private void SynchronizeStateFromValue()
    {
        if ( TimePickerTimeUtilities.TryGetTime( Value, out TimeSpan time ) )
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
        if ( TimePickerTimeUtilities.TryGetTime( Value, out TimeSpan time ) )
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
        NotifyMenuStateChanged();
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

        FocusPart( parts[nextIndex] );
    }

    private int SafeHourIncrement => Math.Max( 1, HourIncrement );

    private int SafeMinuteIncrement => Math.Max( 1, MinuteIncrement );

    private int CurrentHour => selectedTime.Hours;

    private int CurrentMinute => selectedTime.Minutes;

    private int CurrentSecond => selectedTime.Seconds;

    private bool IsPostMeridiem => CurrentHour >= 12;

    private bool TryNormalizeInputValue( string value, out string normalizedValue, out TimeSpan result )
        => TimePickerInputParser.TryNormalize(
            value,
            DisplayFormat,
            EffectiveDisplayFormat,
            Seconds,
            TimeAs24hr,
            IsPostMeridiem,
            Min,
            Max,
            out normalizedValue,
            out result );

    private TimeSpan GetDefaultTime()
        => TimePickerTimeUtilities.GetDefault( DefaultHour, DefaultMinute, Min, Max );

    private TimeSpan ClampTime( TimeSpan time )
        => TimePickerTimeUtilities.Clamp( time, Min, Max );

    private static TimeSpan NormalizeTime( TimeSpan time )
        => TimePickerTimeUtilities.Normalize( time );

    private string FormatTime( TimeSpan time )
        => TimePickerTimeUtilities.FormatDisplay( time, EffectiveDisplayFormat );

    private static string FormatInternalTime( TimeSpan time )
        => TimePickerTimeUtilities.FormatInternal( time );

    private static string FormatNativeTime( TimeSpan time, bool includeSeconds )
        => TimePickerTimeUtilities.FormatNative( time, includeSeconds );

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

            NotifyMenuStateChanged();
            await InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Handles the localization changed event.
    /// </summary>
    private async void OnLocalizationChanged( object sender, EventArgs eventArgs )
    {
        inputText = Value is null ? inputText : FormatTime( selectedTime );

        NotifyMenuStateChanged();
        await InvokeAsync( StateHasChanged );
    }

    private void NotifyMenuStateChanged()
        => MenuStateChanged?.Invoke();

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    private PickerObserverCoordinator ObserverCoordinator
        => observerCoordinator ??= new( DocumentObserver );

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
    /// Gets whether keyboard opening keys are enabled for the visible input.
    /// </summary>
    protected bool InputKeyboardNavigationEnabled => !UseNativeMobilePicker
        && !IsDisabled
        && !ReadOnly
        && !Plaintext
        && OpenTrigger.HasFlag( PickerOpenTrigger.OpenKeys );

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

    internal bool MenuInteractionDisabled => IsDisabled || ReadOnly || Plaintext;

    protected internal string MenuId => $"{ElementId}-menu";

    internal string PickerContainerId => $"{ElementId}-container";

    internal TimePickerMenuContext<TValue> MenuContext
        => menuContext ??= new( this );

    internal TimeSpan PickerSelectedTime => selectedTime;

    internal TimePickerPart PickerFocusedPart => focusedPart;

    internal IClassProvider PickerClassProvider => ClassProvider;

    internal ITextLocalizer PickerLocalizer => Localizer;

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
    /// Defines which interactions can open the time menu.
    /// </summary>
    [Parameter] public PickerOpenTrigger OpenTrigger { get; set; } = PickerOpenTrigger.All;

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