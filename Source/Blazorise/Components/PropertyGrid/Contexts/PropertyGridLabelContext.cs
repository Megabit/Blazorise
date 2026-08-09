namespace Blazorise;

/// <summary>
/// Provides context for a property label template.
/// </summary>
public sealed class PropertyGridLabelContext
{
    #region Constructors

    internal PropertyGridLabelContext( PropertyGridProperty property )
    {
        Property = property;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the property definition.
    /// </summary>
    public PropertyGridProperty Property { get; }

    /// <summary>
    /// Gets the property label.
    /// </summary>
    public string Label => Property.Label;

    /// <summary>
    /// Gets the property description.
    /// </summary>
    public string Description => Property.Description;

    #endregion
}