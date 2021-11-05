namespace Blazorise
{
    /// <summary>
    /// Positioning of the caret on click.
    /// </summary>
    public enum InputMaskCaretPosition
    {
        /// <summary>
        /// Nothing will happen.
        /// </summary>
        None,

        /// <summary>
        /// Based on the last valid position (default).
        /// </summary>
        LastValidPosition,

        /// <summary>
        /// Position caret to radixpoint on initial click.
        /// </summary>
        RadixFocus,

        /// <summary>
        /// Select the whole input.
        /// </summary>
        Select,

        /// <summary>
        /// Ignore the click and continue the mask.
        /// </summary>
        Ignore,
    }
}
