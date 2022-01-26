using System.Collections.Generic;

namespace Blazorise.Markdown.Providers
{
    /// <summary>
    /// Provider for the <see cref="MarkdownAction"/>.
    /// </summary>
    internal static class MarkdownActionProvider
    {
        /// <summary>
        /// Gets the Markdown class for the specified action.
        /// </summary>
        public static string Class( MarkdownAction? action, string name )
        {
            if ( !string.IsNullOrWhiteSpace( name ) )
                return name;

            return action switch
            {
                MarkdownAction.Bold => "bold",
                MarkdownAction.Italic => "italic",
                MarkdownAction.Strikethrough => "strikethrough",
                MarkdownAction.Heading => "heading",
                MarkdownAction.HeadingSmaller => "heading-smaller",
                MarkdownAction.HeadingBigger => "heading-bigger",
                MarkdownAction.Heading1 => "heading-1",
                MarkdownAction.Heading2 => "heading-2",
                MarkdownAction.Heading3 => "heading-3",
                MarkdownAction.Code => "code",
                MarkdownAction.Quote => "quote",
                MarkdownAction.UnorderedList => "unordered-list",
                MarkdownAction.OrderedList => "ordered-list",
                MarkdownAction.CleanBlock => "clean-block",
                MarkdownAction.Link => "link",
                MarkdownAction.Image => "image",
                MarkdownAction.Table => "table",
                MarkdownAction.HorizontalRule => "horizontal-rule",
                MarkdownAction.Preview => "preview",
                MarkdownAction.SideBySide => "side-by-side",
                MarkdownAction.Fullscreen => "fullscreen",
                MarkdownAction.Guide => "guide",
                MarkdownAction.Undo => "undo",
                MarkdownAction.Redo => "redo",
                MarkdownAction.UploadImage => "upload-image",
                _ => name
            };
        }

        /// <summary>
        /// Gets the Markdown event name for the specified action.
        /// </summary>
        public static string Event( MarkdownAction? action ) =>
            action switch
            {
                MarkdownAction.Bold => "toggleBold",
                MarkdownAction.Italic => "toggleItalic",
                MarkdownAction.Strikethrough => "toggleStrikethrough",
                MarkdownAction.Heading => "toggleHeadingSmaller",
                MarkdownAction.HeadingSmaller => "toggleHeadingSmaller",
                MarkdownAction.HeadingBigger => "toggleHeadingBigger",
                MarkdownAction.Heading1 => "toggleHeading1",
                MarkdownAction.Heading2 => "toggleHeading2",
                MarkdownAction.Heading3 => "toggleHeading3",
                MarkdownAction.Code => "toggleCodeBlock",
                MarkdownAction.Quote => "toggleBlockquote",
                MarkdownAction.UnorderedList => "toggleUnorderedList",
                MarkdownAction.OrderedList => "toggleOrderedList",
                MarkdownAction.CleanBlock => "cleanBlock",
                MarkdownAction.Link => "drawLink",
                MarkdownAction.Image => "drawImage",
                MarkdownAction.Table => "drawTable",
                MarkdownAction.HorizontalRule => "drawHorizontalRule",
                MarkdownAction.Preview => "togglePreview",
                MarkdownAction.SideBySide => "toggleSideBySide",
                MarkdownAction.Fullscreen => "toggleFullScreen",
                MarkdownAction.Guide => "https://www.markdownguide.org/basic-syntax/",
                MarkdownAction.Undo => "undo",
                MarkdownAction.Redo => "redo",
                MarkdownAction.UploadImage => "drawUploadedImage",
                _ => null
            };

        /// <summary>
        /// Gets the Markdown icon class name for the specified action.
        /// </summary>
        public static string IconClass( MarkdownAction? action, string icon )
        {
            if ( !string.IsNullOrWhiteSpace( icon ) )
                return icon;

            return action switch
            {
                MarkdownAction.Bold => "fa fa-bold",
                MarkdownAction.Italic => "fa fa-italic",
                MarkdownAction.Strikethrough => "fa fa-strikethrough",
                MarkdownAction.Heading => "fa fa-header",
                MarkdownAction.HeadingSmaller => "fa fa-header",
                MarkdownAction.HeadingBigger => "fa fa-lg fa-header",
                MarkdownAction.Heading1 => "fa fa-header header-1",
                MarkdownAction.Heading2 => "fa fa-header header-2",
                MarkdownAction.Heading3 => "fa fa-header header-3",
                MarkdownAction.Code => "fa fa-code",
                MarkdownAction.Quote => "fa fa-quote-left",
                MarkdownAction.UnorderedList => "fa fa-list-ul",
                MarkdownAction.OrderedList => "fa fa-list-ol",
                MarkdownAction.CleanBlock => "fa fa-eraser",
                MarkdownAction.Link => "fa fa-link",
                MarkdownAction.Image => "fa fa-picture-o",
                MarkdownAction.Table => "fa fa-table",
                MarkdownAction.HorizontalRule => "fa fa-minus",
                MarkdownAction.Preview => "fa fa-eye no-disable",
                MarkdownAction.SideBySide => "fa fa-columns no-disable no-mobile",
                MarkdownAction.Fullscreen => "fa fa-arrows-alt no-disable no-mobile",
                MarkdownAction.Guide => "fa fa-question-circle",
                MarkdownAction.Undo => "fa fa-undo",
                MarkdownAction.Redo => "fa fa-repeat fa-redo",
                MarkdownAction.UploadImage => "fa fa-image",
                _ => icon
            };
        }

        public static IEnumerable<object> Serialize( List<MarkdownToolbarButton> buttons )
        {
            foreach ( var button in buttons )
            {
                if ( button.Separator )
                    yield return "|";

                yield return new
                {
                    Name = Class( button.Action, button.Name ),
                    Value = button.Value,
                    Action = Event( button.Action ),
                    ClassName = IconClass( button.Action, button.Icon ),
                    Title = button.Title,
                };
            }
        }
    }
}
