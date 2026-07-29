namespace Blazorise;

/// <summary>
/// Provides context for a property grid help template.
/// </summary>
public sealed class PropertyGridHelpContext
{
    #region Constructors

    internal PropertyGridHelpContext( PropertyGridProperty property )
    {
        Property = property;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the selected property definition.
    /// </summary>
    public PropertyGridProperty Property { get; }

    /// <summary>
    /// Gets the selected property label.
    /// </summary>
    public string Label => Property.Label;

    /// <summary>
    /// Gets the selected property description.
    /// </summary>
    public string Description => Property.Description;

    #endregion
}