using System;

namespace Blazorise
{
    /// <summary>
    /// Defines the text alignment.
    /// </summary>
    public enum TextAlignment
    {
        /// <summary>
        /// No alignment will be applied.
        /// </summary>
        None,

        /// <summary>
        /// Aligns the text to the left.
        /// </summary>
        [Obsolete( "This parameter will soon be deprecated. Use Start instead." )]
        Left,

        /// <summary>
        /// Aligns the text to the right.
        /// </summary>
        [Obsolete( "This parameter will soon be deprecated. Use End instead." )]
        Right,

        /// <summary>
        /// Aligns the text to the start.
        /// </summary>
        Start,

        /// <summary>
        /// Aligns the text to the end.
        /// </summary>
        End,

        /// <summary>
        /// Centers the text.
        /// </summary>
        Center,

        /// <summary>
        /// Stretches the lines so that each line has equal width.
        /// </summary>
        Justified
    }
}
