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
    /// Gets or sets the selected color.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Occurs when the selected color changes.
    /// </summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets whether to show the color value beside the swatch. Hidden by default.
    /// </summary>
    [Parameter] public bool ShowValue { get; set; }

    /// <summary>
    /// Gets or sets the native color picker size.
    /// </summary>
    [Parameter] public Size Size { get; set; }

    /// <summary>
    /// Gets or sets whether the picker is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the color is read-only.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the palette colors, using the native palette by default.
    /// </summary>
    [Parameter] public string[] Palette { get; set; } = new ColorPicker().Palette;

    /// <summary>
    /// Gets or sets whether to show palette colors in the popup.
    /// </summary>
    [Parameter] public bool ShowPalette { get; set; } = true;

    /// <summary>
    /// Gets or sets whether selecting a palette color closes the popup.
    /// </summary>
    [Parameter] public bool HideAfterPaletteSelect { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the clear button.
    /// </summary>
    [Parameter] public bool ShowClearButton { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the cancel button.
    /// </summary>
    [Parameter] public bool ShowCancelButton { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the opacity slider.
    /// </summary>
    [Parameter] public bool ShowOpacitySlider { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the hue slider.
    /// </summary>
    [Parameter] public bool ShowHueSlider { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the color input field inside the popup.
    /// </summary>
    [Parameter] public bool ShowInputField { get; set; } = true;

    /// <summary>
    /// Gets or sets custom localization for the color picker.
    /// </summary>
    [Parameter] public TextLocalizerHandler PickerLocalizer { get; set; }

    /// <summary>
    /// Gets or sets content passed to the color picker.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}