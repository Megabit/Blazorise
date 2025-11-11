#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Defines the base contract for all fluent gutter builders used to configure horizontal and vertical spacing between grid columns.
/// </summary>
public interface IFluentGutter
{
    /// <summary>
    /// Builds and returns the CSS class string that represents the configured gutter rules.
    /// </summary>
    /// <param name="classProvider">The current <see cref="IClassProvider"/> used to generate provider-specific class names.</param>
    /// <returns>A string containing all CSS classes that represent the configured gutter rules.</returns>
    string Class( IClassProvider classProvider );
}

/// <summary>
/// Represents the set of available gutter configuration rules, excluding size definitions.
/// </summary>
public interface IFluentGutterOnBreakpointWithSide :
    IFluentGutter,
    IFluentGutterFromSide,
    IFluentGutterOnBreakpoint
{
}

/// <summary>
/// Represents the complete set of available gutter configuration rules, including sides, breakpoints, and sizes.
/// </summary>
public interface IFluentGutterOnBreakpointWithSideAndSize :
    IFluentGutter,
    IFluentGutterFromSide,
    IFluentGutterOnBreakpoint,
    IFluentGutterWithSize
{
}

/// <summary>
/// Defines the available axes/sides for applying gutter spacing rules.
/// </summary>
public interface IFluentGutterFromSide :
    IFluentGutter
{
    /// <summary>
    /// Applies gutter spacing on the horizontal axis (left and right).
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnX { get; }

    /// <summary>
    /// Applies gutter spacing on the vertical axis (top and bottom).
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnY { get; }

    /// <summary>
    /// Applies gutter spacing uniformly on both axes.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnAll { get; }
}

/// <summary>
/// Defines the responsive breakpoints at which gutter spacing can be applied.
/// </summary>
public interface IFluentGutterOnBreakpoint :
    IFluentGutter,
    IFluentGutterFromSide
{
    /// <summary>
    /// Applies gutter spacing on all devices (smallest breakpoint).
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnMobile { get; }

    /// <summary>
    /// Applies gutter spacing from the tablet breakpoint and above.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnTablet { get; }

    /// <summary>
    /// Applies gutter spacing from the desktop breakpoint and above.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnDesktop { get; }

    /// <summary>
    /// Applies gutter spacing from the widescreen breakpoint and above.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnWidescreen { get; }

    /// <summary>
    /// Applies gutter spacing from the full HD breakpoint and above.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnFullHD { get; }

    /// <summary>
    /// Applies gutter spacing from the Quad HD breakpoint and above.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize OnQuadHD { get; }
}

/// <summary>
/// Defines the allowed size levels for gutter spacing. The meaning of each level is determined by the active provider.
/// </summary>
public interface IFluentGutterWithSize :
    IFluentGutter
{
    /// <summary>
    /// Applies size level 0 (no gutter spacing).
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize Is0 { get; }

    /// <summary>
    /// Applies size level 1 as defined by the active provider.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize Is1 { get; }

    /// <summary>
    /// Applies size level 2 as defined by the active provider.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize Is2 { get; }

    /// <summary>
    /// Applies size level 3 as defined by the active provider.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize Is3 { get; }

    /// <summary>
    /// Applies size level 4 as defined by the active provider.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize Is4 { get; }

    /// <summary>
    /// Applies size level 5 as defined by the active provider.
    /// </summary>
    IFluentGutterOnBreakpointWithSideAndSize Is5 { get; }

    /// <summary>
    /// Adds a custom CSS class to represent a non-standard gutter rule. The value is passed through to the output unchanged.
    /// </summary>
    /// <param name="value">A custom CSS class representing a gutter spacing rule.</param>
    IFluentGutterWithSize Is( string value );
}

/// <summary>
/// Default implementation of the fluent gutter builder used to compose provider-specific class names.
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
    /// Appends a new size rule to the configuration.
    /// </summary>
    /// <param name="gutterSize">The size level to apply.</param>
    /// <returns>The fluent builder instance for further configuration.</returns>
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
    /// Specifies the axis (horizontal, vertical, or both) to which the current rule applies.
    /// </summary>
    /// <param name="side">The axis/sides to apply the rule on.</param>
    /// <returns>The fluent builder instance for further configuration.</returns>
    public IFluentGutterOnBreakpointWithSideAndSize WithSide( GutterSide side )
    {
        currentGutter.Side = side;
        Dirty();

        return this;
    }

    /// <summary>
    /// Specifies the responsive breakpoint at which the current rule becomes active.
    /// </summary>
    /// <param name="breakpoint">The responsive breakpoint value.</param>
    /// <returns>The fluent builder instance for further configuration.</returns>
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
    /// Adds a custom CSS class that represents a user-defined gutter rule.
    /// </summary>
    /// <param name="value">A custom CSS class name.</param>
    public IFluentGutterWithSize Is( string value ) => WithSize( value );

    #endregion

    #region Properties

    /// <summary>
    /// Applies size level 0 (no gutter spacing).
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize Is0 => WithSize( GutterSize.Is0 );

    /// <summary>
    /// Applies size level 1 as defined by the active provider.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize Is1 => WithSize( GutterSize.Is1 );

    /// <summary>
    /// Applies size level 2 as defined by the active provider.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize Is2 => WithSize( GutterSize.Is2 );

    /// <summary>
    /// Applies size level 3 as defined by the active provider.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize Is3 => WithSize( GutterSize.Is3 );

    /// <summary>
    /// Applies size level 4 as defined by the active provider.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize Is4 => WithSize( GutterSize.Is4 );

    /// <summary>
    /// Applies size level 5 as defined by the active provider.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize Is5 => WithSize( GutterSize.Is5 );

    /// <summary>
    /// Targets the horizontal axis (left and right).
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnX => WithSide( GutterSide.X );

    /// <summary>
    /// Targets the vertical axis (top and bottom).
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnY => WithSide( GutterSide.Y );

    /// <summary>
    /// Targets both axes.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnAll => WithSide( GutterSide.All );

    /// <summary>
    /// Applies the rule on all devices (smallest breakpoint).
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnMobile => WithBreakpoint( Breakpoint.Mobile );

    /// <summary>
    /// Applies the rule from the tablet breakpoint and above.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnTablet => WithBreakpoint( Breakpoint.Tablet );

    /// <summary>
    /// Applies the rule from the desktop breakpoint and above.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnDesktop => WithBreakpoint( Breakpoint.Desktop );

    /// <summary>
    /// Applies the rule from the widescreen breakpoint and above.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

    /// <summary>
    /// Applies the rule from the full HD breakpoint and above.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnFullHD => WithBreakpoint( Breakpoint.FullHD );

    /// <summary>
    /// Applies the rule from the Quad HD breakpoint and above.
    /// </summary>
    public IFluentGutterOnBreakpointWithSideAndSize OnQuadHD => WithBreakpoint( Breakpoint.QuadHD );

    #endregion
}

/// <summary>
/// Provides convenient entry points for creating fluent gutter rules.
/// </summary>
public static class Gutter
{
    /// <summary>
    /// Applies size level 0 (no gutter spacing).
    /// </summary>
    public static IFluentGutterOnBreakpointWithSideAndSize Is0 => new FluentGutter().Is0;

    /// <summary>
    /// Applies size level 1 as defined by the active provider.
    /// </summary>
    public static IFluentGutterOnBreakpointWithSideAndSize Is1 => new FluentGutter().Is1;

    /// <summary>
    /// Applies size level 2 as defined by the active provider.
    /// </summary>
    public static IFluentGutterOnBreakpointWithSideAndSize Is2 => new FluentGutter().Is2;

    /// <summary>
    /// Applies size level 3 as defined by the active provider.
    /// </summary>
    public static IFluentGutterOnBreakpointWithSideAndSize Is3 => new FluentGutter().Is3;

    /// <summary>
    /// Applies size level 4 as defined by the active provider.
    /// </summary>
    public static IFluentGutterOnBreakpointWithSideAndSize Is4 => new FluentGutter().Is4;

    /// <summary>
    /// Applies size level 5 as defined by the active provider.
    /// </summary>
    public static IFluentGutterOnBreakpointWithSideAndSize Is5 => new FluentGutter().Is5;

    /// <summary>
    /// Adds a custom CSS class to represent a user-defined gutter rule. The value is passed through unchanged.
    /// </summary>
    /// <param name="value">A custom CSS class name.</param>
    public static IFluentGutterWithSize Is( string value ) => new FluentGutter().Is( value );
}