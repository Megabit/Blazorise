#region Using directives
using Blazorise.Localization;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Provides custom translations for built-in labels in <see cref="RibbonBackstage"/>.
/// </summary>
public class RibbonBackstageLocalizers
{
    /// <summary>
    /// Overrides the translation for the backstage surface's accessible name.
    /// </summary>
    public TextLocalizerHandler LabelLocalizer { get; set; }

    /// <summary>
    /// Overrides the translation for the backstage navigation's accessible name.
    /// </summary>
    public TextLocalizerHandler NavigationLocalizer { get; set; }

    /// <summary>
    /// Overrides the translation for the command that returns to the document.
    /// </summary>
    public TextLocalizerHandler BackLocalizer { get; set; }
}