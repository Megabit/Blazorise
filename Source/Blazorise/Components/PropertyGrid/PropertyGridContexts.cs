using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazorise;

/// <summary>
/// Provides context for a property grid group template.
/// </summary>
public sealed class PropertyGridGroupContext
{
    private readonly PropertyGridView owner;

    internal PropertyGridGroupContext( PropertyGridView owner, PropertyGridGroupDefinition group )
    {
        this.owner = owner;
        Group = group;
    }

    /// <summary>
    /// Gets the group definition.
    /// </summary>
    public PropertyGridGroupDefinition Group { get; }

    /// <summary>
    /// Gets contexts for the visible properties in the group.
    /// </summary>
    public IEnumerable<PropertyGridItemContext> Properties
        => Group.Properties.Where( property => property.Visible ).Select( property => new PropertyGridItemContext( owner, property ) );
}

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

/// <summary>
/// Provides context for a property editor template.
/// </summary>
public sealed class PropertyGridEditorContext
{
    private readonly PropertyGridView owner;

    internal PropertyGridEditorContext( PropertyGridView owner, PropertyGridProperty property )
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
    /// Gets the current property value as the requested type.
    /// </summary>
    public TValue GetValue<TValue>() => (TValue)Value;

    /// <summary>
    /// Reports a new property value.
    /// </summary>
    public Task SetValueAsync<TValue>( TValue value ) => owner.ChangeValueAsync( Property, value );
}

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

/// <summary>
/// Provides context for a property grid toolbar template.
/// </summary>
public sealed class PropertyGridToolbarContext
{
    private readonly PropertyGridView owner;

    internal PropertyGridToolbarContext( PropertyGridView owner )
    {
        this.owner = owner;
    }

    /// <summary>
    /// Gets the active property arrangement.
    /// </summary>
    public PropertyGridViewMode ViewMode => owner.ViewMode;

    /// <summary>
    /// Gets the categorized button context.
    /// </summary>
    public PropertyGridViewModeContext Categorized => new( owner, PropertyGridViewMode.Categorized );

    /// <summary>
    /// Gets the alphabetical button context.
    /// </summary>
    public PropertyGridViewModeContext Alphabetical => new( owner, PropertyGridViewMode.Alphabetical );

    /// <summary>
    /// Changes the active property arrangement.
    /// </summary>
    public Task SetViewModeAsync( PropertyGridViewMode viewMode ) => owner.ChangeViewModeAsync( viewMode );
}

/// <summary>
/// Provides context for a property grid view mode button template.
/// </summary>
public sealed class PropertyGridViewModeContext
{
    private readonly PropertyGridView owner;

    internal PropertyGridViewModeContext( PropertyGridView owner, PropertyGridViewMode viewMode )
    {
        this.owner = owner;
        ViewMode = viewMode;
    }

    /// <summary>
    /// Gets the property arrangement represented by the button.
    /// </summary>
    public PropertyGridViewMode ViewMode { get; }

    /// <summary>
    /// Gets whether the represented arrangement is active.
    /// </summary>
    public bool Active => owner.ViewMode == ViewMode;

    /// <summary>
    /// Gets the default icon for the represented arrangement.
    /// </summary>
    public IconName Icon => owner.GetViewModeIcon( ViewMode );

    /// <summary>
    /// Gets the accessible title for the represented arrangement.
    /// </summary>
    public string Title => owner.GetViewModeTitle( ViewMode );

    /// <summary>
    /// Activates the represented arrangement.
    /// </summary>
    public Task ActivateAsync() => owner.ChangeViewModeAsync( ViewMode );
}
