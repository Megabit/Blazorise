#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Localization;
using Blazorise.Modules;
using Blazorise.Utilities;
using Blazorise.Vendors;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// An editor that displays a time value and allows a user to edit the value.
/// </summary>
/// <typeparam name="TValue">Data-type to be binded by the <see cref="TimePicker{TValue}"/> property.</typeparam>
public partial class TimePicker<TValue> : BaseTextInput<TValue, TimePickerClasses, TimePickerStyles>, IAsyncDisposable
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
    protected override async Task OnBeforeSetParametersAsync( ParameterView parameters )
    {
        await base.OnBeforeSetParametersAsync( parameters );

        var minChanged = paramMin.Defined && paramMin.Changed;
        var maxChanged = paramMax.Defined && paramMax.Changed;
        var displayFormatChanged = paramDisplayFormat.Defined && paramDisplayFormat.Changed;
        var timeAs24hrChanged = paramTimeAs24hr.Defined && paramTimeAs24hr.Changed;
        var disabledChanged = paramDisabled.Defined && paramDisabled.Changed;
        var readOnlyChanged = paramReadOnly.Defined && paramReadOnly.Changed;
        var inlineChanged = paramInline.Defined && paramInline.Changed;
        var disableMobileChanged = paramDisableMobile.Defined && paramDisableMobile.Changed;
        var placeholderChanged = paramPlaceholder.Defined && paramPlaceholder.Changed;
        var staticPickerChanged = paramStaticPicker.Defined && paramStaticPicker.Changed;
        var secondsChanged = paramSeconds.Defined && paramSeconds.Changed;
        var hourIncrementChanged = paramHourIncrement.Defined && paramHourIncrement.Changed;
        var minuteIncrementChanged = paramMinuteIncrement.Defined && paramMinuteIncrement.Changed;
        var defaultHourChanged = paramDefaultHour.Defined && paramDefaultHour.Changed;
        var defaultMinuteChanged = paramDefaultMinute.Defined && paramDefaultMinute.Changed;

        if ( paramValue.Changed )
        {
            var timeString = FormatValueAsString( paramValue.Value );

            await CurrentValueHandler( timeString );

            if ( Rendered )
            {
                ExecuteAfterRender( async () => await JSModule.UpdateValue( ElementRef, ElementId, timeString ) );
            }
        }

        if ( Rendered && ( minChanged
                           || maxChanged
                           || displayFormatChanged
                           || timeAs24hrChanged
                           || disabledChanged
                           || readOnlyChanged
                           || inlineChanged
                           || disableMobileChanged
                           || placeholderChanged
                           || staticPickerChanged
                           || secondsChanged
                           || hourIncrementChanged
                           || minuteIncrementChanged
                           || defaultHourChanged
                           || defaultMinuteChanged ) )
        {
            ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, new TimePickerUpdateJSOptions
            {
                DisplayFormat = new JSOptionChange<string>( displayFormatChanged, DisplayFormatConverter.Convert( paramDisplayFormat.Value ) ),
                TimeAs24hr = new JSOptionChange<bool>( timeAs24hrChanged, paramTimeAs24hr.Value ),
                Min = new JSOptionChange<string>( minChanged, paramMin.Value?.ToString( Parsers.InternalTimeFormat.ToLowerInvariant() ) ),
                Max = new JSOptionChange<string>( maxChanged, paramMax.Value?.ToString( Parsers.InternalTimeFormat.ToLowerInvariant() ) ),
                Disabled = new JSOptionChange<bool>( disabledChanged, paramDisabled.Value ),
                ReadOnly = new JSOptionChange<bool>( readOnlyChanged, paramReadOnly.Value ),
                Inline = new JSOptionChange<bool>( inlineChanged, paramInline.Value ),
                DisableMobile = new JSOptionChange<bool>( disableMobileChanged, paramDisableMobile.Value ),
                Placeholder = new JSOptionChange<string>( placeholderChanged, paramPlaceholder.Value ),
                StaticPicker = new JSOptionChange<bool>( staticPickerChanged, paramStaticPicker.Value ),
                Seconds = new JSOptionChange<bool>( secondsChanged, paramSeconds.Value ),
                HourIncrement = new JSOptionChange<int>( hourIncrementChanged, paramHourIncrement.Value ),
                MinuteIncrement = new JSOptionChange<int>( minuteIncrementChanged, paramMinuteIncrement.Value ),
                DefaultHour = new JSOptionChange<int>( defaultHourChanged, paramDefaultHour.Value ),
                DefaultMinute = new JSOptionChange<int>( defaultMinuteChanged, paramDefaultMinute.Value ),
            } ) );
        }
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
        await JSModule.Initialize( ElementRef, ElementId, new()
        {
            DisplayFormat = DisplayFormatConverter.Convert( DisplayFormat ),
            TimeAs24hr = TimeAs24hr,
            Default = FormatValueAsString( Value ),
            DefaultHour = DefaultHour,
            DefaultMinute = DefaultMinute,
            Min = Min?.ToString( Parsers.InternalTimeFormat.ToLowerInvariant() ),
            Max = Max?.ToString( Parsers.InternalTimeFormat.ToLowerInvariant() ),
            Disabled = Disabled,
            ReadOnly = ReadOnly,
            Localization = GetLocalizationObject(),
            Inline = Inline,
            DisableMobile = DisableMobile,
            Placeholder = Placeholder,
            StaticPicker = StaticPicker,
            Seconds = Seconds,
            HourIncrement = HourIncrement,
            MinuteIncrement = MinuteIncrement,
        } );


        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

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
    protected override Task OnChangeHandler( ChangeEventArgs e )
    {
        return CurrentValueHandler( e?.Value?.ToString() );
    }

    /// <summary>
    /// Handles the element onclick event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task OnClickHandler( MouseEventArgs e )
    {
        if ( Disabled || ReadOnly )
            return;

        await JSModule.Activate( ElementRef, ElementId, Parsers.InternalTimeFormat );
    }

    /// <inheritdoc/>
    protected override string FormatValueAsString( TValue value )
    {
        return value switch
        {
            null => null,
            TimeSpan timeSpan => timeSpan.ToString( Parsers.InternalTimeFormat.ToLowerInvariant() ),
            TimeOnly timeOnly => timeOnly.ToString( Parsers.InternalTimeFormat ),
            DateTime datetime => datetime.ToString( Parsers.InternalTimeFormat ),
            _ => throw new InvalidOperationException( $"Unsupported type {value.GetType()}" ),
        };
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
    {
        if ( Parsers.TryParseTime<TValue>( value, out var result ) )
        {
            return Task.FromResult( new ParseValue<TValue>( true, result, null ) );
        }
        else
        {
            return Task.FromResult( new ParseValue<TValue>( false, default, null ) );
        }
    }

    /// <inheritdoc/>
    protected override Task OnKeyPressHandler( KeyboardEventArgs eventArgs )
    {
        // just call eventcallback without using debouncer in BaseTextInput
        return KeyPress.InvokeAsync( eventArgs );
    }

    /// <inheritdoc/>
    protected override Task OnBlurHandler( FocusEventArgs eventArgs )
    {
        // just call eventcallback without using debouncer in BaseTextInput
        return Blur.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Opens the time dropdown.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask OpenAsync()
    {
        return JSModule.Open( ElementRef, ElementId );
    }

    /// <summary>
    /// Closes the time dropdown.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask CloseAsync()
    {
        return JSModule.Close( ElementRef, ElementId );
    }

    /// <summary>
    /// Shows/opens the time dropdown if its closed, hides/closes it otherwise.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask ToggleAsync()
    {
        return JSModule.Toggle( ElementRef, ElementId );
    }

    /// <inheritdoc/>
    public override async Task Focus( bool scrollToElement = true )
    {
        await JSModule.Focus( ElementRef, ElementId, scrollToElement );
    }

    /// <inheritdoc/>
    public override async Task Select( bool focus = true )
    {
        await JSModule.Select( ElementRef, ElementId, focus );
    }

    /// <summary>
    /// Handles the localization changed event.
    /// </summary>
    /// <param name="sender">Object that raised the event.</param>
    /// <param name="eventArgs">Data about the localization event.</param>
    private async void OnLocalizationChanged( object sender, EventArgs eventArgs )
    {
        ExecuteAfterRender( async () => await JSModule.UpdateLocalization( ElementRef, ElementId, GetLocalizationObject() ) );

        await InvokeAsync( StateHasChanged );
    }

    private object GetLocalizationObject()
    {
        var strings = Localizer.GetStrings();

        return new
        {
            amPM = new[] { Localizer["AM"], Localizer["PM"] }
        };
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets or sets the <see cref="IJSTimePickerModule"/> instance.
    /// </summary>
    [Inject] public IJSTimePickerModule JSModule { get; set; }

    /// <summary>
    /// Gets or sets the DI registered <see cref="ITextLocalizerService"/>.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Gets or sets the DI registered <see cref="ITextLocalizer{T}"/>.
    /// </summary>
    [Inject] protected ITextLocalizer<TimePicker<TValue>> Localizer { get; set; }

    /// <summary>
    /// Converts the supplied time format into the internal time format.
    /// </summary>
    [Inject] protected IFlatPickrDateTimeDisplayFormatConverter DisplayFormatConverter { get; set; }

    /// <summary>
    /// The earliest time to accept.
    /// </summary>
    [Parameter] public TimeSpan? Min { get; set; }

    /// <summary>
    /// The latest time to accept.
    /// </summary>
    [Parameter] public TimeSpan? Max { get; set; }

    /// <summary>
    /// Defines the display format of the time input.
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
    /// If enabled, it always uses the non-native picker. Default is true.
    /// </summary>
    [Parameter] public bool DisableMobile { get; set; } = true;

    /// <summary>
    /// If enabled, the calendar menu will be positioned as static.
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
    /// Defines the initial value of the hour element.
    /// </summary>
    [Parameter] public int DefaultHour { get; set; } = 12;

    /// <summary>
    /// Defines the initial value of the minute element.
    /// </summary>
    [Parameter] public int DefaultMinute { get; set; } = 0;

    #endregion
}