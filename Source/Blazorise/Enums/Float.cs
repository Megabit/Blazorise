using System;

namespace Blazorise
{
    /// <summary>
    /// Floats an element to the left or right, or disable floating.
    /// </summary>
    public enum Float
    {
        /// <summary>
        /// Don't float on all viewport sizes.
        /// </summary>
        None,

        /// <summary>
        /// Float left on all viewport sizes.
        /// </summary>
        [Obsolete( "This parameter will soon be deprecated. Use Start instead." )]
        Left,

        /// <summary>
        /// Float right on all viewport sizes.
        /// </summary>
        [Obsolete( "This parameter will soon be deprecated. Use End instead." )]
        Right,

        /// <summary>
        /// Float start on all viewport sizes.
        /// </summary>
        Start,

        /// <summary>
        /// Float end on all viewport sizes.
        /// </summary>
        End,
    }
}
