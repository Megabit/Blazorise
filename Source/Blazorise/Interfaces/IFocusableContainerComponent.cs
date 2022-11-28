namespace Blazorise;

/// <summary>
/// Base component that is used as a container for focusable components.
/// </summary>
public interface IFocusableContainerComponent
{
    /// <summary>
    /// Notifies the container component that its <see cref="IFocusableComponent"/> is initialized.
    /// </summary>
    /// <param name="focusableComponent">Nested focusable component.</param>
    void NotifyFocusableComponentInitialized( IFocusableComponent focusableComponent );

    /// <summary>
    /// Notifies the container component that its <see cref="IFocusableComponent"/> is removed.
    /// </summary>
    /// <param name="focusableComponent">Nested focusable component.</param>
    void NotifyFocusableComponentRemoved( IFocusableComponent focusableComponent );
}