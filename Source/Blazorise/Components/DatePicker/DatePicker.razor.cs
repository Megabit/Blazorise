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
        var datesUsed = parameters.TryGetValue( nameof( Dates ), out IReadOnlyList<TValue> paramDates );

        if ( Rendered )
        {
            var dateUsed = parameters.TryGetValue<TValue>( nameof( Date ), out var paramDate );

            var dateChanged = dateUsed && !Date.Equals( paramDate );
            var datesChanged = datesUsed && !Dates.AreEqual( paramDates );
            var minChanged = parameters.TryGetValue( nameof( Min ), out DateTimeOffset? paramMin ) && !Min.IsEqual( paramMin );
            var maxChanged = parameters.TryGetValue( nameof( Max ), out DateTimeOffset? paramMax ) && !Max.IsEqual( paramMax );
            var firstDayOfWeekChanged = parameters.TryGetValue( nameof( FirstDayOfWeek ), out DayOfWeek paramFirstDayOfWeek ) && !FirstDayOfWeek.IsEqual( paramFirstDayOfWeek );
            var displayFormatChanged = parameters.TryGetValue( nameof( DisplayFormat ), out string paramDisplayFormat ) && DisplayFormat != paramDisplayFormat;
            var inputFormatChanged = parameters.TryGetValue( nameof( InputFormat ), out string paramInputFormat ) && InputFormat != paramInputFormat;
            var timeAs24hrChanged = parameters.TryGetValue( nameof( TimeAs24hr ), out bool paramTimeAs24hr ) && TimeAs24hr != paramTimeAs24hr;
            var disabledChanged = parameters.TryGetValue( nameof( Disabled ), out bool paramDisabled ) && Disabled != paramDisabled;
            var readOnlyChanged = parameters.TryGetValue( nameof( ReadOnly ), out bool paramReadOnly ) && ReadOnly != paramReadOnly;
            var disabledDatesChanged = parameters.TryGetValue( nameof( DisabledDates ), out IEnumerable<TValue> paramDisabledDates ) && !DisabledDates.AreEqual( paramDisabledDates );
            var enabledDatesChanged = parameters.TryGetValue( nameof( EnabledDates ), out IEnumerable<TValue>? paramEnabledDates ) && !EnabledDates.AreEqual( paramEnabledDates );
            var disabledDaysChanged = parameters.TryGetValue( nameof( DisabledDays ), out IEnumerable<DayOfWeek> paramDisabledDays ) && !DisabledDays.AreEqual( paramDisabledDays );
            var selectionModeChanged = parameters.TryGetValue( nameof( SelectionMode ), out DateInputSelectionMode paramSelectionMode ) && !SelectionMode.IsEqual( paramSelectionMode );
            var inlineChanged = parameters.TryGetValue( nameof( Inline ), out bool paramInline ) && Inline != paramInline;
            var disableMobileChanged = parameters.TryGetValue( nameof( DisableMobile ), out bool paramDisableMobile ) && DisableMobile != paramDisableMobile;
            var placeholderChanged = parameters.TryGetValue( nameof( Placeholder ), out string paramPlaceholder ) && Placeholder != paramPlaceholder;
            var staticPickerChanged = parameters.TryGetValue( nameof( StaticPicker ), out bool paramStaticPicker ) && StaticPicker != paramStaticPicker;
            var showWeekNumbersChanged = parameters.TryGetValue( nameof( ShowWeekNumbers ), out bool paramShowWeekNumbers ) && ShowWeekNumbers != paramShowWeekNumbers;
            var showTodayButtonChanged = parameters.TryGetValue( nameof( ShowTodayButton ), out bool paramShowTodayButton ) && ShowTodayButton != paramShowTodayButton;
            var showClearButtonChanged = parameters.TryGetValue( nameof( ShowClearButton ), out bool paramShowClearButton ) && ShowClearButton != paramShowClearButton;
            var defaultHourChanged = parameters.TryGetValue( nameof( DefaultHour ), out int paramDefaultHour ) && DefaultHour != paramDefaultHour;
            var defaultMinuteChanged = parameters.TryGetValue( nameof( DefaultMinute ), out int paramDefaultMinute ) && DefaultMinute != paramDefaultMinute;

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
                 || enabledDatesChanged
                 || disabledDaysChanged
                 || selectionModeChanged
                 || inlineChanged
                 || disableMobileChanged
                 || placeholderChanged
                 || staticPickerChanged
                 || showWeekNumbersChanged
                 || showTodayButtonChanged
                 || showClearButtonChanged
                 || defaultHourChanged
                 || defaultMinuteChanged )
            {
                ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, new DatePickerUpdateJSOptions()
                {
                    FirstDayOfWeek = new JSOptionChange<int>( firstDayOfWeekChanged, (int)paramFirstDayOfWeek ),
                    DisplayFormat = new JSOptionChange<string>( displayFormatChanged, DisplayFormatConverter.Convert( paramDisplayFormat ) ),
                    InputFormat = new JSOptionChange<string>( inputFormatChanged, InputFormatConverter.Convert( paramInputFormat ) ),
                    TimeAs24hr = new JSOptionChange<bool>( timeAs24hrChanged, paramTimeAs24hr ),
                    Min = new JSOptionChange<string>( minChanged, paramMin?.ToString( DateFormat ) ),
                    Max = new JSOptionChange<string>( maxChanged, paramMax?.ToString( DateFormat ) ),
                    Disabled = new JSOptionChange<bool>( disabledChanged, paramDisabled ),
                    ReadOnly = new JSOptionChange<bool>( readOnlyChanged, paramReadOnly ),
                    DisabledDates = new JSOptionChange<IEnumerable<string>>( disabledDatesChanged, paramDisabledDates?.Select( x => FormatValueAsString( new TValue[] { x } ) ) ),
                    EnabledDates = new JSOptionChange<IEnumerable<string>>( enabledDatesChanged, paramEnabledDates?.Select( x => FormatValueAsString( new TValue[] { x } ) ) ),
                    DisabledDays = new JSOptionChange<IEnumerable<int>>( disabledDaysChanged, paramDisabledDays?.Select( x => (int)x ) ),
                    SelectionMode = new JSOptionChange<DateInputSelectionMode>( selectionModeChanged, paramSelectionMode ),
                    Inline = new JSOptionChange<bool>( inlineChanged, paramInline ),
                    DisableMobile = new JSOptionChange<bool>( disableMobileChanged, paramDisableMobile ),
                    Placeholder = new JSOptionChange<string>( placeholderChanged, paramPlaceholder ),
                    StaticPicker = new JSOptionChange<bool>( staticPickerChanged, paramStaticPicker ),
                    ShowWeekNumbers = new JSOptionChange<bool>( showWeekNumbersChanged, paramShowWeekNumbers ),
                    ShowTodayButton = new JSOptionChange<bool>( showTodayButtonChanged, paramShowTodayButton ),
                    ShowClearButton = new JSOptionChange<bool>( showClearButtonChanged, paramShowClearButton ),
                    DefaultHour = new JSOptionChange<int>( defaultHourChanged, paramDefaultHour ),
                    DefaultMinute = new JSOptionChange<int>( defaultMinuteChanged, paramDefaultMinute ),
                } ) );
            }
        }

        // Let blazor do its thing!
        await base.SetParametersAsync( parameters );

        if ( ParentValidation is not null )
        {
            if ( datesUsed )
            {
                if ( parameters.TryGetValue<Expression<Func<IReadOnlyList<TValue>>>>( nameof( DatesExpression ), out var datesExpression ) )
                    await ParentValidation.InitializeInputExpression( datesExpression );

                if ( parameters.TryGetValue<string>( nameof( Pattern ), out var pattern ) )
                {
                    await ParentValidation.InitializeInputPattern( pattern, paramDates );
                }
            }
            else // fallback to default behavior
            {
                if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( DateExpression ), out var dateExpression ) )
                    await ParentValidation.InitializeInputExpression( dateExpression );

                if ( parameters.TryGetValue<string>( nameof( Pattern ), out var pattern ) )
                {
                    var value = parameters.TryGetValue<TValue>( nameof( Date ), out var inDate )
                        ? new TValue[] { inDate }
                        : InternalValue;

                    await ParentValidation.InitializeInputPattern( pattern, value );
                }
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

        await JSModule.Initialize( dotNetObjectRef, ElementRef, ElementId, new()
        {
            InputMode = InputMode,
            SelectionMode = SelectionMode.ToDateInputSelectionMode(),
            FirstDayOfWeek = (int)FirstDayOfWeek,
            DisplayFormat = DisplayFormatConverter.Convert( DisplayFormat ),
            InputFormat = InputFormatConverter.Convert( InputFormat ),
            TimeAs24hr = TimeAs24hr,
            DefaultDate = defaultDate,
            DefaultHour = DefaultHour,
            DefaultMinute = DefaultMinute,
            Min = Min?.ToString( DateFormat ),
            Max = Max?.ToString( DateFormat ),
            Disabled = Disabled,
            ReadOnly = ReadOnly,
            DisabledDates = DisabledDates?.Select( x => FormatValueAsString( new TValue[] { x } ) ),
            EnabledDates = EnabledDates?.Select( x => FormatValueAsString( new TValue[] { x } ) ),
            DisabledDays = DisabledDays?.Select( x => (int)x ),
            Localization = GetLocalizationObject(),
            Inline = Inline,
            DisableMobile = DisableMobile,
            Placeholder = Placeholder,
            StaticPicker = StaticPicker,
            ShowWeekNumbers = ShowWeekNumbers,
            ShowTodayButton = ShowTodayButton,
            ShowClearButton = ShowClearButton,
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
        builder.Append( ClassProvider.DatePickerValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

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
            return DateChanged.InvokeAsync( value is null ? default : value.FirstOrDefault() );
    }

    /// <inheritdoc/>
    protected override string FormatValueAsString( IReadOnlyList<TValue> values )
    {
        if ( values is null || values.Count == 0 )
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
            if ( values[0] is null )
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
            Today = Localizer["Today"],
            Clear = Localizer["Clear"],
        };
    }

    /// <inheritdoc/>
    protected override bool IsSameAsInternalValue( IReadOnlyList<TValue> value ) => value.AreEqual( InternalValue );

    /// <inheritdoc/>
    protected override string GetFormatedValueExpression()
    {
        if ( DateExpression is null )
            return null;

        return HtmlFieldPrefix is not null
            ? HtmlFieldPrefix.GetFieldName( DateExpression )
            : ExpressionFormatter.FormatLambda( DateExpression );
    }

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
                Date = value is null ? default : value.FirstOrDefault();
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
    /// The earliest date to accept. Updating this value does not change the selected date, even if it falls below the new minimum.
    /// </summary>
    [Parameter] public DateTimeOffset? Min { get; set; }

    /// <summary>
    /// The latest date to accept. Updating this value does not change the selected date, even if it exceeds the new maximum.
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
    /// List of enabled dates that the user should be able to pick.
    /// </summary>
    [Parameter] public IEnumerable<TValue> EnabledDates { get; set; }
    /// <summary>
    /// List of disabled days in a week that the user should not be able to pick.
    /// </summary>
    [Parameter] public IEnumerable<DayOfWeek> DisabledDays { get; set; }

    /// <summary>
    /// Display the calendar in an always-open state with the inline option.
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
    /// Defines the initial value of the hour element.
    /// </summary>
    [Parameter] public int DefaultHour { get; set; } = 12;

    /// <summary>
    /// Defines the initial value of the minute element.
    /// </summary>
    [Parameter] public int DefaultMinute { get; set; } = 0;

    #endregion
}