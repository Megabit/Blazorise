#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Markdown
{
    /// <summary>
    /// Saves the text that's being written and will load it back in the future.
    /// It will forget the text when the form it's contained in is submitted.
    /// </summary>
    public class MarkdownAutosave
    {
        /// <summary>
        /// If set to true, saves the text automatically.
        /// Defaults to false.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Delay between saves, in milliseconds.
        /// Defaults to 10000 (10s).
        /// </summary>
        public int Delay { get; set; } = 10000;

        /// <summary>
        /// Delay before assuming that submit of the form failed and saving the text, in milliseconds.
        /// Defaults to autosave.delay or 10000 (10s).
        /// </summary>
        [JsonPropertyName( "submit_delay" )]
        public int? SubmitDelay { get; set; }

        /// <summary>
        /// You must set a unique string identifier so that EasyMDE can autosave.
        /// Something that separates this from other instances of EasyMDE elsewhere on your website.
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// Set text for autosave.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Set DateTimeFormat. More information see DateTimeFormat instances.
        /// Default locale: en-US, format: hour:minute.
        /// </summary>
        public MarkdownAutosaveTimeFormat TimeFormat { get; set; }
    }
}
