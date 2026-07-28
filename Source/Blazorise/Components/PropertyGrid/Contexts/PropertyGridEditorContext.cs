#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Provides context for a property editor template.
/// </summary>
public sealed class PropertyGridEditorContext
{
    #region Members

    private readonly PropertyGridView owner;

    #endregion

    #region Constructors

    internal PropertyGridEditorContext( PropertyGridView owner, PropertyGridProperty property )
    {
        this.owner = owner;
        Property = property;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the current property value as the requested type.
    /// </summary>
    public TValue GetValue<TValue>() => (TValue)Value;

    /// <summary>
    /// Reports a new property value.
    /// </summary>
    public Task SetValueAsync<TValue>( TValue value ) => owner.ChangeValueAsync( Property, value );

    #endregion

    #region Properties

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

    #endregion
}