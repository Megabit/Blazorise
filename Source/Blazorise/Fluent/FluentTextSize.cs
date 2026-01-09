#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent text size builders.
/// </summary>
public interface IFluentTextSize : IFluentUtilityTarget<IFluentTextSize>
{
    /// <summary>
    /// Builds the classnames based on text size rules.
    /// </summary>
    /// <param name="classProvider">Currently used class provider.</param>
    /// <returns>List of classnames for the given rules and the class provider.</returns>
    string Class( IClassProvider classProvider );
}

/// <summary>
/// Breakpoints allowed for text size.
/// </summary>
public interface IFluentTextSizeOnBreakpoint :
    IFluentTextSize
{
    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    IFluentTextSizeWithSize OnMobile { get; }

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    IFluentTextSizeWithSize OnTablet { get; }

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    IFluentTextSizeWithSize OnDesktop { get; }

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    IFluentTextSizeWithSize OnWidescreen { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    IFluentTextSizeWithSize OnFullHD { get; }

    /// <summary>
    /// Breakpoint on extra large desktops (extra extra large).
    /// </summary>
    IFluentTextSizeWithSize OnQuadHD { get; }
}

/// <summary>
/// Sizes allowed for text size.
/// </summary>
public interface IFluentTextSizeWithSize
    : IFluentTextSize
{
    /// <summary>
    /// No particular size rule will be applied, meaning a default size will be used.
    /// </summary>
    IFluentTextSizeOnBreakpoint Default { get; }

    /// <summary>
    /// Makes an element text extra small size.
    /// </summary>
    IFluentTextSizeOnBreakpoint ExtraSmall { get; }

    /// <summary>
    /// Makes an element text small size.
    /// </summary>
    IFluentTextSizeOnBreakpoint Small { get; }

    /// <summary>
    /// Makes an element text medium size.
    /// </summary>
    IFluentTextSizeOnBreakpoint Medium { get; }

    /// <summary>
    /// Makes an element text large.
    /// </summary>
    IFluentTextSizeOnBreakpoint Large { get; }

    /// <summary>
    /// Makes an element text extra large.
    /// </summary>
    IFluentTextSizeOnBreakpoint ExtraLarge { get; }

    /// <summary>
    /// Matches the element text size with the h1 text size.
    /// </summary>
    IFluentTextSizeOnBreakpoint Heading1 { get; }

    /// <summary>
    /// Matches the element text size with the h2 text size.
    /// </summary>
    IFluentTextSizeOnBreakpoint Heading2 { get; }

    /// <summary>
    /// Matches the element text size with the h3 text size.
    /// </summary>
    IFluentTextSizeOnBreakpoint Heading3 { get; }

    /// <summary>
    /// Matches the element text size with the h4 text size.
    /// </summary>
    IFluentTextSizeOnBreakpoint Heading4 { get; }

    /// <summary>
    /// Matches the element text size with the h5 text size.
    /// </summary>
    IFluentTextSizeOnBreakpoint Heading5 { get; }

    /// <summary>
    /// Matches the element text size with the h6 text size.
    /// </summary>
    IFluentTextSizeOnBreakpoint Heading6 { get; }
}

/// <summary>
/// Defines the size information for the breakpoint.
/// </summary>
public sealed class TextSizeDefinition
{
    /// <summary>
    /// Defines the breakpoint on which the text size will apply.
    /// </summary>
    public Breakpoint Breakpoint { get; set; }
}

/// <summary>
/// Default implementation of fluent text size builder.
/// </summary>
public class FluentTextSize :
    IFluentTextSizeOnBreakpoint,
    IFluentTextSizeWithSize,
    IUtilityTargeted
{
    #region Members

    private TextSizeDefinition currentTextSizeDefinition;

    private readonly Dictionary<TextSizeType, TextSizeDefinition> rules = new();

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
                    builder.Append( rules
                        .Where( r => r.Key != TextSizeType.Default || r.Value.Breakpoint != Breakpoint.None )
                        .Select( r => classProvider.TextSize( r.Key, r.Value ) ) );
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

    private IFluentTextSize WithUtilityTarget( UtilityTarget target )
    {
        UtilityTarget = target;
        return this;
    }

    /// <summary>
    /// Starts the new text size.
    /// </summary>
    /// <param name="textSize">Text size to start.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentTextSizeOnBreakpoint WithSize( TextSizeType textSize )
    {
        var textSizeDefinition = new TextSizeDefinition { Breakpoint = Breakpoint.None };

        rules.Add( textSize, textSizeDefinition );

        currentTextSizeDefinition = textSizeDefinition;
        Dirty();

        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="breakpoint"></param>
    /// <returns>Next rule reference.</returns>
    public IFluentTextSizeWithSize WithBreakpoint( Breakpoint breakpoint )
    {
        currentTextSizeDefinition.Breakpoint = breakpoint;
        Dirty();

        return this;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the utility target override.
    /// </summary>
    public UtilityTarget? UtilityTarget { get; set; }

    /// <summary>
    /// Targets the utility output to the component element.
    /// </summary>
    public IFluentTextSize OnSelf => WithUtilityTarget( global::Blazorise.UtilityTarget.Self );

    /// <summary>
    /// Targets the utility output to a wrapper element.
    /// </summary>
    public IFluentTextSize OnWrapper => WithUtilityTarget( global::Blazorise.UtilityTarget.Wrapper );

    /// <inheritdoc/>
    public IFluentTextSizeWithSize OnMobile => WithBreakpoint( Breakpoint.Mobile );

    /// <inheritdoc/>
    public IFluentTextSizeWithSize OnTablet => WithBreakpoint( Breakpoint.Tablet );

    /// <inheritdoc/>
    public IFluentTextSizeWithSize OnDesktop => WithBreakpoint( Breakpoint.Desktop );

    /// <inheritdoc/>
    public IFluentTextSizeWithSize OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

    /// <inheritdoc/>
    public IFluentTextSizeWithSize OnFullHD => WithBreakpoint( Breakpoint.FullHD );

    /// <inheritdoc/>
    public IFluentTextSizeWithSize OnQuadHD => WithBreakpoint( Breakpoint.QuadHD );

    /// <inheritdoc/>
    public IFluentTextSizeOnBreakpoint Default => WithSize( TextSizeType.Default );

    /// <inheritdoc/>
    public IFluentTextSizeOnBreakpoint ExtraSmall => WithSize( TextSizeType.ExtraSmall );

    /// <inheritdoc/>
    public IFluentTextSizeOnBreakpoint Small => WithSize( TextSizeType.Small );

    /// <inheritdoc/>
    public IFluentTextSizeOnBreakpoint Medium => WithSize( TextSizeType.Medium );

    /// <inheritdoc/>
    public IFluentTextSizeOnBreakpoint Large => WithSize( TextSizeType.Large );

    /// <inheritdoc/>
    public IFluentTextSizeOnBreakpoint ExtraLarge => WithSize( TextSizeType.ExtraLarge );

    /// <inheritdoc/>
    public IFluentTextSizeOnBreakpoint Heading1 => WithSize( TextSizeType.Heading1 );

    /// <inheritdoc/>
    public IFluentTextSizeOnBreakpoint Heading2 => WithSize( TextSizeType.Heading2 );

    /// <inheritdoc/>
    public IFluentTextSizeOnBreakpoint Heading3 => WithSize( TextSizeType.Heading3 );

    /// <inheritdoc/>
    public IFluentTextSizeOnBreakpoint Heading4 => WithSize( TextSizeType.Heading4 );

    /// <inheritdoc/>
    public IFluentTextSizeOnBreakpoint Heading5 => WithSize( TextSizeType.Heading5 );

    /// <inheritdoc/>
    public IFluentTextSizeOnBreakpoint Heading6 => WithSize( TextSizeType.Heading6 );

    #endregion
}

/// <summary>
/// Defines the text size.
/// </summary>
public static class TextSize
{
    /// <summary>
    /// No particular size rule will be applied, meaning a default size will be used.
    /// </summary>
    public static IFluentTextSizeOnBreakpoint Default => new FluentTextSize().Default;

    /// <summary>
    /// Makes an element text extra small size.
    /// </summary>
    public static IFluentTextSizeOnBreakpoint ExtraSmall => new FluentTextSize().ExtraSmall;

    /// <summary>
    /// Makes an element text small size.
    /// </summary>
    public static IFluentTextSizeOnBreakpoint Small => new FluentTextSize().Small;

    /// <summary>
    /// Makes an element text medium size.
    /// </summary>
    public static IFluentTextSizeOnBreakpoint Medium => new FluentTextSize().Medium;

    /// <summary>
    /// Makes an element text large.
    /// </summary>
    public static IFluentTextSizeOnBreakpoint Large => new FluentTextSize().Large;

    /// <summary>
    /// Makes an element text extra large.
    /// </summary>
    public static IFluentTextSizeOnBreakpoint ExtraLarge => new FluentTextSize().ExtraLarge;

    /// <summary>
    /// Matches the element text size with the h1 text size.
    /// </summary>
    public static IFluentTextSizeOnBreakpoint Heading1 => new FluentTextSize().Heading1;

    /// <summary>
    /// Matches the element text size with the h2 text size.
    /// </summary>
    public static IFluentTextSizeOnBreakpoint Heading2 => new FluentTextSize().Heading2;

    /// <summary>
    /// Matches the element text size with the h3 text size.
    /// </summary>
    public static IFluentTextSizeOnBreakpoint Heading3 => new FluentTextSize().Heading3;

    /// <summary>
    /// Matches the element text size with the h4 text size.
    /// </summary>
    public static IFluentTextSizeOnBreakpoint Heading4 => new FluentTextSize().Heading4;

    /// <summary>
    /// Matches the element text size with the h5 text size.
    /// </summary>
    public static IFluentTextSizeOnBreakpoint Heading5 => new FluentTextSize().Heading5;

    /// <summary>
    /// Matches the element text size with the h6 text size.
    /// </summary>
    public static IFluentTextSizeOnBreakpoint Heading6 => new FluentTextSize().Heading6;
}