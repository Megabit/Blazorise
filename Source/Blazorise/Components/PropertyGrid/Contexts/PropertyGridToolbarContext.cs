using System.Threading.Tasks;

namespace Blazorise;

/// <summary>
/// Provides context for a property grid toolbar template.
/// </summary>
public sealed class PropertyGridToolbarContext
{
    private readonly PropertyGridView owner;

    internal PropertyGridToolbarContext( PropertyGridView owner )
    {
        this.owner = owner;
        Categorized = new( owner, PropertyGridViewMode.Categorized );
        Alphabetical = new( owner, PropertyGridViewMode.Alphabetical );
    }

    /// <summary>
    /// Gets the active property arrangement.
    /// </summary>
    public PropertyGridViewMode ViewMode => owner.ViewMode;

    /// <summary>
    /// Gets the categorized button context.
    /// </summary>
    public PropertyGridViewModeContext Categorized { get; }

    /// <summary>
    /// Gets the alphabetical button context.
    /// </summary>
    public PropertyGridViewModeContext Alphabetical { get; }

    /// <summary>
    /// Changes the active property arrangement.
    /// </summary>
    public Task SetViewModeAsync( PropertyGridViewMode viewMode ) => owner.ChangeViewModeAsync( viewMode );
}