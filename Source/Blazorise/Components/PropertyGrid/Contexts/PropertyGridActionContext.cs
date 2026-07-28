using System.Threading.Tasks;

namespace Blazorise;

/// <summary>
/// Provides context for a property action template.
/// </summary>
public sealed class PropertyGridActionContext
{
    private readonly PropertyGridView owner;

    internal PropertyGridActionContext( PropertyGridView owner, PropertyGridProperty property )
    {
        this.owner = owner;
        Property = property;
    }

    /// <summary>
    /// Gets the property definition.
    /// </summary>
    public PropertyGridProperty Property { get; }

    /// <summary>
    /// Gets the action definition.
    /// </summary>
    public PropertyGridAction Action => Property.Action;

    /// <summary>
    /// Invokes the property action.
    /// </summary>
    public Task InvokeAsync() => owner.InvokeActionAsync( Property );
}