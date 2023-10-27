#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent sizing builders.
/// </summary>
public interface IFluentSizing
{
    /// <summary>
    /// Builds the classnames based on sizing rules.
    /// </summary>
    /// <param name="classProvider">Currently used class provider.</param>
    /// <returns>List of classnames for the given rules and the class provider.</returns>
    string Class( IClassProvider classProvider );

    /// <summary>
    /// Builds the styles based on sizing rules.
    /// </summary>
    /// <param name="styleProvider">Currently used style provider.</param>
    /// <returns>List of styles for the given rules and the style provider.</returns>
    string Style( IStyleProvider styleProvider );
}

/// <summary>
/// Contains all the allowed sizing rules.
/// </summary>
public interface IFluentSizingWithSizeWithMinMaxWithViewportAll :
    IFluentSizing,
    IFluentSizingSize,
    IFluentSizingMinMaxViewport,
    IFluentSizingViewport
{
}

/// <summary>
/// Contains sizing rules for size and breakpoint.
/// </summary>
public interface IFluentSizingWithSizeOnBreakpoint :
    IFluentSizing,
    IFluentSizingSize,
    IFluentSizingOnBreakpoint
{
}

/// <summary>
/// Allowed breakpoints for sizing rules.
/// </summary>
public interface IFluentSizingOnBreakpoint :
    IFluentSizing,
    IFluentSizingSize,
    IFluentSizingMinMaxViewport,
    IFluentSizingViewport
{
    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    IFluentSizingWithSizeWithMinMaxWithViewportAll OnMobile { get; }

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    IFluentSizingWithSizeWithMinMaxWithViewportAll OnTablet { get; }

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    IFluentSizingWithSizeWithMinMaxWithViewportAll OnDesktop { get; }

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    IFluentSizingWithSizeWithMinMaxWithViewportAll OnWidescreen { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    IFluentSizingWithSizeWithMinMaxWithViewportAll OnFullHD { get; }
}

/// <summary>
/// Allowed rules for sizing rule sizes.
/// </summary>
public interface IFluentSizingSize :
    IFluentSizing
{
    /// <summary>
    /// An element will occupy 25% of its parent space.
    /// </summary>
    IFluentSizingOnBreakpoint Is25 { get; }

    /// <summary>
    /// An element will occupy third of its parent space.
    /// </summary>
    IFluentSizingOnBreakpoint Is33 { get; }

    /// <summary>
    /// An element will occupy 50% of its parent space.
    /// </summary>
    IFluentSizingOnBreakpoint Is50 { get; }

    /// <summary>
    /// An element will occupy two thirds of its parent space.
    /// </summary>
    IFluentSizingOnBreakpoint Is66 { get; }

    /// <summary>
    /// An element will occupy 75% of its parent space.
    /// </summary>
    IFluentSizingOnBreakpoint Is75 { get; }

    /// <summary>
    /// An element will occupy 100% of its parent space.
    /// </summary>
    IFluentSizingMinMaxViewportOnBreakpoint Is100 { get; }

    /// <summary>
    /// The browser calculates the size.
    /// </summary>
    IFluentSizing Auto { get; }

    /// <summary>
    /// Defines the manual size.
    /// </summary>
    /// <param name="unit">Unit value, eg. in px, rem, em, etc.</param>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizing"/> reference.</returns>
    IFluentSizing Is( string unit, double size );
}

/// <summary>
/// Contains the min, max and viewport rules.
/// </summary>
public interface IFluentSizingMinMaxViewport :
    IFluentSizing,
    IFluentSizingMin,
    IFluentSizingMax,
    IFluentSizingViewport
{
}

/// <summary>
/// Contains the min, max and viewport rules.
/// </summary>
public interface IFluentSizingMinMaxViewportOnBreakpoint :
    IFluentSizing,
    IFluentSizingMin,
    IFluentSizingMax,
    IFluentSizingViewport,
    IFluentSizingOnBreakpoint
{
}

/// <summary>
/// Contains the min rule.
/// </summary>
public interface IFluentSizingMin :
    IFluentSizing
{
    /// <summary>
    /// Size will be defined for the min attribute(s) of the element style.
    /// </summary>
    IFluentSizingViewport Min { get; }
}

/// <summary>
/// Contains the viewport rule.
/// </summary>
public interface IFluentSizingViewport :
    IFluentSizing
{
    /// <summary>
    /// Size will be defined for the viewport.
    /// </summary>
    IFluentSizing Viewport { get; }
}

/// <summary>
/// Contains the max rule.
/// </summary>
public interface IFluentSizingMax :
    IFluentSizing
{
    /// <summary>
    /// Size will be defined for the max attribute(s) of the element style.
    /// </summary>
    IFluentSizingWithSizeOnBreakpoint Max { get; }
}

/// <summary>
/// Holds the build information for current sizing rules.
/// </summary>
public record SizingDefinition
{
    /// <summary>
    /// Size will be defined for the min attribute(s) of the element style.
    /// </summary>
    public bool IsMin { get; set; }

    /// <summary>
    /// Size will be defined for the max attribute(s) of the element style.
    /// </summary>
    public bool IsMax { get; set; }

    /// <summary>
    /// Size will be defined for the viewport.
    /// </summary>
    public bool IsViewport { get; set; }

    /// <summary>
    /// Defines the media breakpoint where the sizing rule will be applied.
    /// </summary>
    public Breakpoint Breakpoint { get; set; }
}

/// <summary>
/// Default builder implementation of <see cref="IFluentSizing"/>.
/// </summary>
public class FluentSizing :
    IFluentSizing,
    IFluentSizingSize,
    IFluentSizingMin,
    IFluentSizingViewport,
    IFluentSizingMax,
    IFluentSizingMinMaxViewport,
    IFluentSizingMinMaxViewportOnBreakpoint,
    IFluentSizingWithSizeWithMinMaxWithViewportAll,
    IFluentSizingWithSizeOnBreakpoint,
    IFluentSizingOnBreakpoint
{
    #region Members

    /// <summary>
    /// Currently used sizing type.
    /// </summary>
    private readonly SizingType sizingType;

    /// <summary>
    /// Currently used sizing rules.
    /// </summary>
    private SizingDefinition currentSizingDefinition;

    /// <summary>
    /// Holds the list of defined sizing rules.
    /// </summary>
    private readonly Dictionary<SizingSize, List<SizingDefinition>> rules = new();

    /// <summary>
    /// Holds the list of defined sizing rules for styles.
    /// </summary>
    private Dictionary<string, double> styleRules;

    /// <summary>
    /// Indicates if the rules have changed.
    /// </summary>
    private bool dirty = true;

    /// <summary>
    /// Indicates if the style rules have changed.
    /// </summary>
    private bool styleDirty = true;

    /// <summary>
    /// Holds the built classnames based on the sizing rules.
    /// </summary>
    private string classNames;

    /// <summary>
    /// Holds the built stylenames based on the sizing rules.
    /// </summary>
    private string styleNames;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="FluentSizing"/> constructor.
    /// </summary>
    /// <param name="sizingType">Sizing builder type.</param>
    public FluentSizing( SizingType sizingType )
    {
        this.sizingType = sizingType;
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
                if ( rules is not null && rules.Count > 0 )
                    builder.Append( rules.Select( r => classProvider.Sizing( sizingType, r.Key, r.Value ) ) );
            }

            var classBuilder = new ClassBuilder( BuildClasses );

            classNames = classBuilder.Class;

            dirty = false;
        }

        return classNames;
    }

    /// <inheritdoc/>
    public string Style( IStyleProvider styleProvider )
    {
        if ( styleDirty )
        {
            void BuildStyles( StyleBuilder builder )
            {
                if ( styleRules?.Count > 0 )
                {
                    var sizingTypeName = sizingType == SizingType.Width
                        ? "width"
                        : "height";

                    builder.Append( string.Join( ";", styleRules.Select( r => $"{sizingTypeName}:{r.Value:G29}{r.Key}" ) ) );
                }
            }

            var styleBuilder = new StyleBuilder( BuildStyles );

            styleNames = styleBuilder.Styles;

            styleDirty = false;
        }

        return styleNames;
    }

    /// <summary>
    /// Flags the classnames to be rebuilt.
    /// </summary>
    private void Dirty()
    {
        dirty = true;
    }

    /// <summary>
    /// Starts the new sizing rule.
    /// </summary>
    /// <param name="sizingSize">Size of the element.</param>
    /// <returns>Next rule reference.</returns>returns>
    public IFluentSizingMinMaxViewportOnBreakpoint WithSize( SizingSize sizingSize )
    {
        var sizingDefinition = new SizingDefinition { Breakpoint = Breakpoint.None };

        if ( rules.TryGetValue( sizingSize, out var rule ) )
            rule.Add( sizingDefinition );
        else
            rules.Add( sizingSize, new() { sizingDefinition } );

        currentSizingDefinition = sizingDefinition;
        Dirty();

        return this;
    }

    /// <summary>
    /// Starts the new sizing rule for styles.
    /// </summary>
    /// <param name="unit">Unit it the style.</param>
    /// <param name="size">Size of the element.</param>
    /// <returns>Next rule reference.</returns>returns>
    public IFluentSizingMinMaxViewportOnBreakpoint WithSize( string unit, double size )
    {
        styleRules ??= new();

        styleRules[unit] = size;

        return this;
    }

    /// <summary>
    /// Sets the min rule for the current definition.
    /// </summary>
    /// <returns>Next rule reference.</returns>
    public IFluentSizingViewport WithMin()
    {
        currentSizingDefinition.IsMin = true;
        Dirty();

        return this;
    }

    /// <summary>
    /// Sets the max rule for the current definition.
    /// </summary>
    /// <returns>Next rule reference.</returns>
    public IFluentSizingWithSizeOnBreakpoint WithMax()
    {
        currentSizingDefinition.IsMax = true;
        Dirty();

        return this;
    }

    /// <summary>
    /// Sets the viewport rule for the current definition.
    /// </summary>
    /// <returns>Next rule reference.</returns>
    public IFluentSizing WithViewport()
    {
        currentSizingDefinition.IsViewport = true;
        Dirty();

        return this;
    }

    /// <summary>
    /// Appends the new breakpoint rule.
    /// </summary>
    /// <param name="breakpoint">Breakpoint to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentSizingWithSizeWithMinMaxWithViewportAll WithBreakpoint( Breakpoint breakpoint )
    {
        currentSizingDefinition.Breakpoint = breakpoint;
        Dirty();

        return this;
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    IFluentSizingOnBreakpoint IFluentSizingSize.Is25 => WithSize( SizingSize.Is25 );

    /// <inheritdoc/>
    IFluentSizingOnBreakpoint IFluentSizingSize.Is33 => WithSize( SizingSize.Is33 );

    /// <inheritdoc/>
    IFluentSizingOnBreakpoint IFluentSizingSize.Is50 => WithSize( SizingSize.Is50 );

    /// <inheritdoc/>
    IFluentSizingOnBreakpoint IFluentSizingSize.Is66 => WithSize( SizingSize.Is66 );

    /// <inheritdoc/>
    IFluentSizingOnBreakpoint IFluentSizingSize.Is75 => WithSize( SizingSize.Is75 );

    /// <inheritdoc/>
    IFluentSizingMinMaxViewportOnBreakpoint IFluentSizingSize.Is100 => WithSize( SizingSize.Is100 );

    /// <inheritdoc/>
    IFluentSizing IFluentSizingSize.Auto => WithSize( SizingSize.Auto );

    IFluentSizing IFluentSizingSize.Is( string unit, double size ) => WithSize( unit, size );

    /// <inheritdoc/>
    IFluentSizingViewport IFluentSizingMin.Min => WithMin();

    /// <inheritdoc/>
    IFluentSizingWithSizeOnBreakpoint IFluentSizingMax.Max => WithMax();

    /// <inheritdoc/>
    IFluentSizing IFluentSizingViewport.Viewport => WithViewport();

    /// <inheritdoc/>
    public IFluentSizingWithSizeWithMinMaxWithViewportAll OnMobile => WithBreakpoint( Breakpoint.Mobile );

    /// <inheritdoc/>
    public IFluentSizingWithSizeWithMinMaxWithViewportAll OnTablet => WithBreakpoint( Breakpoint.Tablet );

    /// <inheritdoc/>
    public IFluentSizingWithSizeWithMinMaxWithViewportAll OnDesktop => WithBreakpoint( Breakpoint.Desktop );

    /// <inheritdoc/>
    public IFluentSizingWithSizeWithMinMaxWithViewportAll OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

    /// <inheritdoc/>
    public IFluentSizingWithSizeWithMinMaxWithViewportAll OnFullHD => WithBreakpoint( Breakpoint.FullHD );

    #endregion
}

/// <summary>
/// Set of width sizing rules to start the build process.
/// </summary>
public static class Width
{
    /// <summary>
    /// An element will occupy 25% of its parent space.
    /// </summary>
    public static IFluentSizingMinMaxViewportOnBreakpoint Is25 => new FluentSizing( SizingType.Width ).WithSize( SizingSize.Is25 );

    /// <summary>
    /// An element will occupy third of its parent space.
    /// </summary>
    public static IFluentSizingMinMaxViewportOnBreakpoint Is33 => new FluentSizing( SizingType.Width ).WithSize( SizingSize.Is33 );

    /// <summary>
    /// An element will occupy 50% of its parent space.
    /// </summary>
    public static IFluentSizingMinMaxViewportOnBreakpoint Is50 => new FluentSizing( SizingType.Width ).WithSize( SizingSize.Is50 );

    /// <summary>
    /// An element will occupy two thirds of its parent space.
    /// </summary>
    public static IFluentSizingMinMaxViewportOnBreakpoint Is66 => new FluentSizing( SizingType.Width ).WithSize( SizingSize.Is66 );

    /// <summary>
    /// An element will occupy 75% of its parent space.
    /// </summary>
    public static IFluentSizingMinMaxViewportOnBreakpoint Is75 => new FluentSizing( SizingType.Width ).WithSize( SizingSize.Is75 );

    /// <summary>
    /// An element will occupy 100% of its parent space.
    /// </summary>
    public static IFluentSizingMinMaxViewportOnBreakpoint Is100 => new FluentSizing( SizingType.Width ).WithSize( SizingSize.Is100 );

    /// <summary>
    /// The browser calculates the size.
    /// </summary>
    public static IFluentSizingMinMaxViewportOnBreakpoint Auto => new FluentSizing( SizingType.Width ).WithSize( SizingSize.Auto );

    /// <summary>
    /// Defines the manual size in pixels.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizing"/> reference.</returns>
    public static IFluentSizing Px( double size ) => new FluentSizing( SizingType.Width ).WithSize( "px", size );

    /// <summary>
    /// Defines the manual size in pixels.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizing"/> reference.</returns>
    public static IFluentSizing Rem( double size ) => new FluentSizing( SizingType.Width ).WithSize( "rem", size );

    /// <summary>
    /// Defines the manual size in pixels.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizing"/> reference.</returns>
    public static IFluentSizing Em( double size ) => new FluentSizing( SizingType.Width ).WithSize( "em", size );

    /// <summary>
    /// Defines the maximum allowed element width. Shorthand for "Width.Is100.Max".
    /// </summary>
    public static IFluentSizingWithSizeOnBreakpoint Max100 => new FluentSizing( SizingType.Width ).WithSize( SizingSize.Is100 ).Max;
}

/// <summary>
/// Set of height sizing rules to start the build process.
/// </summary>
public static class Height
{
    /// <summary>
    /// An element will occupy 25% of its parent space.
    /// </summary>
    public static IFluentSizingMinMaxViewportOnBreakpoint Is25 => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Is25 );

    /// <summary>
    /// An element will occupy third of its parent space.
    /// </summary>
    public static IFluentSizingMinMaxViewportOnBreakpoint Is33 => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Is33 );

    /// <summary>
    /// An element will occupy 50% of its parent space.
    /// </summary>
    public static IFluentSizingMinMaxViewportOnBreakpoint Is50 => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Is50 );

    /// <summary>
    /// An element will occupy two thirds of its parent space.
    /// </summary>
    public static IFluentSizingMinMaxViewportOnBreakpoint Is66 => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Is66 );

    /// <summary>
    /// An element will occupy 75% of its parent space.
    /// </summary>
    public static IFluentSizingMinMaxViewportOnBreakpoint Is75 => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Is75 );

    /// <summary>
    /// An element will occupy 100% of its parent space.
    /// </summary>
    public static IFluentSizingMinMaxViewportOnBreakpoint Is100 => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Is100 );

    /// <summary>
    /// The browser calculates the size.
    /// </summary>
    public static IFluentSizingMinMaxViewportOnBreakpoint Auto => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Auto );

    /// <summary>
    /// Defines the manual size in pixels.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizing"/> reference.</returns>
    public static IFluentSizing Px( double size ) => new FluentSizing( SizingType.Height ).WithSize( "px", size );

    /// <summary>
    /// Defines the manual size in pixels.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizing"/> reference.</returns>
    public static IFluentSizing Rem( double size ) => new FluentSizing( SizingType.Height ).WithSize( "rem", size );

    /// <summary>
    /// Defines the manual size in pixels.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizing"/> reference.</returns>
    public static IFluentSizing Em( double size ) => new FluentSizing( SizingType.Height ).WithSize( "em", size );

    /// <summary>
    /// Defines the maximum allowed element height. Shorthand for "Height.Is100.Max".
    /// </summary>
    public static IFluentSizingWithSizeOnBreakpoint Max100 => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Is100 ).Max;
}