#region Using directives
using System;
using System.Globalization;
#endregion

namespace Blazorise;

/// <summary>
/// Exposes menu presentation state shared by the default and provider-specific time picker renderers.
/// </summary>
internal sealed class TimePickerMenuContext<TValue>
{
    #region Members

    private readonly TimePicker<TValue> parent;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new time picker menu presentation context.
    /// </summary>
    /// <param name="parent">Time picker that owns the menu.</param>
    public TimePickerMenuContext( TimePicker<TValue> parent )
    {
        this.parent = parent;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the provider classes for a time control.
    /// </summary>
    /// <param name="part">Time part represented by the control.</param>
    /// <returns>The provider-specific class names.</returns>
    public string GetControlClassNames( TimePickerPart part )
        => parent.PickerClassProvider.TimePickerControl( parent.PickerFocusedPart == part && parent.FocusMenuOnOpen );

    /// <summary>
    /// Gets the DOM identifier for a time part.
    /// </summary>
    /// <param name="part">Time part represented by the element.</param>
    /// <returns>The time part element identifier.</returns>
    public string GetPartId( TimePickerPart part )
        => $"{parent.ElementId}-{part.ToString().ToLowerInvariant()}";

    #endregion

    #region Properties

    /// <summary>
    /// Gets the provider classes for the time picker menu.
    /// </summary>
    public string ClassNames => parent.PickerClassProvider.TimePickerMenu( parent.Inline, parent.StaticPicker );

    /// <summary>
    /// Gets the provider classes for the menu backdrop.
    /// </summary>
    public string BackdropClassNames => parent.PickerClassProvider.TimePickerBackdrop();

    /// <summary>
    /// Gets the provider classes for the time controls container.
    /// </summary>
    public string ControlsClassNames => parent.PickerClassProvider.TimePickerControls();

    /// <summary>
    /// Gets the provider classes for a time input.
    /// </summary>
    public string InputClassNames => parent.PickerClassProvider.TimePickerInput();

    /// <summary>
    /// Gets the provider classes for a time separator.
    /// </summary>
    public string SeparatorClassNames => parent.PickerClassProvider.TimePickerSeparator();

    /// <summary>
    /// Gets the provider classes for the meridiem control.
    /// </summary>
    public string MeridiemClassNames => parent.PickerClassProvider.TimePickerMeridiem(
        IsPostMeridiem,
        parent.PickerFocusedPart == TimePickerPart.Meridiem && parent.FocusMenuOnOpen );

    /// <summary>
    /// Gets the DOM identifier of the time part targeted by keyboard navigation.
    /// </summary>
    public string FocusedPartId => GetPartId( parent.PickerFocusedPart );

    /// <summary>
    /// Gets the tab index applied to interactive menu controls.
    /// </summary>
    public int ControlTabIndex => parent.Inline || parent.FocusMenuOnOpen ? 0 : -1;

    /// <summary>
    /// Gets a valid hour increment.
    /// </summary>
    public int SafeHourIncrement => Math.Max( 1, parent.HourIncrement );

    /// <summary>
    /// Gets a valid minute increment.
    /// </summary>
    public int SafeMinuteIncrement => Math.Max( 1, parent.MinuteIncrement );

    /// <summary>
    /// Gets the selected hour in 24-hour form.
    /// </summary>
    public int CurrentHour => parent.PickerSelectedTime.Hours;

    /// <summary>
    /// Gets the selected minute.
    /// </summary>
    public int CurrentMinute => parent.PickerSelectedTime.Minutes;

    /// <summary>
    /// Gets the selected second.
    /// </summary>
    public int CurrentSecond => parent.PickerSelectedTime.Seconds;

    /// <summary>
    /// Gets the hour represented according to the configured clock format.
    /// </summary>
    public int DisplayHour => parent.TimeAs24hr ? CurrentHour : CurrentHour % 12 == 0 ? 12 : CurrentHour % 12;

    /// <summary>
    /// Gets the zero-padded display hour.
    /// </summary>
    public string DisplayHourText => DisplayHour.ToString( "D2", CultureInfo.InvariantCulture );

    /// <summary>
    /// Gets the zero-padded selected minute.
    /// </summary>
    public string CurrentMinuteText => CurrentMinute.ToString( "D2", CultureInfo.InvariantCulture );

    /// <summary>
    /// Gets the zero-padded selected second.
    /// </summary>
    public string CurrentSecondText => CurrentSecond.ToString( "D2", CultureInfo.InvariantCulture );

    /// <summary>
    /// Gets whether the selected time is post meridiem.
    /// </summary>
    public bool IsPostMeridiem => CurrentHour >= 12;

    /// <summary>
    /// Gets the localized ante meridiem text.
    /// </summary>
    public string AnteMeridiemText => parent.PickerLocalizer["AM"];

    /// <summary>
    /// Gets the localized post meridiem text.
    /// </summary>
    public string PostMeridiemText => parent.PickerLocalizer["PM"];

    /// <summary>
    /// Gets the localized meridiem text for the selected time.
    /// </summary>
    public string MeridiemText => IsPostMeridiem ? PostMeridiemText : AnteMeridiemText;

    /// <summary>
    /// Gets the accessible label for the meridiem control.
    /// </summary>
    public string MeridiemLabel => MeridiemText;

    /// <summary>
    /// Gets the localized time section label.
    /// </summary>
    public string TimeText => parent.PickerLocalizer["Time"];

    /// <summary>
    /// Gets the localized hour field label.
    /// </summary>
    public string HourText => parent.PickerLocalizer["Hour"];

    /// <summary>
    /// Gets the localized minute field label.
    /// </summary>
    public string MinuteText => parent.PickerLocalizer["Minute"];

    /// <summary>
    /// Gets the localized second field label.
    /// </summary>
    public string SecondText => parent.PickerLocalizer["Second"];

    #endregion
}