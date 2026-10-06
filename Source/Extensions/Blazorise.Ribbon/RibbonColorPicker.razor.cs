#region Using directives
using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Displays the standard Blazorise color picker in a ribbon.
/// </summary>
public partial class RibbonColorPicker : BaseComponent
{
    #region Members

    private ColorPicker pickerRef;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes compact sizing for color pickers in ribbon rows.
    /// </summary>
    public RibbonColorPicker()
    {
        Width = Blazorise.Width.Auto;
        Flex = Blazorise.Flex.Shrink.Is0;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        ElementRef = pickerRef.ElementRef;

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
    /// Specifies the selected color as a string supported by <see cref="ColorPicker"/>.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Occurs when the selected color changes. Supplies the new color value for two-way binding.
    /// </summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Displays the selected color value beside the swatch when the picker is closed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c> to keep ribbon commands compact.
    /// </remarks>
    [Parameter] public bool ShowValue { get; set; }

    /// <summary>
    /// Controls the size of the color picker rather than the ribbon command layout. Defaults to <see cref="Size.Default"/>.
    /// </summary>
    [Parameter] public Size Size { get; set; }

    /// <summary>
    /// Disables interaction with the color picker. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

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

    /// <summary>
    /// Defines additional content passed to the underlying <see cref="ColorPicker"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}