#region Using directives
using System;
#endregion

namespace Blazorise.Animate
{
    /// <summary>
    /// Defines all the animations settings.
    /// </summary>
    public class AnimateOptions
    {
        /// <summary>
        /// Gets or sets the animation effect.
        /// </summary>
        /// <remarks>
        /// The list of all supported animations can be found in <see cref="Animations"/> class.
        /// </remarks>
        public IAnimation Animation { get; set; }

        /// <summary>
        /// Gets or sets the easing effect.
        /// </summary>
        /// <remarks>
        /// The list of all supported easings can be found in <see cref="Easings"/> class.
        /// </remarks>
        public IEasing Easing { get; set; }

        /// <summary>
        /// Gets os sets the total duration of the animation.
        /// </summary>
        /// <remarks>
        /// Values from 0 to 3000, with step 50ms.
        /// </remarks>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets os sets the delay of the animation before it runs automatically, or manually.
        /// </summary>
        /// <remarks>
        /// Values from 0 to 3000, with step 50ms.
        /// </remarks>
        public TimeSpan Delay { get; set; }

        /// <summary>
        /// Whether elements should animate out while scrolling past them.
        /// </summary>
        public bool Mirror { get; set; }

        /// <summary>
        /// Whether animation should happen only once - while scrolling down.
        /// </summary>
        public bool Once { get; set; }
    }
}
