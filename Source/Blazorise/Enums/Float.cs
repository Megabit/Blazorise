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
        Default,

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
