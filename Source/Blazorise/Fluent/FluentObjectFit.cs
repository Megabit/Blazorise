#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent object-fit builders.
/// </summary>
public interface IFluentObjectFit : IFluentUtilityTarget<IFluentObjectFit>
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
    /// The default object fit behavior.
    /// </summary>
    IFluentObjectFitOnBreakpoint Default { get; }

    /// <summary>
    /// The object does not scale to fit the container and may be clipped.
    /// </summary>
    IFluentObjectFitOnBreakpoint None { get; }

    /// <summary>
    /// The object is scaled to maintain its aspect ratio while fitting within the container.
    /// </summary>
    IFluentObjectFitOnBreakpoint Contain { get; }

    /// <summary>
    /// The object is scaled to cover the entire container, possibly clipping some parts.
    /// </summary>
    IFluentObjectFitOnBreakpoint Cover { get; }

    /// <summary>
    /// The object is scaled to completely fill the container. 
    /// Its aspect ratio is not preserved, and the object may be stretched or compressed.
    /// </summary>
    IFluentObjectFitOnBreakpoint Fill { get; }

    /// <summary>
    /// The object is scaled to fill the container without preserving its aspect ratio.
    /// </summary>
    IFluentObjectFitOnBreakpoint Scale { get; }
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
    IFluentObjectFitWithSize,
    IUtilityTargeted
{
    #region Members

    private ObjectFitDefinition currentObjectFitDefinition;

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

    private IFluentObjectFit WithUtilityTarget( UtilityTarget target )
    {
        UtilityTarget = target;
        return this;
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

        currentObjectFitDefinition = objectFitDefinition;
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
        currentObjectFitDefinition.Breakpoint = breakpoint;
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
    public IFluentObjectFit OnSelf => WithUtilityTarget( Blazorise.UtilityTarget.Self );

    /// <summary>
    /// Targets the utility output to a wrapper element.
    /// </summary>
    public IFluentObjectFit OnWrapper => WithUtilityTarget( Blazorise.UtilityTarget.Wrapper );

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
    public IFluentObjectFitOnBreakpoint None => WithSize( ObjectFitType.None );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint Contain => WithSize( ObjectFitType.Contain );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint Cover => WithSize( ObjectFitType.Cover );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint Fill => WithSize( ObjectFitType.Fill );

    /// <inheritdoc/>
    public IFluentObjectFitOnBreakpoint Scale => WithSize( ObjectFitType.Scale );

    #endregion
}

/// <summary>
/// Defines the object-fit.
/// </summary>
public static class ObjectFit
{
    /// <summary>
    /// The default object fit behavior.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Default => new FluentObjectFit().Default;

    /// <summary>
    /// The object does not scale to fit the container and may be clipped.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint None => new FluentObjectFit().None;

    /// <summary>
    /// The object is scaled to maintain its aspect ratio while fitting within the container.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Contain => new FluentObjectFit().Contain;

    /// <summary>
    /// The object is scaled to cover the entire container, possibly clipping some parts.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Cover => new FluentObjectFit().Cover;

    /// <summary>
    /// The object is scaled to completely fill the container. 
    /// Its aspect ratio is not preserved, and the object may be stretched or compressed.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Fill => new FluentObjectFit().Fill;

    /// <summary>
    /// The object is scaled to fill the container without preserving its aspect ratio.
    /// </summary>
    public static IFluentObjectFitOnBreakpoint Scale => new FluentObjectFit().Scale;
}