namespace Blazorise
{
    /// <summary>
    /// Defines direction of flex items in a flex container.
    /// </summary>
    public enum FlexDirection
    {
        /// <summary>
        /// Direction will not be applied.
        /// </summary>
        Default,

        /// <summary>
        /// The flex container's main-axis is defined to be the same as the text direction. The main-start and main-end points are the same as the content direction.
        /// </summary>
        Row,

        /// <summary>
        /// The flex container's main-axis is the same as the block-axis. The main-start and main-end points are the same as the before and after points of the writing-mode.
        /// </summary>
        Column,

        /// <summary>
        /// Behaves the same as row but the main-start and main-end points are permuted.
        /// </summary>
        ReverseRow,

        /// <summary>
        /// Behaves the same as column but the main-start and main-end are permuted.
        /// </summary>
        ReverseColumn,
    }
}
