using System.Collections.Generic;

namespace Blazorise.Modules;

/// <summary>
/// Represents JavaScript options for configuring a color picker component.
/// </summary>
public class ColorPickerJSOptions
{
    /// <summary>
    /// Gets or sets the default color value for the color picker.
    /// </summary>
    public string Default { get; set; }

    /// <summary>
    /// Gets or sets the palette of colors available in the color picker.
    /// </summary>
    public string[] Palette { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show the color palette.
    /// </summary>
    public bool ShowPalette { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to hide the color picker after a palette color is selected.
    /// </summary>
    public bool HideAfterPaletteSelect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show a clear button to reset the color selection.
    /// </summary>
    public bool ShowClearButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show a cancel button to dismiss the color selection.
    /// </summary>
    public bool ShowCancelButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show an opacity slider for selecting transparency.
    /// </summary>
    public bool ShowOpacitySlider { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show a hue slider for selecting color shades.
    /// </summary>
    public bool ShowHueSlider { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show an input field for direct color input.
    /// </summary>
    public bool ShowInputField { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the color picker is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the color picker is read-only.
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the localization options for the color picker UI, with string keys and values for translations.
    /// </summary>
    public IReadOnlyDictionary<string, string> Localization { get; set; }

    /// <summary>
    /// Gets or sets the CSS selector for the element where the color preview will be displayed.
    /// </summary>
    public string ColorPreviewElementSelector { get; set; }

    /// <summary>
    /// Gets or sets the CSS selector for the element where the color value will be displayed.
    /// </summary>
    public string ColorValueElementSelector { get; set; }
}

/// <summary>
/// Represents JavaScript options for updating specific color picker settings dynamically.
/// </summary>
public class ColorPickerUpdateJsOptions
{
    /// <summary>
    /// Gets or sets the change option for updating the color palette.
    /// </summary>
    public JSOptionChange<string[]> Palette { get; set; }

    /// <summary>
    /// Gets or sets the change option for showing or hiding the color palette.
    /// </summary>
    public JSOptionChange<bool> ShowPalette { get; set; }

    /// <summary>
    /// Gets or sets the change option for hiding the color picker after a palette selection.
    /// </summary>
    public JSOptionChange<bool> HideAfterPaletteSelect { get; set; }

    /// <summary>
    /// Gets or sets the change option for enabling or disabling the color picker.
    /// </summary>
    public JSOptionChange<bool> Disabled { get; set; }

    /// <summary>
    /// Gets or sets the change option for setting the color picker as read-only.
    /// </summary>
    public JSOptionChange<bool> ReadOnly { get; set; }
}
