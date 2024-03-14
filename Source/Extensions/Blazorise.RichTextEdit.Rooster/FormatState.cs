namespace Blazorise.RichTextEdit.Rooster;

/// <summary>
/// The editor format state
/// </summary>
public class FormatState
{
    /// <summary>
    /// Background color
    /// </summary>
    public string BackgroundColor { get; init; }
    /// <summary>
    /// Mode independent background color for dark mode
    /// </summary>
    public ModeIndependentColor BackgroundColors { get; init; }
    /// <summary>
    /// Whether add image alt text command can be called to the text
    /// </summary>
    public bool? CanAddImageAltText { get; init; }
    /// <summary>
    /// Whether we can execute table cell merge operation
    /// </summary>
    public bool? CanMergeTableCell { get; init; }
    /// <summary>
    /// Whether the content can be redone
    /// </summary>
    public bool? CanRedo { get; init; }
    /// <summary>
    /// Whether the content can be undone
    /// </summary>
    public bool? CanUndo { get; init; }
    /// <summary>
    /// Whether unlink command can be called to the text
    /// </summary>
    public bool? CanUnlink { get; init; }
    /// <summary>
    /// Direction of the element ('ltr' or 'rtl')
    /// </summary>
    public string Direction { get; init; }
    /// <summary>
    /// Font name
    /// </summary>
    public string FontName { get; init; }
    /// <summary>
    /// Font size
    /// </summary>
    public string FontSize { get; init; }
    /// <summary>
    /// Font weight
    /// </summary>
    public string FontWeight { get; init; }
    /// <summary>
    /// Heading level (0-6, 0 means no heading)
    /// </summary>
    public int? HeadingLevel { get; init; }
    /// <summary>
    /// Whether the text is in block quote
    /// </summary>
    public bool? IsBlockQuote { get; init; }
    /// <summary>
    /// Whether the text is bolded
    /// </summary>
    public bool? IsBold { get; init; }
    /// <summary>
    /// Whether the text is in bullet mode
    /// </summary>
    public bool? IsBullet { get; init; }
    /// <summary>
    /// Whether the text is in Code block
    /// </summary>
    public bool? IsCodeBlock { get; init; }
    /// <summary>
    /// Whether the text is in Code element
    /// </summary>
    public bool? IsCodeInline { get; init; }
    /// <summary>
    /// Whether editor is in dark mode
    /// </summary>
    public bool? IsDarkMode { get; init; }
    /// <summary>
    /// Whether the cursor is in table
    /// </summary>
    public bool? IsInTable { get; init; }
    /// <summary>
    /// Whether the text is italic
    /// </summary>
    public bool? IsItalic { get; init; }
    /// <summary>
    /// Whether the selected text is multiline
    /// </summary>
    public bool? IsMultilineSelection { get; init; }
    /// <summary>
    /// Whether the text is in numbering mode
    /// </summary>
    public bool? IsNumbering { get; init; }
    /// <summary>
    /// Whether the text has strike through line
    /// </summary>
    public bool? IsStrikeThrough { get; init; }
    /// <summary>
    /// Whether the text is in subscript mode
    /// </summary>
    public bool? IsSubscript { get; init; }
    /// <summary>
    /// Whether the text is in superscript mode
    /// </summary>
    public bool? IsSuperscript { get; init; }
    /// <summary>
    /// Whether the text has underline
    /// </summary>
    public bool? IsUnderline { get; init; }
    /// <summary>
    /// Line height
    /// </summary>
    public string LineHeight { get; init; }
    /// <summary>
    /// Margin Bottom
    /// </summary>
    public string MarginBottom { get; init; }
    /// <summary>
    /// Margin Top
    /// </summary>
    public string MarginTop { get; init; }
    /// <summary>
    /// Format of table, if there is table at cursor position
    /// </summary>
    //TODO public string TableFormat { get; init; }
    /// <summary>
    /// If there is a table, whether the table has header row
    /// </summary>
    public string TableHasHeader { get; init; }
    /// <summary>
    /// Text Align
    /// </summary>
    public string TextAlign { get; init; }
    /// <summary>
    /// Text color
    /// </summary>
    public string TextColor { get; init; }
    /// <summary>
    /// Mode independent background color for dark mode
    /// </summary>
    public ModeIndependentColor TextColors { get; init; }
    /// <summary>
    /// Current zoom scale of editor
    /// </summary>
    public int? ZoomScale { get; init; }
}

/// <summary>
/// A color object contains both light mode and dark mode color
/// </summary>
/// <param name="DarkModeColor">The color to be used in dark mode, if enabled.</param>
/// <param name="LightModeColor">The color to be used in light mode, or stored as the original color in dark mode.</param>
public record ModeIndependentColor( string DarkModeColor, string LightModeColor );