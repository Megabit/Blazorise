#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent overflow builders.
/// </summary>
public interface IFluentOverflow : IFluentUtilityTarget<IFluentOverflow>
{
    /// <summary>
    /// Builds the classnames based on overflow rules.
    /// </summary>
    /// <param name="classProvider">Currently used class provider.</param>
    /// <returns>List of classnames for the given rules and the class provider.</returns>
    string Class( IClassProvider classProvider );
}

/// <summary>
/// Second set of overflow rules.
/// </summary>
public interface IFluentOverflowSecondRule :
    IFluentOverflow
{
    /// <summary>
    /// The overflow is not clipped. The content renders outside the element's box.
    /// </summary>
    IFluentOverflow Visible { get; }

    /// <summary>
    /// The overflow is clipped, and the rest of the content will be invisible.
    /// </summary>
    IFluentOverflow Hidden { get; }

    /// <summary>
    /// The overflow is clipped, and a scrollbar is added to see the rest of the content.
    /// </summary>
    IFluentOverflow Scroll { get; }

    /// <summary>
    /// Similar to scroll, but it adds scrollbars only when necessary.
    /// </summary>
    IFluentOverflow Auto { get; }
}

/// <summary>
/// Default implementation of fluent overflow builder.
/// </summary>
public class FluentOverflow :
    IFluentOverflow,
    IFluentOverflowSecondRule,
    IUtilityTargeted
{
    #region Members

    private static readonly FluentUtilityBuilderCache<FluentOverflow> cachedOperations = new( static ( _, cache ) => new() { cache = cache }, static builder => ref builder.cache );

    private FluentUtilityState<FluentOverflow> cache;

    private UtilityTarget? utilityTarget;

    /// <summary>
    /// Holds the default overflow type.
    /// </summary>
    private OverflowType overflowType;

    /// <summary>
    /// Holds the second overflow type.
    /// </summary>
    private OverflowType secondOverflowType;

    /// <summary>
    /// Indicates if the rules have changed.
    /// </summary>
    private bool dirty = true;

    /// <summary>
    /// Holds the built classnames bases on the overflow rules.
    /// </summary>
    private string classNames;

    #endregion

    #region Methods

    internal static FluentOverflow CreateCached() => cachedOperations.CreateRoot();

    /// <inheritdoc/>
    public string Class( IClassProvider classProvider )
    {
        if ( cache.IsCached )
        {
            return cache.GetClass( this, classProvider, static ( builder, provider ) => cachedOperations.Build( builder ).Class( (IClassProvider)provider ) );
        }

        if ( dirty )
        {
            if ( overflowType != OverflowType.Default )
            {
                classNames = classProvider.Overflow( overflowType, secondOverflowType );
            }

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

    private IFluentOverflow WithUtilityTarget( UtilityTarget target )
    {
        if ( cache.IsCached )
        {
            return cachedOperations.With( this, cache, static ( builder, value, _ ) => builder.UtilityTarget = (UtilityTarget)value, value: (int)target, target: target );
        }

        UtilityTarget = target;

        return this;
    }

    /// <summary>
    /// Starts the new overflow rule.
    /// </summary>
    /// <param name="overflowType">Overflow type to be applied.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentOverflowSecondRule WithOverflow( OverflowType overflowType )
    {
        if ( cache.IsCached )
        {
            return cachedOperations.With( this, cache, static ( builder, value, _ ) => builder.WithOverflow( (OverflowType)value ), value: (int)overflowType );
        }

        this.overflowType = overflowType;
        Dirty();

        return this;
    }

    /// <summary>
    /// Starts the new overflow rule.
    /// </summary>
    /// <param name="overflowType">Overflow type to be applied.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentOverflowSecondRule WithSecondOverflow( OverflowType overflowType )
    {
        if ( cache.IsCached )
        {
            return cachedOperations.With( this, cache, static ( builder, value, _ ) => builder.WithSecondOverflow( (OverflowType)value ), value: (int)overflowType );
        }

        this.secondOverflowType = overflowType;
        Dirty();

        return this;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the utility target override.
    /// </summary>
    public UtilityTarget? UtilityTarget
    {
        get => cache.IsCached ? cache.Target : utilityTarget;
        set
        {
            if ( cache.IsCached )
            {
                throw new InvalidOperationException( "Cached fluent utilities are immutable. Use OnSelf or OnWrapper to change the utility target." );
            }

            utilityTarget = value;
        }
    }

    /// <summary>
    /// Targets the utility output to the component element.
    /// </summary>
    public IFluentOverflow OnSelf => WithUtilityTarget( Blazorise.UtilityTarget.Self );

    /// <summary>
    /// Targets the utility output to a wrapper element.
    /// </summary>
    public IFluentOverflow OnWrapper => WithUtilityTarget( Blazorise.UtilityTarget.Wrapper );

    /// <inheritdoc/>
    public IFluentOverflow Visible => WithSecondOverflow( OverflowType.Visible );

    /// <inheritdoc/>
    public IFluentOverflow Hidden => WithSecondOverflow( OverflowType.Hidden );

    /// <inheritdoc/>
    public IFluentOverflow Scroll => WithSecondOverflow( OverflowType.Scroll );

    /// <inheritdoc/>
    public IFluentOverflow Auto => WithSecondOverflow( OverflowType.Auto );

    #endregion
}

/// <summary>
/// The overflow property controls what happens to content that is too big to fit into an area.
/// </summary>
public static class Overflow
{
    private static readonly FluentOverflow cached = FluentOverflow.CreateCached();

    /// <summary>
    /// The overflow is not clipped. The content renders outside the element's box.
    /// </summary>
    public static IFluentOverflowSecondRule Visible => cached.WithOverflow( OverflowType.Visible );

    /// <summary>
    /// The overflow is clipped, and the rest of the content will be invisible.
    /// </summary>
    public static IFluentOverflowSecondRule Hidden => cached.WithOverflow( OverflowType.Hidden );

    /// <summary>
    /// The overflow is clipped, and a scrollbar is added to see the rest of the content.
    /// </summary>
    public static IFluentOverflowSecondRule Scroll => cached.WithOverflow( OverflowType.Scroll );

    /// <summary>
    /// Similar to scroll, but it adds scrollbars only when necessary.
    /// </summary>
    public static IFluentOverflowSecondRule Auto => cached.WithOverflow( OverflowType.Auto );
}