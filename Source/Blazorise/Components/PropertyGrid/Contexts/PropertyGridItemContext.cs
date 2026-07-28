using System.Threading.Tasks;

namespace Blazorise;

/// <summary>
/// Provides context for a property item template.
/// </summary>
public sealed class PropertyGridItemContext
{
    private readonly PropertyGridView owner;

    internal PropertyGridItemContext( PropertyGridView owner, PropertyGridProperty property )
    {
        this.owner = owner;
        Property = property;
    }

    /// <summary>
    /// Gets the property definition.
    /// </summary>
    public PropertyGridProperty Property { get; }

    /// <summary>
    /// Gets the current property value.
    /// </summary>
    public object Value => Property.Value;

    /// <summary>
    /// Gets whether the property represents multiple different values.
    /// </summary>
    public bool Mixed => Property.Mixed;

    /// <summary>
    /// Gets the property label context.
    /// </summary>
    public PropertyGridLabelContext Label => new( Property );

    /// <summary>
    /// Gets the property editor context.
    /// </summary>
    public PropertyGridEditorContext Editor => new( owner, Property );

    /// <summary>
    /// Gets the property action context when an action is defined.
    /// </summary>
    public PropertyGridActionContext Action => Property.Action is null ? null : new( owner, Property );

    /// <summary>
    /// Reports a new property value.
    /// </summary>
    public Task SetValueAsync<TValue>( TValue value ) => owner.ChangeValueAsync( Property, value );

    /// <summary>
    /// Invokes the property action.
    /// </summary>
    public Task InvokeActionAsync() => owner.InvokeActionAsync( Property );
}