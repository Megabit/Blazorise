#region Using directives
using Blazorise.Localization;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Provides custom translations for built-in labels in <see cref="Ribbon"/>.
/// </summary>
public class RibbonLocalizers
{
    /// <summary>
    /// Overrides the translation for the ribbon tab strip's accessible name.
    /// </summary>
    public TextLocalizerHandler LabelLocalizer { get; set; }

    /// <summary>
    /// Overrides the translation for the quick access toolbar's accessible name.
    /// </summary>
    public TextLocalizerHandler QuickAccessLocalizer { get; set; }

    /// <summary>
    /// Overrides the translation for the action that expands the ribbon.
    /// </summary>
    public TextLocalizerHandler ExpandLocalizer { get; set; }

    /// <summary>
    /// Overrides the translation for the action that collapses the ribbon.
    /// </summary>
    public TextLocalizerHandler CollapseLocalizer { get; set; }
}