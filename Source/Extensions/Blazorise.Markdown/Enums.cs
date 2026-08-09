namespace Blazorise.Markdown;

/// <summary>
/// Markdown toolbar actions.
/// </summary>
/// <seealso href="https://github.com/Ionaru/easy-markdown-editor#toolbar-icons"/> and
/// <seealso href="https://github.com/Ionaru/easy-markdown-editor/blob/master/src/js/easymde.js"/>
public enum MarkdownAction
{
    /// <summary>Wraps the selection in bold markers.</summary>
    Bold,
    /// <summary>Wraps the selection in italic markers.</summary>
    Italic,
    /// <summary>Applies strikethrough syntax.</summary>
    Strikethrough,
    /// <summary>Cycles the current heading level.</summary>
    Heading,
    /// <summary>Reduces the current heading level.</summary>
    HeadingSmaller,
    /// <summary>Increases the current heading level.</summary>
    HeadingBigger,
    /// <summary>Formats the line as a level-one heading.</summary>
    Heading1,
    /// <summary>Formats the line as a level-two heading.</summary>
    Heading2,
    /// <summary>Formats the line as a level-three heading.</summary>
    Heading3,
    /// <summary>Applies inline or fenced code syntax.</summary>
    Code,
    /// <summary>Prefixes the selection as a block quotation.</summary>
    Quote,
    /// <summary>Creates a bulleted list.</summary>
    UnorderedList,
    /// <summary>Creates a numbered list.</summary>
    OrderedList,
    /// <summary>Removes block-level Markdown syntax.</summary>
    CleanBlock,
    /// <summary>Inserts hyperlink syntax.</summary>
    Link,
    /// <summary>Inserts an image reference.</summary>
    Image,
    /// <summary>Inserts a Markdown table template.</summary>
    Table,
    /// <summary>Inserts a horizontal divider.</summary>
    HorizontalRule,
    /// <summary>Toggles rendered preview mode.</summary>
    Preview,
    /// <summary>Shows the editor and preview beside one another.</summary>
    SideBySide,
    /// <summary>Toggles fullscreen editing.</summary>
    Fullscreen,
    /// <summary>Opens the Markdown syntax guide.</summary>
    Guide,
    /// <summary>Reverts the latest edit.</summary>
    Undo,
    /// <summary>Reapplies the latest reverted edit.</summary>
    Redo,
    /// <summary>Starts image selection and upload.</summary>
    UploadImage,
}