using System.Text.Json.Serialization;

namespace Blazorise.RichTextEdit;

internal class RichTextEditJSOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the editor is read-only.
    /// </summary>
    [JsonPropertyName( "readOnly" )]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text for the editor.
    /// </summary>
    [JsonPropertyName( "placeholder" )]
    public string Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the theme for the editor. Expected values are "snow" or "bubble".
    /// </summary>
    [JsonPropertyName( "theme" )]
    public string Theme { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the editor should submit on pressing Enter.
    /// </summary>
    [JsonPropertyName( "submitOnEnter" )]
    public bool SubmitOnEnter { get; set; }

    /// <summary>
    /// Gets or sets the method used to configure Quill.js.
    /// </summary>
    [JsonPropertyName( "configureQuillJsMethod" )]
    public string ConfigureQuillJsMethod { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether tables are enabled in the editor.
    /// </summary>
    [JsonPropertyName( "useTables" )]
    public bool UseTables { get; set; }
}