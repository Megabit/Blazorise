namespace Blazorise
{
    /// <summary>
    /// Determines the events that cause the tooltip to show.
    /// </summary>
    public enum TooltipTrigger
    {
        /// <summary>
        /// Tooltip will show on mouse enter and focus event (default option).
        /// </summary>
        MouseEnterFocus = 0,

        /// <summary>
        /// Tooltip will show on click event only.
        /// </summary>
        Click = 1,

        /// <summary>
        /// Tooltip will show on focus event only.
        /// </summary>
        Focus = 2,

        /// <summary>
        /// Tooltip will show on mouse enter and click event.
        /// </summary>
        MouseEnterClick = 3,
    }
}
