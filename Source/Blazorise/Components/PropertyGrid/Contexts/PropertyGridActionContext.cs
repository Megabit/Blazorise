#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Provides context for a property action template.
/// </summary>
public sealed class PropertyGridActionContext
{
    #region Members

    private readonly PropertyGridView owner;

    #endregion

    #region Constructors

    internal PropertyGridActionContext( PropertyGridView owner, PropertyGridProperty property )
    {
        this.owner = owner;
        Property = property;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invokes the property action.
    /// </summary>
    public Task Invoke() => owner.InvokeActionAsync( Property );

    #endregion

    #region Properties

    /// <summary>
    /// Gets the property definition.
    /// </summary>
    public PropertyGridProperty Property { get; }

    /// <summary>
    /// Gets the action definition.
    /// </summary>
    public PropertyGridAction Action => Property.Action;

    #endregion
}