#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Providers the necessary logic to open or close an animated component
    /// </summary>
    public interface IAnimatedComponent
    {
        /// <summary>
        /// Tracks whether the component has any animations.
        /// </summary>
        public bool IsAnimated { get; }

        /// <summary>
        /// Tracks the animation duration.
        /// </summary>
        public int AnimationDuration { get; }

        /// <summary>
        /// Animates the component.
        /// </summary>
        /// <param name="visible"></param>
        /// <returns></returns>
        public Task Animate( bool visible );

    }
}
