#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Provides context for a property grid toolbar template.
/// </summary>
public sealed class PropertyGridToolbarContext
{
    #region Members

    private readonly PropertyGridView owner;

    #endregion

    #region Constructors

    internal PropertyGridToolbarContext( PropertyGridView owner )
    {
        this.owner = owner;
        Categorized = new( owner, PropertyGridViewMode.Categorized );
        Alphabetical = new( owner, PropertyGridViewMode.Alphabetical );
        Search = new( owner );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Changes the active property arrangement.
    /// </summary>
    public Task SetViewMode( PropertyGridViewMode viewMode ) => owner.ChangeViewModeAsync( viewMode );

    /// <summary>
    /// Changes the property search text.
    /// </summary>
    public Task SetSearchText( string searchText ) => owner.ChangeSearchTextAsync( searchText );

    #endregion

    #region Properties

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
    /// Gets the property search context.
    /// </summary>
    public PropertyGridSearchContext Search { get; }

    /// <summary>
    /// Gets the current property search text.
    /// </summary>
    public string SearchText => owner.SearchText;

    #endregion
}