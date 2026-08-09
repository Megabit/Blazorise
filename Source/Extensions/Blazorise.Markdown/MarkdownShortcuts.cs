namespace Blazorise.Markdown;

/// <summary>
/// Keyboard shortcuts associated with this instance.
/// Defaults to the array of <see href="https://github.com/Ionaru/easy-markdown-editor#keyboard-shortcuts">shortcuts</see>.
/// </summary>
public class MarkdownShortcuts
{
    /// <summary>Key chord that removes block formatting.</summary>
    public string CleanBlock { get; set; } = "Cmd-E";

    /// <summary>Key chord that inserts an image.</summary>
    public string DrawImage { get; set; } = "Cmd-Alt-I";

    /// <summary>Key chord that inserts a hyperlink.</summary>
    public string DrawLink { get; set; } = "Cmd-K";

    /// <summary>Key chord that toggles block-quote syntax.</summary>
    public string ToggleBlockquote { get; set; } = "Cmd-'";

    /// <summary>Key chord that toggles bold emphasis.</summary>
    public string ToggleBold { get; set; } = "Cmd-B";

    /// <summary>Key chord that toggles code-block formatting.</summary>
    public string ToggleCodeBlock { get; set; } = "Cmd-Alt-C";

    /// <summary>Key chord that enters or leaves fullscreen mode.</summary>
    public string ToggleFullScreen { get; set; } = "F11";

    /// <summary>Key chord that increases the heading level.</summary>
    public string ToggleHeadingBigger { get; set; } = "Shift-Cmd-H";

    /// <summary>Key chord that reduces the heading level.</summary>
    public string ToggleHeadingSmaller { get; set; } = "Cmd-H";

    /// <summary>Key chord that toggles italic emphasis.</summary>
    public string ToggleItalic { get; set; } = "Cmd-I";

    /// <summary>Key chord that toggles a numbered list.</summary>
    public string ToggleOrderedList { get; set; } = "Cmd-Alt-L";

    /// <summary>Key chord that shows or hides rendered preview.</summary>
    public string TogglePreview { get; set; } = "Cmd-P";

    /// <summary>Key chord that toggles side-by-side preview.</summary>
    public string ToggleSideBySide { get; set; } = "F9";

    /// <summary>Key chord that toggles a bulleted list.</summary>
    public string ToggleUnorderedList { get; set; } = "Cmd-L";

    /// <summary>Key chord that toggles strikethrough syntax.</summary>
    public string ToggleStrikethrough { get; set; } = "F4";
}