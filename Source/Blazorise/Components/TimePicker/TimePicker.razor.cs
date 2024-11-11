#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Localization;
using Blazorise.Modules;
using Blazorise.Utilities;
using Blazorise.Vendors;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// An editor that displays a time value and allows a user to edit the value.
/// </summary>
/// <typeparam name="TValue">Data-type to be binded by the <see cref="TimePicker{TValue}"/> property.</typeparam>
public partial class TimePicker<TValue> : BaseTextInput<TValue>, IAsyncDisposable
{
    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var timeChanged = parameters.TryGetValue( nameof( Time ), out TValue time ) && !Time.IsEqual( time );
        var minChanged = parameters.TryGetValue( nameof( Min ), out TimeSpan? min ) && !Min.IsEqual( min );
        var maxChanged = parameters.TryGetValue( nameof( Max ), out TimeSpan? max ) && !Max.IsEqual( max );
        var displayFormatChanged = parameters.TryGetValue( nameof( DisplayFormat ), out string displayFormat ) && DisplayFormat != displayFormat;
        var timeAs24hrChanged = parameters.TryGetValue( nameof( TimeAs24hr ), out bool timeAs24hr ) && TimeAs24hr != timeAs24hr;
        var disabledChanged = parameters.TryGetValue( nameof( Disabled ), out bool disabled ) && Disabled != disabled;
        var readOnlyChanged = parameters.TryGetValue( nameof( ReadOnly ), out bool readOnly ) && ReadOnly != readOnly;
        var inlineChanged = parameters.TryGetValue( nameof( Inline ), out bool paramInline ) && Inline != paramInline;
        var placeholderChanged = parameters.TryGetValue( nameof( Placeholder ), out string paramPlaceholder ) && Placeholder != paramPlaceholder;
        var staticPickerChanged = parameters.TryGetValue( nameof( StaticPicker ), out bool paramSaticPicker ) && StaticPicker != paramSaticPicker;

        if ( timeChanged )
        {
            var timeString = FormatValueAsString( time );

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
                           || placeholderChanged
                           || staticPickerChanged ) )
        {
            ExecuteAfterRender(async () => await JSModule.UpdateOptions(ElementRef, ElementId, new TimePickerUpdateJSOptions
            {
                DisplayFormat = new JSOptionChange<string>(displayFormatChanged, DisplayFormatConverter.Convert(displayFormat)),
                TimeAs24hr = new JSOptionChange<bool>(timeAs24hrChanged, timeAs24hr),
                Min = new JSOptionChange<string>(minChanged, min?.ToString(Parsers.InternalTimeFormat.ToLowerInvariant())),
                Max = new JSOptionChange<string>(maxChanged, max?.ToString(Parsers.InternalTimeFormat.ToLowerInvariant())),
                Disabled = new JSOptionChange<bool>(disabledChanged, disabled),
                ReadOnly = new JSOptionChange<bool>(readOnlyChanged, readOnly),
                Inline = new JSOptionChange<bool>(inlineChanged, paramInline),
                Placeholder = new JSOptionChange<string>(placeholderChanged, paramPlaceholder),
                StaticPicker = new JSOptionChange<bool>(staticPickerChanged, paramSaticPicker),
            }));
        }

        // Let blazor do its thing!
        await base.SetParametersAsync( parameters );

        if ( ParentValidation is not null )
        {
            if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( TimeExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            if ( parameters.TryGetValue<string>( nameof( Pattern ), out var pattern ) )
            {
                // make sure we get the newest value
                var value = parameters.TryGetValue<TValue>( nameof( Time ), out var inTime )
                    ? inTime
                    : InternalValue;

                await ParentValidation.InitializeInputPattern( pattern, value );
            }

            await InitializeValidation();
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
        await JSModule.Initialize(ElementRef, ElementId, new TimePickerInitializeJSOptions
        {
            DisplayFormat = DisplayFormatConverter.Convert(DisplayFormat),
            TimeAs24hr = TimeAs24hr,
            Default = FormatValueAsString(Time),
            Min = Min?.ToString(Parsers.InternalTimeFormat.ToLowerInvariant()),
            Max = Max?.ToString(Parsers.InternalTimeFormat.ToLowerInvariant()),
            Disabled = Disabled,
            ReadOnly = ReadOnly,
            Localization = GetLocalizationObject(),
            Inline = Inline,
            Placeholder = Placeholder,
            StaticPicker = StaticPicker,
        });


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
    protected override Task OnInternalValueChanged( TValue value )
    {
        return TimeChanged.InvokeAsync( value );
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

    /// <inheritdoc/>
    protected override string GetFormatedValueExpression()
    {
        if ( TimeExpression is null )
            return null;

        return HtmlFieldPrefix is not null
            ? HtmlFieldPrefix.GetFieldName( TimeExpression )
            : ExpressionFormatter.FormatLambda( TimeExpression );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <inheritdoc/>
    protected override TValue InternalValue { get => Time; set => Time = value; }

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
    /// Gets or sets the input time value.
    /// </summary>
    [Parameter] public TValue Time { get; set; }

    /// <summary>
    /// Occurs when the time has changed.
    /// </summary>
    [Parameter] public EventCallback<TValue> TimeChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the time field.
    /// </summary>
    [Parameter] public Expression<Func<TValue>> TimeExpression { get; set; }

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
    /// If enabled, the calendar menu will be positioned as static.
    /// </summary>
    [Parameter] public bool StaticPicker { get; set; } = true;

    #endregion
}