namespace Blazorise.RichTextEdit.Providers;

/// <summary>
/// Class provider for <see cref="RichTextEditAction"/>
/// </summary>
public sealed class RichTextEditActionClassProvider
{
    /// <summary>
    /// Get the QuillJS class for the specified action.
    /// </summary>
    public static string Class( RichTextEditAction? action ) =>
        action switch
        {
            RichTextEditAction.Bold => "ql-bold",
            RichTextEditAction.Italic => "ql-italic",
            RichTextEditAction.Underline => "ql-underline",
            RichTextEditAction.Strike => "ql-strike",
            RichTextEditAction.Blockquote => "ql-blockquote",
            RichTextEditAction.CodeBlock => "ql-code-block",
            RichTextEditAction.Header => "ql-header",
            RichTextEditAction.List => "ql-list",
            RichTextEditAction.Script => "ql-script",
            RichTextEditAction.Indent => "ql-indent",
            RichTextEditAction.Direction => "ql-direction",
            RichTextEditAction.Size => "ql-size",
            RichTextEditAction.Color => "ql-color",
            RichTextEditAction.Background => "ql-background",
            RichTextEditAction.Font => "ql-font",
            RichTextEditAction.Align => "ql-align",
            RichTextEditAction.Clean => "ql-clean",
            RichTextEditAction.Link => "ql-link",
            RichTextEditAction.Image => "ql-image",
            RichTextEditAction.Table => "ql-table-better",
            null => "",
            _ => $"ql-{action.ToString().ToLower()}"
        };
}