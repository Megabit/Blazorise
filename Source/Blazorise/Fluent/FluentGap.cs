#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent gap builders.
/// </summary>
public interface IFluentGap : IFluentUtilityTarget<IFluentGap>
{
    /// <summary>
    /// Builds the classnames based on gap rules.
    /// </summary>
    /// <param name="classProvider">Currently used class provider.</param>
    /// <returns>List of classnames for the given rules and the class provider.</returns>
    string Class( IClassProvider classProvider );
}

/// <summary>
/// Contains all the allowed gap rules, except sizes.
/// </summary>
public interface IFluentGapWithSide :
    IFluentGap,
    IFluentGapFromSide
{
}

/// <summary>
/// Contains all the allowed gap rules.
/// </summary>
public interface IFluentGapWithSideAndSize :
    IFluentGap,
    IFluentGapFromSide,
    IFluentGapWithSize
{
}

/// <summary>
/// Allowed sides for gap rules.
/// </summary>
public interface IFluentGapFromSide :
    IFluentGap
{
    /// <summary>
    /// For classes that set both *-left and *-right.
    /// </summary>
    IFluentGapWithSideAndSize OnX { get; }

    /// <summary>
    /// For classes that set both *-top and *-bottom.
    /// </summary>
    IFluentGapWithSideAndSize OnY { get; }

    /// <summary>
    /// For classes that set a margin or padding on all 4 sides of the element.
    /// </summary>
    IFluentGapWithSideAndSize OnAll { get; }
}

/// <summary>
/// Allowed sizes for gap rules.
/// </summary>
public interface IFluentGapWithSize :
    IFluentGap
{
    /// <summary>
    /// For classes that eliminate the margin or padding by setting it to 0.
    /// </summary>
    IFluentGapWithSideAndSize Is0 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .25
    /// </summary>
    IFluentGapWithSideAndSize Is1 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .5
    /// </summary>
    IFluentGapWithSideAndSize Is2 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer
    /// </summary>
    IFluentGapWithSideAndSize Is3 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 1.5
    /// </summary>
    IFluentGapWithSideAndSize Is4 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 3
    /// </summary>
    IFluentGapWithSideAndSize Is5 { get; }

    /// <summary>
    /// Used to add custom gap rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    IFluentGapWithSize Is( string value );
}

/// <summary>
/// Default implementation of <see cref="IFluentGap"/>.
/// </summary>
public class FluentGap :
    IFluentGap,
    IFluentGapWithSize,
    IFluentGapFromSide,
    IFluentGapWithSide,
    IFluentGapWithSideAndSize,
    IUtilityTargeted
{
    #region Members

    private static readonly FluentUtilityBuilderCache<FluentGap> cachedOperations = new( static ( _, cache ) => new() { cache = cache }, static builder => ref builder.cache );

    private class GapDefinition
    {
        public GapSide Side { get; set; }
    }

    private GapDefinition currentGap;

    private Dictionary<GapSize, List<GapDefinition>> rules;

    private List<string> customRules;

    private bool dirty = true;

    private string classNames;

    private FluentUtilityState<FluentGap> cache;

    private UtilityTarget? utilityTarget;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public string Class( IClassProvider classProvider )
    {
        if ( cache.IsCached )
        {
            return cache.GetClass( this, classProvider, static ( builder, provider ) => cachedOperations.Build( builder ).Class( (IClassProvider)provider ) );
        }

        if ( dirty )
        {
            classNames = BuildClassNames( classProvider );
            dirty = false;
        }

        return classNames;
    }

    private string BuildClassNames( IClassProvider classProvider )
    {
        void BuildClasses( ClassBuilder builder )
        {
            if ( rules?.Count > 0 )
            {
                builder.Append( rules.Select( r => classProvider.Gap( r.Key, r.Value.Select( v => v.Side ) ) ) );
            }

            if ( customRules?.Count > 0 )
            {
                builder.Append( customRules );
            }
        }

        var classBuilder = new ClassBuilder( BuildClasses );

        return classBuilder.Class;
    }

    internal static FluentGap CreateCached() => cachedOperations.CreateRoot();

    private void Dirty()
    {
        dirty = true;
    }

    private IFluentGap WithUtilityTarget( UtilityTarget target )
    {
        if ( cache.IsCached )
        {
            return cachedOperations.With( this, cache, static ( builder, value, _ ) => builder.UtilityTarget = (UtilityTarget)value, value: (int)target, target: target );
        }

        UtilityTarget = target;

        return this;
    }

    /// <summary>
    /// Appends the new gap size rule.
    /// </summary>
    /// <param name="gapSize">Gap size to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentGapWithSideAndSize WithSize( GapSize gapSize )
    {
        if ( cache.IsCached )
        {
            return cachedOperations.With( this, cache, static ( builder, value, _ ) => builder.WithSize( (GapSize)value ), value: (int)gapSize );
        }

        var gapDefinition = new GapDefinition { Side = GapSide.All };
        rules ??= new();

        if ( rules.TryGetValue( gapSize, out var rule ) )
        {
            rule.Add( gapDefinition );
        }
        else
        {
            rules.Add( gapSize, new() { gapDefinition } );
        }

        currentGap = gapDefinition;
        Dirty();

        return this;
    }

    /// <summary>
    /// Appends the new side rule.
    /// </summary>
    /// <param name="side">Side to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentGapWithSideAndSize WithSide( GapSide side )
    {
        if ( cache.IsCached )
        {
            return cachedOperations.With( this, cache, static ( builder, value, _ ) => builder.WithSide( (GapSide)value ), value: (int)side );
        }

        currentGap.Side = side;
        Dirty();

        return this;
    }

    private IFluentGapWithSize WithSize( string value )
    {
        if ( cache.IsCached )
        {
            return cachedOperations.With( this, cache, static ( builder, _, data ) => builder.WithSize( (string)data ), data: value, retain: false );
        }

        if ( customRules is null )
        {
            customRules = new() { value };
        }
        else
        {
            customRules.Add( value );
        }

        Dirty();

        return this;
    }

    /// <summary>
    /// Used to add custom column rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    public IFluentGapWithSize Is( string value ) => WithSize( value );

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
    public IFluentGap OnSelf => WithUtilityTarget( Blazorise.UtilityTarget.Self );

    /// <summary>
    /// Targets the utility output to a wrapper element.
    /// </summary>
    public IFluentGap OnWrapper => WithUtilityTarget( Blazorise.UtilityTarget.Wrapper );

    /// <summary>
    /// For classes that eliminate the margin or padding by setting it to 0.
    /// </summary>
    public IFluentGapWithSideAndSize Is0 => WithSize( GapSize.Is0 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .25
    /// </summary>
    public IFluentGapWithSideAndSize Is1 => WithSize( GapSize.Is1 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .5
    /// </summary>
    public IFluentGapWithSideAndSize Is2 => WithSize( GapSize.Is2 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer
    /// </summary>
    public IFluentGapWithSideAndSize Is3 => WithSize( GapSize.Is3 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 1.5
    /// </summary>
    public IFluentGapWithSideAndSize Is4 => WithSize( GapSize.Is4 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 3
    /// </summary>
    public IFluentGapWithSideAndSize Is5 => WithSize( GapSize.Is5 );

    /// <summary>
    /// For classes that set both *-left and *-right.
    /// </summary>
    public IFluentGapWithSideAndSize OnX => WithSide( GapSide.X );

    /// <summary>
    /// For classes that set both *-top and *-bottom.
    /// </summary>
    public IFluentGapWithSideAndSize OnY => WithSide( GapSide.Y );

    /// <summary>
    /// For classes that set a margin or padding on all 4 sides of the element.
    /// </summary>
    public IFluentGapWithSideAndSize OnAll => WithSide( GapSide.All );

    #endregion
}

/// <summary>
/// Gap builder.
/// </summary>
public static class Gap
{
    private static readonly FluentGap cached = FluentGap.CreateCached();

    /// <summary>
    /// for classes that eliminate the margin by setting it to 0
    /// </summary>
    public static IFluentGapWithSideAndSize Is0 => cached.Is0;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * .25
    /// </summary>
    public static IFluentGapWithSideAndSize Is1 => cached.Is1;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * .5
    /// </summary>
    public static IFluentGapWithSideAndSize Is2 => cached.Is2;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer
    /// </summary>
    public static IFluentGapWithSideAndSize Is3 => cached.Is3;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * 1.5
    /// </summary>
    public static IFluentGapWithSideAndSize Is4 => cached.Is4;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * 3
    /// </summary>
    public static IFluentGapWithSideAndSize Is5 => cached.Is5;

    /// <summary>
    /// Defines the gap size in pixels.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentGap"/> reference.</returns>
    public static IFluentGap Px( double size ) => size.Px();

    /// <summary>
    /// Defines the gap size relative to the font-size of the root element.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentGap"/> reference.</returns>
    public static IFluentGap Rem( double size ) => size.Rem();

    /// <summary>
    /// Defines the gap size relative to the font-size of the element.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentGap"/> reference.</returns>
    public static IFluentGap Em( double size ) => size.Em();

    /// <summary>
    /// Defines the gap size relative to the advance measure of the glyph "0" in the element's font.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentGap"/> reference.</returns>
    public static IFluentGap Ch( double size ) => size.Ch();

    /// <summary>
    /// Defines the gap size relative to the viewport's width.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentGap"/> reference.</returns>
    public static IFluentGap Vw( double size ) => size.Vw();

    /// <summary>
    /// Defines the gap size relative to the viewport's height.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentGap"/> reference.</returns>
    public static IFluentGap Vh( double size ) => size.Vh();

    /// <summary>
    /// Defines the gap size as a percentage.
    /// </summary>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="IFluentGap"/> reference.</returns>
    public static IFluentGap Percent( double size ) => size.Percent();

    /// <summary>
    /// Add custom margin rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    public static IFluentGapWithSize Is( string value ) => cached.Is( value );
}