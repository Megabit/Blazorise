#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent spacing builders.
/// </summary>
public interface IFluentSpacing : IFluentUtilityTarget<IFluentSpacing>
{
    /// <summary>
    /// Builds the classnames based on spacing rules.
    /// </summary>
    /// <param name="classProvider">Currently used class provider.</param>
    /// <returns>List of classnames for the given rules and the class provider.</returns>
    string Class( IClassProvider classProvider );
}

/// <summary>
/// Contains all the allowed spacing rules, except sizes.
/// </summary>
public interface IFluentSpacingOnBreakpointWithSide :
    IFluentSpacing,
    IFluentSpacingFromSide,
    IFluentSpacingOnBreakpoint
{
}

/// <summary>
/// Contains all the allowed spacing rules.
/// </summary>
public interface IFluentSpacingOnBreakpointWithSideAndSize :
    IFluentSpacing,
    IFluentSpacingFromSide,
    IFluentSpacingOnBreakpoint,
    IFluentSpacingWithSize
{
}

/// <summary>
/// Allowed sides for spacing rules.
/// </summary>
public interface IFluentSpacingFromSide :
    IFluentSpacing
{
    /// <summary>
    /// For classes that set margin-top or padding-top.
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize FromTop { get; }

    /// <summary>
    /// For classes that set margin-bottom or padding-bottom.
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize FromBottom { get; }

    /// <summary>
    /// For classes that set margin-left or padding-left.
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize FromStart { get; }

    /// <summary>
    /// For classes that set margin-right or padding-right.
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize FromEnd { get; }

    /// <summary>
    /// For classes that set both *-left and *-right.
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize OnX { get; }

    /// <summary>
    /// For classes that set both *-top and *-bottom.
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize OnY { get; }

    /// <summary>
    /// For classes that set a margin or padding on all 4 sides of the element.
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize OnAll { get; }
}

/// <summary>
/// Allowed breakpoints for spacing rules.
/// </summary>
public interface IFluentSpacingOnBreakpoint :
    IFluentSpacing,
    IFluentSpacingFromSide
{
    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize OnMobile { get; }

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize OnTablet { get; }

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize OnDesktop { get; }

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize OnWidescreen { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize OnFullHD { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra extra large).
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize OnQuadHD { get; }
}

/// <summary>
/// Allowed sizes for spacing rules.
/// </summary>
public interface IFluentSpacingWithSize :
    IFluentSpacing
{
    /// <summary>
    /// For classes that eliminate the margin or padding by setting it to 0.
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize Is0 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .25
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize Is1 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .5
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize Is2 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize Is3 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 1.5
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize Is4 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 3
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize Is5 { get; }

    /// <summary>
    /// For classes that set the margin to auto.
    /// </summary>
    IFluentSpacingOnBreakpointWithSideAndSize IsAuto { get; }

    /// <summary>
    /// Used to add custom spacing rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    IFluentSpacingWithSize Is( string value );
}

/// <summary>
/// Default implementation of <see cref="IFluentSpacing"/>.
/// </summary>
public abstract class FluentSpacing : IFluentSpacing, IFluentSpacingWithSize, IFluentSpacingOnBreakpoint, IFluentSpacingFromSide, IFluentSpacingOnBreakpointWithSide, IFluentSpacingOnBreakpointWithSideAndSize, IUtilityTargeted
{
    #region Members

    private class SpacingDefinition
    {
        public Side Side { get; set; }

        public Breakpoint Breakpoint { get; set; }
    }

    /// <summary>
    /// Spacing type.
    /// </summary>
    private readonly Spacing spacing;

    private SpacingDefinition currentSpacing;

    private readonly Dictionary<SpacingSize, List<SpacingDefinition>> rules = new();

    private List<string> customRules;

    private bool dirty = true;

    private string classNames;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="FluentSpacing"/> constructor.
    /// </summary>
    /// <param name="spacing">Spacing builder type.</param>
    public FluentSpacing( Spacing spacing )
    {
        this.spacing = spacing;
    }

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
                    builder.Append( rules.Select( r => classProvider.Spacing( spacing, r.Key, r.Value.Select( v => (v.Side, v.Breakpoint) ) ) ) );

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

    private IFluentSpacing WithUtilityTarget( UtilityTarget target )
    {
        UtilityTarget = target;
        return this;
    }

    /// <summary>
    /// Appends the new spacing size rule.
    /// </summary>
    /// <param name="spacingSize">Spacing size to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentSpacingOnBreakpointWithSideAndSize WithSize( SpacingSize spacingSize )
    {
        var spacingDefinition = new SpacingDefinition { Breakpoint = Breakpoint.None, Side = Side.All };

        if ( rules.TryGetValue( spacingSize, out var rule ) )
            rule.Add( spacingDefinition );
        else
            rules.Add( spacingSize, new() { spacingDefinition } );

        currentSpacing = spacingDefinition;
        Dirty();
        return this;
    }

    /// <summary>
    /// Appends the new side rule.
    /// </summary>
    /// <param name="side">Side to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentSpacingOnBreakpointWithSideAndSize WithSide( Side side )
    {
        currentSpacing.Side = side;
        Dirty();

        return this;
    }

    /// <summary>
    /// Appends the new breakpoint rule.
    /// </summary>
    /// <param name="breakpoint">Breakpoint to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentSpacingOnBreakpointWithSideAndSize WithBreakpoint( Breakpoint breakpoint )
    {
        currentSpacing.Breakpoint = breakpoint;
        Dirty();

        return this;
    }

    private IFluentSpacingWithSize WithSize( string value )
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
    public IFluentSpacingWithSize Is( string value ) => WithSize( value );

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the utility target override.
    /// </summary>
    public UtilityTarget? UtilityTarget { get; set; }

    /// <summary>
    /// Targets the utility output to the component element.
    /// </summary>
    public IFluentSpacing OnSelf => WithUtilityTarget( global::Blazorise.UtilityTarget.Self );

    /// <summary>
    /// Targets the utility output to a wrapper element.
    /// </summary>
    public IFluentSpacing OnWrapper => WithUtilityTarget( global::Blazorise.UtilityTarget.Wrapper );

    /// <summary>
    /// Gets the spacing type.
    /// </summary>
    protected Spacing Spacing => spacing;

    /// <summary>
    /// For classes that eliminate the margin or padding by setting it to 0.
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize Is0 => WithSize( SpacingSize.Is0 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .25
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize Is1 => WithSize( SpacingSize.Is1 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .5
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize Is2 => WithSize( SpacingSize.Is2 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize Is3 => WithSize( SpacingSize.Is3 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 1.5
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize Is4 => WithSize( SpacingSize.Is4 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 3
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize Is5 => WithSize( SpacingSize.Is5 );

    /// <summary>
    /// For classes that set the margin to auto.
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize IsAuto => WithSize( SpacingSize.IsAuto );

    /// <summary>
    /// For classes that set margin-top or padding-top.
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize FromTop => WithSide( Side.Top );

    /// <summary>
    /// For classes that set margin-bottom or padding-bottom.
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize FromBottom => WithSide( Side.Bottom );

    /// <summary>
    /// For classes that set margin-left or padding-left.
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize FromStart => WithSide( Side.Start );

    /// <summary>
    /// For classes that set margin-right or padding-right.
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize FromEnd => WithSide( Side.End );

    /// <summary>
    /// For classes that set both *-left and *-right.
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize OnX => WithSide( Side.X );

    /// <summary>
    /// For classes that set both *-top and *-bottom.
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize OnY => WithSide( Side.Y );

    /// <summary>
    /// For classes that set a margin or padding on all 4 sides of the element.
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize OnAll => WithSide( Side.All );

    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize OnMobile => WithBreakpoint( Breakpoint.Mobile );

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize OnTablet => WithBreakpoint( Breakpoint.Tablet );

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize OnDesktop => WithBreakpoint( Breakpoint.Desktop );

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize OnFullHD => WithBreakpoint( Breakpoint.FullHD );

    /// <summary>
    /// Breakpoint on large desktops (extra extra large).
    /// </summary>
    public IFluentSpacingOnBreakpointWithSideAndSize OnQuadHD => WithBreakpoint( Breakpoint.QuadHD );

    #endregion
}

/// <summary>
/// Implementation of margin spacing builder.
/// </summary>
public sealed class FluentMargin : FluentSpacing
{
    /// <summary>
    /// A default <see cref="FluentMargin"/> constructor.
    /// </summary>
    public FluentMargin() : base( Spacing.Margin ) { }
}

/// <summary>
/// Implementation of padding spacing builder.
/// </summary>
public sealed class FluentPadding : FluentSpacing
{
    /// <summary>
    /// A default <see cref="FluentPadding"/> constructor.
    /// </summary>
    public FluentPadding() : base( Spacing.Padding ) { }
}

/// <summary>
/// Margin builder.
/// </summary>
public static class Margin
{
    /// <summary>
    /// for classes that eliminate the margin by setting it to 0
    /// </summary>
    public static IFluentSpacingOnBreakpointWithSideAndSize Is0 => new FluentMargin().Is0;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * .25
    /// </summary>
    public static IFluentSpacingOnBreakpointWithSideAndSize Is1 => new FluentMargin().Is1;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * .5
    /// </summary>
    public static IFluentSpacingOnBreakpointWithSideAndSize Is2 => new FluentMargin().Is2;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer
    /// </summary>
    public static IFluentSpacingOnBreakpointWithSideAndSize Is3 => new FluentMargin().Is3;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * 1.5
    /// </summary>
    public static IFluentSpacingOnBreakpointWithSideAndSize Is4 => new FluentMargin().Is4;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * 3
    /// </summary>
    public static IFluentSpacingOnBreakpointWithSideAndSize Is5 => new FluentMargin().Is5;

    /// <summary>
    /// For classes that set the margin to auto.
    /// </summary>
    public static IFluentSpacingOnBreakpointWithSideAndSize IsAuto => new FluentMargin().IsAuto;

    /// <summary>
    /// Add custom margin rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    public static IFluentSpacingWithSize Is( string value ) => new FluentMargin().Is( value );
}

/// <summary>
/// Padding builder.
/// </summary>
public static class Padding
{
    /// <summary>
    /// for classes that eliminate the padding by setting it to 0
    /// </summary>
    public static IFluentSpacingOnBreakpointWithSideAndSize Is0 => new FluentPadding().Is0;

    /// <summary>
    /// (by default) for classes that set the padding to $spacer * .25
    /// </summary>
    public static IFluentSpacingOnBreakpointWithSideAndSize Is1 => new FluentPadding().Is1;

    /// <summary>
    /// (by default) for classes that set the padding to $spacer * .5
    /// </summary>
    public static IFluentSpacingOnBreakpointWithSideAndSize Is2 => new FluentPadding().Is2;

    /// <summary>
    /// (by default) for classes that set the padding to $spacer
    /// </summary>
    public static IFluentSpacingOnBreakpointWithSideAndSize Is3 => new FluentPadding().Is3;

    /// <summary>
    /// (by default) for classes that set the padding to $spacer * 1.5
    /// </summary>
    public static IFluentSpacingOnBreakpointWithSideAndSize Is4 => new FluentPadding().Is4;

    /// <summary>
    /// (by default) for classes that set the padding to $spacer * 3
    /// </summary>
    public static IFluentSpacingOnBreakpointWithSideAndSize Is5 => new FluentPadding().Is5;

    /// <summary>
    /// For classes that set the margin to auto.
    /// </summary>
    public static IFluentSpacingOnBreakpointWithSideAndSize IsAuto => new FluentPadding().IsAuto;

    /// <summary>
    /// Add custom padding rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    public static IFluentSpacingWithSize Is( string value ) => new FluentPadding().Is( value );
}