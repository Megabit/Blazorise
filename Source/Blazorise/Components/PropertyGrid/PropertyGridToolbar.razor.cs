#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders commands and search controls for a <see cref="PropertyGrid"/>.
/// </summary>
public partial class PropertyGridToolbar : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridToolbar() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the accessible toolbar label.
    /// </summary>
    [Parameter] public string AriaLabel { get; set; } = "Property grid controls";

    /// <summary>
    /// Specifies the content rendered inside the toolbar.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}