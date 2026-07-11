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
        builder.Append( ClassProvider.DockLayoutCompassZone( Zone, Active ) );
        builder.Append( ClassProvider.DockLayoutCompassZonePlacement( CompassZone ) );

        base.BuildClasses( builder );
    }

    private void BuildIconClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockLayoutCompassZoneIcon() );
    }

    #endregion

    #region Properties

    private bool Active => Context?.ActiveDockZone == Zone && Context?.ActiveDockCompassZoneKey == ZoneKey;

    private IconName CompassIconName
        => Zone switch
        {
            DockZone.Left => IconName.ArrowLeft,
            DockZone.Right => IconName.ArrowRight,
            DockZone.Top => IconName.ArrowUp,
            DockZone.Bottom => IconName.ArrowDown,
            _ => IconName.Square,
        };

    private string IconClassNames => IconClassBuilder.Class;

    private ClassBuilder IconClassBuilder { get; set; }

    [CascadingParameter] internal DockLayoutContext Context { get; set; }

    /// <summary>
    /// Gets or sets the dock zone to render.
    /// </summary>
    [Parameter] public DockZone Zone { get; set; }

    /// <summary>
    /// Gets or sets the compass placement to render.
    /// </summary>
    [Parameter] public DockCompassZone CompassZone { get; set; }

    /// <summary>
    /// Gets or sets the compass zone key.
    /// </summary>
    [Parameter] public string ZoneKey { get; set; }

    #endregion
}