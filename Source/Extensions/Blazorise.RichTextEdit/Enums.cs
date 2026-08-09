namespace Blazorise.RichTextEdit;

/// <summary>
/// QuillJS toolbar actions
/// </summary>
/// <seealso href="https://quilljs.com/docs/modules/toolbar/"/>
public enum RichTextEditAction
{
    /// <summary>Applies a bold font weight.</summary>
    Bold,
    /// <summary>Applies italic emphasis.</summary>
    Italic,
    /// <summary>Draws a line beneath text.</summary>
    Underline,
    /// <summary>Draws a line through text.</summary>
    Strike,
    /// <summary>Formats the selection as a block quotation.</summary>
    Blockquote,
    /// <summary>Formats the selection as a code block.</summary>
    CodeBlock,
    /// <summary>Changes the heading level.</summary>
    Header,
    /// <summary>Creates an ordered or unordered list.</summary>
    List,
    /// <summary>Applies superscript or subscript formatting.</summary>
    Script,
    /// <summary>Changes the nesting level of a block.</summary>
    Indent,
    /// <summary>Changes the writing direction.</summary>
    Direction,
    /// <summary>Changes the text size.</summary>
    Size,
    /// <summary>Changes the foreground text color.</summary>
    Color,
    /// <summary>Changes the text background color.</summary>
    Background,
    /// <summary>Selects the font family.</summary>
    Font,
    /// <summary>Changes paragraph alignment.</summary>
    Align,
    /// <summary>Removes formatting from the selection.</summary>
    Clean,
    /// <summary>Creates or edits a hyperlink.</summary>
    Link,
    /// <summary>Embeds an image.</summary>
    Image,
    /// <summary>Inserts or edits a table.</summary>
    Table,
}

/// <summary>
/// QuillJS themes
/// </summary>
/// <seealso href="https://quilljs.com/docs/themes/"/>
public enum RichTextEditTheme
{
    /// <summary>
    /// Snow is a clean, flat toolbar theme.
    /// </summary>
    Snow,

    /// <summary>
    /// Bubble is a simple tooltip based theme.
    /// </summary>
    Bubble
}

/// <summary>
/// Dynamic reference type enumeration
/// </summary>
public enum DynamicReferenceType
{
    /// <summary>
    /// CSS stylesheet
    /// </summary>
    Stylesheet,

    /// <summary>
    /// Javascript
    /// </summary>
    Script
}