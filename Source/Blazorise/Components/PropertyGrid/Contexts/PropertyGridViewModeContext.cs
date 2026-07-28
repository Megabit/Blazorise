#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Provides context for a property grid view mode button template.
/// </summary>
public sealed class PropertyGridViewModeContext
{
    #region Members

    private readonly PropertyGridView owner;

    #endregion

    #region Constructors

    internal PropertyGridViewModeContext( PropertyGridView owner, PropertyGridViewMode viewMode )
    {
        this.owner = owner;
        ViewMode = viewMode;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Activates the represented arrangement.
    /// </summary>
    public Task Activate() => owner.ChangeViewModeAsync( ViewMode );

    #endregion

    #region Properties

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

    #endregion
}