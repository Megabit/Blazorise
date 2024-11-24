#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Providers the necessary logic to open or close an animated component
/// </summary>
public interface IAnimatedComponent : IHideableComponent
{
    /// <summary>
    /// Gets or sets whether the component has any animations.
    /// </summary>
    public bool Animated { get; set; }

    /// <summary>
    /// Gets or sets the animation duration.
    /// </summary>
    public int AnimationDuration { get; set; }

    /// <summary>
    /// Starts the component animation. Should only be used internally.
    /// </summary>
    /// <param name="visible">Visibility flag.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task BeginAnimation( bool visible );

    /// <summary>
    /// Ends the component animation. Should only be used internally.
    /// </summary>
    /// <param name="visible">Visibility flag.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task EndAnimation( bool visible );
}