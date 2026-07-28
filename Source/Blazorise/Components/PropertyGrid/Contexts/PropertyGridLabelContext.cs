namespace Blazorise;

/// <summary>
/// Provides context for a property label template.
/// </summary>
public sealed class PropertyGridLabelContext
{
    internal PropertyGridLabelContext( PropertyGridProperty property )
    {
        Property = property;
    }

    /// <summary>
    /// Gets the property definition.
    /// </summary>
    public PropertyGridProperty Property { get; }

    /// <summary>
    /// Gets the property label.
    /// </summary>
    public string Label => Property.Label;
}