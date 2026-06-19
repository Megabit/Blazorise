#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a single zone inside the dock drop-zone compass.
/// </summary>
public partial class _DockCompassZoneRenderer : BaseComponent
{
    #region Constructors

    /// <summary>
    /// Default <see cref="_DockCompassZoneRenderer"/> constructor.
    /// </summary>
    public _DockCompassZoneRenderer()
    {
        IconClassBuilder = new( BuildIconClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        DirtyClasses();
        IconClassBuilder.Dirty();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockLayoutCompassZone( Zone.Zone, Active ) );
        builder.Append( ClassProvider.DockLayoutCompassZonePlacement( Zone.CompassZone ) );

        base.BuildClasses( builder );
    }

    private void BuildIconClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockLayoutCompassZoneIcon() );
    }

    #endregion

    #region Properties

    private bool Active => Layout?.ActiveDockZone == Zone.Zone && Layout?.ActiveDockCompassZoneKey == Zone.Key;

    private IconName CompassIconName
        => Zone.Zone switch
        {
            DockZone.Left => IconName.ArrowLeft,
            DockZone.Right => IconName.ArrowRight,
            DockZone.Top => IconName.ArrowUp,
            DockZone.Bottom => IconName.ArrowDown,
            _ => IconName.Square,
        };

    private string IconClassNames => IconClassBuilder.Class;

    protected ClassBuilder IconClassBuilder { get; private set; }

    /// <summary>
    /// Gets or sets the owner dock layout.
    /// </summary>
    [Parameter] public DockLayout Layout { get; set; }

    /// <summary>
    /// Gets or sets the compass zone to render.
    /// </summary>
    [Parameter] public DockLayout.DockCompassZoneInfo Zone { get; set; }

    #endregion
}