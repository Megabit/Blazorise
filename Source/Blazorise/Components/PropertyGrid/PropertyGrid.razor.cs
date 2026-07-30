#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Presents groups of editable properties in a compact two-column layout.
/// </summary>
public partial class PropertyGrid : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGrid() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the provider class for the scrollable property viewport.
    /// </summary>
    protected string ViewportClassNames => ClassProvider.PropertyGridViewport();

    /// <summary>
    /// Defines the accessible property grid label.
    /// </summary>
    [Parameter] public string AriaLabel { get; set; } = "Properties";

    /// <summary>
    /// Specifies content rendered above the scrollable property viewport.
    /// </summary>
    [Parameter] public RenderFragment Toolbar { get; set; }

    /// <summary>
    /// Specifies the property groups to be rendered inside this <see cref="PropertyGrid"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Specifies content rendered below the scrollable property viewport.
    /// </summary>
    [Parameter] public RenderFragment Help { get; set; }

    #endregion
}