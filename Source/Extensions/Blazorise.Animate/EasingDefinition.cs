namespace Blazorise.Animate;

/// <summary>
/// Defines an easing from a name and a runtime value.
/// </summary>
public class EasingDefinition : IEasingDefinition
{
    /// <summary>
    /// Initializes a new instance of the easing definition.
    /// </summary>
    /// <param name="name">Easing name.</param>
    /// <param name="value">Runtime easing value.</param>
    public EasingDefinition( string name, object value )
    {
        Name = name;
        Value = value;
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public object Value { get; }
}