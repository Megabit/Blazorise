using System.Threading.Tasks;

namespace Blazorise;

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