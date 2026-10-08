#region Using directives
using Blazorise.Localization;
#endregion

namespace Blazorise.Ribbon.Extensions;

/// <summary>
/// Resolves custom translation handlers before using the component's resource translations.
/// </summary>
internal static class LocalizerExtensions
{
    /// <summary>
    /// Translates a resource key with an optional application-provided handler.
    /// </summary>
    /// <param name="textLocalizer">
    /// Localizer that supplies the component's embedded translations.
    /// </param>
    /// <param name="textLocalizerHandler">
    /// Optional handler that overrides the resource translation.
    /// </param>
    /// <param name="name">
    /// Resource key to translate.
    /// </param>
    /// <param name="arguments">
    /// Arguments used to format the translated text.
    /// </param>
    /// <returns>
    /// The custom translation or localized resource text.
    /// </returns>
    public static string Localize( this ITextLocalizer textLocalizer, TextLocalizerHandler textLocalizerHandler, string name, params object[] arguments )
        => textLocalizerHandler is not null ? textLocalizerHandler.Invoke( name, arguments ) : textLocalizer[name, arguments];
}