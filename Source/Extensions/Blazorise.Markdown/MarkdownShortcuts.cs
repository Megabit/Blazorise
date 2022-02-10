namespace Blazorise.Markdown
{
    /// <summary>
    /// Keyboard shortcuts associated with this instance.
    /// Defaults to the array of <see href="https://github.com/Ionaru/easy-markdown-editor#keyboard-shortcuts">shortcuts</see>.
    /// </summary>
    public class MarkdownShortcuts
    {
        public string CleanBlock { get; set; } = "Cmd-E";

        public string DrawImage { get; set; } = "Cmd-Alt-I";

        public string DrawLink { get; set; } = "Cmd-K";

        public string ToggleBlockquote { get; set; } = "Cmd-'";

        public string ToggleBold { get; set; } = "Cmd-B";

        public string ToggleCodeBlock { get; set; } = "Cmd-Alt-C";

        public string ToggleFullScreen { get; set; } = "F11";

        public string ToggleHeadingBigger { get; set; } = "Shift-Cmd-H";

        public string ToggleHeadingSmaller { get; set; } = "Cmd-H";

        public string ToggleItalic { get; set; } = "Cmd-I";

        public string ToggleOrderedList { get; set; } = "Cmd-Alt-L";

        public string TogglePreview { get; set; } = "Cmd-P";

        public string ToggleSideBySide { get; set; } = "F9";

        public string ToggleUnorderedList { get; set; } = "Cmd-L";

        public string ToggleStrikethrough { get; set; } = "F4";
    }
}
