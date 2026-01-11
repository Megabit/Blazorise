#region Using directives
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Blazorise.Extensions;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent sizing builders.
/// </summary>
public interface IFluentSizing : IFluentUtilityTarget<IFluentSizing>
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

    /// <summary>
    /// Gets the fixed size value, or null if it's undefined.
    /// </summary>
    double? FixedSize { get; }
}

/// <summary>
/// Contains all the allowed sizing style rules.
/// </summary>
public interface IFluentSizingStyle :
    IFluentSizing
{
    /// <summary>
    /// Defines the minimum size using the same unit as the fixed size.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    IFluentSizingStyle Min( double size );

    /// <summary>
    /// Defines the maximum size using the same unit as the fixed size.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    IFluentSizingStyle Max( double size );
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

    /// <summary>
    /// Breakpoint on large desktops (extra extra large).
    /// </summary>
    IFluentSizingWithSizeWithMinMaxWithViewportAll OnQuadHD { get; }
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
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    IFluentSizingStyle Is( string unit, double size );
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
/// Holds the build information for fixes sizing rules.
/// </summary>
public record FixedSizingDefiniton
{
    /// <summary>
    /// Defines the CSS unit name.
    /// </summary>
    public string Unit { get; set; }

    /// <summary>
    /// Defines the size value.
    /// </summary>
    public double? Size { get; set; }

    /// <summary>
    /// Defines the full CSS value.
    /// </summary>
    public string Value { get; set; }
}

/// <summary>
/// Default builder implementation of <see cref="IFluentSizing"/>.
/// </summary>
public class FluentSizing :
    IFluentSizing,
    IFluentSizingStyle,
    IFluentSizingSize,
    IFluentSizingMin,
    IFluentSizingViewport,
    IFluentSizingMax,
    IFluentSizingMinMaxViewport,
    IFluentSizingMinMaxViewportOnBreakpoint,
    IFluentSizingWithSizeWithMinMaxWithViewportAll,
    IFluentSizingWithSizeOnBreakpoint,
    IFluentSizingOnBreakpoint,
    IUtilityTargeted
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
    /// Holds the sizing rules for the style.
    /// </summary>
    private FixedSizingDefiniton styleRule;

    /// <summary>
    /// Holds the sizing rules for the minimum style.
    /// </summary>
    private FixedSizingDefiniton minStyleRule;

    /// <summary>
    /// Holds the sizing rules for the maximum style.
    /// </summary>
    private FixedSizingDefiniton maxStyleRule;

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
                if ( !rules.IsNullOrEmpty() )
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
                var sizingTypeName = sizingType == SizingType.Width
                    ? "width"
                    : "height";

                AppendSizingStyle( builder, sizingTypeName, styleRule );
                AppendSizingStyle( builder, $"min-{sizingTypeName}", minStyleRule );
                AppendSizingStyle( builder, $"max-{sizingTypeName}", maxStyleRule );
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
    /// Flags the stylenames to be rebuilt.
    /// </summary>
    private void DirtyStyles()
    {
        styleDirty = true;
    }

    private IFluentSizing WithUtilityTarget( UtilityTarget target )
    {
        UtilityTarget = target;
        return this;
    }

    private static void AppendSizingStyle( StyleBuilder builder, string propertyName, FixedSizingDefiniton sizingDefinition )
    {
        var value = GetSizingValue( sizingDefinition );

        if ( !string.IsNullOrEmpty( value ) )
            builder.Append( $"{propertyName}: {value}" );
    }

    private static string GetSizingValue( FixedSizingDefiniton sizingDefinition )
    {
        if ( sizingDefinition is null )
            return null;

        if ( !string.IsNullOrEmpty( sizingDefinition.Value ) )
            return sizingDefinition.Value;

        if ( sizingDefinition.Size.HasValue && !string.IsNullOrEmpty( sizingDefinition.Unit ) )
            return $"{sizingDefinition.Size.Value.ToString( "G29", CultureInfo.InvariantCulture )}{sizingDefinition.Unit}";

        return null;
    }

    private static string GetCssVariableValue( string variable )
    {
        if ( string.IsNullOrWhiteSpace( variable ) )
            return null;

        var trimmedVariable = variable.Trim();

        if ( trimmedVariable.StartsWith( "var(", System.StringComparison.Ordinal ) )
            return trimmedVariable;

        if ( !trimmedVariable.StartsWith( "--", System.StringComparison.Ordinal ) )
            trimmedVariable = $"--{trimmedVariable}";

        return $"var({trimmedVariable})";
    }

    /// <summary>
    /// Starts the new sizing rule.
    /// </summary>
    /// <param name="sizingSize">Size of the element.</param>
    /// <returns>Next rule reference.</returns>
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
    /// <returns>Next rule reference.</returns>
    public IFluentSizingStyle WithSize( string unit, double size )
    {
        styleRule = new()
        {
            Unit = unit,
            Size = size,
        };

        minStyleRule = null;
        maxStyleRule = null;
        DirtyStyles();

        return this;
    }

    /// <summary>
    /// Starts the new sizing rule for styles by defining only the unit.
    /// </summary>
    /// <param name="unit">Unit it the style.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentSizingStyle WithUnit( string unit )
    {
        styleRule = new()
        {
            Unit = unit,
        };

        minStyleRule = null;
        maxStyleRule = null;
        DirtyStyles();

        return this;
    }

    /// <summary>
    /// Starts the new sizing rule for styles with a CSS variable.
    /// </summary>
    /// <param name="variable">CSS variable name.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentSizingStyle WithVariable( string variable )
    {
        var value = GetCssVariableValue( variable );

        if ( string.IsNullOrEmpty( value ) )
            return this;

        styleRule = new()
        {
            Value = value,
        };

        minStyleRule = null;
        maxStyleRule = null;
        DirtyStyles();

        return this;
    }

    /// <summary>
    /// Defines the minimum style size.
    /// </summary>
    /// <param name="size">Size of the element.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentSizingStyle Min( double size )
    {
        if ( string.IsNullOrEmpty( styleRule?.Unit ) )
            return this;

        minStyleRule = new()
        {
            Unit = styleRule.Unit,
            Size = size,
        };

        DirtyStyles();

        return this;
    }

    /// <summary>
    /// Defines the maximum style size.
    /// </summary>
    /// <param name="size">Size of the element.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentSizingStyle Max( double size )
    {
        if ( string.IsNullOrEmpty( styleRule?.Unit ) )
            return this;

        maxStyleRule = new()
        {
            Unit = styleRule.Unit,
            Size = size,
        };

        DirtyStyles();

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

    /// <inheritdoc/>
    public double? FixedSize
        => styleRule?.Size;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the utility target override.
    /// </summary>
    public UtilityTarget? UtilityTarget { get; set; }

    /// <summary>
    /// Targets the utility output to the component element.
    /// </summary>
    public IFluentSizing OnSelf => WithUtilityTarget( Blazorise.UtilityTarget.Self );

    /// <summary>
    /// Targets the utility output to a wrapper element.
    /// </summary>
    public IFluentSizing OnWrapper => WithUtilityTarget( Blazorise.UtilityTarget.Wrapper );

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

    IFluentSizingStyle IFluentSizingSize.Is( string unit, double size ) => WithSize( unit, size );

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

    /// <inheritdoc/>
    public IFluentSizingWithSizeWithMinMaxWithViewportAll OnQuadHD => WithBreakpoint( Breakpoint.QuadHD );

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
    /// Defines the element size in pixels (1px = 1/96th of 1in).
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <remarks>
    /// Pixels (px) are relative to the viewing device. For low-dpi devices, 1px is one device pixel (dot) of the display. For printers and high resolution screens 1px implies multiple device pixels.
    /// </remarks>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Px( double size ) => new FluentSizing( SizingType.Width ).WithSize( "px", size );

    /// <summary>
    /// Defines the element size in pixels (1px = 1/96th of 1in).
    /// </summary>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Px() => new FluentSizing( SizingType.Width ).WithUnit( "px" );

    /// <summary>
    /// Defines the element size, relative to font-size of the root element.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Rem( double size ) => new FluentSizing( SizingType.Width ).WithSize( "rem", size );

    /// <summary>
    /// Defines the element size, relative to font-size of the root element.
    /// </summary>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Rem() => new FluentSizing( SizingType.Width ).WithUnit( "rem" );

    /// <summary>
    /// Defines the element size, relative to the font-size of the element (2em means 2 times the size of the current font).
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Em( double size ) => new FluentSizing( SizingType.Width ).WithSize( "em", size );

    /// <summary>
    /// Defines the element size, relative to the font-size of the element (2em means 2 times the size of the current font).
    /// </summary>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Em() => new FluentSizing( SizingType.Width ).WithUnit( "em" );

    /// <summary>
    /// Defines the advance measure (width) of the glyph "0" of the element's font.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Ch( double size ) => new FluentSizing( SizingType.Width ).WithSize( "ch", size );

    /// <summary>
    /// Defines the advance measure (width) of the glyph "0" of the element's font.
    /// </summary>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Ch() => new FluentSizing( SizingType.Width ).WithUnit( "ch" );

    /// <summary>
    /// Defines the element size, relative to the viewport's width.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Vw( double size ) => new FluentSizing( SizingType.Width ).WithSize( "vw", size );

    /// <summary>
    /// Defines the element size, relative to the viewport's width.
    /// </summary>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Vw() => new FluentSizing( SizingType.Width ).WithUnit( "vw" );

    /// <summary>
    /// Defines the element size with a CSS variable.
    /// </summary>
    /// <param name="variable">CSS variable name.</param>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Var( string variable ) => new FluentSizing( SizingType.Width ).WithVariable( variable );

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
    /// Defines the element size in pixels (1px = 1/96th of 1in).
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <remarks>
    /// Pixels (px) are relative to the viewing device. For low-dpi devices, 1px is one device pixel (dot) of the display. For printers and high resolution screens 1px implies multiple device pixels.
    /// </remarks>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Px( double size ) => new FluentSizing( SizingType.Height ).WithSize( "px", size );

    /// <summary>
    /// Defines the element size in pixels (1px = 1/96th of 1in).
    /// </summary>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Px() => new FluentSizing( SizingType.Height ).WithUnit( "px" );

    /// <summary>
    /// Defines the element size, relative to font-size of the root element.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Rem( double size ) => new FluentSizing( SizingType.Height ).WithSize( "rem", size );

    /// <summary>
    /// Defines the element size, relative to font-size of the root element.
    /// </summary>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Rem() => new FluentSizing( SizingType.Height ).WithUnit( "rem" );

    /// <summary>
    /// Defines the element size, relative to the font-size of the element (2em means 2 times the size of the current font).
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Em( double size ) => new FluentSizing( SizingType.Height ).WithSize( "em", size );

    /// <summary>
    /// Defines the element size, relative to the font-size of the element (2em means 2 times the size of the current font).
    /// </summary>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Em() => new FluentSizing( SizingType.Height ).WithUnit( "em" );

    /// <summary>
    /// Defines the advance measure (width) of the glyph "0" of the element's font.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Ch( double size ) => new FluentSizing( SizingType.Height ).WithSize( "ch", size );

    /// <summary>
    /// Defines the advance measure (width) of the glyph "0" of the element's font.
    /// </summary>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Ch() => new FluentSizing( SizingType.Height ).WithUnit( "ch" );

    /// <summary>
    /// Defines the element size, relative to the viewport's height.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Vh( double size ) => new FluentSizing( SizingType.Height ).WithSize( "vh", size );

    /// <summary>
    /// Defines the element size, relative to the viewport's height.
    /// </summary>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Vh() => new FluentSizing( SizingType.Height ).WithUnit( "vh" );

    /// <summary>
    /// Defines the element size with a CSS variable.
    /// </summary>
    /// <param name="variable">CSS variable name.</param>
    /// <returns>Returns the <see cref="IFluentSizingStyle"/> reference.</returns>
    public static IFluentSizingStyle Var( string variable ) => new FluentSizing( SizingType.Height ).WithVariable( variable );

    /// <summary>
    /// Defines the maximum allowed element height. Shorthand for "Height.Is100.Max".
    /// </summary>
    public static IFluentSizingWithSizeOnBreakpoint Max100 => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Is100 ).Max;
}