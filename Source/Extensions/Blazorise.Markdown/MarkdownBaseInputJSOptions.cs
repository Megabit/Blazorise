using System.Collections.Generic;

namespace Blazorise.Markdown;

/// <summary>
/// Represents BaseInputComponent-related options applied to the EasyMDE DOM elements.
/// </summary>
public class MarkdownBaseInputJSOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the editor should be read-only.
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the editor should be disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the computed classnames that should be applied to the editor.
    /// </summary>
    public string ClassNames { get; set; }

    /// <summary>
    /// Gets or sets the computed styles that should be applied to the editor.
    /// </summary>
    public string StyleNames { get; set; }

    /// <summary>
    /// Gets or sets the additional HTML attributes that should be applied to the editor.
    /// </summary>
    public Dictionary<string, object> Attributes { get; set; }
}