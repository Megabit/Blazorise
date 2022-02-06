namespace Blazorise.Markdown
{
    /// <summary>
    /// Set DateTimeFormat. More information see DateTimeFormat instances.
    /// Default locale: en-US, format: hour:minute.
    /// 
    /// See <see href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Intl/DateTimeFormat/resolvedOptions">Intl.DateTimeFormat</see>
    /// for more information
    /// </summary>
    public class MarkdownAutosaveTimeFormat
    {
        /// <summary>
        /// The BCP 47 language tag for the locale actually used. If any Unicode extension values
        /// were requested in the input BCP 47 language tag that led to this locale, the key-value
        /// pairs that were requested and are supported for this locale are included in locale.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Intl.DateTimeFormat options
        /// </summary>
        public MarkdownAutosaveTimeFormatOptions Format { get; set; }
    }
}
