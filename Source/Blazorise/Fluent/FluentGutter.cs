#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent gutter builders.
/// </summary>
public interface IFluentGutter
{
    /// <summary>
    /// Builds the classnames based on gutter rules.
    /// </summary>
    /// <param name="classProvider">Currently used class provider.</param>
    /// <returns>List of classnames for the given rules and the class provider.</returns>
    string Class( IClassProvider classProvider );
}

/// <summary>
/// Contains all the allowed gutter rules, except sizes.
/// </summary>
public interface IFluentGutterOnBreakpointWithSide :
    IFluentGutter,
    IFluentGutterFromSide,
    IFluentGutterOnBreakpoint
{
}

/// <summary>
/// Contains all the allowed spacing rules.
/// </summary>
public interface IFluentGutterOnBreakpointWithSideAndSize :
    IFluentGutter,
    IFluentGutterFromSide,
    IFluentGutterOnBreakpoint,
    IFluentGutterWithSize
{
}

/// <summary>
/// Allowed sides for gutter rules.
/// </summary>
public interface IFluentGutterFromSide :
    IFluentGutter
{
    /// <summary>
    /// For classes that set both *-left and *-right.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnX { get; }

    /// <summary>
    /// For classes that set both *-top and *-bottom.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnY { get; }

    /// <summary>
    /// For classes that set a margin or padding on all 4 sides of the element.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnAll { get; }
}

/// <summary>
/// Breakpoints allowed for gutter.
/// </summary>
public interface IFluentGutterOnBreakpoint :
    IFluentGutter,
    IFluentGutterFromSide
{
    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnMobile { get; }

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnTablet { get; }

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnDesktop { get; }

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnWidescreen { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnFullHD { get; }

    /// <summary>
    /// Breakpoint on extra large desktops (extra extra large).
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnQuadHD { get; }
}

/// <summary>
/// Allowed sizes for gutter rules.
/// </summary>
public interface IFluentGutterWithSize :
    IFluentGutter
{
    /// <summary>
    /// For classes that eliminate the margin or padding by setting it to 0.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize Is0 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .25
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize Is1 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .5
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize Is2 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize Is3 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 1.5
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize Is4 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 3
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize Is5 { get; }

    /// <summary>
    /// Used to add custom gutter rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    IFluentGutterWithSize Is( string value );
}

/// <summary>
/// Default implementation of <see cref="IFluentGutter"/>.
/// </summary>
public class FluentGutter :
    IFluentGutter,
    IFluentGutterWithSize,
    IFluentGutterOnBreakpoint,
    IFluentGutterFromSide,
    IFluentGutterOnBreakpointWithSide,
    IFluentGutterOnBreakpointWithSideAndSize
{
    #region Members

    private class GutterDefinition
    {
        public GutterSide Side { get; set; }

        public Breakpoint Breakpoint { get; set; }
    }

    private GutterDefinition currentGutter;

    private readonly Dictionary<GutterSize, List<GutterDefinition>> rules = new();

    private List<string> customRules;

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
                if ( rules.Count > 0 )
                    builder.Append( rules.Select( r => classProvider.Gutter( r.Key, r.Value.Select( v => (v.Side, v.Breakpoint) ) ) ) );

                if ( customRules?.Count > 0 )
                    builder.Append( customRules );
            }

            var classBuilder = new ClassBuilder( BuildClasses );

            classNames = classBuilder.Class;

            dirty = false;
        }

        return classNames;
    }

    private void Dirty()
    {
        dirty = true;
    }

    /// <summary>
    /// Appends the new gutter size rule.
    /// </summary>
    /// <param name="gutterSize">Gutter size to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentGutterOnBreakpointWithSideAndSize WithSize( GutterSize gutterSize )
    {
        var gutterDefinition = new GutterDefinition { Side = GutterSide.All };

        if ( rules.TryGetValue( gutterSize, out var rule ) )
            rule.Add( gutterDefinition );
        else
            rules.Add( gutterSize, new() { gutterDefinition } );

        currentGutter = gutterDefinition;
        Dirty();

        return this;
    }

    /// <summary>
    /// Appends the new side rule.
    /// </summary>
    /// <param name="side">Side to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentGutterOnBreakpointWithSideAndSize WithSide( GutterSide side )
    {
        currentGutter.Side = side;
        Dirty();

        return this;
    }

    /// <summary>
    /// Appends the new breakpoint rule.
    /// </summary>
    /// <param name="breakpoint">Breakpoint to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentGutterOnBreakpointWithSideAndSize WithBreakpoint( Breakpoint breakpoint )
    {
        currentGutter.Breakpoint = breakpoint;
        Dirty();

        return this;
    }

    private IFluentGutterWithSize WithSize( string value )
    {
        if ( customRules is null )
            customRules = new() { value };
        else
            customRules.Add( value );

        Dirty();

        return this;
    }

    /// <summary>
    /// Used to add custom column rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    public IFluentGutterWithSize Is( string value ) => WithSize( value );

    #endregion

    #region Properties

    /// <summary>
    /// For classes that eliminate the margin or padding by setting it to 0.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize Is0 => WithSize( GutterSize.Is0 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .25
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize Is1 => WithSize( GutterSize.Is1 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .5
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize Is2 => WithSize( GutterSize.Is2 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize Is3 => WithSize( GutterSize.Is3 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 1.5
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize Is4 => WithSize( GutterSize.Is4 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 3
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize Is5 => WithSize( GutterSize.Is5 );

    /// <summary>
    /// For classes that set both *-left and *-right.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnX => WithSide( GutterSide.X );

    /// <summary>
    /// For classes that set both *-top and *-bottom.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnY => WithSide( GutterSide.Y );

    /// <summary>
    /// For classes that set a margin or padding on all 4 sides of the element.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnAll => WithSide( GutterSide.All );

    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnMobile => WithBreakpoint( Breakpoint.Mobile );

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnTablet => WithBreakpoint( Breakpoint.Tablet );

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnDesktop => WithBreakpoint( Breakpoint.Desktop );

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnFullHD => WithBreakpoint( Breakpoint.FullHD );

    /// <summary>
    /// Breakpoint on large desktops (extra extra large).
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnQuadHD => WithBreakpoint( Breakpoint.QuadHD );

    #endregion
}

/// <summary>
/// Gutter builder.
/// </summary>
public static class Gutter
{
    /// <summary>
    /// for classes that eliminate the margin by setting it to 0
    /// </summary>
    public static IFluentGutterOnBreakpointWithSideAndSize Is0 => new FluentGutter().Is0;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * .25
    /// </summary>
    public static IFluentGutterOnBreakpointWithSideAndSize Is1 => new FluentGutter().Is1;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * .5
    /// </summary>
    public static IFluentGutterOnBreakpointWithSideAndSize Is2 => new FluentGutter().Is2;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer
    /// </summary>
    public static IFluentGutterOnBreakpointWithSideAndSize Is3 => new FluentGutter().Is3;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * 1.5
    /// </summary>
    public static IFluentGutterOnBreakpointWithSideAndSize Is4 => new FluentGutter().Is4;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * 3
    /// </summary>
    public static IFluentGutterOnBreakpointWithSideAndSize Is5 => new FluentGutter().Is5;

    /// <summary>
    /// Add custom margin rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    public static IFluentGutterWithSize Is( string value ) => new FluentGutter().Is( value );
}