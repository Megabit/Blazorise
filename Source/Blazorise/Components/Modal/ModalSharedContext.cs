namespace Blazorise;

/// <summary>
/// Holds the shared context for all the modals in current application state.
/// </summary>
public class ModalSharedContext : IModalSharedContext
{
    /// <summary>
    /// Raises the modals <see cref="OpenIndex"/> and returns the new index.
    /// </summary>
    /// <returns>Returns the new open index.</returns>
    public int RaiseModalOpenIndex()
    {
        return ++OpenIndex;
    }

    /// <summary>
    /// Decrease the modals <see cref="OpenIndex"/> and returns the new index.
    /// </summary>
    /// <returns>Returns the new open index.</returns>
    public int DecreaseModalOpenIndex()
    {
        --OpenIndex;

        if ( OpenIndex < 0 )
            OpenIndex = 0;

        return OpenIndex;
    }

    /// <summary>
    /// Gets or sets the modal open index.
    /// </summary>
    public int OpenIndex { get; private set; }
}
