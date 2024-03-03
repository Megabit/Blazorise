#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for column fluent builder.
/// </summary>
public interface IFluentColumn
{
    /// <summary>
    /// Builds and returns the classnames for column sizes.
    /// </summary>
    /// <param name="grid">If true, column is the child of the <see cref="Grid"/> component.</param>
    /// <param name="classProvider">Class provider used by the current framework provider.</param>
    /// <returns>Return list of css classnames.</returns>
    string Class( bool grid, IClassProvider classProvider );

    /// <summary>
    /// True if there are column sizes defined.
    /// </summary>
    bool HasSizes { get; }
}

/// <summary>
/// Contains all the rules for column sizes.
/// </summary>
public interface IFluentColumnOnBreakpointWithOffsetAndSize :
    IFluentColumn,
    IFluentColumnOnBreakpoint,
    IFluentColumnWithOffset,
    IFluentColumnWithSize
{
}

/// <summary>
/// Allowed breakpoints for column rules.
/// </summary>
public interface IFluentColumnOnBreakpoint :
    IFluentColumn
{
    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    IFluentColumnWithSize OnMobile { get; }

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    IFluentColumnWithSize OnTablet { get; }

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    IFluentColumnWithSize OnDesktop { get; }

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    IFluentColumnWithSize OnWidescreen { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    IFluentColumnWithSize OnFullHD { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra extra large).
    /// </summary>
    IFluentColumnWithSize OnFull2K { get; }
}

/// <summary>
/// Offset rule for column rules.
/// </summary>
public interface IFluentColumnWithOffset :
    IFluentColumn
{
    /// <summary>
    /// Move columns to the right.
    /// </summary>
    IFluentColumnOnBreakpoint WithOffset { get; }
}

/// <summary>
/// Allowed sizes for column rules.
/// </summary>
public interface IFluentColumnWithSize :
    IFluentColumn
{
    /// <summary>
    /// One column width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize Is1 { get; }

    /// <summary>
    /// Two columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize Is2 { get; }

    /// <summary>
    /// Three columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize Is3 { get; }

    /// <summary>
    /// Four columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize Is4 { get; }

    /// <summary>
    /// Five columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize Is5 { get; }

    /// <summary>
    /// Six columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize Is6 { get; }

    /// <summary>
    /// Seven columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize Is7 { get; }

    /// <summary>
    /// Eight columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize Is8 { get; }

    /// <summary>
    /// Nine columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize Is9 { get; }

    /// <summary>
    /// Ten columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize Is10 { get; }

    /// <summary>
    /// Eleven columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize Is11 { get; }

    /// <summary>
    /// Twelve columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize Is12 { get; }

    /// <summary>
    /// Twelve columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize IsFull { get; }

    /// <summary>
    /// Six columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize IsHalf { get; }

    /// <summary>
    /// Four columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize IsThird { get; }

    /// <summary>
    /// Three columns width.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize IsQuarter { get; }

    /// <summary>
    /// Fill all available space.
    /// </summary>
    IFluentColumnOnBreakpointWithOffsetAndSize IsAuto { get; }

    /// <summary>
    /// Used to add custom column rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    IFluentColumnWithSize Is( string value );
}

/// <summary>
/// Holds the build information for current column rules.
/// </summary>
public class ColumnDefinition
{
    /// <summary>
    /// Gets or sets the column size.
    /// </summary>
    public ColumnWidth ColumnWidth { get; set; }

    /// <summary>
    /// Gets or sets the breakpoint rule.
    /// </summary>
    public Breakpoint Breakpoint { get; set; }

    /// <summary>
    /// Gets or sets the flag to indicate we want to offset column by the <see cref="ColumnWidth"/>.
    /// </summary>
    public bool Offset { get; set; }
}

/// <summary>
/// Default implementation of fluent column builder.
/// </summary>
public class FluentColumn :
    IFluentColumn,
    IFluentColumnOnBreakpointWithOffsetAndSize,
    IFluentColumnOnBreakpoint,
    IFluentColumnWithSize,
    IFluentColumnWithOffset
{
    #region Members

    private ColumnDefinition currentColumnDefinition;

    private readonly List<ColumnDefinition> columnDefinitions = new();

    private List<string> customRules;

    private bool dirty = true;

    private string classNames;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public string Class( bool grid, IClassProvider classProvider )
    {
        if ( dirty )
        {
            void BuildClasses( ClassBuilder builder )
            {
                if ( HasSizes && columnDefinitions?.Count > 0 )
                {
                    builder.Append( classProvider.Column( grid, columnDefinitions.Where( x => x.ColumnWidth != ColumnWidth.Default ) ) );
                }

                if ( customRules?.Count > 0 )
                    builder.Append( customRules );
            }

            var classBuilder = new ClassBuilder( BuildClasses );

            classNames = classBuilder.Class;

            dirty = false;
        }

        return classNames;
    }

    private static int GetUsedSpace( ColumnWidth columnWidth )
    {
        return columnWidth switch
        {
            Blazorise.ColumnWidth.Is1 => 1,
            Blazorise.ColumnWidth.Is2 => 2,
            Blazorise.ColumnWidth.Is3 or Blazorise.ColumnWidth.Quarter => 3,
            Blazorise.ColumnWidth.Is4 or Blazorise.ColumnWidth.Third => 4,
            Blazorise.ColumnWidth.Is5 => 5,
            Blazorise.ColumnWidth.Is6 or Blazorise.ColumnWidth.Half => 6,
            Blazorise.ColumnWidth.Is7 => 7,
            Blazorise.ColumnWidth.Is8 => 8,
            Blazorise.ColumnWidth.Is9 => 9,
            Blazorise.ColumnWidth.Is10 => 10,
            Blazorise.ColumnWidth.Is11 => 11,
            Blazorise.ColumnWidth.Is12 or Blazorise.ColumnWidth.Full => 12,
            Blazorise.ColumnWidth.Auto => 0,
            _ => 0,
        };
    }

    private void Dirty()
    {
        dirty = true;
    }

    /// <summary>
    /// Appends the new size rule.
    /// </summary>
    /// <param name="columnSize">Column size to start the rule.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentColumnOnBreakpointWithOffsetAndSize WithColumnSize( ColumnWidth columnSize )
    {
        if ( columnSize != ColumnWidth.Default )
            HasSizes = true;

        currentColumnDefinition = new ColumnDefinition { ColumnWidth = columnSize, Breakpoint = Breakpoint.None };

        columnDefinitions.Add( currentColumnDefinition );

        Dirty();

        return this;
    }

    /// <summary>
    /// Appends the new custom size rule.
    /// </summary>
    /// <param name="value">Custom size to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentColumnWithSize WithColumnSize( string value )
    {
        if ( customRules is null )
            customRules = new() { value };
        else
            customRules.Add( value );

        Dirty();

        return this;
    }

    /// <summary>
    /// Appends the new breakpoint rule.
    /// </summary>
    /// <param name="breakpoint">Breakpoint to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentColumnWithSize WithBreakpoint( Breakpoint breakpoint )
    {
        currentColumnDefinition.Breakpoint = breakpoint;
        Dirty();

        return this;
    }

    /// <summary>
    /// Used to add custom column rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    public IFluentColumnWithSize Is( string value ) => WithColumnSize( value );

    #endregion

    #region Properties

    /// <summary>
    /// Defines if column has any size rule defined.
    /// </summary>
    public bool HasSizes { get; private set; }

    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    public IFluentColumnWithSize OnMobile => WithBreakpoint( Breakpoint.Mobile );

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    public IFluentColumnWithSize OnTablet => WithBreakpoint( Breakpoint.Tablet );

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    public IFluentColumnWithSize OnDesktop => WithBreakpoint( Breakpoint.Desktop );

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    public IFluentColumnWithSize OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    public IFluentColumnWithSize OnFullHD => WithBreakpoint( Breakpoint.FullHD );

    /// <summary>
    /// Breakpoint on large desktops (extra extra large).
    /// </summary>
    public IFluentColumnWithSize OnFull2K => WithBreakpoint( Breakpoint.Full2K );

    /// <summary>
    /// Move columns to the right.
    /// </summary>
    public IFluentColumnOnBreakpoint WithOffset { get { currentColumnDefinition.Offset = true; Dirty(); return this; } }

    /// <summary>
    /// One column width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize Is1 { get { return WithColumnSize( ColumnWidth.Is1 ); } }

    /// <summary>
    /// Two columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize Is2 { get { return WithColumnSize( ColumnWidth.Is2 ); } }

    /// <summary>
    /// Three columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize Is3 { get { return WithColumnSize( ColumnWidth.Is3 ); } }

    /// <summary>
    /// Four columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize Is4 { get { return WithColumnSize( ColumnWidth.Is4 ); } }

    /// <summary>
    /// Five columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize Is5 { get { return WithColumnSize( ColumnWidth.Is5 ); } }

    /// <summary>
    /// Six columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize Is6 { get { return WithColumnSize( ColumnWidth.Is6 ); } }

    /// <summary>
    /// Seven columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize Is7 { get { return WithColumnSize( ColumnWidth.Is7 ); } }

    /// <summary>
    /// Eight columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize Is8 { get { return WithColumnSize( ColumnWidth.Is8 ); } }

    /// <summary>
    /// Nine columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize Is9 { get { return WithColumnSize( ColumnWidth.Is9 ); } }

    /// <summary>
    /// Ten columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize Is10 { get { return WithColumnSize( ColumnWidth.Is10 ); } }

    /// <summary>
    /// Eleven columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize Is11 { get { return WithColumnSize( ColumnWidth.Is11 ); } }

    /// <summary>
    /// Twelve columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize Is12 { get { return WithColumnSize( ColumnWidth.Is12 ); } }

    /// <summary>
    /// Twelve columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize IsFull { get { return WithColumnSize( ColumnWidth.Is12 ); } }

    /// <summary>
    /// Six columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize IsHalf { get { return WithColumnSize( ColumnWidth.Is6 ); } }

    /// <summary>
    /// Four columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize IsThird { get { return WithColumnSize( ColumnWidth.Is4 ); } }

    /// <summary>
    /// Three columns width.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize IsQuarter { get { return WithColumnSize( ColumnWidth.Is3 ); } }

    /// <summary>
    /// Fill all available space.
    /// </summary>
    public IFluentColumnOnBreakpointWithOffsetAndSize IsAuto { get { return WithColumnSize( ColumnWidth.Auto ); } }

    #endregion
}

/// <summary>
/// Fluent builder for the column sizes.
/// </summary>
public static class ColumnSize
{
    /// <summary>
    /// One column width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize Is1 { get { return new FluentColumn().Is1; } }

    /// <summary>
    /// Two columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize Is2 { get { return new FluentColumn().Is2; } }

    /// <summary>
    /// Three columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize Is3 { get { return new FluentColumn().Is3; } }

    /// <summary>
    /// Four columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize Is4 { get { return new FluentColumn().Is4; } }

    /// <summary>
    /// Five columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize Is5 { get { return new FluentColumn().Is5; } }

    /// <summary>
    /// Six columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize Is6 { get { return new FluentColumn().Is6; } }

    /// <summary>
    /// Seven columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize Is7 { get { return new FluentColumn().Is7; } }

    /// <summary>
    /// Eight columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize Is8 { get { return new FluentColumn().Is8; } }

    /// <summary>
    /// Nine columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize Is9 { get { return new FluentColumn().Is9; } }

    /// <summary>
    /// Ten columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize Is10 { get { return new FluentColumn().Is10; } }

    /// <summary>
    /// Eleven columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize Is11 { get { return new FluentColumn().Is11; } }

    /// <summary>
    /// Twelve columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize Is12 { get { return new FluentColumn().Is12; } }

    /// <summary>
    /// Twelve columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize IsFull { get { return new FluentColumn().IsFull; } }

    /// <summary>
    /// Six columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize IsHalf { get { return new FluentColumn().IsHalf; } }

    /// <summary>
    /// Four columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize IsThird { get { return new FluentColumn().IsThird; } }

    /// <summary>
    /// Three columns width.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize IsQuarter { get { return new FluentColumn().IsQuarter; } }

    /// <summary>
    /// Fill all available space.
    /// </summary>
    public static IFluentColumnOnBreakpointWithOffsetAndSize IsAuto { get { return new FluentColumn().IsAuto; } }

    /// <summary>
    /// Add custom column rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    public static IFluentColumnWithSize Is( string value ) => new FluentColumn().Is( value );
}