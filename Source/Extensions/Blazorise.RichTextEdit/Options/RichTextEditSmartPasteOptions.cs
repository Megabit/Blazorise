using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blazorise.RichTextEdit;

/// <summary>
/// Represents options used to configure the quill-paste-smart clipboard module.
/// </summary>
public class RichTextEditSmartPasteOptions
{
    /// <summary>
    /// Gets or sets the list of allowed HTML tags.
    /// </summary>
    [JsonPropertyName( "allowedTags" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public IEnumerable<string> AllowedTags { get; set; }

    /// <summary>
    /// Gets or sets the list of allowed HTML attributes.
    /// </summary>
    [JsonPropertyName( "allowedAttributes" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public IEnumerable<string> AllowedAttributes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether pasted content should preserve the current selection.
    /// </summary>
    [JsonPropertyName( "keepSelection" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? KeepSelection { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether block-level elements should be substituted when needed.
    /// </summary>
    [JsonPropertyName( "substituteBlockElements" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? SubstituteBlockElements { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether plain URL text should be converted into links when pasting.
    /// </summary>
    [JsonPropertyName( "magicPasteLinks" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? MagicPasteLinks { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether consecutive substitution tags should be collapsed.
    /// </summary>
    [JsonPropertyName( "removeConsecutiveSubstitutionTags" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? RemoveConsecutiveSubstitutionTags { get; set; }
}