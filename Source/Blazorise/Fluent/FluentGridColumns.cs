#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for grid columns fluent builder.
/// </summary>
public interface IFluentGridColumns
{
    /// <summary>
    /// Builds and returns the classnames for column sizes.
    /// </summary>
    /// <param name="classProvider">Class provider used by the current framework provider.</param>
    /// <returns>Return list of css classnames.</returns>
    string Class( IClassProvider classProvider );

    /// <summary>
    /// True if there are column sizes defined.
    /// </summary>
    bool HasSizes { get; }
}

/// <summary>
/// Allowed breakpoints for grid column rules.
/// </summary>
public interface IFluentGridColumnsBreakpoint :
    IFluentGridColumns
{
    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    IFluentGridColumnsSize OnMobile { get; }

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    IFluentGridColumnsSize OnTablet { get; }

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    IFluentGridColumnsSize OnDesktop { get; }

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    IFluentGridColumnsSize OnWidescreen { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    IFluentGridColumnsSize OnFullHD { get; }
}

/// <summary>
/// Allowed sizes for grid column rules.
/// </summary>
public interface IFluentGridColumnsSize :
    IFluentGridColumns
{
    /// <summary>
    /// One column width.
    /// </summary>
    IFluentGridColumnsAll Are1 { get; }

    /// <summary>
    /// Two columns width.
    /// </summary>
    IFluentGridColumnsAll Are2 { get; }

    /// <summary>
    /// Three columns width.
    /// </summary>
    IFluentGridColumnsAll Are3 { get; }

    /// <summary>
    /// Four columns width.
    /// </summary>
    IFluentGridColumnsAll Are4 { get; }

    /// <summary>
    /// Five columns width.
    /// </summary>
    IFluentGridColumnsAll Are5 { get; }

    /// <summary>
    /// Six columns width.
    /// </summary>
    IFluentGridColumnsAll Are6 { get; }

    /// <summary>
    /// Seven columns per row.
    /// </summary>
    IFluentGridColumnsAll Are7 { get; }

    /// <summary>
    /// Eight columns per row.
    /// </summary>
    IFluentGridColumnsAll Are8 { get; }

    /// <summary>
    /// Nine columns per row.
    /// </summary>
    IFluentGridColumnsAll Are9 { get; }

    /// <summary>
    /// Ten columns per row.
    /// </summary>
    IFluentGridColumnsAll Are10 { get; }

    /// <summary>
    /// Eleven columns per row.
    /// </summary>
    IFluentGridColumnsAll Are11 { get; }

    /// <summary>
    /// Twelve columns per row.
    /// </summary>
    IFluentGridColumnsAll Are12 { get; }
}

/// <summary>
/// Contains all the rules for column sizes.
/// </summary>
public interface IFluentGridColumnsAll :
    IFluentGridColumns,
    IFluentGridColumnsBreakpoint,
    IFluentGridColumnsSize
{
}

/// <summary>
/// Holds the build information for current grid columns rules.
/// </summary>
public record GridColumnsDefinition
{
    /// <summary>
    /// Defines the breakpoint rule.
    /// </summary>
    public Breakpoint Breakpoint { get; set; }
}

/// <summary>
/// Default implementation of fluent grid columns builder.
/// </summary>
public class FluentGridColumns :
    IFluentGridColumns,
    IFluentGridColumnsBreakpoint,
    IFluentGridColumnsSize,
    IFluentGridColumnsAll
{
    #region Members

    private GridColumnsDefinition currentGridColumnsDefinition;

    private readonly Dictionary<GridColumnsSize, GridColumnsDefinition> rules = new();

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
                if ( rules.Any( x => x.Key != GridColumnsSize.Default ) )
                    builder.Append( string.Join( " ", rules.Select( r => classProvider.GridColumns( r.Key, r.Value ) ) ) );
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
    /// Appends the new grid columns rule.
    /// </summary>
    /// <param name="gridColumnsSize">Grid column size to start the rule.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentGridColumnsAll WithGridColumnsSize( GridColumnsSize gridColumnsSize )
    {
        HasSizes = true;

        var gridColumnsDefinition = new GridColumnsDefinition { Breakpoint = Breakpoint.None };

        rules[gridColumnsSize] = gridColumnsDefinition;

        currentGridColumnsDefinition = gridColumnsDefinition;
        Dirty();

        return this;
    }

    /// <summary>
    /// Appends the new breakpoint rule.
    /// </summary>
    /// <param name="breakpoint">Breakpoint to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentGridColumnsSize WithBreakpoint( Breakpoint breakpoint )
    {
        currentGridColumnsDefinition.Breakpoint = breakpoint;
        Dirty();

        return this;
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool HasSizes { get; private set; }

    /// <inheritdoc/>
    public IFluentGridColumnsAll Are1 => WithGridColumnsSize( GridColumnsSize.Are1 );

    /// <inheritdoc/>
    public IFluentGridColumnsAll Are2 => WithGridColumnsSize( GridColumnsSize.Are2 );

    /// <inheritdoc/>
    public IFluentGridColumnsAll Are3 => WithGridColumnsSize( GridColumnsSize.Are3 );

    /// <inheritdoc/>
    public IFluentGridColumnsAll Are4 => WithGridColumnsSize( GridColumnsSize.Are4 );

    /// <inheritdoc/>
    public IFluentGridColumnsAll Are5 => WithGridColumnsSize( GridColumnsSize.Are5 );

    /// <inheritdoc/>
    public IFluentGridColumnsAll Are6 => WithGridColumnsSize( GridColumnsSize.Are6 );

    /// <inheritdoc/>
    public IFluentGridColumnsAll Are7 => WithGridColumnsSize( GridColumnsSize.Are7 );

    /// <inheritdoc/>
    public IFluentGridColumnsAll Are8 => WithGridColumnsSize( GridColumnsSize.Are8 );

    /// <inheritdoc/>
    public IFluentGridColumnsAll Are9 => WithGridColumnsSize( GridColumnsSize.Are9 );

    /// <inheritdoc/>
    public IFluentGridColumnsAll Are10 => WithGridColumnsSize( GridColumnsSize.Are10 );

    /// <inheritdoc/>
    public IFluentGridColumnsAll Are11 => WithGridColumnsSize( GridColumnsSize.Are11 );

    /// <inheritdoc/>
    public IFluentGridColumnsAll Are12 => WithGridColumnsSize( GridColumnsSize.Are12 );

    /// <inheritdoc/>
    public IFluentGridColumnsSize OnMobile => WithBreakpoint( Breakpoint.Mobile );

    /// <inheritdoc/>
    public IFluentGridColumnsSize OnTablet => WithBreakpoint( Breakpoint.Tablet );

    /// <inheritdoc/>
    public IFluentGridColumnsSize OnDesktop => WithBreakpoint( Breakpoint.Desktop );

    /// <inheritdoc/>
    public IFluentGridColumnsSize OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

    /// <inheritdoc/>
    public IFluentGridColumnsSize OnFullHD => WithBreakpoint( Breakpoint.FullHD );

    #endregion
}

/// <summary>
/// Defines the number of columns to show in a grid.
/// </summary>
public static class GridColumns
{
    /// <summary>
    /// One column per row.
    /// </summary>
    public static IFluentGridColumnsAll Are1 { get { return new FluentGridColumns().Are1; } }

    /// <summary>
    /// Two columns per row.
    /// </summary>
    public static IFluentGridColumnsAll Are2 { get { return new FluentGridColumns().Are2; } }

    /// <summary>
    /// Three columns per row.
    /// </summary>
    public static IFluentGridColumnsAll Are3 { get { return new FluentGridColumns().Are3; } }

    /// <summary>
    /// Four columns per row.
    /// </summary>
    public static IFluentGridColumnsAll Are4 { get { return new FluentGridColumns().Are4; } }

    /// <summary>
    /// Five columns per row.
    /// </summary>
    public static IFluentGridColumnsAll Are5 { get { return new FluentGridColumns().Are5; } }

    /// <summary>
    /// Six columns per row.
    /// </summary>
    public static IFluentGridColumnsAll Are6 { get { return new FluentGridColumns().Are6; } }

    /// <summary>
    /// Seven columns per row.
    /// </summary>
    public static IFluentGridColumnsAll Are7 { get { return new FluentGridColumns().Are7; } }

    /// <summary>
    /// Eight columns per row.
    /// </summary>
    public static IFluentGridColumnsAll Are8 { get { return new FluentGridColumns().Are8; } }

    /// <summary>
    /// Nine columns per row.
    /// </summary>
    public static IFluentGridColumnsAll Are9 { get { return new FluentGridColumns().Are9; } }

    /// <summary>
    /// Ten columns per row.
    /// </summary>
    public static IFluentGridColumnsAll Are10 { get { return new FluentGridColumns().Are10; } }

    /// <summary>
    /// Eleven columns per row.
    /// </summary>
    public static IFluentGridColumnsAll Are11 { get { return new FluentGridColumns().Are11; } }

    /// <summary>
    /// Twelve columns per row.
    /// </summary>
    public static IFluentGridColumnsAll Are12 { get { return new FluentGridColumns().Are12; } }
}