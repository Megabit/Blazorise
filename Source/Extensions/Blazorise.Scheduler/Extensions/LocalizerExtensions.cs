using Blazorise.Localization;

namespace Blazorise.Scheduler.Extensions;

/// <summary>
/// Helper extension methods for localization.
/// </summary>
internal static class LocalizerExtensions
{
    /// <summary>
    /// Handles the localization of datagrid based on the built-int localizer and a custom localizer handler.
    /// </summary>
    /// <param name="textLocalizer">Default localizer.</param>
    /// <param name="textLocalizerHandler">Custom localizer.</param>
    /// <param name="name">Localization name.</param>
    /// <param name="arguments">Arguments to format the text.</param>
    /// <returns>Returns the localized text.</returns>
    public static string Localize( this ITextLocalizer textLocalizer, TextLocalizerHandler textLocalizerHandler, string name, params object[] arguments )
    {
        if ( textLocalizerHandler != null )
            return textLocalizerHandler.Invoke( name, arguments );

        return textLocalizer[name, arguments];
    }
}
