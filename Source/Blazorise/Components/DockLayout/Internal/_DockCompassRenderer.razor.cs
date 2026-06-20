#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders the dock drop-zone compass.
/// </summary>
public partial class _DockCompassRenderer : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockLayoutCompass() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the layout render version.
    /// </summary>
    [Parameter] public int RenderVersion { get; set; }

    #endregion
}