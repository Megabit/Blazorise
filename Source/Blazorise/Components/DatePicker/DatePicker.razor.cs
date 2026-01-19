#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
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

    private DotNetObjectReference<DatePickerAdapter> dotNetObjectRef;

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

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void CaptureParameters( ParameterView parameters )
    {
        base.CaptureParameters( parameters );

        parameters.TryGetParameter( nameof( Min ), Min, out paramMin );
        parameters.TryGetParameter( nameof( Max ), Max, out paramMax );
        parameters.TryGetParameter( nameof( FirstDayOfWeek ), FirstDayOfWeek, out paramFirstDayOfWeek );
        parameters.TryGetParameter( nameof( DisplayFormat ), DisplayFormat, out paramDisplayFormat );
        parameters.TryGetParameter( nameof( InputFormat ), InputFormat, out paramInputFormat );
        parameters.TryGetParameter( nameof( TimeAs24hr ), TimeAs24hr, out paramTimeAs24hr );
        parameters.TryGetParameter( nameof( Disabled ), Disabled, out paramDisabled );
        parameters.TryGetParameter( nameof( ReadOnly ), ReadOnly, out paramReadOnly );
        parameters.TryGetParameter( nameof( DisabledDates ), DisabledDates, out paramDisabledDates );
        parameters.TryGetParameter( nameof( EnabledDates ), EnabledDates, out paramEnabledDates );
        parameters.TryGetParameter( nameof( DisabledDays ), DisabledDays, out paramDisabledDays );
        parameters.TryGetParameter( nameof( SelectionMode ), SelectionMode, out paramSelectionMode );
        parameters.TryGetParameter( nameof( Inline ), Inline, out paramInline );
        parameters.TryGetParameter( nameof( DisableMobile ), DisableMobile, out paramDisableMobile );
        parameters.TryGetParameter( nameof( Placeholder ), Placeholder, out paramPlaceholder );
        parameters.TryGetParameter( nameof( StaticPicker ), StaticPicker, out paramStaticPicker );
        parameters.TryGetParameter( nameof( ShowWeekNumbers ), ShowWeekNumbers, out paramShowWeekNumbers );
        parameters.TryGetParameter( nameof( ShowTodayButton ), ShowTodayButton, out paramShowTodayButton );
        parameters.TryGetParameter( nameof( ShowClearButton ), ShowClearButton, out paramShowClearButton );
        parameters.TryGetParameter( nameof( DefaultHour ), DefaultHour, out paramDefaultHour );
        parameters.TryGetParameter( nameof( DefaultMinute ), DefaultMinute, out paramDefaultMinute );
    }

    /// <inheritdoc/>
    protected override async Task OnBeforeSetParametersAsync( ParameterView parameters )
    {
        await base.OnBeforeSetParametersAsync( parameters );

        if ( Rendered )
        {
            var minChanged = paramMin.Defined && paramMin.Changed;
            var maxChanged = paramMax.Defined && paramMax.Changed;
            var firstDayOfWeekChanged = paramFirstDayOfWeek.Defined && paramFirstDayOfWeek.Changed;
            var displayFormatChanged = paramDisplayFormat.Defined && paramDisplayFormat.Changed;
            var inputFormatChanged = paramInputFormat.Defined && paramInputFormat.Changed;
            var timeAs24hrChanged = paramTimeAs24hr.Defined && paramTimeAs24hr.Changed;
            var disabledChanged = paramDisabled.Defined && paramDisabled.Changed;
            var readOnlyChanged = paramReadOnly.Defined && paramReadOnly.Changed;
            var disabledDatesChanged = paramDisabledDates.Defined && paramDisabledDates.Changed;
            var enabledDatesChanged = paramEnabledDates.Defined && paramEnabledDates.Changed;
            var disabledDaysChanged = paramDisabledDays.Defined && paramDisabledDays.Changed;
            var selectionModeChanged = paramSelectionMode.Defined && paramSelectionMode.Changed;
            var inlineChanged = paramInline.Defined && paramInline.Changed;
            var disableMobileChanged = paramDisableMobile.Defined && paramDisableMobile.Changed;
            var placeholderChanged = paramPlaceholder.Defined && paramPlaceholder.Changed;
            var staticPickerChanged = paramStaticPicker.Defined && paramStaticPicker.Changed;
            var showWeekNumbersChanged = paramShowWeekNumbers.Defined && paramShowWeekNumbers.Changed;
            var showTodayButtonChanged = paramShowTodayButton.Defined && paramShowTodayButton.Changed;
            var showClearButtonChanged = paramShowClearButton.Defined && paramShowClearButton.Changed;
            var defaultHourChanged = paramDefaultHour.Defined && paramDefaultHour.Changed;
            var defaultMinuteChanged = paramDefaultMinute.Defined && paramDefaultMinute.Changed;

            if ( paramValue.Changed )
            {
                var formatedDateString = FormatValueAsString( paramValue.Value );

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
                ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, new()
                {
                    FirstDayOfWeek = new JSOptionChange<int>( firstDayOfWeekChanged, (int)paramFirstDayOfWeek.Value ),
                    DisplayFormat = new JSOptionChange<string>( displayFormatChanged, DisplayFormatConverter.Convert( paramDisplayFormat.Value ) ),
                    InputFormat = new JSOptionChange<string>( inputFormatChanged, InputFormatConverter.Convert( paramInputFormat.Value ) ),
                    TimeAs24hr = new JSOptionChange<bool>( timeAs24hrChanged, paramTimeAs24hr.Value ),
                    Min = new JSOptionChange<string>( minChanged, paramMin.Value?.ToString( DateFormat ) ),
                    Max = new JSOptionChange<string>( maxChanged, paramMax.Value?.ToString( DateFormat ) ),
                    Disabled = new JSOptionChange<bool>( disabledChanged, paramDisabled.Value ),
                    ReadOnly = new JSOptionChange<bool>( readOnlyChanged, paramReadOnly.Value ),
                    DisabledDates = new JSOptionChange<IEnumerable<string>>( disabledDatesChanged, FormatDatesAsStrings( paramDisabledDates.Value ) ),
                    EnabledDates = new JSOptionChange<IEnumerable<string>>( enabledDatesChanged, FormatDatesAsStrings( paramEnabledDates.Value ) ),
                    DisabledDays = new JSOptionChange<IEnumerable<int>>( disabledDaysChanged, paramDisabledDays.Value?.Select( x => (int)x ) ),
                    SelectionMode = new JSOptionChange<DateInputSelectionMode>( selectionModeChanged, paramSelectionMode.Value ),
                    Inline = new JSOptionChange<bool>( inlineChanged, paramInline.Value ),
                    DisableMobile = new JSOptionChange<bool>( disableMobileChanged, paramDisableMobile.Value ),
                    Placeholder = new JSOptionChange<string>( placeholderChanged, paramPlaceholder.Value ),
                    StaticPicker = new JSOptionChange<bool>( staticPickerChanged, paramStaticPicker.Value ),
                    ShowWeekNumbers = new JSOptionChange<bool>( showWeekNumbersChanged, paramShowWeekNumbers.Value ),
                    ShowTodayButton = new JSOptionChange<bool>( showTodayButtonChanged, paramShowTodayButton.Value ),
                    ShowClearButton = new JSOptionChange<bool>( showClearButtonChanged, paramShowClearButton.Value ),
                    DefaultHour = new JSOptionChange<int>( defaultHourChanged, paramDefaultHour.Value ),
                    DefaultMinute = new JSOptionChange<int>( defaultMinuteChanged, paramDefaultMinute.Value ),
                } ) );
            }
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
        object defaultDate = FormatValueAsString( Value );

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
            DisabledDates = FormatDatesAsStrings( DisabledDates ),
            EnabledDates = FormatDatesAsStrings( EnabledDates ),
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
    protected override string FormatValueAsString( TValue value )
    {
        if ( value is null )
            return null;

        if ( SelectionMode != DateInputSelectionMode.Single )
        {
            var results = new List<string>();

            if ( value is IEnumerable<TValue> values )
            {
                foreach ( var val in values )
                {
                    results.Add( Formaters.FormatDateValueAsString( val, DateFormat ) );
                }
            }
            else if ( value is IEnumerable objects )
            {
                foreach ( var val in objects )
                {
                    results.Add( Formaters.FormatDateValueAsString( val, DateFormat ) );
                }
            }

            var delimiter = SelectionMode == DateInputSelectionMode.Multiple ? MULTIPLE_DELIMITER : CurrentRangeSeparator;

            return string.Join( delimiter, results );
        }

        return Formaters.FormatDateValueAsString( value, DateFormat );
    }

    private IEnumerable<string> FormatDatesAsStrings( IEnumerable values )
    {
        if ( values is null )
            return null;

        List<string> result = new List<string>();

        foreach ( object value in values )
        {
            result.Add( Formaters.FormatDateValueAsString( value, DateFormat ) );
        }

        return result;
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
    {
        if ( SelectionMode != DateInputSelectionMode.Single )
        {
            var delimiter = SelectionMode == DateInputSelectionMode.Multiple ? MULTIPLE_DELIMITER : CurrentRangeSeparator;

            var readOnlyList = Parsers.ParseCsvDatesToReadOnlyList<TValue>( value, delimiter, InputMode );

            return Task.FromResult( new ParseValue<TValue>( true, readOnlyList, null ) );
        }
        else
        {
            if ( Parsers.TryParseDate<TValue>( value, InputMode, out var result ) )
            {
                return Task.FromResult( new ParseValue<TValue>( true, result, null ) );
            }
            else
            {
                return Task.FromResult( new ParseValue<TValue>( false, default, null ) );
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
    protected override bool IsSameAsInternalValue( TValue value )
    {
        if ( value is IEnumerable<TValue> values1 && Value is IEnumerable<TValue> values2 )
        {
            return values1.AreEqual( values2 );
        }
        else if ( value is IEnumerable objects1 && Value is IEnumerable objects2 )
        {
            return objects1.AreEqual( objects2 );
        }

        return value.IsEqual( Value );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

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