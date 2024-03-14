#region Using directives
using System;
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
    IFluentBarCollapseModeWithMode
{
    #region Members

    private class BarCollapseModeDefinition
    {
        public Breakpoint Breakpoint { get; set; }

        public BarCollapseMode Mode { get; set; }
    }

    private BarCollapseModeDefinition currentBarCollapseMode;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public string Class( IClassProvider classProvider )
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Starts the new mode.
    /// </summary>
    /// <param name="mode">Mode to start.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentBarCollapseModeOnBreakpoint WithMode( BarCollapseMode mode )
    {
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
        currentBarCollapseMode.Breakpoint = breakpoint;

        return this;
    }

    #endregion

    #region Properties

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
    public IFluentBarCollapseModeOnBreakpoint Hide { get { return WithMode( BarCollapseMode.Hide ); } }

    /// <summary>
    /// 
    /// </summary>
    public IFluentBarCollapseModeOnBreakpoint Small { get { return WithMode( BarCollapseMode.Small ); } }

    #endregion
}

/// <summary>
/// Starts the new bar collapse mode.
/// </summary>
public static class BarCollapseModee
{
    /// <summary>
    /// Collapse will hide on breakpoint.
    /// </summary>
    public static IFluentBarCollapseModeOnBreakpoint Hide => new FluentBarCollapseMode().Hide;

    /// <summary>
    /// Collapse will go become smaller on breakpoint.
    /// </summary>
    public static IFluentBarCollapseModeOnBreakpoint Small => new FluentBarCollapseMode().Small;
}