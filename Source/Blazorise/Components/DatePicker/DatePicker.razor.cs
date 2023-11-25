#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
public partial class DatePicker<TValue> : BaseTextInput<IReadOnlyList<TValue>>, IAsyncDisposable, IDatePicker
{
    #region Members

    private DotNetObjectReference<DatePickerAdapter> dotNetObjectRef;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            var dateChanged = parameters.TryGetValue<TValue>( nameof( Date ), out var paramDate ) && !Date.Equals( paramDate );
            var datesChanged = parameters.TryGetValue( nameof( Dates ), out IEnumerable<TValue> paramDates ) && !Dates.AreEqual( paramDates );
            var minChanged = parameters.TryGetValue( nameof( Min ), out DateTimeOffset? paramMin ) && !Min.IsEqual( paramMin );
            var maxChanged = parameters.TryGetValue( nameof( Max ), out DateTimeOffset? paramMax ) && !Max.IsEqual( paramMax );
            var firstDayOfWeekChanged = parameters.TryGetValue( nameof( FirstDayOfWeek ), out DayOfWeek paramFirstDayOfWeek ) && !FirstDayOfWeek.IsEqual( paramFirstDayOfWeek );
            var displayFormatChanged = parameters.TryGetValue( nameof( DisplayFormat ), out string paramDisplayFormat ) && DisplayFormat != paramDisplayFormat;
            var inputFormatChanged = parameters.TryGetValue( nameof( InputFormat ), out string paramInputFormat ) && InputFormat != paramInputFormat;
            var timeAs24hrChanged = parameters.TryGetValue( nameof( TimeAs24hr ), out bool paramTimeAs24hr ) && TimeAs24hr != paramTimeAs24hr;
            var disabledChanged = parameters.TryGetValue( nameof( Disabled ), out bool paramDisabled ) && Disabled != paramDisabled;
            var readOnlyChanged = parameters.TryGetValue( nameof( ReadOnly ), out bool paramReadOnly ) && ReadOnly != paramReadOnly;
            var disabledDatesChanged = parameters.TryGetValue( nameof( DisabledDates ), out IEnumerable<TValue> paramDisabledDates ) && !DisabledDates.AreEqual( paramDisabledDates );
            var selectionModeChanged = parameters.TryGetValue( nameof( SelectionMode ), out DateInputSelectionMode paramSelectionMode ) && !SelectionMode.IsEqual( paramSelectionMode );
            var inlineChanged = parameters.TryGetValue( nameof( Inline ), out bool paramInline ) && Inline != paramInline;
            var disableMobileChanged = parameters.TryGetValue( nameof( DisableMobile ), out bool paramDisableMobile ) && DisableMobile != paramDisableMobile;
            var placeholderChanged = parameters.TryGetValue( nameof( Placeholder ), out string paramPlaceholder ) && Placeholder != paramPlaceholder;
            var staticPickerChanged = parameters.TryGetValue( nameof( StaticPicker ), out bool paramSaticPicker ) && StaticPicker != paramSaticPicker;

            if ( dateChanged || datesChanged )
            {
                var formatedDateString = SelectionMode != DateInputSelectionMode.Single
                    ? FormatValueAsString( paramDates?.ToArray() )
                    : FormatValueAsString( new TValue[] { paramDate } );

                await CurrentValueHandler( formatedDateString );

                if ( Rendered )
                {
                    ExecuteAfterRender( async () => await JSModule.UpdateValue( ElementRef, ElementId, formatedDateString ) );
                }
            }

            if ( minChanged
                 || maxChanged
                 || firstDayOfWeekChanged
                 || displayFormatChanged
                 || inputFormatChanged
                 || timeAs24hrChanged
                 || disabledChanged
                 || readOnlyChanged
                 || disabledDatesChanged
                 || selectionModeChanged
                 || inlineChanged
                 || disableMobileChanged
                 || placeholderChanged
                 || staticPickerChanged )
            {
                ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, new
                {
                    FirstDayOfWeek = new { Changed = firstDayOfWeekChanged, Value = (int)paramFirstDayOfWeek },
                    DisplayFormat = new { Changed = displayFormatChanged, Value = DisplayFormatConverter.Convert( paramDisplayFormat ) },
                    InputFormat = new { Changed = inputFormatChanged, Value = InputFormatConverter.Convert( paramInputFormat ) },
                    TimeAs24hr = new { Changed = timeAs24hrChanged, Value = paramTimeAs24hr },
                    Min = new { Changed = minChanged, Value = paramMin?.ToString( DateFormat ) },
                    Max = new { Changed = maxChanged, Value = paramMax?.ToString( DateFormat ) },
                    Disabled = new { Changed = disabledChanged, Value = paramDisabled },
                    ReadOnly = new { Changed = readOnlyChanged, Value = paramReadOnly },
                    DisabledDates = new { Changed = disabledDatesChanged, Value = paramDisabledDates?.Select( x => FormatValueAsString( new TValue[] { x } ) ) },
                    SelectionMode = new { Changed = selectionModeChanged, Value = paramSelectionMode },
                    Inline = new { Changed = inlineChanged, Value = paramInline },
                    DisableMobile = new { Changed = disableMobileChanged, Value = paramDisableMobile },
                    Placeholder = new { Changed = placeholderChanged, Value = paramPlaceholder },
                    StaticPicker = new { Changed = staticPickerChanged, Value = paramSaticPicker },
                } ) );
            }
        }

        // Let blazor do its thing!
        await base.SetParametersAsync( parameters );

        if ( ParentValidation != null )
        {
            if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( DateExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            if ( parameters.TryGetValue<string>( nameof( Pattern ), out var pattern ) )
            {
                // make sure we get the newest value
                var value = parameters.TryGetValue<TValue>( nameof( Date ), out var inDate )
                    ? new TValue[] { inDate }
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
        dotNetObjectRef ??= CreateDotNetObjectRef( new DatePickerAdapter( this ) );
        object defaultDate = null;

        // for multiple mode default dates must be set as array
        if ( SelectionMode != DateInputSelectionMode.Single )
            defaultDate = Dates?.Select( x => FormatValueAsString( new TValue[] { x } ) )?.ToArray();
        else
            defaultDate = FormatValueAsString( new TValue[] { Date } );

        await JSModule.Initialize( dotNetObjectRef, ElementRef, ElementId, new
        {
            InputMode,
            SelectionMode = SelectionMode.ToDateInputSelectionMode(),
            FirstDayOfWeek = (int)FirstDayOfWeek,
            DisplayFormat = DisplayFormatConverter.Convert( DisplayFormat ),
            InputFormat = InputFormatConverter.Convert( InputFormat ),
            TimeAs24hr,
            DefaultDate = defaultDate,
            Min = Min?.ToString( DateFormat ),
            Max = Max?.ToString( DateFormat ),
            Disabled,
            ReadOnly,
            DisabledDates = DisabledDates?.Select( x => FormatValueAsString( new TValue[] { x } ) ),
            Localization = GetLocalizationObject(),
            Inline,
            DisableMobile,
            Placeholder,
            StaticPicker,
            ValidationStatus = new
            {
                SuccessClass = ClassProvider.DatePickerValidation( ValidationStatus.Success ),
                ErrorClass = ClassProvider.DatePickerValidation( ValidationStatus.Error ),
            }
        } );

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

            DisposeDotNetObjectRef( dotNetObjectRef );
            dotNetObjectRef = null;

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
        builder.Append( ClassProvider.DatePickerValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override Task OnChangeHandler( ChangeEventArgs e )
    {
        return CurrentValueHandler( e?.Value?.ToString() );
    }

    /// <inheritdoc/>
    [JSInvokable]
    protected async Task OnClickHandler( MouseEventArgs e )
    {
        if ( Disabled || ReadOnly )
            return;

        await JSModule.Activate( ElementRef, ElementId, DateFormat );
    }

    /// <inheritdoc/>
    protected override Task OnInternalValueChanged( IReadOnlyList<TValue> value )
    {
        if ( SelectionMode != DateInputSelectionMode.Single )
            return DatesChanged.InvokeAsync( value );
        else
            return DateChanged.InvokeAsync( value == null ? default : value.FirstOrDefault() );
    }

    /// <inheritdoc/>
    protected override string FormatValueAsString( IReadOnlyList<TValue> values )
    {
        if ( values == null || values.Count == 0 )
            return null;

        if ( SelectionMode != DateInputSelectionMode.Single )
        {
            var results = new List<string>();

            foreach ( var value in values )
            {
                results.Add( Formaters.FormatDateValueAsString( value, DateFormat ) );
            }

            return string.Join( SelectionMode == DateInputSelectionMode.Multiple ? ", " : CurrentRangeSeparator, results );
        }
        else
        {
            if ( values[0] == null )
                return null;

            return Formaters.FormatDateValueAsString( values[0], DateFormat );
        }
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<IReadOnlyList<TValue>>> ParseValueFromStringAsync( string value )
    {
        if ( SelectionMode != DateInputSelectionMode.Single )
        {
            var values = value?.Split( SelectionMode == DateInputSelectionMode.Multiple ? ", " : CurrentRangeSeparator );

            if ( values?.Length > 0 )
            {
                var result = new List<TValue>();

                foreach ( var part in values )
                {
                    if ( Parsers.TryParseDate<TValue>( part, InputMode, out var resultValue ) )
                    {
                        result.Add( resultValue );
                    }
                }

                return Task.FromResult( new ParseValue<IReadOnlyList<TValue>>( true, result.ToArray(), null ) );
            }

            return Task.FromResult( new ParseValue<IReadOnlyList<TValue>>( false, new TValue[] { default, default }, null ) );
        }
        else
        {
            if ( Parsers.TryParseDate<TValue>( value, InputMode, out var result ) )
            {
                return Task.FromResult( new ParseValue<IReadOnlyList<TValue>>( true, new TValue[] { result }, null ) );
            }
            else
            {
                return Task.FromResult( new ParseValue<IReadOnlyList<TValue>>( false, new TValue[] { default }, null ) );
            }
        }
    }
    /// <inheritdoc/>
    [JSInvokable]
    public new virtual Task OnKeyDownHandler( KeyboardEventArgs eventArgs )
    {
        return KeyDown.InvokeAsync( eventArgs );
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
    public new virtual Task OnFocusInHandler( FocusEventArgs eventArgs )
    {
        return FocusIn.InvokeAsync( eventArgs );
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
        // just call eventcallback without using debouncer in BaseTextInput
        return KeyPress.InvokeAsync( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public new virtual Task OnBlurHandler( FocusEventArgs eventArgs )
    {
        // just call eventcallback without using debouncer in BaseTextInput
        return Blur.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Opens the calendar dropdown.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask OpenAsync()
    {
        return JSModule.Open( ElementRef, ElementId );
    }

    /// <summary>
    /// Closes the calendar dropdown.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask CloseAsync()
    {
        return JSModule.Close( ElementRef, ElementId );
    }

    /// <summary>
    /// Shows/opens the calendar if its closed, hides/closes it otherwise.
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
        return new
        {
            FirstDayOfWeek = (int)FirstDayOfWeek,
            Weekdays = new
            {
                Shorthand = new[]
                {
                    Localizer["Sun"],
                    Localizer["Mon"],
                    Localizer["Tue"],
                    Localizer["Wed"],
                    Localizer["Thu"],
                    Localizer["Fri"],
                    Localizer["Sat"]
                },
                Longhand = new[]
                {
                    Localizer["Sunday"],
                    Localizer["Monday"],
                    Localizer["Tuesday"],
                    Localizer["Wednesday"],
                    Localizer["Thursday"],
                    Localizer["Friday"],
                    Localizer["Saturday"]
                },
            },
            Months = new
            {
                Shorthand = new[]
                {
                    Localizer["Jan"],
                    Localizer["Feb"],
                    Localizer["Mar"],
                    Localizer["Apr"],
                    Localizer["May"],
                    Localizer["Jun"],
                    Localizer["Jul"],
                    Localizer["Aug"],
                    Localizer["Sep"],
                    Localizer["Oct"],
                    Localizer["Nov"],
                    Localizer["Dec"]
                },
                Longhand = new[]
                {
                    Localizer["January"],
                    Localizer["February"],
                    Localizer["March"],
                    Localizer["April"],
                    Localizer["May"],
                    Localizer["June"],
                    Localizer["July"],
                    Localizer["August"],
                    Localizer["September"],
                    Localizer["October"],
                    Localizer["November"],
                    Localizer["December"]
                }
            },
            amPM = new[] { Localizer["AM"], Localizer["PM"] },
            RangeSeparator = CurrentRangeSeparator,
        };
    }

    /// <inheritdoc/>
    protected override bool IsSameAsInternalValue( IReadOnlyList<TValue> value ) => value.AreEqual( InternalValue );

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <inheritdoc/>
    protected override IReadOnlyList<TValue> InternalValue
    {
        get => SelectionMode != DateInputSelectionMode.Single ? Dates : new TValue[] { Date };
        set
        {
            if ( SelectionMode != DateInputSelectionMode.Single )
            {
                Dates = value;
            }
            else
            {
                Date = value == null ? default : value.FirstOrDefault();
            }
        }
    }

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
    /// Gets or sets the <see cref="IJSDatePickerModule"/> instance.
    /// </summary>
    [Inject] public IJSDatePickerModule JSModule { get; set; }

    /// <summary>
    /// Gets or sets the DI registered <see cref="ITextLocalizerService"/>.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Gets or sets the DI registered <see cref="ITextLocalizer{T}"/>.
    /// </summary>
    [Inject] protected ITextLocalizer<DatePicker<TValue>> Localizer { get; set; }

    /// <summary>
    /// Converts the supplied date format into the internal date format used by the <see cref="DisplayFormat"/> mask.
    /// </summary>
    [Inject] protected IFlatPickrDateTimeDisplayFormatConverter DisplayFormatConverter { get; set; }

    /// <summary>
    /// Converts the supplied date format into the internal date format used by the <see cref="InputFormat"/> mask.
    /// </summary>
    [Inject] protected IInputMaskDateTimeInputFormatConverter InputFormatConverter { get; set; }

    /// <summary>
    /// Hints at the type of data that might be entered by the user while editing the element or its contents.
    /// </summary>
    [Parameter] public DateInputMode InputMode { get; set; } = DateInputMode.Date;

    /// <summary>
    /// Defines the mode in which the dates can be selected.
    /// </summary>
    [Parameter] public DateInputSelectionMode SelectionMode { get; set; } = DateInputSelectionMode.Single;

    /// <summary>
    /// Overrides the range separator that is used to separate date values when <see cref="SelectionMode"/> is set to <see cref="DateInputSelectionMode.Range"/>.
    /// </summary>
    [Parameter] public string RangeSeparator { get; set; }

    /// <summary>
    /// Gets or sets the input date value.
    /// </summary>
    [Parameter] public TValue Date { get; set; }

    /// <summary>
    /// Occurs when the date has changed.
    /// </summary>
    [Parameter] public EventCallback<TValue> DateChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the date value.
    /// </summary>
    [Parameter] public Expression<Func<TValue>> DateExpression { get; set; }

    /// <summary>
    /// Gets or sets the input date value.
    /// </summary>
    [Parameter] public IReadOnlyList<TValue> Dates { get; set; }

    /// <summary>
    /// Occurs when the date has changed.
    /// </summary>
    [Parameter] public EventCallback<IReadOnlyList<TValue>> DatesChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the date value.
    /// </summary>
    [Parameter] public Expression<Func<IReadOnlyList<TValue>>> DatesExpression { get; set; }

    /// <summary>
    /// The earliest date to accept.
    /// </summary>
    [Parameter] public DateTimeOffset? Min { get; set; }

    /// <summary>
    /// The latest date to accept.
    /// </summary>
    [Parameter] public DateTimeOffset? Max { get; set; }

    /// <summary>
    /// Defines the first day of the week.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Monday;

    /// <summary>
    /// Defines the display format of the date input.
    /// </summary>
    [Parameter] public string DisplayFormat { get; set; }

    /// <summary>
    /// Defines the input format mask of the date input.
    /// </summary>
    [Parameter] public string InputFormat { get; set; }

    /// <summary>
    /// Displays time picker in 24 hour mode without AM/PM selection when enabled.
    /// </summary>
    [Parameter] public bool TimeAs24hr { get; set; }

    /// <summary>
    /// List of disabled dates that the user should not be able to pick.
    /// </summary>
    [Parameter] public IEnumerable<TValue> DisabledDates { get; set; }

    /// <summary>
    /// Display the calendar in an always-open state with the inline option.
    /// </summary>
    [Parameter] public bool Inline { get; set; }

    /// <summary>
    /// If enabled, it disables the native input on mobile devices.
    /// </summary>
    [Parameter] public bool DisableMobile { get; set; } = true;

    /// <summary>
    /// If enabled, the calendar menu will be positioned as static.
    /// </summary>
    [Parameter] public bool StaticPicker { get; set; } = true;

    #endregion
}