#region Using directives
using System;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent bar collapse builders.
/// </summary>
public interface IFluentBarCollapseMode
{
    /// <summary>
    /// Builds the classnames based on bar collapse rules.
    /// </summary>
    /// <param name="classProvider">Currently used class provider.</param>
    /// <returns>List of classnames for the given rules and the class provider.</returns>
    string Class( IClassProvider classProvider );
}

/// <summary>
/// Breakpoints allowed for bar collapse.
/// </summary>
public interface IFluentBarCollapseModeOnBreakpoint :
    IFluentDisplay
{
    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    IFluentBarCollapseModeWithMode OnMobile { get; }

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    IFluentBarCollapseModeWithMode OnTablet { get; }

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    IFluentBarCollapseModeWithMode OnDesktop { get; }

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    IFluentBarCollapseModeWithMode OnWidescreen { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    IFluentBarCollapseModeWithMode OnFullHD { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra extra large).
    /// </summary>
    IFluentBarCollapseModeWithMode OnQuadHD { get; }
}

/// <summary>
/// Modes allowed for bar collapse.
/// </summary>
public interface IFluentBarCollapseModeWithMode
{
    /// <summary>
    /// Collapse will hide on breakpoint.
    /// </summary>
    IFluentBarCollapseModeOnBreakpoint Hide { get; }

    /// <summary>
    /// Collapse will go become smaller on breakpoint.
    /// </summary>
    IFluentBarCollapseModeOnBreakpoint Small { get; }
}

/// <summary>
/// Default implementation of fluent bar collapse builder.
/// </summary>
public class FluentBarCollapseMode :
    IFluentBarCollapseMode,
    IFluentBarCollapseModeOnBreakpoint,
    IFluentBarCollapseModeWithMode,
    IUtilityTargeted
{
    #region Members

    private static readonly FluentUtilityBuilderCache<FluentBarCollapseMode> cachedOperations = new( static ( _, cache ) => new() { cache = cache }, static builder => ref builder.cache );

    private FluentUtilityState<FluentBarCollapseMode> cache;

    private UtilityTarget? utilityTarget;

    private class BarCollapseModeDefinition
    {
        public Breakpoint Breakpoint { get; set; }

        public BarCollapseMode Mode { get; set; }
    }

    private BarCollapseModeDefinition currentBarCollapseMode;

    #endregion

    #region Methods

    internal static FluentBarCollapseMode CreateCached() => cachedOperations.CreateRoot();

    /// <inheritdoc/>
    public string Class( IClassProvider classProvider )
    {
        if ( cache.IsCached )
        {
            return cache.GetClass( this, classProvider, static ( builder, provider ) => cachedOperations.Build( builder ).Class( (IClassProvider)provider ) );
        }

        throw new NotImplementedException();
    }

    /// <summary>
    /// Starts the new mode.
    /// </summary>
    /// <param name="mode">Mode to start.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentBarCollapseModeOnBreakpoint WithMode( BarCollapseMode mode )
    {
        if ( cache.IsCached )
        {
            return cachedOperations.With( this, cache, static ( builder, value, _ ) => builder.WithMode( (BarCollapseMode)value ), value: (int)mode );
        }

        var barCollapseModeDefinition = new BarCollapseModeDefinition { Breakpoint = Breakpoint.None };

        currentBarCollapseMode = barCollapseModeDefinition;

        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="breakpoint"></param>
    /// <returns>Next rule reference.</returns>
    public IFluentBarCollapseModeWithMode WithBreakpoint( Breakpoint breakpoint )
    {
        if ( cache.IsCached )
        {
            return cachedOperations.With( this, cache, static ( builder, value, _ ) => builder.WithBreakpoint( (Breakpoint)value ), value: (int)breakpoint );
        }

        currentBarCollapseMode.Breakpoint = breakpoint;

        return this;
    }

    private IFluentDisplay WithUtilityTarget( UtilityTarget target )
    {
        if ( cache.IsCached )
        {
            return cachedOperations.With( this, cache, static ( builder, value, _ ) => builder.UtilityTarget = (UtilityTarget)value, value: (int)target, target: target );
        }

        UtilityTarget = target;

        return this;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the utility target override.
    /// </summary>
    public UtilityTarget? UtilityTarget
    {
        get => cache.IsCached ? cache.Target : utilityTarget;
        set
        {
            if ( cache.IsCached )
            {
                throw new InvalidOperationException( "Cached fluent utilities are immutable. Use OnSelf or OnWrapper to change the utility target." );
            }

            utilityTarget = value;
        }
    }

    /// <summary>
    /// Targets the utility output to the component element.
    /// </summary>
    public IFluentDisplay OnSelf => WithUtilityTarget( Blazorise.UtilityTarget.Self );

    /// <summary>
    /// Targets the utility output to a wrapper element.
    /// </summary>
    public IFluentDisplay OnWrapper => WithUtilityTarget( Blazorise.UtilityTarget.Wrapper );

    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    public IFluentBarCollapseModeWithMode OnMobile => WithBreakpoint( Breakpoint.Mobile );

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    public IFluentBarCollapseModeWithMode OnTablet => WithBreakpoint( Breakpoint.Tablet );

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    public IFluentBarCollapseModeWithMode OnDesktop => WithBreakpoint( Breakpoint.Desktop );

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    public IFluentBarCollapseModeWithMode OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    public IFluentBarCollapseModeWithMode OnFullHD => WithBreakpoint( Breakpoint.FullHD );

    /// <summary>
    /// Breakpoint on large desktops (extra extra large).
    /// </summary>
    public IFluentBarCollapseModeWithMode OnQuadHD => WithBreakpoint( Breakpoint.QuadHD );

    /// <summary>
    /// 
    /// </summary>
    public IFluentBarCollapseModeOnBreakpoint Hide => WithMode( BarCollapseMode.Hide );

    /// <summary>
    /// 
    /// </summary>
    public IFluentBarCollapseModeOnBreakpoint Small => WithMode( BarCollapseMode.Small );

    #endregion
}

/// <summary>
/// Starts the new bar collapse mode.
/// </summary>
public static class BarCollapseModee
{
    private static readonly FluentBarCollapseMode cached = FluentBarCollapseMode.CreateCached();

    /// <summary>
    /// Collapse will hide on breakpoint.
    /// </summary>
    public static IFluentBarCollapseModeOnBreakpoint Hide => cached.Hide;

    /// <summary>
    /// Collapse will go become smaller on breakpoint.
    /// </summary>
    public static IFluentBarCollapseModeOnBreakpoint Small => cached.Small;
}