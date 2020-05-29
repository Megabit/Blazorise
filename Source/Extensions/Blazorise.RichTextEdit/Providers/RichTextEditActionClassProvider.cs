namespace Blazorise.RichTextEdit.Providers
{
    public sealed class RichTextEditActionClassProvider
    {
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
                null => "",
                _ => $"ql-{action.ToString().ToLower()}"
            };
    }
}