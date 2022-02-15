namespace Blazorise
{
    /// <summary>
    /// Defines the default behavior for how flex items are laid out along the cross axis on the current line.
    /// </summary>
    public enum FlexAlignItems
    {
        /// <summary>
        /// Align-items will not be applied.
        /// </summary>
        Default,

        /// <summary>
        /// Items are placed at the start of the cross axis. The difference between these is subtle,
        /// and is about respecting the flex-direction rules or the writing-mode rules.
        /// </summary>
        Start,

        /// <summary>
        /// Items are placed at the end of the cross axis. The difference again is subtle and is about
        /// respecting flex-direction rules vs. writing-mode rules.
        /// </summary>
        End,

        /// <summary>
        /// Items are centered in the cross-axis.
        /// </summary>
        Center,

        /// <summary>
        /// Items are aligned such as their baselines align.
        /// </summary>
        Baseline,

        /// <summary>
        /// Stretch to fill the container (still respect min-width/max-width).
        /// </summary>
        Stretch,
    }
}
