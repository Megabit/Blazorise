#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Renders the dock drop-zone compass.
/// </summary>
public partial class _DockCompassRenderer : _BaseDockRenderer
{
    #region Methods

    /// <inheritdoc/>
    private protected override bool IsAffected( DockLayoutChange change )
        => change.Kind == DockLayoutChangeKind.Compass;

    /// <inheritdoc/>
    private protected override void OnDockLayoutChanged( DockLayoutChange change )
        => DirtyStyles();

    private bool IsActive( DockLayout.DockCompassZoneInfo zone )
        => Context.ActiveDockZone == zone.Zone && Context.ActiveDockCompassZoneKey == zone.Key;

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockLayoutCompass() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"left:{Context.DockCompassX}px" );
        builder.Append( $"top:{Context.DockCompassY}px" );

        base.BuildStyles( builder );
    }

    #endregion

}