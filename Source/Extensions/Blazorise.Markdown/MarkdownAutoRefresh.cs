namespace Blazorise.Markdown
{
    /// <summary>
    /// Useful, when initializing the editor in a hidden DOM node. If set to { delay: 300 },
    /// it will check every 300 ms if the editor is visible and if positive, call CodeMirror's refresh().
    /// </summary>
    public class MarkdownAutoRefresh
    {
        public int Delay { get; set; } = 300;
    }
}
