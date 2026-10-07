#region Using directives
using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Opens a color picker from a standard swatch, an icon command, or custom content.
/// </summary>
public partial class RibbonColorPicker : BaseRibbonItem
{
    #region Members

    private ColorPicker pickerRef;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the default color command presentation.
    /// </summary>
    public RibbonColorPicker()
    {
        Padding = Blazorise.Padding.Is1;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            ElementRef = pickerRef.ElementRef;
        }

        return base.OnAfterRenderAsync( firstRender );
    }

    /// <summary>
    /// Handles changes received from the color picker.
    /// </summary>
    protected Task OnValueChangedHandler( string value ) => HandleValueChanged( value );

    /// <summary>
    /// Updates the selected color before notifying the application.
    /// </summary>
    protected virtual async Task HandleValueChanged( string value )
    {
        if ( Disabled || ReadOnly || Value == value )
        {
            return;
        }

        Value = value;

        await ValueChanged.InvokeAsync( value );
    }

    /// <summary>
    /// Moves keyboard focus to the color picker.
    /// </summary>
    /// <param name="scrollToElement">
    /// Whether to scroll the picker into view.
    /// </param>
    public Task Focus( bool scrollToElement = true ) => pickerRef.Focus( scrollToElement );

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets the selected-color indicator background, clearing it when no color is selected.
    /// </summary>
    protected Background EffectiveIndicatorBackground => string.IsNullOrEmpty( Value ) ? Blazorise.Background.Transparent : new( Value );

    /// <summary>
    /// Specifies the selected color as a string supported by <see cref="ColorPicker"/>.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Occurs when the selected color changes. Supplies the new color value for two-way binding.
    /// </summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Displays the selected color value beside the command preview when the picker is closed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c> to keep ribbon commands compact.
    /// </remarks>
    [Parameter] public bool ShowValue { get; set; }

    /// <summary>
    /// Prevents changing the selected color. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// Defines preset colors for quick selection in the popup.
    /// </summary>
    /// <remarks>
    /// Use this for frequently used document colors. Defaults to the palette supplied by <see cref="ColorPicker"/>.
    /// </remarks>
    [Parameter] public string[] Palette { get; set; } = new ColorPicker().Palette;

    /// <summary>
    /// Displays the preset palette colors in the popup. Defaults to <c>true</c>.
    /// </summary>
    [Parameter] public bool ShowPalette { get; set; } = true;

    /// <summary>
    /// Automatically closes the popup after selecting a palette color. Defaults to <c>true</c>.
    /// </summary>
    [Parameter] public bool HideAfterPaletteSelect { get; set; } = true;

    /// <summary>
    /// Displays the clear button in the popup. Defaults to <c>true</c>.
    /// </summary>
    [Parameter] public bool ShowClearButton { get; set; } = true;

    /// <summary>
    /// Displays the cancel button in the popup. Defaults to <c>true</c>.
    /// </summary>
    [Parameter] public bool ShowCancelButton { get; set; } = true;

    /// <summary>
    /// Displays the opacity slider for adjusting color transparency. Defaults to <c>true</c>.
    /// </summary>
    [Parameter] public bool ShowOpacitySlider { get; set; } = true;

    /// <summary>
    /// Displays the hue slider for choosing the base color. Defaults to <c>true</c>.
    /// </summary>
    [Parameter] public bool ShowHueSlider { get; set; } = true;

    /// <summary>
    /// Displays the color value input field inside the popup. Defaults to <c>true</c>.
    /// </summary>
    [Parameter] public bool ShowInputField { get; set; } = true;

    /// <summary>
    /// Provides custom localization for color picker text, overriding the default translations.
    /// </summary>
    [Parameter] public TextLocalizerHandler PickerLocalizer { get; set; }

    #endregion
}