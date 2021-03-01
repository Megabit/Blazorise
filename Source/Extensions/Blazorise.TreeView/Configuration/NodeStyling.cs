namespace Blazorise.TreeView
{
    /// <summary>
    /// Helper class to override default node styling.
    /// </summary>
    public class NodeStyling
    {
        /// <summary>
        /// Custom classnames.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Custom styles.
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Custom text color.
        /// </summary>
        public TextColor TextColor { get; set; }

        /// <summary>
        /// Custom background color.
        /// </summary>
        public Background Background { get; set; }
    }
}
