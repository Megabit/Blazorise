namespace Blazorise
{
    /// <summary>
    /// Options to override page progress appearance.
    /// </summary>
    public class PageProgressOptions
    {
        /// <summary>
        /// Type or color, of the page progress.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Creates the default progress options.
        /// </summary>
        /// <returns>Default progress options.</returns>
        public static PageProgressOptions Default => new()
        {
        };
    }
}
