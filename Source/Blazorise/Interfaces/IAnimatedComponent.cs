#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Provides the necessary logic to open or close an animated component.
/// </summary>
public interface IAnimatedComponent : IHideableComponent
{
    /// <summary>
    /// Gets or sets whether the component has any animations.
    /// </summary>
    public bool Animated { get; set; }

    /// <summary>
    /// Gets or sets the animation duration, in milliseconds.
    /// </summary>
    public int AnimationDuration { get; set; }

    /// <summary>
    /// Initiates the component animation. This method is intended for internal framework use only and should not be called directly by user code.
    /// </summary>
    /// <param name="visible">A value indicating whether the component should be made visible or hidden.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task BeginAnimation( bool visible );

    /// <summary>
    /// Finalizes the component animation. This method is intended for internal framework use only and should not be called directly by user code.
    /// </summary>
    /// <param name="visible">A value indicating whether the component is visible or hidden.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task EndAnimation( bool visible );
}