#region Using directives
using Blazorise.Utilities;
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

}