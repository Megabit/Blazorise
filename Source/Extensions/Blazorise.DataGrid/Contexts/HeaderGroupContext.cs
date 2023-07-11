namespace Blazorise.DataGrid;

/// <summary>
/// Context for the header group caption.
/// </summary>
public class HeaderGroupContext
{
    #region Constructors

    /// <summary>
    /// Constructor for context.
    /// </summary>
    /// <param name="headerGroupCaption">The defined header group caption.</param>
    public HeaderGroupContext( string headerGroupCaption )
    {
        HeaderGroupCaption = headerGroupCaption;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the defined header group caption.
    /// <para>Suggested usage: rendering content conditionally according to the group caption</para>
    /// </summary>
    public string HeaderGroupCaption { get; }

    #endregion
}