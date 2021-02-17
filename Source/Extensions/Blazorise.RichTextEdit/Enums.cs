namespace Blazorise.RichTextEdit
{
    /// <summary>
    /// QuillJS toolbar actions
    /// </summary>
    /// <seealso href="https://quilljs.com/docs/modules/toolbar/"/>
    public enum RichTextEditAction
    {
        Bold,
        Italic,
        Underline,
        Strike,
        Blockquote,
        CodeBlock,
        Header,
        List,
        Script,
        Indent,
        Direction,
        Size,
        Color,
        Background,
        Font,
        Align,
        Clean,
        Link,
        Image
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
}