#region Using directives
using Blazorise.Localization;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Provides custom translations for built-in labels in <see cref="RibbonColorPicker"/>.
/// </summary>
public class RibbonColorPickerLocalizers
{
    /// <summary>
    /// Overrides the heading for simple or custom colors.
    /// </summary>
    public TextLocalizerHandler ColorsLocalizer { get; set; }

    /// <summary>
    /// Overrides the theme-color heading.
    /// </summary>
    public TextLocalizerHandler ThemeColorsLocalizer { get; set; }

    /// <summary>
    /// Overrides the standard-color heading.
    /// </summary>
    public TextLocalizerHandler StandardColorsLocalizer { get; set; }

    /// <summary>
    /// Overrides the translation for the application-defined automatic color command.
    /// </summary>
    public TextLocalizerHandler AutomaticLocalizer { get; set; }

    /// <summary>
    /// Overrides the translation for the command that clears the selected color.
    /// </summary>
    public TextLocalizerHandler NoColorLocalizer { get; set; }

    /// <summary>
    /// Overrides the translation for the command that opens the full color picker.
    /// </summary>
    public TextLocalizerHandler MoreColorsLocalizer { get; set; }
}