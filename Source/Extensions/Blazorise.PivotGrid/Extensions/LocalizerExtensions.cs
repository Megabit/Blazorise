#region Using directives
using Blazorise.Localization;
#endregion

namespace Blazorise.PivotGrid.Extensions;

/// <summary>
/// Helper extension methods for localization.
/// </summary>
internal static class LocalizerExtensions
{
    /// <summary>
    /// Localizes text with optional custom handler.
    /// </summary>
    public static string Localize( this ITextLocalizer textLocalizer, TextLocalizerHandler textLocalizerHandler, string name, params object[] arguments )
    {
        if ( textLocalizerHandler is not null )
            return textLocalizerHandler.Invoke( name, arguments );

        return textLocalizer[name, arguments];
    }
}