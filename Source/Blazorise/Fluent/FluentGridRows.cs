#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for grid rows fluent builder.
/// </summary>
public interface IFluentGridRows
{
    /// <summary>
    /// Builds and returns the classnames for row sizes.
    /// </summary>
    /// <param name="classProvider">Class provider used by the current framework provider.</param>
    /// <returns>Return list of css classnames.</returns>
    string Class( IClassProvider classProvider );

    /// <summary>
    /// True if there are row sizes defined.
    /// </summary>
    bool HasSizes { get; }
}

/// <summary>
/// Allowed breakpoints for grid row rules.
/// </summary>
public interface IFluentGridRowsBreakpoint :
    IFluentGridRows
{
    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    IFluentGridRowsSize OnMobile { get; }

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    IFluentGridRowsSize OnTablet { get; }

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    IFluentGridRowsSize OnDesktop { get; }

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    IFluentGridRowsSize OnWidescreen { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    IFluentGridRowsSize OnFullHD { get; }
}

/// <summary>
/// Allowed sizes for grid row rules.
/// </summary>
public interface IFluentGridRowsSize :
    IFluentGridRows
{
    /// <summary>
    /// One row.
    /// </summary>
    IFluentGridRowsAll Are1 { get; }

    /// <summary>
    /// Two rows.
    /// </summary>
    IFluentGridRowsAll Are2 { get; }

    /// <summary>
    /// Three rows.
    /// </summary>
    IFluentGridRowsAll Are3 { get; }

    /// <summary>
    /// Four rows.
    /// </summary>
    IFluentGridRowsAll Are4 { get; }

    /// <summary>
    /// Five rows.
    /// </summary>
    IFluentGridRowsAll Are5 { get; }

    /// <summary>
    /// Six rows.
    /// </summary>
    IFluentGridRowsAll Are6 { get; }
}

/// <summary>
/// Contains all the rules for row sizes.
/// </summary>
public interface IFluentGridRowsAll :
    IFluentGridRows,
    IFluentGridRowsBreakpoint,
    IFluentGridRowsSize
{
}

/// <summary>
/// Holds the build information for current grid rows rules.
/// </summary>
public record GridRowsDefinition
{
    /// <summary>
    /// Defines the breakpoint rule.
    /// </summary>
    public Breakpoint Breakpoint { get; set; }
}

/// <summary>
/// Default implementation of fluent grid rows builder.
/// </summary>
public class FluentGridRows :
    IFluentGridRows,
    IFluentGridRowsBreakpoint,
    IFluentGridRowsSize,
    IFluentGridRowsAll
{
    #region Members

    private GridRowsDefinition currentGridRowsDefinition;

    private readonly Dictionary<GridRowsSize, GridRowsDefinition> rules = new();

    private bool dirty = true;

    private string classNames;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public string Class( IClassProvider classProvider )
    {
        if ( dirty )
        {
            void BuildClasses( ClassBuilder builder )
            {
                if ( rules.Any( x => x.Key != GridRowsSize.Default ) )
                    builder.Append( string.Join( " ", rules.Select( r => classProvider.GridRows( r.Key, r.Value ) ) ) );
            }

            var classBuilder = new ClassBuilder( BuildClasses );

            classNames = classBuilder.Class;

            dirty = false;
        }

        return classNames;
    }

    /// <summary>
    /// Flags the classnames to be rebuilt.
    /// </summary>
    private void Dirty()
    {
        dirty = true;
    }

    /// <summary>
    /// Appends the new grid rows rule.
    /// </summary>
    /// <param name="gridRowsSize">Grid row size to start the rule.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentGridRowsAll WithGridRowsSize( GridRowsSize gridRowsSize )
    {
        HasSizes = true;

        var gridRowsDefinition = new GridRowsDefinition { Breakpoint = Breakpoint.None };

        rules[gridRowsSize] = gridRowsDefinition;

        currentGridRowsDefinition = gridRowsDefinition;
        Dirty();

        return this;
    }

    /// <summary>
    /// Appends the new breakpoint rule.
    /// </summary>
    /// <param name="breakpoint">Breakpoint to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentGridRowsSize WithBreakpoint( Breakpoint breakpoint )
    {
        currentGridRowsDefinition.Breakpoint = breakpoint;
        Dirty();

        return this;
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool HasSizes { get; private set; }

    /// <inheritdoc/>
    public IFluentGridRowsAll Are1 => WithGridRowsSize( GridRowsSize.Are1 );

    /// <inheritdoc/>
    public IFluentGridRowsAll Are2 => WithGridRowsSize( GridRowsSize.Are2 );

    /// <inheritdoc/>
    public IFluentGridRowsAll Are3 => WithGridRowsSize( GridRowsSize.Are3 );

    /// <inheritdoc/>
    public IFluentGridRowsAll Are4 => WithGridRowsSize( GridRowsSize.Are4 );

    /// <inheritdoc/>
    public IFluentGridRowsAll Are5 => WithGridRowsSize( GridRowsSize.Are5 );

    /// <inheritdoc/>
    public IFluentGridRowsAll Are6 => WithGridRowsSize( GridRowsSize.Are6 );

    /// <inheritdoc/>
    public IFluentGridRowsSize OnMobile => WithBreakpoint( Breakpoint.Mobile );

    /// <inheritdoc/>
    public IFluentGridRowsSize OnTablet => WithBreakpoint( Breakpoint.Tablet );

    /// <inheritdoc/>
    public IFluentGridRowsSize OnDesktop => WithBreakpoint( Breakpoint.Desktop );

    /// <inheritdoc/>
    public IFluentGridRowsSize OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

    /// <inheritdoc/>
    public IFluentGridRowsSize OnFullHD => WithBreakpoint( Breakpoint.FullHD );

    #endregion
}

/// <summary>
/// Defines the number of rows to show in a grid.
/// </summary>
public static class GridRows
{
    /// <summary>
    /// One row per row.
    /// </summary>
    public static IFluentGridRowsAll Are1 { get { return new FluentGridRows().Are1; } }

    /// <summary>
    /// Two rows per row.
    /// </summary>
    public static IFluentGridRowsAll Are2 { get { return new FluentGridRows().Are2; } }

    /// <summary>
    /// Three rows per row.
    /// </summary>
    public static IFluentGridRowsAll Are3 { get { return new FluentGridRows().Are3; } }

    /// <summary>
    /// Four rows per row.
    /// </summary>
    public static IFluentGridRowsAll Are4 { get { return new FluentGridRows().Are4; } }

    /// <summary>
    /// Five rows per row.
    /// </summary>
    public static IFluentGridRowsAll Are5 { get { return new FluentGridRows().Are5; } }

    /// <summary>
    /// Six rows per row.
    /// </summary>
    public static IFluentGridRowsAll Are6 { get { return new FluentGridRows().Are6; } }
}