#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for row columns fluent builder.
/// </summary>
public interface IFluentRowColumns
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
/// Allowed breakpoints for row column rules.
/// </summary>
public interface IFluentRowColumnsBreakpoint :
    IFluentRowColumns
{
    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    IFluentRowColumnsSize OnMobile { get; }

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    IFluentRowColumnsSize OnTablet { get; }

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    IFluentRowColumnsSize OnDesktop { get; }

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    IFluentRowColumnsSize OnWidescreen { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    IFluentRowColumnsSize OnFullHD { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra extra large).
    /// </summary>
    IFluentRowColumnsSize OnQuadHD { get; }
}

/// <summary>
/// Allowed sizes for row column rules.
/// </summary>
public interface IFluentRowColumnsSize :
    IFluentRowColumns
{
    /// <summary>
    /// One column width.
    /// </summary>
    IFluentRowColumnsAll Are1 { get; }

    /// <summary>
    /// Two columns width.
    /// </summary>
    IFluentRowColumnsAll Are2 { get; }

    /// <summary>
    /// Three columns width.
    /// </summary>
    IFluentRowColumnsAll Are3 { get; }

    /// <summary>
    /// Four columns width.
    /// </summary>
    IFluentRowColumnsAll Are4 { get; }

    /// <summary>
    /// Five columns width.
    /// </summary>
    IFluentRowColumnsAll Are5 { get; }

    /// <summary>
    /// Six columns width.
    /// </summary>
    IFluentRowColumnsAll Are6 { get; }
}

/// <summary>
/// Contains all the rules for column sizes.
/// </summary>
public interface IFluentRowColumnsAll :
    IFluentRowColumns,
    IFluentRowColumnsBreakpoint,
    IFluentRowColumnsSize
{
}

/// <summary>
/// Holds the build information for current row columns rules.
/// </summary>
public record RowColumnsDefinition
{
    /// <summary>
    /// Defines the breakpoint rule.
    /// </summary>
    public Breakpoint Breakpoint { get; set; }
}

/// <summary>
/// Default implementation of fluent row columns builder.
/// </summary>
public class FluentRowColumns :
    IFluentRowColumns,
    IFluentRowColumnsBreakpoint,
    IFluentRowColumnsSize,
    IFluentRowColumnsAll
{
    #region Members

    private RowColumnsDefinition currentRowColumnsDefinition;

    private readonly Dictionary<RowColumnsSize, RowColumnsDefinition> rules = new();

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
                if ( rules.Any( x => x.Key != RowColumnsSize.Default ) )
                    builder.Append( string.Join( " ", rules.Select( r => classProvider.RowColumns( r.Key, r.Value ) ) ) );
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
    /// Appends the new row columns rule.
    /// </summary>
    /// <param name="rowColumnsSize">Row column size to start the rule.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentRowColumnsAll WithRowColumnsSize( RowColumnsSize rowColumnsSize )
    {
        HasSizes = true;

        var rowColumnsDefinition = new RowColumnsDefinition { Breakpoint = Breakpoint.None };

        rules[rowColumnsSize] = rowColumnsDefinition;

        currentRowColumnsDefinition = rowColumnsDefinition;
        Dirty();

        return this;
    }

    /// <summary>
    /// Appends the new breakpoint rule.
    /// </summary>
    /// <param name="breakpoint">Breakpoint to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentRowColumnsSize WithBreakpoint( Breakpoint breakpoint )
    {
        currentRowColumnsDefinition.Breakpoint = breakpoint;
        Dirty();

        return this;
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool HasSizes { get; private set; }

    /// <inheritdoc/>
    public IFluentRowColumnsAll Are1 => WithRowColumnsSize( RowColumnsSize.Are1 );

    /// <inheritdoc/>
    public IFluentRowColumnsAll Are2 => WithRowColumnsSize( RowColumnsSize.Are2 );

    /// <inheritdoc/>
    public IFluentRowColumnsAll Are3 => WithRowColumnsSize( RowColumnsSize.Are3 );

    /// <inheritdoc/>
    public IFluentRowColumnsAll Are4 => WithRowColumnsSize( RowColumnsSize.Are4 );

    /// <inheritdoc/>
    public IFluentRowColumnsAll Are5 => WithRowColumnsSize( RowColumnsSize.Are5 );

    /// <inheritdoc/>
    public IFluentRowColumnsAll Are6 => WithRowColumnsSize( RowColumnsSize.Are6 );

    /// <inheritdoc/>
    public IFluentRowColumnsSize OnMobile => WithBreakpoint( Breakpoint.Mobile );

    /// <inheritdoc/>
    public IFluentRowColumnsSize OnTablet => WithBreakpoint( Breakpoint.Tablet );

    /// <inheritdoc/>
    public IFluentRowColumnsSize OnDesktop => WithBreakpoint( Breakpoint.Desktop );

    /// <inheritdoc/>
    public IFluentRowColumnsSize OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

    /// <inheritdoc/>
    public IFluentRowColumnsSize OnFullHD => WithBreakpoint( Breakpoint.FullHD );

    /// <inheritdoc/>
    public IFluentRowColumnsSize OnQuadHD => WithBreakpoint( Breakpoint.QuadHD );

    #endregion
}

/// <summary>
/// Defines the number of columns to show in a row.
/// </summary>
public static class RowColumns
{
    /// <summary>
    /// One column per row.
    /// </summary>
    public static IFluentRowColumnsAll Are1 { get { return new FluentRowColumns().Are1; } }

    /// <summary>
    /// Two columns per row.
    /// </summary>
    public static IFluentRowColumnsAll Are2 { get { return new FluentRowColumns().Are2; } }

    /// <summary>
    /// Three columns per row.
    /// </summary>
    public static IFluentRowColumnsAll Are3 { get { return new FluentRowColumns().Are3; } }

    /// <summary>
    /// Four columns per row.
    /// </summary>
    public static IFluentRowColumnsAll Are4 { get { return new FluentRowColumns().Are4; } }

    /// <summary>
    /// Five columns per row.
    /// </summary>
    public static IFluentRowColumnsAll Are5 { get { return new FluentRowColumns().Are5; } }

    /// <summary>
    /// Six columns per row.
    /// </summary>
    public static IFluentRowColumnsAll Are6 { get { return new FluentRowColumns().Are6; } }
}