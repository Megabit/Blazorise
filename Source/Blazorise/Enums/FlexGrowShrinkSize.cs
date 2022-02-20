namespace Blazorise
{
    /// <summary>
    /// If all items have flex-grow set to 1, the remaining space in the container will be distributed equally to all children.
    /// </summary>
    public enum FlexGrowShrinkSize
    {
        /// <summary>
        /// No size will be applied.
        /// </summary>
        Default,

        /// <summary>
        /// Element uses a default space.
        /// </summary>
        Is0,

        /// <summary>
        /// Element uses all available space it can.
        /// </summary>
        Is1,
    }
}
