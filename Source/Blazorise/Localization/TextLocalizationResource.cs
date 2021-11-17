#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Localization
{
    /// <summary>
    /// Model that holds all the translations for a given resource.
    /// </summary>
    public class TextLocalizationResource
    {
        /// <summary>
        /// Language culture code like "en-US" or "nl-NL"
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// A key-value translations of this culture.
        /// </summary>
        public IReadOnlyDictionary<string, string> Translations { get; set; }
    }
}
