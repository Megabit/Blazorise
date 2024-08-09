#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent object-fit builders.
/// </summary>
public interface IFluentObjectFit
{
    /// <summary>
    /// Builds the classnames based on object-fit rules.
    /// </summary>
    /// <param name="classProvider">Currently used class provider.</param>
    /// <returns>List of classnames for the given rules and the class provider.</returns>
    string Class( IClassProvider classProvider );
}

/// <summary>
/// Breakpoints allowed for object-fit.
/// </summary>
public interface IFluentObjectFitOnBreakpoint :
    IFluentObjectFit
{
    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    IFluentObjectFitWithSize OnMobile { get; }

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    IFluentObjectFitWithSize OnTablet { get; }

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    IFluentObjectFitWithSize OnDesktop { get; }

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    IFluentObjectFitWithSize OnWidescreen { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    IFluentObjectFitWithSize OnFullHD { get; }

    /// <summary>
    /// Breakpoint on extra large desktops (extra extra large).
    /// </summary>
    IFluentObjectFitWithSize OnQuadHD { get; }
}

/// <summary>
/// Sizes allowed for object-fit.
/// </summary>
public interface IFluentObjectFitWithSize
    : IFluentObjectFit
{
    /// <summary>
    /// No particular size rule will be applied, meaning a default size will be used.
    /// </summary>
    IFluentObjectFitOnBreakpoint Default { get; }

    /// <summary>
    /// Makes an element text extra small size.
    /// </summary>
    IFluentObjectFitOnBreakpoint None { get; }

    /// <summary>
    /// Makes an element text small size.
    /// </summary>
    IFluentObjectFitOnBreakpoint Contain { get; }

    /// <summary>
    /// Makes an element text medium size.
    /// </summary>
    IFluentObjectFitOnBreakpoint Fill { get; }

    /// <summary>
    /// Makes an element text large.
    /// </summary>
    IFluentObjectFitOnBreakpoint Scale { get; }

    /// <summary>
    /// Makes an element text extra large.
    /// </summary>
    IFluentObjectFitOnBreakpoint Cover { get; }

    /// <summary>
    /// Matches the element object-fit with the h1 object-fit.
    /// </summary>
    IFluentObjectFitOnBreakpoint Heading1 { get; }

    /// <summary>
    /// Matches the element object-fit with the h2 object-fit.
    /// </summary>
    IFluentObjectFitOnBreakpoint Heading2 { get; }

    /// <summary>
    /// Matches the element object-fit with the h3 object-fit.
    /// </summary>
    IFluentObjectFitOnBreakpoint Heading3 { get; }

    /// <summary>
    /// Matches the element object-fit with the h4 object-fit.
    /// </summary>
    IFluentObjectFitOnBreakpoint Heading4 { get; }

    /// <summary>
    /// Matches the element object-fit with the h5 object-fit.
    /// </summary>
    IFluentObjectFitOnBreakpoint Heading5 { get; }

    /// <summary>
    /// Matches the element object-fit with the h6 object-fit.
    /// </summary>
    IFluentObjectFitOnBreakpoint Heading6 { get; }
}

/// <summary>
/// Defines the size information for the breakpoint.
/// </summary>
public sealed class ObjectFitDefinition
{
    /// <summary>
    /// Defines the breakpoint on which the object-fit will apply.
    /// </summary>
    public Breakpoint Breakpoint { get; set; }
}

/// <summary>
/// Default implementation of fluent object-fit builder.
/// </summary>
public class FluentObjectFit :
    IFluentObjectFitOnBreakpoint,
    IFluentObjectFitWithSize
{
    #region Members

    private ObjectFitDefinition currentObjectFitDefintion;

    private readonly Dictionary<ObjectFitType, ObjectFitDefinition> rules = new();

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
                        .Where( r => r.Key != ObjectFitType.Default || r.Value.Breakpoint != Breakpoint.None )
                        .Select( r => classProvider.ObjectFit( r.Key, r.Value ) ) );
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
    /// Starts the new object-fit.
    /// </summary>
    /// <param name="objectFit">Object-fit to start.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentObjectFitOnBreakpoint WithSize( ObjectFitType objectFit )
    {
        var objectFitDefinition = new ObjectFitDefinition { Breakpoint = Breakpoint.None };

        rules.Add( objectFit, objectFitDefinition );

        currentObjectFitDefintion = objectFitDefinition;
        Dirty();

        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="breakpoint"></param>
    /// <returns>Next rule reference.</returns>
    public IFluentObjectFitWithSize WithBreakpoint( Breakpoint breakpoint )
    {
        currentObjectFitDefintion.Breakpoint = breakpoint;
        Dirty();

        return this;
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public IFluentObjectFitWithSize OnMobile => WithBreakpoint( Breakpoint.Mobile );

    /// <inheritdoc/>
    public IFluentObjectFitWithSize OnTablet => WithBreakpoint( Breakpoint.Tablet );

    /// <inheritdoc/>
    public IFluentObjectFitWithSize OnDesktop => WithBreakpoint( Breakpoint.Desktop );

    /// <inheritdoc/>
    public IFluentObjectFitWithSize OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

    /// <inheritdoc/>
    public IFluentObjectFitWithSize OnFullHD => WithBreakpoint( Breakpoint.FullHD );

    /// <inheritdoc/>
    public IFluentObjectFitWithSize OnQuadHD => WithBreakpoint( Breakpoint.QuadHD );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint Default => WithSize( ObjectFitType.Default );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint ExtraSmall => WithSize( ObjectFitType.ExtraSmall );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint Small => WithSize( ObjectFitType.Small );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint Medium => WithSize( ObjectFitType.Medium );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint Large => WithSize( ObjectFitType.Large );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint ExtraLarge => WithSize( ObjectFitType.ExtraLarge );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint Heading1 => WithSize( ObjectFitType.Heading1 );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint Heading2 => WithSize( ObjectFitType.Heading2 );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint Heading3 => WithSize( ObjectFitType.Heading3 );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint Heading4 => WithSize( ObjectFitType.Heading4 );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint Heading5 => WithSize( ObjectFitType.Heading5 );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint Heading6 => WithSize( ObjectFitType.Heading6 );

    #endregion
}

/// <summary>
/// Defines the object-fit.
/// </summary>
public static class ObjectFit
{
    /// <summary>
    /// No particular size rule will be applied, meaning a default size will be used.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Default => new FluentObjectFit().Default;

    /// <summary>
    /// Makes an element text extra small size.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint ExtraSmall => new FluentObjectFit().ExtraSmall;

    /// <summary>
    /// Makes an element text small size.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Small => new FluentObjectFit().Small;

    /// <summary>
    /// Makes an element text medium size.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Medium => new FluentObjectFit().Medium;

    /// <summary>
    /// Makes an element text large.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Large => new FluentObjectFit().Large;

    /// <summary>
    /// Makes an element text extra large.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint ExtraLarge => new FluentObjectFit().ExtraLarge;

    /// <summary>
    /// Matches the element object-fit with the h1 object-fit.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Heading1 => new FluentObjectFit().Heading1;

    /// <summary>
    /// Matches the element object-fit with the h2 object-fit.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Heading2 => new FluentObjectFit().Heading2;

    /// <summary>
    /// Matches the element object-fit with the h3 object-fit.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Heading3 => new FluentObjectFit().Heading3;

    /// <summary>
    /// Matches the element object-fit with the h4 object-fit.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Heading4 => new FluentObjectFit().Heading4;

    /// <summary>
    /// Matches the element object-fit with the h5 object-fit.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Heading5 => new FluentObjectFit().Heading5;

    /// <summary>
    /// Matches the element object-fit with the h6 object-fit.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Heading6 => new FluentObjectFit().Heading6;
}