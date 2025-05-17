namespace Blazorise;

/// <summary>
/// Defines the rendering mode of the <see cref="Steps"/> component.
/// </summary>
public enum StepsRenderMode
{
    /// <summary>
    /// Always renders the steps html content to the DOM.
    /// </summary>
    Default,

    /// <summary>
    /// Lazy loads steps, meaning each step will only be rendered/loaded the first time it is visited.
    /// </summary>
    LazyLoad,

    /// <summary>
    /// Lazy loads steps everytime, meaning only the active step will have it's html rendered to the DOM.
    /// </summary>
    LazyReload,
}