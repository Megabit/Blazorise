namespace Blazorise;

/// <summary>
/// Holds the shared context for all the modals in current application state.
/// </summary>
public interface IModalSharedContext
{
    /// <summary>
    /// Raises the modals <see cref="OpenIndex"/> and returns the new index.
    /// </summary>
    /// <returns>Returns the new open index.</returns>
    int RaiseModalOpenIndex();

    /// <summary>
    /// Decrease the modals <see cref="OpenIndex"/> and returns the new index.
    /// </summary>
    /// <returns>Returns the new open index.</returns>
    int DecreaseModalOpenIndex();

    /// <summary>
    /// Gets or sets the modal open index.
    /// </summary>
    int OpenIndex { get; }
}