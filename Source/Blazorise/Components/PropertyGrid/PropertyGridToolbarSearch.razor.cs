#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a property filter inside a <see cref="PropertyGridToolbar"/>.
/// </summary>
public partial class PropertyGridToolbarSearch : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridToolbarSearch() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the property search text.
    /// </summary>
    [Parameter] public string SearchText { get; set; }

    /// <summary>
    /// Occurs after the property search text changes.
    /// </summary>
    [Parameter] public EventCallback<string> SearchTextChanged { get; set; }

    /// <summary>
    /// Defines the search input placeholder.
    /// </summary>
    [Parameter] public string Placeholder { get; set; } = "Search";

    /// <summary>
    /// Defines the accessible search input label.
    /// </summary>
    [Parameter] public string AriaLabel { get; set; } = "Search properties";

    /// <summary>
    /// Defines the search icon.
    /// </summary>
    [Parameter] public object SearchIcon { get; set; } = IconName.Search;

    /// <summary>
    /// Defines whether search input changes are debounced.
    /// </summary>
    [Parameter] public bool Debounce { get; set; } = true;

    /// <summary>
    /// Defines the search debounce interval in milliseconds.
    /// </summary>
    [Parameter] public int DebounceInterval { get; set; } = 300;

    /// <summary>
    /// Defines the search editor size.
    /// </summary>
    [Parameter] public Size Size { get; set; } = Size.Small;

    #endregion
}