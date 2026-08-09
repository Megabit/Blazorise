#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Provides context for a property grid search template.
/// </summary>
public sealed class PropertyGridSearchContext
{
    #region Members

    private readonly PropertyGridView owner;

    #endregion

    #region Constructors

    internal PropertyGridSearchContext( PropertyGridView owner )
    {
        this.owner = owner;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Changes the property search text.
    /// </summary>
    public Task SetSearchText( string searchText ) => owner.ChangeSearchTextAsync( searchText );

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current property search text.
    /// </summary>
    public string SearchText => owner.SearchText;

    /// <summary>
    /// Gets the search input placeholder.
    /// </summary>
    public string Placeholder => owner.SearchPlaceholder;

    #endregion
}