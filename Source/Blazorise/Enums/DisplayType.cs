namespace Blazorise
{
    /// <summary>
    /// The display property specifies the display behavior (the type of rendering box) of an element.
    /// </summary>
    public enum DisplayType
    {
        /// <summary>
        /// Display will not be applied, meaning an element will be visible.
        /// </summary>
        Always,

        /// <summary>
        /// Hides an element.
        /// </summary>
        None,

        /// <summary>
        /// Displays an element as a block element.
        /// </summary>
        /// <remarks>
        /// It starts on a new line, and takes up the whole width.
        /// </remarks>
        Block,

        /// <summary>
        /// Displays an element as an inline element.
        /// </summary>
        /// <remarks>
        /// Any height and width properties will have no effect.
        /// </remarks>
        Inline,

        /// <summary>
        /// Displays an element as an inline-level block container.
        /// </summary>
        /// <remarks>
        /// The element itself is formatted as an inline element, but you can apply height and width values
        /// </remarks>
        InlineBlock,

        /// <summary>
        /// Displays an element as a block-level flex container.
        /// </summary>
        Flex,

        /// <summary>
        /// Displays an element as an inline-level flex container.
        /// </summary>
        InlineFlex,

        /// <summary>
        /// Let the element behave like a table element.
        /// </summary>
        Table,

        /// <summary>
        /// Let the element behave like a tr element.
        /// </summary>
        TableRow,

        /// <summary>
        /// Let the element behave like a td element.
        /// </summary>
        TableCell,
    }
}
